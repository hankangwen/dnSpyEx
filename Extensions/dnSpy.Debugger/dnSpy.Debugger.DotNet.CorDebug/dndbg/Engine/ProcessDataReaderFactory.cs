/*
    Copyright (C) 2024 ElektroKill

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

using dndbg.DotNet;
using dnlib.IO;

namespace dndbg.Engine {
	sealed class ProcessDataReaderFactory : DataReaderFactory {
		DataStream? stream;
		uint length;

		public override string? Filename => null;

		public override uint Length => length;

		public ProcessDataReaderFactory(ProcessBinaryReader data, uint length) {
			this.length = length;
			stream = new ProcessDataStream(data);
		}

		public override DataReader CreateReader(uint offset, uint length) => CreateReader(stream, offset, length);

		public override void Dispose() {
			stream = null;
			length = 0;
		}
	}
}
