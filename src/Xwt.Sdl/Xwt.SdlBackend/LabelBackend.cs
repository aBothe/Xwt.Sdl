//
// LabelBackend.cs
//
// Author:
//       Alexander Bothe <info@alexanderbothe.com>
//
// Copyright (c) 2013 Alexander Bothe
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Xwt.Backends;
using SDL2;
using Xwt.CairoBackend;

namespace Xwt.Sdl
{
	public class LabelBackend : WidgetBackend,ILabelBackend
	{
		protected override bool WidgetSupportsFocusing {
			get {
				return false;
			}
		}

		public override void Draw (CairoContextBackend c, Rectangle dirtyRect)
		{
			base.Draw (c, dirtyRect);

			c.Context.SelectFont (base.FontBackend);

		}

		protected override Size GetPreferredSize (Cairo.Context fontExtentContext, double maxWidth, double maxHeight)
		{
			fontExtentContext.SelectFont (FontBackend);
			var st = WidgetStyles.Instance;
			var ext = fontExtentContext.TextExtents (text ?? string.Empty);


			return new Size (Math.Min(maxWidth, ext.Width+st.LabelXPadding), Math.Min(maxHeight, ext.Height+st.LabelYPadding));
		}

		#region ILabelBackend implementation

		public void SetFormattedText (FormattedText text)
		{
			throw new NotImplementedException ();
		}

		string text;
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				Invalidate ();
			}
		}

		Xwt.Drawing.Color textCol;
		public Xwt.Drawing.Color TextColor {
			get {
				return textCol;
			}
			set {
				textCol = value;
				Invalidate ();
			}
		}

		Alignment alignment;
		public Alignment TextAlignment {
			get {
				return alignment;
			}
			set {
				alignment = value;
				Invalidate ();
			}
		}

		EllipsizeMode ellipsize;
		public EllipsizeMode Ellipsize {
			get {
				return ellipsize;
			}
			set {
				ellipsize = value;
				Invalidate ();
			}
		}

		WrapMode wrap;
		public WrapMode Wrap {
			get {
				return wrap;
			}
			set {
				wrap = value;
				Invalidate ();
			}
		}

		#endregion
	}
}

