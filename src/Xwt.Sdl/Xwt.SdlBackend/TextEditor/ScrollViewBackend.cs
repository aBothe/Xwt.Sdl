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
	public class ScrollViewBackend : WidgetBackend, IScrollViewBackend
	{
		#region Properties
		WidgetBackend child;
		Size childSize;
		readonly ScrollBarBackend VScrollbar, HScrollbar;
		ScrollPolicy vScrollPolicy, hScrollPolicy;
		bool showBorder = false;

		#endregion

		public ScrollViewBackend ()
		{
			VScrollbar = new ScrollBarBackend ();
			HScrollbar = new ScrollBarBackend ();

			VScrollbar.Initialize (Orientation.Vertical);
			HScrollbar.Initialize (Orientation.Horizontal);

			VScrollBar.Parent = this;
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
			return base.GetChildAt (x, y);
		}

		public override Size GetPreferredSize (Cairo.Context fontExtentContext, double maxWidth, double maxHeight)
		{
			return base.GetPreferredSize (fontExtentContext, maxWidth, maxHeight);
		}

		internal override void OnBoundsChanged (double x, double y, double width, double height)
		{
			base.OnBoundsChanged (x, y, width, height);

			RealignEverything ();
		}


		void RealignEverything()
		{
			// Set the child's size

			// Set the scrollbars' pagesizes
			// Important: Persist the current scroll values ranging from 0 to 100, except a new child has been set.

			// Show/hide scrollbars
		}


		public override void Draw (CairoContextBackend c, Rectangle dirtyRect)
		{
			base.Draw (c, dirtyRect);

			Rectangle absRect;

			// Draw child
			if (child != null) {
				var absLoc = child.AbsoluteLocation;
				absRect = new Rectangle(absLoc, child.Size).Intersect (dirtyRect);
				if (!absRect.IsEmpty) {
					/* Problem: Passing the dirtyRect to the draw method will not cause the widget to pay attention to being scrolled through or so.
					 * Perhaps this could be solved by temporarily adjusting 
					 * the render context's global x/y offsets and offsetting the dirtyRect 
					 * by the theoretical distance between the 
					 * child's upper left corner and the visible upper left corner, so the child will think it's drawing beginning from 0/0.
					 */
					double dx, dy;

					var visRect = VisibleRect;

					dx = 0;
					dy = 0;

					c.GlobalXOffset -= dx;
					c.GlobalYOffset -= dy;

					child.Draw (c, visRect.Intersect(dirtyRect.Offset(dx,dy)));

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

