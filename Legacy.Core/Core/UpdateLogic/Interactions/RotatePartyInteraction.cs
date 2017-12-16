using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class RotatePartyInteraction : BaseInteraction
	{
		protected InteractiveObject m_parent;

		public RotatePartyInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			InteractiveObject interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			if (interactiveObject == null)
			{
				throw new InvalidOperationException("Tried to find the position of something that is not an object!");
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			EDirection edirection = EDirection.COUNT;
			if (party.Position != interactiveObject.Position)
			{
				edirection = EDirectionFunctions.GetDirection(party.Position, interactiveObject.Position);
			}
			else
			{
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position);
				foreach (Spawn spawn in slot.SpawnObjects)
				{
					if (spawn.ID == m_targetSpawnID)
					{
						edirection = spawn.Direction;
						break;
					}
				}
			}
			Int32 num = party.Direction - edirection;
			if (num == 1 || num == -3)
			{
				RotateCommand p_command = RotateCommand.Left;
				LegacyLogic.Instance.CommandManager.AddCommand(p_command);
			}
			if (num == 2 || num == -2)
			{
				RotateCommand p_command = RotateCommand.TurnAround;
				LegacyLogic.Instance.CommandManager.AddCommand(p_command);
			}
			if (num == 3 || num == -1)
			{
				RotateCommand p_command = RotateCommand.Right;
				LegacyLogic.Instance.CommandManager.AddCommand(p_command);
			}
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
	}
}
