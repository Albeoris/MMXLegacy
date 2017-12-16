using System;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.PartyManagement
{
	public struct InventorySlotRef : IEquatable<InventorySlotRef>
	{
		public static InventorySlotRef Empty = new InventorySlotRef(null, -1);

		public IInventory Inventory;

		public Int32 Slot;

		public InventorySlotRef(IInventory p_inventory, Int32 p_slot)
		{
			Inventory = p_inventory;
			Slot = p_slot;
		}

		public BaseItem GetItem()
		{
			return Inventory.GetItemAt(Slot);
		}

		public Boolean Equals(InventorySlotRef other)
		{
			return Object.Equals(other, this);
		}

		public override Boolean Equals(Object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			InventorySlotRef inventorySlotRef = (InventorySlotRef)obj;
			return inventorySlotRef.Slot == Slot && inventorySlotRef.Inventory == Inventory;
		}

		public override Int32 GetHashCode()
		{
			return Slot.GetHashCode();
		}

		public static Boolean operator ==(InventorySlotRef slot1, InventorySlotRef slot2)
		{
			return slot1.Equals(slot2);
		}

		public static Boolean operator !=(InventorySlotRef slot1, InventorySlotRef slot2)
		{
			return !slot1.Equals(slot2);
		}
	}
}
