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
	public class ScrollBarBackend : WidgetBackend, IScrollbarBackend, IScrollAdjustmentBackend, IScrollControlBackend, IInWindowDrag
	{
		#region Properties
		Orientation orientation;
		IScrollAdjustmentEventSink scrollEventSink;
		IScrollControlEventSink scrollCtrlEventSink;

		bool barHovered;
		bool barClicked;
		Rectangle barRect = new Rectangle();
		double barClicked_MouseDistanceToBarBegin;

		double lowerVal;
		double upperVal;
		double val;
		double pageSize;
		double pageIncrement;
		double stepIncrement;

		public double LowerValue {
			get {
				return lowerVal;
			}
		}

		public double UpperValue {
			get {
				return upperVal + PageSize;
			}
		}

		public double PageSize {
			get {
				return pageSize;
			}
		}

		public double PageIncrement {
			get {
				return pageIncrement;
			}
		}

		public double StepIncrement {
			get {
				throw new NotImplementedException ();
			}
		}
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

		public void Initialize (IScrollControlEventSink eventSink)
		{
			this.scrollCtrlEventSink = eventSink;
		}

		public void SetRange (double lowerValue, double upperValue, double pageSize, double pageIncrement, double stepIncrement, double value)
		{
			this.lowerVal = Math.Max(lowerValue, 0.0);
			this.pageSize = Math.Max(pageSize, 0.0);
			this.upperVal = Math.Max(upperValue, this.pageSize) - this.pageSize;
			this.pageIncrement = pageIncrement;
			this.stepIncrement = stepIncrement;
			Value = value; // implies UpdateBarRect,Invalidate()
		}

		public double Value {
			get {
				return val;
			}
			set {
				if(val == (val = Math.Max(Math.Min(value,upperVal),lowerVal)))
					return;

				if (scrollEventSink != null)
					scrollEventSink.OnValueChanged ();
				if (scrollCtrlEventSink != null)
					scrollCtrlEventSink.OnValueChanged ();

				UpdateBarRect ();
				Invalidate ();
			}
		}

		double BarLength
		{ get { return (orientation == Orientation.Vertical ? Height : Width) * pageSize / (upperVal - lowerVal); } }

		double VisualBarBegin
		{ get { return (((orientation == Orientation.Vertical ? Height : Width) - BarLength) * val / (upperVal - lowerVal)); } }

		void UpdateBarRect()
		{
			var ws = WidgetStyles.Instance;
			double absX, absY;
			GetAbsoluteLocation (out absX, out absY);

			switch (orientation) {
				case Orientation.Vertical:
					barRect.X = absX + ws.ScrollbarPadding;
					barRect.Y = absY + VisualBarBegin;
					barRect.Width = Width - ws.ScrollbarPadding * 2.0;
					barRect.Height = BarLength;
					break;
				case Orientation.Horizontal:
					barRect.X = absX + VisualBarBegin;
					barRect.Y = absY + ws.ScrollbarPadding;
					barRect.Width = BarLength;
					barRect.Height = Height - ws.ScrollbarPadding * 2.0;
					break;
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

		internal override bool OnBoundsChanged (double x, double y, double width, double height)
		{
			if (!base.OnBoundsChanged (x, y, width, height))
				return false;

			UpdateBarRect ();
			// Don't invalidate again because it's already been done in base.OnBoundsChanged
			return true;
		}

		const double barPadding = 2;

		protected override void DrawInternally (CairoContextBackend c, Rectangle rect, double absX, double absY)
		{
			var ws = WidgetStyles.Instance;
			// Background
			base.DrawInternally (c, rect, absX, absY);

			// Bar
			c.Rectangle (barRect);

			c.SetColor (barHovered || barClicked ? ws.ScrollbarHoveredColor : ws.ScrollbarColor);
			c.Fill ();
		}



		protected override void SensitivityChanged ()
		{
			base.SensitivityChanged ();

			if (!Sensitive)
				Finish ();
		}

		internal override void FireMouseLeave ()
		{
			base.FireMouseLeave ();

			if (barHovered) {
				barHovered = false;
				Invalidate ();
			}
		}

		#region IInWindowDrag implementation

		public bool MouseWheel(int x, int y, ScrollDirection dir)
		{
			if (!Sensitive || val > upperVal || val < lowerVal)
				return false;

			switch (dir) {
				case ScrollDirection.Down:
				case ScrollDirection.Right:
					Value += pageIncrement;
					break;
				default:
					Value -= pageIncrement;
					break;
			}

			return true;
		}

		public void MouseMove (int x, int y)
		{
			if (!Sensitive)
				return;

			double absX, absY;
			GetAbsoluteLocation (out absX, out absY);

			switch (orientation) {
				case Orientation.Vertical:
					Value = (y-absY - barClicked_MouseDistanceToBarBegin) * (upperVal - lowerVal) / (Height-BarLength);
					break;
				case Orientation.Horizontal:
					Value = (x-absX - barClicked_MouseDistanceToBarBegin) * (upperVal - lowerVal) / (Width-BarLength);
					break;
			}
		}

		/// <summary>
		/// Finishes the drag operation.
		/// </summary>
		public void Finish ()
		{
			barClicked = false;
			barHovered = false;
			Invalidate ();
		}

		public PointerButton ReleaseButton {			get;			set;		}

		#endregion

		internal override bool FireMouseMoved (uint timestamp, int x, int y)
		{
			if (Sensitive) {
				if (barHovered != (barHovered = barRect.Contains (x, y)))
					Invalidate ();

				if(barClicked)
					MouseMove (x, y);
			}

			return base.FireMouseMoved (timestamp, x, y);
		}

		internal override bool FireMouseButton (bool down, PointerButton butt, int x, int y, int multiplePress = 1)
		{
			if (Sensitive && barHovered) {
				if (barClicked = down) {
					ReleaseButton = butt;
					var win = ParentWindow;
					if (win != null)
						win.StartInWindowDrag(this);

					switch (orientation) {
						case Orientation.Vertical:
							barClicked_MouseDistanceToBarBegin = y - barRect.Y;
							break;
						case Orientation.Horizontal:
							barClicked_MouseDistanceToBarBegin = x - barRect.X;
							break;
					}
				}
				return true;
			}

			return base.FireMouseButton (down, butt, x, y, multiplePress);
		}

		internal override bool FireMouseWheel (uint timestamp, int x, int y, ScrollDirection dir)
		{
			return 
				base.FireMouseWheel (timestamp, x, y, dir) || 
				MouseWheel (x, y, dir);
		}
	}
}

