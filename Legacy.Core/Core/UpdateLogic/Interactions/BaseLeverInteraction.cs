using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public abstract class BaseLeverInteraction : BaseInteraction
	{
		protected InteractiveObject m_parent;

		protected Lever m_targetLever;

		public BaseLeverInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
			InteractiveObject interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_targetLever = (interactiveObject as Lever);
		}

		public InteractiveObject Target => m_targetLever;

	    protected override void DoExecute()
		{
			SetStates();
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(m_targetLever, m_targetLever.Position);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.LEVER_STATE_CHANGED, p_eventArgs);
		}

		protected abstract void SetStates();

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
