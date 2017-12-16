using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityRegeneration : MonsterAbilityBase
	{
		public MonsterAbilityRegeneration() : base(EMonsterAbilityType.REGENERATION)
		{
			m_executionPhase = EExecutionPhase.END_OF_MONSTERS_TURN;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			if (p_monster.CurrentHealth == p_monster.MaxHealth)
			{
				return;
			}
			AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
			Int32 num = (Int32)(p_monster.MaxHealth * (m_staticData.GetValues(m_level)[0] * 0.01f));
			p_monster.ChangeHP(num, null);
			MonsterBuff p_buff = new MonsterBuff(20, p_monster.MagicPower);
			MonsterBuffDamageEntryEventArgs p_args2 = new MonsterBuffDamageEntryEventArgs(p_buff, p_monster, new AttackResult
			{
				Result = EResultType.HEAL,
				DamageResults = 
				{
					new DamageResult(EDamageType.HEAL, num, 0, 1f)
				}
			});
			p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
			p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args2);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
		}
	}
}
