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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using dnSpy.Contracts.Controls;
using dnSpy.Contracts.Decompiler;
using dnSpy.Contracts.Documents;
using dnSpy.Contracts.Documents.Tabs;
using dnSpy.Contracts.ToolWindows;
using dnSpy.Contracts.ToolWindows.App;
using dnSpy.Contracts.TreeView;
using dnSpy.Contracts.TreeView.Text;
using dnSpy.Decompiler.ILSpy.Core.ILAst;
using dnSpy.Decompiler.ILSpy.Core.Settings;
using ICSharpCode.Decompiler.IL.Transforms;

#if DEBUG
namespace dnSpy.Decompiler.ILSpy.ILAst {
	[Export(typeof(IToolWindowContentProvider))]
	sealed class AnalyzerToolWindowContentProvider : IToolWindowContentProvider {
		readonly IDecompilerService decompilerService;
		readonly IDocumentTabService documentTabService;
		readonly ITreeViewService treeViewService;
		readonly ITreeViewNodeTextElementProvider treeViewNodeTextElementProvider;

		public ILAstStepperToolWindowContent DocumentTreeViewWindowContent => analyzerToolWindowContent ??= new ILAstStepperToolWindowContent(decompilerService, documentTabService, treeViewService, treeViewNodeTextElementProvider);
		ILAstStepperToolWindowContent? analyzerToolWindowContent;

		[ImportingConstructor]
		AnalyzerToolWindowContentProvider(IDecompilerService decompilerService, IDocumentTabService documentTabService, ITreeViewService treeViewService, ITreeViewNodeTextElementProvider treeViewNodeTextElementProvider) {
			this.decompilerService = decompilerService;
			this.documentTabService = documentTabService;
			this.treeViewService = treeViewService;
			this.treeViewNodeTextElementProvider = treeViewNodeTextElementProvider;
		}

		public IEnumerable<ToolWindowContentInfo> ContentInfos {
			get { yield return new ToolWindowContentInfo(StepperConstants.ToolWindowGuid); }
		}

		public ToolWindowContent? GetOrCreate(Guid guid) => guid == StepperConstants.ToolWindowGuid ? DocumentTreeViewWindowContent : null;
	}

	sealed class ILAstStepperToolWindowContent : ToolWindowContent, IFocusable, IStepperTreeNodeDataContext {
		readonly IDecompilerService decompilerService;
		readonly IDocumentTabService documentTabService;
		readonly ITreeViewNodeTextElementProvider treeViewNodeTextElementProvider;
		readonly ITreeView treeView;
		ILAstDecompiler? decompiler;

		public override object UIObject => treeView.UIObject;
		public override IInputElement? FocusedElement => null;
		public override FrameworkElement ZoomElement => treeView.UIObject;
		public override Guid Guid => StepperConstants.ToolWindowGuid;
		public override string Title => "ILAst Stepper";
		public bool CanFocus => true;
		public ITreeView TreeView => treeView;
		public ITreeViewNodeTextElementProvider TreeViewNodeTextElementProvider => treeViewNodeTextElementProvider;
		public Stepper.Node? CurrentState { get; private set; }

		public ILAstStepperToolWindowContent(IDecompilerService decompilerService, IDocumentTabService documentTabService, ITreeViewService treeViewService, ITreeViewNodeTextElementProvider treeViewNodeTextElementProvider) {
			this.decompilerService = decompilerService;
			this.documentTabService = documentTabService;
			this.treeViewNodeTextElementProvider = treeViewNodeTextElementProvider;
			decompilerService.DecompilerChanged += DecompilerService_DecompilerChanged;

			var options = new TreeViewOptions {
				CanDragAndDrop = false,
				SelectionMode = SelectionMode.Single,
			};
			treeView = treeViewService.Create(StepperConstants.TreeViewGuid, options);
			treeView.UIObject.Padding = new Thickness(0, 2, 0, 2);
			treeView.UIObject.BorderThickness = new Thickness(1);

			OnDecompilerSelected(decompilerService.Decompiler);
		}

		void DecompilerService_DecompilerChanged(object? sender, EventArgs e) =>
			OnDecompilerSelected(decompilerService.Decompiler);

		void OnDecompilerSelected(IDecompiler newDecompiler) {
			if (decompiler is not null)
				decompiler.StepperUpdated -= ILAstStepperUpdated;
			decompiler = newDecompiler as ILAstDecompiler;
			if (decompiler is not null)
				decompiler.StepperUpdated += ILAstStepperUpdated;
			else {
				CurrentState = null;
				treeView.UIObject.Dispatcher.Invoke(() => treeView.Root.Children.Clear());
			}
		}

		void ILAstStepperUpdated(object? sender, EventArgs e) {
			if (decompiler is null)
				return;

			CurrentState = null;

			treeView.UIObject.Dispatcher.Invoke(() => {
				treeView.Root.Children.Clear();
				for (var i = 0; i < decompiler.Stepper.Steps.Count; i++)
					treeView.Root.AddChild(treeView.Create(new StepperNodeTreeNodeData(this, decompiler.Stepper.Steps[i])));
			});
		}

		public void ShowStateAfter(Stepper.Node node) {
			CurrentState = node;
			SetNewStepLimit(node.EndStep);
			RefreshTabs();
			treeView.RefreshAllNodes();
		}

		void SetNewStepLimit(int stepLimit) {
			if (decompiler is not null && decompiler.Settings is ILAstDecompilerSettings settings)
				settings.StepLimit = stepLimit;
		}

		void RefreshTabs() {
			var toRefresh = new HashSet<IDocumentTab>();
			foreach (var tab in documentTabService.VisibleFirstTabs) {
				var decomp = (tab.Content as IDecompilerTabContent)?.Decompiler;
				if (decomp is not null && decomp == decompiler)
					toRefresh.Add(tab);
			}
			documentTabService.Refresh(toRefresh.ToArray());
		}

		public void Focus() => treeView.Focus();
	}
}
#endif
