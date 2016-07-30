using System;
using System.Drawing;

namespace Xwt.Sdl
{
	public class ImageBackend
	{
		public Bitmap Bitmap { get; private set; }

		public ImageBackend (Bitmap bitmap)
		{
			this.Bitmap = bitmap;
		}
	}
}

