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

namespace Xwt.Sdl.Backends
{
	public abstract class WidgetBackend : IWidgetBackend
	{
		#region Properties
		static int gid;
		public readonly int Id;
		static Dictionary<int, WeakReference> widgetStore = new Dictionary<int, WeakReference>();

		WeakReference parentRef;
		public WidgetBackend Parent { get { return parentRef.Target as WidgetBackend; }}
		public WindowBackend ParentWindow {
			get{
				if (parentRef.Target is WidgetBackend)
					return (parentRef.Target as WidgetBackend).ParentWindow;

				return parentRef.Target as WindowBackend; 
			}
		}

		IWidgetEventSink eventSink;
		bool focused;
		double minWidth, minHeight;

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

		public void Invalidate()
		{

		}

		public void Draw()
		{

		}

		#endregion

		#region IWidgetBackend implementation

		public void Initialize (IWidgetEventSink eventSink)
		{
			this.eventSink = eventSink;
		}

		public void Dispose ()
		{
			widgetStore.Remove (Id);
		}

		public Point ConvertToScreenCoordinates (Point widgetCoordinates)
		{
			throw new NotImplementedException ();
		}

		public void SetMinSize (double width, double height)
		{
			minWidth = width;
			minHeight = height;
		}

		public void SetSizeRequest (double width, double height)
		{
			throw new NotImplementedException ();
		}

		public void SetFocus ()
		{
			ParentWindow.SetFocusedWidget (Id);
		}

		public void UpdateLayout ()
		{
			throw new NotImplementedException ();
		}

		public Size GetPreferredSize (SizeConstraint widthConstraint, SizeConstraint heightConstraint)
		{
			throw new NotImplementedException ();
		}

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
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool Sensitive {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
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

		public double Opacity {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public Size Size {
			get {
				throw new NotImplementedException ();
			}
		}

		public object NativeWidget {
			get {
				throw new NotImplementedException ();
			}
		}

		public object Font {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
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

