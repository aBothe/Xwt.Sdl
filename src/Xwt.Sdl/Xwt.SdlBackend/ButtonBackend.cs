//
// ButtonBackend.cs
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

namespace Xwt.Sdl
{
	public class ButtonBackend : WidgetBackend, IButtonBackend
	{
		#region Properties
		ButtonStyle style;
		ButtonType type;
		string label;
		bool mnemonic;
		ImageDescription image;
		ContentPosition pos;
		public new IButtonEventSink EventSink {get{return base.EventSink as IButtonEventSink;}}

		bool clicked;
		float v,w;
		#endregion

		const double imageToLabelSpace = 10.0;
		const double yMargin = 8;
		const double xMargin = 8;
		const double cornerRadius = 5;

		public override void Draw (CairoBackend.CairoContextBackend c,Rectangle dirtyRect)
		{
			double X, Y;
			GetAbsoluteLocation (out X, out Y);

			// Border
			{
				const double borderColor = 0.6;

				// Top left corner
				c.Context.Arc (X +cornerRadius, Y + cornerRadius, cornerRadius, -Math.PI, -Math.PI/2);

				// top edge
				c.Context.RelLineTo (Width - (cornerRadius*2), 0);

				// top right corner
				c.Context.Arc (X + Width -cornerRadius, Y + cornerRadius, cornerRadius, -Math.PI/2, 0);

				// left edge
				c.Context.RelLineTo (0, Height-cornerRadius*2);

				// bottom right corner
				c.Context.Arc (X + Width -cornerRadius, Y + Height - cornerRadius, cornerRadius, 0, Math.PI/2);

				// bottom corner
				c.Context.RelLineTo (-Width+(cornerRadius*2), 0);

				// bottom left corner
				c.Context.Arc (X + cornerRadius, Y + Height - cornerRadius, cornerRadius, Math.PI/2, Math.PI);

				c.Context.ClosePath ();
				c.Context.SetSourceRGB (borderColor, borderColor, borderColor);
				c.Context.LineWidth = 1;
				c.Context.StrokePreserve ();
			}

			// button background
			{
				double grey;
				if (!Sensitive)
					grey = 0.7;
				else if (clicked)
					grey = 0.9;
				else if (MouseEntered)
					grey = 1;
				else
					grey = 0.95;

				var g = new Cairo.LinearGradient (X + Width / 2, Y, X + Width / 2, Y + Height);
				g.AddColorStop (0, new Cairo.Color (grey, grey, grey));

				grey /= 1.2;
				g.AddColorStop (1, new Cairo.Color (grey, grey, grey));

				c.Context.SetSource (g);
				c.Context.Fill ();

				g.Dispose ();
			}

			CairoBackend.CairoConversion.SelectFont (c.Context, FontBackend);

			Cairo.TextExtents labelExt;
			if (label != null)
				labelExt = c.Context.TextExtents (label);
			else
				labelExt = new Cairo.TextExtents ();

			// Image
			if (!image.IsNull) {
				var imgBck = image.Backend as System.Drawing.Bitmap;
				var data = imgBck.LockBits (new System.Drawing.Rectangle (0, 0, imgBck.Width, imgBck.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				var imgSurf = new Cairo.ImageSurface (data.Scan0, Cairo.Format.Argb32, data.Width, data.Height, data.Stride);

				var imgY = Y + Height / 2.0 - imgSurf.Height / 2;
				if(string.IsNullOrEmpty(label))
					c.Context.SetSource (imgSurf,X + Width/2 - imgSurf.Width/2, imgY);
				else
					c.Context.SetSource (imgSurf,X + Width/2 - (labelExt.Width + imageToLabelSpace + imgSurf.Width)/2, imgY);
				c.Context.Paint ();

				imgSurf.Dispose ();
				imgBck.UnlockBits (data);
			}

			// Label
			if (label != null) {
				if (!Sensitive)
					c.Context.SetSourceRGB (0.5,0.5,0.5);
				else
					c.Context.SetSourceRGB (0, 0, 0);

				c.Context.MoveTo (X + Width/2.0 + (-labelExt.Width + (image.IsNull ? 0.0 : (imageToLabelSpace + (image.Backend as System.Drawing.Bitmap).Width)))/2, 
					Y + Height/2.0d + labelExt.Height/2.5d);
				c.Context.ShowText (label);
			}
		}

		protected override Size GetPreferredSize (Cairo.Context c)
		{
			var ext = string.IsNullOrEmpty(label) ? new Cairo.TextExtents() : c.TextExtents (label);
			var imgSz = image.Size;
			var x = ext.Width + imgSz.Width + cornerRadius/2;
			if (!string.IsNullOrEmpty (label) && imgSz.Width > 0)
				x += imageToLabelSpace;
			var y = Math.Max(ext.Height, imgSz.Height);
			return new Size (x + xMargin, y + yMargin + cornerRadius/2);
		}

		internal override void FireMouseEnter ()
		{
			clicked = false;
			base.FireMouseEnter ();
			Invalidate ();
		}

		internal override void FireMouseLeave ()
		{
			clicked = false;
			base.FireMouseLeave ();
			Invalidate ();
		}

		internal override bool FireMouseButton (bool down, PointerButton butt, int x, int y, int multiplePress = 1)
		{
			var ret = base.FireMouseButton (down, butt, x, y, multiplePress);
			if (Sensitive) {
				if (down) {
					clicked = true;
				} else if(clicked) {
					clicked = false;
					EventSink.OnClicked ();
				}
				Invalidate ();
			}
			return ret;
		}

		#region IButtonBackend implementation

		public void SetButtonStyle (ButtonStyle style)
		{
			this.style = style;
			Invalidate ();
		}

		public void SetButtonType (ButtonType type)
		{
			this.type = type;
			Invalidate ();
		}

		public void SetContent (string label, bool useMnemonic, ImageDescription image, ContentPosition position)
		{
			this.label = label;
			this.mnemonic = useMnemonic;
			this.image = image;
			this.pos = position;
			Invalidate ();
		}

		#endregion
	}
}

