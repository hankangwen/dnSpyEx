using System.Diagnostics;

namespace dnSpy.Contracts.Documents.TreeView {
	/// <summary>
	/// JSON bundle entry node
	/// </summary>
	public abstract class JsonBundleEntryNode : DocumentTreeNodeData {
		/// <summary>
		/// Constructor
		/// </summary>
		protected JsonBundleEntryNode(BundleEntry bundleEntry) => Debug2.Assert(bundleEntry is not null);
	}
}
