//
// TextEntryBackend.cs
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
	public class TextEntryBackend : WidgetBackend, ITextEntryBackend
	{
		#region Properties
		string text;
		public TextEntry TextEntry {get{ return this.Frontend as TextEntry; }}
		Alignment alignment = Alignment.Start;
		string placeholderText;
		bool Readonlyness;
		bool showFrame;
		bool multiLine;

		int caretPosition;
		int caretLine;
		int caretColumn;

		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				// Set selection to index 0; length 0
				// Separate text into lines -> Fill buffer
				Invalidate ();
			}
		}

		public Alignment TextAlignment {
			get {
				return alignment;
			}
			set {
				alignment = value;
				Invalidate ();
			}
		}

		public string PlaceholderText {
			get {
				return placeholderText;
			}
			set {
				placeholderText = value;
				if (string.IsNullOrEmpty (text))
					Invalidate ();
			}
		}

		public bool ReadOnly {
			get {
				return Readonlyness;
			}
			set {
				if (Readonlyness != (Readonlyness = value))
					Invalidate ();
			}
		}

		public bool ShowFrame {
			get {
				return showFrame;
			}
			set {
				if (showFrame != (showFrame = value))
					Invalidate ();
			}
		}

		public bool MultiLine {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public int CursorPosition {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public int SelectionStart {
			get {
				return CursorPosition;
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public int SelectionLength {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public string SelectedText {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}
		#endregion

		public override Size GetPreferredSize (Cairo.Context fontExtentContext, double maxWidth, double maxHeight)
		{
			return base.GetPreferredSize (fontExtentContext, maxWidth, maxHeight);
		}

		internal override void OnBoundsChanged (double x, double y, double width, double height)
		{
			base.OnBoundsChanged (x, y, width, height);
		}

		protected override void SensitivityChanged ()
		{
			base.SensitivityChanged ();
		}
			
		internal override void FireGainedFocus ()
		{
			base.FireGainedFocus ();
		}

		internal override void FireLostFocus ()
		{
			base.FireLostFocus ();
		}

		internal override bool FireKeyDown (Key k, char ch, ModifierKeys mods, bool rep, uint timestamp)
		{
			return base.FireKeyDown (k, ch, mods, rep, timestamp);
		}

		internal override bool FireKeyUp (Key k, char ch, ModifierKeys mods, bool rep, uint timestamp)
		{
			return base.FireKeyUp (k, ch, mods, rep, timestamp);
		}

		internal override bool FireMouseButton (bool down, PointerButton butt, int x, int y, int multiplePress = 1)
		{
			return base.FireMouseButton (down, butt, x, y, multiplePress);
		}

		internal override bool FireMouseMoved (uint timestamp, int x, int y)
		{
			return base.FireMouseMoved (timestamp, x, y);
		}

		protected override void DrawInternally (CairoContextBackend c, Rectangle dirtyRect, double absX, double absY)
		{
			// Draw background
			base.DrawInternally (c,dirtyRect, absX, absY);

			// Draw border if required and if within dirtyRect
			if (showFrame) {

			}

			// Draw currently visible lines that are located within dirtyRect
			{
				// Foreach visible line (determined by scrolling states):

				// Incrementally draw unselected texts (before; after selection)

				// Incrementally draw selected text part

				// Draw caret if selection length is 0 && widget has focus
			}

		}


	}
}

