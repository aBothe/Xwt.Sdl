//
// SdlDesktopBackend.cs
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
using System.Collections.Generic;

namespace Xwt.Sdl
{
	public class SdlDesktopBackend : DesktopBackend
	{
		#region implemented abstract members of DesktopBackend

		public override Point GetMouseLocation ()
		{
			//TODO: Big problem: This is just the position related to the focused window - so add the window's position to it

			int mx, my, wx, wy;
			SDL.SDL_GetMouseState (out mx, out my);

			var activeWin = SDL.SDL_GetMouseFocus ();
			SDL.SDL_GetWindowPosition (activeWin, out wx, out wy); 

			return new Point ((double)(mx+wx), (double)(my+wy));
		}

		public class Screen
		{
			public int id;
			public Rectangle Bounds;
		}

		public override IEnumerable<object> GetScreens ()
		{
			var screenCount = SDL.SDL_GetNumVideoDisplays ();

			SDL.SDL_Rect bounds;
			for (int i = 0; i < screenCount; i++) {
				SDL.SDL_GetDisplayBounds (i, out bounds);
				yield return new Screen { 
					id = i,
					Bounds = new Rectangle((double)bounds.x, (double)bounds.y, (double)bounds.w, (double)bounds.h) 
				};
			}
		}

		public override bool IsPrimaryScreen (object backend)
		{
			return (backend as Screen).id == 0;
		}

		public override Rectangle GetScreenBounds (object backend)
		{
			return (backend as Screen).Bounds;
		}

		public override Rectangle GetScreenVisibleBounds (object backend)
		{
			return GetScreenBounds (backend);
		}

		public override string GetScreenDeviceName (object backend)
		{
			return string.Format ("Display {0}", (backend as Screen).id);
		}

		#endregion
	}
}

