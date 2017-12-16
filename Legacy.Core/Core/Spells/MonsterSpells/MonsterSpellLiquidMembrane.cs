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
	public class MonsterSpellLiquidMembrane : MonsterSpell
	{
		private List<Monster> m_targetBuffer = new List<Monster>();

		public MonsterSpellLiquidMembrane(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.LIQUID_MEMBRANE, p_effectAnimationClip, p_castProbability)
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
				if (monster != null && !monster.BuffHandler.HasBuff(EMonsterBuffType.LIQUIDMEMBRANE))
				{
					m_targetBuffer.Add(monster);
				}
			}
			Int32 index = Random.Range(0, m_targetBuffer.Count);
			if (m_targetBuffer.Count != 0)
			{
				Monster monster2 = m_targetBuffer[index];
				monster2.AddBuff(BuffFactory.CreateMonsterBuff(EMonsterBuffType.LIQUIDMEMBRANE, p_monster.MagicPower, m_level));
			}
			m_cooldown = 3;
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, 14);
			Single p_value = (Single)Math.Round(staticData.GetBuffValues(m_level)[0] * p_monster.MagicPower * 0.1f, MidpointRounding.AwayFromZero);
			SetDescriptionValue(0, p_value);
			return Localization.Instance.GetText("MONSTER_SPELL_LIQUID_MEMBRANE_INFO", m_descriptionValues);
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
					if (!monster.BuffHandler.HasBuff(EMonsterBuffType.LIQUIDMEMBRANE))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
