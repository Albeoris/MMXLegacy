using System;
using System.Xml.Serialization;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class UserInputFunction : DialogFunction
	{
		private Int32 m_successDialogID;

		private Int32 m_failDialogID;

		private String[] m_inputSolutions;

		private String m_userInput = String.Empty;

		public UserInputFunction()
		{
		}

		public UserInputFunction(Int32 p_successDialogId, Int32 p_failDialogId, String[] p_solutions, String p_userInput)
		{
			m_successDialogID = p_successDialogId;
			m_failDialogID = p_failDialogId;
			m_inputSolutions = p_solutions;
			m_userInput = p_userInput;
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

		[XmlAttribute("userInputSolutions")]
		public String[] InputSolutions
		{
			get => m_inputSolutions;
		    set => m_inputSolutions = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
		}

		public void EvaluateUserInput()
		{
			Int32 p_dialogID = m_failDialogID;
			if (!String.IsNullOrEmpty(m_userInput))
			{
				String text = m_userInput.ToUpper();
				foreach (String value in m_inputSolutions)
				{
					if (text.Equals(value))
					{
						p_dialogID = m_successDialogID;
						break;
					}
				}
			}
			LegacyLogic.Instance.ConversationManager._ChangeDialog(LegacyLogic.Instance.ConversationManager.CurrentNpc.StaticID, p_dialogID);
		}
	}
}
