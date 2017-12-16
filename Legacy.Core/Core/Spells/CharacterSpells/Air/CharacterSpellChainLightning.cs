using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Air
{
	public class CharacterSpellChainLightning : CharacterSpell
	{
		public CharacterSpellChainLightning() : base(ECharacterSpell.SPELL_AIR_CHAIN_LIGHTNING)
		{
		}

		protected override AttackResult DoAttackMonster(Character p_attacker, Monster p_target, Single p_magicPower)
		{
			Int32 num = 1;
			if (p_target != null)
			{
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_target.Position);
				if (slot != null)
				{
					num = slot.Entities.Count;
				}
				Single criticalMagicHitChance = p_attacker.FightValues.CriticalMagicHitChance;
				Single magicalCriticalDamageMod = p_attacker.FightValues.MagicalCriticalDamageMod;
				EDamageType edamageType = ESkillIDToEDamageType(m_staticData.SkillID);
				List<Damage> list = new List<Damage>();
				for (Int32 i = 0; i < m_staticData.Damage.Length; i++)
				{
					DamageData p_data = DamageData.Scale(m_staticData.Damage[i], p_magicPower);
					Damage item = Damage.Create(p_data, magicalCriticalDamageMod);
					if (item.Type == edamageType)
					{
						item.IgnoreResistance = p_attacker.SkillHandler.GetResistanceIgnoreValue(m_staticData.SkillID);
					}
					item.Value *= num;
					list.Add(item);
				}
				Attack p_attack = new Attack(0f, criticalMagicHitChance, list);
				return p_target.CombatHandler.AttackMonster(p_attacker, p_attack, false, true, edamageType, true, p_attacker.SkillHandler.GetResistanceIgnoreValue(m_staticData.SkillID));
			}
			return null;
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, StaticData.Range);
			SetDescriptionValue(1, GetDamageAsString(0, p_magicFactor));
		}
	}
}
