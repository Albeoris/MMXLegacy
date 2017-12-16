using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class UpdateGridSlotHeightInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 3;

		private Int32 m_gridX;

		private Int32 m_gridY;

		private Single m_delta;

		protected InteractiveObject m_parent;

		public UpdateGridSlotHeightInteraction()
		{
		}

		public UpdateGridSlotHeightInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			GridSlot slot = Grid.GetSlot(new Position(m_gridX, m_gridY));
			slot.Height += m_delta;
			LegacyLogic.Instance.EventManager.InvokeEvent(slot, EEventType.SLOT_HEIGHT_CHANGED, null);
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
			if (array.Length != 3)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					3
				}));
			}
			m_gridX = Convert.ToInt32(array[0]);
			m_gridY = Convert.ToInt32(array[1]);
			Single.TryParse(array[2], out m_delta);
		}
	}
}
