using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using dnlib.IO;

namespace dnSpy.Contracts.Documents {
	/// <summary>
	/// Represents one entry in a <see cref="SingleFileBundle"/>
	/// </summary>
	public sealed class BundleEntry {
		byte[]? data;
		DataReader reader;

		/// <summary>
		/// Type of the entry <seealso cref="BundleFileType"/>
		/// </summary>
		public BundleFileType Type { get; }

		/// <summary>
		/// Path of an embedded file, relative to the Bundle source-directory.
		/// </summary>
		public string RelativePath { get; }

		/// <summary>
		/// The offset of the entry's data.
		/// </summary>
		public long Offset { get; }

		/// <summary>
		/// The size of the entry's data.
		/// </summary>
		public long Size { get; }

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
		public byte[] Data {
			get {
				if (data is not null)
					return data;
				Interlocked.CompareExchange(ref data, reader.ReadRemainingBytes(), null);
				return data;
			}
		}

		BundleEntry(BundleFileType type, string relativePath, long offset, long size, byte[] data) {
			Type = type;
			RelativePath = relativePath.Replace('/', '\\');
			Offset = offset;
			Size = size;
			this.data = data;
		}

		BundleEntry(BundleFileType type, string relativePath, long offset, long size, DataReader reader) {
			Type = type;
			RelativePath = relativePath.Replace('/', '\\');
			Offset = offset;
			Size = size;
			this.reader = reader;
		}

		internal static IReadOnlyList<BundleEntry> ReadEntries(DataReader reader, int count, bool allowCompression) {
			var res = new BundleEntry[count];

			for (int i = 0; i < count; i++) {
				long offset = reader.ReadInt64();
				long size = reader.ReadInt64();
				long compSize = allowCompression ? reader.ReadInt64() : 0;
				var type = (BundleFileType)reader.ReadByte();
				string path = reader.ReadSerializedString();

				if (compSize == 0)
					res[i] = new BundleEntry(type, path, offset, size, reader.Slice((uint)offset, (uint)size));
				else
					res[i] = new BundleEntry(type, path, offset, size, ReadCompressedEntryData(reader, offset, size, compSize));
			}

			return res;
		}

		static byte[] ReadCompressedEntryData(DataReader reader, long offset, long size, long compSize) {
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
