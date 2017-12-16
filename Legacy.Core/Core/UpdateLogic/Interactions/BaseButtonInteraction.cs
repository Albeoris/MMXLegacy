using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public abstract class BaseButtonInteraction : BaseInteraction
	{
		protected InteractiveObject m_targetButton;

		protected InteractiveObject m_parent;

		public BaseButtonInteraction()
		{
		}

		public BaseButtonInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
			if (m_parent == null)
			{
				LegacyLogger.LogError("Not found parent trigger ID: " + p_parentID);
			}
			m_targetButton = Grid.FindInteractiveObject(m_targetSpawnID);
			if (m_targetButton == null)
			{
				LegacyLogger.LogError("Not found target trigger ID: " + m_targetSpawnID);
			}
		}

		public InteractiveObject Target => m_targetButton;

	    protected override void DoExecute()
		{
			SetStates();
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(m_targetButton, m_targetButton.Position);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.OBJECT_STATE_CHANGED, p_eventArgs);
			FinishExecution();
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
