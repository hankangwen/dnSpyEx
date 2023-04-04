using System.Collections.Generic;
using System.Collections.ObjectModel;
using dnSpy.Debugger.DotNet.Metadata;

namespace dnSpy.Roslyn.Debugger.Formatters {
	sealed class CustomAttributeAdditionalTypeInfoProvider : IAdditionalTypeInfoProvider {
		readonly ReadOnlyCollection<DmdCustomAttributeData> customAttributes;

		CustomAttributeAdditionalTypeInfoProvider(ReadOnlyCollection<DmdCustomAttributeData> customAttributes) => this.customAttributes = customAttributes;

		public static IAdditionalTypeInfoProvider? Create(IDmdCustomAttributeProvider? provider) {
			if (provider is null)
				return null;
			return new CustomAttributeAdditionalTypeInfoProvider(provider.CustomAttributes);
		}

		public static IAdditionalTypeInfoProvider? Create(ReadOnlyCollection<DmdCustomAttributeData>? customAttributes) {
			if (customAttributes is null)
				return null;
			return new CustomAttributeAdditionalTypeInfoProvider(customAttributes);
		}

		bool dynamicValuesInitialized;
		bool dynamicMatchAll;
		IList<DmdCustomAttributeTypedArgument>? dynamicValues;

		public bool IsDynamicType(int typeIndex) {
			if (!dynamicValuesInitialized) {
				foreach (var a in customAttributes) {
					if (a.AttributeType.FullName != "System.Runtime.CompilerServices.DynamicAttribute")
						continue;
					if (a.ConstructorArguments.Count == 0) {
						dynamicMatchAll = true;
						break;
					}
					if (a.ConstructorArguments.Count == 1 && a.ConstructorArguments[0].Value is IList<DmdCustomAttributeTypedArgument> values) {
						dynamicValues = values;
						break;
					}
				}
				dynamicValuesInitialized = true;
			}

			return dynamicMatchAll || dynamicValues is not null && typeIndex < dynamicValues.Count && dynamicValues[typeIndex].Value is bool b && b;
		}

		bool tupleElementNamesInitialized;
		IList<DmdCustomAttributeTypedArgument>? tupleElementNames;

		public string? GetTupleElementName(int typeIndex) {
			if (!tupleElementNamesInitialized) {
				foreach (var a in customAttributes) {
					if (a.AttributeType.FullName != "System.Runtime.CompilerServices.TupleElementNamesAttribute")
						continue;
					if (a.ConstructorArguments.Count != 1)
						continue;
					if (a.ConstructorArguments[0].Value is not IList<DmdCustomAttributeTypedArgument> argumentList)
						continue;
					tupleElementNames = argumentList;
					break;
				}

				tupleElementNamesInitialized = true;
			}

			return typeIndex < tupleElementNames?.Count ? tupleElementNames[typeIndex].Value as string : null;
		}

		bool nativeIntegerValuesInitialized;
		bool nativeIntegerMatchAll;
		IList<DmdCustomAttributeTypedArgument>? nativeIntegerValues;

		public bool IsNativeIntegerType(int typeIndex) {
			if (!nativeIntegerValuesInitialized) {
				foreach (var a in customAttributes) {
					if (a.AttributeType.FullName != "System.Runtime.CompilerServices.NativeIntegerAttribute")
						continue;
					if (a.ConstructorArguments.Count == 0) {
						nativeIntegerMatchAll = true;
						break;
					}
					if (a.ConstructorArguments.Count == 1 && a.ConstructorArguments[0].Value is IList<DmdCustomAttributeTypedArgument> values) {
						nativeIntegerValues = values;
						break;
					}
				}
				nativeIntegerValuesInitialized = true;
			}

			return nativeIntegerMatchAll || nativeIntegerValues is not null && typeIndex < nativeIntegerValues.Count && nativeIntegerValues[typeIndex].Value is bool b && b;
		}
	}
}
