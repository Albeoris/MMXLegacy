using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Quests;
using Legacy.Game.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDQuestLogEntryNew")]
	public class NewHUDQuestLogEntry : MonoBehaviour
	{
		private const Single FADE_DELAY_TIME = 1f;

		private const Single FADE_OUT_TIME = 1.75f;

		private const Single SPACING = 8f;

		private const Single SPACING_TO_NEXT_ENTRY = 15f;

		private const Single DELTA_ALPHA = 0.05f;

		[SerializeField]
		private UILabel m_questName;

		[SerializeField]
		private UISprite m_questIcon;

		[SerializeField]
		private Color m_textColor = Color.yellow;

		[SerializeField]
		private Color m_textColorSolved = Color.green;

		[SerializeField]
		private GameObject m_prefabObjective;

		[SerializeField]
		private GameObject m_objectiveHook;

		[SerializeField]
		private String m_mainQuestSpriteName = "ICO_quest_main";

		[SerializeField]
		private String m_sideQuestSpriteName = "ICO_quest_side";

		[SerializeField]
		private String m_ongoingQuestSpriteName = "ICO_quest_ongoing";

		[SerializeField]
		private String m_solvedSpriteName = "ICO_checkbox_checked";

		private QuestStep m_questStep;

		private Boolean m_isHovered;

		private IScrollingListener m_scrollingListener;

		private BoxCollider m_boxCollider;

		private List<NewHUDQuestObjective> m_objectives;

		private Single m_height;

		private List<NewHUDQuestObjective> m_fadingObjectives;

		private Int32 m_listIndex;

		private Boolean m_removeEntry;

		private Boolean m_isBusy;

		private Boolean m_doneFading;

		private Single m_waitBeforeFadeTime;

		public event EventHandler QuestHovered;

		public event EventHandler BusinessChanged;

		public QuestStep QuestStep => m_questStep;

	    public Boolean IsHovered => m_isHovered;

	    public Single Height => m_height;

	    public Int32 ListIndex
		{
			get => m_listIndex;
	        set => m_listIndex = value;
	    }

		public Boolean IsBusy => m_isBusy;

	    public Boolean CanRemoveEntry => m_removeEntry;

	    public Boolean DoneFading => m_doneFading;

	    public void Init(QuestStep p_questStep, Int32 p_index, IScrollingListener p_scrollingListener, Boolean p_silent)
		{
			m_questStep = p_questStep;
			m_listIndex = p_index;
			m_isHovered = false;
			m_scrollingListener = p_scrollingListener;
			m_boxCollider = this.gameObject.GetComponent<BoxCollider>();
			m_objectives = new List<NewHUDQuestObjective>();
			m_fadingObjectives = new List<NewHUDQuestObjective>();
			m_height = 0f;
			m_removeEntry = false;
			m_doneFading = true;
			m_isBusy = false;
			m_waitBeforeFadeTime = -1f;
			m_questName.text = LocaManager.GetText(m_questStep.StaticData.Name);
			m_questName.color = ((m_questStep.QuestState != EQuestState.SOLVED) ? m_textColor : m_textColorSolved);
			if (m_questStep.StaticData.Type == EQuestType.QUEST_TYPE_MAIN)
			{
				m_questIcon.spriteName = m_mainQuestSpriteName;
			}
			else if (m_questStep.StaticData.Type == EQuestType.QUEST_TYPE_ONGOING)
			{
				m_questIcon.spriteName = m_ongoingQuestSpriteName;
			}
			else
			{
				m_questIcon.spriteName = m_sideQuestSpriteName;
			}
			Int32 num = 0;
			foreach (QuestObjective questObjective in m_questStep.Objectives)
			{
				if (questObjective.QuestState == EQuestState.ACTIVE)
				{
					GameObject gameObject = NGUITools.AddChild(m_objectiveHook, m_prefabObjective);
					NewHUDQuestObjective component = gameObject.GetComponent<NewHUDQuestObjective>();
					component.Init(this, questObjective, num);
					ScrollingHelper.InitScrollListeners(m_scrollingListener, gameObject);
					m_objectives.Add(component);
					num++;
				}
			}
			if (p_silent)
			{
				FadeByLog(0f);
			}
			else
			{
				FadeByLog(0f);
				m_isBusy = true;
				m_doneFading = false;
			}
			RepositionObjectives();
		}

		private void RemoveObjective(NewHUDQuestObjective p_objective)
		{
			if (!m_objectives.Contains(p_objective))
			{
				return;
			}
			Int32 listIndex = p_objective.ListIndex;
			NewHUDQuestObjective p_obj = m_objectives[listIndex];
			m_objectives.RemoveAt(listIndex);
			Helper.DestroyGO<NewHUDQuestObjective>(p_obj);
			for (Int32 i = listIndex; i < m_objectives.Count; i++)
			{
				m_objectives[i].ListIndex = i;
			}
			RepositionObjectives();
		}

		private void AddObjective(QuestObjective p_newObjective)
		{
			GameObject gameObject = NGUITools.AddChild(m_objectiveHook, m_prefabObjective);
			NewHUDQuestObjective component = gameObject.GetComponent<NewHUDQuestObjective>();
			component.Init(this, p_newObjective, m_objectives.Count);
			ScrollingHelper.InitScrollListeners(m_scrollingListener, gameObject);
			m_objectives.Add(component);
			m_fadingObjectives.Add(component);
			RepositionObjectives();
		}

		public void UpdateObjectives()
		{
			List<QuestObjective> list = new List<QuestObjective>();
			foreach (QuestObjective questObjective in m_questStep.Objectives)
			{
				if (questObjective.QuestState == EQuestState.ACTIVE)
				{
					list.Add(questObjective);
				}
			}
			foreach (NewHUDQuestObjective newHUDQuestObjective in m_objectives)
			{
				if (newHUDQuestObjective.UpdateObjective() && !m_fadingObjectives.Contains(newHUDQuestObjective))
				{
					m_doneFading = false;
					m_fadingObjectives.Add(newHUDQuestObjective);
					m_isBusy = true;
					AlertParent();
				}
				list.Remove(newHUDQuestObjective.Objective);
			}
			if (list.Count > 0)
			{
				m_isBusy = true;
				AlertParent();
				foreach (QuestObjective p_newObjective in list)
				{
					AddObjective(p_newObjective);
				}
			}
		}

		public void UpdateStep()
		{
			if (m_questStep.QuestState == EQuestState.SOLVED)
			{
				m_doneFading = false;
				m_fadingObjectives.Clear();
				foreach (NewHUDQuestObjective newHUDQuestObjective in m_objectives)
				{
					newHUDQuestObjective.UpdateObjective();
				}
				m_questName.color = m_textColorSolved;
				m_questIcon.spriteName = m_solvedSpriteName;
				Vector3 localScale = new Vector3(32f, 32f, m_questIcon.transform.localScale.z);
				m_questIcon.transform.localScale = localScale;
				Vector3 localPosition = new Vector3(-32f, m_questIcon.transform.localPosition.y, m_questIcon.transform.localPosition.z);
				m_questIcon.transform.localPosition = localPosition;
				m_isBusy = true;
				AlertParent();
			}
		}

		public void RepositionObjectives()
		{
			Single num = 0f;
			foreach (NewHUDQuestObjective newHUDQuestObjective in m_objectives)
			{
				Vector3 localPosition = newHUDQuestObjective.transform.localPosition;
				localPosition.y = num;
				TweenPosition.Begin(newHUDQuestObjective.gameObject, 0.3f, localPosition);
				num -= newHUDQuestObjective.Height + 8f;
			}
			num += 8f;
			num -= m_questName.transform.localScale.y * m_questName.relativeSize.y;
			m_height = Math.Abs(num);
			Vector3 size = new Vector3(m_boxCollider.size.x, m_height + 15f, 1f);
			Vector3 center = new Vector3(m_boxCollider.center.x, -(size.y / 2f), -2f);
			m_boxCollider.size = size;
			m_boxCollider.center = center;
		}

		private void AlertParent()
		{
			if (BusinessChanged != null)
			{
				BusinessChanged(this, EventArgs.Empty);
			}
		}

		public void UpdateEntry()
		{
			if (m_questStep.QuestState == EQuestState.SOLVED)
			{
				FadeAll(0.05f);
			}
			else if (m_fadingObjectives != null && m_fadingObjectives.Count > 0)
			{
				for (Int32 i = m_fadingObjectives.Count - 1; i >= 0; i--)
				{
					NewHUDQuestObjective newHUDQuestObjective = m_fadingObjectives[i];
					newHUDQuestObjective.Fade(0.05f);
					if (newHUDQuestObjective.DoneFading)
					{
						m_fadingObjectives.RemoveAt(i);
						if (newHUDQuestObjective.Objective.QuestState == EQuestState.SOLVED)
						{
							RemoveObjective(newHUDQuestObjective);
						}
					}
				}
				RepositionObjectives();
			}
			else
			{
				FadeAll(0.05f);
			}
			if (m_doneFading && m_fadingObjectives.Count == 0)
			{
				if (m_questStep.QuestState == EQuestState.SOLVED)
				{
					m_removeEntry = true;
				}
				m_isBusy = false;
				AlertParent();
			}
		}

		private void Fade(Single p_deltaAlpha)
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
			if (m_questStep.QuestState != EQuestState.ACTIVE)
			{
				p_deltaAlpha = -p_deltaAlpha;
			}
			m_questName.alpha += p_deltaAlpha;
			m_questIcon.alpha = m_questName.alpha;
			if (m_questName.alpha <= 0f || m_questName.alpha >= 1f)
			{
				m_doneFading = true;
				m_waitBeforeFadeTime = -1f;
			}
		}

		public void FadeByLog(Single p_currentAlpha)
		{
			if (m_isBusy && !m_removeEntry && m_fadingObjectives.Count == 0)
			{
				return;
			}
			m_questName.alpha = p_currentAlpha;
			m_questIcon.alpha = m_questName.alpha;
			foreach (NewHUDQuestObjective newHUDQuestObjective in m_objectives)
			{
				newHUDQuestObjective.FadeByLog(p_currentAlpha);
			}
		}

		public void FadeAll(Single p_deltaAlpha)
		{
			Fade(p_deltaAlpha);
			foreach (NewHUDQuestObjective newHUDQuestObjective in m_objectives)
			{
				newHUDQuestObjective.Fade(p_deltaAlpha);
			}
		}

		public override Boolean Equals(Object p_other)
		{
			return p_other is NewHUDQuestLogEntry && ((NewHUDQuestLogEntry)p_other).QuestStep.StaticData.StaticID == m_questStep.StaticData.StaticID;
		}

		public override Int32 GetHashCode()
		{
			return base.GetHashCode();
		}

		private void OnHover(Boolean p_isHovered)
		{
			m_isHovered = p_isHovered;
			if (QuestHovered != null)
			{
				QuestHovered(this, EventArgs.Empty);
			}
		}

		private void OnClick()
		{
			OnHover(false);
			LegacyLogic.Instance.EventManager.InvokeEvent(m_questStep, EEventType.QUESTLOG_SELECTED, EventArgs.Empty);
		}
	}
}
