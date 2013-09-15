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

namespace Xwt.Sdl.Backends
{
	public class WindowBackend : IWindowBackend
	{


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
			throw new NotImplementedException ();
		}

		public void GetMetrics (out Size minSize, out Size decorationSize)
		{
			throw new NotImplementedException ();
		}

		public void SetMinSize (Size size)
		{
			throw new NotImplementedException ();
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
			throw new NotImplementedException ();
		}

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}

		public void Move (double x, double y)
		{
			throw new NotImplementedException ();
		}

		public void SetSize (double width, double height)
		{
			throw new NotImplementedException ();
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
			throw new NotImplementedException ();
		}

		public Rectangle Bounds {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool Visible {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public string Title {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool Decorated {
			get {
				throw new NotImplementedException ();
			}
			set {
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
				throw new NotImplementedException ();
			}
			set {
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
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
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
			throw new NotImplementedException ();
		}

		public void EnableEvent (object eventId)
		{
			throw new NotImplementedException ();
		}

		public void DisableEvent (object eventId)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

