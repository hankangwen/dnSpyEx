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
