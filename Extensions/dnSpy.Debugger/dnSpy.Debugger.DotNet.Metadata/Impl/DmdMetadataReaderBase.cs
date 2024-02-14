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

namespace dnSpy.Debugger.DotNet.Metadata.Impl {
	abstract class DmdMetadataReaderBase : DmdMetadataReader {
		protected const bool resolveTypes = true;

		static DmdMemberInfo TryResolve(DmdMemberInfo member, DmdResolveOptions options) => (options & DmdResolveOptions.NoTryResolveRefs) != 0 ? member : member.ResolveMemberNoThrow() ?? member;
		static DmdType TryResolve(DmdType member, DmdResolveOptions options) => (options & DmdResolveOptions.NoTryResolveRefs) != 0 ? member : member.ResolveNoThrow() ?? member;
		static DmdFieldInfo TryResolve(DmdFieldInfo member, DmdResolveOptions options) => (options & DmdResolveOptions.NoTryResolveRefs) != 0 ? member : member.ResolveNoThrow() ?? member;
		static DmdMethodBase TryResolve(DmdMethodBase member, DmdResolveOptions options) => (options & DmdResolveOptions.NoTryResolveRefs) != 0 ? member : member.ResolveMethodBaseNoThrow() ?? member;

		public sealed override DmdMethodBase? ResolveMethod(int metadataToken, IList<DmdType>? genericTypeArguments, IList<DmdType>? genericMethodArguments, DmdResolveOptions options) {
			uint rid = MDToken.ToRID(metadataToken);
			switch (MDToken.ToTable(metadataToken)) {
			case Table.Method:
				var method = ResolveMethodDef(rid);
				if (method is not null)
					return method;
				break;

			case Table.MemberRef:
				var mr = ResolveMemberRef(rid, genericTypeArguments, genericMethodArguments);
				if (mr is not null) {
					if (mr is DmdMethodBase methodRef)
						return TryResolve(methodRef, options);
					if ((options & DmdResolveOptions.ThrowOnError) != 0)
						throw new ArgumentException();
				}
				break;

			case Table.MethodSpec:
				var methodSpec = ResolveMethodSpec(rid, genericTypeArguments, genericMethodArguments);
				if (methodSpec is not null)
					return TryResolve(methodSpec, options);
				break;
			}

			if ((options & DmdResolveOptions.ThrowOnError) != 0)
				throw new ArgumentOutOfRangeException(nameof(metadataToken));
			return null;
		}

		public sealed override DmdFieldInfo? ResolveField(int metadataToken, IList<DmdType>? genericTypeArguments, IList<DmdType>? genericMethodArguments, DmdResolveOptions options) {
			uint rid = MDToken.ToRID(metadataToken);
			switch (MDToken.ToTable(metadataToken)) {
			case Table.Field:
				var field = ResolveFieldDef(rid);
				if (field is not null)
					return field;
				break;

			case Table.MemberRef:
				var memberRef = ResolveMemberRef(rid, genericTypeArguments, genericMethodArguments);
				if (memberRef is not null) {
					if (memberRef is DmdFieldInfo fieldRef)
						return TryResolve(fieldRef, options);
					if ((options & DmdResolveOptions.ThrowOnError) != 0)
						throw new ArgumentException();
				}
				break;
			}

			if ((options & DmdResolveOptions.ThrowOnError) != 0)
				throw new ArgumentOutOfRangeException(nameof(metadataToken));
			return null;
		}

		public sealed override DmdType? ResolveType(int metadataToken, IList<DmdType>? genericTypeArguments, IList<DmdType>? genericMethodArguments, DmdResolveOptions options) {
			uint rid = MDToken.ToRID(metadataToken);
			switch (MDToken.ToTable(metadataToken)) {
			case Table.TypeRef:
				var typeRef = ResolveTypeRef(rid);
				if (typeRef is not null)
					return TryResolve(typeRef, options);
				break;

			case Table.TypeDef:
				var typeDef = ResolveTypeDef(rid);
				if (typeDef is not null)
					return typeDef;
				break;

			case Table.TypeSpec:
				var typeSpec = ResolveTypeSpec(rid, genericTypeArguments, genericMethodArguments);
				if (typeSpec is not null)
					return TryResolve(typeSpec, options);
				break;

			case Table.ExportedType:
				var exportedType = ResolveExportedType(rid);
				if (exportedType is not null)
					return exportedType;// Don't try to resolve it, callers want the actual reference
				break;
			}

			if ((options & DmdResolveOptions.ThrowOnError) != 0)
				throw new ArgumentOutOfRangeException(nameof(metadataToken));
			return null;
		}

		public sealed override DmdMemberInfo? ResolveMember(int metadataToken, IList<DmdType>? genericTypeArguments, IList<DmdType>? genericMethodArguments, DmdResolveOptions options) {
			uint rid = MDToken.ToRID(metadataToken);
			switch (MDToken.ToTable(metadataToken)) {
			case Table.TypeRef:
				var typeRef = ResolveTypeRef(rid);
				if (typeRef is not null)
					return TryResolve(typeRef, options);
				break;

			case Table.TypeDef:
				var typeDef = ResolveTypeDef(rid);
				if (typeDef is not null)
					return typeDef;
				break;

			case Table.Field:
				var field = ResolveFieldDef(rid);
				if (field is not null)
					return field;
				break;

			case Table.Method:
				var method = ResolveMethodDef(rid);
				if (method is not null)
					return method;
				break;

			case Table.MemberRef:
				var memberRef = ResolveMemberRef(rid, genericTypeArguments, genericMethodArguments);
				if (memberRef is not null)
					return TryResolve(memberRef, options);
				break;

			case Table.TypeSpec:
				var typeSpec = ResolveTypeSpec(rid, genericTypeArguments, genericMethodArguments);
				if (typeSpec is not null)
					return TryResolve(typeSpec, options);
				break;

			case Table.ExportedType:
				var exportedType = ResolveExportedType(rid);
				if (exportedType is not null)
					return exportedType;// Don't try to resolve it, callers want the actual reference
				break;

			case Table.MethodSpec:
				var methodSpec = ResolveMethodSpec(rid, genericTypeArguments, genericMethodArguments);
				if (methodSpec is not null)
					return TryResolve(methodSpec, options);
				break;
			}

			if ((options & DmdResolveOptions.ThrowOnError) != 0)
				throw new ArgumentOutOfRangeException(nameof(metadataToken));
			return null;
		}

		public sealed override DmdMethodSignature? ResolveMethodSignature(int metadataToken, IList<DmdType>? genericTypeArguments, IList<DmdType>? genericMethodArguments, DmdResolveOptions options) {
			uint rid = MDToken.ToRID(metadataToken);
			switch (MDToken.ToTable(metadataToken)) {
			case Table.StandAloneSig:
				var methodSig = ResolveMethodSignature(rid, genericTypeArguments, genericMethodArguments);
				if (methodSig is not null)
					return methodSig;
				break;
			}

			if ((options & DmdResolveOptions.ThrowOnError) != 0)
				throw new ArgumentOutOfRangeException(nameof(metadataToken));
			return null;
		}

		protected abstract DmdTypeRef? ResolveTypeRef(uint rid);
		protected abstract DmdTypeDef? ResolveTypeDef(uint rid);
		protected abstract DmdFieldDef? ResolveFieldDef(uint rid);
		protected abstract DmdMethodBase? ResolveMethodDef(uint rid);
		protected abstract DmdMemberInfo? ResolveMemberRef(uint rid, IList<DmdType>? genericTypeArguments, IList<DmdType>? genericMethodArguments);
		protected abstract DmdEventDef? ResolveEventDef(uint rid);
		protected abstract DmdPropertyDef? ResolvePropertyDef(uint rid);
		protected abstract DmdType? ResolveTypeSpec(uint rid, IList<DmdType>? genericTypeArguments, IList<DmdType>? genericMethodArguments);
		protected abstract DmdTypeRef? ResolveExportedType(uint rid);
		protected abstract DmdMethodBase? ResolveMethodSpec(uint rid, IList<DmdType>? genericTypeArguments, IList<DmdType>? genericMethodArguments);
		protected abstract DmdMethodSignature? ResolveMethodSignature(uint rid, IList<DmdType>? genericTypeArguments, IList<DmdType>? genericMethodArguments);

		public sealed override byte[]? ResolveSignature(int metadataToken) {
			byte[]? res;
			uint rid = MDToken.ToRID(metadataToken);
			switch (MDToken.ToTable(metadataToken)) {
			case Table.Field:			res = ResolveFieldSignature(rid); break;
			case Table.Method:			res = ResolveMethodSignature(rid); break;
			case Table.MemberRef:		res = ResolveMemberRefSignature(rid); break;
			case Table.StandAloneSig:	res = ResolveStandAloneSigSignature(rid); break;
			case Table.TypeSpec:		res = ResolveTypeSpecSignature(rid); break;
			case Table.MethodSpec:		res = ResolveMethodSpecSignature(rid); break;
			default:					res = null; break;
			}
			return res ?? throw new ArgumentOutOfRangeException(nameof(metadataToken));
		}

		protected abstract byte[]? ResolveFieldSignature(uint rid);
		protected abstract byte[]? ResolveMethodSignature(uint rid);
		protected abstract byte[]? ResolveMemberRefSignature(uint rid);
		protected abstract byte[]? ResolveStandAloneSigSignature(uint rid);
		protected abstract byte[]? ResolveTypeSpecSignature(uint rid);
		protected abstract byte[]? ResolveMethodSpecSignature(uint rid);

		public sealed override string ResolveString(int metadataToken) {
			if (((uint)metadataToken >> 24) != 0x70)
				throw new ArgumentOutOfRangeException(nameof(metadataToken));
			uint offset = (uint)metadataToken & 0x00FFFFFF;
			if (offset == 0)
				return string.Empty;
			return ResolveStringCore(offset) ?? throw new ArgumentOutOfRangeException(nameof(offset));
		}

		protected abstract string ResolveStringCore(uint offset);

		public sealed override DmdCustomAttributeData[] ReadCustomAttributes(int metadataToken) {
			uint rid = MDToken.ToRID(metadataToken);
			switch (MDToken.ToTable(metadataToken)) {
			case Table.Module:		return ReadModuleCustomAttributes(rid);
			case Table.TypeDef:		return ReadTypeDefCustomAttributes(rid);
			case Table.Field:		return ReadFieldCustomAttributes(rid);
			case Table.Method:		return ReadMethodCustomAttributes(rid);
			case Table.Param:		return ReadParamCustomAttributes(rid);
			case Table.Event:		return ReadEventCustomAttributes(rid);
			case Table.Property:	return ReadPropertyCustomAttributes(rid);
			case Table.Assembly:	return ReadAssemblyCustomAttributes(rid);
			default: throw new ArgumentOutOfRangeException(nameof(metadataToken));
			}
		}

		protected abstract DmdCustomAttributeData[] ReadAssemblyCustomAttributes(uint rid);
		protected abstract DmdCustomAttributeData[] ReadModuleCustomAttributes(uint rid);
		protected abstract DmdCustomAttributeData[] ReadTypeDefCustomAttributes(uint rid);
		protected abstract DmdCustomAttributeData[] ReadFieldCustomAttributes(uint rid);
		protected abstract DmdCustomAttributeData[] ReadMethodCustomAttributes(uint rid);
		protected abstract DmdCustomAttributeData[] ReadParamCustomAttributes(uint rid);
		protected abstract DmdCustomAttributeData[] ReadEventCustomAttributes(uint rid);
		protected abstract DmdCustomAttributeData[] ReadPropertyCustomAttributes(uint rid);

		public sealed override DmdCustomAttributeData[] ReadSecurityAttributes(int metadataToken) {
			uint rid = MDToken.ToRID(metadataToken);
			switch (MDToken.ToTable(metadataToken)) {
			case Table.TypeDef:		return ReadTypeDefSecurityAttributes(rid);
			case Table.Method:		return ReadMethodSecurityAttributes(rid);
			case Table.Assembly:	return ReadAssemblySecurityAttributes(rid);
			default: throw new ArgumentOutOfRangeException(nameof(metadataToken));
			}
		}

		protected abstract DmdCustomAttributeData[] ReadAssemblySecurityAttributes(uint rid);
		protected abstract DmdCustomAttributeData[] ReadTypeDefSecurityAttributes(uint rid);
		protected abstract DmdCustomAttributeData[] ReadMethodSecurityAttributes(uint rid);

		public override event EventHandler<DmdTypesUpdatedEventArgs>? TypesUpdated { add { } remove { } }
	}
}
