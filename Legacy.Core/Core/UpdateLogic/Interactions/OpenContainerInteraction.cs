using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class OpenContainerInteraction : BaseInteraction
	{
		private InteractiveObject m_parent;

		private Container m_container;

		public OpenContainerInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
			m_container = Grid.FindInteractiveObject<Container>(m_targetSpawnID);
			DeactiveContainerIfEmpty();
		}

		protected override void DoExecute()
		{
			if (m_container == null)
			{
				throw new InvalidOperationException("Tried to open something that is not a container!");
			}
			m_container.InteractionOpenContainer(this);
			FinishExecution();
		}

		public override void FinishExecution()
		{
			if (m_container != null && m_parent != null)
			{
				m_container.OpenContainer();
				if (!DeactiveContainerIfEmpty() && m_activateCount > 0 && m_container.IsEmptyCheckWithoutMonster())
				{
					m_activateCount--;
					m_parent.DecreaseActivate(m_commandIndex);
				}
			}
			base.FinishExecution();
		}

		public Boolean DeactiveContainerIfEmpty()
		{
			if (m_container != null && m_container.IsEmpty())
			{
				m_activateCount = 0;
				m_parent.Commands[m_commandIndex].ActivateCount = 0;
				return true;
			}
			if (m_container != null && !m_container.IsEmpty())
			{
				m_activateCount = 1;
				m_parent.Commands[m_commandIndex].ActivateCount = 1;
			}
			return false;
		}

		protected override void ParseExtra(String p_extra)
		{
		}
	}
}
