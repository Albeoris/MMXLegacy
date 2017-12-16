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
	public class MonsterAbilityEnraged : MonsterAbilityBase
	{
		public MonsterAbilityEnraged() : base(EMonsterAbilityType.ENRAGED)
		{
			m_executionPhase = EExecutionPhase.BEFORE_MONSTER_ATTACK;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			Single num = p_monster.CurrentHealth / (Single)p_monster.MaxHealth;
			if (num < 0.5f)
			{
				p_monster.CombatHandler.MeleeStrikes *= 2;
				AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
				p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
			}
			else
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_REMOVED, new AbilityEventArgs(p_monster, this));
			}
		}
	}
}
