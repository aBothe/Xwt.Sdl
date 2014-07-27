//
// ListBoxBackend.cs
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

namespace Xwt.Sdl
{
	public class ListBoxBackend : WidgetBackend, IListBoxBackend
	{
		public void SetViews (CellViewCollection views)
		{
			throw new NotImplementedException ();
		}

		public void SetSource (IListDataSource source, IBackend sourceBackend)
		{
			throw new NotImplementedException ();
		}

		public void SetSelectionMode (SelectionMode mode)
		{
			throw new NotImplementedException ();
		}

		public void SelectAll ()
		{
			throw new NotImplementedException ();
		}

		public void UnselectAll ()
		{
			throw new NotImplementedException ();
		}

		public void SelectRow (int pos)
		{
			throw new NotImplementedException ();
		}

		public void UnselectRow (int pos)
		{
			throw new NotImplementedException ();
		}

		public void ScrollToRow (int row)
		{
			throw new NotImplementedException ();
		}

		public int[] SelectedRows {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool GridLinesVisible {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public IScrollControlBackend CreateVerticalScrollControl ()
		{
			throw new NotImplementedException ();
		}

		public IScrollControlBackend CreateHorizontalScrollControl ()
		{
			throw new NotImplementedException ();
		}

		public ScrollPolicy VerticalScrollPolicy {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public ScrollPolicy HorizontalScrollPolicy {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}


	}
}

