using System;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.ActionLogging
{
	public class SkillTierBonusEventArgs : LogEntryEventArgs
	{
		public SkillTierBonusEventArgs(ETier p_tier, Character p_chara, String p_skillName)
		{
			Tier = p_tier;
			Chara = p_chara;
			SkillName = p_skillName;
		}

		public ETier Tier { get; private set; }

		public Character Chara { get; private set; }

		public String SkillName { get; private set; }
	}
}
