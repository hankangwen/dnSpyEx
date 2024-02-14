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
using dnSpy.Contracts.App;
using dnSpy.Controls;

namespace dnSpy.MainApp {
	[Export]
	sealed class AppInfoBar : IAppInfoBar, IStackedContentChild {
		readonly InfoBar infoBar;
		readonly InfoBarVM infoBarVM;

		public object UIObject => infoBar;

		public AppInfoBar() => infoBar = new InfoBar { DataContext = infoBarVM = new InfoBarVM() };

		public IInfoBarElement Show(string message, InfoBarIcon icon = InfoBarIcon.Information, params InfoBarInteraction[] interactions) {
			var notification = new InfoBarElementVM(infoBarVM, message, icon);
			foreach (var interaction in interactions)
				notification.AddInteraction(interaction.Text, interaction.Action);
			infoBarVM.Elements.Insert(0, notification);
			return notification;
		}
	}
}
