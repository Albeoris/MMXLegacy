using System;
using System.Threading;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.Entities.AI.Events
{
	public abstract class AIEvent
	{
		protected TriggerHelper m_trigger;

		private Boolean m_autoReset;

		public AIEvent() : this(false)
		{
		}

		public AIEvent(Boolean p_autoReset)
		{
			m_trigger = new TriggerHelper();
			m_autoReset = p_autoReset;
		}

		public event Action OnTrigger;

		public Boolean IsTriggered => m_trigger.IsTriggered;

	    public abstract void Update();

		public void Reset()
		{
			m_trigger.Reset();
		}

		public void Trigger()
		{
			if (OnTrigger != null)
			{
				OnTrigger();
			}
			if (!m_autoReset)
			{
				m_trigger.Trigger();
			}
		}
	}
}
