//
// ScrollBarBackend.cs
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
	public class ScrollBarBackend : WidgetBackend, IScrollbarBackend, IScrollAdjustmentBackend
	{
		#region Properties
		Orientation orientation;
		IScrollAdjustmentEventSink scrollEventSink;

		double lowerVal;
		double upperVal;
		double val;
		double pageSize;
		double pageIncrement;
		double stepIncrement;
		#endregion

		public ScrollBarBackend()
		{
			BackgroundColor = WidgetStyles.Instance.ScrollbarBackground;
		}

		public void Initialize (Orientation dir)
		{
			this.orientation = dir;
			Invalidate ();
		}

		public IScrollAdjustmentBackend CreateAdjustment ()
		{
			return this;
		}

		public void Initialize (IScrollAdjustmentEventSink eventSink)
		{
			this.scrollEventSink = eventSink;
		}

		public void SetRange (double lowerValue, double upperValue, double pageSize, double pageIncrement, double stepIncrement, double value)
		{
			this.lowerVal = lowerValue;
			this.upperVal = upperValue;
			this.pageSize = pageSize;
			this.pageIncrement = pageIncrement;
			this.stepIncrement = stepIncrement;
			Value = value; // implies Invalidate()
		}

		public double Value {
			get {
				return val;
			}
			set {
				val = value;
				if (scrollEventSink != null)
					scrollEventSink.OnValueChanged ();
				Invalidate ();
			}
		}

		public override Size GetPreferredSize (Cairo.Context fontExtentContext, double maxWidth, double maxHeight)
		{
			switch (orientation) {
				case Orientation.Horizontal:
					return new Size (maxWidth, Math.Min(maxHeight, WidgetStyles.Instance.ScrollbarWidth));
				default:
					return new Size (Math.Min(maxWidth, WidgetStyles.Instance.ScrollbarWidth), maxHeight);
			}
		}

		internal override void OnBoundsChanged (double x, double y, double width, double height)
		{
			base.OnBoundsChanged (x, y, width, height);

			Invalidate ();
		}

		const double barPadding = 2;

		public override void Draw (CairoContextBackend c, Rectangle rect)
		{
			var ws = WidgetStyles.Instance;
			// Background
			base.Draw (c, rect);

			double x, y;
			GetAbsoluteLocation (out x, out y);

			// Bar
			var visualValue = ((orientation == Orientation.Vertical ? Height : Width) / (upperVal - lowerVal)) * val;
			var barLength = ((orientation == Orientation.Vertical ? Height : Width) / (upperVal - lowerVal)) * pageSize;

			switch (orientation) {
				case Orientation.Vertical:
					c.Context.Rectangle (x+ws.ScrollbarPadding,y+visualValue-barLength/2,Width-ws.ScrollbarPadding, barLength);
					break;
				case Orientation.Horizontal:

					break;
			}
		}

		internal override bool FireMouseMoved (uint timestamp, int x, int y)
		{
			return base.FireMouseMoved (timestamp, x, y);
		}

		internal override bool FireMouseButton (bool down, PointerButton butt, int x, int y, int multiplePress = 1)
		{
			return base.FireMouseButton (down, butt, x, y, multiplePress);
		}
	}
}

