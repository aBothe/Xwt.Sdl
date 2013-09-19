//
// WidgetBackend.cs
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
using Xwt.Drawing;
using System.Collections.Generic;

namespace Xwt.Sdl
{
	public abstract class WidgetBackend : IWidgetBackend
	{
		#region Properties
		static int gid;
		public readonly int Id;
		static Dictionary<int, WeakReference> widgetStore = new Dictionary<int, WeakReference>();

		Widget frontend;
		public Widget Frontend {get{return frontend;}}

		public ApplicationContext ApplicationContext {
			get;
			private set;
		}

		WeakReference parentRef;
		public WidgetBackend Parent { get { return parentRef.Target as WidgetBackend; }}
		public WindowBackend ParentWindow {
			get{
				if (parentRef.Target is WidgetBackend)
					return (parentRef.Target as WidgetBackend).ParentWindow;

				return parentRef.Target as WindowBackend; 
			}
			set{ 
				if (value != null)
					parentRef = new WeakReference (value);
				else
					parentRef = null;
			}
		}

		IWidgetEventSink eventSink;
		public IWidgetEventSink EventSink {get{return eventSink;}}
		bool focused;
		bool visible;
		bool sensitive;

		double minWidth, minHeight;
		double x,y, width, height;
		SizeConstraint currentWidthConstraint = SizeConstraint.Unconstrained;
		SizeConstraint currentHeightConstraint = SizeConstraint.Unconstrained;
/// <summary>
		/// Horizontal distance to the parent widget's left border.
		/// </summary>
		public double X { 
			get{return x;}
			set{
				x = value;
				Invalidate ();
			}
		}
		/// <summary>
		/// Vertical distance to the parent widget's top border
		/// </summary>
		/// <value>The y.</value>
		public double Y {
			get{return y;}
			set{ 
 				y = value;
				Invalidate ();
			}
		}

		public double Width {
			get{return width;}
			set{ 
				width = value;
				Invalidate ();
			}
		}
		public double Height {
			get{return height;}
			set{ 
				height = value;
				Invalidate ();
			}
		}

		/// <summary>
		/// The location relative to the parent widget.
		/// </summary>
		public Point RelativeLocation
		{
			get{ 
				return new Point (x, y);
			}
		}

		/// <summary>
		/// The location relative to the absolute parent window.
		/// </summary>
		public Point AbsoluteLocation
		{
			get{ 
				double absX = x, absY = y;
				WeakReference wref = parentRef;
				WidgetBackend w;
				while (wref != null && (w = wref.Target as WidgetBackend) != null) {
					absX += w.x;
					absY += w.y;
					wref = w.parentRef;
				}

				return new Point (absX, absY);
			}
		}

		#endregion

		#region Init / Constructor
		public WidgetBackend ()
		{
			Id = ++gid;
			widgetStore [Id] = new WeakReference(this);
		}
		#endregion

		#region Extension
		public static WidgetBackend GetById(int id)
		{
			WeakReference w;
			if (widgetStore.TryGetValue (id, out w) && w.IsAlive)
				return w.Target as WidgetBackend;

			return null;
		}

		internal void FireLostFocus()
		{
			focused = false;
			this.eventSink.OnLostFocus ();
		}

		internal void FireGainedFocus()
		{
			focused = true;
			this.eventSink.OnGotFocus ();
		}

		internal void OnWidgetResized()
		{
			this.eventSink.OnBoundsChanged ();
			Invalidate ();
		}

		internal void FireMouseEnter()
		{
			eventSink.OnMouseEntered ();
		}

		internal void FireMouseLeave()
		{
			this.eventSink.OnMouseExited ();
		}

		internal void OnBoundsChanged(double x, double y, double width, double height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;

			OnWidgetResized ();
		}

		public virtual WidgetBackend GetChildAt(double x, double y)
		{
			return null;
		}

		/// <summary>
		/// Queues redrawing the widget.
		/// </summary>
		public void Invalidate()
		{
			ParentWindow.Invalidate (new Rectangle(x,y, width, height));
		}

		public void Invalidate(Rectangle rect)
		{
			ParentWindow.Invalidate (rect);
		}

		public virtual void Draw()
		{
			OpenTK.Graphics.OpenGL.GL.Color3 (0.0, 0.0, .3);
			OpenTK.Graphics.OpenGL.GL.Rect (x, y, width, height);
		}

		#endregion

		#region IWidgetBackend implementation

		public void Initialize (IWidgetEventSink eventSink)
		{
			this.eventSink = eventSink;
			Initialize ();
		}

		public virtual void Initialize() {}

		public void Dispose ()
		{
			widgetStore.Remove (Id);
		}

		public Point ConvertToScreenCoordinates (Point widgetCoordinates)
		{
			//TODO: Add the client-windowframe offsets and the window coordinates
			return AbsoluteLocation.Offset (widgetCoordinates);
		}

		public void SetMinSize (double width, double height)
		{
			minWidth = width;
			minHeight = height;
		}

		public void SetSizeRequest (double width, double height)
		{
			this.width = minWidth > 0.0 && minWidth > width ? minWidth : width;
			this.height = minHeight > 0.0 && minHeight > height ? minHeight : height;
			OnWidgetResized ();
		}

		public void SetFocus ()
		{
			if(CanGetFocus)
				ParentWindow.SetFocusedWidget (Id);
		}

		public void UpdateLayout ()
		{
			throw new NotImplementedException ();
		}

		public Size GetPreferredSize (SizeConstraint widthConstraint, SizeConstraint heightConstraint)
		{
			currentWidthConstraint = widthConstraint;
			currentHeightConstraint = heightConstraint;
			return GetPreferredSize ();
		}

		public virtual Size GetPreferredSize() { return Size.Zero; }

		public void DragStart (DragStartData data)
		{
			throw new NotImplementedException ();
		}

		public void SetDragSource (TransferDataType[] types, DragDropAction dragAction)
		{
			throw new NotImplementedException ();
		}

		public void SetDragTarget (TransferDataType[] types, DragDropAction dragAction)
		{
			throw new NotImplementedException ();
		}

		public void SetCursor (CursorType cursorType)
		{
			throw new NotImplementedException ();
		}

		public bool Visible {
			get {
				return visible;
			}
			set {
				visible = value;
				Invalidate ();
			}
		}

		public bool Sensitive {
			get {
				return sensitive;
			}
			set {
				sensitive = value;
				Invalidate ();
			}
		}

		public bool CanGetFocus {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool HasFocus {
			get {
				return focused;
			}
		}

		double opacity = 1.0;
		public double Opacity {
			get {
				return opacity;
			}
			set {
				opacity = value;
				Invalidate ();
			}
		}

		public Size Size {
			get {
				return new Size (width, height);
			}
		}

		public object NativeWidget {
			get {
				throw new NotImplementedException ();
			}
		}

		public virtual object Font {
			get {
				return null;
			}
			set {

			}
		}

		Color backgroundColor;
		public Color BackgroundColor {
			get { return backgroundColor;}
			set { backgroundColor = value; Invalidate ();}
		}

		public string TooltipText {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		#endregion

		#region IBackend implementation

		public void InitializeBackend (object frontend, ApplicationContext context)
		{
			this.frontend = frontend as Widget;
			this.ApplicationContext = context;
		}

		public void EnableEvent (object eventId)
		{

		}

		public void DisableEvent (object eventId)
		{

		}

		#endregion
	}
}

