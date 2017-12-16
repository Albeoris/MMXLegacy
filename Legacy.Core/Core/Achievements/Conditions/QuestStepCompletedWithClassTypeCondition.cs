using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;
using Legacy.Core.Quests;

namespace Legacy.Core.Achievements.Conditions
{
	public class QuestStepCompletedWithClassTypeCondition : AchievementCondition
	{
		private Int32 m_questID;

		private EClassType m_classType;

		public QuestStepCompletedWithClassTypeCondition(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
		}

		public override void ParseParameter(String p_parameterString)
		{
			String[] array = p_parameterString.Split(new Char[]
			{
				','
			});
			Int32.TryParse(array[0], out m_questID);
			if (Enum.IsDefined(typeof(EClassType), array[1]))
			{
				m_classType = (EClassType)Enum.Parse(typeof(EClassType), array[1]);
			}
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = 0;
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_questID);
			if (step != null && step.QuestState == EQuestState.SOLVED && CheckClasses())
			{
				p_count = 1;
				return true;
			}
			return false;
		}

		private Boolean CheckClasses()
		{
			Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
			for (Int32 i = 0; i < members.Length; i++)
			{
				if (members[i].Class.GetClassType() != m_classType)
				{
					return false;
				}
			}
			return true;
		}

		internal EClassType ClassType => m_classType;

	    internal Int32 QuestID => m_questID;
	}
}
