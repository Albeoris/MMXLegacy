using System;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;

namespace Legacy.Core.EventManagement
{
	public class BuffPerformedEventArgs : EventArgs
	{
		public BuffPerformedEventArgs(MonsterBuff buff, AttackResult result)
		{
			MonsterBuff = buff;
			Result = result;
		}

		public MonsterBuff MonsterBuff { get; set; }

		public AttackResult Result { get; set; }
	}
}
