namespace dnSpy.Roslyn.Debugger.Formatters {
	struct AdditionalTypeInfoState {
		internal readonly IAdditionalTypeInfoProvider? TypeInfoProvider;

		internal int DynamicTypeIndex;
		internal int NativeIntTypeIndex;
		internal int TupleNameIndex;

		public AdditionalTypeInfoState(IAdditionalTypeInfoProvider? typeInfoProvider) => TypeInfoProvider = typeInfoProvider;
	}
}
