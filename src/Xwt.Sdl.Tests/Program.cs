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
			ctx.SetColor (Colors.Black);

			/*
			using (var tl = new TextLayout (this)) {
				tl.Text = "hello";
				ctx.DrawTextLayout (tl, b.X, b.Y);
			}*/
			ctx.SetLineWidth (4);
			ctx.MoveTo (dirtyRect.Left, dirtyRect.Top);
			ctx.LineTo (dirtyRect.Right, dirtyRect.Bottom);
			ctx.Stroke ();
			//ctx.MoveTo (0, 0);
			//ctx.LineTo (dirtyRect.Right, dirtyRect.Bottom);
			//ctx.RelLineTo (-120, -150);
			//ctx.ClosePath ();
			//ctx.Fill ();
			//ctx.Stroke ();

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

			Button butt;

			var tabs = new Notebook ();//tabs.TabOrientation = NotebookTabOrientation.Left;
			tabs.WidthRequest = 500;
			tabs.HeightRequest = 300;
			mw.Content = tabs;

			var scroll = new VScrollbar ();
			scroll.PageSize = 50;
			scroll.LowerValue = 0;
			scroll.UpperValue = 100;
			scroll.PageIncrement = 10;
			scroll.StepIncrement = 10;
			scroll.Value = 30;
			scroll.ValueChanged += (sender, e) => tabs.CurrentTab.Label = scroll.Value.ToString();

			tabs.Add (scroll,"Scroll!");
			//tabs.Add (new TextEntry { Text = "My Text\nNew line\nTHird line", MultiLine = true, ExpandVertical = true }, "Tab 3");

			butt = new Button ();
			butt.ExpandHorizontal = true;
			butt.ExpandVertical = true;
			butt.Label = "Tab 2 Button";
			tabs.Add (butt, "Tabz 2");
			//mw.Size = new Size (100, 150);


			//mw.MainMenu = new Menu ();
			var c = new MyCanvas ();
			c.ExpandHorizontal = true;
			c.ExpandVertical = true;
			tabs.Add (c, "Tab 1 #######################");

			var box = new VBox ();
			box.WidthRequest = 150;
			box.HeightRequest = 200;
			//box.BackgroundColor = Colors.GreenYellow;
			c.AddChild (box);

			var labelT = new Label ("Hi derppp") { BackgroundColor = Colors.Red };	box.PackStart (labelT);

			butt = new Button();
			butt.Font = butt.Font.WithWeight (FontWeight.Bold);
			butt.Label = "B";
			butt.Cursor = CursorType.Hand;
			butt.Image = Image.FromFile ("./ts.png");
			butt.Clicked+=(sender, e) => {
				(sender as Button).WidthRequest=40; 
				labelT.Text = "######################";
			};

			box.PackEnd (butt);

			butt = new Button ();
			butt.Label = "Hey ho";
			butt.ExpandHorizontal = true;

			box.PackEnd (butt);



			c.MouseMoved += (sender, e) => mw.Title = string.Format("x={0}\ty={1}",e.X, e.Y);
			//c.MouseEntered += (sender, e) => mw.Title = "Canvas";
			c.MouseExited += (sender, e) =>  mw.Title = "------";


			mw.Title = "SDL2 Test!";
			mw.CloseRequested+=
				(sender, a) => Application.Exit();
			mw.Show();



			Application.Run ();
		}
	}
}
