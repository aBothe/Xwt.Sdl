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
using Xwt.Backends;
using SDL2;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Xwt.Sdl
{
	public class WindowBackend : IWindowBackend
	{
		#region Properties
		internal static Dictionary<uint, WeakReference> windowCache = new Dictionary<uint, WeakReference>();
		IntPtr window;
		IntPtr ctxt;
		uint id;
		public uint WindowId {get{return id;}}
		public IWindowFrameEventSink eventSink;

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
		int focusedWidget;
		#endregion

		#region Extension
		static WindowBackend()
		{
			// Workaround, so that this "No current gl context" exception won't become thrown.
			GraphicsContext.CurrentContext = new IntPtr (1);
		}

		void UpdateViewPort()
		{
			SDL.SDL_GL_MakeCurrent (window, ctxt);
			GL.Viewport (0, 0, Width, Height);
			GL.MatrixMode (MatrixMode.Projection);
			GL.LoadIdentity ();
			GL.Ortho (0.0, (double)Width, (double)Height, 0.0, -1, 1);
			GL.MatrixMode (MatrixMode.Modelview);
		}

		internal bool HandleWindowEvent(SDL.SDL_Event ev)
		{
			if (eventSink != null) {
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
					case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
						int w, h;
						SDL.SDL_GetWindowSize (window, out w, out h);
						eventSink.OnBoundsChanged (new Rectangle ((double)ev.window.data1, (double)ev.window.data2, (double)w, (double)h));
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
				}
			}
			return false;
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

			SDL.SDL_GL_MakeCurrent (window, ctxt);

			// Is clearing the background really needed?
			if (!padding.IsEmpty) {
				GL.ClearColor (1f, 1f, 1f, 1f);
				GL.Clear (ClearBufferMask.ColorBufferBit);
			}

			GL.LoadIdentity ();

			if (menu != null) {
				menu.Draw (Width);
				GL.Translate (0.0, (float)menu.Height, 0.0);
			}

			if (child != null)
				child.Draw ();

			SDL.SDL_GL_SwapWindow (window);

			return true;
		}

		public void SetFocusedWidget(int Id)
		{
			var w = WidgetBackend.GetById (focusedWidget);
			if (w != null) {
				w.FireLostFocus ();
			}
			focusedWidget = Id;
			w = WidgetBackend.GetById (Id);
			if (w != null)
				w.FireGainedFocus ();
		}

		void UpdateChildBounds()
		{
			if (child == null)
				return;

			// The padding does NOT affect the menu position - only the child position!
			child.OnBoundsChanged (padding.Left, 
				padding.Top, 
				(double)Width-padding.Width, 
				(double)(Height-(menu == null ? 0 : menu.Height)) - padding.Height);
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
			UpdateChildBounds ();
			Invalidate ();
		}

		public void SetPadding (double left, double top, double right, double bottom)
		{
			padding = new Rectangle (left, top, right, bottom);
			UpdateChildBounds ();
		}

		public void GetMetrics (out Size minSize, out Size decorationSize)
		{
			minSize = new Size ();
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

			window = SDL.SDL_CreateWindow (null, 0, 0, 400, 300, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

			if(window == IntPtr.Zero)
				throw new SdlException ();

			ctxt = SDL.SDL_GL_CreateContext (window);
			if (ctxt == IntPtr.Zero)
				throw new SdlException ();

			SDL.SDL_GL_MakeCurrent (window, ctxt);
			SDL.SDL_GL_SetAttribute (SDL.SDL_GLattr.SDL_GL_MULTISAMPLESAMPLES, 8);
			SDL.SDL_GL_SetAttribute (SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);

			id = SDL.SDL_GetWindowID (window);

			windowCache.Add (id, new WeakReference (this));
		}

		public virtual void Dispose ()
		{
			SDL.SDL_GL_DeleteContext (ctxt);
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

