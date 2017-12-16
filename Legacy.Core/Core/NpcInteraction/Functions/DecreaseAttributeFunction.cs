using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class DecreaseAttributeFunction : DialogFunction
	{
		private EPotionTarget m_attribute;

		private Int32 m_dialogID;

		[XmlAttribute("attribute")]
		public EPotionTarget Attribute
		{
			get => m_attribute;
		    set => m_attribute = value;
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = party.GetMember(i);
				Attributes baseAttributes = member.BaseAttributes;
				if (m_attribute == EPotionTarget.MIGHT)
				{
					baseAttributes.Might--;
				}
				else if (m_attribute == EPotionTarget.MAGIC)
				{
					baseAttributes.Magic--;
				}
				else if (m_attribute == EPotionTarget.PERCEPTION)
				{
					baseAttributes.Perception--;
				}
				else if (m_attribute == EPotionTarget.DESTINY)
				{
					baseAttributes.Destiny--;
				}
				else if (m_attribute == EPotionTarget.VITALITY)
				{
					baseAttributes.Vitality--;
				}
				else if (m_attribute == EPotionTarget.SPIRIT)
				{
					baseAttributes.Spirit--;
				}
				member.BaseAttributes = baseAttributes;
				member.CalculateCurrentAttributes();
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
	}
}
