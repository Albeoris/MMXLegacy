using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityUnstoppableStrikes : MonsterAbilityBase
	{
		private Boolean m_isFirstStrike = true;

		private Int32 m_passedStrikes;

		public MonsterAbilityUnstoppableStrikes() : base(EMonsterAbilityType.UNSTOPPABLE_STRIKES)
		{
			m_executionPhase = EExecutionPhase.AFTER_MONSTER_ATTACK;
		}

		public override void ResetAbilityValues()
		{
			m_isFirstStrike = true;
			m_passedStrikes = 0;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			if (p_isMagic)
			{
				return;
			}
			if (p_attackList == null)
			{
				return;
			}
			for (Int32 i = 0; i < p_attackList.Count; i++)
			{
				m_passedStrikes++;
				AttackResult attackResult = p_attackList[i];
				if (attackResult.Result == EResultType.HIT || attackResult.Result == EResultType.CRITICAL_HIT)
				{
					p_character.FightValues.GeneralBlockChance -= p_character.FightValues.GeneralBlockChance * m_staticData.GetValues(m_level)[0];
					if (m_isFirstStrike)
					{
						AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
						p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
						m_isFirstStrike = false;
					}
				}
				if ((m_passedStrikes == p_monster.CombatHandler.MeleeStrikes && !p_isRanged) || (m_passedStrikes == p_monster.CombatHandler.RangedStrikes && p_isRanged))
				{
					p_character.CalculateCurrentAttributes();
				}
			}
		}

		public override String GetDescription()
		{
			return Localization.Instance.GetText(m_staticData.NameKey + "_INFO", m_staticData.GetValues(m_level)[0]);
		}
	}
}
