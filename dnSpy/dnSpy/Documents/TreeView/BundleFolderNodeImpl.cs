using System;
using System.Collections.Generic;
using dnSpy.Contracts.Decompiler;
using dnSpy.Contracts.Documents;
using dnSpy.Contracts.Documents.TreeView;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.Text;
using dnSpy.Contracts.TreeView;

namespace dnSpy.Documents.TreeView {
	sealed class BundleFolderNodeImpl : BundleFolderNode {
		public override Guid Guid => new Guid(DocumentTreeViewConstants.BUNDLE_FOLDER_NODE_GUID);
		protected override ImageReference GetIcon(IDotNetImageService dnImgMgr) => DsImages.FolderClosed;
		protected override ImageReference? GetExpandedIcon(IDotNetImageService dnImgMgr) => DsImages.FolderOpened;
		public override NodePathName NodePathName => new NodePathName(Guid);
		public override void Initialize() => TreeNode.LazyLoading = true;

		readonly BundleFolder bundleFolder;
		readonly BundleDocumentNode owner;

		public BundleFolderNodeImpl(BundleDocumentNode owner, BundleFolder bundleFolder) : base(bundleFolder) {
			this.bundleFolder = bundleFolder;
			this.owner = owner;
		}

		public override IEnumerable<TreeNodeData> CreateChildren() {
			foreach (var folder in bundleFolder.Folders) {
				yield return new BundleFolderNodeImpl(owner, folder);
			}

			foreach (var entry in bundleFolder.Entries) {
				if (entry.Document is not null)
					yield return Context.DocumentTreeView.CreateNode(owner, entry.Document);
			}
		}

		protected override void WriteCore(ITextColorWriter output, IDecompiler decompiler, DocumentNodeWriteOptions options) {
			output.Write(BoxedTextColor.Text, bundleFolder.Name);
			if ((options & DocumentNodeWriteOptions.ToolTip) != 0) {
				output.WriteLine();

				if (bundleFolder.Entries.Count != 0) {
					// TODO: localize string
					output.Write(BoxedTextColor.Text, $"Entries: {bundleFolder.Entries.Count}");
				}

				if (bundleFolder.Folders.Count != 0) {
					// TODO: localize string
					output.Write(BoxedTextColor.Text, $"Subfolders: {bundleFolder.Folders.Count}");
				}
			}
		}
	}
}
