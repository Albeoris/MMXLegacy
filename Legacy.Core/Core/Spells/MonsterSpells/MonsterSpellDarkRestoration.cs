using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellDarkRestoration : MonsterSpell
	{
		public MonsterSpellDarkRestoration(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.DARK_RESTORATION, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			p_monster.ChangeHP(p_monster.MaxHealth, null);
		}
	}
}
