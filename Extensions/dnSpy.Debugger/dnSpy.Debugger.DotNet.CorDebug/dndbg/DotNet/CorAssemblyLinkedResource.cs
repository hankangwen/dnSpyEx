using dnlib.DotNet;

namespace dndbg.DotNet {
	sealed class CorAssemblyLinkedResource : AssemblyLinkedResource, ICorHasCustomAttribute {
		readonly CorModuleDef readerModule;
		readonly uint origRid;

		public MDToken OriginalToken => new MDToken(MDToken.Table, origRid);

		protected override void InitializeCustomAttributes() => readerModule.InitCustomAttributes(this, ref customAttributes, new GenericParamContext());

		public CorAssemblyLinkedResource(CorModuleDef readerModule, ManifestResource mr, AssemblyRef asmRef) : base(mr.Name, asmRef, mr.Flags) {
			this.readerModule = readerModule;
			Rid = origRid = mr.Rid;
			Offset = mr.Offset;
		}
	}
}
