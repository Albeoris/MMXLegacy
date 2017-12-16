using System;
using Legacy.Core.Abilities;
using Legacy.Core.ActionLogging;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Earth
{
	public class CharacterSpellEntangle : CharacterSpell
	{
		public CharacterSpellEntangle() : base(ECharacterSpell.SPELL_EARTH_ENTANGLE)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
			SetDescriptionValue(1, StaticData.Range);
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(2, staticData.GetDuration(1));
		}

		protected override void HandleMonsterBuffs(Character p_sorcerer, SpellEventArgs p_result, Monster p_target, Single p_magicFactor)
		{
			if (!p_target.AbilityHandler.HasAbility(EMonsterAbilityType.LARGE))
			{
				base.HandleMonsterBuffs(p_sorcerer, p_result, p_target, p_magicFactor);
			}
			else
			{
				MonsterAbilityBase ability = p_target.AbilityHandler.GetAbility(EMonsterAbilityType.LARGE);
				AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_target, ability);
				p_target.AbilityHandler.AddEntry(ability.ExecutionPhase, p_args);
			}
		}
	}
}
