using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Quests;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/Cheats/CheatsQuests")]
	public class CheatsQuest : MonoBehaviour
	{
		[SerializeField]
		private UIPopupList m_newQuestsList;

		[SerializeField]
		private UIPopupList m_openQuestsList;

		private List<Int32> m_newQuestIDs;

		private List<Int32> m_openQuestIDs;

		private void Awake()
		{
			m_newQuestIDs = new List<Int32>();
			m_openQuestIDs = new List<Int32>();
		}

		private void OnEnable()
		{
			UpdateNewQuestList();
			UpdateOpenQuestList();
		}

		private void UpdateNewQuestList()
		{
			m_newQuestsList.items.Clear();
			m_newQuestIDs.Clear();
			IEnumerable<QuestStepStaticData> iterator = StaticDataHandler.GetIterator<QuestStepStaticData>(EDataType.QUEST_STEPS);
			foreach (QuestStepStaticData questStepStaticData in iterator)
			{
				QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(questStepStaticData.StaticID);
				if (step != null && step.QuestState == EQuestState.INACTIVE)
				{
					m_newQuestsList.items.Add(LocaManager.GetText(step.StaticData.Name));
					m_newQuestIDs.Add(step.StaticData.StaticID);
				}
			}
			SelectDefaultNewQuestEntry();
		}

		private void SelectDefaultNewQuestEntry()
		{
			if (m_newQuestsList.items.Count > 0)
			{
				m_newQuestsList.selection = m_newQuestsList.items[0];
			}
			else
			{
				m_newQuestsList.selection = String.Empty;
			}
		}

		private void UpdateOpenQuestList()
		{
			m_openQuestsList.items.Clear();
			m_openQuestIDs.Clear();
			IEnumerable<QuestStepStaticData> iterator = StaticDataHandler.GetIterator<QuestStepStaticData>(EDataType.QUEST_STEPS);
			foreach (QuestStepStaticData questStepStaticData in iterator)
			{
				QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(questStepStaticData.StaticID);
				if (step != null && step.QuestState == EQuestState.ACTIVE)
				{
					m_openQuestsList.items.Add(LocaManager.GetText(step.StaticData.Name));
					m_openQuestIDs.Add(step.StaticData.StaticID);
				}
			}
			SelectDefaultOpenQuestEntry();
		}

		private void SelectDefaultOpenQuestEntry()
		{
			if (m_openQuestsList.items.Count > 0)
			{
				m_openQuestsList.selection = m_openQuestsList.items[0];
			}
			else
			{
				m_openQuestsList.selection = String.Empty;
			}
		}

		public void OnAddQuestButtonClicked()
		{
			Int32 num = m_newQuestsList.items.IndexOf(m_newQuestsList.selection);
			if (num >= 0)
			{
				LegacyLogic.Instance.WorldManager.QuestHandler.ActivateQuest(m_newQuestIDs[num]);
				UpdateNewQuestList();
				UpdateOpenQuestList();
			}
		}

		public void OnCompleteQuestButtonClicked()
		{
			Int32 num = m_openQuestsList.items.IndexOf(m_openQuestsList.selection);
			if (num >= 0)
			{
				QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_openQuestIDs[num]);
				step.Solve();
				LegacyLogic.Instance.WorldManager.QuestHandler.FinalizeStep(step);
				UpdateOpenQuestList();
			}
		}
	}
}
