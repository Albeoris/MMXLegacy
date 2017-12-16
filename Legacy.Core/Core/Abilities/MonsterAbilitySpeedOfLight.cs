using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilitySpeedOfLight : MonsterAbilityBase
	{
		public MonsterAbilitySpeedOfLight() : base(EMonsterAbilityType.SPEED_OF_LIGHT)
		{
			m_executionPhase = EExecutionPhase.BEGIN_OF_MONSTERS_TURN;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
			p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
		}
	}
}
