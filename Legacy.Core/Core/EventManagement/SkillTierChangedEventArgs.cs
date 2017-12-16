using System;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.EventManagement
{
	public class SkillTierChangedEventArgs : EventArgs
	{
		public SkillTierChangedEventArgs(Skill p_skill, Character p_character)
		{
			ChangedSkill = p_skill;
			SkillOwner = p_character;
		}

		public Skill ChangedSkill { get; private set; }

		public Character SkillOwner { get; private set; }
	}
}
