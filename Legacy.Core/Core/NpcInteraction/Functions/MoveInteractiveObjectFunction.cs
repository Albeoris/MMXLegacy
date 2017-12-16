using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class MoveInteractiveObjectFunction : DialogFunction
	{
		private Int32 m_spawnId;

		private Int32 m_posX;

		private Int32 m_posY;

		private Int32 m_dialogID;

		[XmlAttribute("spawnID")]
		public Int32 SpawnID
		{
			get => m_spawnId;
		    set => m_spawnId = value;
		}

		[XmlAttribute("posX")]
		public Int32 PosX
		{
			get => m_posX;
		    set => m_posX = value;
		}

		[XmlAttribute("posY")]
		public Int32 PosY
		{
			get => m_posY;
		    set => m_posY = value;
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			InteractiveObject interactiveObject = LegacyLogic.Instance.WorldManager.FindObjectBySpawnerId(m_spawnId) as InteractiveObject;
			Position position = new Position(m_posX, m_posY);
			if (interactiveObject == null)
			{
				return;
			}
			interactiveObject.Position = position;
			p_manager._ChangeDialog(0, m_dialogID);
		}
	}
}
