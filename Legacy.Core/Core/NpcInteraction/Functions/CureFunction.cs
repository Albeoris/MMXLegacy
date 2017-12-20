using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class CureFunction : DialogFunction
	{
		private Int32 m_dialogID;

		private Int32 m_price;

		private Character m_char;

		private ECondition[] m_curableConditions;

		public CureFunction(Character p_char, ECondition[] p_curableConditions, Int32 p_price, Int32 p_dialogID)
		{
			m_price = p_price;
			m_char = p_char;
			m_curableConditions = p_curableConditions;
			m_dialogID = p_dialogID;
		}

		public CureFunction()
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

		[XmlAttribute("conditions")]
		public ECondition[] CurableConditions
		{
			get => m_curableConditions;
		    set => m_curableConditions = value;
		}

        public override void OnShow(Func<string, string> localisation)
        {
            RaiseEventShow(localisation);
        }

	    public static void RaiseEventShow(Func<String, String> localisation)
	    {
	        LegacyLogic.Instance.EventManager.Get<InitServiceDialogArgs>().TryInvoke(() =>
	        {
	            String caption = localisation("DIALOG_OPTION_SERVICES") + ":"; // Services:
	            String title = localisation("DIALOG_OPTION_CURE"); // Cure

	            return new InitServiceDialogArgs(caption, title);
	        });
	    }

	    public override void Trigger(ConversationManager p_manager)
		{
			if (!m_char.ConditionHandler.HasCondition(ECondition.DEAD) && m_char.ConditionHandler.GetVisibleCondition() != ECondition.NONE)
			{
				m_char.ResetRestTime();
				Int32 num = 0;
				foreach (ECondition p_condition in m_curableConditions)
				{
					if (m_char.ConditionHandler.HasCondition(p_condition))
					{
						m_char.ConditionHandler.RemoveCondition(p_condition);
						num += m_price;
					}
				}
				LegacyLogic.Instance.WorldManager.Party.ChangeGold(-num);
			}
			p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
		}
	}
}
