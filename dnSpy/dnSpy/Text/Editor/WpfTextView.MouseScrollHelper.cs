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

using System;
using System.Windows;
using System.Windows.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;

namespace dnSpy.Text.Editor {
	sealed partial class WpfTextView {
		sealed class MouseScrollHelper {
			const int SB_LINEUP = 0;
			const int SB_LINELEFT = 0;
			const int SB_LINEDOWN = 1;
			const int SB_LINERIGHT = 1;
			const int SB_PAGEUP = 2;
			const int SB_PAGELEFT = 2;
			const int SB_PAGEDOWN = 3;
			const int SB_PAGERIGHT = 3;
			const int SB_THUMBPOSITION = 4;
			const int SB_THUMBTRACK = 5;
			const int SB_TOP = 6;
			const int SB_LEFT = 6;
			const int SB_BOTTOM = 7;
			const int SB_RIGHT = 7;

			const int WM_HSCROLL = 276;
			const int WM_VSCROLL = 277;
			const int WM_MOUSEHWHEEL = 526;

			readonly WpfTextView owner;

			public MouseScrollHelper(WpfTextView owner) {
				this.owner = owner;
				PresentationSource.AddSourceChangedHandler(owner, OnSourceChanged);
			}

			public void OnClosed() {
				PresentationSource.RemoveSourceChangedHandler(owner, OnSourceChanged);
				if (PresentationSource.FromVisual(owner) is HwndSource hwndSource)
					hwndSource.RemoveHook(MouseScrollHook);
			}

			void OnSourceChanged(object sender, SourceChangedEventArgs e) {
            	if (owner.IsClosed)
            		return;
            	if (e.OldSource is HwndSource oldSource)
            		oldSource.RemoveHook(MouseScrollHook);
            	if (e.NewSource is HwndSource newSource)
            		newSource.AddHook(MouseScrollHook);
            }

            IntPtr MouseScrollHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            	if (owner.IsClosed)
            		return IntPtr.Zero;
				if (!(owner.container ?? owner).IsMouseOver)
					return IntPtr.Zero;
				if (msg == WM_VSCROLL) {
					VerticalScroll(LoWord(wParam));
					handled = true;
				}
				else if (msg == WM_HSCROLL) {
					if ((owner.Options.WordWrapStyle() & WordWrapStyles.WordWrap) == 0)
						HorizontalScroll(LoWord(wParam), HiWord(wParam));
					handled = true;
				}
				else if (msg == WM_MOUSEHWHEEL) {
					int delta = HiWord(wParam);
					owner.ViewScroller.ScrollViewportHorizontallyByPixels(owner.FormattedLineSource!.ColumnWidth * 10D * delta / 120.0);
					handled = true;
				}
				return IntPtr.Zero;
			}

			static int HiWord(IntPtr wParam) => unchecked((short)(wParam.ToInt64() >> 16));

			static int LoWord(IntPtr wParam) => unchecked((short)wParam.ToInt64());

			void VerticalScroll(int iCode) {
				switch (iCode) {
				case SB_LINEUP:
					owner.ViewScroller.ScrollViewportVerticallyByPixels(owner.LineHeight);
					return;
				case SB_LINEDOWN:
					owner.ViewScroller.ScrollViewportVerticallyByPixels(-owner.LineHeight);
					return;
				case SB_PAGEUP:
					owner.ViewScroller.ScrollViewportVerticallyByPage(ScrollDirection.Up);
					return;
				case SB_PAGEDOWN:
					owner.ViewScroller.ScrollViewportVerticallyByPage(ScrollDirection.Down);
					return;
				case SB_TOP:
					owner.DisplayTextLineContainingBufferPosition(new SnapshotPoint(owner.TextSnapshot, 0), 0.0, ViewRelativePosition.Top);
					return;
				case SB_BOTTOM:
					owner.DisplayTextLineContainingBufferPosition(new SnapshotPoint(owner.TextSnapshot, owner.TextSnapshot.Length), 0.0, ViewRelativePosition.Bottom);
					return;
				default:
					return;
				}
			}

			void HorizontalScroll(int iCode, int columnNumber) {
            	switch (iCode) {
            	case SB_LINELEFT:
					owner.ViewScroller.ScrollViewportHorizontallyByPixels(-owner.FormattedLineSource!.ColumnWidth);
            		return;
            	case SB_LINERIGHT:
					owner.ViewScroller.ScrollViewportHorizontallyByPixels(owner.FormattedLineSource!.ColumnWidth);
            		return;
            	case SB_PAGELEFT:
					owner.ViewScroller.ScrollViewportHorizontallyByPixels(-owner.ViewportWidth / 4.0);
            		return;
            	case SB_PAGERIGHT:
					owner.ViewScroller.ScrollViewportHorizontallyByPixels(owner.ViewportWidth / 4.0);
            		return;
            	case SB_THUMBPOSITION:
            	case SB_THUMBTRACK:
            		if (columnNumber < 0)
            			columnNumber = 0;
					owner.ViewportLeft = columnNumber * owner.FormattedLineSource!.ColumnWidth;
            		return;
            	case SB_LEFT:
					owner.ViewportLeft = 0.0;
            		return;
            	case SB_RIGHT:
					owner.ViewportLeft = Math.Max(owner.MaxTextRightCoordinate, owner.TextCaret.Right) + WpfTextViewConstants.EXTRA_HORIZONTAL_SCROLLBAR_WIDTH;
            		return;
            	default:
            		return;
            	}
            }
		}
	}
}
