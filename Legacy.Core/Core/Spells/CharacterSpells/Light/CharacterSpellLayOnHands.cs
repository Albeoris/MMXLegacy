using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Light
{
	public class CharacterSpellLayOnHands : CharacterSpell
	{
		private Character m_sorcerer;

		public CharacterSpellLayOnHands() : base(ECharacterSpell.SPELL_LIGHT_LAY_ON_HANDS)
		{
			m_sorcerer = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
		}

		public override Boolean CastSpell(Character p_sorcerer, Boolean p_fromScroll, Int32 p_scrollTier, List<Object> p_targets)
		{
			m_sorcerer = p_sorcerer;
			return base.CastSpell(p_sorcerer, p_fromScroll, p_scrollTier, p_targets);
		}

		protected override void HandlePartyMemberHealing(Single p_magicFactor, Single p_critChance, Int32 p_critHealValue, SpellEventArgs p_result, List<Object> p_targets)
		{
			if (m_sorcerer == null)
			{
				m_sorcerer = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			}
			for (Int32 i = 0; i < p_targets.Count; i++)
			{
				Character character = (Character)p_targets[i];
				if (!character.ConditionHandler.HasCondition(ECondition.DEAD))
				{
					Int32 num = character.MaximumHealthPoints - character.HealthPoints;
					character.ChangeHP(num);
					DamageEventArgs p_eventArgs = new DamageEventArgs(new AttackResult
					{
						Result = EResultType.HEAL,
						DamageResults = 
						{
							new DamageResult(EDamageType.HEAL, num, 0, 1f)
						}
					});
					LegacyLogic.Instance.EventManager.InvokeEvent(character, EEventType.CHARACTER_HEALS, p_eventArgs);
					p_result.SpellTargets.Add(new HealedTarget(character, num, false));
				}
			}
			m_sorcerer.ChangeMP(-m_sorcerer.ManaPoints);
		}

		protected override Boolean PartyMemberNeedsHealing()
		{
			foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
			{
				if (character.HealthPoints < character.MaximumHealthPoints)
				{
					return true;
				}
			}
			return false;
		}

		public override Boolean HasResources(Character p_sorcerer)
		{
			return p_sorcerer.ManaPoints >= 1;
		}
	}
}
