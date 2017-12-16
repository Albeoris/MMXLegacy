using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Quests;
using UnityEngine;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDSideInfoQuest")]
	public class HUDSideInfoQuest : HUDSideInfoBase
	{
		[SerializeField]
		private String m_newQuestSpriteName = String.Empty;

		[SerializeField]
		private String m_updatedQuestSpriteName = String.Empty;

		[SerializeField]
		private String m_completedQuestSpriteName = String.Empty;

		private QuestStep m_questStep;

		private void Start()
		{
			if (String.IsNullOrEmpty(m_newQuestSpriteName) || String.IsNullOrEmpty(m_updatedQuestSpriteName) || String.IsNullOrEmpty(m_completedQuestSpriteName))
			{
				Debug.LogError("Quest icon names not set!");
			}
		}

		public void Init(QuestStep pStep, QuestState pState)
		{
			m_questStep = pStep;
			String text = "\n" + LocaManager.GetText(pStep.StaticData.Name);
			String icon;
			Color color;
			if (pState != QuestState.ADDED)
			{
				if (pState != QuestState.UPDATED)
				{
					text = LocaManager.GetText("COMPLETED_QUEST_INFO", text);
					icon = m_completedQuestSpriteName;
					color = Color.green;
				}
				else
				{
					text = LocaManager.GetText("UPDATED_QUEST_INFO", text);
					icon = m_updatedQuestSpriteName;
					color = new Color(0f, 0.7529412f, 1f, 1f);
				}
			}
			else
			{
				text = LocaManager.GetText("ADDED_QUEST_INFO", text);
				icon = m_newQuestSpriteName;
				color = Color.white;
			}
			base.Init(text, icon, 0, color);
		}

		protected override Boolean IsTooltipNeeded()
		{
			return false;
		}

		protected override void ShowTooltip()
		{
		}

		protected override void HideTooltip()
		{
		}

		protected override void OnClick()
		{
			if (m_questStep != null)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(m_questStep, EEventType.QUESTLOG_SELECTED, EventArgs.Empty);
			}
		}

		public enum QuestState
		{
			ADDED,
			UPDATED,
			COMPLETED
		}
	}
}
