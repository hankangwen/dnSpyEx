namespace dnSpy.Roslyn.Debugger.Formatters {
	struct AdditionalTypeInfoState {
		internal readonly IAdditionalTypeInfoProvider? TypeInfoProvider;

		internal int DynamicTypeIndex;
		internal int NativeIntTypeIndex;
		internal int TupleNameIndex;

		public AdditionalTypeInfoState(IAdditionalTypeInfoProvider? typeInfoProvider) => TypeInfoProvider = typeInfoProvider;

		public override bool Equals(object? obj) {
			if (obj is not AdditionalTypeInfoState other)
				return false;
			if (!Equals(TypeInfoProvider, other.TypeInfoProvider))
				return false;
			if (DynamicTypeIndex != other.DynamicTypeIndex)
				return false;
			if (NativeIntTypeIndex != other.NativeIntTypeIndex)
				return false;
			if (TupleNameIndex != other.TupleNameIndex)
				return false;
			return true;
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = TypeInfoProvider is not null ? TypeInfoProvider.GetHashCode() : 0;
				hashCode = hashCode * 397 ^ DynamicTypeIndex;
				hashCode = hashCode * 397 ^ NativeIntTypeIndex;
				hashCode = hashCode * 397 ^ TupleNameIndex;
				return hashCode;
			}
		}
	}
}
