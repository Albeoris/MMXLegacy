using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Light
{
	public class CharacterSpellResurrection : CharacterSpell
	{
		public CharacterSpellResurrection() : base(ECharacterSpell.SPELL_LIGHT_RESURRECTION)
		{
		}

		protected override void HandleConditions(SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			if (p_targets.Count > 0)
			{
				Character character = p_targets[0] as Character;
				if (character != null && character.ConditionHandler.HasCondition(ECondition.DEAD))
				{
					Int32 num = Math.Abs(character.HealthPoints) + 1;
					character.ConditionHandler.RemoveCondition(ECondition.DEAD);
					character.ChangeHP(num);
					p_result.SpellTargets.Add(new RemovedConditionTarget(character, ECondition.DEAD));
					p_result.SpellTargets.Add(new HealedTarget(character, num, false));
					m_didPushEntry = true;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_REVIVED, EventArgs.Empty);
				}
			}
		}
	}
}
