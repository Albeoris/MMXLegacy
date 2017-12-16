using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.EventManagement
{
	public class PartyBuffTarget : SpellTarget
	{
		public readonly EPartyBuffs Buff;

		public PartyBuffTarget(Object p_Target, EPartyBuffs p_buff) : base(p_Target)
		{
			Buff = p_buff;
		}

		public override String ToString()
		{
			return "[PartyBuffTarget Buff=" + Buff + "]";
		}
	}
}
