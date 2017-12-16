using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Items;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class NoShoesInteraction : BaseInteraction
	{
		protected InteractiveObject m_parent;

		public NoShoesInteraction()
		{
		}

		public NoShoesInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void ParseExtra(String p_extra)
		{
		}

		protected override void DoExecute()
		{
			Boolean flag = false;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = party.GetMember(i);
				BaseItem itemAt = member.Equipment.Equipment.GetItemAt(6);
				if (itemAt != null)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				grid.MoveEntity(party, EDirectionFunctions.GetOppositeDir(party.Direction));
			}
			FinishExecution();
			if (flag)
			{
				m_stateMachine.ChangeState(3);
			}
			else
			{
				m_stateMachine.ChangeState(2);
			}
		}

		public override void FinishExecution()
		{
			if (m_parent != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_parent.DecreaseActivate(m_commandIndex);
			}
		}
	}
}
