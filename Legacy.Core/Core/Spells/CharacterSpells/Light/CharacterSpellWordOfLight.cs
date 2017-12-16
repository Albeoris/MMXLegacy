using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Light
{
	public class CharacterSpellWordOfLight : CharacterSpell
	{
		public CharacterSpellWordOfLight() : base(ECharacterSpell.SPELL_LIGHT_WORD_OF_LIGHT)
		{
		}

		protected override void HandlePartyMemberHealing(Single p_magicFactor, Single p_critChance, Int32 p_critHealValue, SpellEventArgs p_result, List<Object> p_targets)
		{
			SpellEventArgs eventArgs = GetEventArgs();
			if (m_staticData.Damage != null && m_staticData.Damage.Length > 0 && m_staticData.Damage[0].Type == EDamageType.HEAL)
			{
				Int32 value = Damage.Create(m_staticData.Damage[0] * p_magicFactor, 0f).Value;
				for (Int32 i = 0; i < LegacyLogic.Instance.WorldManager.Party.Members.Length; i++)
				{
					Character character = LegacyLogic.Instance.WorldManager.Party.Members[i];
					if (character.HealthPoints < character.MaximumHealthPoints)
					{
						Boolean flag = Random.Range(0f, 1f) < p_critChance;
						Int32 num = value;
						if (flag)
						{
							num = p_critHealValue;
						}
						eventArgs.SpellTargets.Add(new HealedTarget(character, num, flag));
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
					}
				}
			}
			eventArgs.Result = ESpellResult.OK;
			LegacyLogic.Instance.ActionLog.PushEntry(new SpellEffectEntryEventArgs(null, eventArgs));
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(1, p_magicFactor));
			SetDescriptionValue(1, GetDamageAsString(0, p_magicFactor));
			SetDescriptionValue(2, m_staticData.Range);
		}
	}
}
