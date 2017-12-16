using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class CutsceneInteraction : BaseInteraction
	{
		protected Cutscene m_targetCutscene;

		protected InteractiveObject m_parent;

		public CutsceneInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_targetCutscene = (Grid.FindInteractiveObject(m_targetSpawnID) as Cutscene);
			if (m_targetCutscene == null)
			{
				throw new Exception("Target cutscene ID=" + m_targetSpawnID + " not found!!");
			}
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			m_targetCutscene.StartCutscene(this);
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

		protected override void ParseExtra(String p_extra)
		{
		}
	}
}
