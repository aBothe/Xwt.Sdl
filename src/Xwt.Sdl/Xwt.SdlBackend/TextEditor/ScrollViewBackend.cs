//
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

				if (!childHasCustomScrolling) {
					yield return VScrollbar;
					yield return HScrollbar;
				}
			}
		}
		Size childSize;
		bool childHasCustomScrolling;
		static double scrollbarWidth {get{ return WidgetStyles.Instance.ScrollbarWidth; }}
		readonly ScrollBarBackend VScrollbar, HScrollbar;
		ScrollPolicy vScrollPolicy, hScrollPolicy;
		bool showBorder = false;
		double VisualChildWidth { get{ return Math.Min(Width- (VScrollbar.Visible ? scrollbarWidth : 0), childSize.Width); } }
		double VisualChildHeight { get{ return Math.Min(Height - (HScrollbar.Visible ? scrollbarWidth : 0), childSize.Height); } }

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
			child = c as WidgetBackend;

			if (child != null) {
				child.Parent = this;
				childHasCustomScrolling = true;
			}
		}

		public void SetChildSize (Size size)
		{
			childHasCustomScrolling = false;
			if(childSize != (childSize = size))
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
					RealignEverything();
			}
		}

		public ScrollPolicy HorizontalScrollPolicy {
			get {
				return hScrollPolicy;
			}
			set {
				if (hScrollPolicy != (hScrollPolicy = value))
					RealignEverything();
			}
		}

		public void UpdateChildPlacement (IWidgetBackend childBackend)
		{
			throw new NotImplementedException ();
		}




		public override WidgetBackend GetChildAt (double x, double y)
		{
			if (childHasCustomScrolling)
				return child;

			x -= viewPortProxyX;
			y -= viewPortProxyY;

			bool verticallyAtScrollbar = y >= Height - scrollbarWidth;

			if (x >= Width - scrollbarWidth)
				return verticallyAtScrollbar ? null : VScrollbar;

			if (verticallyAtScrollbar)
				return HScrollbar;

			if (child == null || 
				x <= 0 || y <= 0 ||
				x > childSize.Width || 
				y > childSize.Height)
				return null;

			x += viewPortProxyX;
			y += viewPortProxyY;

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

		internal override bool OnBoundsChanged (double x, double y, double width, double height)
		{
			if (base.OnBoundsChanged (x, y, width, height)) {
				RealignEverything ();
				return true;
			}
			return false;
		}



		void RealignEverything()
		{
			if (childHasCustomScrolling) {
				if (child != null)
					child.OnBoundsChanged (0, 0, Width, Height);
				VScrollbar.Visible = false;
				HScrollbar.Visible = false;
				return;
			}

			// Set child size
			if (child != null)
				child.OnBoundsChanged (0, 0, childSize.Width, childSize.Height);

			var needVSroll = Height > 0 && childSize.Height > Height;
			var needHScroll = Width > 0 && childSize.Width > Width;
			const double normalizedMaximum = 100.0;
			double pageIncrement = normalizedMaximum / WidgetStyles.Instance.ScrollViewStepIncrementPercent;

			// Set the scrollbars' pagesizes.
			// Important: Persist the current scroll values ranging from 0 to 100, except a new child has been set.
			var vPageSize = needVSroll ? normalizedMaximum / (childSize.Height / Height) : normalizedMaximum;
			var hPageSize = needHScroll ? normalizedMaximum / (childSize.Width / Width) : normalizedMaximum;

			VScrollbar.SetRange (0, normalizedMaximum + vPageSize, vPageSize, pageIncrement, pageIncrement, VScrollbar.Value);
			HScrollbar.SetRange (0, normalizedMaximum + hPageSize, hPageSize, pageIncrement, pageIncrement, HScrollbar.Value);

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

		protected override void DrawInternally (CairoContextBackend c, Rectangle dirtyRect, double absLocX, double absLocY)
		{
			base.DrawInternally (c,dirtyRect, absLocX, absLocY);

			// Draw child
			if (child != null) {
				if (childHasCustomScrolling) {
					child.Draw (c, dirtyRect);
					return;
				}

				if( dirtyRect.X >= absLocX && dirtyRect.X <= absLocX + VisualChildWidth &&
					dirtyRect.Y >= absLocY && dirtyRect.Y <= absLocY + VisualChildHeight)
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

					child.Draw (c, visRect.Offset(absLocX, absLocY).Intersect(dirtyRect.Offset(visRect.X, visRect.Y)));

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

		internal override bool FireMouseWheel (uint timestamp, int x, int y, ScrollDirection dir)
		{
			if (!base.FireMouseWheel (timestamp, x, y, dir)) {
				switch (dir) {
				case ScrollDirection.Down:
				case ScrollDirection.Up:
					return VScrollbar.FireMouseWheel (timestamp, x, y, dir);
				case ScrollDirection.Left:
				case ScrollDirection.Right:
					return HScrollbar.FireMouseWheel (timestamp, x, y, dir);
				}
			}
			return true;
		}
	}
}

