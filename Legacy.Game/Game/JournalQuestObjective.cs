using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Quests;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/JournalQuestObjective")]
	public class JournalQuestObjective : MonoBehaviour
	{
		private const Single FADE_DELAY_TIME = 1f;

		private const Single FADE_OUT_TIME = 1.25f;

		[SerializeField]
		private UILabel m_objectiveName;

		[SerializeField]
		private UISprite m_objectiveIcon;

		[SerializeField]
		private Color m_textColor = Color.yellow;

		[SerializeField]
		private Color m_textColorSolved = Color.green;

		[SerializeField]
		private String m_bulletSpriteName = String.Empty;

		[SerializeField]
		private String m_solvedSpriteName = String.Empty;

		private QuestStep m_myQuestStep;

		private Boolean m_isHidden;

		private Single m_fadeOutStartTime = -1f;

		public Single Height
		{
			get
			{
				if (m_isHidden)
				{
					return 0f;
				}
				return m_objectiveName.transform.localScale.y;
			}
		}

		public void OpenQuestJournal()
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(m_myQuestStep, EEventType.QUESTLOG_SELECTED, EventArgs.Empty);
		}

		public void SetObjective(QuestStep p_questStep, QuestObjective p_objective)
		{
			m_myQuestStep = p_questStep;
			if (p_objective.MaxCounter <= 1)
			{
				m_objectiveName.text = LocaManager.GetText(p_objective.StaticData.Description);
			}
			else
			{
				m_objectiveName.text = String.Format(LocaManager.GetText(p_objective.StaticData.Description), p_objective.MaxCounter) + " " + String.Format(LocaManager.GetText("OBJECTIVE_X_OF_Y"), p_objective.CurrentCounter, p_objective.MaxCounter);
			}
			if (!String.IsNullOrEmpty(p_objective.StaticData.Location))
			{
				String text = LocaManager.GetText(p_objective.StaticData.Location);
				if (text.Contains("@"))
				{
					text = text.Replace("@", ", ");
				}
				UILabel objectiveName = m_objectiveName;
				objectiveName.text = objectiveName.text + " (" + text + ")";
			}
			Color color = (p_objective.QuestState != EQuestState.SOLVED) ? m_textColor : m_textColorSolved;
			m_objectiveName.color = color;
			m_objectiveIcon.spriteName = ((p_objective.QuestState != EQuestState.SOLVED) ? m_bulletSpriteName : m_solvedSpriteName);
		}

		public void FadeOut()
		{
			if (m_fadeOutStartTime == -1f)
			{
				m_fadeOutStartTime = Time.time;
				Show();
			}
		}

		public Single GetHeight()
		{
			return m_objectiveName.relativeSize.y * 37f;
		}

		public void Show()
		{
			m_objectiveName.alpha = 1f;
			m_objectiveIcon.alpha = 1f;
			m_isHidden = false;
		}

		public void Hide()
		{
			m_objectiveName.alpha = 0f;
			m_objectiveIcon.alpha = 0f;
			m_isHidden = true;
		}

		private void Update()
		{
			if (m_fadeOutStartTime != -1f && m_fadeOutStartTime + 1f < Time.time && m_fadeOutStartTime + 1f + 1.25f > Time.time)
			{
				Single alpha = 1f - (Time.time - m_fadeOutStartTime - 1f) / 1.25f;
				m_objectiveName.alpha = alpha;
				m_objectiveIcon.alpha = alpha;
			}
		}
	}
}
