using System;

namespace Legacy.Core.Entities.AI.Events
{
	public class AIEventHealthPercent : AIEvent
	{
		private Single m_percentValue;

		private Monster m_owner;

		public AIEventHealthPercent(Single p_percentValue, Monster p_owner) : this(p_percentValue, p_owner, false)
		{
		}

		public AIEventHealthPercent(Single p_percentValue, Monster p_owner, Boolean p_autoReset) : base(p_autoReset)
		{
			m_percentValue = p_percentValue;
			m_owner = p_owner;
		}

		public override void Update()
		{
			if (!IsTriggered && m_owner.CurrentHealth != 0f && m_owner.CurrentHealth / (Single)m_owner.MaxHealth <= m_percentValue)
			{
				Trigger();
			}
		}
	}
}
