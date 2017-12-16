using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ToggleButtonInteraction : BaseButtonInteraction
	{
		public ToggleButtonInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
		}

		protected override void SetStates()
		{
			if (m_targetButton == null)
			{
				throw new InvalidOperationException("Tried to push or pull something that is not a lever!");
			}
			if (m_targetButton is Button)
			{
				((Button)m_targetButton).ToggleState();
			}
			else if (m_targetButton is PressurePlate)
			{
				((PressurePlate)m_targetButton).ToggleState();
			}
		}
	}
}
