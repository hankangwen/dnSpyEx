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
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dnSpy.Contracts.Decompiler;

namespace dnSpy.BamlDecompiler {
	readonly struct XamlOutputCreator {
		readonly XamlOutputOptions options;

		public XamlOutputCreator(XamlOutputOptions options) => this.options = options ?? throw new ArgumentNullException(nameof(options));

		public string CreateText(XDocument document) {
			if (options is null)
				throw new InvalidOperationException();
			if (document is null)
				throw new ArgumentNullException(nameof(document));

			var settings = new XmlWriterSettings {
				Indent = true,
				IndentChars = options.IndentChars ?? "\t",
				NewLineChars = options.NewLineChars ?? Environment.NewLine,
				NewLineOnAttributes = options.NewLineOnAttributes,
				OmitXmlDeclaration = true,
			};
			using (var writer = new StringWriter(CultureInfo.InvariantCulture)) {
				using (var xmlWriter = new XmlWriterWithMarkupExtensionSupport(XmlWriter.Create(writer, settings)))
					document.WriteTo(xmlWriter);
				// WriteTo() doesn't add a final newline
				writer.WriteLine();
				return writer.ToString();
			}
		}
	}

	sealed class XmlWriterWithMarkupExtensionSupport : XmlWriter {
		readonly XmlWriter writer;
		bool inAttribute;

		public XmlWriterWithMarkupExtensionSupport(XmlWriter baseWriter) => writer = baseWriter ?? throw new ArgumentNullException(nameof(baseWriter));

		public override void WriteStartAttribute(string prefix, string localName, string ns) {
			inAttribute = true;
			writer.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteEndAttribute() {
			inAttribute = false;
			writer.WriteEndAttribute();
		}

		public override void WriteString(string text) {
			if (inAttribute && text.Length > 3 && text[0] == '{' && text[1] != '}' && text[text.Length - 1] == '}')
				writer.WriteRaw(text);
			else
				writer.WriteString(text);
		}

		protected override void Dispose(bool disposing) {
			if (!disposing)
				return;
			writer.Dispose();
		}

		public override XmlWriterSettings Settings => writer.Settings;
		public override WriteState WriteState => writer.WriteState;
		public override XmlSpace XmlSpace => writer.XmlSpace;
		public override string XmlLang => writer.XmlLang;
		public override void WriteStartDocument() => writer.WriteStartDocument();
		public override void WriteStartDocument(bool standalone) => writer.WriteStartDocument(standalone);
		public override void WriteEndDocument() => writer.WriteEndDocument();
		public override void WriteDocType(string name, string pubid, string sysid, string subset) => writer.WriteDocType(name, pubid, sysid, subset);
		public override void WriteStartElement(string prefix, string localName, string ns) => writer.WriteStartElement(prefix, localName, ns);
		public override void WriteEndElement() => writer.WriteEndElement();
		public override void WriteFullEndElement() => writer.WriteFullEndElement();
		public override void WriteCData(string text) => writer.WriteCData(text);
		public override void WriteComment(string text) => writer.WriteComment(text);
		public override void WriteProcessingInstruction(string name, string text) => writer.WriteProcessingInstruction(name, text);
		public override void WriteEntityRef(string name) => writer.WriteEntityRef(name);
		public override void WriteCharEntity(char ch) => writer.WriteCharEntity(ch);
		public override void WriteWhitespace(string ws) => writer.WriteWhitespace(ws);
		public override void WriteSurrogateCharEntity(char lowChar, char highChar) => writer.WriteSurrogateCharEntity(lowChar, highChar);
		public override void WriteChars(char[] buffer, int index, int count) => writer.WriteChars(buffer, index, count);
		public override void WriteRaw(char[] buffer, int index, int count) => writer.WriteRaw(buffer, index, count);
		public override void WriteRaw(string data) => writer.WriteRaw(data);
		public override void WriteBase64(byte[] buffer, int index, int count) => writer.WriteBase64(buffer, index, count);
		public override void Close() => writer.Close();
		public override void Flush() => writer.Flush();
		public override string LookupPrefix(string ns) => writer.LookupPrefix(ns);
		public override void WriteValue(object value) => writer.WriteValue(value);
		public override void WriteValue(string value) => writer.WriteValue(value);
		public override void WriteValue(bool value) => writer.WriteValue(value);
		public override void WriteValue(DateTime value) => writer.WriteValue(value);
		public override void WriteValue(DateTimeOffset value) => writer.WriteValue(value);
		public override void WriteValue(double value) => writer.WriteValue(value);
		public override void WriteValue(float value) => writer.WriteValue(value);
		public override void WriteValue(Decimal value) => writer.WriteValue(value);
		public override void WriteValue(int value) => writer.WriteValue(value);
		public override void WriteValue(long value) => writer.WriteValue(value);
		public override Task WriteStartDocumentAsync() => writer.WriteStartDocumentAsync();
		public override Task WriteStartDocumentAsync(bool standalone) => writer.WriteStartDocumentAsync(standalone);
		public override Task WriteEndDocumentAsync() => writer.WriteEndDocumentAsync();
		public override Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset) => writer.WriteDocTypeAsync(name, pubid, sysid, subset);
		public override Task WriteStartElementAsync(string prefix, string localName, string ns) => writer.WriteStartElementAsync(prefix, localName, ns);
		public override Task WriteEndElementAsync() => writer.WriteEndElementAsync();
		public override Task WriteFullEndElementAsync() => writer.WriteFullEndElementAsync();
		public override Task WriteCDataAsync(string text) => writer.WriteCDataAsync(text);
		public override Task WriteCommentAsync(string text) => writer.WriteCommentAsync(text);
		public override Task WriteProcessingInstructionAsync(string name, string text) => writer.WriteProcessingInstructionAsync(name, text);
		public override Task WriteEntityRefAsync(string name) => writer.WriteEntityRefAsync(name);
		public override Task WriteCharEntityAsync(char ch) => writer.WriteCharEntityAsync(ch);
		public override Task WriteWhitespaceAsync(string ws) => writer.WriteWhitespaceAsync(ws);
		public override Task WriteStringAsync(string text) => writer.WriteStringAsync(text);
		public override Task WriteSurrogateCharEntityAsync(char lowChar, char highChar) => writer.WriteSurrogateCharEntityAsync(lowChar, highChar);
		public override Task WriteCharsAsync(char[] buffer, int index, int count) => writer.WriteCharsAsync(buffer, index, count);
		public override Task WriteRawAsync(char[] buffer, int index, int count) => writer.WriteRawAsync(buffer, index, count);
		public override Task WriteRawAsync(string data) => writer.WriteRawAsync(data);
		public override Task WriteBase64Async(byte[] buffer, int index, int count) => writer.WriteBase64Async(buffer, index, count);
		public override Task FlushAsync() => writer.FlushAsync();
	}
}
