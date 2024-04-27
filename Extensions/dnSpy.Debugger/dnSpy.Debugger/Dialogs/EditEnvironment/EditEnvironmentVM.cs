/*
    Copyright (C) 2024 ElektroKill

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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using dnSpy.Contracts.Controls.ToolWindows;
using dnSpy.Contracts.Debugger;
using dnSpy.Contracts.MVVM;
using dnSpy.Contracts.Text;

namespace dnSpy.Debugger.Dialogs.EditEnvironment {
	sealed class EditEnvironmentVM : ViewModelBase {
		readonly EditValueProviderService editValueProviderService;
		readonly DbgEnvironment originalEnvironment;

		public ObservableCollection<EnvironmentVariableVM> EnvironmentVariables { get; }
		public ObservableCollection<EnvironmentVariableVM> SelectedItems { get; }

		IEditValueProvider KeyEditValueProvider => keyEditValueProvider ?? (keyEditValueProvider = editValueProviderService.Create(ContentTypes.EnvironmentVariableKey, Array.Empty<string>()));
		IEditValueProvider? keyEditValueProvider;
		IEditValueProvider ValueEditValueProvider => valueEditValueProvider ?? (valueEditValueProvider = editValueProviderService.Create(ContentTypes.EnvironmentVariableValue, Array.Empty<string>()));
		IEditValueProvider? valueEditValueProvider;

		public ICommand AddEnvironmentVariableCommand => new RelayCommand(a => AddEnvironmentVariable());
		public ICommand RemoveEnvironmentVariableCommand => new RelayCommand(a => RemoveEnvironmentVariable(), a => SelectedItems.Count > 0);
		public ICommand ResetCommand => new RelayCommand(a => Reset());

		public EditEnvironmentVM(EditValueProviderService editValueProviderService) : this(editValueProviderService, new DbgEnvironment()) { }

		public EditEnvironmentVM(EditValueProviderService editValueProviderService, DbgEnvironment environment) {
			this.editValueProviderService = editValueProviderService;
			originalEnvironment = environment;

			EnvironmentVariables = new ObservableCollection<EnvironmentVariableVM>();
			SelectedItems = new ObservableCollection<EnvironmentVariableVM>();

			InitializeFrom(environment);
		}

		void AddEnvironmentVariable() {
			SelectedItems.Clear();
			var vm = new EnvironmentVariableVM(KeyEditValueProvider, ValueEditValueProvider);
			EnvironmentVariables.Insert(0, vm);
			SelectedItems.Add(vm);
		}

		void RemoveEnvironmentVariable() {
			var itemsToRemove = new List<EnvironmentVariableVM>(SelectedItems);
			SelectedItems.Clear();
			foreach (var environmentVariableVm in itemsToRemove)
				EnvironmentVariables.Remove(environmentVariableVm);
		}

		void Reset() {
			SelectedItems.Clear();
			EnvironmentVariables.Clear();
			InitializeFrom(originalEnvironment);
		}

		void InitializeFrom(DbgEnvironment environment) {
			foreach (var pair in environment.Environment) {
				EnvironmentVariables.Add(new EnvironmentVariableVM(KeyEditValueProvider, ValueEditValueProvider) {
					Key = pair.Key,
					Value = pair.Value
				});
			}
		}

		public void CopyTo(DbgEnvironment environment) {
			environment.Clear();
			foreach (var environmentVariableVm in EnvironmentVariables)
				environment.Add(environmentVariableVm.Key, environmentVariableVm.Value);
		}
	}
}
