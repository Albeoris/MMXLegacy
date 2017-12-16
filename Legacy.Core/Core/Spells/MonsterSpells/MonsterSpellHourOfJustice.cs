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
	public class MonsterSpellHourOfJustice : MonsterSpell
	{
		private List<Monster> m_targetBuffer = new List<Monster>();

		public MonsterSpellHourOfJustice(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.HOUR_OF_JUSTICE, p_effectAnimationClip, p_castProbability)
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
				if (monster != null && !monster.BuffHandler.HasBuff(EMonsterBuffType.HOUR_OF_JUSTICE))
				{
					m_targetBuffer.Add(monster);
				}
			}
			Int32 index = Random.Range(0, m_targetBuffer.Count);
			if (m_targetBuffer.Count != 0)
			{
				Monster monster2 = m_targetBuffer[index];
				monster2.AddBuff(BuffFactory.CreateMonsterBuff(EMonsterBuffType.HOUR_OF_JUSTICE, p_monster.MagicPower, m_level));
			}
			m_cooldown = 3;
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, 28);
			Int32 p_value = (Int32)Math.Round(staticData.GetBuffValues(m_level)[0] * p_monster.MagicPower * 100f, MidpointRounding.AwayFromZero);
			Int32 p_value2 = (Int32)Math.Round(staticData.GetBuffValues(m_level)[1] * p_monster.MagicPower * 100f, MidpointRounding.AwayFromZero);
			SetDescriptionValue(0, p_value);
			SetDescriptionValue(1, p_value2);
			SetDescriptionValue(2, staticData.GetDuration(m_level));
			return Localization.Instance.GetText("MONSTER_SPELL_HOUR_OF_JUSTICE_INFO", m_descriptionValues);
		}

		public override Boolean CheckSpellPreconditions(Monster p_monster)
		{
			if (m_cooldown > 0)
			{
				return false;
			}
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_monster.Position);
			IList<MovingEntity> entities = slot.Entities;
			for (Int32 i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Monster)
				{
					Monster monster = (Monster)entities[i];
					if (!monster.BuffHandler.HasBuff(EMonsterBuffType.HOUR_OF_JUSTICE))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
