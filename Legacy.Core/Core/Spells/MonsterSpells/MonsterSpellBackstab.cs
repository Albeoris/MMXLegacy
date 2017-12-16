using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellBackstab : MonsterSpell
	{
		public MonsterSpellBackstab(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.BACKSTAB, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
		}
	}
}
