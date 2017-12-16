using System;
using System.Xml.Serialization;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class GoToFunction : DialogFunction
	{
		private Int32 m_npcID;

		private Int32 m_dialogID;

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
			p_manager._ChangeDialog(m_npcID, m_dialogID);
		}
	}
}
