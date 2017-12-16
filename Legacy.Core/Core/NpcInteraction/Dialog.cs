using System;
using System.Collections.Generic;
using System.IO;
using Legacy.Core.Configuration;
using Legacy.Core.Internationalization;
using Legacy.Core.NpcInteraction.Conditions;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction
{
	public class Dialog
	{
		private Int32 m_id;

		private Boolean m_randomText;

		private DialogText m_text;

		private DialogEntry[] m_entries;

		private DialogText[] m_texts;

		private List<DialogText> m_randomList;

		private NpcConversationStaticData.DialogFeature m_feature;

		private Int32 m_fakeNpcID;

		private Boolean m_hideBackButton;

		private Boolean m_hideNpcsAndCloseButton;

		private Boolean m_hideNpcAndPortrait;

		private Int32 m_backToDialogId;

		private List<Int32> m_neededTokens = new List<Int32>();

		public Dialog(NpcConversationStaticData.Dialog p_dialogData)
		{
			if (p_dialogData.m_texts == null || p_dialogData.m_texts.Length == 0)
			{
				throw new InvalidDataException("No DialogTexts defined in dialog " + p_dialogData.m_id);
			}
			m_randomList = new List<DialogText>();
			m_id = p_dialogData.m_id;
			m_randomText = p_dialogData.m_randomText;
			m_feature = p_dialogData.m_feature;
			m_fakeNpcID = p_dialogData.m_fakeNpcID;
			m_hideBackButton = p_dialogData.m_hideBackButton;
			m_hideNpcsAndCloseButton = p_dialogData.m_hideNpcsAndCloseButton;
			m_hideNpcAndPortrait = p_dialogData.m_hideNpcAndPortrait;
			m_backToDialogId = p_dialogData.m_backToDialodId;
			if (p_dialogData.m_entries != null && p_dialogData.m_entries.Length > 0)
			{
				m_entries = new DialogEntry[p_dialogData.m_entries.Length];
				for (Int32 i = 0; i < p_dialogData.m_entries.Length; i++)
				{
					SaveNeededTokens(p_dialogData.m_entries[i]);
					m_entries[i] = new DialogEntry(p_dialogData.m_entries[i]);
				}
			}
			else
			{
				m_entries = new DialogEntry[0];
			}
			m_texts = new DialogText[p_dialogData.m_texts.Length];
			for (Int32 j = 0; j < p_dialogData.m_texts.Length; j++)
			{
				SaveNeededTokens(p_dialogData.m_texts[j]);
				m_texts[j] = new DialogText(p_dialogData.m_texts[j]);
			}
		}

		public Int32 ID => m_id;

	    public Boolean RandomText => m_randomText;

	    public DialogText DialogText => m_text;

	    public String Text => GetReplacedText();

	    public DialogEntry[] Entries => m_entries;

	    public NpcConversationStaticData.DialogFeature Feature => m_feature;

	    public Int32 FakeNpcID => m_fakeNpcID;

	    public Boolean HideBackButton => m_hideBackButton;

	    public Boolean HideNpcsAndCloseButton => m_hideNpcsAndCloseButton;

	    public Boolean HideNpcAndPortrait => m_hideNpcAndPortrait;

	    public List<Int32> NeededTokens => m_neededTokens;

	    public Int32 BackToDialogId => m_backToDialogId;

	    internal void CheckDialog(Npc p_npc)
		{
			m_text = null;
			m_randomList.Clear();
			for (Int32 i = 0; i < m_texts.Length; i++)
			{
				m_texts[i].CheckCondition(p_npc);
				if (m_texts[i].State == EDialogState.NORMAL)
				{
					if (!m_randomText)
					{
						m_text = m_texts[i];
						break;
					}
					m_randomList.Add(m_texts[i]);
				}
			}
			if (m_randomText && m_randomList.Count > 0)
			{
				m_text = m_randomList[Random.Range(0, m_randomList.Count - 1)];
			}
			if (m_text == null)
			{
				throw new InvalidOperationException("condition check failed, no dialog texts found! dialog " + m_id);
			}
			for (Int32 j = 0; j < m_entries.Length; j++)
			{
				m_entries[j].CheckEntry(p_npc);
			}
		}

		private void SaveNeededTokens(NpcConversationStaticData.DialogText p_dialogStaticData)
		{
			if (p_dialogStaticData.m_conditions != null)
			{
				for (Int32 i = 0; i < p_dialogStaticData.m_conditions.Length; i++)
				{
					SaveNeededTokens(p_dialogStaticData.m_conditions[i]);
				}
			}
		}

		private void SaveNeededTokens(NpcConversationStaticData.DialogEntry p_entryStaticData)
		{
			if (p_entryStaticData.m_conditions != null)
			{
				for (Int32 i = 0; i < p_entryStaticData.m_conditions.Length; i++)
				{
					SaveNeededTokens(p_entryStaticData.m_conditions[i]);
				}
			}
			if (p_entryStaticData.m_texts != null)
			{
				for (Int32 j = 0; j < p_entryStaticData.m_texts.Length; j++)
				{
					SaveNeededTokens(p_entryStaticData.m_texts[j]);
				}
			}
		}

		private void SaveNeededTokens(DialogCondition p_condition)
		{
			if (p_condition is TokenAcquiredCondition)
			{
				m_neededTokens.Add(((TokenAcquiredCondition)p_condition).TokenID);
			}
		}

		private String GetReplacedText()
		{
			if (m_text.Replacement == EDialogReplacement.ALL_CHARS)
			{
				return m_text.ReplaceWithAllChars();
			}
			if (m_text.Replacement == EDialogReplacement.CLASS_MERCENARY || m_text.Replacement == EDialogReplacement.CLASS_CRUSADER || m_text.Replacement == EDialogReplacement.CLASS_FREEMAGE || m_text.Replacement == EDialogReplacement.CLASS_BLADEDANCER || m_text.Replacement == EDialogReplacement.CLASS_RANGER || m_text.Replacement == EDialogReplacement.CLASS_DRUID || m_text.Replacement == EDialogReplacement.CLASS_DEFENDER || m_text.Replacement == EDialogReplacement.CLASS_SCOUT || m_text.Replacement == EDialogReplacement.CLASS_RUNEPRIEST || m_text.Replacement == EDialogReplacement.CLASS_BARBARIAN || m_text.Replacement == EDialogReplacement.CLASS_HUNTER || m_text.Replacement == EDialogReplacement.CLASS_SHAMAN)
			{
				return m_text.ReplaceWithClass();
			}
			if (m_text.Replacement == EDialogReplacement.RACE_HUMAN || m_text.Replacement == EDialogReplacement.RACE_DWARF || m_text.Replacement == EDialogReplacement.RACE_ELF || m_text.Replacement == EDialogReplacement.RACE_ORC)
			{
				return m_text.ReplaceWithRace();
			}
			if (m_text.Replacement == EDialogReplacement.FEATURE_PRICE)
			{
				return m_text.ReplacementWithFeature(Feature);
			}
			if (m_text.Replacement == EDialogReplacement.SKILL_EXPERT_PRICE)
			{
				return m_text.ReplacementWithPrice(ConfigManager.Instance.Game.SkillExpertPrice);
			}
			if (m_text.Replacement == EDialogReplacement.SKILL_MASTER_PRICE)
			{
				return m_text.ReplacementWithPrice(ConfigManager.Instance.Game.SkillMasterPrice);
			}
			if (m_text.Replacement == EDialogReplacement.SKILL_GRANDMASTER_PRICE)
			{
				return m_text.ReplacementWithPrice(ConfigManager.Instance.Game.SkillGrandmasterPrice);
			}
			if (m_text.Replacement == EDialogReplacement.HIRELING_ABSOLUTE)
			{
				return m_text.ReplacementWithHirelingAbsolute(Feature.m_npcID);
			}
			if (m_text.Replacement == EDialogReplacement.HIRELING_ABSOLUTE_PERCENT)
			{
				return m_text.ReplacementWithHirelingAbsolutePercent(Feature.m_npcID);
			}
			if (m_text.Replacement == EDialogReplacement.LINEBREAKS_TO_SPACE)
			{
				return m_text.ReplaceLinebreaksWithSpace();
			}
			return Localization.Instance.GetText(m_text.LocaKey);
		}
	}
}
