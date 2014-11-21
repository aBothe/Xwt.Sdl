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
				if (VScrollbar.Visible)
					yield return VScrollbar;
				if (HScrollbar.Visible)
					yield return HScrollbar;
			}
		}
		Size childSize;
		readonly ScrollBarBackend VScrollbar, HScrollbar;
		ScrollPolicy vScrollPolicy, hScrollPolicy;
		bool showBorder = false;
		double VisualChildWidth { get{ return Width - (HScrollbar.Visible ? HScrollbar.Width : 0); } }
		double VisualChildHeight { get{ return Height - (VScrollbar.Visible ? VScrollbar.Height : 0); } }

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

		public Rectangle VisibleRect {
			get {
				// Use the child viewport and the h/v-scroll values to calculate the top left child-related corner. Width and Height are easily derivable from this.
				double childX, childY;

				childX = 0;
				childY = 0;

				return new Rectangle (childX, childY, child.Width, child.Height);
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
			if (x <= Width - (VScrollbar.Visible ? WidgetStyles.Instance.ScrollbarWidth:0)) {

			}

			if (y <= Height - (HScrollbar.Visible ? WidgetStyles.Instance.ScrollbarWidth:0)) {

			}
			//TODO: Offset x,y by scroll positions
			return base.GetChildAt (x, y);
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
			var scrollBarWidth = WidgetStyles.Instance.ScrollbarWidth;

			var needVSroll = Height > 0 && childSize.Height > (Height - scrollBarWidth);
			var needHScroll = Width > 0 && childSize.Width > (Width - scrollBarWidth);

			// Set the scrollbars' pagesizes.
			// Important: Persist the current scroll values ranging from 0 to 100, except a new child has been set.
			VScrollbar.SetRange (0, 100, needVSroll ? Height / childSize.Height : 100.0, 10, 10, VScrollbar.Value);
			HScrollbar.SetRange (0, 100, needHScroll ? Width / childSize.Width : 100.0, 10, 10, HScrollbar.Value);

			// Show/hide scrollbars
			var vScrollVisiblePreviously = VScrollbar.Visible;
			var hScrollVisiblePreviously = HScrollbar.Visible;

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
				VScrollbar.OnBoundsChanged (Width - scrollBarWidth, 0, scrollBarWidth, Height - (HScrollbar.Visible ? scrollBarWidth : 0));
			}

			if (HScrollbar.Visible) {
				HScrollbar.OnBoundsChanged (0, Height- scrollBarWidth, Width - (VScrollbar.Visible ? scrollBarWidth : 0), scrollBarWidth);
			}

			Invalidate ();
		}

		public override void Draw (CairoContextBackend c, Rectangle dirtyRect)
		{
			base.Draw (c, dirtyRect);

			Rectangle absRect;

			// Draw child
			if (child != null) {
				var absLoc = child.AbsoluteLocation;
				absRect = new Rectangle(absLoc, childSize).Intersect (dirtyRect);
				if (!absRect.IsEmpty) {
					/* Problem: Passing the dirtyRect to the draw method will not cause the widget to pay attention to being scrolled through or so.
					 * Perhaps this could be solved by temporarily adjusting 
					 * the render context's global x/y offsets and offsetting the dirtyRect 
					 * by the theoretical distance between the 
					 * child's upper left corner and the visible upper left corner, so the child will think it's drawing beginning from 0/0.
					 */
					double dx, dy;

					var visRect = VisibleRect;

					dx = visRect.X;
					dy = visRect.Y;

					c.GlobalXOffset -= dx;
					c.GlobalYOffset -= dy;

					child.Draw (c, visRect.Intersect(dirtyRect));

					c.GlobalXOffset += dx;
					c.GlobalYOffset += dy;
				}
			}

			// Draw scroll bars if needed
			if (VScrollbar.Visible) {
				absRect = VScrollbar.AbsoluteBounds.Intersect (dirtyRect);
				if (!absRect.IsEmpty)
					VScrollbar.Draw (c, dirtyRect);
			}

			if (HScrollbar.Visible) {
				absRect = HScrollbar.AbsoluteBounds.Intersect (dirtyRect);
				if (!absRect.IsEmpty)
					HScrollbar.Draw (c, dirtyRect);
			}
		}
	}
}

