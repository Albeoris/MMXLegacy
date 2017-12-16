using System;
using Legacy.Core.Quests;

namespace Legacy.Core
{
	public class QuestChangedEventArgs : EventArgs
	{
		private Type m_Type;

		private QuestStep m_QuestStep;

		public QuestChangedEventArgs(Type p_Type, QuestStep p_QuestStep)
		{
			m_Type = p_Type;
			m_QuestStep = p_QuestStep;
		}

		public Type ChangeType => m_Type;

	    public QuestStep QuestStep => m_QuestStep;

	    public enum Type
		{
			NONE,
			NEW_QUEST,
			COMPLETED_QUEST,
			COMPLETED_OBJECTIVE
		}
	}
}
