using System;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareShatter : CharacterSpell
	{
		public CharacterWarfareShatter() : base(ECharacterSpell.WARFARE_SHATTER)
		{
		}

		public override Single GetMagicFactor(Character p_sorcerer, Boolean p_fromScroll, Int32 p_scrollTier)
		{
			Int32 num = 1;
			Skill skill = p_sorcerer.SkillHandler.FindSkill(11);
			if (skill != null)
			{
				num = skill.Level + skill.VirtualSkillLevel;
			}
			return 1f + 0.075f * num + 0.02f * p_sorcerer.CurrentAttributes.Might + 0.02f * p_sorcerer.CurrentAttributes.Perception;
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, (Int32)(staticData.GetBuffValues(1)[0] * p_magicFactor + 0.5f));
		}
	}
}
