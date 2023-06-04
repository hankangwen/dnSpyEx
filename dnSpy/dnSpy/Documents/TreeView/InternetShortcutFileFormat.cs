using System.Runtime.InteropServices;
using System.Text;

namespace dnSpy.Documents.TreeView;

// Helper class to interact with Microsoft' InternetShortcut file format used in .url files
static class InternetShortcutFileFormat {
	internal static string? GetUrlFromInternetShortcutFile(string filename) {
		var urlValue = new StringBuilder(255);
		int charsCopied = GetPrivateProfileString("InternetShortcut", "URL", "", urlValue, 255, filename);
		return charsCopied < 1 ? null : urlValue.ToString();
	}

	[DllImport("kernel32", CharSet = CharSet.Unicode)]
	static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
}
