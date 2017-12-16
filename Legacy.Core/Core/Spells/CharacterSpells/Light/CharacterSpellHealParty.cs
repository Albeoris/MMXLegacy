using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Light
{
	public class CharacterSpellHealParty : CharacterSpell
	{
		public CharacterSpellHealParty() : base(ECharacterSpell.SPELL_LIGHT_HEAL_PARTY)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
		}

		protected override void HandlePartyMemberHealing(Single p_magicFactor, Single p_critChance, Int32 p_critHealValue, SpellEventArgs p_result, List<Object> p_targets)
		{
			Int32 value = Damage.Create(m_staticData.Damage[0] * p_magicFactor, 0f).Value;
			for (Int32 i = 0; i < p_targets.Count; i++)
			{
				if (!((Character)p_targets[i]).ConditionHandler.HasCondition(ECondition.DEAD))
				{
					Int32 num = value;
					if (Random.Range(0f, 1f) < p_critChance)
					{
						num = p_critHealValue;
					}
					((Character)p_targets[i]).ChangeHP(num);
					DamageEventArgs p_eventArgs = new DamageEventArgs(new AttackResult
					{
						Result = EResultType.HEAL,
						DamageResults = 
						{
							new DamageResult(EDamageType.HEAL, num, 0, 1f)
						}
					});
					LegacyLogic.Instance.EventManager.InvokeEvent((Character)p_targets[i], EEventType.CHARACTER_HEALS, p_eventArgs);
				}
			}
			p_result.SpellTargets.Add(new HealedTarget(LegacyLogic.Instance.WorldManager.Party, value, false));
		}
	}
}
