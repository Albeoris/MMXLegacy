using System;
using System.Xml.Serialization;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class WhoWillFunction : DialogFunction
	{
		private EPotionTarget m_attribute;

		private Int32 m_minValue;

		private Int32 m_successDialogID;

		private Int32 m_failDialogID;

		private Character m_char;

		public WhoWillFunction(Character p_char, EPotionTarget p_attribute, Int32 p_minValue, Int32 p_successDialogId, Int32 p_failDialogId)
		{
			m_char = p_char;
			m_attribute = p_attribute;
			m_minValue = p_minValue;
			m_successDialogID = p_successDialogId;
			m_failDialogID = p_failDialogId;
		}

		public WhoWillFunction()
		{
		}

		[XmlAttribute("characterAttribute")]
		public EPotionTarget Attribute
		{
			get => m_attribute;
		    set => m_attribute = value;
		}

		[XmlAttribute("minimumValue")]
		public Int32 MinimumValue
		{
			get => m_minValue;
		    set => m_minValue = value;
		}

		[XmlAttribute("successDialogID")]
		public Int32 SuccessDialogID
		{
			get => m_successDialogID;
		    set => m_successDialogID = value;
		}

		[XmlAttribute("failDialogID")]
		public Int32 FailDialogID
		{
			get => m_failDialogID;
		    set => m_failDialogID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			Boolean flag = false;
			switch (m_attribute)
			{
			case EPotionTarget.PERCEPTION:
				flag = (m_minValue <= m_char.CurrentAttributes.Perception);
				break;
			case EPotionTarget.DESTINY:
				flag = (m_minValue <= m_char.CurrentAttributes.Destiny);
				break;
			case EPotionTarget.VITALITY:
				flag = (m_minValue <= m_char.CurrentAttributes.Vitality);
				break;
			case EPotionTarget.SPIRIT:
				flag = (m_minValue <= m_char.CurrentAttributes.Spirit);
				break;
			}
			if (flag)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_successDialogID);
			}
			else
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_failDialogID);
			}
		}
	}
}
