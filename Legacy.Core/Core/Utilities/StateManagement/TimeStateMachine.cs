using System;
using System.Collections.Generic;

namespace Legacy.Core.Utilities.StateManagement
{
	public class TimeStateMachine<T> : StateMachine<T, TimeState<T>, Transition<T>>
	{
		protected Single m_currentStateDuration;

		protected Single m_currentStateTime;

		public TimeStateMachine(IEqualityComparer<T> p_idComparer) : base(p_idComparer)
		{
		}

		public TimeStateMachine()
		{
		}

		public Single CurrentStateDuration => m_currentStateDuration;

	    public Single CurrentStateTime
		{
			get => m_currentStateTime;
	        set => m_currentStateTime = ((value <= m_currentStateDuration) ? value : m_currentStateDuration);
	    }

		public Single CurrentStateTimePer => m_currentStateTime / (m_currentStateDuration + Single.Epsilon);

	    public Boolean IsStateTimeout => m_currentStateTime >= m_currentStateDuration;

	    public Single TimeLeft
		{
			get => m_currentStateTime - m_currentStateDuration;
	        set => m_currentStateTime = Math.Max(m_currentStateDuration - value, 0f);
	    }

		public void FinishState()
		{
			m_currentStateTime = m_currentStateDuration;
		}

		public void Update(Single deltaTime)
		{
			base.Update();
			m_currentStateTime += deltaTime;
			if (m_currentStateTime > m_currentStateDuration)
			{
				m_currentStateTime = m_currentStateDuration;
			}
		}

		public override void ChangeState(T p_newStateId)
		{
			base.ChangeState(p_newStateId);
			m_currentStateTime = 0f;
			m_currentStateDuration = m_currentState.StateDuration;
		}

		public void ChangeState(T p_newStateId, Single p_stateDuration)
		{
			if (p_stateDuration < 0f)
			{
				throw new ArgumentOutOfRangeException("p_stateDuration", "p_stateDuration < 0");
			}
			base.ChangeState(p_newStateId);
			m_currentStateTime = 0f;
			m_currentStateDuration = p_stateDuration;
		}
	}
}
