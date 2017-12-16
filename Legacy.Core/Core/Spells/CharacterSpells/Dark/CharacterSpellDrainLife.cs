using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Dark
{
	public class CharacterSpellDrainLife : CharacterSpell
	{
		private Boolean m_resisted;

		public CharacterSpellDrainLife() : base(ECharacterSpell.SPELL_DARK_DRAIN_LIFE)
		{
		}

		protected override void HandlePartyMemberHealing(Single p_magicFactor, Single p_critChance, Int32 p_critHealValue, SpellEventArgs p_result, List<Object> p_targets)
		{
			List<Object> list = new List<Object>(LegacyLogic.Instance.WorldManager.Party.Members);
			if (m_staticData.Damage != null && m_staticData.Damage.Length > 0 && m_staticData.Damage[0].Type == EDamageType.HEAL && !m_resisted)
			{
				Int32 value = Damage.Create(m_staticData.Damage[0] * p_magicFactor, 0f).Value;
				for (Int32 i = 0; i < list.Count; i++)
				{
					Int32 num;
					Boolean p_IsCritical;
					if (Random.Range(0f, 1f) < p_critChance)
					{
						num = p_critHealValue;
						p_IsCritical = true;
					}
					else
					{
						num = value;
						p_IsCritical = false;
					}
					p_result.SpellTargets.Add(new HealedTarget(list[i], num, p_IsCritical));
					((Character)list[i]).ChangeHP(num);
					DamageEventArgs p_eventArgs = new DamageEventArgs(new AttackResult
					{
						Result = EResultType.HEAL,
						DamageResults = 
						{
							new DamageResult(EDamageType.HEAL, num, 0, 1f)
						}
					});
					LegacyLogic.Instance.EventManager.InvokeEvent(list[i], EEventType.CHARACTER_HEALS, p_eventArgs);
				}
			}
		}

		protected override AttackResult DoAttackMonster(Character p_sorcerer, Monster p_target, Single p_magicPower)
		{
			AttackResult attackResult = base.DoAttackMonster(p_sorcerer, p_target, p_magicPower);
			m_resisted = (attackResult.Result == EResultType.EVADE || attackResult.Result == EResultType.IMMUNE);
			return attackResult;
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(1, p_magicFactor));
			SetDescriptionValue(1, GetDamageAsString(0, p_magicFactor));
		}
	}
}
