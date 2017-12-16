using System;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.EventManagement
{
	public class SpiritBeaconEventArgs : EventArgs
	{
		public SpiritBeaconEventArgs(EResult p_result)
		{
			Result = p_result;
		}

		public SpiritBeaconEventArgs(Scroll p_fromScroll)
		{
			Scroll = p_fromScroll;
		}

		public EResult Result { get; private set; }

		public Scroll Scroll { get; private set; }

		public enum EResult
		{
			CANCEL,
			TRAVEL,
			SET_POINT
		}
	}
}
