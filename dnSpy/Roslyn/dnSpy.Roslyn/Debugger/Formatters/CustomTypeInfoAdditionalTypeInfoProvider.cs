using System.Collections.ObjectModel;
using dnSpy.Contracts.Debugger.DotNet.Evaluation;
using Microsoft.CodeAnalysis.ExpressionEvaluator;

namespace dnSpy.Roslyn.Debugger.Formatters {
	sealed class CustomTypeInfoAdditionalTypeInfoProvider : IAdditionalTypeInfoProvider {
		readonly ReadOnlyCollection<byte>? dynamicFlags;
		readonly ReadOnlyCollection<string?>? tupleElementNames;

		CustomTypeInfoAdditionalTypeInfoProvider(DbgDotNetCustomTypeInfo customTypeInfo) => CustomTypeInfo.Decode(customTypeInfo.CustomTypeInfoId, customTypeInfo.CustomTypeInfo, out dynamicFlags, out tupleElementNames);

		public static CustomTypeInfoAdditionalTypeInfoProvider? TryCreate(DbgDotNetCustomTypeInfo customTypeInfo) {
			if (customTypeInfo.CustomTypeInfoId != CustomTypeInfo.PayloadTypeId)
				return null;
			return new CustomTypeInfoAdditionalTypeInfoProvider(customTypeInfo);
		}

		public bool IsDynamicType(int typeIndex) => DynamicFlagsCustomTypeInfo.GetFlag(dynamicFlags, typeIndex);

		public string? GetTupleElementName(int typeIndex) => CustomTypeInfo.GetTupleElementNameIfAny(tupleElementNames, typeIndex);

		public bool IsNativeIntegerType(int typeIndex) => false;
	}
}
