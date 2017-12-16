using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Earth
{
	public class CharacterSpellCrushingWeight : CharacterSpell
	{
		public CharacterSpellCrushingWeight() : base(ECharacterSpell.SPELL_EARTH_CRUSHING_WEIGHT)
		{
		}

		protected override AttackResult DoAttackMonster(Character p_attacker, Monster p_target, Single p_magicPower)
		{
			if (p_target != null)
			{
				Single criticalMagicHitChance = p_attacker.FightValues.CriticalMagicHitChance;
				Single magicalCriticalDamageMod = p_attacker.FightValues.MagicalCriticalDamageMod;
				List<Damage> list = new List<Damage>();
				Int32 num = Convert.ToInt32(Math.Round(p_target.CurrentHealth * m_staticData.Damage[0].Minimum / 100f, MidpointRounding.AwayFromZero));
				DamageData p_data = new DamageData(EDamageType.EARTH, num, num);
				list.Add(Damage.Create(p_data, magicalCriticalDamageMod));
				Attack p_attack = new Attack(0f, criticalMagicHitChance, list);
				EDamageType p_damageType = ESkillIDToEDamageType(m_staticData.SkillID);
				return p_target.CombatHandler.AttackMonster(p_attacker, p_attack, false, true, p_damageType, true, p_attacker.SkillHandler.GetResistanceIgnoreValue(m_staticData.SkillID));
			}
			return null;
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, (Int32)(m_staticData.Damage[0].Minimum + 0.5f));
		}
	}
}
