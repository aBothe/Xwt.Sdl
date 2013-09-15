//
// MyClass.cs
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
using SDL2;
using Xwt.Backends;
using System.Threading;

namespace Xwt.Sdl.Backends
{
	public class SdlEngine : ToolkitEngineBackend
	{
		ManualResetEvent pendingEvents = new ManualResetEvent(true);
		bool run;

		public override void InitializeApplication ()
		{
			OpenTK.Graphics.OpenGL.GL.LoadAll ();
			if (SDL.SDL_Init (SDL.SDL_INIT_VIDEO) < 0)
				throw new SdlException ();
		}

		public override void InitializeBackends ()
		{
			//RegisterBackend<ICustomWidgetBackend, CustomWidgetBackend> ();
			RegisterBackend<IWindowBackend, WindowBackend> ();
			/*RegisterBackend<ILabelBackend, LabelBackend> ();
			RegisterBackend<IBoxBackend, BoxBackend> ();
			RegisterBackend<IButtonBackend, ButtonBackend> ();
			RegisterBackend<INotebookBackend, NotebookBackend> ();
			RegisterBackend<ITreeViewBackend, TreeViewBackend> ();
			RegisterBackend<ITreeStoreBackend, TreeStoreBackend> ();
			RegisterBackend<IListViewBackend, ListViewBackend> ();
			RegisterBackend<IListStoreBackend, ListStoreBackend> ();
			RegisterBackend<ICanvasBackend, CanvasBackend> ();
			RegisterBackend<ImageBackendHandler, ImageHandler> ();
			RegisterBackend<Xwt.Backends.ContextBackendHandler, CairoContextBackendHandler> ();
			RegisterBackend<TextLayoutBackendHandler, GtkTextLayoutBackendHandler> ();
			RegisterBackend<DrawingPathBackendHandler, CairoContextBackendHandler> ();
			RegisterBackend<GradientBackendHandler, CairoGradientBackendHandler> ();
			RegisterBackend<FontBackendHandler, GtkFontBackendHandler> ();
			RegisterBackend<IMenuBackend, MenuBackend> ();
			RegisterBackend<IMenuItemBackend, MenuItemBackend> ();
			RegisterBackend<ICheckBoxMenuItemBackend, CheckBoxMenuItemBackend> ();
			RegisterBackend<IRadioButtonMenuItemBackend, RadioButtonMenuItemBackend> ();
			RegisterBackend<ISeparatorMenuItemBackend, SeparatorMenuItemBackend> ();
			RegisterBackend<IScrollViewBackend, ScrollViewBackend> ();
			RegisterBackend<IComboBoxBackend, ComboBoxBackend> ();
			RegisterBackend<IDesignerSurfaceBackend, DesignerSurfaceBackend> ();
			RegisterBackend<IMenuButtonBackend, MenuButtonBackend> ();
			RegisterBackend<ITextEntryBackend, TextEntryBackend> ();
			RegisterBackend<IToggleButtonBackend, ToggleButtonBackend> ();
			RegisterBackend<IImageViewBackend, ImageViewBackend> ();
			RegisterBackend<IAlertDialogBackend, AlertDialogBackend> ();
			RegisterBackend<ICheckBoxBackend, CheckBoxBackend> ();
			RegisterBackend<IFrameBackend, FrameBackend> ();
			RegisterBackend<ISeparatorBackend, SeparatorBackend> ();
			RegisterBackend<IDialogBackend, DialogBackend> ();
			RegisterBackend<IComboBoxEntryBackend, ComboBoxEntryBackend> ();
			RegisterBackend<ClipboardBackend, GtkClipboardBackend> ();
			RegisterBackend<ImagePatternBackendHandler, GtkImagePatternBackendHandler> ();
			RegisterBackend<ImageBuilderBackendHandler, ImageBuilderBackend> ();
			RegisterBackend<IScrollAdjustmentBackend, ScrollAdjustmentBackend> ();
			RegisterBackend<IOpenFileDialogBackend, OpenFileDialogBackend> ();
			RegisterBackend<ISaveFileDialogBackend, SaveFileDialogBackend> ();
			RegisterBackend<ISelectFolderDialogBackend, SelectFolderDialogBackend> ();
			RegisterBackend<IPanedBackend, PanedBackend> ();
			RegisterBackend<ISelectColorDialogBackend, SelectColorDialogBackend> ();
			RegisterBackend<IListBoxBackend, ListBoxBackend> ();
			RegisterBackend<IStatusIconBackend, StatusIconBackend> ();
			RegisterBackend<IProgressBarBackend, ProgressBarBackend> ();
			RegisterBackend<IPopoverBackend, PopoverBackend> ();
			RegisterBackend<ISpinButtonBackend, SpinButtonBackend> ();
			RegisterBackend<IDatePickerBackend, DatePickerBackend> ();
			RegisterBackend<ILinkLabelBackend, LinkLabelBackend> ();
			RegisterBackend<ISpinnerBackend, SpinnerBackend> ();
			RegisterBackend<IRichTextViewBackend, RichTextViewBackend> ();
			RegisterBackend<IExpanderBackend, ExpanderBackend> ();
			RegisterBackend<DesktopBackend, GtkDesktopBackend> ();
			RegisterBackend<IEmbeddedWidgetBackend, EmbeddedWidgetBackend> ();
			RegisterBackend<ISegmentedButtonBackend, SegmentedButtonBackend> ();
			RegisterBackend<ISliderBackend, SliderBackend> ();
			RegisterBackend<IRadioButtonBackend, RadioButtonBackend> ();
			RegisterBackend<IScrollbarBackend, ScrollbarBackend> ();
			RegisterBackend<IPasswordEntryBackend, PasswordEntryBackend> ();
			*/
		}

		public override void RunApplication ()
		{
			run = true;
			SDL.SDL_Event ev;
			while (run) {
				pendingEvents.Reset ();
				while (SDL.SDL_PollEvent (out ev) != 0) {
					if (ev.type == SDL.SDL_EventType.SDL_QUIT) {
						run = false;
						return;
					}
					HandleEvent (ev);
				}
				pendingEvents.Set ();

				if(!Draw())
					Thread.Sleep (15);
			}
		}

		bool HandleEvent(SDL.SDL_Event ev)
		{
			switch (ev.type) {
				case SDL.SDL_EventType.SDL_WINDOWEVENT:
					WeakReference wr;
					if (WindowBackend.windowCache.TryGetValue (ev.window.windowID, out wr) && wr.IsAlive)
						return (wr.Target as WindowBackend).HandleWindowEvent (ev);
					break;
			}

			return false;
		}

		bool Draw()
		{
			bool d=false;
			WindowBackend w;

			foreach (var kv in WindowBackend.windowCache) {
				w = kv.Value.Target as WindowBackend;
				if (w != null && w.Draw ())
					d = true;
			}

			return d;
		}

		public override void ExitApplication ()
		{
			run = false;
			SDL.SDL_Quit ();
		}

		public override void InvokeAsync (Action action)
		{
			ThreadPool.QueueUserWorkItem ((state) => action());
		}

		public override object TimerInvoke (Func<bool> action, TimeSpan timeSpan)
		{
			throw new NotImplementedException ();
		}

		public override void CancelTimerInvoke (object id)
		{
			throw new NotImplementedException ();
		}

		public override object GetNativeWidget (Widget w)
		{
			throw new NotImplementedException ();
		}

		public override void DispatchPendingEvents ()
		{
			pendingEvents.WaitOne ();
		}

		public override IWindowFrameBackend GetBackendForWindow (object nativeWindow)
		{
			throw new NotImplementedException ();
		}

		public override bool HasNativeParent (Widget w)
		{
			throw new NotImplementedException ();
		}
	}
}

