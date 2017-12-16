using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ChangeCounterInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private Int32 m_delta;

		protected CounterObject m_interactiveObject;

		protected InteractiveObject m_parent;

		public ChangeCounterInteraction()
		{
		}

		public ChangeCounterInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
			InteractiveObject interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_interactiveObject = (interactiveObject as CounterObject);
			if (m_interactiveObject == null)
			{
				throw new InvalidOperationException("Tried to set counter for something that is not a CounterObject!");
			}
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
			m_delta = Convert.ToInt32(array[0]);
		}

		protected override void DoExecute()
		{
			m_interactiveObject.ChangeCounter(m_delta);
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
	}
}
