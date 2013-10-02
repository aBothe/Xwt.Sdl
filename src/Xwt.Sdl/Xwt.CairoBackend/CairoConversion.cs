// 
// CairoConversion.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc
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
using Xwt.Drawing;

namespace Xwt.CairoBackend
{
	public static class CairoConversion
	{
		public static Cairo.Rectangle ToCairoRectangle(this Rectangle rect)
		{
			return new Cairo.Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Rectangle ToXwtRectangle(this Cairo.Rectangle rect)
		{
			return new Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Cairo.Color ToCairoColor (this Color col)
		{
			return new Cairo.Color (col.Red, col.Green, col.Blue, col.Alpha);
		}

		internal static void SelectFont(this Cairo.Context c, InternalFontDescription f)
		{
			Cairo.FontSlant slant;
			switch (f.Style) {
				case FontStyle.Oblique: slant = Cairo.FontSlant.Oblique; break;
				case FontStyle.Italic: slant = Cairo.FontSlant.Italic; break;
				default: slant = Cairo.FontSlant.Normal; break;
			}

			Cairo.FontWeight w = f.Weight >= FontWeight.Bold ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal;

			c.SelectFontFace (f.Family, slant, w);
			c.SetFontSize (f.Scale);
		}
		
		public static void SelectFont (this Cairo.Context ctx, Font font)
		{
			Cairo.FontSlant slant;
			switch (font.Style) {
			case FontStyle.Oblique: slant = Cairo.FontSlant.Oblique; break;
			case FontStyle.Italic: slant = Cairo.FontSlant.Italic; break;
			default: slant = Cairo.FontSlant.Normal; break;
			}
			
			Cairo.FontWeight w = font.Weight >= FontWeight.Bold ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal;

			ctx.SelectFontFace (font.Family, slant, w);
		}
	}
}

