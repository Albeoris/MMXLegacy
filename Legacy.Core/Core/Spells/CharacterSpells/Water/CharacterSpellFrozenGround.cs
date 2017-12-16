using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Water
{
	public class CharacterSpellFrozenGround : CharacterSpell
	{
		public CharacterSpellFrozenGround() : base(ECharacterSpell.SPELL_WATER_FROZEN_GROUND)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, m_staticData.Range);
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(1, staticData.GetDuration(1));
		}

		protected override void HandleMonsterBuffs(Character p_sorcerer, SpellEventArgs p_result, Monster p_target, Single p_magicFactor)
		{
			if (!p_target.AbilityHandler.HasAbility(EMonsterAbilityType.LARGE))
			{
				base.HandleMonsterBuffs(p_sorcerer, p_result, p_target, p_magicFactor);
			}
		}
	}
}
