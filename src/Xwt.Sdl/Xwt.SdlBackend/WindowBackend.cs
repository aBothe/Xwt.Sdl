//
// WindowBackend.cs
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
using System.Runtime.InteropServices;
using Xwt.Backends;
using SDL2;
using System.Collections.Generic;

namespace Xwt.Sdl
{
	public class WindowBackend : IWindowBackend
	{
		#region Properties
		internal static Dictionary<uint, WeakReference> windowCache = new Dictionary<uint, WeakReference>();
		IntPtr window;
		SDL.SDL_Surface windowSurface;

		uint id;
		public uint WindowId {get{return id;}}
		public IWindowFrameEventSink eventSink;

		double menuHeight=0;
		int oldWidth;
		int oldHeight;
		int Width;
		int Height;
		/// <summary>
		/// http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-14-render-to-texture/
		/// </summary>
		Rectangle invalidatedRegion;
		bool redraw;

		Rectangle padding;
		WidgetBackend child;
		MenuBackend menu;

		/// <summary>
		/// Focused widget to which keyboard & mouse events are redirected.
		/// </summary>
		WidgetBackend focusedWidget;
		WidgetBackend hoveredWidget;
		internal static WindowBackend WindowHoveredByMouse;
		bool focused;
		public bool HasFocus {get{return focused;}}
		#endregion

		#region Extension
		void UpdateViewPort()
		{
			windowSurface = SDL_.SDL_GetWindowSurface (window);
		}

		public WidgetBackend GetWidgetAt(double x, double y)
		{
			if (menu != null) {
				if (y <= menuHeight) // Normalize y value
					return null; // .. or return a widget wrapper
			}

			if (child == null || 
				y < padding.Top + menuHeight || x < padding.Left || 
				y+padding.Height > Height || x + padding.Width > Width)
				return null;

			var w = child;
			while (true) {
				var ch = w.GetChildAt (x, y);
				if (ch == null)
					return w;
				w = ch;
			}
		}

		void FocusWidget(WidgetBackend w)
		{
			if (focusedWidget != w) {
				if (focusedWidget != null)
					focusedWidget.FireLostFocus ();

				focusedWidget = w;

				if (w != null)
					w.FireGainedFocus ();
			}
		}

		void HandleMouseButtonEvent(SDL.SDL_MouseButtonEvent ev)
		{
			if (focusedWidget == null)
				return;

			bool isButtonDownEvt = ev.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN;

			PointerButton butt;
			switch (ev.button) {
				default:
					butt = PointerButton.Left;
					break;
				case (byte)SDL.SDL_BUTTON_MIDDLE:
					butt = PointerButton.Middle;
					break;
				case (byte)SDL.SDL_BUTTON_RIGHT:
					butt = PointerButton.Right;
					break;
				case (byte)SDL.SDL_BUTTON_X1:
					butt = PointerButton.ExtendedButton1;
					break;
				case (byte)SDL.SDL_BUTTON_X2:
					butt = PointerButton.ExtendedButton2;
					break;
			}

			var w = focusedWidget;
			while (w != null && !w.FireMouseButton (isButtonDownEvt, butt, ev.x, ev.y-(int)menuHeight))
				w = w.Parent;
		}

		void HandleKeyEvent(SDL.SDL_KeyboardEvent ev)
		{
			Key k;
			ModifierKeys mods;

			Sdl.KeyCodes.ConvertToXwtKey (ev.keysym, out k, out mods);

			var w = focusedWidget;
			if(ev.type == SDL.SDL_EventType.SDL_KEYDOWN)
				while (w != null && !w.FireKeyDown(k, (char)ev.keysym.unicode, mods, ev.repeat != 0, ev.timestamp))
					w = w.Parent;
			else
				while (w != null && !w.FireKeyUp(k, (char)ev.keysym.unicode, mods, ev.repeat != 0, ev.timestamp))
					w = w.Parent;
		}

		internal void HandleWindowEvent(SDL.SDL_Event ev)
		{
			WidgetBackend w;
			if (eventSink == null)
				return;

			switch (ev.type) {
				case SDL.SDL_EventType.SDL_KEYUP:
				case SDL.SDL_EventType.SDL_KEYDOWN:
					HandleKeyEvent (ev.key);
					break;

				case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
					if (hoveredWidget == null || hoveredWidget.CanGetFocus) {
						// TODO: Define 'focus' rules
						FocusWidget (hoveredWidget);
					}

					HandleMouseButtonEvent (ev.button);
					return;
				
				case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
					HandleMouseButtonEvent (ev.button);
					return;
				case SDL.SDL_EventType.SDL_MOUSEMOTION:
					if (!focused)
						return;

					int x = ev.motion.x, y = ev.motion.y;

					if (menu != null && y <= menu.Height) {
						if (hoveredWidget != null) {
							hoveredWidget.FireMouseLeave ();
							hoveredWidget = null;
						}

						//TODO: Menu implementation
						return;
					}

					w = GetWidgetAt ((double)x, (double)y);
					if (w != hoveredWidget) {
						if (hoveredWidget != null)
							hoveredWidget.FireMouseLeave ();
						hoveredWidget = w;
						if (hoveredWidget != null)
							hoveredWidget.FireMouseEnter ();
					}

					while (w != null && !w.FireMouseMoved (ev.motion.timestamp, x-(int)padding.Left, y-(int)menuHeight-(int)padding.Top))
						w = w.Parent;
					return;
				case SDL.SDL_EventType.SDL_MOUSEWHEEL:
					if(hoveredWidget!=null)
					{
						ScrollDirection dir;
						if(ev.wheel.y > 0)
							dir = ScrollDirection.Up;
						else
							dir = ScrollDirection.Down;

						if(ev.wheel.x > 0)
							dir |= ScrollDirection.Right;
						else if(ev.wheel.x < 0)
							dir |= ScrollDirection.Left;

						// TODO: Are X/Y values absolute or relative scroll values?
						w = hoveredWidget;
						while (w != null && !w.FireMouseWheel (ev.wheel.timestamp, ev.wheel.x, ev.wheel.y, dir))
							w = w.Parent;
					}
					return;

				case SDL.SDL_EventType.SDL_WINDOWEVENT:
					break;
				default:
					return;
			}

			switch (ev.window.windowEvent) {
				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
					if (!eventSink.OnCloseRequested ()) {
						Dispose ();
					}
					break;
				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
					eventSink.OnHidden ();
					break;
				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
					SDL.SDL_GetWindowSize (window, out Width, out Height);
					UpdateViewPort ();
					Invalidate ();
					eventSink.OnShown ();
					break;
				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
					Invalidate ();
					break;
				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
					int width, h;
					SDL.SDL_GetWindowSize (window, out width, out h);
					eventSink.OnBoundsChanged (new Rectangle ((double)ev.window.data1, (double)ev.window.data2, (double)width, (double)h));
					break;
				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
					int x, y;
					SDL.SDL_GetWindowPosition (window, out x, out y);

					Width = ev.window.data1;
					Height = ev.window.data2;

					UpdateViewPort ();

					UpdateChildBounds();
					Invalidate ();

					eventSink.OnBoundsChanged (new Rectangle ((double)x, (double)y, (double)ev.window.data1, (double)ev.window.data2));

					oldWidth = ev.window.data1;
					oldHeight = ev.window.data2;
					break;

				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
					WindowHoveredByMouse = this;
					break;
				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
					WindowHoveredByMouse = null;
					break;
				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
					focused = false;
					break;
				case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
					focused = true;
					break;
			}
		}

		public void Invalidate()
		{
			Invalidate (Bounds);
		}

		public void Invalidate(Rectangle region)
		{
			invalidatedRegion = region;
			redraw = true;
		}

		internal bool Draw()
		{
			if (!redraw)
				return false;
			redraw = false;

			using (var cairoSurface = new Cairo.ImageSurface (windowSurface.pixels, Cairo.Format.RGB24, windowSurface.w, windowSurface.h, windowSurface.pitch))
			using(var cctxt = new Cairo.Context (cairoSurface))
			using(var ctxt = new CairoBackend.CairoContextBackend(1) { TempSurface = cairoSurface, Context =  cctxt })
			{
				if (menu != null)
					menu.Draw (ctxt, Width);

				if (child != null)
					child.Draw (ctxt, new Rectangle (padding.Left, padding.Top+menuHeight, Width - padding.Right, Height - menuHeight - padding.Bottom));

				SDL.SDL_UpdateWindowSurface (window);
			}

			return true;
		}

		public void SetFocusedWidget(int Id)
		{
			if (focusedWidget != null) {
				focusedWidget.FireLostFocus ();
			}
			focusedWidget = WidgetBackend.GetById(Id);
			if (focusedWidget != null)
				focusedWidget.FireGainedFocus ();
		}

		void UpdateChildBounds()
		{
			if (child == null)
				return;

			// The padding does NOT affect the menu position - only the child position!
			child.OnBoundsChanged (padding.Left, 
				padding.Top + menuHeight, 
				(double)Width-padding.Right, 
				(double)Height - menuHeight - padding.Bottom);
		}
		#endregion

		#region IWindowBackend implementation

		public void SetChild (IWidgetBackend child)
		{
			this.child = child as WidgetBackend;
			this.child.ParentWindow = this;
			UpdateChildBounds ();
			Invalidate ();
		}

		public void SetMainMenu (IMenuBackend menu)
		{
			this.menu = menu as MenuBackend;
			menuHeight = menu == null ? 0 : (int)this.menu.Height;
			UpdateChildBounds ();
			Invalidate ();
		}

		public Rectangle Padding {get{ return padding; }}
		public void SetPadding (double left, double top, double right, double bottom)
		{
			padding = new Rectangle (left, top, right, bottom);
			UpdateChildBounds ();
		}

		public void GetMetrics (out Size minSize, out Size decorationSize)
		{
			minSize = new Size(padding.Right, padding.Bottom);

			if(menu != null)
				minSize.Height+=menu.Height;

			decorationSize = new Size ();
		}

		public void SetMinSize (Size size)
		{
			SDL_.SDL_SetWindowMinimumSize (window, (int)size.Width, (int)size.Height);
		}

		#endregion

		#region IChildPlacementHandler implementation

		public void UpdateChildPlacement (IWidgetBackend childBackend)
		{
			if(childBackend==child)
				Invalidate ();
		}

		#endregion

		#region IWindowFrameBackend implementation

		public void Initialize (IWindowFrameEventSink eventSink)
		{
			this.eventSink = eventSink;

			window = SDL.SDL_CreateWindow (null, 0, 0, 400, 300, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE/* | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL*/);

			if(window == IntPtr.Zero)
				throw new SdlException ();

			id = SDL.SDL_GetWindowID (window);

			windowCache.Add (id, new WeakReference (this));
		}

		public virtual void Dispose ()
		{
			SDL.SDL_DestroyWindow (window);

			windowCache.Remove (id);
		}

		public void Move (double x, double y)
		{
			SDL.SDL_SetWindowPosition (window, (int)x, (int)y);
		}

		public void SetSize (double width, double height)
		{
			SDL.SDL_SetWindowSize (window, (int)width, (int)height);
		}

		public void SetTransientFor (IWindowFrameBackend window)
		{
			throw new NotImplementedException ();
		}

		public void SetIcon (ImageDescription image)
		{
			throw new NotImplementedException ();
		}

		public void Present ()
		{
			SDL.SDL_ShowWindow (window);
		}

		public Rectangle Bounds {
			get {
				int x, y, w, h;
				SDL.SDL_GetWindowPosition (window, out x, out y);
				SDL.SDL_GetWindowSize (window, out w, out h);
				return new Rectangle ((double)x, (double)y, (double)w, (double)h);
			}
			set {
				Move (value.X, value.Y);
				SetSize (value.Width, value.Height);
			}
		}

		public bool Visible {
			get {
				return (SDL.SDL_GetWindowFlags (window) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN) != 0;
			}
			set {
				if (value)
					SDL.SDL_ShowWindow (window);
				else
					SDL.SDL_HideWindow (window);
			}
		}

		public string Title {
			get {
				return SDL.SDL_GetWindowTitle (window);
			}
			set {
				SDL.SDL_SetWindowTitle (window, value);
			}
		}

		public bool Decorated {
			get {
				return (SDL.SDL_GetWindowFlags (window) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS) != 0;
			}
			set {
				SDL.SDL_SetWindowBordered (window, value ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
			}
		}

		public bool ShowInTaskbar {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool Resizable {
			get {
				return (SDL.SDL_GetWindowFlags (window) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0;
			}
			set {
				/*
				 * Perhaps destroy old window, reallocate it with(out) the DECORATED flag?
				 * What about restoring drawing contexts?
				 */
				throw new NotImplementedException ();
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

		public bool FullScreen {
			get {
				return (SDL.SDL_GetWindowFlags (window) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0;
			}
			set {
				if (SDL.SDL_SetWindowFullscreen (window, value ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0u) != 0)
					throw new SdlException();
			}
		}

		public object Screen {
			get {
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

