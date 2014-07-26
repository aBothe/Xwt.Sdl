//
// WidgetStyles.cs
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
using Xwt.Drawing;

namespace Xwt.Sdl
{
	public class WidgetStyles
	{
		static WidgetStyles inst;
		public static WidgetStyles Instance
		{
			get{ 
				if (inst != null)
					return inst;

				return inst = new WidgetStyles ();
			}
		}

		private WidgetStyles() {}

		#region Common widget styles
		public Color FocusDashBorderColor = Colors.Black;
		public double[] FocusDashLine = new[]{ 1.0,2.0 };
		#endregion

		#region Button styles
		public Color ButtonBorderColor = new Color(0.6,0.6,0.6,1);
		public double ButtonBorderLineWidth = 1;

		public double ButtonDefaultGrey = 0.95;
		public double ButtonInsensitiveGrey = 0.7;
		public double ButtonClickedGrey = 0.9;
		public double ButtonHoveredGrey = 1;

		public Color ButtonLabelColor = Colors.Black;
		public Color ButtonInsensitiveLabelColor = new Color(0.5,0.5,0.5);
		#endregion

		#region Label styles
		public double LabelYPadding = 3;
		public double LabelXPadding = 4;
		#endregion

		#region Notebook Styles
		public Color NotebookBorderColor = new Color(0.6,0.6,0.6,1);
		public Rectangle NotebookTabHeaderPadding = new Rectangle(3,2,3,2);
		public double NotebookChildPadding = 3;
		public double NotebookTabHeadDistance = 3;
		#endregion
	}
}

