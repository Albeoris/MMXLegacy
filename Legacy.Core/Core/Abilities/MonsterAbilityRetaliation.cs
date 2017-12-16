using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityRetaliation : MonsterAbilityBase
	{
		public MonsterAbilityRetaliation() : base(EMonsterAbilityType.RETALIATION)
		{
			m_executionPhase = EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_AFTER_DAMAGE_REDUCTION;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			if (p_isMagic || p_attackList == null)
			{
				return;
			}
			Position position = LegacyLogic.Instance.WorldManager.Party.Position;
			Position position2 = p_monster.Position;
			Boolean flag = LegacyLogic.Instance.MapLoader.Grid.LineOfSight(position, position2);
			Boolean flag2 = false;
			foreach (AttackResult attackResult in p_attackList)
			{
				for (Int32 i = 0; i < attackResult.DamageResults.Count; i++)
				{
					if (attackResult.DamageResults[i].Type == EDamageType.PHYSICAL && Position.Distance(position, position2) <= 1.5f && flag)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2 && !p_monster.CounterAttack)
				{
					p_monster.CounterAttack = true;
					p_character.CounterAttackMonster = p_monster;
					AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
					p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
					break;
				}
			}
		}
	}
}
