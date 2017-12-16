using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Entities.TrapEffects
{
	public class DamageDealingTrapEffect : BaseTrapEffect
	{
		protected AttackResult[] attackResults;

		protected ECondition[] conditionsReceived;

		public DamageDealingTrapEffect(Int32 p_staticID, InteractiveObject p_parent) : base(p_staticID, p_parent)
		{
			attackResults = new AttackResult[4];
			conditionsReceived = new ECondition[4];
			for (Int32 i = 0; i < 4; i++)
			{
				conditionsReceived[i] = ECondition.NONE;
			}
		}

		public AttackResult[] AttackResults => attackResults;

	    public ECondition[] ConditionsReceived => conditionsReceived;

	    protected override void NotifyListeners(Party p_party)
		{
			base.NotifyListeners(p_party);
			for (Int32 i = 0; i < 4; i++)
			{
				AttackResult p_result = attackResults[i];
				Character member = p_party.GetMember(i);
				TrapDamageEventArgs p_args = new TrapDamageEventArgs(this, member, p_result, conditionsReceived[i]);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
		}
	}
}
