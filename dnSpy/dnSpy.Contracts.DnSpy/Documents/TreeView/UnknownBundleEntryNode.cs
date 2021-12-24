using System.Diagnostics;

namespace dnSpy.Contracts.Documents.TreeView {
	/// <summary>
	/// Unknown bundle entry node
	/// </summary>
	public abstract class UnknownBundleEntryNode : DocumentTreeNodeData  {
		/// <summary>
		/// Constructor
		/// </summary>
		protected UnknownBundleEntryNode(BundleEntry bundleEntry) => Debug2.Assert(bundleEntry is not null);
	}
}
