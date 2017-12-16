using System;
using System.Xml.Serialization;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class SelectCharacterFunction : DialogFunction
	{
		private Int32 m_dialogID;

		private Int32 m_characterIndex;

		private DialogFunction m_function;

		public SelectCharacterFunction(Int32 p_characterIndex, Int32 p_dialogID)
		{
			m_characterIndex = p_characterIndex;
			m_dialogID = p_dialogID;
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("characterIndex")]
		public Int32 CharacterIndex
		{
			get => m_characterIndex;
		    set => m_characterIndex = value;
		}

		public DialogFunction Function
		{
			get => m_function;
		    set => m_function = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			LegacyLogic.Instance.WorldManager.Party.SelectCharacter(m_characterIndex);
			if (m_function != null)
			{
				m_function.Trigger(p_manager);
			}
			else
			{
				p_manager._ChangeDialog(LegacyLogic.Instance.ConversationManager.CurrentNpc.StaticID, m_dialogID);
			}
		}
	}
}
