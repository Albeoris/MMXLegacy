using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Entities.TrapEffects
{
	public class BaseTrapEffect : BaseObject
	{
		protected TrapEffectStaticData m_staticData;

		protected InteractiveObject m_parent;

		public BaseTrapEffect(Int32 p_staticID, InteractiveObject p_parent) : base(p_staticID, EObjectType.TRAP_EFFECT, 0)
		{
			m_parent = p_parent;
		}

		public TrapEffectStaticData StaticData => m_staticData;

	    protected override void LoadStaticData()
		{
			if (StaticID == 0)
			{
				return;
			}
			m_staticData = StaticDataHandler.GetStaticData<TrapEffectStaticData>(EDataType.TRAP_EFFECT, StaticID);
		}

		public virtual void ResolveEffect(Party p_party)
		{
			throw new NotSupportedException();
		}

		protected virtual void NotifyListeners(Party p_party)
		{
			BarkEventArgs[] array = new BarkEventArgs[4];
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = p_party.GetMember(i);
				array[i] = member.BarkHandler.GenerateBarkEventArgs(EBarks.GET_HIT);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.TRAP_TRIGGERED, new TrapEventArgs(m_staticData, m_parent, array));
			TrapTriggeredEventArgs p_args = new TrapTriggeredEventArgs(this);
			LegacyLogic.Instance.ActionLog.PushEntry(p_args);
		}
	}
}
