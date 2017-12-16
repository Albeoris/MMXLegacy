using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Entities.TrapEffects
{
	public class RotTrapEffect : BaseTrapEffect
	{
		public RotTrapEffect(Int32 p_staticID, InteractiveObject p_parent) : base(p_staticID, p_parent)
		{
		}

		public override void ResolveEffect(Party p_party)
		{
			Int32 num = 0;
			for (Int32 i = 0; i < p_party.Supplies; i++)
			{
				if (Random.Range(0, 100) < m_staticData.PercentageValue)
				{
					num++;
				}
			}
			p_party.ConsumeSupply(num);
			NotifyListeners(p_party);
			if (num > 0)
			{
				TrapFoodLostEventArgs p_args = new TrapFoodLostEventArgs(num);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
		}
	}
}
