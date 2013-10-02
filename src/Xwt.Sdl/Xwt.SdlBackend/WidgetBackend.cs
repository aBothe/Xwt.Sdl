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
using Xwt.CairoBackend;

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
		public WidgetBackend Parent { 
			get { return parentRef != null ? parentRef.Target as WidgetBackend : null; }
			set{ parentRef = (value != null) ? new WeakReference (value) : null; }
		}
		public WindowBackend ParentWindow {
			get{
				if (parentRef == null)
					return null;

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
		bool trackMouseMoved = false;

		bool focused;
		bool visible;
		bool sensitive = true;

		double minWidth, minHeight;
		double x,y, width, height;
		public Rectangle Bounds
		{
			get{return new Rectangle (x, y, width, height);}
		}
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
				OnWidgetResized ();
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
				OnWidgetResized ();
			}
		}

		public double Width {
			get{return width;}
			set{ 
				width = value;
				Invalidate ();
				OnWidgetResized ();
			}
		}

		public double Height {
			get{return height;}
			set{ 
				height = value;
				Invalidate ();
				OnWidgetResized ();
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

		public void GetAbsoluteLocation(out double absX, out double absY)
		{
			absX = x;
			absY = y;
			WeakReference wref = parentRef;
			WidgetBackend w;
			while (wref != null && (w = wref.Target as WidgetBackend) != null) {
				absX += w.x;
				absY += w.y;
				wref = w.parentRef;
			}
		}

		/// <summary>
		/// The location relative to the absolute parent window.
		/// </summary>
		public Point AbsoluteLocation
		{
			get{ 
				double absX, absY;
				GetAbsoluteLocation (out absX, out absY);
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

		internal virtual void FireLostFocus()
		{
			focused = false;
			this.eventSink.OnLostFocus ();
		}

		internal virtual void FireGainedFocus()
		{
			focused = true;
			this.eventSink.OnGotFocus ();
		}

		internal virtual void OnWidgetResized()
		{
			this.eventSink.OnBoundsChanged ();
			Invalidate ();
		}

		protected bool MouseEntered;
		internal virtual void FireMouseEnter()
		{
			MouseEntered = true;
			if(cursorSet)
				SetSysCursor (cursor);
			eventSink.OnMouseEntered ();
		}

		static IntPtr oldCursor;
		public static void SetSysCursor(CursorType cur)
		{
			if (oldCursor != IntPtr.Zero) {
				SDL2.SDL.SDL_FreeCursor (oldCursor);
				oldCursor = IntPtr.Zero;
			}
			SDL2.SDL.SDL_SystemCursor sysCur;

			if (cur == CursorType.Arrow)
				sysCur = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW;
			else if (cur == CursorType.Crosshair)
				sysCur = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_CROSSHAIR;
			else if (cur == CursorType.Hand)
				sysCur = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_HAND;
			else if (cur == CursorType.IBeam)
				sysCur = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_IBEAM;
			else if (cur == CursorType.ResizeLeft)
				return;
			else if (cur == CursorType.ResizeLeftRight)
				sysCur = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE;
			else if (cur == CursorType.ResizeRight)
				return;
			else if (cur == CursorType.ResizeUp)
				return;
			else if (cur == CursorType.ResizeUpDown)
				sysCur = SDL2.SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENS;
			else if (cur == CursorType.ResizeDown)
				return;
			else
				return;

			oldCursor = SDL2.SDL.SDL_CreateSystemCursor (sysCur);
			SDL2.SDL.SDL_SetCursor (oldCursor);
		} 

		internal virtual bool FireMouseMoved(uint timestamp, int x, int y)
		{
			MouseMovedEventArgs mouseMovedEA;
			if (trackMouseMoved) {
				eventSink.OnMouseMoved (mouseMovedEA = new MouseMovedEventArgs((long)timestamp, (double)x,(double)y));
				return mouseMovedEA.Handled;
			}
			return false;
		}

		internal virtual void FireMouseLeave()
		{
			MouseEntered = false;
			if(cursorSet)
				SetSysCursor (CursorType.Arrow);
			this.eventSink.OnMouseExited ();
		}

		readonly ButtonEventArgs buttonEA = new ButtonEventArgs();
		internal virtual bool FireMouseButton(bool down, PointerButton butt,int x, int y, int multiplePress = 1)
		{
			buttonEA.Handled = false;
			buttonEA.X = (double)x-this.x;
			buttonEA.Y = (double)y-this.y;
			buttonEA.Button = butt;
			buttonEA.MultiplePress = multiplePress;

			if (down)
				this.eventSink.OnButtonPressed (buttonEA);
			else
				this.eventSink.OnButtonReleased (buttonEA);

			return buttonEA.Handled;
		}

		internal virtual bool FireMouseWheel(uint timestamp, int x, int y, ScrollDirection dir)
		{
			var mouseScrolledEA = new MouseScrolledEventArgs ((long)timestamp, (double)x-this.x, (double)y-this.y, dir);
			eventSink.OnMouseScrolled (mouseScrolledEA);
			return mouseScrolledEA.Handled;
		}

		internal virtual bool FireKeyDown(Key k, char ch, ModifierKeys mods, bool rep, uint timestamp)
		{
			var ea = new KeyEventArgs(k, mods, rep, (long)timestamp);
			this.eventSink.OnKeyPressed(ea);
			return ea.Handled;
		}

		internal virtual bool FireKeyUp(Key k, char ch, ModifierKeys mods, bool rep, uint timestamp)
		{
			var ea = new KeyEventArgs(k, mods, rep, (long)timestamp);
			this.eventSink.OnKeyPressed(ea);
			return ea.Handled;
		}

		internal virtual void OnBoundsChanged(double x, double y, double width, double height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;

			OnWidgetResized ();
		}

		public virtual IEnumerable<WidgetBackend> Children {get{ return null; }}

		public virtual WidgetBackend GetChildAt(double x, double y)
		{
			var children = Children;
			if(children != null)
				foreach (var ch in children)
					if (x >= ch.X && y >= ch.Y && x <= ch.X + ch.Width && y <= ch.Y + ch.Height)
						return ch;
			return null;
		}

		/// <summary>
		/// Queues redrawing the widget.
		/// </summary>
		public void Invalidate()
		{
			double x, y;
			GetAbsoluteLocation (out x, out y);
			var pw = ParentWindow;
			if(pw != null)
				pw.Invalidate (new Rectangle(x, y, width, height));
		}

		public void Invalidate(Rectangle rect)
		{
			double x, y;
			GetAbsoluteLocation (out x, out y);

			var pw = ParentWindow;
			if(pw != null)
				pw.Invalidate (rect.Offset(x,y));
		}

		public virtual void Draw(CairoContextBackend c,Rectangle dirtyRect)
		{
			c.Context.SetSourceRGBA (backgroundColor.Red, backgroundColor.Green, backgroundColor.Blue, backgroundColor.Alpha);
			c.Context.Rectangle (dirtyRect.X + c.GlobalXOffset, dirtyRect.Y + c.GlobalYOffset, dirtyRect.Width, dirtyRect.Height);
			c.Context.Fill ();
			/*
			var loc = AbsoluteLocation;
			c.Context.Rectangle (loc.X, loc.Y, Width, Height);
			c.Context.Clip ();*/
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
			/*
			if (customFont != null)
				customFont.Dispose ();*/
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
			Invalidate ();
		}

		public Size GetPreferredSize (SizeConstraint widthConstraint, SizeConstraint heightConstraint)
		{
			currentWidthConstraint = widthConstraint;
			currentHeightConstraint = heightConstraint;

			using (var surf = new Cairo.ImageSurface (Cairo.Format.A1, 0, 0))
			using (var c = new Cairo.Context (surf)) {
				var sz = GetPreferredSize (c);

				//ISSUE: Maybe put the widget's padding directly into this class in order to get the exact padding measurements?
				var win = ParentWindow;
				if (win != null && !win.Padding.IsEmpty) {
					sz.Height += win.Padding.Bottom;
					sz.Width += win.Padding.Right;
				}
				return sz;
			}
		}

		public virtual Size GetPreferredSize(Cairo.Context fontExtentContext) { return Size.Zero; }

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

		bool cursorSet = false;
		CursorType cursor;
		public void SetCursor (CursorType cursorType)
		{
			cursor = cursorType;
			if (MouseEntered)
				SetSysCursor (cursor);
			cursorSet = true;
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

		protected virtual bool WidgetSupportsFocusing {get{return true;}}
		bool cangetfocus = true;
		public bool CanGetFocus {
			get {
				return WidgetSupportsFocusing && cangetfocus;
			}
			set {
				cangetfocus = value;
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
				return null;
			}
		}

		internal InternalFontDescription FontBackend {get{return customFont ?? CairoFontBackendHandler.SystemDefaultFont;}}
		InternalFontDescription customFont;
		public virtual object Font {
			get {
				return FontBackend;
			}
			set {
				customFont = value as InternalFontDescription;
				Invalidate ();
			}
		}

		Color backgroundColor=Colors.White;
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
			if ((int)eventId == (int)WidgetEvent.MouseMoved)
				trackMouseMoved = true;
		}

		public void DisableEvent (object eventId)
		{
			if ((int)eventId == (int)WidgetEvent.MouseMoved)
				trackMouseMoved = false;
		}

		#endregion
	}
}

