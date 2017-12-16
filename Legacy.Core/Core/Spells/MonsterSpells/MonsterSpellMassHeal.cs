using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellMassHeal : MonsterSpell
	{
		public MonsterSpellMassHeal(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.MASS_HEAL, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			p_spellArgs.SpellTargets.Clear();
			DamageData damageData = BaseDamage[0];
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_monster.Position);
			IList<MovingEntity> entities = slot.Entities;
			for (Int32 i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Monster)
				{
					Int32 num = Random.Range(damageData.Minimum, damageData.Maximum + 1);
					Int32 num2 = (Int32)p_monster.MagicPower;
					num *= num2;
					((Monster)entities[i]).ChangeHP(num, null);
					p_spellArgs.SpellTargets.Add(new HealedTarget((Monster)entities[i], num, false));
				}
			}
			p_spellArgs.Result = ESpellResult.OK;
			m_cooldown = 3;
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, GetDamageAsString());
			return Localization.Instance.GetText("MONSTER_SPELL_MASS_HEAL_INFO", m_descriptionValues);
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
					if (monster.CurrentHealth < monster.MaxHealth)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
