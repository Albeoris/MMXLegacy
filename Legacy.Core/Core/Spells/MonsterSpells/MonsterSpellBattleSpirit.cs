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
	public class MonsterSpellBattleSpirit : MonsterSpell
	{
		public MonsterSpellBattleSpirit(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.BATTLE_SPIRIT, p_effectAnimationClip, p_castProbability)
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
					monster.AddBuff(BuffFactory.CreateMonsterBuff(EMonsterBuffType.BATTLESPIRIT, p_monster.MagicPower, m_level));
				}
			}
			m_cooldown = 3;
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, p_monster.MagicPower);
			return Localization.Instance.GetText("MONSTER_SPELL_BATTLE_SPIRIT_INFO", m_descriptionValues);
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
					if (!monster.BuffHandler.HasBuff(EMonsterBuffType.BATTLESPIRIT))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
