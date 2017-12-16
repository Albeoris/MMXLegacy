using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellCelestialArmour : MonsterSpell
	{
		private List<Monster> m_targetBuffer = new List<Monster>();

		public MonsterSpellCelestialArmour(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.CELESTIAL_ARMOUR, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_monster.Position);
			IList<MovingEntity> entities = slot.Entities;
			m_targetBuffer.Clear();
			foreach (MovingEntity movingEntity in entities)
			{
				Monster monster = movingEntity as Monster;
				if (monster != null && !monster.BuffHandler.HasBuff(EMonsterBuffType.CELESTIAL_ARMOUR))
				{
					m_targetBuffer.Add(monster);
				}
			}
			Int32 index = Random.Range(0, m_targetBuffer.Count);
			if (m_targetBuffer.Count != 0)
			{
				Monster monster2 = m_targetBuffer[index];
				monster2.AddBuff(BuffFactory.CreateMonsterBuff(EMonsterBuffType.CELESTIAL_ARMOUR, p_monster.MagicPower, m_level));
			}
			m_cooldown = 3;
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, 27);
			Int32 p_value = (Int32)Math.Round(staticData.GetBuffValues(m_level)[0] * p_monster.MagicPower, MidpointRounding.AwayFromZero);
			SetDescriptionValue(0, p_value);
			SetDescriptionValue(1, staticData.GetDuration(0));
			return Localization.Instance.GetText("MONSTER_SPELL_CELESTIAL_ARMOUR_INFO", m_descriptionValues);
		}
	}
}
