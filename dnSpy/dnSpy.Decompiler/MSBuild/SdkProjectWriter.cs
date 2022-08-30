/*
    Copyright (C) 2014-2019 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using dnSpy.Contracts.Decompiler;

namespace dnSpy.Decompiler.MSBuild {
	sealed class SdkProjectWriter : ProjectWriterBase {
		static readonly HashSet<string> implicitReferences = new HashSet<string> {
			"mscorlib",
			"netstandard",
			"PresentationFramework",
			"System",
			"System.Diagnostics.Debug",
			"System.Diagnostics.Tools",
			"System.Drawing",
			"System.Runtime",
			"System.Runtime.Extensions",
			"System.Windows.Forms",
			"System.Xaml",
		};

		enum ProjectType : byte {
			Default,
			WinForms,
			Wpf,
			Web
		}

		public SdkProjectWriter(Project project, ProjectVersion projectVersion, IList<Project> allProjects,
			IList<string> userGACPaths) : base(project, projectVersion, allProjects, userGACPaths) { }

		public override void Write() {
			project.OnWrite();
			var settings = new XmlWriterSettings {
				Encoding = Encoding.UTF8,
				Indent = true,
			};
			using (var writer = XmlWriter.Create(project.Filename, settings)) {
				project.Platform = GetPlatformString();

				writer.WriteStartElement("Project");

				var projectType = GetProjectType();
				writer.WriteAttributeString("Sdk", GetSdkString(projectType));

				writer.WriteStartElement("PropertyGroup");

				writer.WriteElementString("ProjectGuid", project.Guid.ToString("B").ToUpperInvariant());
				writer.WriteElementString("OutputType", GetOutputType());
				writer.WriteElementString("RootNamespace", GetRootNamespace());
				var asmName = GetAssemblyName();
				if (!string.IsNullOrEmpty(asmName))
					writer.WriteElementString("AssemblyName", GetAssemblyName());
				writer.WriteElementString("GenerateAssemblyInfo", "False");

				writer.WriteElementString("FileAlignment", GetFileAlignment());

				var moniker = TargetFrameworkInfo.Create(project.Module).GetTargetFrameworkMoniker();
				if (moniker is null)
					throw new NotSupportedException("This assembly cannot be decompiled to a SDK style project.");

				writer.WriteElementString("TargetFramework", moniker);

				if (projectType == ProjectType.Wpf)
					writer.WriteElementString("UseWPF", "True");
				else if (projectType == ProjectType.WinForms)
					writer.WriteElementString("UseWindowsForms", "True");

				if (project.Platform != "AnyCPU")
					writer.WriteElementString("PlatformTarget", project.Platform);
				else if (project.Module.Is32BitPreferred)
					writer.WriteElementString("Prefer32Bit", "True");

				writer.WriteEndElement();

				writer.WriteStartElement("PropertyGroup");
				if (project.Options.DontReferenceStdLib)
					writer.WriteElementString("NoStdLib", "True");
				if (project.AllowUnsafeBlocks)
					writer.WriteElementString("AllowUnsafeBlocks", "True");
				writer.WriteElementString("EnableDefaultItems", "False");
				writer.WriteEndElement();

				writer.WriteStartElement("PropertyGroup");
				if (project.ApplicationManifest is not null)
					writer.WriteElementString("ApplicationManifest", GetRelativePath(project.ApplicationManifest.Filename));
				if (project.ApplicationIcon is not null)
					writer.WriteElementString("ApplicationIcon", GetRelativePath(project.ApplicationIcon.Filename));
				if (project.StartupObject is not null)
					writer.WriteElementString("StartupObject", project.StartupObject);
				writer.WriteEndElement();

				Write(writer, BuildAction.Compile);
				Write(writer, BuildAction.EmbeddedResource);

				// Project references
				var projRefs = project.Module.GetAssemblyRefs().
					Select(a => project.Module.Context.AssemblyResolver.Resolve(a, project.Module)).
					Select(a => a is null ? null : FindOtherProject(a.ManifestModule.Location)).
					OfType<Project>().OrderBy(a => a.Filename, StringComparer.OrdinalIgnoreCase).ToArray();
				if (projRefs.Length > 0) {
					writer.WriteStartElement("ItemGroup");
					foreach (var otherProj in projRefs) {
						writer.WriteStartElement("ProjectReference");
						writer.WriteAttributeString("Include", GetRelativePath(otherProj.Filename));
						writer.WriteStartElement("Project");
						var guidString = otherProj.Guid.ToString("B");
						if (projectVersion < ProjectVersion.VS2012)
							guidString = guidString.ToUpperInvariant();
						writer.WriteString(guidString);
						writer.WriteEndElement();
						writer.WriteStartElement("Name");
						writer.WriteString(IdentifierEscaper.Escape(otherProj.Module.Assembly is null ? string.Empty : otherProj.Module.Assembly.Name.String));
						writer.WriteEndElement();
						writer.WriteEndElement();
					}
					writer.WriteEndElement();
				}

				var gacRefs = project.Module.GetAssemblyRefs().Where(a => !implicitReferences.Contains(a.Name)).OrderBy(a => a.Name.String, StringComparer.OrdinalIgnoreCase).ToArray();
				var extraRefsWithoutImplicitRefs = project.ExtraAssemblyReferences.Where(a => !implicitReferences.Contains(a)).ToArray();
				if (gacRefs.Length > 0 || extraRefsWithoutImplicitRefs.Length > 0) {
					writer.WriteStartElement("ItemGroup");
					var hash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					foreach (var r in gacRefs) {
						var asm = project.Module.Context.AssemblyResolver.Resolve(r, project.Module);
						if (asm is not null && ExistsInProject(asm.ManifestModule.Location))
							continue;
						hash.Add(r.Name);
						writer.WriteStartElement("Reference");
						writer.WriteAttributeString("Include", IdentifierEscaper.Escape(r.Name));
						var hintPath = GetHintPath(asm);
						if (hintPath is not null)
							writer.WriteElementString("HintPath", hintPath);
						writer.WriteEndElement();
					}
					foreach (var r in extraRefsWithoutImplicitRefs) {
						if (hash.Contains(r) || AssemblyExistsInProject(r))
							continue;
						hash.Add(r);
						writer.WriteStartElement("Reference");
						writer.WriteAttributeString("Include", IdentifierEscaper.Escape(r));
						writer.WriteEndElement();
					}

					writer.WriteEndElement();
				}

				Write(writer, BuildAction.None);
				Write(writer, BuildAction.ApplicationDefinition);
				Write(writer, BuildAction.Page);
				Write(writer, BuildAction.Resource);
				Write(writer, BuildAction.SplashScreen);

				writer.WriteEndElement();
			}
		}

		void Write(XmlWriter writer, BuildAction buildAction) {
			var files = project.Files.Where(a => a.BuildAction == buildAction).OrderBy(a => a.Filename, StringComparer.OrdinalIgnoreCase).ToArray();
			if (files.Length == 0)
				return;
			writer.WriteStartElement("ItemGroup");
			foreach (var file in files) {
				if (file.BuildAction == BuildAction.DontIncludeInProjectFile)
					continue;
				writer.WriteStartElement(ToString(buildAction));
				writer.WriteAttributeString("Include", GetRelativePath(file.Filename));
				if (file.DependentUpon is not null)
					writer.WriteElementString("DependentUpon", GetRelativePath(Path.GetDirectoryName(file.Filename)!, file.DependentUpon.Filename));
				if (file.SubType is not null)
					writer.WriteElementString("SubType", file.SubType);
				if (file.Generator is not null)
					writer.WriteElementString("Generator", file.Generator);
				if (file.LastGenOutput is not null)
					writer.WriteElementString("LastGenOutput", GetRelativePath(Path.GetDirectoryName(file.Filename)!, file.LastGenOutput.Filename));
				if (file.AutoGen)
					writer.WriteElementString("AutoGen", "True");
				if (file.DesignTime)
					writer.WriteElementString("DesignTime", "True");
				if (file.DesignTimeSharedInput)
					writer.WriteElementString("DesignTimeSharedInput", "True");
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		static string GetSdkString(ProjectType projectType) {
			switch (projectType) {
			case ProjectType.WinForms:
			case ProjectType.Wpf:
				return "Microsoft.NET.Sdk.WindowsDesktop";
			case ProjectType.Web:
				return "Microsoft.NET.Sdk.Web";
			default:
				return "Microsoft.NET.Sdk";
			}
		}

		ProjectType GetProjectType() {
			foreach (var referenceName in project.Module.GetAssemblyRefs().Select(r => r.Name)) {
				if (referenceName.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal))
					return ProjectType.Web;
				if (referenceName == "PresentationFramework")
					return ProjectType.Wpf;
				if (referenceName == "System.Windows.Forms")
					return ProjectType.WinForms;
			}

			return ProjectType.Default;
		}
	}
}
