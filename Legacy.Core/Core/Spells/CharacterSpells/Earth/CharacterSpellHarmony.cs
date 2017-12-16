using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Earth
{
	public class CharacterSpellHarmony : CharacterSpell
	{
		public CharacterSpellHarmony() : base(ECharacterSpell.SPELL_EARTH_HARMONY)
		{
		}

		protected override void HandleMonsters(Character p_sorcerer, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			List<Monster> objectsByType = LegacyLogic.Instance.WorldManager.GetObjectsByType<Monster>();
			foreach (Monster monster in objectsByType)
			{
				MonsterBuff monsterBuff = BuffFactory.CreateMonsterBuff(EMonsterBuffType.HARMONY, p_magicFactor);
				monsterBuff.ScaleDuration(p_magicFactor);
				monster.BuffHandler.AddBuff(monsterBuff);
				p_targets.Add(monster);
				Boolean p_Successful = monster.BuffHandler.HasBuff(EMonsterBuffType.HARMONY);
				Boolean p_IsImmune = false;
				if (!monster.AbilityHandler.CanAddBuff(monsterBuff.Type))
				{
					p_IsImmune = true;
				}
				p_result.SpellTargets.Add(new MonsterBuffTarget(monster, EMonsterBuffType.HARMONY, p_Successful, p_IsImmune));
			}
		}

		public override Boolean CheckSpellConditions(Character p_sorcerer)
		{
			return LegacyLogic.Instance.WorldManager.Party.HasAggro();
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, 21);
			SetDescriptionValue(0, (Int32)(p_magicFactor * staticData.GetDuration(1) + 0.5f));
		}
	}
}
