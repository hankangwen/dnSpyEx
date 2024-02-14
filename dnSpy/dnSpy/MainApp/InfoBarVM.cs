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

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using dnSpy.Contracts.App;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.MVVM;

namespace dnSpy.MainApp {
	sealed class InfoBarVM : ViewModelBase {
		public ObservableCollection<InfoBarElementVM> Elements { get; } = new ObservableCollection<InfoBarElementVM>();

		public ICommand RemoveElementCommand { get; }

		public InfoBarVM() => RemoveElementCommand = new RelayCommand(RemoveElement);

		internal void RemoveElement(object? obj) {
			if (obj is not InfoBarElementVM notification)
				return;
			Elements.Remove(notification);
		}
	}

	sealed class InfoBarElementVM : ViewModelBase, IInfoBarElement {
		readonly InfoBarVM parent;

		public string Message { get; }

		public InfoBarIcon Icon { get; }

		public ImageReference Image => Icon switch {
			InfoBarIcon.Information => DsImages.StatusInformation,
			InfoBarIcon.Warning => DsImages.StatusWarning,
			InfoBarIcon.Error => DsImages.StatusError,
			_ => DsImages.QuestionMark
		};

		public ObservableCollection<InfoBarInteractionVM> Interactions { get; } = new ObservableCollection<InfoBarInteractionVM>();

		public InfoBarElementVM(InfoBarVM parent, string message, InfoBarIcon icon) {
			this.parent = parent;
			Message = message;
			Icon = icon;
		}

		public void Close() => parent.RemoveElement(this);

		public void AddInteraction(string text, Action<IInfoBarInteractionContext> action) => Interactions.Add(new InfoBarInteractionVM(this, text, action));

		public void RemoveInteraction(InfoBarInteractionVM interaction) => Interactions.Remove(interaction);
	}

	sealed class InfoBarInteractionVM : ViewModelBase {
		internal readonly InfoBarElementVM parent;

		public string Text { get; }
		public ICommand ActionCommand { get; }

		public InfoBarInteractionVM(InfoBarElementVM parent, string text, Action<IInfoBarInteractionContext> action) {
			this.parent = parent;
			Text = text;
			ActionCommand = new RelayCommand(vm => {
				if (vm is not InfoBarInteractionVM interactionVM)
					return;
				action(new NotificationInteractionContext(interactionVM));
			});
		}
	}

	sealed class NotificationInteractionContext : IInfoBarInteractionContext {
		readonly InfoBarInteractionVM _interactionVm;

		public string InteractionText => _interactionVm.Text;

		public NotificationInteractionContext(InfoBarInteractionVM interactionVm) => _interactionVm = interactionVm;

		public void CloseElement() => _interactionVm.parent.Close();

		public void RemoveInteraction() => _interactionVm.parent.RemoveInteraction(_interactionVm);
	}
}
