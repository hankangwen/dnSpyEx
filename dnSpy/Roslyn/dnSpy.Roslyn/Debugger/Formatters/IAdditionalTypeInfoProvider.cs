namespace dnSpy.Roslyn.Debugger.Formatters {
	interface IAdditionalTypeInfoProvider {
		bool IsDynamicType(int typeIndex);
		string? GetTupleElementName(int typeIndex);
		bool IsNativeIntegerType(int typeIndex);
	}
}
