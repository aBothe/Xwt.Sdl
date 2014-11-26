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
using Xwt.CairoBackend;

namespace Xwt.Sdl
{
	public class ButtonBackend : WidgetBackend, IButtonBackend
	{
		#region Properties
		ButtonStyle style;
		ButtonType type;
		Label label;
		public Size LabelSize
		{
			get{
				return label != null ? label.Size : new Size();
			}
		}

		bool mnemonic;
		ImageDescription image;
		ContentPosition pos;
		public new IButtonEventSink EventSink {get{return base.EventSink as IButtonEventSink;}}

		bool clicked;
		#endregion

		protected override void SensitivityChanged ()
		{
			if (!Sensitive)
				clicked = false;
		}

		const double imageToLabelSpace = 10.0;
		const double yPadding = 5;
		const double xPadding = 10;
		const double cornerRadius = 3;

		protected override void DrawInternally (CairoBackend.CairoContextBackend c,Rectangle dirtyRect)
		{
			var style = WidgetStyles.Instance;
			double X, Y;
			GetAbsoluteLocation (out X, out Y);

			// Border
			{
				c.Context.MoveTo (X, Y + cornerRadius);

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
				c.Context.SetColor (style.ButtonBorderColor);
				c.Context.LineWidth = style.ButtonBorderLineWidth;
				c.Context.StrokePreserve ();
			}

			// button background
			{
				double grey;
				if (!Sensitive)
					grey = style.ButtonInsensitiveGrey;
				else if (clicked)
					grey = style.ButtonClickedGrey;
				else if (MouseEntered)
					grey = style.ButtonHoveredGrey;
				else
					grey = style.ButtonDefaultGrey;

				var g = new Cairo.LinearGradient (X + Width / 2, Y, X + Width / 2, Y + Height);
				g.AddColorStop (0, new Cairo.Color (grey, grey, grey));

				grey /= 1.2;
				g.AddColorStop (1, new Cairo.Color (grey, grey, grey));

				c.Context.SetSource (g);
				c.Context.Fill ();

				g.Dispose ();
			}

			// Focus dash border
			if (HasFocus) {
				c.Context.LineWidth = 1;
				c.Context.SetColor (style.FocusDashBorderColor);
				c.Context.SetDash (style.FocusDashLine, 0);

				c.Context.Rectangle (X + xPadding/2, Y + yPadding/2, Width - xPadding, Height - yPadding);
				c.Context.Stroke ();
			}

			// Image
			if (!image.IsNull) {
				var imgBck = image.Backend as System.Drawing.Bitmap;
				var data = imgBck.LockBits (new System.Drawing.Rectangle (0, 0, imgBck.Width, imgBck.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				var imgSurf = new Cairo.ImageSurface (data.Scan0, Cairo.Format.Argb32, data.Width, data.Height, data.Stride);
				var imageWidth = (double)imgSurf.Width;

				var imgY = Y + Height / 2.0 - imgSurf.Height / 2;
				if (label == null)
					c.Context.SetSource (imgSurf, X + Width / 2 - imageWidth / 2, imgY);
				else {
					var contentWidth = (label != null ? (label.GetBackend() as LabelBackend).Width : 0) + imageToLabelSpace + imageWidth;
					if (contentWidth < Width)
						c.Context.SetSource (imgSurf, X + Width / 2 - contentWidth / 2, imgY);
					else
						c.Context.SetSource (imgSurf, X + xPadding, imgY);
				}
				c.Context.Paint ();

				imgSurf.Dispose ();
				imgBck.UnlockBits (data);
			}

			// Label
			if (label != null) {
				var labelBack = label.GetBackend () as LabelBackend;

				labelBack.textCol = Sensitive ? style.ButtonLabelColor : style.ButtonInsensitiveLabelColor;
				labelBack.Draw (c, dirtyRect);
			}
		}

		public override Size GetPreferredSize (Cairo.Context c, double maxX, double maxY)
		{
			var ext = label != null ? (label.GetBackend () as LabelBackend).GetPreferredSize(c,maxX, maxY) : new Size();
			var imgSz = image.Size;
			var x = ext.Width + imgSz.Width + cornerRadius/2;
			if (label != null && imgSz.Width > 0)
				x += imageToLabelSpace;
			var y = Math.Max(ext.Height, imgSz.Height);
			return new Size (x + xPadding, y + yPadding + cornerRadius/2);
		}

		internal override void OnBoundsChanged (double x, double y, double width, double height)
		{
			base.OnBoundsChanged (x, y, width, height);

			if (label != null) {
				var ll = (label.GetBackend () as LabelBackend);
				var imageWidth = image.IsNull ? 0.0 : (image.Size.Width + imageToLabelSpace);

				var labelSize = ll.GetPreferredSize (
					SizeConstraint.WithSize (width - imageWidth - 2 * cornerRadius), 
					SizeConstraint.WithSize (height - 2* cornerRadius));

				double movX;
				if (labelSize.Width + imageWidth < Width)
					movX = Width / 2.0 + (-labelSize.Width + imageWidth) / 2;
				else
					movX = xPadding + imageWidth;

				ll.OnBoundsChanged(movX, Height / 2d - labelSize.Height / 2d, 
					Math.Max(0.0, Math.Min(labelSize.Width, Width - imageWidth)),
					Math.Max(0.0, Math.Min(labelSize.Height, Height - 2 * cornerRadius)));
			}
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

		internal override bool FireKeyDown (Key k, char ch, ModifierKeys mods, bool rep, uint timestamp)
		{
			if (Sensitive && mods == ModifierKeys.None && (k == Key.Space || k == Key.Return)) {
				clicked = true;
				Invalidate ();
				return true;
			}

			return base.FireKeyDown (k, ch, mods, rep, timestamp);
		}

		internal override bool FireKeyUp (Key k, char ch, ModifierKeys mods, bool rep, uint timestamp)
		{
			if (clicked) {
				clicked = false;
				EventSink.OnClicked ();
				Invalidate ();
				return true;
			}

			return base.FireKeyUp (k, ch, mods, rep, timestamp);
		}

		public override object Font {
			set {
				base.Font = value;
				if (label != null)
					label.Font = Frontend.Font;
				UpdateWidgetPreferredSize ();
			}
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

		public void SetContent (string text, bool useMnemonic, ImageDescription image, ContentPosition position)
		{
			if (string.IsNullOrEmpty (text)) {
				if (label != null) {
					label.Dispose ();
					label = null;
				}
			} else if (label != null)
				label.Text = text;
			else {
				label = new Label (text);
				var back = label.GetBackend () as LabelBackend;
				back.Parent = this;
				back.havePadding = false;
				back.Font = Font;
			}

			this.mnemonic = useMnemonic;
			this.image = image;
			this.pos = position;
			Invalidate ();
		}

		#endregion
	}
}

