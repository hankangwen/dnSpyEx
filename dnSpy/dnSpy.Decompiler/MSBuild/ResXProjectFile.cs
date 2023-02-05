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
using dnlib.DotNet;
using dnlib.DotNet.Resources;
using dnSpy.Decompiler.Properties;

namespace dnSpy.Decompiler.MSBuild {
	sealed class ResXProjectFile : ProjectFile {
		public override string Description => dnSpy_Decompiler_Resources.MSBuild_CreateResXFile;
		public override BuildAction BuildAction => BuildAction.EmbeddedResource;
		public override string Filename => filename;
		readonly string filename;

		public string TypeFullName { get; }
		public bool IsSatelliteFile { get; set; }

		readonly ResourceElementSet resourceElementSet;
		readonly Dictionary<IAssembly, IAssembly> newToOldAsm;

		public ResXProjectFile(ModuleDef module, string filename, string typeFullName, ResourceElementSet resourceElementSet) {
			this.filename = filename;
			TypeFullName = typeFullName;
			this.resourceElementSet = resourceElementSet;

			newToOldAsm = new Dictionary<IAssembly, IAssembly>(new AssemblyNameComparer(AssemblyNameComparerFlags.All & ~AssemblyNameComparerFlags.Version));
			foreach (var asmRef in module.GetAssemblyRefs())
				newToOldAsm[asmRef] = asmRef;
		}

		public override void Create(DecompileContext ctx) {
			using (var writer = new ResXResourceFileWriter(Filename, TypeNameConverter)) {
				foreach (var resourceElement in resourceElementSet.ResourceElements) {
					ctx.CancellationToken.ThrowIfCancellationRequested();
					writer.AddResourceData(resourceElement);
				}
			}
		}

		string TypeNameConverter(Type type) {
			var newAsm = new AssemblyNameInfo(type.Assembly.GetName());
			if (!newToOldAsm.TryGetValue(newAsm, out var oldAsm))
				return type.AssemblyQualifiedName ?? throw new ArgumentException();
			if (type.IsGenericType)
				return type.AssemblyQualifiedName ?? throw new ArgumentException();
			if (AssemblyNameComparer.CompareAll.Equals(oldAsm, newAsm))
				return type.AssemblyQualifiedName ?? throw new ArgumentException();
			return $"{type.FullName}, {oldAsm.FullName}";
		}
	}
}
