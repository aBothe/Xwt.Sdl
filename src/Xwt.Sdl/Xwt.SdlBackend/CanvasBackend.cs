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
		List<WidgetBackend> children = new List<WidgetBackend>();
		public new ICanvasEventSink EventSink {get{return base.EventSink as ICanvasEventSink; }}

		public override void Draw (CairoContextBackend c,Rectangle dirtyRect)
		{
			base.Draw (c,dirtyRect);

			// Draw actual content
			EventSink.OnDraw (c, new Rectangle(0,0, Width, Height));

			// Draw child widgets
			foreach (var ch in children)
				if (ch.Bounds.IntersectsWith (dirtyRect)) {
					c.PushGlobalOffset ();
					c.GlobalXOffset += ch.X;
					c.GlobalYOffset += ch.Y;
					c.Context.MoveTo (c.GlobalXOffset, c.GlobalYOffset);
					ch.Draw (c, dirtyRect);
					c.PopGlobalOffset ();
				}
			//c.Restore ();
		}

		public override WidgetBackend GetChildAt (double x, double y)
		{
			foreach (var ch in children)
				if (ch.X >= x && ch.Y >= y && x <= ch.X + ch.Width && y <= ch.Y + ch.Height)
					return ch;
			return null;
		}

		#region ICanvasBackend implementation

		public void QueueDraw ()
		{
			Invalidate ();
		}

		public void QueueDraw (Rectangle rect)
		{
			Invalidate (rect);
		}

		public void AddChild (IWidgetBackend widget, Rectangle bounds)
		{
			var w = widget as WidgetBackend;
			w.Parent = this;
			w.OnBoundsChanged (bounds.X, bounds.Y, bounds.Width, bounds.Height);
			children.Add (w);
			Invalidate (bounds);
		}

		public void SetChildBounds (IWidgetBackend widget, Rectangle bounds)
		{
			(widget as WidgetBackend).OnBoundsChanged (bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}

		public void RemoveChild (IWidgetBackend widget)
		{
			var w = widget as WidgetBackend;
			children.Remove (w);
		}

		#endregion
	}
}

