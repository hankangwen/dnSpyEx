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

using dnlib.DotNet;
using dnSpy.BamlDecompiler.Baml;
using dnSpy.Contracts.Documents.Tabs.DocViewer.ToolTips;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.Text;

namespace dnSpy.BamlDecompiler {
	[ExportDocumentViewerToolTipProvider]
	sealed class BamlDocumentViewerToolTipProvider : IDocumentViewerToolTipProvider {
		public object Create(IDocumentViewerToolTipProviderContext context, object @ref) {
			if (@ref is BamlStringReference bref) {
				var provider = context.Create();
				provider.Image = DsImages.String;
				provider.Output.Write(BoxedTextColor.String, bref.String);
				return provider.Create();
			}
			if (@ref is BamlAssemblyReference aref) {
				var provider = context.Create();
				provider.Image = DsImages.Assembly;
				provider.Output.Write(aref.Assembly);
				return provider.Create();
			}
			if (@ref is BamlRecordReference recRef) {
				var provider = context.Create();
				provider.Output.Write(BoxedTextColor.Keyword, recRef.Record.Type.ToString());
				provider.Output.WriteLine();
				provider.Output.Write(BoxedTextColor.Text, "Position");
				provider.Output.Write(BoxedTextColor.Punctuation, ":");
				provider.Output.Write(BoxedTextColor.Text, " ");
				provider.Output.Write(BoxedTextColor.Number, $"0x{recRef.Record.Position:x}");
				return provider.Create();
			}
			if (@ref is BamlResourceReference resRef) {
				var provider = context.Create();
				provider.Output.Write(BoxedTextColor.Type, resRef.TypeName);
				provider.Output.Write(BoxedTextColor.Punctuation, ".");
				if (resRef.KeyName is not null)
					provider.Output.Write(BoxedTextColor.Text, resRef.KeyName);
				else
					provider.Output.Write(BoxedTextColor.InstanceProperty, resRef.PropertyName);
				return provider.Create();
			}
			if (@ref is BamlTypeReference typeRef) {
				var provider = context.Create();
				provider.Image = DsImages.ClassPublic;
				if (!string.IsNullOrEmpty(typeRef.Namespace)) {
					var nsParts = typeRef.Namespace.Split('.');
					for (var i = 0; i < nsParts.Length; i++) {
						provider.Output.Write(BoxedTextColor.Namespace, nsParts[i]);
						provider.Output.Write(BoxedTextColor.Punctuation, ".");
					}
				}
				provider.Output.Write(BoxedTextColor.Type, typeRef.Name);
				return provider.Create();
			}
			if (@ref is BamlAttributeReference attrRef) {
				var provider = context.Create();
				provider.Image = DsImages.Property;
				if (!string.IsNullOrEmpty(attrRef.Namespace)) {
					var nsParts = attrRef.Namespace.Split('.');
					for (var i = 0; i < nsParts.Length; i++) {
						provider.Output.Write(BoxedTextColor.Namespace, nsParts[i]);
						provider.Output.Write(BoxedTextColor.Punctuation, ".");
					}
				}
				provider.Output.Write(BoxedTextColor.Type, attrRef.TypeName);
				provider.Output.Write(BoxedTextColor.Punctuation, ".");
				provider.Output.Write(BoxedTextColor.InstanceProperty, attrRef.MemberName);
				return provider.Create();
			}

			return null;
		}
	}

	// Don't use a string since it should only show tooltips if it's from the baml disassembler
	sealed class BamlStringReference {
		public static object Create(string s) => string.IsNullOrEmpty(s) ? null : new BamlStringReference(s);
		public string String { get; }

		BamlStringReference(string s) => String = s;
	}

	sealed class BamlRecordReference {
		public BamlRecord Record { get; }

		BamlRecordReference(BamlRecord record) => Record = record;

		public static object Create(BamlRecord record) {
			if (record is null)
				return null;
			return new BamlRecordReference(record);
		}
	}

	sealed class BamlAssemblyReference {
		public AssemblyNameInfo Assembly { get; }

		BamlAssemblyReference(AssemblyNameInfo assemblyName) => Assembly = assemblyName;

		public static object Create(string assemblyFullName) {
			if (string.IsNullOrEmpty(assemblyFullName))
				return null;
			return new BamlAssemblyReference(new AssemblyNameInfo(assemblyFullName));
		}
	}

	sealed class BamlResourceReference {
		public string TypeName { get; }

		public string KeyName { get; }

		public string PropertyName { get; }

		BamlResourceReference(string typeName, string keyName, string propertyName) {
			TypeName = typeName;
			KeyName = keyName;
			PropertyName = propertyName;
		}

		public static object CreateKey(string typeName, string keyName) {
			if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(keyName))
				return null;
			return new BamlResourceReference(typeName, keyName, null);
		}

		public static object CreateProperty(string typeName, string propertyName) {
			if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(propertyName))
				return null;
			return new BamlResourceReference(typeName, null, propertyName);
		}
	}

	sealed class BamlTypeReference {
		public string Namespace { get; }

		public string Name { get; }

		BamlTypeReference(string ns, string name) {
			Namespace = ns;
			Name = name;
		}

		public static object Create(string ns, string name) {
			if (ns is null || string.IsNullOrEmpty(name))
				return null;
			return new BamlTypeReference(ns, name);
		}
	}

	sealed class BamlAttributeReference {
		public string Namespace { get; }

		public string TypeName { get; }

		public string MemberName { get; }

		BamlAttributeReference(string ns, string typeName, string memberName) {
			Namespace = ns;
			TypeName = typeName;
			MemberName = memberName;
		}

		public static object Create(string ns, string typeName, string memberName) {
			if (ns is null || string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(memberName))
				return null;
			return new BamlAttributeReference(ns, typeName, memberName);
		}
	}
}
