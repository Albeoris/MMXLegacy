using System;
using Legacy.Core.Buffs;

namespace Legacy.Core.EventManagement
{
	public class MonsterBuffTarget : SpellTarget
	{
		public readonly EMonsterBuffType Buff;

		public readonly Boolean Successful;

		public readonly Boolean IsImmune;

		public MonsterBuffTarget(Object p_Target, EMonsterBuffType p_Buff, Boolean p_Successful, Boolean p_IsImmune) : base(p_Target)
		{
			Buff = p_Buff;
			Successful = p_Successful;
			IsImmune = p_IsImmune;
		}

		public override String ToString()
		{
			return String.Concat(new Object[]
			{
				"[MonsterBuffTarget Target=",
				Target,
				" Buff=",
				Buff,
				" Successful=",
				Successful,
				"]"
			});
		}
	}
}
