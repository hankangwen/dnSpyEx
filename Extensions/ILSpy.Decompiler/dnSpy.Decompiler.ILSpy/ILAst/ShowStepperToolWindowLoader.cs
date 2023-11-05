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
using dnSpy.Contracts.Extension;
using dnSpy.Contracts.ToolWindows.App;
using dnSpy.Decompiler.ILSpy.Core.ILAst;

#if DEBUG
namespace dnSpy.Decompiler.ILSpy.ILAst {
	[ExportAutoLoaded]
	sealed class ShowStepperToolWindowLoader : IAutoLoaded {
		[ImportingConstructor]
		public ShowStepperToolWindowLoader(IDsToolWindowService toolWindowService) =>
			toolWindowService.Show(StepperConstants.ToolWindowGuid);
	}
}
#endif
