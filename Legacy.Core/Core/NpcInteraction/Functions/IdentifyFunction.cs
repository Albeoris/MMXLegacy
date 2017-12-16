using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Configuration;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class IdentifyFunction : DialogFunction
	{
		private ConversationManager m_manager;

		private Int32 m_dialogID;

		private Int32 m_price;

		public IdentifyFunction(Int32 p_dialogId, Int32 p_price)
		{
			m_dialogID = p_dialogId;
			m_price = p_price;
		}

		public IdentifyFunction()
		{
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("price")]
		public Int32 Price
		{
			get => m_price;
		    set => m_price = value;
		}

		public override Boolean RequireGold => true;

	    public override void Trigger(ConversationManager p_manager)
		{
			Int32 num = m_price;
			if (num == -1)
			{
				num = ConfigManager.Instance.Game.CostIdentify;
			}
			if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
			{
				num = (Int32)Math.Ceiling(num * ConfigManager.Instance.Game.NpcItemIdentifyFactor);
			}
			m_manager = p_manager;
			m_manager.CurrentNpc.IdentifyController.IdentifyPrice = num;
			m_manager.CurrentNpc.IdentifyController.Init(m_manager.CurrentNpc);
			m_manager.CurrentNpc.IdentifyController.UpdateDialog += OnEndIdentify;
			m_manager.CurrentNpc.IdentifyController.StartIdentify();
		}

		private void OnEndIdentify(Object p_sender, EventArgs p_args)
		{
			m_manager.CurrentNpc.IdentifyController.UpdateDialog -= OnEndIdentify;
			m_manager._ChangeDialog(m_manager.CurrentNpc.StaticID, m_dialogID);
		}
	}
}
