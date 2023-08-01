/*
    Copyright (C) 2023 ElektroKill

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/


using System.Collections.ObjectModel;
using dnSpy.Contracts.Debugger.DotNet.Evaluation;
using Microsoft.CodeAnalysis.ExpressionEvaluator;

namespace dnSpy.Roslyn.Debugger.Formatters {
	sealed class CustomTypeInfoAdditionalTypeInfoProvider : IAdditionalTypeInfoProvider {
		readonly DbgDotNetCustomTypeInfo customTypeInfo;
		readonly ReadOnlyCollection<byte>? dynamicFlags;
		readonly ReadOnlyCollection<string?>? tupleElementNames;

		CustomTypeInfoAdditionalTypeInfoProvider(DbgDotNetCustomTypeInfo customTypeInfo) {
			this.customTypeInfo = customTypeInfo;
			CustomTypeInfo.Decode(customTypeInfo.CustomTypeInfoId, customTypeInfo.CustomTypeInfo, out dynamicFlags, out tupleElementNames);
		}

		public static CustomTypeInfoAdditionalTypeInfoProvider? TryCreate(DbgDotNetCustomTypeInfo? customTypeInfo) {
			if (customTypeInfo?.CustomTypeInfoId != CustomTypeInfo.PayloadTypeId)
				return null;
			return new CustomTypeInfoAdditionalTypeInfoProvider(customTypeInfo);
		}

		public bool IsDynamicType(int typeIndex) => DynamicFlagsCustomTypeInfo.GetFlag(dynamicFlags, typeIndex);

		public string? GetTupleElementName(int typeIndex) => CustomTypeInfo.GetTupleElementNameIfAny(tupleElementNames, typeIndex);

		public bool IsNativeIntegerType(int typeIndex) => false;

		public override bool Equals(object? obj) {
			if (ReferenceEquals(this, obj))
				return true;
			return obj is CustomTypeInfoAdditionalTypeInfoProvider other && customTypeInfo.Equals(other.customTypeInfo);
		}

		public override int GetHashCode() => customTypeInfo.GetHashCode();
	}
}
