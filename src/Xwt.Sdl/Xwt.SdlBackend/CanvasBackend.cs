//
// CanvasBackend.cs
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
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using Xwt.CairoBackend;

namespace Xwt.Sdl
{
	public class CanvasBackend : WidgetBackend, ICanvasBackend
	{
		public Canvas CanvasFrontend {get{ return Frontend as Canvas; }}

		public override IEnumerable<WidgetBackend> Children {
			get {
				foreach (var w in CanvasFrontend.Children)
					yield return w.GetBackend () as WidgetBackend;
			}
		}

		public new ICanvasEventSink EventSink {get{return base.EventSink as ICanvasEventSink; }}

		protected override void DrawInternally (CairoContextBackend c,Rectangle dirtyRect, double absX, double absY)
		{
			base.DrawInternally (c,dirtyRect, absX, absY);

			// Draw actual content
			c.GlobalXOffset += absX;
			c.GlobalYOffset += absY;

			EventSink.OnDraw (c,new Rectangle (
				Math.Max (0d, dirtyRect.X - absX),
				Math.Max (0d, dirtyRect.Y - absY), 
				Math.Min (Width, dirtyRect.Width),
				Math.Min (Height, dirtyRect.Height)));

			c.GlobalXOffset -= absX;
			c.GlobalYOffset -= absY;
			
			// Draw child widgets
			foreach (var ch in Children){
				ch.Draw (c, dirtyRect, false);
			}
		}

		#region ICanvasBackend implementation

		public void QueueDraw ()
		{
			Invalidate ();
		}

		public void QueueDraw (Rectangle rect)
		{
			Invalidate (rect.Offset(AbsoluteLocation));
		}

		public void AddChild (IWidgetBackend widget, Rectangle bounds)
		{
			var w = widget as WidgetBackend;
			w.Parent = this;
			w.OnBoundsChanged (bounds.X, bounds.Y, bounds.Width, bounds.Height);

			Invalidate (bounds);
		}

		public void SetChildBounds (IWidgetBackend widget, Rectangle bounds)
		{
			(widget as WidgetBackend).OnBoundsChanged (bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}

		public void RemoveChild(IWidgetBackend w)
		{
			Invalidate ();
		}

		#endregion
	}
}

