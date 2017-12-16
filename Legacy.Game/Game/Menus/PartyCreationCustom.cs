using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI;
using Legacy.Game.MMGUI.PartyCreate;
using Legacy.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/PartyCreationCustom")]
	public class PartyCreationCustom : MonoBehaviour
	{
		[SerializeField]
		private TabController m_tabController;

		[SerializeField]
		private PartyCreationCharacter[] m_characters;

		[SerializeField]
		private PartyCreationRaces m_racesTab;

		[SerializeField]
		private PartyCreationClasses m_classesTab;

		[SerializeField]
		private PartyCreationCustomize m_customizeTab;

		[SerializeField]
		private PartyCreationSkills m_skillsTab;

		[SerializeField]
		private PartyCreationAttributes m_attributesTab;

		[SerializeField]
		private GUIMultiSpriteButton m_backBtn;

		[SerializeField]
		private GUIMultiSpriteButton m_nextBtn;

		[SerializeField]
		private GUIMultiSpriteButton m_finishBtn;

		private PartyCreator m_partyCreator;

		private Int32 m_currentTab;

		private void Awake()
		{
			m_partyCreator = LegacyLogic.Instance.WorldManager.PartyCreator;
		}

		public void Init()
		{
			m_partyCreator.SelectCharacter(0);
			InitParty(m_partyCreator);
			m_racesTab.Init(m_partyCreator);
			m_classesTab.Init(m_partyCreator);
			m_customizeTab.Init(m_partyCreator);
			m_skillsTab.Init(m_partyCreator);
			m_attributesTab.Init(m_partyCreator);
			m_currentTab = m_partyCreator.GetDummyCharacter(0).LastConfirmedStep;
			if (m_currentTab < 0)
			{
				m_currentTab = 0;
			}
			if (m_currentTab > 4)
			{
				m_currentTab = 4;
			}
			m_tabController.TabIndexChanged += UpdateTabs;
			m_tabController.SelectTab(m_currentTab, true);
			UpdateTabsForCharacter();
			CheckButtons(null, EventArgs.Empty);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DUMMY_CHARACTER_POINTS_CHANGED, new EventHandler(CheckButtons));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DUMMY_CHARACTER_STATUS_CHANGED, new EventHandler(OnRaceClassChanged));
		}

		private void InitParty(PartyCreator p_partyCreator)
		{
			if (m_characters != null)
			{
				for (Int32 i = 0; i < m_characters.Length; i++)
				{
					DummyCharacter dummyCharacter = p_partyCreator.GetDummyCharacter(i);
					if (dummyCharacter != null)
					{
						m_characters[i].Init(dummyCharacter, i);
						m_characters[i].OnCharacterClicked += OnCharacterClicked;
						m_characters[i].SetSelected(dummyCharacter == p_partyCreator.GetSelectedDummyCharacter());
						m_characters[i].SetTickState(m_partyCreator.CheckSkillsFinished(i) && m_partyCreator.CheckAttributesFinished(i));
					}
				}
			}
		}

		private void OnCharacterClicked(Object p_sender, EventArgs p_args)
		{
			PartyCreationCharacter partyCreationCharacter = p_sender as PartyCreationCharacter;
			if (partyCreationCharacter != null)
			{
				m_partyCreator.SelectCharacter(partyCreationCharacter.Index);
				m_currentTab = Math.Min(m_currentTab, m_partyCreator.GetSelectedDummyCharacter().GetStepToShow());
				UpdateTabsForCharacter();
				m_tabController.SelectTab(m_currentTab, true);
				AudioController.Play("PortraitSelect");
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(m_partyCreator.GetSelectedDummyCharacter(), EEventType.DUMMY_CHARACTER_SELECTED, EventArgs.Empty);
		}

		public void Cleanup()
		{
			if (m_characters != null)
			{
				for (Int32 i = 0; i < m_characters.Length; i++)
				{
					m_characters[i].OnCharacterClicked -= OnCharacterClicked;
					m_characters[i].Cleanup();
				}
			}
			m_racesTab.Cleanup();
			m_classesTab.Cleanup();
			m_customizeTab.Cleanup();
			m_skillsTab.Cleanup();
			m_attributesTab.Cleanup();
			m_tabController.TabIndexChanged -= UpdateTabs;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DUMMY_CHARACTER_POINTS_CHANGED, new EventHandler(CheckButtons));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DUMMY_CHARACTER_STATUS_CHANGED, new EventHandler(OnRaceClassChanged));
		}

		public void PreviousTab()
		{
			if (m_tabController.CurrentTabIndex == 0)
			{
				m_racesTab.UndoSelection();
				m_partyCreator.GetDummyCharacter(m_partyCreator.m_selectedCharacter).Race = ERace.NONE;
			}
			else if (m_tabController.CurrentTabIndex == 1)
			{
				m_classesTab.UndoSelection();
				m_partyCreator.GetDummyCharacter(m_partyCreator.m_selectedCharacter).Class = EClass.NONE;
			}
			else if (m_tabController.CurrentTabIndex == 2)
			{
				m_customizeTab.UndoSelection();
			}
			else if (m_tabController.CurrentTabIndex == 3)
			{
				m_skillsTab.UndoSelection();
			}
			else if (m_tabController.CurrentTabIndex == 4)
			{
				m_attributesTab.UndoSelection();
			}
			m_currentTab--;
			m_partyCreator.GetSelectedDummyCharacter().LastConfirmedStep = m_currentTab - 1;
			if (m_currentTab < 0)
			{
				m_partyCreator.SelectPreviousCharacter();
				m_currentTab = m_partyCreator.GetSelectedDummyCharacter().GetStepToShow();
			}
			UpdateTabsForCharacter();
			m_tabController.SelectTab(m_currentTab, true);
			CheckButtons(null, EventArgs.Empty);
		}

		public void NextTab()
		{
			m_partyCreator.GetSelectedDummyCharacter().LastConfirmedStep = m_currentTab;
			m_currentTab++;
			if (m_currentTab > 4)
			{
				m_partyCreator.SelectNextCharacter();
				m_currentTab = m_partyCreator.GetSelectedDummyCharacter().GetStepToShow();
			}
			UpdateTabsForCharacter();
			m_tabController.SelectTab(m_currentTab, true);
			CheckButtons(null, EventArgs.Empty);
		}

		private void OnRaceClassChanged(Object p_sender, EventArgs p_args)
		{
			DummyCharacter dummyCharacter = m_partyCreator.GetDummyCharacter(m_partyCreator.m_selectedCharacter);
			if (m_currentTab == 0)
			{
				if (dummyCharacter.Gender != EGender.NOT_SELECTED)
				{
					dummyCharacter.Gender = EGender.NOT_SELECTED;
				}
				if (dummyCharacter.Class != EClass.NONE)
				{
					dummyCharacter.Class = EClass.NONE;
				}
				m_characters[m_partyCreator.m_selectedCharacter].ResetPortrait();
			}
			UpdateTabsForCharacter();
		}

		private void UpdateTabsForCharacter()
		{
			Int32 stepToShow = m_partyCreator.GetSelectedDummyCharacter().GetStepToShow();
			for (Int32 i = 0; i < m_tabController.Tabs.Length; i++)
			{
				m_tabController.Tabs[i].SetEnabled(i <= stepToShow);
			}
		}

		private void UpdateTabs(Object p_sender, EventArgs p_args)
		{
			m_currentTab = m_tabController.CurrentTabIndex;
			if (m_tabController.CurrentTabIndex == 0)
			{
				m_racesTab.OnAfterActivate();
			}
			else if (m_tabController.CurrentTabIndex == 1)
			{
				m_classesTab.OnAfterActivate();
			}
			else if (m_tabController.CurrentTabIndex == 2)
			{
				m_customizeTab.OnAfterActivate();
			}
			else if (m_tabController.CurrentTabIndex == 3)
			{
				m_skillsTab.OnAfterActivate();
			}
			else if (m_tabController.CurrentTabIndex == 4)
			{
				m_attributesTab.OnAfterActivate();
			}
			if (m_tabController.CurrentTabIndex == 0)
			{
				m_backBtn.SetLabelText(LocaManager.GetText("GUI_PREVIOUS_CHARACTER"));
			}
			else
			{
				m_backBtn.SetLabelText(LocaManager.GetText("Gui/Options/Back"));
			}
			if (m_tabController.CurrentTabIndex == 4)
			{
				m_nextBtn.SetLabelText(LocaManager.GetText("GUI_NEXT_CHARACTER"));
			}
			else
			{
				m_nextBtn.SetLabelText(LocaManager.GetText("GUI_CONTINUE"));
			}
		}

		public void CheckButtons(Object p_sender, EventArgs p_args)
		{
			if (m_tabController.CurrentTabIndex == 3)
			{
				m_nextBtn.IsEnabled = m_partyCreator.CheckSkillsFinished();
			}
			else if (m_tabController.CurrentTabIndex == 4)
			{
				m_nextBtn.IsEnabled = m_partyCreator.CheckAttributesFinished();
			}
			else
			{
				m_nextBtn.IsEnabled = true;
			}
			if (m_tabController.CurrentTabIndex == 4 && m_partyCreator.GetSelectedCharacterIndex() == 3)
			{
				NGUITools.SetActive(m_nextBtn.gameObject, false);
			}
			else
			{
				NGUITools.SetActive(m_nextBtn.gameObject, true);
			}
			if (m_tabController.CurrentTabIndex == 0 && m_partyCreator.GetSelectedCharacterIndex() == 0)
			{
				NGUITools.SetActive(m_backBtn.gameObject, false);
			}
			else
			{
				NGUITools.SetActive(m_backBtn.gameObject, true);
			}
			m_characters[m_partyCreator.GetSelectedCharacterIndex()].SetTickState(m_partyCreator.CheckSkillsFinished() && m_partyCreator.CheckAttributesFinished());
			NGUITools.SetActive(m_finishBtn.gameObject, m_partyCreator.CheckCustomFinished());
			m_finishBtn.IsEnabled = m_partyCreator.CheckCustomFinished();
		}
	}
}
