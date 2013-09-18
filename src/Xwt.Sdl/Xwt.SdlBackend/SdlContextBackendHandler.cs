//
// ContextBackendHandler.cs
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
	public class SdlContextBackendHandler : ContextBackendHandler
	{
		public SdlContextBackendHandler ()
		{
		}

		#region implemented abstract members of DrawingPathBackendHandler

		public override void Arc (object backend, double xc, double yc, double radius, double angle1, double angle2)
		{
			// http://www.opengl.org/discussion_boards/showthread.php/167955-drawing-a-smooth-circle
			throw new NotImplementedException ();
		}

		public override void ArcNegative (object backend, double xc, double yc, double radius, double angle1, double angle2)
		{
			throw new NotImplementedException ();
		}

		public override void ClosePath (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void CurveTo (object backend, double x1, double y1, double x2, double y2, double x3, double y3)
		{
			throw new NotImplementedException ();
		}

		public override void LineTo (object backend, double x, double y)
		{
			throw new NotImplementedException ();
		}

		public override void MoveTo (object backend, double x, double y)
		{
			throw new NotImplementedException ();
		}

		public override void Rectangle (object backend, double x, double y, double width, double height)
		{
			throw new NotImplementedException ();
		}

		public override void RelCurveTo (object backend, double dx1, double dy1, double dx2, double dy2, double dx3, double dy3)
		{
			throw new NotImplementedException ();
		}

		public override void RelLineTo (object backend, double dx, double dy)
		{
			throw new NotImplementedException ();
		}

		public override void RelMoveTo (object backend, double dx, double dy)
		{
			throw new NotImplementedException ();
		}

		public override object CreatePath ()
		{
			throw new NotImplementedException ();
		}

		public override object CopyPath (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void AppendPath (object backend, object otherBackend)
		{
			throw new NotImplementedException ();
		}

		public override bool IsPointInFill (object backend, double x, double y)
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region implemented abstract members of ContextBackendHandler

		public override void Save (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void Restore (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void Clip (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void ClipPreserve (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void Fill (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void FillPreserve (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void NewPath (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void Stroke (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void StrokePreserve (object backend)
		{
			throw new NotImplementedException ();
		}

		public override void SetColor (object backend, Xwt.Drawing.Color color)
		{
			throw new NotImplementedException ();
		}

		public override void SetLineWidth (object backend, double width)
		{
			throw new NotImplementedException ();
		}

		public override void SetLineDash (object backend, double offset, params double[] pattern)
		{
			throw new NotImplementedException ();
		}

		public override void SetPattern (object backend, object p)
		{
			throw new NotImplementedException ();
		}

		public override void DrawTextLayout (object backend, TextLayout layout, double x, double y)
		{
			throw new NotImplementedException ();
		}

		public override void DrawImage (object backend, ImageDescription img, double x, double y)
		{
			throw new NotImplementedException ();
		}

		public override void DrawImage (object backend, ImageDescription img, Rectangle srcRect, Rectangle destRect)
		{
			throw new NotImplementedException ();
		}

		public override void Rotate (object backend, double angle)
		{
			throw new NotImplementedException ();
		}

		public override void Scale (object backend, double scaleX, double scaleY)
		{
			throw new NotImplementedException ();
		}

		public override void Translate (object backend, double tx, double ty)
		{
			throw new NotImplementedException ();
		}

		public override void ModifyCTM (object backend, Matrix transform)
		{
			throw new NotImplementedException ();
		}

		public override Xwt.Drawing.Matrix GetCTM (object backend)
		{
			throw new NotImplementedException ();
		}

		public override bool IsPointInStroke (object backend, double x, double y)
		{
			throw new NotImplementedException ();
		}

		public override void SetGlobalAlpha (object backend, double globalAlpha)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

