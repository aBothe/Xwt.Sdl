//
// SdlTextLayoutBackendHandler.cs
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

namespace Xwt.Sdl
{
	public class SdlTextLayoutBackendHandler : TextLayoutBackendHandler
	{
		public SdlTextLayoutBackendHandler ()
		{
		}

		#region implemented abstract members of TextLayoutBackendHandler

		public override object Create ()
		{
			throw new NotImplementedException ();
		}

		public override void SetWidth (object backend, double value)
		{
			throw new NotImplementedException ();
		}

		public override void SetHeight (object backend, double value)
		{
			throw new NotImplementedException ();
		}

		public override void SetText (object backend, string text)
		{
			throw new NotImplementedException ();
		}

		public override void SetFont (object backend, Font font)
		{
			throw new NotImplementedException ();
		}

		public override void SetTrimming (object backend, Xwt.Drawing.TextTrimming textTrimming)
		{
			throw new NotImplementedException ();
		}

		public override Size GetSize (object backend)
		{
			throw new NotImplementedException ();
		}

		public override int GetIndexFromCoordinates (object backend, double x, double y)
		{
			throw new NotImplementedException ();
		}

		public override Point GetCoordinateFromIndex (object backend, int index)
		{
			throw new NotImplementedException ();
		}

		public override void AddAttribute (object backend, TextAttribute attribute)
		{
			throw new NotImplementedException ();
		}

		public override void ClearAttributes (object backend)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

