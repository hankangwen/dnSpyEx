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

using System;
using System.Diagnostics;
using System.IO;
using dndbg.COM.MetaData;
using dndbg.DotNet;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.PE;
using dnSpy.Contracts.Utilities;

namespace dndbg.Engine {
	sealed class BreakProcessHelper {
		readonly DnDebugger debugger;
		readonly BreakProcessKind type;
		DnBreakpoint? breakpoint;

		public BreakProcessHelper(DnDebugger debugger, BreakProcessKind type) {
			this.debugger = debugger ?? throw new ArgumentNullException(nameof(debugger));
			this.type = type;
			AddStartupBreakpoint();
		}

		void AddStartupBreakpoint() {
			switch (type) {
			case BreakProcessKind.None:
				break;

			case BreakProcessKind.ModuleCctorOrEntryPoint:
			case BreakProcessKind.EntryPoint:
				breakpoint = debugger.CreateBreakpoint(DebugEventBreakpointKind.LoadModule, OnLoadModule);
				break;

			default:
				Debug.Fail($"Unknown BreakProcessKind: {type}");
				break;
			}
		}

		void SetILBreakpoint(DnModuleId moduleId, uint token) {
			Debug2.Assert(token != 0 && breakpoint is null);
			DnBreakpoint? bp = null;
			bp = debugger.CreateBreakpoint(moduleId, token, 0, ctx2 => {
				debugger.RemoveBreakpoint(bp!);
				ctx2.E.AddPauseState(new EntryPointBreakpointPauseState(ctx2.E.CorAppDomain, ctx2.E.CorThread));
				return false;
			});
		}

		bool OnLoadModule(DebugEventBreakpointConditionContext ctx) {
			var lmArgs = (LoadModuleDebugCallbackEventArgs)ctx.EventArgs;
			var mod = lmArgs.CorModule;
			if (mod is null || !IsPrimaryProgramModule(mod))
				return false;

			uint methodToken = 0;
			if (type == BreakProcessKind.ModuleCctorOrEntryPoint)
				methodToken = GetGlobalStaticConstructor(mod.GetMetaDataInterface<IMetaDataImport>());

			if (methodToken == 0)
				methodToken = GetEntryPointToken(mod);

			if (MDToken.ToTable(methodToken) != Table.Method || MDToken.ToRID(methodToken) == 0)
				return false;

			debugger.RemoveBreakpoint(breakpoint!);
			breakpoint = null;
			Debug.Assert(!mod.IsDynamic && !mod.IsInMemory);
			// It's not a dyn/in-mem module so id isn't used
			var moduleId = debugger.TryGetModuleId(mod) ?? mod.GetModuleId(uint.MaxValue);
			SetILBreakpoint(moduleId, methodToken);
			return false;
		}

		bool IsPrimaryProgramModule(CorModule module) {
			if (module.IsDynamic)
				return false;

			if (module.IsInMemory)
				return true;

			var filename = module.Name;
			if (!File.Exists(filename))
				return true;

			if (GacInfo.IsGacPath(filename))
				return false;
			if (IsInDirOrSubDir(Path.GetDirectoryName(debugger.CLRPath)!, filename))
				return false;

			return true;
		}

		static bool IsInDirOrSubDir(string dir, string filename) {
			dir = dir.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			if (dir.Length > 0 && dir[dir.Length - 1] != Path.DirectorySeparatorChar)
				dir += Path.DirectorySeparatorChar.ToString();
			filename = filename.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			return filename.StartsWith(dir, StringComparison.OrdinalIgnoreCase);
		}

		static uint GetEntryPointToken(CorModule mod) {
			if (File.Exists(mod.Name)) {
				try {
					using var peImage = new PEImage(mod.Name);
					if (GetEntryPointToken(peImage, out uint entryPointToken))
						return entryPointToken;
				}
				catch {
				}
			}
			var process = mod.Process;
			if (process is not null) {
				try {
					using var moduleReader = new ProcessBinaryReader(new CorProcessReader(process), mod.Address);
					using var dataReaderFactory = new ProcessDataReaderFactory(moduleReader, mod.Size);

					var imageLayout = !mod.IsDynamic && mod.IsInMemory ? ImageLayout.File : ImageLayout.Memory;
					using var peImage = new PEImage(dataReaderFactory, imageLayout, true);

					if (GetEntryPointToken(peImage, out uint entryPointToken))
						return entryPointToken;
				}
				catch {
				}
			}
			return 0;
		}

		static bool GetEntryPointToken(PEImage peImage, out uint entryPointToken) {
			entryPointToken = 0;
			var dotNetDir = peImage.ImageNTHeaders.OptionalHeader.DataDirectories[14];
			if (dotNetDir.VirtualAddress == 0)
				return false;
			var cor20HeaderReader = peImage.CreateReader(dotNetDir.VirtualAddress, 0x48);
			var cor20Header = new ImageCor20Header(ref cor20HeaderReader, true);
			if ((cor20Header.Flags & ComImageFlags.NativeEntryPoint) != 0)
				return false;
			uint token = cor20Header.EntryPointToken_or_RVA;
			if (MDToken.ToTable(token) == Table.Method && MDToken.ToRID(token) != 0) {
				entryPointToken = token;
				return true;
			}
			return false;
		}

		static uint GetGlobalStaticConstructor(IMetaDataImport? mdi) {
			var mdTokens = MDAPI.GetMethodTokens(mdi, 0x02000001);
			foreach (uint mdToken in mdTokens) {
				string? name = MDAPI.GetMethodName(mdi, mdToken);
				if (name is null || name != ".cctor")
					continue;
				if (!MDAPI.GetMethodAttributes(mdi, mdToken, out var attrs, out _))
					continue;
				if ((attrs & MethodAttributes.RTSpecialName) == 0)
					continue;
				if ((attrs & MethodAttributes.Static) == 0)
					continue;

				return mdToken;
			}

			return 0;
		}
	}
}
