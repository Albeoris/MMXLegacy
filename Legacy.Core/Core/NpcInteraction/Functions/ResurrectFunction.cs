using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class ResurrectFunction : DialogFunction
	{
		private Int32 m_dialogID;

		private Int32 m_price;

		private Character m_char;

		public ResurrectFunction(Character p_char, Int32 p_price, Int32 p_dialogID)
		{
			m_price = p_price;
			m_char = p_char;
			m_dialogID = p_dialogID;
		}

		public ResurrectFunction()
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

		public Character Chara => m_char;

        public override void OnShow(Func<string, string> localisation)
        {
            RaiseEventShow(localisation);
        }

	    public static void RaiseEventShow(Func<String, String> localisation)
	    {
	        LegacyLogic.Instance.EventManager.Get<InitServiceDialogArgs>().TryInvoke(() =>
	        {
	            String caption = localisation("DIALOG_OPTION_SERVICES") + ":"; // Services:
	            String title = localisation("DIALOG_OPTION_RESURRECT"); // Resurrection

	            return new InitServiceDialogArgs(caption, title);
	        });
	    }

	    public override void Trigger(ConversationManager p_manager)
		{
			if (m_char.ConditionHandler.HasCondition(ECondition.DEAD))
			{
				m_char.ConditionHandler.RemoveCondition(ECondition.DEAD);
				m_char.Resurrect();
				LegacyLogic.Instance.WorldManager.Party.ChangeGold(-m_price);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_REVIVED, EventArgs.Empty);
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
			}
		}
	}
}
