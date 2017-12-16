using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class StartDialogueInteraction : BaseInteraction
	{
		private NpcContainer m_npcs;

		protected InteractiveObject m_parent;

		public StartDialogueInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			InteractiveObject interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_npcs = (interactiveObject as NpcContainer);
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			if (m_npcs == null)
			{
				throw new InvalidOperationException("Tried to start a conversation with something that is not an NPC!");
			}
			Boolean flag = false;
			if (m_npcs.HasScene)
			{
				foreach (Npc npc in m_npcs.Npcs)
				{
					if (npc.IsEnabled)
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				LegacyLogic.Instance.CommandManager.AllowContinuousCommands = false;
			}
			m_npcs.StartConversation();
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

		public override void PrewarmAfterCreate()
		{
			base.PrewarmAfterCreate();
			for (Int32 i = 0; i < m_npcs.Npcs.Count; i++)
			{
				List<Int32> neededTokens = m_npcs.Npcs[i].ConversationData.NeededTokens;
				for (Int32 j = 0; j < neededTokens.Count; j++)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ANNOUNCE_NEEDED_TOKEN, new TokenEventArgs(neededTokens[j]));
				}
			}
		}

		protected override void ParseExtra(String p_extra)
		{
		}
	}
}
