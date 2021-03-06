﻿//
// ListViewBackend.cs
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
	public class ListViewBackend : WidgetBackend,IListViewBackend
	{
		public ListViewBackend ()
		{
		}

		#region IListViewBackend implementation

		public int CurrentEventRow {
			get {
				throw new NotImplementedException ();
			}
		}

		public void SetSource (IListDataSource source, IBackend sourceBackend)
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

		public int GetRowAtPosition (Point p)
		{
			throw new NotImplementedException ();
		}

		public Rectangle GetCellBounds (int row, CellView cell, bool includeMargin)
		{
			throw new NotImplementedException ();
		}

		public int[] SelectedRows {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool BorderVisible {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public GridLines GridLinesVisible {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool HeadersVisible {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public int FocusedRow {
			get {
				throw new NotImplementedException ();
			}

			set {
				throw new NotImplementedException ();
			}
		}

		#endregion

		#region ITableViewBackend implementation

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

		#endregion

		#region IScrollableWidgetBackend implementation

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

		#endregion

		#region IColumnContainerBackend implementation

		public object AddColumn (ListViewColumn col)
		{
			throw new NotImplementedException ();
		}

		public void RemoveColumn (ListViewColumn col, object handle)
		{
			throw new NotImplementedException ();
		}

		public void UpdateColumn (ListViewColumn col, object handle, ListViewColumnChange change)
		{
			throw new NotImplementedException ();
		}

		public void StartEditingCell (int row, CellView cell)
		{
			throw new NotImplementedException ();
		}

		public Rectangle GetRowBounds (int row, bool includeMargin)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

