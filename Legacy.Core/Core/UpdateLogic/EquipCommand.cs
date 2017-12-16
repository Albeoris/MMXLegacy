using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic
{
	public class EquipCommand : Command
	{
		public EquipCommand(IInventory p_equipInventory, IInventory p_otherInventory, BaseItem p_equipItem, BaseItem p_otherItem, Int32 p_equipSlot, Int32 p_otherSlot) : base(ECommandTypes.EQUIP)
		{
			OtherInventory = p_otherInventory;
			EquipInventory = p_equipInventory;
			OtherItem = p_otherItem;
			EquipItem = p_equipItem;
			OtherSlot = p_otherSlot;
			EquipSlot = p_equipSlot;
		}

		public IInventory OtherInventory { get; private set; }

		public IInventory EquipInventory { get; private set; }

		public BaseItem OtherItem { get; private set; }

		public BaseItem EquipItem { get; private set; }

		public Int32 OtherSlot { get; private set; }

		public Int32 EquipSlot { get; private set; }

		public override void CancelCommand()
		{
			OtherInventory.AddItem(OtherItem, OtherSlot);
			EquipInventory.AddItem(EquipItem, EquipSlot);
		}
	}
}
