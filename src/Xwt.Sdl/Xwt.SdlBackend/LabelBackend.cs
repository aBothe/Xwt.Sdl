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
using Pango;

namespace Xwt.Sdl
{
	public class LabelBackend : WidgetBackend,ILabelBackend
	{
		#region Properties

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

		public Pango.Layout CreateLayout(Cairo.Context c)
		{
			var lay = CairoHelper.CreateLayout (c);

			lay.FontDescription = FontBackend;

			lay.SetText (text ?? string.Empty);

			Pango.Alignment al;
			switch (alignment) {
				default:
					al = Pango.Alignment.Center;
					break;
				case Alignment.Start:
					al = Pango.Alignment.Left;
					break;
				case Alignment.End:
					al = Pango.Alignment.Right;
					break;
			}

			lay.Alignment = al;

			Pango.EllipsizeMode el;
			switch (this.ellipsize) {
				default:
					el = Pango.EllipsizeMode.None;
					break;
				case Xwt.EllipsizeMode.Middle:
					el = Pango.EllipsizeMode.Middle;
					break;
				case Xwt.EllipsizeMode.End:
					el = Pango.EllipsizeMode.End;
					break;
				case Xwt.EllipsizeMode.Start:
					el = Pango.EllipsizeMode.Start;
					break;
			}
			lay.Ellipsize = el;

			switch (this.wrap) {
				case Xwt.WrapMode.Character:
					lay.Wrap = Pango.WrapMode.Char;
					break;
				case Xwt.WrapMode.Word:
					lay.Wrap = Pango.WrapMode.Word;
					break;
				case Xwt.WrapMode.WordAndCharacter:
					lay.Wrap = Pango.WrapMode.WordChar;
					break;
			}

			//TODO: Remaining properties

			return lay;
		}

		public override void Draw (CairoContextBackend c, Rectangle dirtyRect)
		{
			base.Draw (c, dirtyRect);

			using (var lay = CreateLayout(c.Context)) {
				int w, h;
				lay.GetPixelSize (out w, out h);
				c.Context.RelMoveTo (0, -h);
				CairoHelper.ShowLayout (c.Context, lay);
			}
		}

		protected override Size GetPreferredSize (Cairo.Context fontExtentContext, double maxWidth, double maxHeight)
		{
			var st = WidgetStyles.Instance;
			using(var lay = CreateLayout(fontExtentContext)){
				int w, h;
				lay.GetPixelSize (out w, out h);
				return new Size (Math.Min(maxWidth, w+st.LabelXPadding), Math.Min(maxHeight, h+st.LabelYPadding));
			}
		}
			
		public void SetFormattedText (FormattedText text)
		{
			throw new NotImplementedException ();
		}
	}
}

