using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Utilities;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class StartDLCFunction : DialogFunction
	{
		private Int32 m_dialogID;

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			RemoveHirelings();
			RemoveEquipment();
			if (m_dialogID > 0)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
			}
			else if (m_dialogID == 0)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, p_manager.CurrentConversation.RootDialog.ID);
			}
			else
			{
				p_manager.CloseNpcContainer(null);
			}
		}

		private void RemoveHirelings()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.HirelingHandler.HirelingHired(16) && party.TokenHandler.GetTokens(500) == 0)
			{
				party.TokenHandler.AddToken(500);
			}
			party.HirelingHandler.FireByIndex(0);
			party.HirelingHandler.FireByIndex(1);
		}

		private void RemoveEquipment()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Int32 num = 0;
			Int32 maximumItemCount;
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = party.GetMember(i);
				if (member != null)
				{
					HiddenInventoryController hiddenInventory = party.GetHiddenInventory(i);
					maximumItemCount = member.Equipment.Equipment.GetMaximumItemCount();
					for (Int32 j = 0; j < maximumItemCount; j++)
					{
						BaseItem itemAt = member.Equipment.Equipment.GetItemAt(j);
						if (itemAt != null)
						{
							Equipment equipment = itemAt as Equipment;
							if (equipment == null || j != 1 || equipment.ItemSlot != EItemSlot.ITEM_SLOT_2_HAND)
							{
								hiddenInventory.AddItem(itemAt);
								num++;
							}
						}
					}
					member.Equipment.Equipment.Clear();
				}
			}
			maximumItemCount = party.Inventory.Inventory.GetMaximumItemCount();
			HiddenInventoryController hiddenInventory2 = party.GetHiddenInventory(4);
			for (Int32 k = 0; k < maximumItemCount; k++)
			{
				BaseItem itemAt = party.Inventory.Inventory.GetItemAt(k);
				if (itemAt != null)
				{
					hiddenInventory2.AddItem(itemAt);
					num++;
				}
			}
			LegacyLogger.Log("Items moved: " + num);
			party.Inventory.Inventory.Clear();
		}
	}
}
