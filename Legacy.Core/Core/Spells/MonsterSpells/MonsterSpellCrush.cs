using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellCrush : MonsterSpell
	{
		public MonsterSpellCrush(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.CRUSH, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			if (Random.Value < m_staticData.GetAdditionalValue(m_level))
			{
				for (Int32 i = 0; i < m_staticData.InflictedConditions.Length; i++)
				{
					p_target.ConditionHandler.AddCondition(m_staticData.InflictedConditions[i]);
				}
			}
		}
	}
}
