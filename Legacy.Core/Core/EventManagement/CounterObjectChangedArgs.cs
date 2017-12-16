using System;

namespace Legacy.Core.EventManagement
{
	public class CounterObjectChangedArgs : EventArgs
	{
		private Int32 m_oldCount;

		private Int32 m_newCount;

		private Int32 m_changeCount;

		public CounterObjectChangedArgs(Int32 p_oldCount, Int32 p_newCount, Int32 p_changeCount)
		{
			m_oldCount = p_oldCount;
			m_newCount = p_newCount;
			m_changeCount = p_changeCount;
		}

		public Int32 OldCount => m_oldCount;

	    public Int32 NewCount => m_newCount;

	    public Int32 ChangeCount => m_changeCount;
	}
}
