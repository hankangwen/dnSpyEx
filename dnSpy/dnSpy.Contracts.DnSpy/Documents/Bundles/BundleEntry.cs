using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using dnlib.IO;

namespace dnSpy.Contracts.Documents {
	/// <summary>
	/// Represents one entry in a <see cref="SingleFileBundle"/>
	/// </summary>
	public sealed class BundleEntry {
		/// <summary>
		/// Type of the entry <seealso cref="BundleFileType"/>
		/// </summary>
		public BundleFileType Type { get; }

		/// <summary>
		/// Path of an embedded file, relative to the Bundle source-directory.
		/// </summary>
		public string RelativePath { get; }

		/// <summary>
		/// The file name of the entry.
		/// </summary>
		public string? FileName { get; internal set; }

		/// <summary>
		/// Docuemnt assosciated with this entry.
		/// </summary>
		public IDsDocument? Document { get; internal set; }

		/// <summary>
		/// The raw data of the entry.
		/// </summary>
		public byte[] Data { get; }

		BundleEntry(BundleFileType type, string relativePath, byte[] data) {
			Type = type;
			RelativePath = relativePath.Replace('/', '\\');
			Data = data;
		}

		internal static IReadOnlyList<BundleEntry> ReadEntries(DataReader reader, int count, bool allowCompression) {
			var res = new BundleEntry[count];

			for (int i = 0; i < count; i++) {
				long offset = reader.ReadInt64();
				long size = reader.ReadInt64();
				long compSize = allowCompression ? reader.ReadInt64() : 0;
				var type = (BundleFileType)reader.ReadByte();
				string path = reader.ReadSerializedString();

				res[i] = new BundleEntry(type, path, ReadEntryData(reader, offset, size, compSize));
			}

			return res;
		}

		static byte[] ReadEntryData(DataReader reader, long offset, long size, long compSize) {
			if (compSize == 0) {
				reader.Position = (uint)offset;
				return reader.ReadBytes((int)size);
			}

			using (var decompressedStream = new MemoryStream((int)size)) {
				using (var compressedStream = reader.Slice((uint)offset, (uint)compSize).AsStream()) {
					using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress)) {
						deflateStream.CopyTo(decompressedStream);
						return decompressedStream.ToArray();
					}
				}
			}
		}
	}
}
