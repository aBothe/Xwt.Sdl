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
	class InternalFontDescription
	{
		public string Family;
		public Xwt.Drawing.FontWeight Weight;
		public FontSlant Slant;
		public FontStyle Style;
		public double Scale;
		public FontStretch Stretch;

		public InternalFontDescription Clone()
		{
			return new InternalFontDescription{ 
				Family = Family, 
				Scale = Scale, 
				Style = Style,
				Weight = Weight,
				Stretch = Stretch,
				Slant = Slant
			};
		}
	}

	public class CairoFontBackendHandler : FontBackendHandler
	{
		internal static InternalFontDescription ToFontDescription(System.Drawing.Font ft)
		{
			var fd = new InternalFontDescription ();

			fd.Family = ft.FontFamily.Name;
			fd.Scale = (double)ft.Size;

			if (ft.Bold) {
				fd.Slant |= FontSlant.Oblique;
				fd.Style |= FontStyle.Oblique;
			}

			if (ft.Italic) {
				fd.Slant |= FontSlant.Italic;
				fd.Style |= FontStyle.Italic;
			}

			// TODO: Font weights?

			return fd;
		}

		#region implemented abstract members of FontBackendHandler
		static InternalFontDescription sysFont;
		internal static InternalFontDescription SystemDefaultFont
		{
			get{ 
				if(sysFont != null)
					return sysFont;

				return ToFontDescription (System.Drawing.SystemFonts.CaptionFont);
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
			return new InternalFontDescription{ Family = fontName, Scale = size, Style = style, Weight = weight, Stretch = stretch };
		}

		public override object Copy (object handle)
		{
			return copy(handle);
		}

		static InternalFontDescription copy (object handle)
		{
			return (handle as InternalFontDescription).Clone ();
		}

		public override object SetSize (object handle, double size)
		{
			var f = copy (handle);
			f.Scale = size;
			return f;
		}

		public override object SetFamily (object handle, string family)
		{
			var f = copy (handle);
			f.Family = family;
			return f;
		}

		public override object SetStyle (object handle, FontStyle style)
		{
			var f = copy (handle);
			f.Style = style;
			return f;
		}

		public override object SetWeight (object handle, Xwt.Drawing.FontWeight weight)
		{
			var f = copy (handle);
			f.Weight = weight;
			return f;
		}

		public override object SetStretch (object handle, FontStretch stretch)
		{
			var f = copy (handle);
			f.Stretch = stretch;
			return f;
		}

		public override double GetSize (object handle)
		{
			return (handle as InternalFontDescription).Scale;
		}

		public override string GetFamily (object handle)
		{
			return (handle as InternalFontDescription).Family;
		}

		public override FontStyle GetStyle (object handle)
		{
			return (handle as InternalFontDescription).Style;
		}

		public override Xwt.Drawing.FontWeight GetWeight (object handle)
		{
			return (handle as InternalFontDescription).Weight;
		}

		public override FontStretch GetStretch (object handle)
		{
			return (handle as InternalFontDescription).Stretch;
		}

		#endregion
	}
}

