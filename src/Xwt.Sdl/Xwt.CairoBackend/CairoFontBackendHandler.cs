//
// SdlFontBackendHandler.cs
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
using Xwt.Drawing;
using Cairo;
using System.Collections.Generic;

namespace Xwt.CairoBackend
{
	class FontHandle
	{
		public FontFace Face;

	}

	public class CairoFontBackendHandler : FontBackendHandler
	{
		public CairoFontBackendHandler ()
		{

		}

		#region implemented abstract members of FontBackendHandler
		static FontFace sysFont;
		public static FontFace SystemDefaultFont
		{
			get{ 
				if(sysFont != null)
					return sysFont;

				return null;
				/*
				sysFont =  ("/usr/share/fonts/TTF/SourceSansPro-Regular.ttf");
				sysFont.FontSize = 13;
				return sysFont;*/
			}
		}

		public override object GetSystemDefaultFont ()
		{
			return SystemDefaultFont;
		}

		public override IEnumerable<string> GetInstalledFonts ()
		{
			throw new NotImplementedException ();
		}

		public override object Create (string fontName, double size, FontStyle style, Xwt.Drawing.FontWeight weight, FontStretch stretch)
		{
			throw new NotImplementedException ();
		}

		public override object Copy (object handle)
		{
			throw new NotImplementedException ();
		}

		public override object SetSize (object handle, double size)
		{
			throw new NotImplementedException ();
		}

		public override object SetFamily (object handle, string family)
		{
			throw new NotImplementedException ();
		}

		public override object SetStyle (object handle, FontStyle style)
		{
			throw new NotImplementedException ();
		}

		public override object SetWeight (object handle, Xwt.Drawing.FontWeight weight)
		{
			throw new NotImplementedException ();
		}

		public override object SetStretch (object handle, FontStretch stretch)
		{
			throw new NotImplementedException ();
		}

		public override double GetSize (object handle)
		{
			throw new NotImplementedException ();
		}

		public override string GetFamily (object handle)
		{
			throw new NotImplementedException ();
		}

		public override FontStyle GetStyle (object handle)
		{
			throw new NotImplementedException ();
		}

		public override Xwt.Drawing.FontWeight GetWeight (object handle)
		{
			throw new NotImplementedException ();
		}

		public override FontStretch GetStretch (object handle)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

