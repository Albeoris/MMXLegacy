using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Entities.TrapEffects
{
	public class DispelMagicTrapEffect : BaseTrapEffect
	{
		public DispelMagicTrapEffect(Int32 p_staticID, InteractiveObject p_parent) : base(p_staticID, p_parent)
		{
		}

		public override void ResolveEffect(Party p_party)
		{
			p_party.Buffs.RemoveAllBuffs();
			NotifyListeners(p_party);
			MessageEventArgs p_args = new MessageEventArgs("ACTION_LOG_TRAP_BUFFS_REMOVED");
			LegacyLogic.Instance.ActionLog.PushEntry(p_args);
		}
	}
}
