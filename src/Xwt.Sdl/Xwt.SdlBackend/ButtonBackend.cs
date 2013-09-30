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
		bool hovered;
		float v,w;
		#endregion

		public override void Draw (CairoBackend.CairoContextBackend c,Rectangle dirtyRect)
		{
			// Generic background?
			base.Draw (c, dirtyRect);

			// Border
			{
				var Y = this.Y + 1;
				var X = this.X + 1;
				var Width = this.Width - 2;
				var Height = this.Height - 2;

				const double borderColor = 0.6;
				const double radius = 3;

				// Top left corner
				c.Context.Arc (X +radius, Y + radius, radius, -Math.PI, -Math.PI/2);

				// top edge
				c.Context.RelLineTo (Width - (radius*2), 0);

				// top right corner
				c.Context.Arc (X + Width -radius, Y + radius, radius, -Math.PI/2, 0);

				// left edge
				c.Context.RelLineTo (0, Height-radius*2);

				// bottom right corner
				c.Context.Arc (X + Width -radius, Y + Height - radius, radius, 0, Math.PI/2);

				// bottom corner
				c.Context.RelLineTo (-Width+(radius*2), 0);

				// bottom left corner
				c.Context.Arc (X + radius, Y + Height - radius, radius, Math.PI/2, Math.PI);

				c.Context.ClosePath ();
				c.Context.SetSourceRGB (borderColor, borderColor, borderColor);
				c.Context.StrokePreserve ();
			}

			// button background
			{
				double grey;
				if (clicked)
					grey = 0.9;
				else if (hovered)
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

			// Label
			if (label != null) {
				c.Context.SetSourceRGB (0, 0, 0);
				var ext = c.Context.TextExtents (label);
				c.Context.MoveTo (X + Width/2.0 -  ext.Width/2.0d, Y + Height/2.0d + ext.Height/2.5d);
				c.Context.ShowText (label);
			}
		}

		internal override void FireMouseEnter ()
		{
			hovered = Sensitive;
			clicked = false;
			base.FireMouseEnter ();
			Invalidate ();
		}

		internal override void FireMouseLeave ()
		{
			hovered = false;
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

		public override Size GetPreferredSize (Cairo.Context c)
		{
			var ext = c.TextExtents (label ?? string.Empty);
			var x = ext.Width + 5;
			var y = (!string.IsNullOrEmpty (label) ? ext.Height : 0.0d) + 10d;
			return new Size (x, y);
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

