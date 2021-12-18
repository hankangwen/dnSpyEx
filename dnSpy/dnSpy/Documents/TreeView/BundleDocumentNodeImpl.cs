using System;
using System.Collections.Generic;
using System.Diagnostics;
using dnSpy.Contracts.Decompiler;
using dnSpy.Contracts.Documents;
using dnSpy.Contracts.Documents.TreeView;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.Text;
using dnSpy.Contracts.TreeView;
using dnSpy.Decompiler;

namespace dnSpy.Documents.TreeView {
	sealed class BundleDocumentNodeImpl : BundleDocumentNode {
		public BundleDocumentNodeImpl(IDsDocument document) : base(document) { }

		public override Guid Guid => new Guid(DocumentTreeViewConstants.BUNDLE_NODE_GUID);

		protected override ImageReference GetIcon(IDotNetImageService dnImgMgr) => dnImgMgr.GetImageReference(Document.PEImage!);

		public override IEnumerable<TreeNodeData> CreateChildren() {
			Debug2.Assert(Document.SingleFileBundle is not null);

			// Ensure docuemt children are initialized.
			// This is needed as loading the Children of the docment will assign the Document property of BundleEntry objects.
			var _ = Document.Children;

			foreach (var bundleFolder in Document.SingleFileBundle.TopLevelFolders) {
				yield return new BundleFolderNodeImpl(this, bundleFolder);
			}

			foreach (var entry in Document.SingleFileBundle.TopLevelEntries) {
				if (entry.Document is not null)
					yield return Context.DocumentTreeView.CreateNode(this, entry.Document);
			}

			// TODO: return all bundle entries
		}

		protected override void WriteCore(ITextColorWriter output, IDecompiler decompiler, DocumentNodeWriteOptions options) {
			Debug2.Assert(Document.SingleFileBundle is not null);
			Debug2.Assert(Document.PEImage is not null);
			if ((options & DocumentNodeWriteOptions.ToolTip) == 0)
				new NodeFormatter().Write(output, decompiler, Document);
			else {
				output.Write(BoxedTextColor.Text, TargetFrameworkUtils.GetArchString(Document.PEImage.ImageNTHeaders.FileHeader.Machine));

				output.WriteLine();
				output.WriteFilename(Document.Filename);
			}
		}

		public override FilterType GetFilterType(IDocumentTreeNodeFilter filter) =>
			filter.GetResult(Document).FilterType;
	}
}
