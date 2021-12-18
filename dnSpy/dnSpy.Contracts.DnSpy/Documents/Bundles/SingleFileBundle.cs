using System.Collections.Generic;
using System.Linq;
using dnlib.IO;
using dnlib.PE;

namespace dnSpy.Contracts.Documents {
	/// <summary>
	/// Class for dealing with .NET 5 single-file bundles.
	///
	/// Based on code from Microsoft.NET.HostModel.
	/// </summary>
	public sealed class SingleFileBundle {
		static readonly byte[] bundleSignature = {
			// 32 bytes represent the bundle signature: SHA-256 for ".net core bundle"
			0x8b, 0x12, 0x02, 0xb9, 0x6a, 0x61, 0x20, 0x38,
			0x72, 0x7b, 0x93, 0x02, 0x14, 0xd7, 0xa0, 0x32,
			0x13, 0xf5, 0xb9, 0xe6, 0xef, 0xae, 0x33, 0x18,
			0xee, 0x3b, 0x2d, 0xce, 0x24, 0xb3, 0x6a, 0xae
		};

		/// <summary>
		/// The major version of the bundle.
		/// </summary>
		public uint MajorVersion { get; }

		/// <summary>
		/// The minor version of the bundle.
		/// </summary>
		public uint MinorVersion { get; }

		/// <summary>
		/// Number of entries in the bundle.
		/// </summary>
		public int EntryCount { get; }

		/// <summary>
		/// ID of the bundle.
		/// </summary>
		public string BundleID { get; }

		/// <summary>
		/// Offset of the embedded *.deps.json file.
		/// Only present in version 2.0 and above.
		/// </summary>
		public long DepsJsonOffset { get; }

		/// <summary>
		/// Size of the embedded *.deps.json file.
		/// Only present in version 2.0 and above.
		/// </summary>
		public long DepsJsonSize { get; }

		/// <summary>
		/// Offset of the embedded *.runtimeconfig.json file.
		/// Only present in version 2.0 and above.
		/// </summary>
		public long RuntimeConfigJsonOffset { get; }

		/// <summary>
		/// Size of the embedded *.runtimeconfig.json file.
		/// Only present in version 2.0 and above.
		/// </summary>
		public long RuntimeConfigJsonSize { get; }

		/// <summary>
		/// Bundle flags
		/// Only present in version 2.0 and above.
		/// </summary>
		public ulong Flags { get; }

		/// <summary>
		/// All of the entries present in the bundle
		/// </summary>
		public IReadOnlyList<BundleEntry> Entries { get; }

		/// <summary>
		/// The top level entries present in the bundle
		/// </summary>
		public IReadOnlyList<BundleEntry> TopLevelEntries { get; }

		/// <summary>
		/// The top level folders present in the bundle.
		/// </summary>
		public IReadOnlyList<BundleFolder> TopLevelFolders { get; }

		SingleFileBundle(DataReader reader, uint major, uint minor) {
			MajorVersion = major;
			MinorVersion = minor;
			EntryCount = reader.ReadInt32();
			BundleID = reader.ReadSerializedString();
			if (MajorVersion >= 2) {
				DepsJsonOffset = reader.ReadInt64();
				DepsJsonSize = reader.ReadInt64();
				RuntimeConfigJsonOffset = reader.ReadInt64();
				RuntimeConfigJsonSize = reader.ReadInt64();
				Flags = reader.ReadUInt64();
			}

			Entries = BundleEntry.ReadEntries(reader, EntryCount, MajorVersion >= 6);

			var rootFolder = new BundleFolder("");
			var folders = new Dictionary<string, BundleFolder> { { "", rootFolder } };
			foreach (var entry in Entries) {
				(string dirname, string filename) = SeperateFileName(entry.RelativePath);
				entry.FileName = filename;
				GetFolder(dirname).Entries.Add(entry);
			}
			TopLevelEntries = rootFolder.Entries;
			TopLevelFolders = rootFolder.Folders;

			static (string directory, string file) SeperateFileName(string filename) {
				int pos = filename.LastIndexOfAny(new[] { '/', '\\' });
				return pos == -1 ? ("", filename) : (filename.Substring(0, pos), filename.Substring(pos + 1));
			}

			BundleFolder GetFolder(string name) {
				if (folders.TryGetValue(name, out var result))
					return result;
				(string dirname, string basename) = SeperateFileName(name);
				result = new BundleFolder(basename);
				GetFolder(dirname).Folders.Add(result);
				folders.Add(name, result);
				return result;
			}
		}

		/// <summary>
		/// Parses a bundle from the provided <see cref="IPEImage"/>
		/// </summary>
		/// <param name="peImage">The <see cref="IPEImage"/></param>
		/// <returns>The <see cref="SingleFileBundle"/> or null if its not a bundle.</returns>
		public static SingleFileBundle? FromPEImage(IPEImage peImage) {
			if (!IsBundle(peImage, out long bundleHeaderOffset))
				return null;
			var reader = peImage.CreateReader();
			reader.Position = (uint)bundleHeaderOffset;
			uint major = reader.ReadUInt32();
			if (major < 1 || major > 6)
				return null;
			uint minor = reader.ReadUInt32();
			return new SingleFileBundle(reader, major, minor);
		}

		static bool IsBundle(IPEImage peImage, out long bundleHeaderOffset) {
			var reader = peImage.CreateReader();

			byte[] buffer = new byte[bundleSignature.Length];
			uint end = reader.Length - (uint)bundleSignature.Length;
			for (uint i = 0; i < end; i++) {
				reader.Position = i;
				buffer[0] = reader.ReadByte();
				if (buffer[0] != 0x8b)
					continue;
				reader.ReadBytes(buffer, 1, bundleSignature.Length - 1);
				if (!buffer.SequenceEqual(bundleSignature))
					continue;
				reader.Position = i - sizeof(long);
				bundleHeaderOffset = reader.ReadInt64();
				if (bundleHeaderOffset <= 0 || bundleHeaderOffset >= reader.Length)
					continue;
				return true;
			}

			bundleHeaderOffset = 0;
			return false;
		}
	}
}
