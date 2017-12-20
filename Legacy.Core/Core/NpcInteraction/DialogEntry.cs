using System;
using System.IO;
using Legacy.Core.NpcInteraction.Conditions;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction
{
	public class DialogEntry
	{
		private NpcConversationStaticData.DialogEntry m_staticData;

		private EDialogState m_state;

		private DialogText m_text;

		private DialogText[] m_texts;

		public DialogEntry(NpcConversationStaticData.DialogEntry p_entyData)
		{
			m_staticData = p_entyData;
			if (p_entyData.m_texts == null)
			{
				throw new InvalidDataException("The EntryData must contains at least 1 DialogText");
			}
			m_texts = new DialogText[p_entyData.m_texts.Length];
			for (Int32 i = 0; i < p_entyData.m_texts.Length; i++)
			{
				m_texts[i] = new DialogText(p_entyData.m_texts[i]);
			}
			if (m_staticData.m_functions == null || m_staticData.m_functions.Length == 0)
			{
				throw new InvalidDataException("The Entry has no DialogFunction!!! : " + m_staticData.m_texts[0].m_locaKey);
			}
		}

		public EDialogState State => m_state;

	    public DialogText Text => m_text;

	    public void ExecuteFunction(ConversationManager p_manager)
		{
			if (m_staticData.m_functions != null)
			{
				for (Int32 i = 0; i < m_staticData.m_functions.Length; i++)
				{
					m_staticData.m_functions[i].Trigger(p_manager);
				}
			}
		}

		internal void CheckEntry(Npc p_npc)
		{
			DialogCondition[] conditions = m_staticData.m_conditions;
			m_state = EDialogState.NORMAL;
			if (conditions != null)
			{
				for (Int32 i = 0; i < conditions.Length; i++)
				{
					EDialogState edialogState = conditions[i].CheckCondition(p_npc);
					if (edialogState > m_state)
					{
						m_state = edialogState;
						if (m_state == EDialogState.HIDDEN)
						{
							break;
						}
					}
				}
			}
			m_text = null;
			if (m_state != EDialogState.HIDDEN)
			{
				for (Int32 j = 0; j < m_texts.Length; j++)
				{
					m_texts[j].CheckCondition(p_npc);
					if (m_texts[j].State != EDialogState.HIDDEN)
					{
						m_text = m_texts[j];
						break;
					}
				}
				if (m_text == null)
				{
					throw new InvalidDataException("All DialogText Conditions failed!!!!");
				}
			}
		}

		public Boolean NeedGold()
		{
			if (m_staticData.m_functions != null)
			{
				for (Int32 i = 0; i < m_staticData.m_functions.Length; i++)
				{
					if (m_staticData.m_functions[i].RequireGold)
					{
						return true;
					}
				}
			}
			return false;
		}

		public Boolean NeedFood()
		{
			return false;
		}

	    public void NotifyShow(Func<String,String> localisation)
	    {
	        if (m_staticData.m_functions != null)
	        {
	            foreach (DialogFunction function in m_staticData.m_functions)
	                function.OnShow(localisation);
	        }
	    }

	    public override String ToString()
		{
			if (m_staticData.m_functions != null)
			{
				return String.Format("[DialogEntry: State={0}, Text={1}, FunctionCount={2}]", State, Text, m_staticData.m_functions.Length.ToString());
			}
			return String.Format("[DialogEntry: State={0}, Text={1}]", State, Text);
		}
	}
}
