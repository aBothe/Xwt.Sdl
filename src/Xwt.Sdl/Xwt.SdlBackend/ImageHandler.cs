//
// ImageHandler.cs
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
using System.IO;

namespace Xwt.Sdl
{
	public class ImageHandler : ImageBackendHandler
	{
		public ImageHandler ()
		{
		}

		#region implemented abstract members of ImageBackendHandler
		public override object LoadFromStream (System.IO.Stream stream)
		{
			throw new NotImplementedException ();
		}
		public override void SaveToStream (object backend, Stream stream, Xwt.Drawing.ImageFileType fileType)
		{
			throw new NotImplementedException ();
		}
		public override Image GetStockIcon (string id)
		{
			throw new NotImplementedException ();
		}
		public override object ConvertToBitmap (object handle, double width, double height, double scaleFactor, ImageFormat format)
		{
			throw new NotImplementedException ();
		}
		public override bool IsBitmap (object handle)
		{
			throw new NotImplementedException ();
		}
		public override bool HasMultipleSizes (object handle)
		{
			throw new NotImplementedException ();
		}
		public override Size GetSize (object handle)
		{
			throw new NotImplementedException ();
		}
		public override object CopyBitmap (object handle)
		{
			throw new NotImplementedException ();
		}
		public override void CopyBitmapArea (object srcHandle, int srcX, int srcY, int width, int height, object destHandle, int destX, int destY)
		{
			throw new NotImplementedException ();
		}
		public override object CropBitmap (object handle, int srcX, int srcY, int width, int height)
		{
			throw new NotImplementedException ();
		}
		public override void SetBitmapPixel (object handle, int x, int y, Color color)
		{
			throw new NotImplementedException ();
		}
		public override Color GetBitmapPixel (object handle, int x, int y)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

