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

#if DEBUG
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using dnSpy.Contracts.Decompiler;

namespace dnSpy.Decompiler.ILSpy.Core.Settings {
	sealed class ILAstDecompilerSettings : DecompilerSettingsBase {
		public override int Version => settingsVersion;
		int settingsVersion;

		public override event EventHandler? VersionChanged;

		public override IEnumerable<IDecompilerOption> Options {
			get { yield break; }
		}

		public int StepLimit {
			get => stepLimit;
			set {
				if (stepLimit == value)
					return;
				stepLimit = value;
				OnPropertyChanged();
			}
		}
		int stepLimit = int.MaxValue;

		public ILAstDecompilerSettings() { }

		ILAstDecompilerSettings(ILAstDecompilerSettings other) => stepLimit = other.stepLimit;

		void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
			Interlocked.Increment(ref settingsVersion);
			VersionChanged?.Invoke(this, EventArgs.Empty);
		}

		public override DecompilerSettingsBase Clone() => new ILAstDecompilerSettings(this);

		public override bool Equals(object? obj) => obj is ILAstDecompilerSettings settings && settings.stepLimit == stepLimit;

		// ReSharper disable once NonReadonlyMemberInGetHashCode
		public override int GetHashCode() => stepLimit;
	}
}
#endif
