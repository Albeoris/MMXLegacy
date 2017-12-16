using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class SkillLevelReachedCondition : DialogCondition
	{
		[XmlAttribute("skill")]
		public ESkillID NeededSkill { get; set; }

		[XmlAttribute("level")]
		public Int32 NeededSkillLevel { get; set; }

		public override EDialogState CheckCondition(Npc p_npc)
		{
			Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			if (selectedCharacter != null && selectedCharacter.SkillHandler.HasRequiredSkill((Int32)NeededSkill) && selectedCharacter.SkillHandler.HasRequiredSkillLevel((Int32)NeededSkill, NeededSkillLevel))
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
