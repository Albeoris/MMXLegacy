using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityRelentless : MonsterAbilityBase
	{
		private Single m_percentValue = 1f;

		private Int32 m_passedStrikes;

		public MonsterAbilityRelentless() : base(EMonsterAbilityType.RELENTLESS)
		{
			m_executionPhase = EExecutionPhase.AFTER_DAMAGE_CALCULATION;
		}

		public override void ResetAbilityValues()
		{
			m_percentValue = 1f;
			m_passedStrikes = 0;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			if (p_attackList == null)
			{
				return;
			}
			Single num = m_staticData.GetValues(m_level)[0] * 0.01f;
			for (Int32 i = 0; i < p_attackList.Count; i++)
			{
				AttackResult attackResult = p_attackList[i];
				for (Int32 j = 0; j < attackResult.DamageResults.Count; j++)
				{
					DamageResult value = attackResult.DamageResults[j];
					value.EffectiveValue = (Int32)(value.EffectiveValue * m_percentValue + 0.5f);
					attackResult.DamageResults[j] = value;
				}
				m_passedStrikes++;
				if (attackResult.Result != EResultType.BLOCK && attackResult.Result != EResultType.EVADE)
				{
					if (m_percentValue == 1f && p_monster.CombatHandler.MeleeStrikes > m_passedStrikes)
					{
						AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
						p_monster.AbilityHandler.AddEntry(EExecutionPhase.AFTER_MONSTER_ATTACK, p_args);
					}
					m_percentValue += num;
				}
			}
		}
	}
}
