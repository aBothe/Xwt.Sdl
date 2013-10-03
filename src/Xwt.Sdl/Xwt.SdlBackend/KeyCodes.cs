//
// KeyCodes.cs
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
using SDL2;

namespace Xwt.Sdl
{
	public class KeyCodes
	{
		public static void ConvertToXwtKey(SDL.SDL_Keysym s, out Key k, out ModifierKeys mod)
		{
			switch (s.mod) {
				case SDL.SDL_Keymod.KMOD_ALT:
					mod = ModifierKeys.Alt;
					break;
				case SDL.SDL_Keymod.KMOD_CTRL:
					mod = ModifierKeys.Control;
					break;
				case SDL.SDL_Keymod.KMOD_SHIFT:
					mod = ModifierKeys.Shift;
					break;
				case SDL.SDL_Keymod.KMOD_GUI:
					mod = ModifierKeys.Command;
					break;
				default:
					mod = ModifierKeys.None;
					break;
			}

			switch (s.sym) {
				case SDL.SDL_Keycode.SDLK_SPACE:
					k = Key.Space;
					break;
				case SDL.SDL_Keycode.SDLK_KP_SPACE:
					k = Key.NumPadSpace;
					break;
				case SDL.SDL_Keycode.SDLK_TAB:
					k = Key.Tab;
					break;
				case SDL.SDL_Keycode.SDLK_KP_TAB:
					k = Key.NumPadTab;
					break;
				case SDL.SDL_Keycode.SDLK_RETURN2:
				case SDL.SDL_Keycode.SDLK_RETURN:
					k = Key.Return;
					break;
				default:
					k = Key.a;
					break;
			}
		}
	}
}

