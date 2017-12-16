using System;
using Legacy.Core.Entities;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ToggleLeverInteraction : BaseLeverInteraction
	{
		public ToggleLeverInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
		}

		protected override void SetStates()
		{
			if (m_targetLever == null)
			{
				throw new InvalidOperationException("Tried to push or pull something that is not a lever!");
			}
			m_targetLever.ToggleState();
		}
	}
}
