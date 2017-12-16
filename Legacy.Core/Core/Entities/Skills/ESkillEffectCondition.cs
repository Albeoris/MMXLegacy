using System;

namespace Legacy.Core.Entities.Skills
{
	public enum ESkillEffectCondition
	{
		NONE,
		IS_EQUIPED,
		FOR_EACH_EQUIPMENT,
		CRITICAL_HIT,
		FIRST_CRITICAL_HIT,
		FIRST_MELEE_STRIKE_BLOCKED,
		RANGED_ATTACK_AGAINST_PARTY,
		RESIST_SPELL,
		EVADE_MELEE_ATTACK
	}
}
