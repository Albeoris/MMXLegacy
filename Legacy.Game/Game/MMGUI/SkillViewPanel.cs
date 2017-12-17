using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SkillViewPanel")]
	public class SkillViewPanel : MonoBehaviour
	{
		private const Int32 VIEWS_PER_PAGE = 11;

		[SerializeField]
		private UILabel m_skillPointsText;

	    private UILabel m_partySkillPointsText;

		[SerializeField]
		private SkillView[] m_skillViews;

		[SerializeField]
		private TabController m_skillTabs;

		[SerializeField]
		private SpellbookPager m_pager;

		[SerializeField]
		private UICheckbox m_checkBox;

		[SerializeField]
		private Color m_skillpointsColor = new Color(0f, 0.75f, 0f);

		[SerializeField]
		private Color m_noSkillpointsColor = Color.red;

		private Party m_party;

		private Int32 m_skillCount;

		private CharacterSkillHandler m_skillHandler;

		private Int32 m_currentViewNr;

		private Int32 m_page;

		private Int32 m_pageCount = 1;

		private String m_skillpointsColorHex;

		private String m_noSkillpointsColorHex;

		public Int32 Page => m_page;

	    public void Init(Party p_party)
		{
			m_party = p_party;
			m_skillpointsColorHex = "[" + NGUITools.EncodeColor(m_skillpointsColor) + "]";
			m_noSkillpointsColorHex = "[" + NGUITools.EncodeColor(m_noSkillpointsColor) + "]";
			UICheckbox checkBox = m_checkBox;
			checkBox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(checkBox.onStateChange, new UICheckbox.OnStateChange(OnCheckboxStateChanged));
			m_skillTabs.TabIndexChanged += OnSkillTabSelected;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_SKILL_POINTS_CHANGED, new EventHandler(OnSkillChangedEvent));
		}

		public void ChangeParty(Party p_party)
		{
			m_party = p_party;
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_SKILL_POINTS_CHANGED, new EventHandler(OnSkillChangedEvent));
		}

		public void Open()
		{
			m_skillTabs.SelectTab(0, true);
			UpdateTabs();
		}

		private void OnSkillChangedEvent(Object p_sender, EventArgs p_args)
		{
			if (m_skillHandler != null && p_sender == m_skillHandler.Character)
			{
				UpdateSkillPointsText();
				foreach (SkillView skillView in m_skillViews)
				{
					skillView.UpdateLevelUpButtons();
				}
			}
		}

		private void OnSkillTabSelected(Object p_sender, EventArgs p_args)
		{
			UpdatePage(0);
		}

		public void CharacterChanged()
		{
			UpdateTabs();
			if (!m_skillTabs.Tabs[m_skillTabs.CurrentTabIndex].IsEnabled)
			{
				m_skillTabs.SelectTab(0, true);
			}
			else
			{
				UpdatePage(0);
			}
		}

		public void UpdatePage(Int32 p_page)
		{
			m_page = p_page;
			UpdateContent();
			m_pager.UpdatePager(m_page, m_pageCount);
		}

		public void UpdateTabs()
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			m_skillTabs.Tabs[3].SetEnabled(selectedCharacter.Class.Class != EClass.BARBARIAN);
		}

		public void UpdateContent()
		{
			m_currentViewNr = 0;
			m_skillHandler = m_party.GetMember(m_party.CurrentCharacter).SkillHandler;
			m_skillCount = m_skillHandler.AvailableSkills.Count;
		    switch (m_skillTabs.CurrentTabIndex)
		    {
		        case 0:
		            FillAllSkills();
		            break;
		        case 1:
		            FillSkillForCategory(ESkillCategory.WEAPON);
		            break;
		        case 2:
		            FillSkillForCategory(ESkillCategory.MISC);
		            FillSkillForCategory(ESkillCategory.PARTY); // Temp
                    break;
		        case 3:
		            FillSkillForCategory(ESkillCategory.MAGIC);
		            break;
		        case 4:
		            FillSkillForCategory(ESkillCategory.PARTY);
		            break;
		    }
		    m_pageCount = (m_currentViewNr - 1) / 11 + 1;
			while (m_currentViewNr < (m_page + 1) * 11)
			{
				Int32 num = m_currentViewNr - m_page * 11;
				if (num >= 0)
				{
					m_skillViews[num].Hide();
				}
				m_currentViewNr++;
			}
			UpdateSkillPointsText();
		}

		private void FillAllSkills()
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			for (Int32 i = 0; i < m_skillCount; i++)
			{
				Skill skill = selectedCharacter.SkillHandler.AvailableSkills[i];
				if (!m_checkBox.isChecked || skill.Level > 0)
				{
					InitSkillView(skill);
				}
			}
		}

		private void FillSkillForCategory(ESkillCategory p_category)
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			for (Int32 i = 0; i < m_skillCount; i++)
			{
				Skill skill = selectedCharacter.SkillHandler.AvailableSkills[i];
				if ((!m_checkBox.isChecked || skill.Level > 0) && p_category == skill.Category)
				{
					InitSkillView(skill);
				}
			}
		}

		private void InitSkillView(Skill p_skill)
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			if (m_currentViewNr >= m_page * 11 && m_currentViewNr < (m_page + 1) * 11)
			{
				Int32 num = m_currentViewNr - m_page * 11;
				m_skillViews[num].SetSkill(p_skill, selectedCharacter.SkillHandler);
			}
			m_currentViewNr++;
		}

		private void UpdateSkillPointsText()
		{
			String arg = (m_skillHandler.Character.SkillPoints <= 0) ? m_noSkillpointsColorHex : m_skillpointsColorHex;
			String arg2 = arg + m_skillHandler.Character.SkillPoints + "[-]";
			m_skillPointsText.text = LocaManager.GetText("GUI_SKILLS_AVAILABLE_SKILLPOINTS", arg2);
        }

        private void OnCheckboxStateChanged(Boolean p_state)
		{
			UpdatePage(0);
		}

		private enum ESkillTabs
		{
			ALL,
			WEAPON,
			MISC,
			MAGIC,
		    PARTY
        }
	}
}
