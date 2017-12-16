using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class BrokenItemsCondition : DialogCondition
	{
		private ERepairType m_repairType;

		[XmlAttribute("repairType")]
		public ERepairType RepairType
		{
			get => m_repairType;
		    set => m_repairType = value;
		}

		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			foreach (Character character in party.Members)
			{
				if (character != null && character.Equipment.Equipment.HasBrokenItems() && m_repairType == ERepairType.ALL)
				{
					return EDialogState.NORMAL;
				}
				if (character != null && character.Equipment.Equipment.HasBrokenWeapons() && m_repairType == ERepairType.WEAPONS)
				{
					return EDialogState.NORMAL;
				}
				if (character != null && character.Equipment.Equipment.HasBrokenArmors() && m_repairType == ERepairType.ARMOR_AND_SHIELD)
				{
					return EDialogState.NORMAL;
				}
			}
			if (((party.Inventory.Inventory.HasBrokenItems() || party.MuleInventory.Inventory.HasBrokenItems()) && m_repairType == ERepairType.ALL) || ((party.Inventory.Inventory.HasBrokenWeapons() || party.MuleInventory.Inventory.HasBrokenWeapons()) && m_repairType == ERepairType.WEAPONS) || ((party.Inventory.Inventory.HasBrokenArmors() || party.MuleInventory.Inventory.HasBrokenArmors()) && m_repairType == ERepairType.ARMOR_AND_SHIELD))
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
