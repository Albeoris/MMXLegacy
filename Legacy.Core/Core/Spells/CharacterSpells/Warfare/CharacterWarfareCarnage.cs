using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareCarnage : CharacterSpell
	{
		private Monster m_previouslySelectedMonster;

		public CharacterWarfareCarnage() : base(ECharacterSpell.WARFARE_CARNAGE)
		{
		}

		protected override void HandleMonsters(Character p_sorcerer, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			m_previouslySelectedMonster = LegacyLogic.Instance.WorldManager.Party.SelectedMonster;
			base.HandleMonsters(p_sorcerer, p_result, p_targets, p_magicFactor);
			LegacyLogic.Instance.WorldManager.Party.SelectMonsterWithoutEvent(m_previouslySelectedMonster);
			m_previouslySelectedMonster = null;
		}

		protected override List<AttackResult> MeleeAttackMonster(Character p_sorcerer, Monster p_target)
		{
			LegacyLogic.Instance.WorldManager.Party.SelectMonsterWithoutEvent(p_target);
			return p_sorcerer.FightHandler.ExecuteMeleeAttack();
		}
	}
}
