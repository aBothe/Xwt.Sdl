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

		bool hovered;
		float v,w;
		#endregion

		static IntPtr font;
		static ButtonBackend()
		{
			font = Fonts.CreateTextureFont("/usr/share/fonts/TTF/SourceSansPro-Regular.ttf");
			GL.Enable (EnableCap.ColorMaterial);
			GL.Enable (EnableCap.Blend);
			GL.Enable (EnableCap.Texture2D);

			if(font == IntPtr.Zero)
				throw new FTGLException();

			if (Fonts.SetFontFaceSize(font, 15) != 1)
				throw new FTGLException ();

			var err = Fonts.GetFontError (font);

			if (err != IntPtr.Zero)
				throw new FTGLException ();
		}

		~ButtonBackend()
		{
			//Fonts.DestroyFont(font);
		}


		public override void Draw (Rectangle dirtyRect)
		{
			GL.Color3 (0f, w, v);
			GL.Rect (X, Y, Width, Height);

			if (label != null) {
				GL.PushMatrix ();
				var abs = AbsoluteLocation;
				GL.Translate (0.0, Fonts.GetFontFaceSize(font), 0.0);
				GL.Rotate (180f, 1f, 0f, 0f); 
				GL.Color4 (1f, 0f, 0f,1f);
				Fonts.RenderFont (font, label, RenderMode.Front);
				GL.PopMatrix ();
			}
			//base.Draw (dirtyRect);
		}

		internal override void FireMouseEnter ()
		{
			hovered = true;
			base.FireMouseEnter ();
			Invalidate ();
		}

		internal override void FireMouseLeave ()
		{
			hovered = false;
			base.FireMouseLeave ();
			Invalidate ();
		}

		internal override bool FireMouseButton (bool down, PointerButton butt, int x, int y, int multiplePress = 1)
		{
			var ret = base.FireMouseButton (down, butt, x, y, multiplePress);
			EventSink.OnClicked ();
			return ret;
		}

		internal override bool FireMouseMoved (uint timestamp, int x, int y)
		{
			v = x / (float)Width;
			w = y / (float)Height;
			Invalidate ();
			return base.FireMouseMoved (timestamp, x, y);
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

