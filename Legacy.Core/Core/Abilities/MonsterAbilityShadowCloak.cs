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
	public class MonsterAbilityShadowCloak : MonsterAbilityBase
	{
		public MonsterAbilityShadowCloak() : base(EMonsterAbilityType.SHADOW_CLOAK)
		{
			m_executionPhase = EExecutionPhase.BEGIN_OF_MONSTERS_TURN;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			if (!p_monster.BuffHandler.HasBuff(EMonsterBuffType.SHADOW_CLOAK))
			{
				MonsterBuffShadowCloak p_buff = (MonsterBuffShadowCloak)BuffFactory.CreateMonsterBuff(EMonsterBuffType.SHADOW_CLOAK, 1f, m_level);
				p_monster.AddBuff(p_buff);
				AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
				p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
			}
		}
	}
}
