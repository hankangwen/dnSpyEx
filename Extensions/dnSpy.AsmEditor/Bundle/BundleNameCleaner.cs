using System.Text;
using System;
using System.IO;
using System.Collections.Generic;

namespace dnSpy.AsmEditor.Bundle {
	/// <summary>
	/// Cleans bundle entry name
	/// </summary>
	public static class BundleNameCleaner {
		static readonly HashSet<char> invalidFileNameChar = new HashSet<char>();
		static BundleNameCleaner() {
			foreach (var c in Path.GetInvalidFileNameChars())
				invalidFileNameChar.Add(c);
			foreach (var c in Path.GetInvalidPathChars())
				invalidFileNameChar.Add(c);
		}

		public static string GetCleanedPath(string s, bool useSubDirs) {
			if (!useSubDirs)
				return FixFileNamePart(GetFileName(s));

			string res = string.Empty;
			foreach (var part in s.Replace('/', '\\').Split('\\'))
				res = Path.Combine(res, FixFileNamePart(part));
			return res;
		}

		public static string GetFileName(string s) {
			int index = Math.Max(s.LastIndexOf('/'), s.LastIndexOf('\\'));
			if (index < 0)
				return s;
			return s.Substring(index + 1);
		}

		public static string FixFileNamePart(string s) {
			var sb = new StringBuilder(s.Length);

			foreach (var c in s) {
				if (invalidFileNameChar.Contains(c))
					sb.Append('_');
				else
					sb.Append(c);
			}

			return sb.ToString();
		}
	}
}
