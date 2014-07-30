//
// TextEntryBuffer.cs
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
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Xwt.Sdl
{
	public class TextEntryBuffer
	{
		readonly List<StringBuilder> Lines = new List<StringBuilder>(); 

		public string Text
		{
			get{ 
				var sb = new StringBuilder ();
				foreach(var line in Lines)
				{
					sb.AppendLine (line.ToString());
				}
				return sb.ToString ();
			}
			set{ 
				Lines.Clear ();
				if (string.IsNullOrEmpty (value))
					return;

				using (var sr = new StringReader (value)) {
					var eolSize = Environment.NewLine.Length;
					string lineText;
					int offset = 0;
					while((lineText = sr.ReadLine ()) != null)
						Lines.Add (new StringBuilder(lineText));
				}
			}
		}

		public int GetTextOffset(int line, int column)
		{
			if (line > Lines.Count)
				throw new IndexOutOfRangeException ("Invalid line");
			var ln = Lines [line];
			if (column > ln.Length)
				throw new IndexOutOfRangeException ("Invalid column");
			int offset = column;
			int eolSize = Environment.NewLine.Length;

			for (; line > 0;)
				offset += Lines [--line].Length + eolSize;

			return offset;
		}

		public void GetTextLocation(int offset, out int line, out int column)
		{
			if (offset < 0)
				throw new IndexOutOfRangeException ("offset");

			var lineCount = Lines.Count;
			if (lineCount == 1) {
				line = 0;
				if (Lines [0].Length > offset)
					throw new IndexOutOfRangeException ("offset");
				column = offset;
				return;
			}

			var eolSize = Environment.NewLine.Length;

			for (line = 0; offset >= 0; line++){
				if (line >= lineCount)
					throw new IndexOutOfRangeException ("offset");
				offset -= Lines [line].Length + eolSize;
			}

			column = offset + Lines [line].Length + eolSize;
			line--;
		}

		/// <summary>
		/// Inserts a string into the buffer.
		/// The string may contains line breaks.
		/// </summary>
		public void Insert(string s, int line, int col)
		{
			if (line >= Lines.Count)
				throw new IndexOutOfRangeException ("line");

			using (var sr = new StringReader (s)) {
				Lines [line].Insert (col, sr.ReadLine());
				string lineText;
				while ((lineText = sr.ReadLine ()) != null) {
					Lines.Insert (++line, new StringBuilder(lineText));
				}
			}
		}

		public void Insert(char c, int line, int col)
		{
			if (line >= Lines.Count)
				throw new IndexOutOfRangeException ("line");
			Lines [line].Insert (col, c);
		}

		public void Remove(int line, int col, int length)
		{
			if (line >= Lines.Count)
				throw new IndexOutOfRangeException ("line");

			var ln = Lines [line];
			if (line == Lines.Count-1 && ln.Length < length)
				throw new IndexOutOfRangeException ("length");
			ln.Remove (col, Math.Min (ln.Length-col, length));
			if (length + col <= ln.Length)
				return;
			col = 0;

			length -= ln.Length - col;

			while (length > 0) {

			}
		}
	}
}

