using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class GameOverFunction : DialogFunction
	{
		private Int32 m_dialogID;

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GAME_OVER, EventArgs.Empty);
			if (m_dialogID > 0)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
			}
			else if (m_dialogID == 0)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, p_manager.CurrentConversation.RootDialog.ID);
			}
			else
			{
				p_manager.CloseNpcContainer(null);
			}
		}
	}
}
