using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace dnSpy.MainApp {
	static class UpdateChecker {
		internal readonly struct UpdateCheckInfo {
			public readonly bool Success;
			public readonly bool UpdateAvailable;
			public readonly VersionInfo VersionInfo;

			public UpdateCheckInfo(bool updateAvailable, VersionInfo versionInfo) {
				Success = true;
				UpdateAvailable = updateAvailable;
				VersionInfo = versionInfo;
			}
		}

		internal readonly struct VersionInfo {
			public readonly Version Version;
			public readonly string DownloadUrl;

			public VersionInfo(Version version, string url) {
				Version = version;
				DownloadUrl = url;
			}
		}

		static readonly Version currentVersion = GetCurrentVersion();

		static Version GetCurrentVersion() {
			var currentAsm = typeof(StartUpClass).Assembly;
			try {
				var fileVer = FileVersionInfo.GetVersionInfo(currentAsm.Location).FileVersion;
				if (!string2.IsNullOrEmpty(fileVer))
					return new Version(fileVer);
			}
			catch {
			}
			return currentAsm.GetName().Version!;
		}

		public static async Task<UpdateCheckInfo> CheckForUpdatesAsync() {
			var updateInfo = await TryGetLatestVersionAsync();
			if (updateInfo is not null) {
				if (updateInfo.Value.Version > currentVersion)
					return new UpdateCheckInfo(true, updateInfo.Value);
				return new UpdateCheckInfo(false, default);
			}

			return default;
		}

		static async Task<VersionInfo?> TryGetLatestVersionAsync() {
			try {
				string result;
				using (var client = new HttpClient(new HttpClientHandler { UseProxy = true, UseDefaultCredentials = true })) {
					client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("dnSpyEx", currentVersion.ToString()));

					result = await client.GetStringAsync("https://api.github.com/repos/dnSpyEx/dnSpy/releases/latest");
				}

				var json = JObject.Parse(result);
				if (!json.TryGetValue("tag_name", out var tagToken) || tagToken is not JValue tagValue || tagValue.Value is not string tagName)
					return null;
				if (!json.TryGetValue("html_url", out var urlToken) || urlToken is not JValue urlValue || urlValue.Value is not string htmlUrl)
					return null;
				if (tagName[0] == 'v')
					tagName = tagName.Remove(0, 1);
				if (Version.TryParse(tagName, out var version))
					return new VersionInfo(version, htmlUrl);
			}
			catch {
			}
			return null;
		}
	}
}
