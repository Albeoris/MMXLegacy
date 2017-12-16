using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Quests;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class ObjectiveNotSolvedCondition : DialogCondition
	{
		[XmlAttribute("questID")]
		public Int32 QuestID { get; set; }

		[XmlAttribute("objectiveID")]
		public Int32 ObjectiveID { get; set; }

		public override EDialogState CheckCondition(Npc p_npc)
		{
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(QuestID);
			if (step != null)
			{
				QuestObjective objective = step.GetObjective(ObjectiveID);
				if (objective != null && objective.QuestState != EQuestState.SOLVED)
				{
					return EDialogState.NORMAL;
				}
			}
			return FailState;
		}
	}
}
