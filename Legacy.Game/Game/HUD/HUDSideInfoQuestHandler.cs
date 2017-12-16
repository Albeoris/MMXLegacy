using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Quests;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDSideInfoQuestHandler")]
	public class HUDSideInfoQuestHandler : HUDSideInfoHandlerBase
	{
		[SerializeField]
		protected GameObject m_QuestEntryPrefab;

		[SerializeField]
		protected GameObject m_BookEntryPrefab;

		[SerializeField]
		protected GameObject m_TokenEntryPrefab;

		private Queue<QueuedQuest> m_questQueue;

		private Queue<LoreBookStaticData> m_loreBookQueue;

		private Queue<TokenStaticData> m_tokenQueue;

		private List<Int32> m_neededTokens = new List<Int32>();

		protected override void Awake()
		{
			base.Awake();
			if (m_QuestEntryPrefab == null || m_BookEntryPrefab == null)
			{
				Debug.LogError("HUDSideInfoQuestHandler: entry prefabs not set!");
			}
			m_questQueue = new Queue<QueuedQuest>();
			m_loreBookQueue = new Queue<LoreBookStaticData>();
			m_tokenQueue = new Queue<TokenStaticData>();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.QUESTLOG_CHANGED, new EventHandler(OnQuestlogChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ADD_LOREBOOK, new EventHandler(OnLorebookAdded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnLoadingScreenDeactivates));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ANNOUNCE_NEEDED_TOKEN, new EventHandler(OnAnnounceNeededToken));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.QUESTLOG_CHANGED, new EventHandler(OnQuestlogChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ADD_LOREBOOK, new EventHandler(OnLorebookAdded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnLoadingScreenDeactivates));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ANNOUNCE_NEEDED_TOKEN, new EventHandler(OnAnnounceNeededToken));
		}

		protected override void Update()
		{
			base.Update();
			if (IsFreeSlot)
			{
				if (m_tokenQueue.Count > 0)
				{
					GameObject gameObject = Helper.Instantiate<GameObject>(m_TokenEntryPrefab);
					HUDSideInfoToken component = gameObject.GetComponent<HUDSideInfoToken>();
					AddEntry(component);
					TokenStaticData tokenStaticData = m_tokenQueue.Dequeue();
					component.Init(tokenStaticData, m_neededTokens.Contains(tokenStaticData.StaticID));
				}
				else if (m_loreBookQueue.Count > 0)
				{
					GameObject gameObject2 = Helper.Instantiate<GameObject>(m_BookEntryPrefab);
					HUDSideInfoBook component2 = gameObject2.GetComponent<HUDSideInfoBook>();
					AddEntry(component2);
					component2.Init(m_loreBookQueue.Dequeue());
				}
				else if (m_questQueue.Count > 0 && IsFreeSlot)
				{
					GameObject gameObject3 = Helper.Instantiate<GameObject>(m_QuestEntryPrefab);
					HUDSideInfoQuest component3 = gameObject3.GetComponent<HUDSideInfoQuest>();
					QueuedQuest queuedQuest = m_questQueue.Dequeue();
					HUDSideInfoQuest.QuestState pState = HUDSideInfoQuest.QuestState.UPDATED;
					QuestChangedEventArgs.Type changeType = queuedQuest.ChangeType;
					if (changeType != QuestChangedEventArgs.Type.NEW_QUEST)
					{
						if (changeType == QuestChangedEventArgs.Type.COMPLETED_QUEST)
						{
							pState = HUDSideInfoQuest.QuestState.COMPLETED;
						}
					}
					else
					{
						pState = HUDSideInfoQuest.QuestState.ADDED;
					}
					if (queuedQuest.Quest.QuestState == EQuestState.SOLVED)
					{
						pState = HUDSideInfoQuest.QuestState.COMPLETED;
					}
					if (m_questQueue.Count > 0)
					{
						QueuedQuest queuedQuest2 = m_questQueue.Peek();
						if (queuedQuest2.Quest.StaticData.FollowupStep == queuedQuest.Quest.StaticData.StaticID)
						{
							m_questQueue.Dequeue();
							pState = HUDSideInfoQuest.QuestState.UPDATED;
						}
						else if (queuedQuest.Quest.StaticData.FollowupStep == queuedQuest2.Quest.StaticData.StaticID)
						{
							queuedQuest = m_questQueue.Dequeue();
							pState = HUDSideInfoQuest.QuestState.UPDATED;
						}
					}
					AddEntry(component3);
					component3.Init(queuedQuest.Quest, pState);
				}
			}
		}

		private void OnQuestlogChanged(Object sender, EventArgs args)
		{
			QuestChangedEventArgs questChangedEventArgs = (QuestChangedEventArgs)args;
			if (questChangedEventArgs.QuestStep != null && (questChangedEventArgs.ChangeType == QuestChangedEventArgs.Type.NEW_QUEST || questChangedEventArgs.ChangeType == QuestChangedEventArgs.Type.COMPLETED_QUEST || questChangedEventArgs.ChangeType == QuestChangedEventArgs.Type.COMPLETED_OBJECTIVE))
			{
				foreach (QueuedQuest queuedQuest in m_questQueue)
				{
					if (queuedQuest.Quest.StaticData.StaticID == questChangedEventArgs.QuestStep.StaticData.StaticID)
					{
						if (queuedQuest.ChangeType == QuestChangedEventArgs.Type.COMPLETED_QUEST || questChangedEventArgs.ChangeType == QuestChangedEventArgs.Type.COMPLETED_QUEST)
						{
							queuedQuest.ChangeType = QuestChangedEventArgs.Type.COMPLETED_QUEST;
						}
						return;
					}
				}
				m_questQueue.Enqueue(new QueuedQuest(questChangedEventArgs.QuestStep, questChangedEventArgs.ChangeType));
			}
		}

		private void OnLorebookAdded(Object p_sender, EventArgs p_args)
		{
			LoreBookStaticData item = p_sender as LoreBookStaticData;
			m_loreBookQueue.Enqueue(item);
		}

		private void OnStartSceneLoad(Object p_sender, EventArgs p_args)
		{
			m_neededTokens.Clear();
			for (Int32 i = 0; i < m_entries.Count; i++)
			{
				if (m_entries[i] is HUDSideInfoToken)
				{
					((HUDSideInfoToken)m_entries[i]).HideToken();
				}
			}
			NGUITools.SetActive(gameObject, false);
		}

		private void OnLoadingScreenDeactivates(Object p_sender, EventArgs p_args)
		{
			List<Int32> collectedTokens = LegacyLogic.Instance.WorldManager.Party.TokenHandler.CollectedTokens;
			foreach (Int32 num in collectedTokens)
			{
				if (m_neededTokens.Contains(num))
				{
					TokenStaticData tokenData = TokenHandler.GetTokenData(num);
					if (tokenData != null && tokenData.TokenVisible)
					{
						m_tokenQueue.Enqueue(TokenHandler.GetTokenData(num));
					}
				}
			}
			NGUITools.SetActive(gameObject, true);
		}

		private void OnTokenAdded(Object p_sender, EventArgs p_args)
		{
			if (p_args is TokenEventArgs)
			{
				TokenEventArgs tokenEventArgs = (TokenEventArgs)p_args;
				if (IsRelevantToken(tokenEventArgs.TokenID))
				{
					TokenStaticData tokenData = TokenHandler.GetTokenData(tokenEventArgs.TokenID);
					if (tokenData.TokenVisible)
					{
						m_tokenQueue.Enqueue(tokenData);
					}
				}
			}
		}

		private void OnAnnounceNeededToken(Object p_sender, EventArgs p_args)
		{
			if (p_args is TokenEventArgs)
			{
				TokenEventArgs tokenEventArgs = (TokenEventArgs)p_args;
				if (IsRelevantToken(tokenEventArgs.TokenID) && !m_neededTokens.Contains(tokenEventArgs.TokenID))
				{
					if (tokenEventArgs.TokenID == 552)
					{
						Boolean flag = false;
						for (Int32 i = 0; i < 4; i++)
						{
							if (LegacyLogic.Instance.WorldManager.Party.GetMember(i).Class.Class == EClass.FREEMAGE)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							return;
						}
					}
					m_neededTokens.Add(tokenEventArgs.TokenID);
				}
			}
		}

		private Boolean IsRelevantToken(Int32 p_id)
		{
			IEnumerator enumerator = Enum.GetValues(typeof(ETokenID)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Object obj = enumerator.Current;
					ETokenID etokenID = (ETokenID)obj;
					if (p_id == (Int32)etokenID)
					{
						return false;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return true;
		}

		private class QueuedQuest
		{
			private QuestStep m_quest;

			private QuestChangedEventArgs.Type m_changeType;

			public QueuedQuest(QuestStep p_quest, QuestChangedEventArgs.Type p_changeType)
			{
				m_quest = p_quest;
				m_changeType = p_changeType;
			}

			public QuestStep Quest => m_quest;

		    public QuestChangedEventArgs.Type ChangeType
			{
				get => m_changeType;
			    set => m_changeType = value;
			}
		}
	}
}
