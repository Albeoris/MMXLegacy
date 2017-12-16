using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Quests;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class QuestNotInactiveCondition : DialogCondition
	{
		[XmlAttribute("questID")]
		public Int32 QuestID { get; set; }

		public override EDialogState CheckCondition(Npc p_npc)
		{
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(QuestID);
			if (step != null && step.QuestState != EQuestState.INACTIVE)
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
