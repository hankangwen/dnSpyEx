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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace dnSpy.BamlDecompiler.Xaml {
	sealed class XamlExtension {
		public XamlType ExtensionType { get; }
		public object[] Initializer { get; set; }
		public IDictionary<string, object> NamedArguments { get; }

		public XamlExtension(XamlType type) {
			ExtensionType = type;
			NamedArguments = new Dictionary<string, object>();
		}

		static void WriteObject(StringBuilder sb, XamlContext ctx, XElement ctxElement, object value) {
			if (value is XamlExtension extension)
				sb.Append(extension.ToString(ctx, ctxElement));
			else if (value is XamlMemberName memberName)
				sb.Append(memberName.MemberName);
			else if (value is string str)
				sb.Append(EscapeOrEncapsulateInQuotes(str));
			else
				sb.Append(value);
		}

		public string ToString(XamlContext ctx, XElement ctxElement) {
			var sb = new StringBuilder();
			sb.Append('{');

			var typeName = ExtensionType.ToMarkupExtensionName(ctx, ctxElement);
			if (typeName.EndsWith("Extension", StringComparison.Ordinal))
				sb.Append(typeName.Substring(0, typeName.Length - 9));
			else
				sb.Append(typeName);

			bool comma = false;
			if (Initializer is not null && Initializer.Length > 0) {
				sb.Append(' ');
				for (int i = 0; i < Initializer.Length; i++) {
					if (comma)
						sb.Append(", ");
					WriteObject(sb, ctx, ctxElement, Initializer[i]);
					comma = true;
				}
			}

			if (NamedArguments.Count > 0) {
				foreach (var kvp in NamedArguments) {
					if (comma)
						sb.Append(", ");
					else {
						sb.Append(' ');
						comma = true;
					}
					sb.AppendFormat("{0}=", kvp.Key);
					WriteObject(sb, ctx, ctxElement, kvp.Value);
				}
			}

			sb.Append('}');
			return sb.ToString();
		}

		static string EscapeOrEncapsulateInQuotes(string value) {
			var escaped = EscapeAttributeValue(value);
			for (int i = 0; i < escaped.Length; i++) {
				char c = escaped[i];
				if (char.IsWhiteSpace(c) || c == '\'' || c == ',' || c == '=')
					return $"'{value.Replace("'", "\\'")}'";
			}
			return escaped;
		}

		static string EscapeAttributeValue(string value) {
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			StringBuilder stringBuilder = null;
			for (int i = 0; i < value.Length; i++) {
				char c = value[i];
				if (c == '"' || c == '<' || c == '>' || c == '&') {
					stringBuilder ??= new StringBuilder(value.Length + 8).Append(value, 0, i);
					stringBuilder.Append(EscapeChar(c));
				}
				else
					stringBuilder?.Append(c);
			}
			return stringBuilder is not null ? stringBuilder.ToString() : value;
		}

		static string EscapeChar(char c) => c switch {
			'"' => "&quot;",
			'&' => "&amp;",
			'\'' => "&apos;",
			'<' => "&lt;",
			'>' => "&gt;",
			_ => null
		};
	}
}
