/*
	Copyright (c) 2015 Ki

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/

using System.Text;
using System.Xml;
using System.Xml.Linq;
using dnlib.DotNet;
using dnSpy.Contracts.Decompiler;

namespace dnSpy.BamlDecompiler.Xaml {
	sealed class XamlProperty {
		public XamlType DeclaringType { get; }
		public string PropertyName { get; }

		public IMemberDef ResolvedMember { get; set; }

		public ITypeDefOrRef ResolvedMemberDeclaringType { get; set; }

		public XamlPropertyKind PropertyKind { get; set; }

		public XamlProperty(XamlType type, string name) {
			DeclaringType = type;
			PropertyName = name;
		}

		public void TryResolve() {
			if (ResolvedMember is not null) {
				ResolvedMemberDeclaringType ??= ResolvedMember.DeclaringType;
				return;
			}

			(ResolvedMember, ResolvedMemberDeclaringType) = FindProperty(DeclaringType.ResolvedType, PropertyName);
			if (ResolvedMember is not null)
				return;

			(ResolvedMember, ResolvedMemberDeclaringType) = FindField(DeclaringType.ResolvedType, PropertyName + "Property");
			if (ResolvedMember is not null)
				return;

			(ResolvedMember, ResolvedMemberDeclaringType) = FindEvent(DeclaringType.ResolvedType, PropertyName);
			if (ResolvedMember is not null)
				return;

			(ResolvedMember, ResolvedMemberDeclaringType) = FindField(DeclaringType.ResolvedType, PropertyName + "Event");
		}

		static (PropertyDef, ITypeDefOrRef) FindProperty(ITypeDefOrRef declType, string name) {
			while (declType is not null) {
				var td = declType.ResolveTypeDef();
				if (td is null)
					break;

				var pd = td.FindProperty(name);
				if (pd is not null)
					return (pd, declType);

				declType = GetInstantiatedBaseType(declType);
			}

			return default;
		}

		static (EventDef, ITypeDefOrRef) FindEvent(ITypeDefOrRef declType, string name) {
			while (declType is not null) {
				var td = declType.ResolveTypeDef();
				if (td is null)
					break;

				var ed = td.FindEvent(name);
				if (ed is not null)
					return (ed, declType);

				declType = GetInstantiatedBaseType(declType);
			}

			return default;
		}

		static (FieldDef, ITypeDefOrRef) FindField(ITypeDefOrRef declType, string name) {
			while (declType is not null) {
				var td = declType.ResolveTypeDef();
				if (td is null)
					break;

				var fd = td.FindField(name);
				if (fd is not null)
					return (fd, declType);

				declType = GetInstantiatedBaseType(declType);
			}

			return default;
		}

		public bool IsAttachedTo(XamlType type) {
			if (type is null || ResolvedMember is null || type.ResolvedType is null)
				return true;

			var declType = ResolvedMemberDeclaringType;
			var t = type.ResolvedType;
			var comparer = new SigComparer();
			do {
				if (comparer.Equals(t, declType))
					return false;
				t = GetInstantiatedBaseType(t);
			} while (t is not null);
			return true;
		}

		static ITypeDefOrRef GetInstantiatedBaseType(ITypeDefOrRef type) {
			var baseType = type.GetBaseType();
			if (type is TypeSpec typeSpec && typeSpec.TypeSig is GenericInstSig genericInstSig && baseType is TypeSpec baseTypeSpec)
				baseType = GenericArgumentResolver.Resolve(baseTypeSpec.TypeSig, genericInstSig.GenericArguments, null).ToTypeDefOrRef();
			return baseType;
		}

		public XName ToXName(XamlContext ctx, XElement parent, bool isFullName = true) {
			var typeName = DeclaringType.ToXName(ctx);
			XName name;
			if (!isFullName)
				name = XmlConvert.EncodeLocalName(PropertyName);
			else {
				name = typeName.LocalName + "." + XmlConvert.EncodeLocalName(PropertyName);
				if (parent == null || parent.GetDefaultNamespace() != typeName.Namespace)
					name = typeName.Namespace + name.LocalName;
			}
			return name;
		}

		public string ToMarkupExtensionName(XamlContext ctx, XElement parent, bool isFullName = true) {
			if (!isFullName)
				return XmlConvert.EncodeLocalName(PropertyName);

			var sb = new StringBuilder();
			if (DeclaringType.Namespace != parent.GetDefaultNamespace()) {
				var prefix = parent.GetPrefixOfNamespace(DeclaringType.Namespace);
				if (!string.IsNullOrEmpty(prefix)) {
					sb.Append(prefix);
					sb.Append(':');
				}
			}

			DeclaringType.TypeName.AppendEncodedName(sb);
			sb.Append('.');
			sb.Append(XmlConvert.EncodeLocalName(PropertyName));
			return sb.ToString();
		}

		public override string ToString() => PropertyName;
	}
}
