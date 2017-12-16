using System;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.PartyManagement
{
	public interface IInventory
	{
		Int32 GetCurrentItemCount();

		Int32 GetMaximumItemCount();

		Boolean IsFull();

		Boolean CanAddItem(BaseItem p_item);

		Boolean AddItem(BaseItem p_item);

		Int32 GetAutoSlot(BaseItem p_item);

		void AddItem(BaseItem p_item, Int32 p_slot);

		void RemoveItemAt(Int32 p_slot);

		BaseItem GetItemAt(Int32 p_slot);

		void ChangeItemPosition(Int32 p_slot1, Int32 p_slot2);

		Boolean IsItemPlaceableAt(BaseItem p_item, Int32 p_slot);

		Boolean IsSlotOccupied(Int32 p_slot);

		void SellItem(BaseItem p_item, Int32 p_itemSlot, Int32 p_count);

		IInventory GetEventSource();
	}
}
