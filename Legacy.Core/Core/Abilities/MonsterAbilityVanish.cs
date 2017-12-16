using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityVanish : MonsterAbilityBase
	{
		private Monster m_monster;

		public MonsterAbilityVanish() : base(EMonsterAbilityType.VANISH)
		{
			m_executionPhase = EExecutionPhase.AFTER_MONSTER_SPELL;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			m_monster = p_monster;
			m_monster.AiHandler.Vanish();
			AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
			p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
		}

		public override String GetDescription()
		{
			return Localization.Instance.GetText("MONSTER_BUFF_VANISH_INFO");
		}
	}
}
