using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class SetStateInteraction : BaseInteraction
	{
		private InteractiveObject m_targetObject;

		private InteractiveObject m_parent;

		private EInteractiveObjectState m_newState;

		public SetStateInteraction()
		{
		}

		public SetStateInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_targetObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		public InteractiveObject Target => m_targetObject;

	    protected override void DoExecute()
		{
			if (m_targetObject == null)
			{
				throw new InvalidOperationException("Tried to set state for an invalid object!");
			}
			m_targetObject.State = m_newState;
			LegacyLogic.Instance.EventManager.InvokeEvent(m_targetObject, EEventType.OBJECT_STATE_CHANGED, EventArgs.Empty);
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
					"Could not parse interaction params '",
					p_extra,
					"' because it contains ",
					array.Length,
					" arguments instead of ",
					1
				}));
			}
			m_newState = (EInteractiveObjectState)Enum.Parse(typeof(EInteractiveObjectState), array[0]);
		}
	}
}
