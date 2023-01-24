using dnlib.DotNet;

namespace dndbg.DotNet {
	sealed class CorLinkedResource : LinkedResource, ICorHasCustomAttribute {
		readonly CorModuleDef readerModule;
		readonly uint origRid;

		public MDToken OriginalToken => new MDToken(MDToken.Table, origRid);

		protected override void InitializeCustomAttributes() => readerModule.InitCustomAttributes(this, ref customAttributes, new GenericParamContext());

		public CorLinkedResource(CorModuleDef readerModule, ManifestResource mr, FileDef file) : base(mr.Name, file, mr.Flags) {
			this.readerModule = readerModule;
			Rid = origRid = mr.Rid;
			Offset = mr.Offset;
		}
	}
}
