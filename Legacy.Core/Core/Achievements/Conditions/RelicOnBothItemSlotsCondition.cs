using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Achievements.Conditions
{
	public class RelicOnBothItemSlotsCondition : AchievementCondition
	{
		private Int32 m_relicCount;

		public RelicOnBothItemSlotsCondition(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
		}

		public override void ParseParameter(String p_parameterString)
		{
			Int32.TryParse(p_parameterString, out m_relicCount);
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			p_count = 0;
			for (Int32 i = 0; i < party.Members.Length; i++)
			{
				Int32 num = 0;
				Character character = party.Members[i];
				for (Int32 j = 0; j < character.Equipment.GetMaximumItemCount(); j++)
				{
					Equipment equipment = (Equipment)character.Equipment.GetItemAt((EEquipSlots)j);
					if (equipment == null || j != 1 || equipment.ItemSlot != EItemSlot.ITEM_SLOT_2_HAND)
					{
						if (equipment != null && equipment.IsRelic())
						{
							num++;
						}
					}
				}
				if (num >= m_relicCount)
				{
					p_count = 1;
					return true;
				}
			}
			return false;
		}
	}
}
