using System;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;

namespace Legacy.Core.EventManagement
{
	public class ItemSuffixApplyBuffEventArgs : EventArgs
	{
		public ItemSuffixApplyBuffEventArgs(Monster p_targetMonster, String p_suffixKey, EMonsterBuffType p_buff)
		{
			TargetMonster = p_targetMonster;
			SuffixKey = p_suffixKey;
			Buff = p_buff;
		}

		public Monster TargetMonster { get; private set; }

		public String SuffixKey { get; private set; }

		public EMonsterBuffType Buff { get; private set; }
	}
}
