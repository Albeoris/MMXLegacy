using System;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class EquipAction : BaseCharacterAction
	{
		public EquipAction(Int32 characterIndex) : base(characterIndex)
		{
		}

		public override void DoAction(Command p_command)
		{
			EquipCommand equipCommand = (EquipCommand)p_command;
			Boolean flag = IsTwoHanded(equipCommand.OtherItem);
			Boolean flag2 = IsTwoHanded(equipCommand.EquipItem);
			BaseItem baseItem = null;
			if (flag && !flag2)
			{
				if (equipCommand.EquipSlot == 0)
				{
					baseItem = equipCommand.EquipInventory.GetItemAt(1);
				}
				else
				{
					baseItem = equipCommand.EquipInventory.GetItemAt(0);
				}
			}
			equipCommand.OtherInventory.AddItem(equipCommand.EquipItem, equipCommand.OtherSlot);
			equipCommand.EquipInventory.AddItem(equipCommand.OtherItem, equipCommand.EquipSlot);
			if (baseItem != null)
			{
				equipCommand.OtherInventory.AddItem(baseItem);
			}
		}

		public override Boolean IsActionDone()
		{
			return true;
		}

		public override Boolean ActionAvailable()
		{
			return !Character.DoneTurn && !Character.ConditionHandler.CantDoAnything();
		}

		public override Boolean CanDoAction(Command p_command)
		{
			EquipCommand equipCommand = (EquipCommand)p_command;
			if (equipCommand.EquipInventory != Character.Equipment)
			{
				return false;
			}
			Boolean flag = IsTwoHanded(equipCommand.OtherItem);
			if (flag)
			{
				BaseItem itemAt;
				if (equipCommand.EquipSlot == 0)
				{
					itemAt = equipCommand.EquipInventory.GetItemAt(1);
				}
				else
				{
					itemAt = equipCommand.EquipInventory.GetItemAt(0);
				}
				if (itemAt != null && equipCommand.OtherInventory.IsFull())
				{
					return false;
				}
			}
			Boolean flag2 = equipCommand.OtherInventory.IsItemPlaceableAt(equipCommand.EquipItem, equipCommand.OtherSlot);
			Boolean flag3 = equipCommand.EquipInventory.IsItemPlaceableAt(equipCommand.OtherItem, equipCommand.EquipSlot);
			return flag2 && flag3;
		}

		private Boolean IsTwoHanded(BaseItem p_item)
		{
			return p_item != null && p_item is Equipment && ((Equipment)p_item).ItemSlot == EItemSlot.ITEM_SLOT_2_HAND;
		}
	}
}
