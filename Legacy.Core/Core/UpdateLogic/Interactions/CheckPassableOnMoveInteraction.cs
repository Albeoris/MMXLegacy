using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class CheckPassableOnMoveInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private Boolean m_newValue;

		protected InteractiveObject m_parent;

		public CheckPassableOnMoveInteraction()
		{
		}

		public CheckPassableOnMoveInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			LegacyLogic.Instance.WorldManager.CheckPassableOnMovement = m_newValue;
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
			if (!Boolean.TryParse(array[0], out m_newValue))
			{
				throw new FormatException("First parameter was not a bool!");
			}
		}
	}
}
