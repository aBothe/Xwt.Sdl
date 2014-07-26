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
using System.Collections.Generic;
using Xwt.Drawing;

namespace Xwt.Sdl
{
	public class NotebookBackend : WidgetBackend,INotebookBackend
	{
		#region Properties
		int currentTab = 0;
		NotebookTabOrientation tabOrientation = NotebookTabOrientation.Top;
		public Notebook Notebook {get{ return Frontend as Notebook; }}
		public INotebookEventSink NotebookEventSink {get{ return EventSink as INotebookEventSink; }}

		readonly List<TabHead> TabHeaders = new List<TabHead>();
		public WidgetBackend CurrentChildWidget {get{ return Notebook.Tabs.Count == 0 || CurrentTab < 0 ? null : Notebook.Tabs [CurrentTab].Child.GetBackend() as WidgetBackend; } } 
		#endregion

		class TabHead : WidgetBackend
		{
			readonly Label HeadLabel = new Label();
			public NotebookBackend noteBook{get{return Parent as NotebookBackend;}}
			public readonly NotebookTab Tab;

			public TabHead(NotebookBackend noteBook, NotebookTab tab)
			{
				Tab = tab;
				Parent = noteBook;
				var ll = (HeadLabel.GetBackend() as LabelBackend);
				ll.Parent = this;
				Text = tab.Label;
			}

			public string Text
			{
				get{ return HeadLabel.Text; }
				set{ 
					HeadLabel.Text = value;
					Invalidate ();
				}
			}

			public override Size GetPreferredSize (Cairo.Context c, double maxX, double maxY)
			{
				var ws = WidgetStyles.Instance;
				var xPad = ws.NotebookTabHeaderPadding.Right;
				var yPad = ws.NotebookTabHeaderPadding.Bottom;

				var ext = (HeadLabel.GetBackend () as LabelBackend).GetPreferredSize(c,maxX - xPad, maxY - yPad);

				ext.Width += xPad;
				ext.Height += yPad;

				return ext;
			}

			public override void Draw (CairoContextBackend c, Rectangle rect)
			{
				bool isCurrent = Tab == noteBook.Notebook.CurrentTab;
				var ws = WidgetStyles.Instance;

				double absX, absY;
				GetAbsoluteLocation (out absX, out absY);

				// Background
				{
					c.Context.NewPath ();

					c.Context.MoveTo (absX, absY);
					c.Context.RelLineTo (Width, 0);
					c.Context.RelLineTo (0, Height);
					c.Context.RelLineTo (-Width, 0);
					c.Context.RelLineTo (0, -Height);

					if (isCurrent)
						c.Context.SetColor (new Color (ws.ButtonDefaultGrey,ws.ButtonDefaultGrey,ws.ButtonDefaultGrey));
					else
						c.Context.SetColor (new Color (ws.ButtonClickedGrey,ws.ButtonClickedGrey,ws.ButtonClickedGrey));

					c.Context.Fill ();
				}

				// Border
				{
					c.Context.NewPath ();
					if (!isCurrent) {
						c.Context.Rectangle (absX, absY, Width, Height);
					} else {
						switch (noteBook.TabOrientation) {
							default: // omit bottom border
								c.Context.MoveTo (absX, absY + Height);
								c.Context.RelLineTo (0.0, -Height);
								c.Context.RelLineTo (Width, 0);
								c.Context.RelLineTo (0, Height);
								break;
							case NotebookTabOrientation.Bottom:
								c.Context.MoveTo (absX, absY);
								c.Context.RelLineTo (0.0, Height);
								c.Context.RelLineTo (Width, 0);
								c.Context.RelLineTo (0, -Height);
								break;
							case NotebookTabOrientation.Left:
								c.Context.MoveTo (absX + Width, absY);
								c.Context.RelLineTo (-Width, 0);
								c.Context.RelLineTo (0, Height);
								c.Context.RelLineTo (Width, 0);
								break;
							case NotebookTabOrientation.Right:
								c.Context.MoveTo (absX, absY);
								c.Context.RelLineTo (Width, 0);
								c.Context.RelLineTo (0, Height);
								c.Context.RelLineTo (-Width, 0);
								break;
						}
					}

					c.Context.LineWidth = 1;
					c.Context.SetColor (ws.NotebookBorderColor);
					c.Context.Stroke ();
				}

				// Label
				{
					var ll = HeadLabel.GetBackend () as LabelBackend;
					ll.Draw (c, rect);
				}
			}

			internal override void OnBoundsChanged (double x, double y, double width, double height)
			{
				base.OnBoundsChanged (x, y, width, height);
				var ll = HeadLabel.GetBackend () as LabelBackend;
				var ws = WidgetStyles.Instance;
				ll.OnBoundsChanged(
					ws.NotebookTabHeaderPadding.Left, ws.NotebookTabHeaderPadding.Top, 
					Math.Max(0.0, width - ws.NotebookTabHeaderPadding.Right), Math.Max(0.0, height - ws.NotebookTabHeaderPadding.Bottom));
			}
		}

		#region INotebookBackend implementation

		public void Add (IWidgetBackend widget, NotebookTab tab)
		{
			if (widget is WidgetBackend)
				(widget as WidgetBackend).Parent = this;
			TabHeaders.Add (new TabHead(this, tab));
			RealignTabHeaders ();
			Invalidate ();
		}

		public void Remove (IWidgetBackend widget)
		{
			foreach (var th in TabHeaders)
				if (th.Tab.Child.GetBackend () == widget) {
					TabHeaders.Remove (th);
					break;
				}
			RealignTabHeaders ();
			Invalidate ();
		}

		public void UpdateLabel (NotebookTab tab, string hint)
		{
			foreach (var th in TabHeaders)
				if (th.Tab == tab) {
					th.Text = tab.Label;
					return;
				}
		}

		public int CurrentTab {
			get {
				if (currentTab >= Notebook.Tabs.Count)
					currentTab = Notebook.Tabs.Count - 1;
				return currentTab;
			}
			set {
				if (value < 0)
					value = 0;
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
			if (childBackend is WidgetBackend)
				(childBackend as WidgetBackend).Parent = this;
			Invalidate ();
		}

		#endregion

		public override IEnumerable<WidgetBackend> Children {
			get {
				foreach (var ch in Notebook.Tabs)
					if(ch.Child != null)
						yield return ch.Child.GetBackend() as WidgetBackend;

				foreach (var tab in TabHeaders)
					yield return tab;
			}
		}

		public override WidgetBackend GetChildAt (double x, double y)
		{
			var w = CurrentChildWidget;
			if (w != null && w.AbsoluteBounds.Contains (x, y))
				return w;

			foreach (var tab in TabHeaders)
				if (tab.AbsoluteBounds.Contains (x, y))
					return tab;

			return null;
		}

		Rectangle CalculateChildArea()
		{
			double maxTabW = 0, maxTabH = 0;
			var ws = WidgetStyles.Instance;

			foreach (var tab in this.TabHeaders) {
				var sz = tab.GetPreferredSize(SizeConstraint.Unconstrained, SizeConstraint.Unconstrained);
				if (sz.Width > maxTabW)
					maxTabW = sz.Width;
				if (sz.Height > maxTabH)
					maxTabH = sz.Height;
			}

			double x = ws.NotebookChildPadding, y = ws.NotebookChildPadding, w, h;

			switch (TabOrientation) {
				default:
					y += maxTabH - 1;
					w = Width - ws.NotebookChildPadding - ws.NotebookChildPadding;
					h = Height - maxTabH - ws.NotebookChildPadding - ws.NotebookChildPadding;
					break;
				case NotebookTabOrientation.Bottom:
					w = Width - ws.NotebookChildPadding - ws.NotebookChildPadding;
					h = Height - maxTabH - ws.NotebookChildPadding - ws.NotebookChildPadding;
					break;
				case NotebookTabOrientation.Left:
					x += maxTabW - 1;
					w = Width - ws.NotebookChildPadding - ws.NotebookChildPadding - maxTabW;
					h = Height - ws.NotebookChildPadding - ws.NotebookChildPadding;
					break;
				case NotebookTabOrientation.Right:
					w = Width - ws.NotebookChildPadding - ws.NotebookChildPadding - maxTabW;
					h = Height - ws.NotebookChildPadding - ws.NotebookChildPadding;
					break;
			}

			return new Rectangle (x,y,Math.Max(0.0, w),Math.Max(0.0, h));
		}

		internal override void OnBoundsChanged (double x, double y, double width, double height)
		{
			base.OnBoundsChanged (x, y, width, height);

			// Realign children
			var chArea = CalculateChildArea();
			foreach (var tab in Notebook.Tabs)
				if(tab.Child != null)
					(tab.Child.GetBackend () as WidgetBackend).OnBoundsChanged (chArea.X, chArea.Y, chArea.Width, chArea.Height);

			RealignTabHeaders ();
		}

		void RealignTabHeaders()
		{
			var ws = WidgetStyles.Instance;
			double totalWidth = ws.NotebookTabHeadDistance, totalHeight = ws.NotebookTabHeadDistance;
			double maxTabW = 0, maxTabH = 0;
			double x, y;
			var sizes = new Dictionary<TabHead, Size> ();

			foreach (var tab in this.TabHeaders) {
				var sz = tab.GetPreferredSize(SizeConstraint.Unconstrained, SizeConstraint.Unconstrained);
				sizes [tab] = sz;
				totalWidth += sz.Width + ws.NotebookTabHeadDistance;
				totalHeight += sz.Height + ws.NotebookTabHeadDistance;

				if (sz.Width > maxTabW)
					maxTabW = sz.Width;
				if (sz.Height > maxTabH)
					maxTabH = sz.Height;
			}

			switch (TabOrientation) {
				default:
					x = 0;
					y = TabOrientation == NotebookTabOrientation.Bottom ? (Height - maxTabH) : 0;

					if (totalWidth > Width) {
						double avg = Width / TabHeaders.Count;
						foreach (var tab in TabHeaders) {
							tab.OnBoundsChanged (x, y, avg - ws.NotebookTabHeadDistance, sizes [tab].Height);
							x += avg;
						}
						return;
					}
					break;
				case NotebookTabOrientation.Left:
				case NotebookTabOrientation.Right:
					x = TabOrientation == NotebookTabOrientation.Right ? (Width - maxTabW) : 0;
					y = 0;

					if (totalHeight > Height) {
						double avg = Height / TabHeaders.Count;

						foreach (var tab in TabHeaders) {
							tab.OnBoundsChanged (x + (TabOrientation == NotebookTabOrientation.Left ? (maxTabW-sizes[tab].Width) : 0), y, sizes [tab].Width, avg - ws.NotebookTabHeadDistance);
							y += avg;
						}
						return;
					}
					break;
			}

			switch (TabOrientation) {
				case NotebookTabOrientation.Top:
				case NotebookTabOrientation.Bottom:
					foreach (var tab in TabHeaders) {
						tab.OnBoundsChanged (x, y, sizes [tab].Width, sizes [tab].Height);
						x += ws.NotebookTabHeadDistance + sizes [tab].Width;
					}
					break;
				case NotebookTabOrientation.Left:
					foreach (var tab in TabHeaders) {
						tab.OnBoundsChanged (x + maxTabW-sizes[tab].Width, y, sizes [tab].Width, sizes [tab].Height);
						y += ws.NotebookTabHeadDistance + sizes[tab].Height;
					}
					break;
				case NotebookTabOrientation.Right:
					foreach (var tab in TabHeaders) {
						tab.OnBoundsChanged (x, y, sizes [tab].Width, sizes [tab].Height);
						y += ws.NotebookTabHeadDistance + sizes[tab].Height;
					}
					break;
			}
		}

		public override void Draw (CairoContextBackend c, Rectangle dirtyRect)
		{
			base.Draw (c, dirtyRect);

			var w = CurrentChildWidget;
			if (w == null)
				return;

			// Optionally only draw the child item incrementally
			var absBounds = w.AbsoluteBounds;
			if (absBounds.Contains (dirtyRect)) {
				// Presumes that no current-tab change has been done before -- this would invalidate the entire widget area
				w.Draw (c, dirtyRect);
				return;
			}

			// Draw content area
			{
				var ws = WidgetStyles.Instance;

				c.Context.NewPath ();
				c.Context.Rectangle (
					absBounds.X - ws.NotebookChildPadding, absBounds.Y - ws.NotebookChildPadding, 
					w.Width + 2*ws.NotebookChildPadding, w.Height + 2*ws.NotebookChildPadding);

				// Background
				c.Context.SetColor (new Color (ws.ButtonDefaultGrey,ws.ButtonDefaultGrey,ws.ButtonDefaultGrey));
				c.Context.FillPreserve ();

				// Border
				c.Context.LineWidth = 1;
				c.Context.SetColor (ws.NotebookBorderColor);
				c.Context.Stroke ();
			}

			// Draw tabs
			{
				// Inactive ones
				for (int i=0; i < TabHeaders.Count; i++)
					if(i != currentTab)
						TabHeaders[i].Draw (c, TabHeaders[i].AbsoluteBounds.Intersect (dirtyRect));

				// Current
				TabHeaders[currentTab].Draw (c, TabHeaders[currentTab].AbsoluteBounds.Intersect (dirtyRect));
			}

			// Child
			w.Draw (c, absBounds.Intersect (dirtyRect));
		}
	}
}

