using System;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Earth
{
	public class CharacterSpellPoisonSpray : CharacterSpell
	{
		public CharacterSpellPoisonSpray() : base(ECharacterSpell.SPELL_EARTH_POISON_SPRAY)
		{
		}

		protected override AttackResult DoAttackMonster(Character p_sorcerer, Monster p_target, Single p_magicPower)
		{
			if (p_target.AbilityHandler.HasAbility(EMonsterAbilityType.UNDEAD))
			{
				return new AttackResult
				{
					Result = EResultType.IMMUNE
				};
			}
			return base.DoAttackMonster(p_sorcerer, p_target, p_magicPower);
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
			SetDescriptionValue(1, (Int32)(staticData.GetBuffValues(1)[1] * p_magicFactor + 0.5f));
			SetDescriptionValue(2, (Int32)(staticData.GetBuffValues(1)[0] * p_magicFactor + 0.5f));
		}
	}
}
