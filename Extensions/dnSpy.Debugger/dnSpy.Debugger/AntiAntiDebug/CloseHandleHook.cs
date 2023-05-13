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
using System.Diagnostics;
using dnSpy.Contracts.Debugger;
using dnSpy.Contracts.Debugger.AntiAntiDebug;

namespace dnSpy.Debugger.AntiAntiDebug {
	[ExportDbgNativeFunctionHook(Kernel32DllName, FuncName, new DbgArchitecture[0], new[] { DbgOperatingSystem.Windows })]
	sealed class CloseHandleHook : IDbgNativeFunctionHook {
		readonly DebuggerSettings debuggerSettings;

		public const string Kernel32DllName = "kernel32.dll";
		public const string FuncName = "CloseHandle";

		[ImportingConstructor]
		public CloseHandleHook(DebuggerSettings debuggerSettings) => this.debuggerSettings = debuggerSettings;

		public bool IsEnabled(DbgNativeFunctionHookContext context) => debuggerSettings.AntiCloseHandle;

		public void Hook(DbgNativeFunctionHookContext context, out string? errorMessage) {
			switch (context.Process.Architecture) {
			case DbgArchitecture.X86:
				HookX86(context, out errorMessage);
				break;

			case DbgArchitecture.X64:
				HookX64(context, out errorMessage);
				break;

			default:
				Debug.Fail($"Unsupported architecture: {context.Process.Architecture}");
				errorMessage = $"Unsupported architecture: {context.Process.Architecture}";
				break;
			}
		}

		void HookX86(DbgNativeFunctionHookContext context, out string? errorMessage) =>
			new CloseHandlePatcherX86(context).TryPatchX86(Kernel32DllName, out errorMessage);

		void HookX64(DbgNativeFunctionHookContext context, out string? errorMessage) =>
			new CloseHandlePatcherX86(context).TryPatchX64(Kernel32DllName, out errorMessage);
	}
}
