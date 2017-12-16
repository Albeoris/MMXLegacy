using System;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.EffectEngine;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI;
using Legacy.Loading;
using Legacy.MMInput;
using Legacy.NpcInteraction;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	public class ConversationView : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private DialogView m_conversation;

		[SerializeField]
		private NpcContainerView m_container;

		[SerializeField]
		private GameObject m_veil;

		private String m_currentIndoorScene;

		private Boolean m_finishIndoorSceneLoad;

		private Boolean m_finishFade;

		private IndoorSceneRoot m_indoorModel;

		private Boolean m_InConversation;

		private Boolean m_popupActive;

		public Boolean InConversation => m_InConversation;

	    private void Awake()
		{
			NGUITools.SetActiveSelf(m_veil, false);
			m_popupActive = false;
			IndoorSceneLoader.Instance.FinishLoadIndoorScene += HandleFinishLoadIndoorScene;
			LegacyLogic.Instance.ConversationManager.HideConversationForEntranceEvent += OnConversationHideForEntrance;
			LegacyLogic.Instance.ConversationManager.ChangeConversation += HandleChangeConversation;
			LegacyLogic.Instance.ConversationManager.HideNPCsEvent += OnHideNPCs;
			LegacyLogic.Instance.ConversationManager.ShowNPCsEvent += OnShowNPCs;
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnExitKey));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_1, new EventHandler<HotkeyEventArgs>(SelectMember1KeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_2, new EventHandler<HotkeyEventArgs>(SelectMember2KeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_3, new EventHandler<HotkeyEventArgs>(SelectMember3KeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_4, new EventHandler<HotkeyEventArgs>(SelectMember4KeyPressed));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_TRADE_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_SPELL_TRADE_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_TRADE_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_SPELL_TRADE_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_IDENTIFY_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_IDENTIFY_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_REPAIR_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_REPAIR_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.HIDDEN_INVENTORY_START_TRADE, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.HIDDEN_INVENTORY_STOP_TRADE, new EventHandler(OnTradeStop));
		}

		private void OnDestroy()
		{
			IndoorSceneLoader.Instance.FinishLoadIndoorScene -= HandleFinishLoadIndoorScene;
			LegacyLogic.Instance.ConversationManager.HideConversationForEntranceEvent -= OnConversationHideForEntrance;
			LegacyLogic.Instance.ConversationManager.ChangeConversation -= HandleChangeConversation;
			LegacyLogic.Instance.ConversationManager.HideNPCsEvent -= OnHideNPCs;
			LegacyLogic.Instance.ConversationManager.ShowNPCsEvent -= OnShowNPCs;
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnExitKey));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_1, new EventHandler<HotkeyEventArgs>(SelectMember1KeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_2, new EventHandler<HotkeyEventArgs>(SelectMember2KeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_3, new EventHandler<HotkeyEventArgs>(SelectMember3KeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_4, new EventHandler<HotkeyEventArgs>(SelectMember4KeyPressed));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_TRADE_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_SPELL_TRADE_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_TRADE_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_SPELL_TRADE_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_IDENTIFY_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_IDENTIFY_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_REPAIR_START, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_REPAIR_STOP, new EventHandler(OnTradeStop));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.HIDDEN_INVENTORY_START_TRADE, new EventHandler(OnTradeStart));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.HIDDEN_INVENTORY_STOP_TRADE, new EventHandler(OnTradeStop));
		}

		public void Activate()
		{
		}

		public void Deactivate()
		{
		}

		public void HandleBeginConversation(IngameController main)
		{
			m_popupActive = false;
			m_currentIndoorScene = LegacyLogic.Instance.ConversationManager.IndoorScene;
			if (String.IsNullOrEmpty(m_currentIndoorScene))
			{
				main.Minimap.gameObject.SetActive(false);
				main.HUDControl.gameObject.SetActive(false);
				main.DangerSense.gameObject.SetActive(false);
				m_finishIndoorSceneLoad = true;
				m_finishFade = true;
				m_conversation.FinishFade = true;
				m_conversation.gameObject.SetActive(true);
				m_conversation.Init();
				m_container.Init();
			}
			else
			{
				m_finishIndoorSceneLoad = false;
				m_finishFade = false;
				m_conversation.FinishFade = false;
				m_conversation.gameObject.SetActive(false);
				OverlayManager overlay = IngameController.Instance.Overlay;
				EventHandler callback = delegate(Object sender, EventArgs e)
				{
					main.Minimap.gameObject.SetActive(false);
					main.HUDControl.gameObject.SetActive(false);
					main.DangerSense.gameObject.SetActive(false);
					m_finishFade = true;
					m_conversation.FinishFade = true;
					m_conversation.gameObject.SetActive(true);
					m_conversation.Init();
					m_container.Init();
					IndoorSceneLoader.Instance.RequestIndoorScene(m_currentIndoorScene);
					CheckOverlayTransition();
				};
				overlay.FadeFrontTo(1f, 1f, callback);
			}
			main.ChangeIngameContext(this);
			m_InConversation = true;
		}

		public void HandleEndConversation(IngameController main)
		{
			IndoorSceneRoot currentModel = m_indoorModel;
			if (!String.IsNullOrEmpty(m_currentIndoorScene) && !m_conversation.m_skipFade)
			{
				OverlayManager overlay = IngameController.Instance.Overlay;
				EventHandler callback = delegate(Object sender, EventArgs e)
				{
					main.Minimap.gameObject.SetActive(true);
					main.HUDControl.gameObject.SetActive(true);
					main.DangerSense.gameObject.SetActive(true);
					m_conversation.gameObject.SetActive(false);
					FXMainCamera instance2 = FXMainCamera.Instance;
					instance2.SwitchCamera(null);
					if (currentModel != null)
					{
						currentModel.gameObject.SetActive(false);
						AudioManager.Instance.RequestPlayAudioID(currentModel.ExitAudioID, 0, -1f, instance2.transform, 1f, 0f, 0f, null);
					}
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.INDOOR_SCENE_CLOSED, EventArgs.Empty);
					overlay.FadeFrontTo(0f);
				};
				overlay.FadeFrontTo(1f, 1f, callback);
			}
			else
			{
				main.Minimap.gameObject.SetActive(true);
				main.HUDControl.gameObject.SetActive(true);
				main.DangerSense.gameObject.SetActive(true);
				m_conversation.gameObject.SetActive(false);
				FXMainCamera instance = FXMainCamera.Instance;
				instance.SwitchCamera(null);
				if (currentModel != null && currentModel.gameObject.activeSelf)
				{
					currentModel.gameObject.SetActive(false);
					AudioManager.Instance.RequestPlayAudioID(currentModel.ExitAudioID, 0, -1f, instance.transform, 1f, 0f, 0f, null);
				}
				LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.INDOOR_SCENE_CLOSED, EventArgs.Empty);
			}
			m_conversation.m_skipFade = false;
			m_currentIndoorScene = null;
			m_InConversation = false;
		}

		public void HandleChangeConversation(Object p_sender, EventArgs p_args)
		{
			IngameController main = IngameController.Instance;
			if (String.IsNullOrEmpty(m_currentIndoorScene) || m_indoorModel == null)
			{
				HandleBeginConversation(main);
			}
			else
			{
				m_popupActive = false;
				m_InConversation = true;
				m_currentIndoorScene = LegacyLogic.Instance.ConversationManager.IndoorScene;
				if (String.IsNullOrEmpty(m_currentIndoorScene))
				{
					main.Minimap.gameObject.SetActive(false);
					main.HUDControl.gameObject.SetActive(false);
					main.DangerSense.gameObject.SetActive(false);
					m_finishIndoorSceneLoad = true;
					m_finishFade = true;
					m_conversation.FinishFade = true;
					m_conversation.gameObject.SetActive(true);
					m_conversation.Init();
					m_container.Init();
				}
				else
				{
					m_finishIndoorSceneLoad = false;
					m_finishFade = false;
					m_conversation.FinishFade = false;
					m_conversation.gameObject.SetActive(false);
					OverlayManager overlay = IngameController.Instance.Overlay;
					IndoorSceneRoot currentModel = m_indoorModel;
					EventHandler callback = delegate(Object sender, EventArgs e)
					{
						main.Minimap.gameObject.SetActive(false);
						main.HUDControl.gameObject.SetActive(false);
						main.DangerSense.gameObject.SetActive(false);
						IndoorSceneLoader.Instance.RequestIndoorScene(m_currentIndoorScene);
						m_finishFade = true;
						m_conversation.FinishFade = true;
						m_conversation.gameObject.SetActive(true);
						FXMainCamera instance = FXMainCamera.Instance;
						instance.SwitchCamera(null);
						if (currentModel != null)
						{
							currentModel.gameObject.SetActive(false);
						}
						m_conversation.Init();
						m_container.Init();
					};
					overlay.FadeFrontTo(1f, 1f, callback);
				}
			}
		}

		public void OnTradeStart(Object p_sender, EventArgs p_args)
		{
			NGUITools.SetActiveSelf(m_veil, true);
		}

		public void OnTradeStop(Object p_sender, EventArgs p_args)
		{
			NGUITools.SetActiveSelf(m_veil, false);
		}

		public void OnExitKey(Object p_sender, HotkeyEventArgs p_args)
		{
			ConversationManager conversationManager = LegacyLogic.Instance.ConversationManager;
			if (p_args.KeyDown && conversationManager.IsOpen && m_finishFade && m_finishIndoorSceneLoad)
			{
				if (conversationManager.CurrentNpc != null && conversationManager.CurrentNpc.TradingInventory.IsTrading)
				{
					conversationManager.CurrentNpc.TradingInventory.StopTrading();
				}
				else if (conversationManager.CurrentNpc != null && conversationManager.CurrentNpc.TradingSpells.IsTrading)
				{
					conversationManager.CurrentNpc.TradingSpells.StopTrading();
				}
				else if (conversationManager.CurrentNpc != null && conversationManager.CurrentNpc.IdentifyController.IsIdentifying)
				{
					conversationManager.CurrentNpc.IdentifyController.StopIdentify();
				}
				else if (conversationManager.CurrentNpc != null && conversationManager.CurrentNpc.RepairController.IsRepairing)
				{
					conversationManager.CurrentNpc.RepairController.StopRepair();
				}
				else if (!PopupRequest.Instance.IsActive)
				{
					if (conversationManager.ShowNpcs)
					{
						LegacyLogic.Instance.ConversationManager.CloseNpcContainer(null);
					}
				}
			}
		}

		public void SelectMember1KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && LegacyLogic.Instance.ConversationManager.IsOpen && LegacyLogic.Instance.ConversationManager.ShowNpcs)
			{
				m_container.SelectNPCView(0);
			}
		}

		public void SelectMember2KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && LegacyLogic.Instance.ConversationManager.IsOpen && LegacyLogic.Instance.ConversationManager.ShowNpcs)
			{
				m_container.SelectNPCView(1);
			}
		}

		public void SelectMember3KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && LegacyLogic.Instance.ConversationManager.IsOpen && LegacyLogic.Instance.ConversationManager.ShowNpcs)
			{
				m_container.SelectNPCView(2);
			}
		}

		public void SelectMember4KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && LegacyLogic.Instance.ConversationManager.IsOpen && LegacyLogic.Instance.ConversationManager.ShowNpcs)
			{
				m_container.SelectNPCView(3);
			}
		}

		private void OnConversationHideForEntrance(Object p_sender, EventArgs p_args)
		{
			m_conversation.HideForEntrance();
			m_container.HideForEntrance();
		}

		private void OnHideNPCs(Object p_sender, EventArgs p_args)
		{
			m_container.SetNPCsVisible(false);
		}

		private void OnShowNPCs(Object p_sender, EventArgs p_args)
		{
			m_container.SetNPCsVisible(true);
		}

		private void HandleFinishLoadIndoorScene(Object p_sender, FinishLoadIndoorSceneEventArgs p_e)
		{
			m_indoorModel = p_e.Root;
			m_finishIndoorSceneLoad = true;
			CheckOverlayTransition();
		}

		private void CheckOverlayTransition()
		{
			if (m_finishFade && m_finishIndoorSceneLoad)
			{
				m_conversation.gameObject.SetActive(true);
				m_indoorModel.gameObject.SetActive(true);
				FXMainCamera.Instance.SwitchCamera(m_indoorModel.SceneCamera.gameObject);
				AudioManager.Instance.RequestPlayAudioID(m_indoorModel.EnterAudioID, 0, -1f, m_indoorModel.SceneCamera.transform, 1f, 0f, 0f, null);
				IngameController.Instance.Overlay.FadeFrontTo(0f);
				if (!m_popupActive && LegacyLogic.Instance.ConversationManager.IsForEntrance && LegacyLogic.Instance.ConversationManager.NPCs.Count == 0)
				{
					m_popupActive = true;
					m_conversation.ShowPopup();
				}
			}
		}
	}
}
