using System;
using System.IO;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class UpdateQuestObjectiveInteraction : BaseInteraction
	{
		private Int32 m_questObjectiveID;

		protected InteractiveObject m_parent;

		public UpdateQuestObjectiveInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			LegacyLogic.Instance.WorldManager.QuestHandler.ObjectInteraction(m_questObjectiveID);
			FinishExecution();
		}

		protected override void ParseExtra(String p_extra)
		{
			if (!String.IsNullOrEmpty(p_extra))
			{
				m_questObjectiveID = Int32.Parse(p_extra);
				return;
			}
			throw new InvalidDataException("The extra value contains no valid data");
		}

		public override void FinishExecution()
		{
			if (m_parent != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_parent.DecreaseActivate(m_commandIndex);
			}
			base.FinishExecution();
		}
	}
}
