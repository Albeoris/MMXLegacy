using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class RestFunction : DialogFunction
	{
		private Int32 m_dialogID;

		private Int32 m_price;

		private Boolean m_wellRested;

		public RestFunction()
		{
		}

		public RestFunction(Boolean p_wellRested, Int32 p_dialogID, Int32 p_price)
		{
			m_wellRested = p_wellRested;
			m_dialogID = p_dialogID;
			m_price = p_price;
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

		[XmlAttribute("wellrested")]
		public Boolean WellRested
		{
			get => m_wellRested;
		    set => m_wellRested = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.IsNpcRest = true;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_RESTING, EventArgs.Empty);
			if (m_price != 0)
			{
				party.ChangeGold(-m_price);
			}
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

		private void OnPartyRested(Object sender, EventArgs e)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
			if (m_wellRested)
			{
				party.Buffs.AddBuff(EPartyBuffs.WELL_RESTED, 1f);
			}
		}
	}
}
