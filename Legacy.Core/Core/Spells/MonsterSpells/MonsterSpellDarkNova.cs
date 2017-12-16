using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellDarkNova : MonsterSpell
	{
		public MonsterSpellDarkNova(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.DARK_NOVA, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			if (Random.Value < m_staticData.GetAdditionalValue(m_level))
			{
				base.DoEffect(p_monster, p_target, p_spellArgs);
			}
		}
	}
}
