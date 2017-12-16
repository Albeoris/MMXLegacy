using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Prime
{
	public class CharacterSpellImplosion : CharacterSpell
	{
		private const Single SPLASHDAMAGEFACTOR = 0.3f;

		public CharacterSpellImplosion() : base(ECharacterSpell.SPELL_PRIME_IMPLOSION)
		{
		}

		protected override void HandleMonsters(Character p_attacker, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			if (p_targets.Count > 0 && p_targets[0] is Monster)
			{
				Monster monster = (Monster)p_targets[0];
				AttackResult attackResult = DoAttackMonster(p_attacker, monster, p_magicFactor);
				p_result.SpellTargets.Add(new AttackedTarget(monster, attackResult));
				if (attackResult.Result == EResultType.EVADE)
				{
					return;
				}
				if (attackResult.DamageDone > 0)
				{
					Single num = attackResult.DamageDone * 0.3f;
					List<Object> otherMonstersOnSlot = LegacyLogic.Instance.MapLoader.Grid.GetOtherMonstersOnSlot(monster);
					AttackMonsters(p_attacker, otherMonstersOnSlot, p_result, (Int32)num);
					otherMonstersOnSlot.Add(monster);
					SetMonsterBuffs(p_attacker, otherMonstersOnSlot, p_result, p_magicFactor);
				}
				monster.ApplyDamages(attackResult, p_attacker);
			}
		}

		private void SetMonsterBuffs(Character p_attacker, List<Object> p_otherTargets, SpellEventArgs p_result, Single p_magicFactor)
		{
			if (m_staticData.MonsterBuffs == null || m_staticData.MonsterBuffs.Length == 0)
			{
				return;
			}
			for (Int32 i = 0; i < p_otherTargets.Count; i++)
			{
				if (p_otherTargets[i] != null && p_otherTargets[i] is Monster && ((Monster)p_otherTargets[i]).CurrentHealth > 0)
				{
					for (Int32 j = 0; j < m_staticData.MonsterBuffs.Length; j++)
					{
						if (m_staticData.MonsterBuffs[j] != EMonsterBuffType.NONE)
						{
							Monster monster = (Monster)p_otherTargets[i];
							AddMonsterBuff(monster, m_staticData.MonsterBuffs[j], p_magicFactor);
							Boolean p_Successful = monster.BuffHandler.HasBuff(m_staticData.MonsterBuffs[j]);
							Boolean p_IsImmune = false;
							if (!monster.AbilityHandler.CanAddBuff(m_staticData.MonsterBuffs[j]))
							{
								p_IsImmune = true;
							}
							p_result.SpellTargets.Add(new MonsterBuffTarget(p_otherTargets[i], m_staticData.MonsterBuffs[j], p_Successful, p_IsImmune));
						}
					}
				}
			}
		}

		private void AttackMonsters(Character p_attacker, List<Object> p_otherTargets, SpellEventArgs p_result, Int32 p_splashDamage)
		{
			if (p_otherTargets != null && p_otherTargets.Count > 0)
			{
				Single criticalMagicHitChance = p_attacker.FightValues.CriticalMagicHitChance;
				Single magicalCriticalDamageMod = p_attacker.FightValues.MagicalCriticalDamageMod;
				EDamageType edamageType = ESkillIDToEDamageType(m_staticData.SkillID);
				DamageData p_data = new DamageData(edamageType, p_splashDamage, p_splashDamage);
				Damage item = Damage.Create(p_data, magicalCriticalDamageMod);
				item.IgnoreResistance = p_attacker.SkillHandler.GetResistanceIgnoreValue(m_staticData.SkillID);
				Attack p_attack = new Attack(0f, criticalMagicHitChance, new List<Damage>
				{
					item
				});
				foreach (Object obj in p_otherTargets)
				{
					if (obj != null && obj is Monster && ((Monster)obj).CurrentHealth > 0)
					{
						AttackResult attackResult = ((Monster)obj).CombatHandler.AttackMonster(p_attacker, p_attack, false, true, edamageType, true, p_attacker.SkillHandler.GetResistanceIgnoreValue(m_staticData.SkillID));
						((Monster)obj).ApplyDamages(attackResult, p_attacker);
						p_result.SpellTargets.Add(new AttackedTarget((Monster)obj, attackResult));
					}
				}
			}
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, StaticData.Range);
			SetDescriptionValue(1, GetDamageAsString(0, p_magicFactor));
			SetDescriptionValue(2, Convert.ToInt32(30.0000019f));
		}
	}
}
