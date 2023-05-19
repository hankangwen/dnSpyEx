/*
	Copyright (c) 2023 ElektroKill

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


using System.Collections.Generic;
using System.Text;
using System.Xml;
using dnlib.DotNet;

namespace dnSpy.BamlDecompiler.Xaml {
	public sealed class XamlTypeName {
		public string[] DeclaringTypeNames { get; }

		public string TypeName { get; }

		XamlTypeName(string[] declaringTypeNames, string typeName) {
			DeclaringTypeNames = declaringTypeNames;
			TypeName = typeName;
		}

		public static XamlTypeName From(ITypeDefOrRef type, out string clrNs) {
			var typeName = type.ReflectionName;

			var declaringTypes = new List<string>();
			while (type.DeclaringType is ITypeDefOrRef declaringType) {
				declaringTypes.Insert(0, declaringType.ReflectionName);
				type = declaringType;
			}

			clrNs = type.ReflectionNamespace;
			return new XamlTypeName(declaringTypes.ToArray(), typeName);
		}

		public string ToEncodedName() => AppendEncodedName(new StringBuilder()).ToString();

		public StringBuilder AppendEncodedName(StringBuilder sb) {
			for (var i = 0; i < DeclaringTypeNames.Length; i++) {
				sb.Append(XmlConvert.EncodeLocalName(DeclaringTypeNames[i]));
				sb.Append('+');
			}
			sb.Append(XmlConvert.EncodeLocalName(TypeName));
			return sb;
		}

		public static implicit operator string(XamlTypeName typeName) => typeName.ToString();

		public override string ToString() {
			var sb = new StringBuilder();
			for (var i = 0; i < DeclaringTypeNames.Length; i++) {
				sb.Append(DeclaringTypeNames[i]);
				sb.Append('+');
			}
			sb.Append(TypeName);
			return sb.ToString();
		}
	}
}
