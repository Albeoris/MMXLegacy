using System;

namespace Legacy.Core.EventManagement
{
	public class StateChangedEventArgs : EventArgs
	{
		private Int32 m_fromState;

		private Int32 m_toState;

		public StateChangedEventArgs(Int32 p_fromState, Int32 p_toState)
		{
			m_fromState = p_fromState;
			m_toState = p_toState;
		}

		public Int32 FromState => m_fromState;

	    public Int32 ToState => m_toState;
	}
}
