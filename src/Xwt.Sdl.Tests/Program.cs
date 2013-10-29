//
// Program.cs
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
using Xwt;
using Xwt.Drawing;

namespace Xwt.Sdl.Tests
{
	class MyCanvas : Canvas
	{
		public double XX;

		public MyCanvas()
		{
			CanGetFocus = false;
			base.BackgroundColor = Colors.LightSteelBlue;
		}

		protected override void OnMouseScrolled (MouseScrolledEventArgs args)
		{
			const double steps = 20;
			var w = Size.Width;
			if (args.Direction == ScrollDirection.Down) {
				XX -= w / steps;
				if (XX < 0)
					XX = 0;
			} else {
				XX += w / steps;
				if (XX >= w)
					XX = w - 1;
			}
			QueueDraw ();
		}

		protected override void OnDraw (Context ctx, Rectangle dirtyRect)
		{
			var b = Bounds;
			ctx.MoveTo (b.X, b.Y);
			ctx.SetColor (Colors.Black);

			using (var tl = new TextLayout (this)) {
				tl.Text = "hello";
				ctx.DrawTextLayout (tl, b.X, b.Y);
			}
			ctx.SetLineWidth (4);
			ctx.LineTo (dirtyRect.Right, dirtyRect.Bottom);
			ctx.MoveTo (0, 0);
			ctx.LineTo (XX % Size.Width, b.Bottom);
			//ctx.RelLineTo (-120, -150);
			//ctx.ClosePath ();
			//ctx.Fill ();
			ctx.Stroke ();

			//ctx.NewPath ();
			/*
			ctx.SetColor (Colors.Red);
			ctx.SetLineWidth (4);
			ctx.MoveTo (0, 0);
			ctx.LineTo (dirtyRect.Right/2, dirtyRect.Bottom);
			ctx.Stroke ();*/
		}

		protected override Size OnGetPreferredSize (SizeConstraint widthConstraint, SizeConstraint heightConstraint)
		{
			return new Size(400,300);
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			if (true)
				Application.Initialize ("Xwt.Sdl.SdlEngine, Xwt.Sdl");
			else
				Application.Initialize (ToolkitType.Gtk);

			var mw = new Window();
			//mw.Size = new Size (100, 150);

			mw.MainMenu = new Menu ();
			var c = new MyCanvas ();
			var labelT = new Label ("Hi derppp") { BackgroundColor = Colors.Red };

			var butt = new Button();
			butt.Label = "Button Test Caption";
			butt.Cursor = CursorType.Hand;
			butt.Image = Image.FromFile ("./ts.png");
			butt.Clicked+=(sender, e) => {(sender as Button).WidthRequest=200; labelT.Text = "##############";};
			mw.Content = c;

			c.AddChild (butt, 30, 120);

			butt = new Button ();
			butt.Label = "Hey ho";
			c.AddChild (butt, 10, 90);
			butt.ExpandHorizontal = true;
			c.AddChild (labelT,100,50);

			c.MouseMoved += (sender, e) => mw.Title = string.Format("x={0}\ty={1}",e.X, e.Y);
			//c.MouseEntered += (sender, e) => mw.Title = "Canvas";
			c.MouseExited += (sender, e) =>  mw.Title = "------";
			mw.Title = "SDL2 Test!";
			mw.CloseRequested+=
				(sender, a) => Application.Exit();
			mw.Show();

			/*
			var mw2 = new Window ();
			bool bb=true;
			mw2.Size = new Size (500, 100);
			mw2.Title = "Shallow";
			mw2.Show ();
			*/
			Application.Run ();
		}
	}
}
