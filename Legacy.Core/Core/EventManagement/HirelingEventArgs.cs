using System;
using Legacy.Core.NpcInteraction;

namespace Legacy.Core.EventManagement
{
	public class HirelingEventArgs : EventArgs
	{
		private ETargetCondition m_condition;

		private Npc m_npc;

		private Int32 m_index;

		public HirelingEventArgs(ETargetCondition p_condition, Npc p_npc, Int32 p_index)
		{
			m_condition = p_condition;
			m_npc = p_npc;
			m_index = p_index;
		}

		public ETargetCondition Condition => m_condition;

	    public Npc Npc => m_npc;

	    public Int32 Index => m_index;
	}
}
