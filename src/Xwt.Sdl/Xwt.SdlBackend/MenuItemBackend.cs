using System;
using Xwt.Backends;

namespace Xwt.Sdl
{
	public class MenuItemBackend : IMenuItemBackend
	{
		string label;
		bool sensitive;
		bool mnemonic;
		bool visible;

		public string Label {
			get {
				return label;
			}

			set {
				if (label != (label = value))
					Invalidate ();
			}
		}

		public bool Sensitive {
			get {
				return sensitive;
			}

			set {
				if (sensitive != (sensitive = value))
					Invalidate ();
			}
		}

		public bool UseMnemonic {
			get {
				return mnemonic;
			}

			set {
				if (mnemonic != (mnemonic = value))
					Invalidate ();
			}
		}

		public string TooltipText { get; set; }

		public bool Visible {
			get {
				throw new NotImplementedException ();
			}

			set {
				throw new NotImplementedException ();
			}
		}

		public void Initialize (IMenuItemEventSink eventSink)
		{
		}

		public void SetImage (ImageDescription image)
		{
			
		}

		public void SetSubmenu (IMenuBackend menu)
		{

		}

		void Invalidate ()
		{
		}

		public void InitializeBackend (object frontend, ApplicationContext context)
		{
		}

		public void EnableEvent (object eventId)
		{
		}

		public void DisableEvent (object eventId)
		{
		}
	}
}

