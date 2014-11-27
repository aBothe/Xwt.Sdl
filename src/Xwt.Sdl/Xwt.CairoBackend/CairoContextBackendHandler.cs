// 
// ContextBackendHandler.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
//       Hywel Thomas <hywel.w.thomas@gmail.com>
// 
// Copyright (c) 2011 Xamarin Inc
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
using System.Collections.Generic;

namespace Xwt.CairoBackend
{
	public class CairoContextBackend : IDisposable
	{
		public const double Degrees = System.Math.PI / 180d;
		public const double Radiants = 180d / System.Math.PI;
		public double GlobalAlpha = 1;
		public Cairo.Context Context;
		public Cairo.Surface TempSurface;

		public double ScaleFactor = 1;
		public double PatternAlpha = 1;
		public readonly bool AllowContextDisposal;
		public virtual void Dispose() { 
			if (AllowContextDisposal) {
				Context.Dispose (); 
				TempSurface.Dispose ();
			}
		}
		public string Text;

		public double GlobalXOffset;
		public double GlobalYOffset;

		Stack<Tuple<double, double>> globalOffsetStack = new Stack<Tuple<double, double>>();

		Stack<Data> dataStack = new Stack<Data> ();

		struct Data {
			public double PatternAlpha;
			public string Text;
		}

		public CairoContextBackend (double scaleFactor, Cairo.Context ctxt, Cairo.Surface surf, bool allowDispose=false)
		{
			this.AllowContextDisposal = allowDispose;
			this.Context = ctxt;
			this.TempSurface = surf;
			ScaleFactor = scaleFactor;
		}

		public void Save ()
		{
			Context.Save ();
			dataStack.Push (new Data () {
				PatternAlpha = PatternAlpha,
				Text = Text
			});
		}

		public void Restore ()
		{
			Context.Restore ();
			var d = dataStack.Pop ();
			PatternAlpha = d.PatternAlpha;
			Text = d.Text;
		}

		public void Arc (double xc, double yc, double radius, double angle1_deg, double angle2_deg)
		{
			Context.Arc (xc+GlobalXOffset, yc+GlobalYOffset, radius, angle1_deg * Degrees, angle2_deg * Degrees);
		}

		public void ArcNegative (double xc, double yc, double radius, double angle1_deg, double angle2_deg)
		{
			Context.ArcNegative (xc+GlobalXOffset, yc+GlobalYOffset, radius, angle1_deg * Degrees, angle2_deg * Degrees);
		}

		public void CurveTo (double x1, double y1, double x2, double y2, double x3, double y3)
		{
			Context.CurveTo (x1+GlobalXOffset, y1+GlobalYOffset, x2+GlobalXOffset, y2+GlobalYOffset, x3+GlobalXOffset, y3+GlobalYOffset);
		}

		public void Fill ()
		{
			var alpha = GlobalAlpha * PatternAlpha;

			if (alpha == 1.0)
				Context.Fill ();
			else {
				Context.Save ();
				Context.Clip ();
				Context.PaintWithAlpha (alpha);
				Context.Restore ();
			}
		}

		public void FillPreserve ()
		{
			var alpha = GlobalAlpha * PatternAlpha;

			if (alpha == 1)
				Context.FillPreserve ();
			else {
				Context.Save ();
				Context.ClipPreserve ();
				Context.PaintWithAlpha (alpha);
				Context.Restore ();
			}
		}

		public void LineTo (double x, double y)
		{
			Context.LineTo (x+GlobalXOffset, y+GlobalYOffset);
		}

		public void MoveTo (double x, double y)
		{
			Context.MoveTo (x+GlobalXOffset, y+GlobalYOffset);
		}

		public void Rectangle (double x, double y, double width, double height)
		{
			Context.Rectangle (x+GlobalXOffset, y+GlobalYOffset, width, height);
		}

		public void Rectangle(Rectangle r)
		{
			Context.Rectangle (r.X + GlobalXOffset, r.Y + GlobalYOffset, r.Width, r.Height);
		}

		public void SetColor (Xwt.Drawing.Color color)
		{
			Context.SetSourceRGBA (color.Red, color.Green, color.Blue, color.Alpha * GlobalAlpha);
			PatternAlpha = 1;
		}

		public void SetPattern (object p)
		{
			/*if (p is ImagePatternBackend) {
				cb.PatternAlpha = ((ImagePatternBackend)p).Image.Alpha;
				p = ((ImagePatternBackend)p).GetPattern (ApplicationContext, ((CairoContextBackend)backend).ScaleFactor);
			} else*/
			PatternAlpha = 1;

			if (p != null)
				Context.SetSource ((Cairo.Pattern) p);
			else
				Context.SetSource ((Cairo.Pattern) null);
		}

		public void DrawTextLayout (TextLayout layout, double x, double y)
		{
			var be = (Xwt.CairoBackend.CairoTextLayoutBackendHandler.PangoBackend)layout.GetBackend();
			var pl = be.Layout;

			Context.MoveTo (x+GlobalXOffset, y+GlobalYOffset);

			if (layout.Height <= 0) {
				Pango.CairoHelper.ShowLayout (Context, pl);
			} else {
				var lc = pl.LineCount;
				var scale = Pango.Scale.PangoScale;
				double h = 0;
				for (int i=0; i<lc; i++) {
					var line = pl.Lines [i];
					var ext = new Pango.Rectangle ();
					var extl = new Pango.Rectangle ();
					line.GetExtents (ref ext, ref extl);
					h += (extl.Height / scale);
					if (h > layout.Height)
						break;
					Context.MoveTo (x, y + h);
					Pango.CairoHelper.ShowLayoutLine (Context, line);
				}
			}

			Context.MoveTo (x+GlobalXOffset, y+GlobalYOffset);
		}

		public void DrawImage (ImageDescription img, double x, double y)
		{
			img.Alpha *= GlobalAlpha;/*
			var pix = (Xwt.GtkBackend.GtkImage) img.Backend;

			pix.Draw (ApplicationContext, ctx.Context, ctx.ScaleFactor, x, y, img);*/
		}

		public void DrawImage (ImageDescription img, Rectangle srcRect, Rectangle destRect)
		{
			Context.Save ();
			Context.NewPath();
			Context.Rectangle (destRect.X, destRect.Y, destRect.Width, destRect.Height);
			Context.Clip ();
			Context.Translate (destRect.X-srcRect.X, destRect.Y-srcRect.Y);
			double sx = destRect.Width / srcRect.Width;
			double sy = destRect.Height / srcRect.Height;
			Context.Scale (sx, sy);
			img.Alpha *= GlobalAlpha;
			/*
			var pix = (Xwt.GtkBackend.GtkImage) img.Backend;
			pix.Draw (ApplicationContext, ctx.Context, ctx.ScaleFactor, 0, 0, img);*/
			Context.Restore ();
		}

		public void Rotate (double angle_deg)
		{
			Context.Rotate (angle_deg * Degrees);
		}

		public void Scale (double scaleX, double scaleY)
		{
			Context.Scale (scaleX, scaleY);
		}

		public void Translate (double tx, double ty)
		{
			Context.Translate (tx, ty);
		}

		public void ModifyCTM (Matrix m)
		{
			var t = new Cairo.Matrix (m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
			Context.Transform (t);
		}

		public Matrix GetCTM ()
		{
			var t = Context.Matrix;
			return new Matrix (t.Xx, t.Yx, t.Xy, t.Yy, t.X0, t.Y0);
		}

		public bool IsPointInFill (double x, double y)
		{
			return Context.InFill (x+GlobalXOffset, y+GlobalYOffset);
		}

		public bool IsPointInStroke (double x, double y)
		{
			return Context.InStroke (x+GlobalXOffset, y+GlobalYOffset);
		}
	}
	
	public class CairoContextBackendHandler: ContextBackendHandler
	{
		public override bool DisposeHandleOnUiThread {
			get {
				return true;
			}
		}

		#region IContextBackendHandler implementation

		public override double GetScaleFactor (object backend)
		{
			return (backend as CairoContextBackend).ScaleFactor;
		}

		public override void Save (object backend)
		{
			(backend as CairoContextBackend).Save ();
		}
		
		public override void Restore (object backend)
		{
			(backend as CairoContextBackend).Restore ();
		}
		
		public override void SetGlobalAlpha (object backend, double alpha)
		{
			(backend as CairoContextBackend).GlobalAlpha = alpha;
		}

		public override void Arc (object backend, double xc, double yc, double radius, double angle1, double angle2)
		{
			(backend as CairoContextBackend).Arc (xc, yc, radius, angle1, angle2);
		}

		public override void ArcNegative (object backend, double xc, double yc, double radius, double angle1, double angle2)
		{
			(backend as CairoContextBackend).ArcNegative (xc, yc, radius, angle1, angle2);
		}

		public override void Clip (object backend)
		{
			Cairo.Context ctx = ((CairoContextBackend) backend).Context;
			ctx.Clip ();
		}

		public override void ClipPreserve (object backend)
		{
			Cairo.Context ctx = ((CairoContextBackend) backend).Context;
			ctx.ClipPreserve ();
		}

		public override void ClosePath (object backend)
		{
			Cairo.Context ctx = ((CairoContextBackend) backend).Context;
			ctx.ClosePath ();
		}

		public override void CurveTo (object backend, double x1, double y1, double x2, double y2, double x3, double y3)
		{
			(backend as CairoContextBackend).CurveTo (x1, y1, x2, y2, x3, y3);
		}

		public override void Fill (object backend)
		{
			(backend as CairoContextBackend).Fill ();
		}

		public override void FillPreserve (object backend)
		{
			(backend as CairoContextBackend).FillPreserve ();
		}

		public override void LineTo (object backend, double x, double y)
		{
			(backend as CairoContextBackend).LineTo (x, y);
		}

		public override void MoveTo (object backend, double x, double y)
		{
			(backend as CairoContextBackend).MoveTo (x, y);
		}

		public override void NewPath (object backend)
		{
			Cairo.Context ctx = ((CairoContextBackend) backend).Context;
			ctx.NewPath ();
		}

		public override void Rectangle (object backend, double x, double y, double width, double height)
		{
			(backend as CairoContextBackend).Rectangle (x, y, width, height);
		}

		public override void RelCurveTo (object backend, double dx1, double dy1, double dx2, double dy2, double dx3, double dy3)
		{
			(backend as CairoContextBackend).Context.RelCurveTo (dx1, dy1, dx2, dy2, dx3, dy3);
		}

		public override void RelLineTo (object backend, double dx, double dy)
		{
			(backend as CairoContextBackend).Context.RelLineTo (dx, dy);
		}

		public override void RelMoveTo (object backend, double dx, double dy)
		{
			(backend as CairoContextBackend).Context.RelMoveTo (dx, dy);
		}

		public override void Stroke (object backend)
		{
			(backend as CairoContextBackend).Context.Stroke ();
		}

		public override void StrokePreserve (object backend)
		{
			(backend as CairoContextBackend).Context.StrokePreserve ();
		}

		public override void SetColor (object backend, Xwt.Drawing.Color color)
		{
			(backend as CairoContextBackend).SetColor (color);
		}
		
		public override void SetLineWidth (object backend, double width)
		{
			(backend as CairoContextBackend).Context.LineWidth = width;
		}
		
		public override void SetLineDash (object backend, double offset, params double[] pattern)
		{
			(backend as CairoContextBackend).Context.SetDash (pattern, offset);
		}
		
		public override void SetPattern (object backend, object p)
		{
			(backend as CairoContextBackend).SetPattern (p);
		}
		
		public override void DrawTextLayout (object backend, TextLayout layout, double x, double y)
		{
			(backend as CairoContextBackend).DrawTextLayout (layout, x, y);
		}

		public override void DrawImage (object backend, ImageDescription img, double x, double y)
		{
			(backend as CairoContextBackend).DrawImage (img, x, y);
		}

		public override void DrawImage (object backend, ImageDescription img, Xwt.Rectangle srcRect, Xwt.Rectangle destRect)
		{
			(backend as CairoContextBackend).DrawImage (img, srcRect, destRect);
		}
		
		protected virtual Size GetImageSize (object img)
		{
			return new Size (0,0);
		}
		
		public override void Rotate (object backend, double angle)
		{
			(backend as CairoContextBackend).Rotate (angle);
		}
		
		public override void Scale (object backend, double scaleX, double scaleY)
		{
			(backend as CairoContextBackend).Scale (scaleX, scaleY);
		}

		public override void Translate (object backend, double tx, double ty)
		{
			(backend as CairoContextBackend).Translate (tx, ty);
		}

		public override void ModifyCTM (object backend, Matrix m)
		{
			(backend as CairoContextBackend).ModifyCTM (m);
		}

		public override Matrix GetCTM (object backend)
		{
			return (backend as CairoContextBackend).GetCTM ();
		}

		public override object CreatePath ()
		{
			Cairo.Surface sf = new Cairo.ImageSurface (null, Cairo.Format.A1, 0, 0, 0);
			return new CairoContextBackend (1, new Cairo.Context (sf), sf, true); // scale doesn't matter here, we are going to use it only for creating a path
		}

		public override object CopyPath (object backend)
		{
			var newPath = CreatePath ();
			AppendPath (newPath, backend);
			return newPath;
		}

		public override void AppendPath (object backend, object otherBackend)
		{
			Cairo.Context dest = ((CairoContextBackend)backend).Context;
			Cairo.Context src = ((CairoContextBackend)otherBackend).Context;

			using (var path = src.CopyPath ())
				dest.AppendPath (path);
		}

		public override bool IsPointInFill (object backend, double x, double y)
		{
			return (backend as CairoContextBackend).IsPointInFill (x, y);
		}

		public override bool IsPointInStroke (object backend, double x, double y)
		{
			return (backend as CairoContextBackend).IsPointInStroke (x, y);
		}

		public override void Dispose (object backend)
		{
			(backend as CairoContextBackend).Dispose ();
		}

		#endregion
	}
}

