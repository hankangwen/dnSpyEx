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
using dnSpy.Contracts.Images;
using dnSpy.Contracts.Text;
using dnSpy.Contracts.Text.Classification;
using dnSpy.Contracts.TreeView;
using dnSpy.Contracts.TreeView.Text;
using ICSharpCode.Decompiler.IL.Transforms;

#if DEBUG
namespace dnSpy.Decompiler.ILSpy.ILAst {
	sealed class StepperNodeTreeNodeData : TreeNodeData {
		readonly IStepperTreeNodeDataContext context;
		readonly Stepper.Node node;

		public override Guid Guid => Guid.Empty;

		static class Cache {
			static readonly TextClassifierTextColorWriter writer = new TextClassifierTextColorWriter();
			public static TextClassifierTextColorWriter GetWriter() => writer;
			public static void FreeWriter(TextClassifierTextColorWriter writer) => writer.Clear();
		}

		public override object Text {
			get {
				var writer = Cache.GetWriter();
				try {
					writer.Write(node == context.CurrentState ? BoxedTextColor.Blue : BoxedTextColor.Text, node.Description);
					var classifierContext = new TreeViewNodeClassifierContext(writer.Text, context.TreeView, this, isToolTip: false, colorize: true, colors: writer.Colors);
					return context.TreeViewNodeTextElementProvider.CreateTextElement(classifierContext, TreeViewContentTypes.TreeViewNode, TextElementFlags.FilterOutNewLines);
				}
				finally {
					Cache.FreeWriter(writer);
				}
			}
		}

		public override object? ToolTip => null;

		public override ImageReference Icon => ImageReference.None;

		public StepperNodeTreeNodeData(IStepperTreeNodeDataContext context, Stepper.Node node) {
			this.context = context;
			this.node = node;
		}

		public override IEnumerable<TreeNodeData> CreateChildren() {
			for (int i = 0; i < node.Children.Count; i++)
				yield return new StepperNodeTreeNodeData(context, node.Children[i]);
		}

		public override bool Activate() {
			context.ShowStateAfter(node);
			return true;
		}

		public override void OnRefreshUI() { }
	}
}
#endif
