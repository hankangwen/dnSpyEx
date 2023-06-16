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
	sealed class CloseHandlePatcherX86 : PatcherX86 {
		const uint STATUS_HANDLE_NOT_CLOSABLE = 0xC0000235;
		const uint STATUS_INVALID_HANDLE = 0xC0000008;
		const int ObjectHandleFlagInformation = 4;
		const int OBJECT_HANDLE_FLAG_INFORMATION_SIZE = 2;

		public CloseHandlePatcherX86(DbgNativeFunctionHookContext context) : base(context) { }

		/*
		 * The code below generates the equivalent of following code:
		 *
		 * OBJECT_HANDLE_FLAG_INFORMATION info;
		 * int result = NtQueryObject(handle, ObjectHandleFlagInformation, &info, sizeof(OBJECT_HANDLE_FLAG_INFORMATION), nullptr);
		 * if (result >= 0) {
		 * 		if(info.ProtectFromClose)
		 *			return STATUS_HANDLE_NOT_CLOSABLE;
		 *		return OriginalCloseHandle(handle);
		 *	}
		 *	return STATUS_INVALID_HANDLE;
		 */

		public bool TryPatchX86(string dllName, [NotNullWhen(false)] out string? errorMessage) {
			var function = functionProvider.GetFunction(dllName, CloseHandleHook.FuncName);

			if (!functionProvider.TryGetFunction("ntdll.dll", "NtQueryObject", out var ntQueryObjectAddress)) {
				errorMessage = "Failed to get address of NtQueryObject";
				return false;
			}

			var c = new Assembler(process.Bitness);

			var l7 = c.CreateLabel();
			var l6 = c.CreateLabel();
			var l9 = c.CreateLabel();

			c.push(ebp);
			c.mov(ebp, esp);
			c.sub(esp, 16);
			c.push(0);
			c.push(OBJECT_HANDLE_FLAG_INFORMATION_SIZE);
			c.lea(eax, __[ebp - 6]);
			c.push(eax);
			c.push(ObjectHandleFlagInformation);
			c.push(__dword_ptr[ebp + 8]);
			c.call(ntQueryObjectAddress);
			c.mov(__dword_ptr[ebp - 4], eax);
			c.cmp(__dword_ptr[ebp - 4], 0);
			c.js(l6);
			c.movzx(eax, __byte_ptr[ebp - 5]);
			c.test(al, al);
			c.je(l7);
			c.mov(eax, STATUS_HANDLE_NOT_CLOSABLE);
			c.jmp(l9);

			c.Label(ref l7);
			c.push(__dword_ptr[ebp + 8]);
			c.call(function.NewFunctionAddress);
			c.jmp(l9);

			c.Label(ref l6);
			c.mov(eax, STATUS_INVALID_HANDLE);

			c.Label(ref l9);
			c.leave();
			c.ret(4);

			if (!c.TryAssemble(new CodeWriterImpl(function), function.NewCodeAddress, out var encErrMsg, out _)) {
				errorMessage = $"Failed to encode: {encErrMsg}";
				return false;
			}

			errorMessage = null;
			return true;
		}

		public bool TryPatchX64(string dllName, [NotNullWhen(false)] out string? errorMessage) {
			var function = functionProvider.GetFunction(dllName, CloseHandleHook.FuncName);

			if (!functionProvider.TryGetFunction("ntdll.dll", "NtQueryObject", out var ntQueryObjectAddress)) {
				errorMessage = $"Failed to get address of NtQueryObject";
				return false;
			}

			var c = new Assembler(process.Bitness);

			var l7 = c.CreateLabel();
			var l6 = c.CreateLabel();
			var l9 = c.CreateLabel();

			c.push(rbp);
			c.mov(rbp, rsp);
			c.sub(rsp, 64);
			c.mov(__qword_ptr[rbp + 16], rcx);
			c.lea(rdx, __[rbp - 6]);
			c.mov(rax, __qword_ptr[rbp + 16]);
			c.mov(__qword_ptr[rsp + 32], 0);
			c.mov(r9d, OBJECT_HANDLE_FLAG_INFORMATION_SIZE);
			c.mov(r8, rdx);
			c.mov(edx, ObjectHandleFlagInformation);
			c.mov(rcx, rax);
			c.mov(r10, ntQueryObjectAddress);
			c.call(r10);
			c.mov(__dword_ptr[rbp - 4], eax);
			c.cmp(__dword_ptr[rbp - 4], 0);
			c.js(l6);
			c.movzx(eax, __byte_ptr[rbp - 5]);
			c.test(al, al);
			c.je(l7);
			c.mov(eax, STATUS_HANDLE_NOT_CLOSABLE);
			c.jmp(l9);

			c.Label(ref l7);
			c.mov(rax, __qword_ptr[rbp + 16]);
			c.mov(rcx, rax);
			c.mov(r10, function.NewFunctionAddress);
			c.call(r10);
			c.jmp(l9);

			c.Label(ref l6);
			c.mov(eax, STATUS_INVALID_HANDLE);

			c.Label(ref l9);
			c.leave();
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
