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

using dnSpy.Contracts.TreeView;
using dnSpy.Contracts.TreeView.Text;
using ICSharpCode.Decompiler.IL.Transforms;

#if DEBUG
namespace dnSpy.Decompiler.ILSpy.ILAst {
	interface IStepperTreeNodeDataContext {
		ITreeView TreeView { get; }
		ITreeViewNodeTextElementProvider TreeViewNodeTextElementProvider { get; }
		Stepper.Node? CurrentState { get; }
		void ShowStateAfter(Stepper.Node node);
	}
}
#endif
