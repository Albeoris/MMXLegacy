using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ExecuteInteractiveObjectInteraction : BaseInteraction
	{
		private InteractiveObject m_targetObject;

		protected InteractiveObject m_parent;

		public ExecuteInteractiveObjectInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(p_parentID);
			m_targetObject = Grid.FindInteractiveObject(m_targetSpawnID);
		}

		public InteractiveObject Target => m_targetObject;

	    protected override void DoExecute()
		{
			InteractiveObject interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			if (interactiveObject == null)
			{
				throw new InvalidOperationException("Tried to execute!");
			}
			interactiveObject.ClearInteractions();
			interactiveObject.Execute(LegacyLogic.Instance.MapLoader.Grid);
			interactiveObject.Update();
			FinishExecution();
		}

		protected override void ParseExtra(String p_extra)
		{
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
