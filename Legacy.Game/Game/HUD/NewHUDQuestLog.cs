using System;
using System.Collections.Generic;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.Quests;
using Legacy.Game.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDQuestLogNew")]
	public class NewHUDQuestLog : BaseHUDLog, IResizable, IScrollingListener
	{
		private const Single SPACING = 15f;

		private const Single DELTA_ALPHA = 0.02f;

		private Boolean m_waitForLoadingScreen;

		[SerializeField]
		private UIPanel m_questLogPanel;

		[SerializeField]
		private Single m_panelOffset = 3f;

		[SerializeField]
		private UISprite m_background;

		[SerializeField]
		private UIScrollBar m_scrollBar;

		[SerializeField]
		private UISprite m_scrollBackground;

		[SerializeField]
		private UISprite m_buttonHideIcon;

		[SerializeField]
		private UIButton m_buttonHide;

		[SerializeField]
		private HUDActionLogDragButton m_buttonChangeSize;

		[SerializeField]
		private Single m_scrollBarHeightReduce = 10f;

		[SerializeField]
		private Single m_minSize = 265f;

		[SerializeField]
		private Single m_maxSize = 912f;

		[SerializeField]
		private GameObject m_prefabEntry;

		[SerializeField]
		private GameObject m_entryHook;

		[SerializeField]
		private Single m_saveAfterTime = 3f;

		private UIRoot m_uiRoot;

		private Single m_buttonChangeSizeOffset;

		private Single m_backgroundOffset;

		private Single m_collisionBoxOffset;

		private Single m_collisionBoxScaleOffset;

		private Single m_scrollDuration = 0.25f;

		private Single m_sizeChangeSaveTimer;

		private BoxCollider m_boxCollider;

		private List<QuestStep> m_quests;

		private QuestHandler m_questHandler;

		private List<NewHUDQuestLogEntry> m_entries;

		private List<NewHUDQuestLogEntry> m_busyEntries;

		private Single m_height;

		private Boolean m_minimized;

		private Boolean m_visible;

		private NewHUDQuestLogEntry m_currentlyVisibleEntry;

		private Boolean m_componentHovered;

		private Single m_waitTimeForRemovedEntry = -1f;

		private Single m_questCompletedSoundEndTime = -1f;

		public void Init(QuestHandler p_questHandler)
		{
			base.Init();
			m_questHandler = p_questHandler;
			m_waitForLoadingScreen = false;
			m_initialBackgroundAlpha = ConfigManager.Instance.Options.LogOpacity;
			m_boxCollider = gameObject.GetComponent<BoxCollider>();
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			Single y = m_scrollBackground.transform.localScale.y;
			m_backgroundOffset = m_background.gameObject.transform.localScale.y - y;
			m_buttonChangeSizeOffset = m_buttonChangeSize.gameObject.transform.localPosition.y + y;
			m_collisionBoxScaleOffset = m_boxCollider.size.y - y;
			m_collisionBoxOffset = m_boxCollider.center.y + y / 2f;
			m_buttonChangeSize.Init(this);
			m_buttonChangeSize.ButtonReleased += OnButtonReleased;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.QUESTLOG_CHANGED, new EventHandler(OnQuestLogChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAME_LOADED, new EventHandler(OnSaveGameLoaded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(DoUpdate));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(DoNotUpdate));
			m_quests = new List<QuestStep>();
			if (m_entries == null)
			{
				m_entries = new List<NewHUDQuestLogEntry>();
			}
			m_busyEntries = new List<NewHUDQuestLogEntry>();
			m_height = 0f;
			m_minimized = false;
			m_componentHovered = false;
			InitLogEntries();
			Vector4 clipRange = m_questLogPanel.clipRange;
			clipRange.w = ConfigManager.Instance.Options.QuestLogSize;
			clipRange.y = -(clipRange.w / 2f - m_panelOffset);
			m_questLogPanel.clipRange = clipRange;
			UpdateSize();
		}

		public void CleanUp()
		{
			ClearEntries();
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Remove(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			m_buttonChangeSize.ButtonReleased -= OnButtonReleased;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.QUESTLOG_CHANGED, new EventHandler(OnQuestLogChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAME_LOADED, new EventHandler(OnSaveGameLoaded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(DoUpdate));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(DoNotUpdate));
		}

		private void DoUpdate(Object p_sender, EventArgs p_args)
		{
			ResetWaitTime();
			m_waitForLoadingScreen = false;
		}

		private void DoNotUpdate(Object p_sender, EventArgs p_args)
		{
			m_waitForLoadingScreen = true;
		}

		private void ClearEntries()
		{
			m_busyEntries = new List<NewHUDQuestLogEntry>();
			if (m_entries != null)
			{
				for (Int32 i = m_entries.Count - 1; i >= 0; i--)
				{
					NewHUDQuestLogEntry newHUDQuestLogEntry = m_entries[i];
					m_entries.RemoveAt(i);
					newHUDQuestLogEntry.BusinessChanged -= OnBusinessChanged;
					newHUDQuestLogEntry.QuestHovered -= OnQuestHovered;
					newHUDQuestLogEntry.gameObject.collider.enabled = false;
					Helper.DestroyGO<NewHUDQuestLogEntry>(newHUDQuestLogEntry);
				}
				m_entries.Clear();
			}
			m_entries = new List<NewHUDQuestLogEntry>();
		}

		private void Start()
		{
			if (m_uiRoot == null)
			{
				m_uiRoot = NGUITools.FindInParents<UIRoot>(gameObject);
			}
		}

		private void OnDestroy()
		{
			Cleanup();
			CleanUp();
			if (m_scrollBar != null)
			{
				Destroy(m_scrollBar.gameObject);
			}
		}

		private void InitLogEntries()
		{
			ClearEntries();
			m_quests.Clear();
			m_quests.AddRange(m_questHandler.GetActiveStepsByCategory(EQuestType.QUEST_TYPE_MAIN));
			m_quests.AddRange(m_questHandler.GetActiveStepsByCategory(EQuestType.QUEST_TYPE_SIDE));
			m_quests.AddRange(m_questHandler.GetActiveStepsByCategory(EQuestType.QUEST_TYPE_ONGOING));
			m_quests.AddRange(m_questHandler.GetActiveStepsByCategory(EQuestType.QUEST_TYPE_GRANDMASTER));
			m_quests.AddRange(m_questHandler.GetActiveStepsByCategory(EQuestType.QUEST_TYPE_PROMOTION));
			Int32 num = 0;
			foreach (QuestStep p_questStep in m_quests)
			{
				GameObject gameObject = NGUITools.AddChild(m_entryHook, m_prefabEntry);
				NewHUDQuestLogEntry component = gameObject.GetComponent<NewHUDQuestLogEntry>();
				component.Init(p_questStep, num, this, false);
				ScrollingHelper.InitScrollListeners(this, gameObject);
				component.BusinessChanged += OnBusinessChanged;
				component.QuestHovered += OnQuestHovered;
				m_entries.Insert(num, component);
				m_busyEntries.Add(component);
				num++;
			}
			RepositionEntries();
			if (m_entries.Count > 0)
			{
				m_currentlyVisibleEntry = m_entries[0];
			}
		}

		private void RepositionEntries()
		{
			Int32 num = 0;
			Single num2 = 0f;
			foreach (NewHUDQuestLogEntry newHUDQuestLogEntry in m_entries)
			{
				newHUDQuestLogEntry.RepositionObjectives();
				Vector3 localPosition = newHUDQuestLogEntry.transform.localPosition;
				localPosition.y = num2;
				TweenPosition.Begin(newHUDQuestLogEntry.gameObject, 0.3f, localPosition);
				num++;
				num2 -= 15f + newHUDQuestLogEntry.Height;
			}
			m_height = Math.Abs(num2);
			OnScrollBarChange(m_scrollBar);
		}

		private void RemoveEntry(Int32 p_index)
		{
			m_waitTimeForRemovedEntry = Time.time + m_configFadeDelay;
			NewHUDQuestLogEntry newHUDQuestLogEntry = m_entries[p_index];
			m_entries.RemoveAt(p_index);
			NGUITools.SetActive(newHUDQuestLogEntry.gameObject, false);
			Helper.DestroyGO<NewHUDQuestLogEntry>(newHUDQuestLogEntry);
			UpdateIndexProperties();
			if (m_entries.Count == 0)
			{
				m_currentlyVisibleEntry = null;
			}
			else if (p_index >= m_entries.Count)
			{
				m_currentlyVisibleEntry = m_entries[p_index - 1];
			}
			else if (p_index < m_entries.Count)
			{
				m_currentlyVisibleEntry = m_entries[p_index];
			}
			RepositionEntries();
		}

		private void AddEntry(QuestStep p_newQuest)
		{
			Int32 positionForEntry = GetPositionForEntry(p_newQuest);
			if (positionForEntry < 0)
			{
				return;
			}
			GameObject gameObject = NGUITools.AddChild(m_entryHook, m_prefabEntry);
			NewHUDQuestLogEntry component = gameObject.GetComponent<NewHUDQuestLogEntry>();
			component.Init(p_newQuest, positionForEntry, this, false);
			ScrollingHelper.InitScrollListeners(this, gameObject);
			component.BusinessChanged += OnBusinessChanged;
			component.QuestHovered += OnQuestHovered;
			m_entries.Insert(positionForEntry, component);
			UpdateIndexProperties();
			RepositionEntries();
			OnBusinessChanged(component, EventArgs.Empty);
		}

		private void UpdateIndexProperties()
		{
			for (Int32 i = 0; i < m_entries.Count; i++)
			{
				m_entries[i].ListIndex = i;
			}
		}

		private Int32 GetPositionForEntry(QuestStep p_new)
		{
			Int32 type = (Int32)p_new.StaticData.Type;
			Int32 num = 0;
			foreach (NewHUDQuestLogEntry newHUDQuestLogEntry in m_entries)
			{
				if (p_new.StaticData.StaticID == newHUDQuestLogEntry.QuestStep.StaticData.StaticID)
				{
					return -1;
				}
				if (type < (Int32)newHUDQuestLogEntry.QuestStep.StaticData.Type)
				{
					return num;
				}
				num++;
			}
			return num;
		}

		public void SetVisible(Boolean p_visible)
		{
			m_visible = p_visible;
			UpdateVisibility();
			ScrollingHelper.InitScrollListeners(this, m_scrollBar.gameObject);
		}

		private void UpdateVisibility()
		{
			NGUITools.SetActive(m_scrollBar.gameObject, !m_minimized && m_visible);
			NGUITools.SetActive(m_entryHook.gameObject, !m_minimized && m_visible);
			NGUITools.SetActive(m_buttonChangeSize.gameObject, !m_minimized && m_visible);
			m_buttonChangeSize.FadeButton(m_currentAlpha * 0.5f);
			NGUITools.SetActive(m_background.gameObject, !m_minimized && m_visible);
			m_boxCollider.enabled = (!m_minimized && m_visible);
			if (!m_minimized)
			{
				m_buttonHideIcon.spriteName = "ICO_window_minimize";
				m_buttonHideIcon.Update();
			}
			else
			{
				m_buttonHideIcon.spriteName = "ICO_window_maximize";
				m_buttonHideIcon.Update();
			}
			m_buttonHide.isEnabled = m_visible;
		}

		private void ScrollToEntry(NewHUDQuestLogEntry p_entry)
		{
			if (p_entry == null)
			{
				return;
			}
			Single scrollPotential = GetScrollPotential(m_questLogPanel.clipRange.w);
			Single num = -p_entry.transform.localPosition.y;
			if (p_entry.ListIndex > 0 && num == 0f)
			{
				return;
			}
			m_scrollBar.scrollValue = num / scrollPotential;
			m_currentlyVisibleEntry = p_entry;
		}

		public override void OnOptionsChanged()
		{
			m_initialBackgroundAlpha = ConfigManager.Instance.Options.LogOpacity;
			if (m_configSaysFade)
			{
				if (m_busyEntries.Count == 0)
				{
					m_locked = false;
					m_fadeState = EFadeState.WAITING;
					ResetWaitTime();
				}
			}
			else
			{
				m_background.alpha = m_initialBackgroundAlpha;
			}
		}

		private void OnQuestLogChanged(Object p_sender, EventArgs p_args)
		{
			QuestChangedEventArgs questChangedEventArgs = (QuestChangedEventArgs)p_args;
			if (questChangedEventArgs == null)
			{
				return;
			}
			switch (questChangedEventArgs.ChangeType)
			{
			case QuestChangedEventArgs.Type.NEW_QUEST:
				AudioController.Play("QuestRecieved", 1f, Mathf.Max(0f, 0.9f * (m_questCompletedSoundEndTime - Time.realtimeSinceStartup)), 0f);
				AddEntry(questChangedEventArgs.QuestStep);
				break;
			case QuestChangedEventArgs.Type.COMPLETED_QUEST:
			{
				AudioObject audioObject = AudioController.Play("QuestCompleted");
				if (audioObject != null)
				{
					m_questCompletedSoundEndTime = Time.realtimeSinceStartup + audioObject.clipLength;
				}
				else if (!AudioController.IsValidAudioID("QuestCompleted"))
				{
					Debug.LogError("Could not play quest completed sound. AudioID=QuestCompleted");
				}
				foreach (NewHUDQuestLogEntry newHUDQuestLogEntry in m_entries)
				{
					if (questChangedEventArgs.QuestStep.StaticData.StaticID == newHUDQuestLogEntry.QuestStep.StaticData.StaticID)
					{
						newHUDQuestLogEntry.UpdateStep();
						m_currentlyVisibleEntry = newHUDQuestLogEntry;
					}
				}
				break;
			}
			case QuestChangedEventArgs.Type.COMPLETED_OBJECTIVE:
				if (m_questCompletedSoundEndTime - 0.25f < Time.realtimeSinceStartup)
				{
					AudioController.Play("QuestObjectiveCompleted");
				}
				foreach (NewHUDQuestLogEntry newHUDQuestLogEntry2 in m_entries)
				{
					if (questChangedEventArgs.QuestStep.StaticData.StaticID == newHUDQuestLogEntry2.QuestStep.StaticData.StaticID)
					{
						newHUDQuestLogEntry2.UpdateObjectives();
						m_currentlyVisibleEntry = newHUDQuestLogEntry2;
					}
				}
				RepositionEntries();
				break;
			}
		}

		private void OnSaveGameLoaded(Object p_sender, EventArgs p_args)
		{
			InitLogEntries();
		}

		private void OnQuestHovered(Object p_sender, EventArgs p_args)
		{
			if (p_sender is NewHUDQuestLogEntry)
			{
				NewHUDQuestLogEntry newHUDQuestLogEntry = (NewHUDQuestLogEntry)p_sender;
				m_componentHovered = newHUDQuestLogEntry.IsHovered;
				OnHover(m_componentHovered);
			}
		}

		private void OnBusinessChanged(Object p_sender, EventArgs p_args)
		{
			NewHUDQuestLogEntry newHUDQuestLogEntry = (NewHUDQuestLogEntry)p_sender;
			if (newHUDQuestLogEntry == null)
			{
				return;
			}
			if (newHUDQuestLogEntry.IsBusy)
			{
				if (!m_busyEntries.Contains(newHUDQuestLogEntry))
				{
					m_busyEntries.Add(newHUDQuestLogEntry);
				}
			}
			else if (newHUDQuestLogEntry.CanRemoveEntry)
			{
				if (!m_busyEntries.Contains(newHUDQuestLogEntry))
				{
					m_busyEntries.Add(newHUDQuestLogEntry);
				}
			}
			else
			{
				m_locked = false;
				for (Int32 i = m_busyEntries.Count - 1; i >= 0; i--)
				{
					NewHUDQuestLogEntry newHUDQuestLogEntry2 = m_busyEntries[i];
					if (newHUDQuestLogEntry2.QuestStep.StaticData.StaticID == newHUDQuestLogEntry.QuestStep.StaticData.StaticID)
					{
						m_busyEntries.RemoveAt(i);
						break;
					}
				}
				ResetWaitTime();
			}
			RepositionEntries();
			if (m_scrollBar.barSize != 1f)
			{
				ScrollToEntry(newHUDQuestLogEntry);
			}
		}

		private void OnButtonReleased(Object p_sender, EventArgs p_args)
		{
			HUDActionLogDragButton x = (HUDActionLogDragButton)p_sender;
			if (x != null && x == m_buttonChangeSize)
			{
				m_buttonChangeSize.FadeButton(m_initialScrollBarAlpha);
				ResetWaitTime();
				m_locked = false;
			}
		}

		public override void FadeComponents(Single p_alpha)
		{
			base.FadeComponents(p_alpha);
			if (p_alpha <= m_initialScrollBarAlpha)
			{
				m_scrollBar.alpha = p_alpha;
				m_buttonChangeSize.FadeButton(p_alpha);
			}
			else
			{
				m_scrollBar.alpha = m_initialScrollBarAlpha;
				m_buttonChangeSize.FadeButton(m_initialScrollBarAlpha);
			}
			if (p_alpha <= m_initialBackgroundAlpha)
			{
				m_background.alpha = p_alpha;
			}
			else
			{
				m_background.alpha = m_initialBackgroundAlpha;
			}
			if (m_entries != null)
			{
				foreach (NewHUDQuestLogEntry newHUDQuestLogEntry in m_entries)
				{
					newHUDQuestLogEntry.FadeByLog(p_alpha);
				}
			}
		}

		protected override void Update()
		{
			if (m_sizeChangeSaveTimer > 0f)
			{
				m_sizeChangeSaveTimer -= Time.deltaTime;
				if (m_sizeChangeSaveTimer <= 0f)
				{
					Single w = m_questLogPanel.clipRange.w;
					if (w != ConfigManager.Instance.Options.QuestLogSize)
					{
						ConfigManager.Instance.Options.QuestLogSize = w;
						ConfigManager.Instance.WriteConfigurations();
					}
				}
			}
			if (!m_configSaysFade)
			{
				if (m_currentAlpha < 1f)
				{
					m_currentAlpha = 1f;
					FadeComponents(m_currentAlpha);
					m_fadeState = EFadeState.WAITING;
					m_locked = true;
				}
				if (m_busyEntries.Count > 0)
				{
					for (Int32 i = m_busyEntries.Count - 1; i >= 0; i--)
					{
						NewHUDQuestLogEntry newHUDQuestLogEntry = m_busyEntries[i];
						newHUDQuestLogEntry.UpdateEntry();
						if (newHUDQuestLogEntry.CanRemoveEntry)
						{
							Int32 listIndex = newHUDQuestLogEntry.ListIndex;
							RemoveEntry(listIndex);
							m_busyEntries.Remove(newHUDQuestLogEntry);
						}
					}
				}
				return;
			}
			if (m_waitForLoadingScreen || LegacyLogic.Instance.ConversationManager.IsOpen)
			{
				return;
			}
			if (m_waitTimeForRemovedEntry > -1f && m_waitTimeForRemovedEntry > Time.time)
			{
				return;
			}
			m_waitTimeForRemovedEntry = -1f;
			if (m_busyEntries != null && m_busyEntries.Count > 0)
			{
				if (m_fadeState == EFadeState.FADE_IN && m_currentAlpha >= 1f)
				{
					m_fadeState = EFadeState.WAITING;
				}
				if (m_fadeState == EFadeState.DONE || m_fadeState == EFadeState.FADE_OUT)
				{
					m_fadeState = EFadeState.FADE_IN;
				}
				else if (m_fadeState == EFadeState.WAITING)
				{
					m_locked = true;
					for (Int32 j = m_busyEntries.Count - 1; j >= 0; j--)
					{
						NewHUDQuestLogEntry newHUDQuestLogEntry2 = m_busyEntries[j];
						newHUDQuestLogEntry2.UpdateEntry();
						if (newHUDQuestLogEntry2.CanRemoveEntry)
						{
							Int32 listIndex2 = newHUDQuestLogEntry2.ListIndex;
							RemoveEntry(listIndex2);
							m_busyEntries.Remove(newHUDQuestLogEntry2);
							if (m_busyEntries.Count == 0)
							{
								m_locked = false;
							}
						}
					}
				}
			}
			if (m_buttonChangeSize.IsPressed && m_fadeState == EFadeState.WAITING)
			{
				m_locked = true;
			}
			base.Update();
		}

		private void OnComponentHover(Boolean p_state)
		{
			m_componentHovered = p_state;
			OnHover(p_state);
		}

		public override void OnHover(Boolean p_isHovered)
		{
			m_isHovered = (p_isHovered || m_componentHovered);
			if (m_isAlreadyFadingIn || m_locked)
			{
				return;
			}
			if (m_isHovered)
			{
				m_fadeState = EFadeState.FADE_IN;
			}
			else if (m_currentAlpha < 1f)
			{
				m_fadeState = EFadeState.FADE_OUT;
			}
			else
			{
				m_waitTime = Time.time + m_configFadeDelay;
				m_fadeState = EFadeState.WAITING;
			}
		}

		public void OnHideButtonClicked()
		{
			m_minimized = !m_minimized;
			UpdateVisibility();
		}

		private Single GetScrollPotential(Single p_clipRange)
		{
			Single num = m_height - p_clipRange;
			Single num2 = 0f;
			for (Int32 i = 0; i < m_entries.Count; i++)
			{
				num2 += m_entries[i].Height + 15f;
				if (num2 > num)
				{
					return num2;
				}
			}
			return num;
		}

		private void OnScrollBarChange(UIScrollBar p_scrollBar)
		{
			Single scrollPotential = GetScrollPotential(m_questLogPanel.clipRange.w);
			if (m_entries.Count == 0)
			{
				m_scrollBar.barSize = 1f;
			}
			else
			{
				m_scrollBar.barSize = m_questLogPanel.clipRange.w / m_height;
			}
			if (scrollPotential > 0f)
			{
				Single nearestEntryPosition = GetNearestEntryPosition(m_scrollBar.scrollValue * scrollPotential);
				SetVerticalPosition(nearestEntryPosition);
				m_scrollBar.scrollValue = nearestEntryPosition / scrollPotential;
			}
			else
			{
				m_scrollBar.scrollValue = 0f;
			}
		}

		private Single GetNearestEntryPosition(Single p_yPos)
		{
			Single result = p_yPos;
			if (m_entries != null && m_entries.Count > 0)
			{
				foreach (NewHUDQuestLogEntry newHUDQuestLogEntry in m_entries)
				{
					if (Mathf.Abs(p_yPos) >= Mathf.Abs(newHUDQuestLogEntry.transform.localPosition.y))
					{
						result = -newHUDQuestLogEntry.transform.localPosition.y;
						m_currentlyVisibleEntry = newHUDQuestLogEntry;
					}
				}
			}
			return result;
		}

		private void UpdateSize()
		{
			Single num = m_questLogPanel.clipRange.w - m_scrollBarHeightReduce;
			Vector3 localScale = m_scrollBackground.transform.localScale;
			localScale.y = num;
			m_scrollBackground.transform.localScale = localScale;
			Vector3 localPosition = m_buttonChangeSize.gameObject.transform.localPosition;
			localPosition.y = -num + m_buttonChangeSizeOffset;
			m_buttonChangeSize.gameObject.transform.localPosition = localPosition;
			Vector3 localScale2 = m_background.gameObject.transform.localScale;
			localScale2.y = num + m_backgroundOffset;
			m_background.gameObject.transform.localScale = localScale2;
			Vector3 size = m_boxCollider.size;
			size.y = m_collisionBoxScaleOffset + num;
			m_boxCollider.size = size;
			Vector3 center = m_boxCollider.center;
			center.y = m_collisionBoxOffset - num / 2f;
			m_boxCollider.center = center;
			m_scrollBar.barSize = 0f;
			OnScrollBarChange(m_scrollBar);
		}

		private Single GetPreviousEntryPosition()
		{
			Single result = 0f;
			if (m_entries != null && m_entries.Count > 0)
			{
				Int32 listIndex = m_currentlyVisibleEntry.ListIndex;
				if (listIndex == 0)
				{
					result = -m_currentlyVisibleEntry.transform.localPosition.y;
				}
				else
				{
					result = -m_entries[listIndex - 1].transform.localPosition.y + 1f;
					m_currentlyVisibleEntry = m_entries[listIndex - 1];
				}
			}
			return result;
		}

		private Single GetNextEntryPosition()
		{
			Single result = 0f;
			if (m_entries != null && m_entries.Count > 0)
			{
				Int32 listIndex = m_currentlyVisibleEntry.ListIndex;
				if (listIndex == m_entries.Count - 1)
				{
					result = -m_currentlyVisibleEntry.transform.localPosition.y;
				}
				else
				{
					result = -m_entries[listIndex + 1].transform.localPosition.y + 1f;
					m_currentlyVisibleEntry = m_entries[listIndex + 1];
				}
			}
			return result;
		}

		public void SetVerticalPosition(Single p_yPos)
		{
			TweenPosition.Begin(m_entryHook.gameObject, m_scrollDuration, new Vector3(0f, p_yPos, 0f));
		}

		public void OnSizeChanged()
		{
			Single pixelSizeAdjustment = m_uiRoot.pixelSizeAdjustment;
			Single num = m_buttonChangeSize.DragYTotalDelta * pixelSizeAdjustment;
			m_sizeChangeSaveTimer = m_saveAfterTime;
			Vector4 clipRange = m_questLogPanel.clipRange;
			Single num2 = -num;
			if (clipRange.w + num2 < m_minSize)
			{
				num2 = m_minSize - clipRange.w;
			}
			else if (clipRange.w + num2 > m_maxSize)
			{
				num2 = m_maxSize - clipRange.w;
			}
			if (num2 != 0f)
			{
				clipRange.w += num2;
				clipRange.y = -(clipRange.w / 2f - m_panelOffset);
				m_questLogPanel.clipRange = clipRange;
				m_buttonChangeSize.DragYTotalDelta += num2 / pixelSizeAdjustment;
				UpdateSize();
			}
		}

		public void OnScroll(Single p_delta)
		{
			if (!m_minimized)
			{
				Single scrollPotential = GetScrollPotential(m_questLogPanel.clipRange.w);
				if (p_delta < 0f)
				{
					if (m_scrollBar.barSize < 1f && m_scrollBar.scrollValue < 1f)
					{
						Single nextEntryPosition = GetNextEntryPosition();
						m_scrollBar.scrollValue = nextEntryPosition / scrollPotential;
					}
				}
				else if (p_delta > 0f)
				{
					Single previousEntryPosition = GetPreviousEntryPosition();
					m_scrollBar.scrollValue = previousEntryPosition / scrollPotential;
				}
			}
		}
	}
}
