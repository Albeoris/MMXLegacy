using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;

namespace Legacy.Core.Entities.TrapEffects
{
	public class IntruderAlertTrapEffect : BaseTrapEffect
	{
		public IntruderAlertTrapEffect(Int32 p_staticID, InteractiveObject p_parent) : base(p_staticID, p_parent)
		{
		}

		public override void ResolveEffect(Party p_party)
		{
			MonsterTurnActor monsterTurnActor = LegacyLogic.Instance.UpdateManager.MonsterTurnActor;
			monsterTurnActor.OverrideAggroRanges(m_staticData.AbsoluteValueMin);
			NotifyListeners(p_party);
			MessageEventArgs p_args = new MessageEventArgs("ACTION_LOG_TRAP_INTRUDER_ALERT");
			LegacyLogic.Instance.ActionLog.PushEntry(p_args);
		}
	}
}
