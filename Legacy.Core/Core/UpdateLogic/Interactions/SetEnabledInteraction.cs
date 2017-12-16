using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class SetEnabledInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private InteractiveObject m_targetObject;

		protected InteractiveObject m_parent;

		private Boolean m_newEnabled;

		public SetEnabledInteraction()
		{
		}

		public SetEnabledInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_targetObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			if (m_targetObject == null)
			{
				throw new InvalidOperationException("Tried to enable an invalid object!");
			}
			m_targetObject.Enabled = m_newEnabled;
			LegacyLogic.Instance.EventManager.InvokeEvent(m_targetObject, EEventType.OBJECT_ENABLED_CHANGED, EventArgs.Empty);
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
			if (!Boolean.TryParse(array[0], out m_newEnabled))
			{
				throw new FormatException("First parameter was not a bool!");
			}
		}
	}
}
