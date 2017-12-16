using System;
using Legacy.Core.Quests;
using Legacy.Game.HUD;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/Journal")]
	public class Journal : MonoBehaviour
	{
		[SerializeField]
		private TabController m_tabController;

		[SerializeField]
		private QuestSelectPanel m_questSelectPanel;

		[SerializeField]
		private LoreBookController m_loreBookController;

		[SerializeField]
		private BestiaryController m_bestiaryController;

		[SerializeField]
		private PageableListController m_pageableList;

		private Boolean m_isOpen = true;

		public Boolean IsOpen
		{
			get => m_isOpen;
		    set => m_isOpen = value;
		}

		public void Init(IngameController p_ingameController)
		{
			m_tabController.TabIndexChanged += OnTabSelectionChanged;
			m_questSelectPanel.Init();
			m_loreBookController.Init();
			m_bestiaryController.Init();
			m_pageableList.Init();
		}

		public void Cleanup()
		{
			m_tabController.TabIndexChanged -= OnTabSelectionChanged;
			m_questSelectPanel.Cleanup();
			m_loreBookController.Cleanup();
			m_bestiaryController.Cleanup();
			m_pageableList.Cleanup();
		}

		public void ToggleQuests()
		{
			if (IsOpen)
			{
				if (m_tabController.CurrentTabIndex == 0)
				{
					Close();
				}
				else
				{
					m_tabController.SelectTab(0, false);
				}
			}
			else
			{
				Open();
				m_tabController.SelectTab(0, true);
			}
		}

		public void ToggleBestiary()
		{
			if (IsOpen)
			{
				if (m_tabController.CurrentTabIndex == 1)
				{
					Close();
				}
				else
				{
					m_tabController.SelectTab(1, false);
				}
			}
			else
			{
				Open();
				m_tabController.SelectTab(1, true);
			}
		}

		public void ToggleLore()
		{
			if (IsOpen)
			{
				if (m_tabController.CurrentTabIndex == 2)
				{
					Close();
				}
				else
				{
					m_tabController.SelectTab(2, false);
				}
			}
			else
			{
				Open();
				m_tabController.SelectTab(2, true);
			}
		}

		private void Open()
		{
			IsOpen = !IsOpen;
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnInteractHotkey));
			IngameController.Instance.UpdateLogs();
			NGUITools.SetActive(gameObject, IsOpen);
			AudioController.Play("ButtonClickBookOpen");
		}

		public void Close()
		{
			IsOpen = !IsOpen;
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnInteractHotkey));
			IngameController.Instance.UpdateLogs();
			NGUITools.SetActive(gameObject, IsOpen);
			AudioController.Play("ButtonClickBookClose");
		}

		private void OnTabSelectionChanged(Object sender, EventArgs e)
		{
			NGUITools.SetActive(m_pageableList.gameObject, true);
			m_bestiaryController.Close();
			m_loreBookController.Close();
			m_questSelectPanel.Close();
			if (m_tabController.CurrentTabIndex == 0)
			{
				m_questSelectPanel.Open();
			}
			else if (m_tabController.CurrentTabIndex == 1)
			{
				m_bestiaryController.Open();
			}
			else
			{
				m_loreBookController.Open();
			}
		}

		public void GotoQuest(QuestStep p_step)
		{
			m_questSelectPanel.GoToQuest(p_step);
		}

		public void GotoBook(HUDSideInfoBook p_bookEntry)
		{
			m_loreBookController.OnBookSelected(p_bookEntry.BookId);
		}

		public void GotoTokens()
		{
			m_questSelectPanel.GoToTokens();
		}

		private void OnInteractHotkey(Object sender, HotkeyEventArgs e)
		{
			OnCloseJournal();
		}

		public void OnCloseJournal()
		{
			m_bestiaryController.Close();
			m_loreBookController.Close();
			m_questSelectPanel.Close();
			m_bestiaryController.ResetFakeScene();
			Close();
		}

		private enum EJournalTabs
		{
			QUESTS,
			BESTIARY,
			LOREBOOK
		}
	}
}
