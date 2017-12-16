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
	public class MonsterAbilityPainReflection : MonsterAbilityBase
	{
		public MonsterAbilityPainReflection() : base(EMonsterAbilityType.PAIN_REFLECTION)
		{
			m_executionPhase = EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_AFTER_DAMAGE_REDUCTION;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			if (p_attackList == null)
			{
				return;
			}
			if (p_monster.DistanceToParty > 1.1f)
			{
				return;
			}
			AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
			Single num = m_staticData.GetValues(m_level)[0] * 0.01f;
			Boolean flag = false;
			Int32 num2 = 0;
			foreach (AttackResult attackResult in p_attackList)
			{
				List<Damage> list = new List<Damage>();
				Int32 num3 = 0;
				Int32[] array = new Int32[8];
				for (Int32 i = 0; i < attackResult.DamageResults.Count; i++)
				{
					Single num4 = attackResult.DamageResults[i].EffectiveValue;
					array[(Int32)attackResult.DamageResults[i].Type] += (Int32)(num4 * num + 0.5f);
					num3 += (Int32)(num4 * num + 0.5f);
					flag = true;
					list.Add(new Damage(attackResult.DamageResults[i].Type, array[(Int32)attackResult.DamageResults[i].Type], 1f, 1f));
				}
				num2 += num3;
				attackResult.ReflectedDamage = new Attack(1f, 0f, list);
				attackResult.ReflectedDamageSource = this;
			}
			if (flag && num2 > 0)
			{
				p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
			}
		}
	}
}
