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
	public class MonsterAbilitySweepingAttack : MonsterAbilityBase
	{
		public MonsterAbilitySweepingAttack() : base(EMonsterAbilityType.SWEEPING_ATTACK)
		{
			m_executionPhase = EExecutionPhase.BEFORE_TARGET_SELECTION;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			if (p_monster.CombatHandler.MeleeStrikes > 1 && !p_isMagic)
			{
				p_monster.DivideAttacksToPartyCharacters = true;
				AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
				p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
			}
		}
	}
}
