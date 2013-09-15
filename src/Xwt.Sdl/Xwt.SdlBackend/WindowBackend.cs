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

namespace Xwt.Sdl.Backends
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

		Rectangle padding;
		#endregion

		#region Extension
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
						eventSink.OnBoundsChanged (new Rectangle ((double)x, (double)y, (double)ev.window.data1, (double)ev.window.data2));
						break;
				}
			}
			return false;
		}

		internal bool Draw()
		{
			if (SDL.SDL_GL_MakeCurrent (window, ctxt) != 0)
				throw new SdlException ();

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			/*
			SDL.SDL_SetRenderDrawColor (renderer, 0, 0, 0, 0);

			SDL.SDL_RenderClear (renderer);
			*/
			GL.Begin (BeginMode.Triangles);
			GL.Vertex2 (0, 0);
			GL.Vertex2 (50, 50);
			GL.Vertex2 (200, -200);
			GL.End ();
			/*
			var rect = new SDL2.SDL.SDL_Rect();
			rect.x = 100;
			rect.y = 100;
			rect.w = 50;
			rect.h = 100;
			SDL.SDL_RenderFillRect (renderer, ref rect);
			*/

			SDL.SDL_GL_SwapWindow (window);

			return false;
			return true;
		}
		#endregion

		#region IWindowBackend implementation

		public void SetChild (IWidgetBackend child)
		{
			throw new NotImplementedException ();
		}

		public void SetMainMenu (IMenuBackend menu)
		{
			throw new NotImplementedException ();
		}

		public void SetPadding (double left, double top, double right, double bottom)
		{
			padding = new Rectangle (left, top, right, bottom);
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
			throw new NotImplementedException ();
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
				if (Decorated != value) {
					/*
					 * Perhaps destroy old window, reallocate it with(out) the DECORATED flag?
					 * What about restoring drawing contexts?
					 */
				}
				throw new NotImplementedException ();
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
				// See Decorated
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

