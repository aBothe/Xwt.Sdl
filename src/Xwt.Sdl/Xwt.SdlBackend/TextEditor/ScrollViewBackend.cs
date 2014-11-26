﻿//
// ScrollViewBackend.cs
//
// Author:
//       Alexander Bothe <info@alexanderbothe.com>
//
// Copyright (c) 2014 Alexander Bothe
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
using Xwt.CairoBackend;

namespace Xwt.Sdl
{
	public class ScrollViewBackend : WidgetBackend, IScrollViewBackend, IScrollAdjustmentEventSink
	{
		#region Properties
		WidgetBackend child;
		public override System.Collections.Generic.IEnumerable<WidgetBackend> Children {
			get {
				if(child != null)
					yield return child;
				if (VScrollbar.Visible)
					yield return VScrollbar;
				if (HScrollbar.Visible)
					yield return HScrollbar;
			}
		}
		Size childSize;
		static double scrollbarWidth {get{ return WidgetStyles.Instance.ScrollbarWidth; }}
		readonly ScrollBarBackend VScrollbar, HScrollbar;
		ScrollPolicy vScrollPolicy, hScrollPolicy;
		bool showBorder = false;
		double VisualChildWidth { get{ return Math.Min(Width, childSize.Width) - (VScrollbar.Visible ? scrollbarWidth : 0); } }
		double VisualChildHeight { get{ return Math.Min(Height, childSize.Height) - (HScrollbar.Visible ? scrollbarWidth : 0); } }

		public Rectangle VisibleRect
		{
			get{
				double dx, dy, vx = VisualChildWidth, vy = VisualChildHeight;

				dx = (HScrollbar.Value*0.01) * (childSize.Width-vx);
				dy = (VScrollbar.Value*0.01) * (childSize.Height-vy);

				return new Rectangle(dx,dy,vx, vy);
			}
		}
		#endregion

		public ScrollViewBackend ()
		{
			VScrollbar = new ScrollBarBackend ();
			HScrollbar = new ScrollBarBackend ();

			VScrollbar.Initialize (Orientation.Vertical);
			HScrollbar.Initialize (Orientation.Horizontal);

			VScrollbar.Initialize (this);
			HScrollbar.Initialize (this);

			VScrollbar.Parent = this;
			HScrollbar.Parent = this;
		}

		public void SetChild (IWidgetBackend c)
		{
			if (c == null)
				return;

			this.child = c as WidgetBackend;
			if(c != null)
				child.Parent = this;

			RealignEverything ();
		}

		public void SetChildSize (Size size)
		{
			childSize = size;

			RealignEverything ();
		}

		public bool BorderVisible {
			get {
				return showBorder;
			}
			set {
				if (showBorder != (showBorder = value))
					RealignEverything ();
			}
		}

		public IScrollControlBackend CreateVerticalScrollControl ()
		{
			return VScrollbar;
		}

		public IScrollControlBackend CreateHorizontalScrollControl ()
		{
			return HScrollbar;
		}

		public ScrollPolicy VerticalScrollPolicy {
			get {
				return vScrollPolicy;
			}
			set {
				if (vScrollPolicy != (vScrollPolicy = value))
					return;

				RealignEverything();
			}
		}

		public ScrollPolicy HorizontalScrollPolicy {
			get {
				return hScrollPolicy;
			}
			set {
				if (hScrollPolicy != (hScrollPolicy = value))
					return;

				RealignEverything();
			}
		}

		public void UpdateChildPlacement (IWidgetBackend childBackend)
		{
			throw new NotImplementedException ();
		}




		public override WidgetBackend GetChildAt (double x, double y)
		{
			bool verticallyAtScrollbar = y >= Height - scrollbarWidth;

			if (x >= Width - scrollbarWidth)
				return verticallyAtScrollbar ? null : VScrollbar;

			if (verticallyAtScrollbar)
				return HScrollbar;

			if (child == null || 
				x > childSize.Width || 
				y > childSize.Height)
				return null;

			// Offset x,y by scroll positions
			var visRect = VisibleRect;
			x -= visRect.X;
			y -= visRect.Y;

			var w = child;
			while (true) {
				var ch = w.GetChildAt (x, y);
				if (ch == w || ch == null)
					return w;
				x -= w.X;
				y -= w.Y;
				w = ch;
			}

			return null;
		}

		public override Size GetPreferredSize (Cairo.Context fontExtentContext, double maxWidth, double maxHeight)
		{
			return base.GetPreferredSize (fontExtentContext, maxWidth, maxHeight);
		}

		#region IScrollAdjustmentEventSink implementation

		public void OnValueChanged ()
		{
			Invalidate ();
		}

		#endregion

		internal override void OnBoundsChanged (double x, double y, double width, double height)
		{
			base.OnBoundsChanged (x, y, width, height);

			RealignEverything ();
		}



		void RealignEverything()
		{
			var scrollbarWidth = ScrollViewBackend.scrollbarWidth;
			var needVSroll = Height > 0 && childSize.Height > (Height - scrollbarWidth);
			var needHScroll = Width > 0 && childSize.Width > (Width - scrollbarWidth);

			// Set the scrollbars' pagesizes.
			// Important: Persist the current scroll values ranging from 0 to 100, except a new child has been set.
			VScrollbar.SetRange (0, 100, needVSroll ? 100.0/(childSize.Height/Height) : 100.0, 1, 1, VScrollbar.Value);
			HScrollbar.SetRange (0, 100, needHScroll ? 100.0/(childSize.Width/Width) : 100.0, 1, 1, HScrollbar.Value);

			// Show/hide scrollbars
			switch (VerticalScrollPolicy) {
				case ScrollPolicy.Always:
					VScrollbar.Visible = true;
					break;
				case ScrollPolicy.Automatic:
					VScrollbar.Visible = needVSroll;
					break;
				case ScrollPolicy.Never:
					VScrollbar.Visible = false;
					break;
			}

			switch (HorizontalScrollPolicy) {
				case ScrollPolicy.Always:
					HScrollbar.Visible = true;
					break;
				case ScrollPolicy.Automatic:
					HScrollbar.Visible = needHScroll;
					break;
				case ScrollPolicy.Never:
					HScrollbar.Visible = false;
					break;
			}

			// Layout scrollbars
			if (VScrollbar.Visible) {
				VScrollbar.Sensitive = needVSroll;
				VScrollbar.OnBoundsChanged (Width - scrollbarWidth, 0, scrollbarWidth, VisualChildHeight);
			}

			if (HScrollbar.Visible) {
				HScrollbar.Sensitive = needHScroll;
				HScrollbar.OnBoundsChanged (0, Height- scrollbarWidth, VisualChildWidth, scrollbarWidth);
			}

			Invalidate ();
		}

		protected override void DrawInternally (CairoContextBackend c, Rectangle dirtyRect)
		{
			base.DrawInternally (c, dirtyRect);

			// Draw child
			if (child != null) {
				var absLoc = AbsoluteLocation;
				if( dirtyRect.X >= absLoc.X && 
					dirtyRect.Y >= absLoc.Y &&
					dirtyRect.Width >= VisualChildWidth &&
					dirtyRect.Height >= VisualChildHeight)
				{
					var visRect = VisibleRect;
					/* Problem: Passing the dirtyRect to the draw method will not cause the widget to pay attention to being scrolled through or so.
					 * Perhaps this could be solved by temporarily adjusting 
					 * the render context's global x/y offsets and offsetting the dirtyRect 
					 * by the theoretical distance between the 
					 * child's upper left corner and the visible upper left corner, so the child will think it's drawing beginning from 0/0.
					 */

					c.GlobalXOffset -= visRect.X;
					c.GlobalYOffset -= visRect.Y;

					child.Draw (c, visRect.Offset(absLoc.X, absLoc.Y).Intersect(dirtyRect.Offset(visRect.X, visRect.Y)));

					c.GlobalXOffset += visRect.X;
					c.GlobalYOffset += visRect.Y;
				}
			}

			// Draw scroll bars if needed
			if (VScrollbar.Visible)
				VScrollbar.Draw (c, dirtyRect, false);

			if (HScrollbar.Visible)
				HScrollbar.Draw (c, dirtyRect, false);
		}
	}
}

