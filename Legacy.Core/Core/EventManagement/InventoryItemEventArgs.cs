using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.EventManagement
{
	public class InventoryItemEventArgs : EventArgs
	{
		private InventorySlotRef m_slot;

		public InventoryItemEventArgs(InventorySlotRef p_slot)
		{
			m_slot = p_slot;
			Type = ERepairType.NONE;
		}

		public InventoryItemEventArgs(ERepairType p_type)
		{
			Type = p_type;
		}

		public InventorySlotRef Slot => m_slot;

	    public ERepairType Type { get; private set; }

		public enum ERepairType
		{
			NONE,
			ALL,
			WEAPONS,
			ARMOR
		}
	}
}
