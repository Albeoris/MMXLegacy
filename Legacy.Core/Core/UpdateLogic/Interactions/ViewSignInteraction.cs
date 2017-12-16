using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ViewSignInteraction : BaseInteraction
	{
		protected InteractiveObject m_parent;

		protected InteractiveObject m_targetObject;

		public ViewSignInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
			m_targetObject = Grid.FindInteractiveObject(m_targetSpawnID);
		}

		public InteractiveObject Target => m_targetObject;

	    public override void FinishExecution()
		{
			if (m_parent != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_parent.DecreaseActivate(m_commandIndex);
			}
			base.FinishExecution();
		}

		protected override void ParseExtra(String p_extra)
		{
		}
	}
}
