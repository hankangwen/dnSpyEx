/*
    Copyright (C) 2023 ElektroKill

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

using System.ComponentModel.Composition;
using System.Threading.Tasks;
using dnSpy.Contracts.App;
using dnSpy.Contracts.Extension;
using dnSpy.Properties;
using dnSpy.UI;

namespace dnSpy.MainApp {
	[ExportAutoLoaded]
	sealed class StartupUpdateCheck : IAutoLoaded {
		readonly IUpdateService updateService;
		readonly IMessageBoxService messageBoxService;
		readonly UIDispatcher uiDispatcher;
		readonly IAppWindow appWindow;

		[ImportingConstructor]
		public StartupUpdateCheck(IUpdateService updateService, IMessageBoxService messageBoxService, IAppWindow appWindow, UIDispatcher uiDispatcher) {
			this.updateService = updateService;
			this.messageBoxService = messageBoxService;
			this.uiDispatcher = uiDispatcher;
			this.appWindow = appWindow;

			if (updateService.CheckForUpdatesOnStartup)
				Task.Run(CheckForUpdate);
		}

		async void CheckForUpdate() {
			var updateInfo = await updateService.CheckForUpdatesAsync();

			if (!updateInfo.Success)
				return;
			if (!updateInfo.UpdateAvailable || updateService.IsUpdateIgnored(updateInfo.VersionInfo))
				return;

			string message = string.Format(dnSpy_Resources.InfoBar_NewUpdateAvailable, updateInfo.VersionInfo.Version);
			DisplayNotification(message, InfoBarIcon.Information, new[] {
				new InfoBarInteraction(dnSpy_Resources.InfoBar_OpenDownloadPage, ctx => {
					AboutHelpers.OpenWebPage(updateInfo.VersionInfo.DownloadUrl, messageBoxService);
					ctx.CloseElement();
				}),
				new InfoBarInteraction(dnSpy_Resources.InfoBar_IgnoreThisUpdate, ctx => {
					updateService.MarkUpdateAsIgnored(updateInfo.VersionInfo);
					ctx.CloseElement();
				})
			});
		}

		void DisplayNotification(string message, InfoBarIcon icon, InfoBarInteraction[] interactions) {
			if (uiDispatcher.CheckAccess()) {
				appWindow.InfoBar.Show(message, icon, interactions);
				return;
			}
			uiDispatcher.UIBackground(() => appWindow.InfoBar.Show(message, icon, interactions));
		}
	}
}
