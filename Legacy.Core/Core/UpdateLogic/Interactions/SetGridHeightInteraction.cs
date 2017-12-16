using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class SetGridHeightInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private Single m_newHeight;

		protected InteractiveObject m_parent;

		public SetGridHeightInteraction()
		{
		}

		public SetGridHeightInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		public Single NewHeight => m_newHeight;

	    protected override void DoExecute()
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GRID_HEIGHT_SET, null);
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
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (array.Length != 1)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					1
				}));
			}
			Single.TryParse(array[0], out m_newHeight);
		}

		public void NotifyHeightChanged(Position p_pos, Single p_newHeight)
		{
			GridSlot slot = Grid.GetSlot(new Position(p_pos.X, p_pos.Y));
			slot.Height = p_newHeight;
		}
	}
}
