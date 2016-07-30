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
		#region implemented abstract members of ImageBackendHandler
		public override object LoadFromStream (Stream stream)
		{
			return new ImageBackend(new System.Drawing.Bitmap (stream));
		}
		public override void Dispose (object backend)
		{
			(backend as ImageBackend).Bitmap.Dispose ();
		}

		public override void SaveToStream (object backend, Stream stream, ImageFileType fileType)
		{
			var bmp = backend as System.Drawing.Bitmap;

			System.Drawing.Imaging.ImageFormat fmt;
			switch (fileType) {
				case ImageFileType.Bmp:
					fmt = System.Drawing.Imaging.ImageFormat.Bmp;
					break;
				case ImageFileType.Jpeg:
					fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
					break;
				case ImageFileType.Png:
					fmt = System.Drawing.Imaging.ImageFormat.Png;
					break;
				default:
					throw new ArgumentOutOfRangeException ("fileType", "Invalid image type!");
			}
			bmp.Save (stream, fmt);
		}
		public override Image GetStockIcon (string id)
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
			var img = (handle as ImageBackend).Bitmap;
			return new Size (img.Width, img.Height);
		}
		public override object CopyBitmap (object handle)
		{
			return (handle as ImageBackend).Bitmap.Clone ();
		}
		public override void CopyBitmapArea (object srcHandle, int srcX, int srcY, int width, int height, object destHandle, int destX, int destY)
		{
			throw new NotImplementedException ();
		}
		public override object CropBitmap (object handle, int srcX, int srcY, int width, int height)
		{
			throw new NotImplementedException ();
		}
		public override void SetBitmapPixel (object handle, int x, int y, Xwt.Drawing.Color color)
		{
			(handle as ImageBackend).Bitmap.SetPixel (x, y,
				System.Drawing.Color.FromArgb(
					(int)(color.Alpha*255.0),
					(int)(color.Red*255.0),
					(int)(color.Green*255.0),
					(int)(color.Blue*255.0)));
		}
		public override Xwt.Drawing.Color GetBitmapPixel (object handle, int x, int y)
		{
			var col = (handle as ImageBackend).Bitmap.GetPixel (x, y);
			return new Xwt.Drawing.Color ((double)col.R/255.0, (double)col.G, (double)col.B/255.0, (double)col.A/255.0);
		}

		public override object ConvertToBitmap (ImageDescription idesc, double scaleFactor, ImageFormat format)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

