using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Quests;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/QuestSelectPanel")]
	public class QuestSelectPanel : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_nameLabel;

		[SerializeField]
		private GameObject m_objectives;

		[SerializeField]
		private GameObject m_objectivePrefab;

		[SerializeField]
		private GameObject m_detailsPanel;

		[SerializeField]
		private UILabel m_descriptionLabel;

		[SerializeField]
		private UILabel m_flavorLabel;

		[SerializeField]
		private UICheckbox m_showFinished;

		[SerializeField]
		private PageableListController m_pageableList;

		[SerializeField]
		private UISprite m_frameBackground;

		[SerializeField]
		private UISprite m_frameBorder;

		[SerializeField]
		private TabController m_questTabController;

		[SerializeField]
		private UILabel m_headerLabel;

		[SerializeField]
		private QuestTokenPanel m_tokenPanel;

		private List<JournalQuestObjective> m_objectiveList;

		private EQuestType m_questType;

		private QuestStep m_selectedQuestStep;

		public void Init()
		{
			if (m_objectiveList != null)
			{
				foreach (JournalQuestObjective journalQuestObjective in m_objectiveList)
				{
					Destroy(journalQuestObjective.gameObject);
				}
				m_objectiveList.Clear();
			}
			else
			{
				m_objectiveList = new List<JournalQuestObjective>();
			}
			m_tokenPanel.Init();
		}

		public void Cleanup()
		{
			CleanupPageableList();
		}

		private void UpdateList(Object sender, EventArgs e)
		{
			UpdateQuestList(m_questType);
			m_pageableList.Show();
		}

		public void UpdateEntries(EQuestType p_type)
		{
			m_pageableList.CurrentIndex = 0;
			Int32 bookCount = UpdateQuestList(p_type);
			m_pageableList.Finish(bookCount);
		}

		private Int32 UpdateQuestList(EQuestType p_type)
		{
			CleanupPageableList();
			m_questType = p_type;
			Boolean flag = false;
			List<QuestStep> stepsByState;
			if (m_showFinished.isChecked)
			{
				stepsByState = LegacyLogic.Instance.WorldManager.QuestHandler.GetStepsByState(EQuestState.ACTIVE);
				stepsByState.AddRange(LegacyLogic.Instance.WorldManager.QuestHandler.GetStepsByState(EQuestState.SOLVED));
			}
			else
			{
				stepsByState = LegacyLogic.Instance.WorldManager.QuestHandler.GetStepsByState(EQuestState.ACTIVE);
			}
			List<QuestStep> list = new List<QuestStep>();
			foreach (QuestStep questStep in stepsByState)
			{
				Boolean flag2 = false;
				if (p_type == EQuestType.ALL)
				{
					flag2 = true;
				}
				else if (p_type == EQuestType.QUEST_TYPE_MAIN && questStep.StaticData.Type == p_type)
				{
					flag2 = true;
				}
				else if (p_type == EQuestType.QUEST_TYPE_ONGOING && questStep.StaticData.Type == p_type)
				{
					flag2 = true;
				}
				else if (p_type == EQuestType.QUEST_TYPE_SIDE && (questStep.StaticData.Type == EQuestType.QUEST_TYPE_GRANDMASTER || questStep.StaticData.Type == EQuestType.QUEST_TYPE_PROMOTION || questStep.StaticData.Type == EQuestType.QUEST_TYPE_REPEATABLE || questStep.StaticData.Type == EQuestType.QUEST_TYPE_SIDE))
				{
					flag2 = true;
				}
				if (flag2)
				{
					if (m_selectedQuestStep != null && m_selectedQuestStep == questStep)
					{
						flag = true;
					}
					list.Add(questStep);
				}
			}
			for (Int32 i = m_pageableList.CurrentIndex; i < m_pageableList.EndIndex; i++)
			{
				if (i < list.Count)
				{
					QuestStep questStep2 = list[i];
					BookEntry entry = m_pageableList.GetEntry();
					entry.Init(questStep2.StaticData.StaticID, questStep2.StaticData.Name);
					QuestEntry questEntry = entry;
					QuestEntry questEntry2 = questEntry;
					questEntry2.OnQuestClicked = (EventHandler)Delegate.Combine(questEntry2.OnQuestClicked, new EventHandler(QuestClick));
					questEntry.RestorePositions();
					questEntry.SetQuestStep(questStep2);
				}
				else
				{
					BookEntry entry2 = m_pageableList.GetEntry();
					entry2.Init(0, String.Empty);
				}
			}
			if (list.Count > 0)
			{
				if (flag)
				{
					QuestClick(m_selectedQuestStep, EventArgs.Empty);
				}
				else
				{
					m_selectedQuestStep = null;
					QuestClick(m_pageableList.EntryList[0].GetQuestStep(), EventArgs.Empty);
				}
			}
			else
			{
				ClearDetails();
			}
			return list.Count;
		}

		public void QuestClick(Object sender, EventArgs p_args)
		{
			m_selectedQuestStep = (QuestStep)sender;
			SetSelection(m_selectedQuestStep);
			ClearDetails();
			m_nameLabel.text = LocaManager.GetText(m_selectedQuestStep.StaticData.Name);
			NGUITools.SetActive(m_frameBorder.gameObject, true);
			NGUITools.SetActive(m_frameBackground.gameObject, true);
			Single num = 0f;
			foreach (QuestObjective p_objective in m_selectedQuestStep.GetNonInactiveObjectives())
			{
				GameObject gameObject = NGUITools.AddChild(m_objectives.gameObject, m_objectivePrefab);
				JournalQuestObjective component = gameObject.GetComponent<JournalQuestObjective>();
				component.SetObjective(m_selectedQuestStep, p_objective);
				Vector3 localPosition = component.transform.localPosition;
				localPosition.x = 0f;
				localPosition.y = num;
				num -= component.GetHeight();
				component.transform.localPosition = localPosition;
				Destroy(component.GetComponentInChildren<UIButtonMessage>());
				m_objectiveList.Add(component);
			}
			Vector3 localPosition2 = m_descriptionLabel.transform.localPosition;
			localPosition2.y = m_objectives.transform.localPosition.y + num - 40f;
			m_descriptionLabel.transform.localPosition = localPosition2;
			m_descriptionLabel.text = LocaManager.GetText("GUI_DESCRIPTION");
			Vector2 relativeSize = m_descriptionLabel.relativeSize;
			localPosition2 = m_flavorLabel.transform.localPosition;
			localPosition2.y = m_descriptionLabel.transform.localPosition.y - 5f - relativeSize.y * 37f;
			m_flavorLabel.transform.localPosition = localPosition2;
			m_flavorLabel.text = LocaManager.GetText(m_selectedQuestStep.StaticData.FlavorDescription);
			relativeSize = m_flavorLabel.relativeSize;
			Vector3 localScale = m_frameBackground.transform.localScale;
			Single y = Math.Abs(m_flavorLabel.transform.localPosition.y) + m_flavorLabel.transform.localScale.y * relativeSize.y + 10f;
			m_frameBackground.transform.localScale = new Vector3(localScale.x, y, localScale.z);
			m_frameBorder.transform.localScale = new Vector3(localScale.x, y, localScale.z);
		}

		public void SetShowFinished(Boolean p_isShowFinished)
		{
			m_showFinished.isChecked = p_isShowFinished;
		}

		private void SetSelection(QuestStep qs)
		{
			foreach (BookEntry bookEntry in m_pageableList.EntryList)
			{
				bookEntry.SetSelection(qs);
			}
		}

		private void CleanupPageableList()
		{
			foreach (BookEntry bookEntry in m_pageableList.EntryList)
			{
				if (bookEntry != null)
				{
					bookEntry.Init(0, String.Empty);
					bookEntry.SetSelected(false);
					QuestEntry questEntry = bookEntry;
					if (questEntry != null)
					{
						QuestEntry questEntry2 = questEntry;
						questEntry2.OnQuestClicked = (EventHandler)Delegate.Remove(questEntry2.OnQuestClicked, new EventHandler(QuestClick));
					}
				}
			}
		}

		private void OnQuestTabIndexChanged(Object sender, EventArgs e)
		{
			Tab tab = m_questTabController.Tabs[m_questTabController.CurrentTabIndex];
			QuestTabInfo component = tab.GetComponent<QuestTabInfo>();
			if (component == null)
			{
				m_headerLabel.text = LocaManager.GetText("TT_QUEST_TOKENS");
				m_tokenPanel.Show();
				NGUITools.SetActive(m_showFinished.gameObject, false);
				NGUITools.SetActive(m_detailsPanel.gameObject, false);
				NGUITools.SetActive(m_pageableList.gameObject, false);
				return;
			}
			m_tokenPanel.Hide();
			NGUITools.SetActive(m_showFinished.gameObject, true);
			NGUITools.SetActive(m_detailsPanel.gameObject, true);
			NGUITools.SetActive(m_pageableList.gameObject, true);
			EQuestType questType = component.QuestType;
			switch (questType)
			{
			case EQuestType.QUEST_TYPE_MAIN:
				m_headerLabel.text = LocaManager.GetText("TT_QUEST_MAIN");
				break;
			default:
				if (questType != EQuestType.ALL)
				{
					m_headerLabel.text = String.Empty;
				}
				else
				{
					m_headerLabel.text = LocaManager.GetText("TT_QUEST_ALL");
				}
				break;
			case EQuestType.QUEST_TYPE_SIDE:
				m_headerLabel.text = LocaManager.GetText("TT_QUEST_SIDE");
				break;
			case EQuestType.QUEST_TYPE_ONGOING:
				m_headerLabel.text = LocaManager.GetText("TT_QUEST_ONGOING");
				break;
			}
			UpdateEntries(component.QuestType);
		}

		public void GoToQuest(QuestStep p_step)
		{
			Int32 p_tabID = 0;
			for (Int32 i = 0; i < m_questTabController.Tabs.Length; i++)
			{
				QuestTabInfo component = m_questTabController.Tabs[i].GetComponent<QuestTabInfo>();
				if (component != null && component.QuestType == p_step.StaticData.Type)
				{
					p_tabID = i;
				}
			}
			if (p_step.QuestState == EQuestState.SOLVED)
			{
				if (!m_showFinished.isChecked)
				{
					SetShowFinished(true);
				}
				else
				{
					UpdateEntries(p_step.StaticData.Type);
				}
			}
			m_questTabController.SelectTab(p_tabID, false);
			QuestClick(p_step, EventArgs.Empty);
		}

		public void GoToTokens()
		{
			Int32 p_tabID = 0;
			for (Int32 i = 0; i < m_questTabController.Tabs.Length; i++)
			{
				QuestTabInfo component = m_questTabController.Tabs[i].GetComponent<QuestTabInfo>();
				if (component == null && m_questTabController.Tabs[i].name.ToLower().Contains("tokens"))
				{
					p_tabID = i;
					break;
				}
			}
			m_questTabController.SelectTab(p_tabID, false);
		}

		public void Open()
		{
			m_pageableList.OnNextPageEvent += UpdateList;
			m_pageableList.OnPrevPageEvent += UpdateList;
			m_questTabController.TabIndexChanged += OnQuestTabIndexChanged;
			UICheckbox showFinished = m_showFinished;
			showFinished.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(showFinished.onStateChange, new UICheckbox.OnStateChange(OnCheckBoxChanged));
			m_questTabController.SelectTab(0, true);
		}

		public void Close()
		{
			m_pageableList.OnNextPageEvent -= UpdateList;
			m_pageableList.OnPrevPageEvent -= UpdateList;
			m_questTabController.TabIndexChanged -= OnQuestTabIndexChanged;
			UICheckbox showFinished = m_showFinished;
			showFinished.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(showFinished.onStateChange, new UICheckbox.OnStateChange(OnCheckBoxChanged));
			CleanupPageableList();
		}

		private void ClearDetails()
		{
			m_nameLabel.text = String.Empty;
			m_descriptionLabel.text = String.Empty;
			m_flavorLabel.text = String.Empty;
			NGUITools.SetActive(m_frameBorder.gameObject, false);
			NGUITools.SetActive(m_frameBackground.gameObject, false);
			foreach (JournalQuestObjective journalQuestObjective in m_objectiveList)
			{
				Destroy(journalQuestObjective.gameObject);
			}
			m_objectiveList.Clear();
		}

		private void OnCheckBoxChanged(Boolean state)
		{
			OnQuestTabIndexChanged(m_questTabController, EventArgs.Empty);
		}
	}
}
