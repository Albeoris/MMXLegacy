using System;
using UnityEngine;

namespace Legacy.MMInput
{
	public class HotkeyEventArgs : EventArgs
	{
		public HotkeyEventArgs(EHotkeyType action, Boolean keyDown)
		{
			Action = action;
			KeyDown = keyDown;
		}

		public HotkeyEventArgs(EHotkeyType action, Boolean keyDown, KeyCode key) : this(action, keyDown)
		{
			Key = key;
		}

		public EHotkeyType Action { get; private set; }

		public Boolean KeyDown { get; private set; }

		public Boolean KeyUp => !KeyDown;

	    public KeyCode Key { get; private set; }
	}
}
