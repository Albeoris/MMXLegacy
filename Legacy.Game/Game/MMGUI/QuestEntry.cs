using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Quests;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/QuestEntry")]
	public class QuestEntry : MonoBehaviour
	{
		[SerializeField]
		protected UILabel m_title;

		[SerializeField]
		protected UISprite m_questIcon;

		[SerializeField]
		protected UISprite m_marker;

		[SerializeField]
		protected UISprite m_background;

		[SerializeField]
		protected UISprite m_markerNewEntry;

		public Boolean m_selected;

		public Boolean m_finished;

		protected QuestStep m_step;

		public EventHandler OnQuestClicked;

		public void SetQuestStep(QuestStep p_step)
		{
			NGUITools.SetActive(m_markerNewEntry.gameObject, false);
			m_step = p_step;
			m_selected = false;
			m_finished = (p_step.QuestState == EQuestState.SOLVED);
			m_title.text = LocaManager.GetText(p_step.StaticData.Name);
			m_marker.alpha = 0f;
			m_title.color = Color.black;
			if (m_finished)
			{
				m_questIcon.alpha = 0.5f;
				m_title.alpha = 0.5f;
			}
			else
			{
				m_questIcon.alpha = 1f;
				m_title.alpha = 1f;
			}
			if (m_step.StaticData.Type == EQuestType.QUEST_TYPE_MAIN)
			{
				m_questIcon.spriteName = "ICO_quest_main";
			}
			else if (m_step.StaticData.Type == EQuestType.QUEST_TYPE_ONGOING)
			{
				m_questIcon.spriteName = "ICO_quest_ongoing";
			}
			else
			{
				m_questIcon.spriteName = "ICO_quest_side";
			}
		}

		public void RestorePositions()
		{
			if (m_questIcon.alpha == 0f)
			{
				m_questIcon.alpha = 1f;
			}
			Vector3 localPosition = new Vector3(45f, m_title.transform.localPosition.y, m_title.transform.localPosition.z);
			m_title.transform.localPosition = localPosition;
		}

		public QuestStep GetQuestStep()
		{
			return m_step;
		}

		public void OnQuestClick(Object sender)
		{
			if (OnQuestClicked != null)
			{
				OnQuestClicked(m_step, EventArgs.Empty);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(m_step, EEventType.QUEST_SELECTED, EventArgs.Empty);
		}

		public void SetSelection(QuestStep qs)
		{
			if (qs == m_step)
			{
				m_title.color = Color.white;
				m_selected = true;
				m_marker.alpha = 1f;
				m_background.enabled = true;
				if (m_finished)
				{
					m_questIcon.alpha = 0.5f;
				}
				else
				{
					m_questIcon.alpha = 1f;
				}
				m_title.alpha = 1f;
			}
			else
			{
				m_title.color = Color.black;
				m_selected = false;
				m_marker.alpha = 0f;
				m_background.enabled = false;
				if (m_finished)
				{
					m_questIcon.alpha = 0.5f;
					m_title.alpha = 0.5f;
				}
				else
				{
					m_questIcon.alpha = 1f;
					m_title.alpha = 1f;
				}
			}
		}

		public virtual void OnHover(Boolean isOver)
		{
			if (isOver)
			{
				m_title.color = Color.white;
				m_marker.alpha = 1f;
				m_background.enabled = true;
				m_marker.spriteName = "GUI_quest_marker";
				if (m_finished)
				{
					m_questIcon.alpha = 0.5f;
				}
				else
				{
					m_questIcon.alpha = 1f;
				}
				m_title.alpha = 1f;
			}
			else if (m_selected)
			{
				m_title.color = Color.white;
				m_marker.alpha = 1f;
				m_background.enabled = true;
				m_marker.spriteName = "GUI_quest_marker_active";
				if (m_finished)
				{
					m_questIcon.alpha = 0.5f;
				}
				else
				{
					m_questIcon.alpha = 1f;
				}
				m_title.alpha = 1f;
			}
			else
			{
				m_title.color = Color.black;
				m_marker.alpha = 0f;
				m_background.enabled = false;
				if (m_finished)
				{
					m_questIcon.alpha = 0.5f;
					m_title.alpha = 0.5f;
				}
				else
				{
					m_questIcon.alpha = 1f;
					m_title.alpha = 1f;
				}
			}
		}
	}
}
