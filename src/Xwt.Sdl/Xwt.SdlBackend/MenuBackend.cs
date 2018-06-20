//
// MenuBackend.cs
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
using System.Collections.Generic;
using Xwt.Backends;

namespace Xwt.Sdl
{
	public class MenuBackend : WidgetBackend, IMenuBackend
	{
		List<MenuItemBackend> items = new List<MenuItemBackend> ();


		public MenuBackend ()
		{
			Height = WidgetStyles.Instance.MenuBarHeight;
		}

		public void DrawAsMenuBar(CairoBackend.CairoContextBackend c,int width)
		{
			c.Context.SetSourceRGB (1, 0, 0);
			c.Rectangle (0, 0, (double)width, (double)Height);
			c.Fill ();
		}

		#region IMenuBackend implementation

		public void InsertItem (int index, IMenuItemBackend menuItem)
		{
			if (index >= items.Count)
				items.Add ((MenuItemBackend)menuItem);
			else
				items.Insert (index, (MenuItemBackend)menuItem);
		}

		public void RemoveItem (IMenuItemBackend menuItem)
		{
			items.Remove (menuItem as MenuItemBackend);
		}

		public void Popup ()
		{
			throw new NotImplementedException ();
		}

		public void Popup (IWidgetBackend widget, double x, double y)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

