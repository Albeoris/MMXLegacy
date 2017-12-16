using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Entities.TrapEffects
{
	public class ManaVoidTrapEffect : BaseTrapEffect
	{
		public ManaVoidTrapEffect(Int32 p_staticID, InteractiveObject p_parent) : base(p_staticID, p_parent)
		{
		}

		public override void ResolveEffect(Party p_party)
		{
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = p_party.GetMember(i);
				Single num = member.ManaPoints * m_staticData.PercentageValue * 0.01f * ((100 - member.BaseResistance[EDamageType.PRIMORDIAL].Value) * 0.01f);
				member.ChangeMP(-(Int32)num);
				if (num > 0f)
				{
					TrapManaLostEventArgs p_args = new TrapManaLostEventArgs(member, (Int32)num);
					LegacyLogic.Instance.ActionLog.PushEntry(p_args);
				}
			}
			NotifyListeners(p_party);
		}
	}
}
