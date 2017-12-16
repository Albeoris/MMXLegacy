using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	public class CharacterPosing : MonoBehaviour
	{
		private void OnDrop()
		{
			if (UICamera.currentTouchID == -1)
			{
				if (DragDropManager.Instance.DraggedItem is ItemDragObject)
				{
					Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
					ItemDragObject itemDragObject = (ItemDragObject)DragDropManager.Instance.DraggedItem;
					if (itemDragObject.ItemSlot != null && !selectedCharacter.DoneTurn && !selectedCharacter.ConditionHandler.CantDoAnything())
					{
						LegacyLogic.Instance.WorldManager.Party.SelectCharacter(selectedCharacter.Index);
						Int32 index = itemDragObject.ItemSlot.Index;
						Potion potion = itemDragObject.Item as Potion;
						Equipment equipment = itemDragObject.Item as Equipment;
						PartyInventoryController partyInventoryController = itemDragObject.ItemSlot.Parent.Inventory as PartyInventoryController;
						if (potion != null && partyInventoryController != null)
						{
							partyInventoryController.ConsumeItem(index, selectedCharacter.Index);
						}
						if (equipment != null)
						{
							Int32 autoSlot = (Int32)selectedCharacter.Equipment.GetAutoSlot(equipment);
							if (autoSlot >= 0 && selectedCharacter.Equipment.IsItemPlaceableAt(equipment, autoSlot))
							{
								EquipCommand p_command = new EquipCommand(selectedCharacter.Equipment, partyInventoryController, selectedCharacter.Equipment.GetItemAt((EEquipSlots)autoSlot), equipment, autoSlot, index);
								if (LegacyLogic.Instance.UpdateManager.PartyTurnActor.CanDoCommand(p_command, LegacyLogic.Instance.WorldManager.Party.CurrentCharacter))
								{
									LegacyLogic.Instance.CommandManager.AddCommand(p_command);
								}
							}
						}
					}
				}
				DragDropManager.Instance.CancelDragAction();
			}
		}
	}
}
