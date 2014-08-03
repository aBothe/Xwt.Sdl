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

namespace Xwt.Sdl
{
	public class ScrollViewBackend : WidgetBackend, IScrollViewBackend
	{
		#region Properties
		WidgetBackend child;
		Size childSize;
		readonly ScrollBarBackend VScrollBar, HScrollbar;
		ScrollPolicy vScrollPolicy, hScrollPolicy;
		bool showBorder = false;

		#endregion

		public ScrollViewBackend ()
		{
			VScrollBar = new ScrollBarBackend ();
			HScrollbar = new ScrollBarBackend ();

			VScrollBar.Initialize (Orientation.Vertical);
			HScrollbar.Initialize (Orientation.Horizontal);

			VScrollBar.Parent = this;
			HScrollbar.Parent = this;
		}

		public void SetChild (IWidgetBackend c)
		{
			this.child = c as WidgetBackend;
			child.Parent = this;
		}

		public void SetChildSize (Size size)
		{
			childSize = size;

		}

		public bool BorderVisible {
			get {
				return showBorder;
			}
			set {
				if (showBorder != (showBorder = value))
					Invalidate ();
			}
		}

		public Rectangle VisibleRect {
			get {
				throw new NotImplementedException ();
			}
		}

		public IScrollControlBackend CreateVerticalScrollControl ()
		{
			return VScrollBar;
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

				UpdateScrollbarPositions();
			}
		}

		public ScrollPolicy HorizontalScrollPolicy {
			get {
				return hScrollPolicy;
			}
			set {
				if (hScrollPolicy != (hScrollPolicy = value))
					return;

				UpdateScrollbarPositions();
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
		}


		void UpdateScrollbarPositions()
		{

		}

		void UpdateAdjustments(){

		}
	}
}

