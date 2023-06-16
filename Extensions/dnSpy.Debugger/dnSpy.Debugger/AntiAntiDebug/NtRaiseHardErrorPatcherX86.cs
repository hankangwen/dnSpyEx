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

using System.Diagnostics.CodeAnalysis;
using dnSpy.Contracts.Debugger.AntiAntiDebug;
using Iced.Intel;
using static Iced.Intel.AssemblerRegisters;

namespace dnSpy.Debugger.AntiAntiDebug {
	sealed class NtRaiseHardErrorPatcherX86 : PatcherX86 {
		const uint STATUS_SERVICE_NOTIFICATION = 0x50000018;

		public NtRaiseHardErrorPatcherX86(DbgNativeFunctionHookContext context) : base(context) { }

		public bool TryPatchX86(string dllName, [NotNullWhen(false)] out string? errorMessage) {
			var function = functionProvider.GetFunction(dllName, NtRaiseHardErrorHook.FuncName);
			const int numParameters = 6;

			var c = new Assembler(process.Bitness);

			c.cmp(__dword_ptr[esp + 4], STATUS_SERVICE_NOTIFICATION);
			c.je(function.NewFunctionAddress);
			c.xor(eax, eax);
			c.ret(4 * numParameters);

			if (!c.TryAssemble(new CodeWriterImpl(function), function.NewCodeAddress, out var encErrMsg, out _)) {
				errorMessage = $"Failed to encode: {encErrMsg}";
				return false;
			}

			errorMessage = null;
			return true;
		}

		public bool TryPatchX64(string dllName, [NotNullWhen(false)] out string? errorMessage) {
			var function = functionProvider.GetFunction(dllName, NtRaiseHardErrorHook.FuncName);

			var c = new Assembler(process.Bitness);
			var return_0 = c.CreateLabel();

			c.cmp(ecx, STATUS_SERVICE_NOTIFICATION);
			c.jne(return_0);

			c.mov(r10, function.NewFunctionAddress);
			c.jmp(r10);

			c.Label(ref return_0);
			c.xor(eax, eax);
			c.ret();

			if (!c.TryAssemble(new CodeWriterImpl(function), function.NewCodeAddress, out var encErrMsg, out _)) {
				errorMessage = $"Failed to encode: {encErrMsg}";
				return false;
			}

			errorMessage = null;
			return true;
		}
	}
}
