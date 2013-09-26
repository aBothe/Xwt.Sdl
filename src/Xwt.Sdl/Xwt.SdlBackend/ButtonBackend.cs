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
using OpenTK.Graphics.OpenGL;
using FTGL;

namespace Xwt.Sdl
{
	public class ButtonBackend : WidgetBackend, IButtonBackend
	{
		#region Properties
		ButtonStyle style;
		ButtonType type;
		string label = "Button";
		bool mnemonic;
		ImageDescription image;
		ContentPosition pos;
		public new IButtonEventSink EventSink {get{return base.EventSink as IButtonEventSink;}}

		bool clicked;
		bool hovered;
		float v,w;
		#endregion

		public override void Draw (Rectangle dirtyRect)
		{
			if (clicked)
				GL.Color3 (0.5f, .5f, .5f);
			else if (hovered)
				GL.Color3 (0.8f, 0.8f, 0.8f);
			else
				GL.Color3 (0.6f, 0.6f, 0.6f);

			GL.Rect (X, Y, X + Width, Y+ Height);

			if (label != null) {
				var font = FontBackend;
				GL.PushMatrix ();
				GL.Translate (X + Width/2.0 - font.GetAdvance (label), Y + Height/2.0 - font.FontSize/2.0, 0.0);
				GL.Rotate (180f, 1f, 0f, 0f); 
				GL.Color4 (0f, 0f, 0f,1f);
				font.Render (label);
				GL.PopMatrix ();
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

