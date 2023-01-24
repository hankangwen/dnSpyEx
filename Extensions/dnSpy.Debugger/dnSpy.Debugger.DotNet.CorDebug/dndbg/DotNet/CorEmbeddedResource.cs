using dnlib.DotNet;
using dnlib.IO;

namespace dndbg.DotNet {
	sealed class CorEmbeddedResource : EmbeddedResource, ICorHasCustomAttribute {
		readonly CorModuleDef readerModule;
		readonly uint origRid;

		public MDToken OriginalToken => new MDToken(MDToken.Table, origRid);

		protected override void InitializeCustomAttributes() => readerModule.InitCustomAttributes(this, ref customAttributes, new GenericParamContext());

		public CorEmbeddedResource(CorModuleDef readerModule, ManifestResource mr, byte[] data)
			: this(readerModule, mr, ByteArrayDataReaderFactory.Create(data, filename: null), 0, (uint)data.Length) {
		}

		public CorEmbeddedResource(CorModuleDef readerModule, ManifestResource mr, DataReaderFactory dataReaderFactory, uint offset, uint length)
			: base(mr.Name, dataReaderFactory, offset, length, mr.Flags) {
			this.readerModule = readerModule;
			Rid = origRid =mr.Rid;
			Offset = mr.Offset;
		}
	}
}
