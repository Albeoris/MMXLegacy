using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityVindictive : MonsterAbilityBase
	{
		public MonsterAbilityVindictive() : base(EMonsterAbilityType.VINDICTIVE)
		{
			m_executionPhase = EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_AFTER_DAMAGE_REDUCTION;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			if (p_attackList == null)
			{
				return;
			}
			foreach (AttackResult attackResult in p_attackList)
			{
				if (attackResult.Result == EResultType.BLOCK || attackResult.Result == EResultType.EVADE)
				{
					p_monster.CombatHandler.MeleeStrikesRoundBonus += (Int32)m_staticData.GetValues(m_level)[0];
					AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
					p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
				}
			}
		}
	}
}
