using System;
using Legacy.Core.Quests;
using UnityEngine;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDQuestObjectiveNew")]
	public class NewHUDQuestObjective : MonoBehaviour
	{
		private const Single FADE_DELAY_TIME = 1f;

		private const Single FADE_OUT_TIME = 1.25f;

		private const Single SLIDE_ANIM_TIME = 0.25f;

		[SerializeField]
		private UILabel m_objectiveName;

		[SerializeField]
		private UISprite m_objectiveIcon;

		[SerializeField]
		private Color m_textColorSolved = Color.green;

		[SerializeField]
		private String m_solvedSpriteName = "ICO_checkbox_checked";

		private QuestObjective m_objective;

		private NewHUDQuestLogEntry m_parent;

		private Single m_waitBeforeFadeTime = -1f;

		private Boolean m_doneFading;

		private Int32 m_index;

		public QuestObjective Objective => m_objective;

	    public Boolean DoneFading => m_doneFading;

	    public Single Height => Math.Abs(m_objectiveName.transform.localScale.y * m_objectiveName.relativeSize.y);

	    public Int32 ListIndex
		{
			get => m_index;
		    set => m_index = value;
		}

		public void Init(NewHUDQuestLogEntry p_parent, QuestObjective p_objective, Int32 p_index)
		{
			m_parent = p_parent;
			m_objective = p_objective;
			m_index = p_index;
			if (p_objective.MaxCounter <= 1)
			{
				m_objectiveName.text = LocaManager.GetText(p_objective.StaticData.Description);
			}
			else
			{
				m_objectiveName.text = String.Format(LocaManager.GetText(p_objective.StaticData.Description), p_objective.MaxCounter) + " " + String.Format(LocaManager.GetText("OBJECTIVE_X_OF_Y"), p_objective.CurrentCounter, p_objective.MaxCounter);
			}
			Vector3 localPosition = m_objectiveIcon.transform.localPosition;
			localPosition.y -= Height / 2f;
			m_objectiveIcon.transform.localPosition = localPosition;
			m_doneFading = false;
		}

		public Boolean UpdateObjective()
		{
			if (m_objective.QuestState == EQuestState.SOLVED || m_parent.QuestStep.QuestState == EQuestState.SOLVED)
			{
				if (m_objective.MaxCounter > 1)
				{
					m_objectiveName.text = String.Format(LocaManager.GetText(m_objective.StaticData.Description), m_objective.MaxCounter) + " " + String.Format(LocaManager.GetText("OBJECTIVE_X_OF_Y"), m_objective.CurrentCounter, m_objective.MaxCounter);
				}
				m_doneFading = false;
				m_objectiveName.color = m_textColorSolved;
				m_objectiveIcon.spriteName = m_solvedSpriteName;
				return true;
			}
			if (m_objective.MaxCounter > 1)
			{
				m_doneFading = false;
				m_objectiveName.text = String.Format(LocaManager.GetText(m_objective.StaticData.Description), m_objective.MaxCounter) + " " + String.Format(LocaManager.GetText("OBJECTIVE_X_OF_Y"), m_objective.CurrentCounter, m_objective.MaxCounter);
				return true;
			}
			return false;
		}

		public void Fade(Single p_deltaAlpha)
		{
			if (m_waitBeforeFadeTime == -1f)
			{
				m_waitBeforeFadeTime = Time.time + 0.5f;
			}
			if (m_waitBeforeFadeTime > Time.time)
			{
				return;
			}
			if (m_doneFading)
			{
				return;
			}
			if (m_objective.QuestState == EQuestState.SOLVED)
			{
				p_deltaAlpha = -p_deltaAlpha;
			}
			else if (m_parent.QuestStep.QuestState == EQuestState.SOLVED)
			{
				p_deltaAlpha = -p_deltaAlpha;
			}
			m_objectiveName.alpha += p_deltaAlpha;
			m_objectiveIcon.alpha = m_objectiveName.alpha;
			if (m_objectiveName.alpha <= 0f || m_objectiveName.alpha >= 1f)
			{
				m_doneFading = true;
				m_waitBeforeFadeTime = -1f;
			}
		}

		public void FadeByLog(Single p_currentAlpha)
		{
			m_objectiveName.alpha = p_currentAlpha;
			m_objectiveIcon.alpha = m_objectiveName.alpha;
		}
	}
}
