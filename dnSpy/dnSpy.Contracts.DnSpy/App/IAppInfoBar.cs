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

namespace dnSpy.Contracts.App {
	/// <summary>
	/// App info bar
	/// </summary>
	public interface IAppInfoBar {
		/// <summary>
		///	Shows a new info bar element of the specific icon with the given message and interactions.
		/// </summary>
		/// <param name="message">The message to display</param>
		/// <param name="icon">The icon of message</param>
		/// <param name="interactions">Possible interactions on the element</param>
		public IInfoBarElement Show(string message, InfoBarIcon icon = InfoBarIcon.Information, params InfoBarInteraction[] interactions);
	}
}
