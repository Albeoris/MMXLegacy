using System;

namespace Legacy.Core.Utilities.StateManagement
{
	public class TriggerHelper
	{
		protected Boolean m_isTriggered;

		public TriggerHelper()
		{
			Reset();
		}

		public Boolean IsTriggered => m_isTriggered;

	    public void Reset()
		{
			m_isTriggered = false;
		}

		public void Trigger()
		{
			m_isTriggered = true;
		}
	}
}
