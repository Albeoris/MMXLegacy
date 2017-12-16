using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityPoisoning : MonsterAbilityBase
	{
		public MonsterAbilityPoisoning() : base(EMonsterAbilityType.POISONING_STRIKES)
		{
			m_executionPhase = EExecutionPhase.AFTER_MONSTER_ATTACK;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			if (p_attackList == null)
			{
				return;
			}
			Single num = p_character.FightValues.Resistance[EDamageType.EARTH].Value * 0.5f;
			if (Random.Range(0f, 100f) > num)
			{
				AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
				for (Int32 i = 0; i < p_attackList.Count; i++)
				{
					if (p_attackList[i].Result == EResultType.HIT && !p_isMagic)
					{
						Single num2 = Random.Range(0f, 1f);
						if (num2 < m_staticData.GetValues(m_level)[0] * 0.01f)
						{
							p_character.ConditionHandler.AddCondition(ECondition.POISONED);
							if (p_character.ConditionHandler.HasCondition(ECondition.POISONED))
							{
								p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
							}
							break;
						}
					}
					else if (p_attackList[i].Result == EResultType.CRITICAL_HIT && !p_isMagic)
					{
						p_character.ConditionHandler.AddCondition(ECondition.POISONED);
						if (p_character.ConditionHandler.HasCondition(ECondition.POISONED))
						{
							p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
						}
						break;
					}
				}
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
			}
		}
	}
}
