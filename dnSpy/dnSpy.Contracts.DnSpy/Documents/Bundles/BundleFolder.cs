using System.Collections.Generic;

namespace dnSpy.Contracts.Documents {
	/// <summary>
	/// Represents one folder in a <see cref="SingleFileBundle"/>
	/// </summary>
	public sealed class BundleFolder {
		/// <summary>
		/// Gets the short name of the folder.
		/// </summary>
		public string Name { get; }

		internal BundleFolder(string name) => Name = name;

		/// <summary>
		/// The folders nested within this folder.
		/// </summary>
		public List<BundleFolder> Folders { get; } = new List<BundleFolder>();

		/// <summary>
		/// The entires in this folder.
		/// </summary>
		public List<BundleEntry> Entries { get; } = new List<BundleEntry>();
	}
}
