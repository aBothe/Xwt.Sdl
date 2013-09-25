//
// FontWrapper.cs
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
using System.IO;
using System.Collections.Generic;

namespace FTGL
{
	public enum FontKind
	{
		Pixmap,
		Texture
	}

	public class FontWrapper : IDisposable
	{
		class FontEntry : IDisposable
		{
			public readonly IntPtr Handle;
			public int ReferenceCount;
			public readonly bool DisposeAfterLastRef;

			public FontEntry(IntPtr h, bool disp)
			{
				Handle = h;
				DisposeAfterLastRef = disp;
				ReferenceCount = 1;
			}

			public virtual void Dispose ()
			{
				if(DisposeAfterLastRef)
					Fonts.DestroyFont(Handle);
			}
		}

		static Dictionary<int, FontEntry> fontCache = new Dictionary<int, FontEntry>();

		readonly int Hash;
		public override int GetHashCode (){return Hash;}
		readonly IntPtr font;
		readonly bool disp;
		uint RenderSize;

		public uint Size
		{
			get{ 
				return RenderSize != 0 ? RenderSize : Fonts.GetFontFaceSize (font);
			}
			set{
				RenderSize= value;
			}
		}

		void SetRenderSize()
		{
			Fonts.SetFontFaceSize (font, RenderSize);
		}

		public float LineHeight
		{
			get{
				SetRenderSize ();
				return Fonts.GetFontLineHeight (font);
			}
		}

		public float GetAdvance(string text)
		{
			SetRenderSize ();
			return Fonts.GetFontAdvance (font, text);
		}

		public FontBoundaries GetBoundaries(string text)
		{
			SetRenderSize ();
			return Fonts.GetFontBoundaryBox (font, text);
		}

		public void Render(string text, RenderMode mode = RenderMode.Front)
		{
			if (text != null) {
				SetRenderSize ();
				Fonts.RenderFont (font, text, mode);
			}
		}

		public static int CalculateHashCode(string fontFace, FontKind kind)
		{
			return fontFace.GetHashCode () + ((byte)kind << 24);
		}

		public static FontWrapper LoadFile(string pathToFont, FontKind kind = FontKind.Texture)
		{
			if (!File.Exists (pathToFont))
				throw new FileNotFoundException (pathToFont + " could not be found!");

			var hash = CalculateHashCode (pathToFont, kind);
			FontEntry fe;
			if (fontCache.TryGetValue (hash, out fe)) {
				fe.ReferenceCount++;
				return new FontWrapper (hash,fe.Handle,fe.DisposeAfterLastRef);
			}

			IntPtr font;
			bool disp;

			switch (kind) {
				case FontKind.Pixmap:
					disp = true;
					font = Fonts.CreatePixmapFont (pathToFont);
					break;
				default:
				case FontKind.Texture:
					font = Fonts.CreateTextureFont (pathToFont);
					disp = false;
					break;
			}

			if (font == IntPtr.Zero)
				throw new FTGLException (IntPtr.Zero);

			fontCache [hash] = new FontEntry(font, disp);
			return new FontWrapper (hash, font, disp);
		}

		private FontWrapper(int hash,IntPtr font, bool disposeOnDestroy=true)
		{
			this.Hash = hash;
			this.disp = disposeOnDestroy;
			this.font = font;
		}

		~FontWrapper()
		{
			Dispose ();
		}

		bool disposed;
		public virtual void Dispose ()
		{
			if (disp && !disposed) {
				disposed = true;
				var fe = fontCache [Hash];
				if (--fe.ReferenceCount <= 0) {
					fe.Dispose ();
					fontCache.Remove (Hash);
				}
			}
		}
	}
}
