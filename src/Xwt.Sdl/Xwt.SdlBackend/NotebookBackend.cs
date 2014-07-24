//
// NotebookBackend.cs
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
using Xwt.CairoBackend;

namespace Xwt.Sdl
{
	public class NotebookBackend : WidgetBackend,INotebookBackend
	{
		#region Properties
		int currentTab = -1;
		NotebookTabOrientation tabOrientation = NotebookTabOrientation.Top;
		public Notebook Notebook {get{ return Frontend as Notebook; }}
		public INotebookEventSink NotebookEventSink {get{ return EventSink as INotebookEventSink; }}
		#endregion

		#region INotebookBackend implementation

		public void Add (IWidgetBackend widget, NotebookTab tab)
		{
			if (currentTab == -1)
				CurrentTab = 0;
			Invalidate ();
		}

		public void Remove (IWidgetBackend widget)
		{
			if (currentTab == Notebook.Tabs.Count - 1)
				CurrentTab--;
			Invalidate ();
		}

		public void UpdateLabel (NotebookTab tab, string hint)
		{
			Invalidate ();
		}

		public int CurrentTab {
			get {
				return currentTab;
			}
			set {
				if (currentTab != (currentTab = value)) {
					NotebookEventSink.OnCurrentTabChanged ();
					Invalidate ();
				}
			}
		}

		public NotebookTabOrientation TabOrientation {
			get {
				return tabOrientation;
			}
			set {
				if (tabOrientation != (tabOrientation = value))
					Invalidate ();
			}
		}

		#endregion

		#region IChildPlacementHandler implementation

		public void UpdateChildPlacement (IWidgetBackend childBackend)
		{
			Console.WriteLine ("UpdateChildPlacement");
			Invalidate ();
		}

		#endregion


		public override void Draw (CairoContextBackend c, Rectangle dirtyRect)
		{
			base.Draw (c, dirtyRect);

			WidgetBackend w;
			var children = Notebook.Tabs;
			if (children.Count == 0)
				return;

			if (currentTab >= 0 && currentTab < children.Count) {
				w = children [currentTab].Child.GetBackend() as WidgetBackend;

				var absBounds = w.AbsoluteBounds;
				if (absBounds.Contains (dirtyRect)) {
					w.Draw (c, dirtyRect.Intersect (absBounds));
					return;
				}
			}
		}
	}
}

