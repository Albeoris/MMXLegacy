using System;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellWitheringBreath : MonsterSpell
	{
		public MonsterSpellWitheringBreath(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.WITHERING_BREATH, p_effectAnimationClip, p_castProbability)
		{
		}
	}
}
