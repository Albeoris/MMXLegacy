using System;
using System.IO;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class StartDefinedNpcDialogInteraction : BaseInteraction
	{
		private Int32 m_npcID;

		private Int32 m_dialogID;

		protected InteractiveObject m_parent;

		public StartDefinedNpcDialogInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void ParseExtra(String p_extra)
		{
			if (!String.IsNullOrEmpty(p_extra))
			{
				String[] array = p_extra.Split(new Char[]
				{
					','
				});
				if (array.Length == 2)
				{
					m_npcID = Int32.Parse(array[0]);
					m_dialogID = Int32.Parse(array[1]);
				}
				return;
			}
			throw new InvalidDataException("The extra value contains no valid data");
		}

		protected override void DoExecute()
		{
			LegacyLogic.Instance.CommandManager.AllowContinuousCommands = false;
			LegacyLogic.Instance.ConversationManager.OpenNpcDialog(LegacyLogic.Instance.WorldManager.NpcFactory.Get(m_npcID), m_dialogID);
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
