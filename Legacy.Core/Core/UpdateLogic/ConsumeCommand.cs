using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic
{
	public class ConsumeCommand : Command
	{
		private InventorySlotRef m_slot;

		private Int32 m_targetCharacter;

		public ConsumeCommand(InventorySlotRef p_slot, Int32 p_targetCharacter) : base(ECommandTypes.CONSUME)
		{
			m_slot = p_slot;
			m_targetCharacter = p_targetCharacter;
		}

		public InventorySlotRef Slot
		{
			get => m_slot;
		    set => m_slot = value;
		}

		public Int32 TargetCharacter => m_targetCharacter;
	}
}
