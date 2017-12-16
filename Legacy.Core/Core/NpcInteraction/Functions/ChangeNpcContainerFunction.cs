using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class ChangeNpcContainerFunction : DialogFunction
	{
		private Int32 m_containerId;

		private Int32 m_npcID;

		private Int32 m_dialogID;

		[XmlAttribute("containerID")]
		public Int32 ContainerID
		{
			get => m_containerId;
		    set => m_containerId = value;
		}

		[XmlAttribute("npcID")]
		public Int32 NpcID
		{
			get => m_npcID;
		    set => m_npcID = value;
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			NpcContainer npcContainer = grid.FindInteractiveObject(m_containerId) as NpcContainer;
			if (npcContainer == null)
			{
				throw new InvalidOperationException("There's no NpcContainer with ID " + m_containerId + " on this map!");
			}
			if (npcContainer.Npcs.Count == 0)
			{
				throw new InvalidOperationException("There are no NPCs in container " + m_containerId + "!");
			}
			Int32 num = -1;
			if (m_npcID == 0)
			{
				num = npcContainer.Npcs[0].StaticData.StaticID;
			}
			else
			{
				foreach (Npc npc in npcContainer.Npcs)
				{
					if (npc.StaticData.StaticID == m_npcID)
					{
						num = npc.StaticData.StaticID;
						break;
					}
				}
			}
			if (num == -1)
			{
				throw new InvalidOperationException(String.Concat(new Object[]
				{
					"The NPC with ID ",
					m_npcID,
					" could not be found in container ",
					m_containerId,
					"!"
				}));
			}
			p_manager.ChangeNpcContainer(npcContainer, num, m_dialogID);
		}
	}
}
