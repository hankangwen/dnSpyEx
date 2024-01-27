using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using dnSpy.AsmEditor.Commands;
using dnSpy.AsmEditor.Properties;
using dnSpy.Contracts.Documents;
using dnSpy.Contracts.Documents.TreeView;
using dnSpy.Contracts.Menus;
using dnSpy.Contracts.TreeView;

namespace dnSpy.AsmEditor.Bundle {
	sealed class SaveBundleContentsCommand {
		[ExportMenuItem(Header = "res:SaveBundleContents", Group = MenuConstants.GROUP_CTX_DOCUMENTS_ASMED_BUNDLE, Order = 0)]
		sealed class DocumentsCommand : DocumentsContextMenuHandler {
			public override bool IsVisible(AsmEditorContext context) => SaveBundleContentsCommand.CanExecute(context);
			public override void Execute(AsmEditorContext context) => SaveBundleContentsCommand.Execute(context);
		}

		[ExportMenuItem(OwnerGuid = MenuConstants.APP_MENU_EDIT_GUID, Header = "res:SaveBundleContents", Group = MenuConstants.GROUP_APP_MENU_EDIT_ASMED_BUNDLE, Order = 0)]
		sealed class EditMenuCommand : EditMenuHandler {
			[ImportingConstructor]
			public EditMenuCommand(IAppService appService) : base(appService.DocumentTreeView) {
			}
			public override bool IsVisible(AsmEditorContext context) => SaveBundleContentsCommand.CanExecute(context);
			public override void Execute(AsmEditorContext context) => SaveBundleContentsCommand.Execute(context);
		}

		static bool IsSingleFileBundle(AsmEditorContext context) => context.Nodes.Length == 1 && context.Nodes[0] is BundleDocumentNode;
		static bool CanExecute(AsmEditorContext context) => SaveBundleContentsCommand.IsSingleFileBundle(context);
		static void Execute(AsmEditorContext context) {
			var docNode = context.Nodes[0].GetDocumentNode();
			var bundleDoc = docNode!.Document as DsBundleDocument;
			Debug2.Assert(bundleDoc != null);
			Debug2.Assert(bundleDoc.SingleFileBundle != null);
			SaveBundle.Save(bundleDoc.SingleFileBundle.Entries.ToArray(), dnSpy_AsmEditor_Resources.SaveBundleContents);
		}
	}

	sealed class SaveRawEntryCommand {
		[ExportMenuItem(Header = "res:SaveRawEntry", Group = MenuConstants.GROUP_CTX_DOCUMENTS_ASMED_BUNDLE, Order = 1)]
		sealed class DocumentsCommand : DocumentsContextMenuHandler {
			public override bool IsVisible(AsmEditorContext context) => SaveRawEntryCommand.CanExecute(context);
			public override void Execute(AsmEditorContext context) => SaveRawEntryCommand.Execute(context);
		}

		[ExportMenuItem(OwnerGuid = MenuConstants.APP_MENU_EDIT_GUID, Header = "res:SaveRawEntry", Group = MenuConstants.GROUP_APP_MENU_EDIT_ASMED_BUNDLE, Order = 1)]
		sealed class EditMenuCommand : EditMenuHandler {
			[ImportingConstructor]
			public EditMenuCommand(IAppService appService) : base(appService.DocumentTreeView) {
			}
			public override bool IsVisible(AsmEditorContext context) => SaveRawEntryCommand.CanExecute(context);
			public override void Execute(AsmEditorContext context) => SaveRawEntryCommand.Execute(context);
		}

		static bool IsBundleSingleSelection(AsmEditorContext context) => context.Nodes.Length == 1 && context.Nodes[0] is IBundleEntryNode;
		static bool CanExecute(AsmEditorContext context) => SaveRawEntryCommand.IsBundleSingleSelection(context);
		static void Execute(AsmEditorContext context) {
			var bundleEntryNode = (IBundleEntryNode)context.Nodes[0];
			SaveBundle.Save([bundleEntryNode.BundleEntry!], dnSpy_AsmEditor_Resources.SaveRawEntry);
		}
	}

	class SaveRawEntriesCommand {
		[ExportMenuItem(Header = "res:SaveRawEntries", Group = MenuConstants.GROUP_CTX_DOCUMENTS_ASMED_BUNDLE, Order = 2)]
		sealed class DocumentsCommand : DocumentsContextMenuHandler {
			public override bool IsVisible(AsmEditorContext context) => SaveRawEntriesCommand.CanExecute(context);
			public override void Execute(AsmEditorContext context) => SaveRawEntriesCommand.Execute(context);
		}

		[ExportMenuItem(OwnerGuid = MenuConstants.APP_MENU_EDIT_GUID, Header = "res:SaveRawEntries", Group = MenuConstants.GROUP_APP_MENU_EDIT_ASMED_BUNDLE, Order = 2)]
		sealed class EditMenuCommand : EditMenuHandler {
			[ImportingConstructor]
			public EditMenuCommand(IAppService appService) : base(appService.DocumentTreeView) {
			}
			public override bool IsVisible(AsmEditorContext context) => SaveRawEntriesCommand.CanExecute(context);
			public override void Execute(AsmEditorContext context) => SaveRawEntriesCommand.Execute(context);
		}
		private static bool IsBundleMultipleSelection(AsmEditorContext context) => context.Nodes.Length > 1 && context.Nodes.All(node => node is IBundleEntryNode);
		static bool CanExecute(AsmEditorContext context) => SaveRawEntriesCommand.IsBundleMultipleSelection(context);
		static void Execute(AsmEditorContext context) {
			var bundleEntries = context.Nodes.Select(x => ((IBundleEntryNode)x).BundleEntry!);
			SaveBundle.Save(bundleEntries.ToArray(), dnSpy_AsmEditor_Resources.SaveRawEntries);
		}
	}
}
