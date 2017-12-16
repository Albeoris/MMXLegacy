using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ResetGridHeightInteraction : BaseInteraction
	{
		protected InteractiveObject m_parent;

		public ResetGridHeightInteraction()
		{
		}

		public ResetGridHeightInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GRID_HEIGHT_RESET, null);
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.REEVALUATE_PASSABLE, EventArgs.Empty);
			FinishExecution();
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

		public void NotifyHeightChanged(Position p_pos, Single p_newHeight)
		{
			GridSlot slot = Grid.GetSlot(new Position(p_pos.X, p_pos.Y));
			slot.Height = p_newHeight;
		}
	}
}
