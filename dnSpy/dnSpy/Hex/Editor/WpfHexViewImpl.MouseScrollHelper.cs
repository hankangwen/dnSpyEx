using System;
using System.Windows;
using System.Windows.Interop;
using dnSpy.Contracts.Hex;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;

namespace dnSpy.Hex.Editor {
	sealed partial class WpfHexViewImpl {
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

			readonly WpfHexViewImpl owner;

			public MouseScrollHelper(WpfHexViewImpl owner) {
				this.owner = owner;
				PresentationSource.AddSourceChangedHandler(owner.VisualElement, OnSourceChanged);
			}

			public void OnClosed() {
				PresentationSource.RemoveSourceChangedHandler(owner.VisualElement, OnSourceChanged);
				if (PresentationSource.FromVisual(owner.VisualElement) is HwndSource hwndSource)
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
				if (!(owner.container ?? owner.VisualElement).IsMouseOver)
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
					owner.ViewScroller.ScrollViewportHorizontallyByPixels(owner.FormattedLineSource.ColumnWidth * 10D * delta / 120.0);
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
					owner.DisplayHexLineContainingBufferPosition(new HexBufferPoint(owner.Buffer, owner.Buffer.Span.Start), 0.0, ViewRelativePosition.Top);
					return;
				case SB_BOTTOM:
					owner.DisplayHexLineContainingBufferPosition(new HexBufferPoint(owner.Buffer, owner.Buffer.Span.End), 0.0, ViewRelativePosition.Bottom);
					return;
				default:
					return;
				}
			}

			void HorizontalScroll(int iCode, int columnNumber) {
            	switch (iCode) {
            	case SB_LINELEFT:
					owner.ViewScroller.ScrollViewportHorizontallyByPixels(-owner.FormattedLineSource.ColumnWidth);
            		return;
            	case SB_LINERIGHT:
					owner.ViewScroller.ScrollViewportHorizontallyByPixels(owner.FormattedLineSource.ColumnWidth);
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
					owner.ViewportLeft = columnNumber * owner.FormattedLineSource.ColumnWidth;
            		return;
            	case SB_LEFT:
					owner.ViewportLeft = 0.0;
            		return;
            	case SB_RIGHT:
					owner.ViewportLeft = owner.MaxTextRightCoordinate + WpfHexViewConstants.EXTRA_HORIZONTAL_SCROLLBAR_WIDTH;
            		return;
            	default:
            		return;
            	}
            }
		}
	}
}
