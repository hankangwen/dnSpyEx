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
using dnlib.DotNet.MD;

namespace dnSpy.Debugger.DotNet.Metadata.Impl.COMD {
	sealed class DmdExportedTypeCOMD : DmdTypeRef {
		public override DmdTypeScope TypeScope { get; }
		public override string? MetadataNamespace { get; }
		public override string? MetadataName { get; }

		readonly DmdComMetadataReader reader;
		readonly int baseTypeToken;

		public DmdExportedTypeCOMD(DmdComMetadataReader reader, uint rid, IList<DmdCustomModifier>? customModifiers) : base(reader.Module, rid, customModifiers) {
			this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
			reader.Dispatcher.VerifyAccess();

			uint token = new MDToken(Table.ExportedType, rid).Raw;
			DmdTypeUtilities.SplitFullName(MDAPI.GetExportedTypeName(reader.MetaDataAssemblyImport, token) ?? string.Empty, out var @namespace, out var name);
			MetadataNamespace = @namespace;
			MetadataName = name;

			MDAPI.GetExportedTypeProps(reader.MetaDataAssemblyImport, token, out var implTokenRaw, out _, out _);
			var implToken = new MDToken(implTokenRaw);
			switch (implToken.Table) {
			case Table.AssemblyRef:
				TypeScope = new DmdTypeScope(reader.ReadAssemblyName_COMThread(implToken.Rid));
				break;

			case Table.File:
				var moduleName = MDAPI.GetFileName(reader.MetaDataAssemblyImport, implToken.Raw) ?? string.Empty;
				TypeScope = new DmdTypeScope(reader.GetName(), moduleName);
				break;

			case Table.ExportedType:
				TypeScope = DmdTypeScope.Invalid;
				baseTypeToken = implToken.ToInt32();
				break;

			default:
				TypeScope = DmdTypeScope.Invalid;
				break;
			}
		}

		T COMThread<T>(Func<T> action) => reader.Dispatcher.Invoke(action);

		protected override int GetDeclaringTypeRefToken() => baseTypeToken;

		public override DmdType WithCustomModifiers(IList<DmdCustomModifier>? customModifiers) {
			VerifyCustomModifiers(customModifiers);
			// Don't intern exported type refs
			return COMThread(() => new DmdExportedTypeCOMD(reader, Rid, customModifiers));
		}

		// Don't intern exported type refs
		public override DmdType WithoutCustomModifiers() => GetCustomModifiers().Count == 0 ? this : COMThread(() => new DmdExportedTypeCOMD(reader, Rid, null));
	}
}
