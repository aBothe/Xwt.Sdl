﻿//
// BoxBackend.cs
//
// Author:
//       Alexander Bothe <info@alexanderbothe.com>
//
// Copyright (c) 2014 Alexander Bothe
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
using System.Collections.Generic;
using Xwt.CairoBackend;

namespace Xwt.Sdl
{
	public class BoxBackend : WidgetBackend,IBoxBackend
	{
		public Box BoxFrontend {get{ return Frontend as Box; }}
		protected override bool WidgetSupportsFocusing { get { return false; } }

		public override IEnumerable<WidgetBackend> Children {
			get {
				foreach (var w in BoxFrontend.Children)
					yield return w.GetBackend () as WidgetBackend;
			}
		}

		#region IBoxBackend implementation

		public void Add (IWidgetBackend widget)
		{
			var w = widget as WidgetBackend;
			if (w == null)
				throw new ArgumentNullException ("widget");
			w.Parent = this;
		}

		public void Remove (IWidgetBackend widget)
		{
			var w = widget as WidgetBackend;
			if (w != null) {
				w.Parent = null;
				if(w.Visible)
					Invalidate (w.Bounds, false);
			}
		}

		public void SetAllocation (IWidgetBackend[] widget, Rectangle[] rect)
		{
			for (int i = widget.Length - 1; i >= 0; i--) {
				var w = widget [i] as WidgetBackend;
				w.OnBoundsChanged (rect [i].X, rect [i].Y, rect [i].Width, rect [i].Height);
			}
		}

		#endregion

		protected override void DrawInternally (CairoContextBackend c, Rectangle dirtyRect, double absX, double absY)
		{
			base.DrawInternally (c,dirtyRect, absX, absY);

			foreach (var w in Children) {
				w.Draw (c, dirtyRect, false);
			}
		}
	}
}

