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
using Legacy.Utilities;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellPurify : MonsterSpell
	{
		public MonsterSpellPurify(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.PURIFY, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_monster.Position);
			IList<MovingEntity> entities = slot.Entities;
			for (Int32 i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Monster)
				{
					Monster monster = (Monster)entities[i];
					List<MonsterBuff> buffList = monster.BuffHandler.BuffList;
					if (buffList != null && buffList.Count != 0)
					{
						List<MonsterBuff> list = new List<MonsterBuff>();
						for (Int32 j = 0; j < buffList.Count; j++)
						{
							if (buffList[j].IsDebuff && (buffList[j].StaticID != 3 || monster != p_monster))
							{
								list.Add(buffList[j]);
							}
						}
						if (list.Count > 0)
						{
							Int32 index = Random.Range(0, list.Count);
							monster.BuffHandler.RemoveBuff(list[index]);
						}
					}
				}
			}
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			return Localization.Instance.GetText("MONSTER_SPELL_PURIFY_INFO", m_descriptionValues);
		}

		public override Boolean CheckSpellPreconditions(Monster p_monster)
		{
			try
			{
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_monster.Position);
				IList<MovingEntity> entities = slot.Entities;
				for (Int32 i = 0; i < entities.Count; i++)
				{
					if (entities[i] is Monster)
					{
						Monster monster = (Monster)entities[i];
						List<MonsterBuff> buffList = monster.BuffHandler.BuffList;
						for (Int32 j = 0; j < buffList.Count; j++)
						{
							if (buffList[j].IsDebuff && (buffList[j].StaticID != 3 || monster != p_monster))
							{
								return true;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LegacyLogger.LogError(ex.Message);
			}
			return false;
		}
	}
}
