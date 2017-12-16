using System;
using System.Collections;
using System.IO;
using Legacy.Audio;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.NpcInteraction;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.PartyManagement;
using Legacy.Core.Quests;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.EffectEngine;
using Legacy.Game.Cheats;
using Legacy.Game.Context;
using Legacy.Game.HUD;
using Legacy.Game.Menus;
using Legacy.Game.MMGUI;
using Legacy.Game.MMGUI.Minimap;
using Legacy.Game.MMGUI.WorldMap;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.IngameManagement
{
	[AddComponentMenu("MM Legacy/IngameManagement/IngameController")]
	public class IngameController : MonoBehaviour
	{
		[SerializeField]
		private UICamera m_camera;

		[SerializeField]
		private GameObject m_uiRoot;

		[SerializeField]
		private IngameMenu m_ingameMenu;

		[SerializeField]
		private OptionsMenu m_optionsMenu;

		[SerializeField]
		private HUDController m_hudController;

		[SerializeField]
		private CheatController m_cheatController;

		[SerializeField]
		private BilateralScreen m_bilateralScreen;

		[SerializeField]
		private OverlayManager m_overlayManager;

		[SerializeField]
		private SpellSelectPartyTarget m_spellPartySelect;

		[SerializeField]
		private Spellbook m_spellBook;

		[SerializeField]
		private ConversationView m_conversationView;

		[SerializeField]
		private Journal m_journal;

		[SerializeField]
		private HelpScreen m_helpScreen;

		[SerializeField]
		private MouseInteraction m_mouseInteraction;

		[SerializeField]
		private MapController m_mapController;

		[SerializeField]
		private PopupRequest m_popupRequest;

		[SerializeField]
		private PopupHint m_popupHint;

		[SerializeField]
		private SpellTradingScreen m_spellTradingScreen;

		[SerializeField]
		private PreconditionPopupConfirm m_preconditionConfirm;

		[SerializeField]
		private PreconditionPopupInput m_preconditionInput;

		[SerializeField]
		private PreconditionPopupWhoWill m_preconditionWhoWill;

		[SerializeField]
		private PreconditionPopupYesNo m_preconditionYesNo;

		[SerializeField]
		private SaveGameMenuController m_saveGameMenueController;

		[SerializeField]
		private MinimapView m_miniMap;

		[SerializeField]
		private UIPanel m_locaTest;

		[SerializeField]
		private HUDDangerSense m_dangerSense;

		[SerializeField]
		private Single m_gameOverDelay = 2f;

		[SerializeField]
		private GameObject m_inventoryVeil;

		[SerializeField]
		private IdentifyScreen m_identifyScreen;

		[SerializeField]
		private SpellSpiritBeaconPopup m_spiritBeaconPopup;

		private IIngameContext m_currentIngameContext;

		private IIngameContext m_previousIngameContext;

		private IngameInput m_ingameInput;

		private LootContainerView m_lootContainerView;

		private Boolean m_contextChanged;

		private Boolean m_canChangeContext;

		private DelayedEventMethod m_delayedEventMethod;

		private Object m_delayedSender;

		private EventArgs m_delayedArgs;

		private Single m_gameOverTimer;

		private Boolean m_gameOver;

		private Boolean m_canOpenCheats;

		private PartyBuff m_requestedCancelBuff;

		private Boolean m_lockedIngameContext;

		private ObeliskFunction m_callingObeliskFunction;

		public static IngameController Instance { get; private set; }

		public IIngameContext CurrentIngameContext => m_currentIngameContext;

	    public MinimapView Minimap => m_miniMap;

	    public HUDDangerSense DangerSense => m_dangerSense;

	    public LootContainerView LootContainerView => m_lootContainerView;

	    public MapController MapController => m_mapController;

	    public IngameInput IngameInput => m_ingameInput;

	    public OverlayManager Overlay => m_overlayManager;

	    public ConversationView ConversationView => m_conversationView;

	    public HUDController HUDControl => m_hudController;

	    public BilateralScreen BilateralScreen => m_bilateralScreen;

	    public PopupRequest PopupRequest => m_popupRequest;

	    private void Awake()
		{
			if (Instance != null)
			{
				return;
			}
			Instance = this;
			DontDestroyOnLoad(gameObject);
			m_ingameInput = new IngameInput(LegacyLogic.Instance.CommandManager, m_mouseInteraction);
			m_lootContainerView = new LootContainerView(m_bilateralScreen);
			ContextManager.OnContextChanging += OnContextChanging;
			m_ingameInput.CancelBackClicked += OnEscapeButton;
			m_ingameInput.OpenSpellbookEvent += ToggleSpellbook;
			m_ingameInput.OpenSkillsEvent += ToggleSkillMenu;
			m_ingameInput.OpenJournalEvent += HandleOpenJournalEvent;
			m_ingameInput.OpenBestiaryEvent += HandleOpenBestiaryEvent;
			m_ingameInput.OpenLoreEvent += HandleOpenLoreEvent;
			m_ingameInput.OpenMapEvent += ToggleWorldMap;
			m_ingameInput.OpenAreaMapEvent += ToggleAreaMap;
			m_ingameInput.OnStartedQuickSave += HandleOnQuickSave;
			m_ingameMenu.OpenOptionsEvent += OnOpenOptionsMenu;
			m_ingameMenu.OpenSaveMenu += OnOpenSaveMenu;
			m_ingameMenu.OpenLoadMenu += OnOpenLoadMenu;
			m_ingameMenu.OpenHelpScreenEvent += OpenHelpScreen;
			m_ingameMenu.CancelBackClicked += OnEscapeButton;
			m_saveGameMenueController.OnClose += OnOpenSaveMenu;
			m_saveGameMenueController.OnSave += HandleOnStartedSave;
			m_bilateralScreen.OccupyScreenSpaceEvent += OnOccupyScreenSpace;
			m_bilateralScreen.ReleaseScreenSpaceEvent += OnReleaseScreenSpace;
			m_spellPartySelect.OpenSpellSelectPartyTarget += OnOpenSpellPartySelect;
			m_spellPartySelect.CloseSpellSelectPartyTarget += OnCloseSpellPartySelect;
			m_popupRequest.OpenPopupRequest += OnOpenPopupRequest;
			m_popupRequest.ClosePopupRequest += OnClosePopupRequest;
			m_spiritBeaconPopup.CloseSpellSpiritBeaconPopup += CloseSpellSpiritBeaconPopup;
			m_canOpenCheats = WorldManager.HasCheatsCheckFile();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SAVEGAME_STARTED_SAVE, new EventHandler(HandleOnStartedSave));
			m_hudController.AddSelectionListener(m_spellPartySelect);
			if (m_canOpenCheats)
			{
				InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CHEAT_MENU, new EventHandler<HotkeyEventArgs>(OnOpenCheatMenu));
			}
			InputManager.RegisterHotkeyEvent(EHotkeyType.MAKE_SCREENSHOT, new EventHandler<HotkeyEventArgs>(OnMakeSceenShot));
			InputManager.RegisterHotkeyEvent(EHotkeyType.DISABLE_GUI, new EventHandler<HotkeyEventArgs>(OnDisableGui));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_LOCA_TEST, new EventHandler<HotkeyEventArgs>(OnOpenLocaTest));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_UPLAY_OVERLAY, new EventHandler<HotkeyEventArgs>(OnOpenUplayOverlay));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_CAST_SPELL_WITH_CHARACTER_SELECTION, new EventHandler(OnOpenSpellPartySelect));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_ROUND_STARTED, new EventHandler(OnMonsterRoundStarted));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_ROUND_FINISHED, new EventHandler(OnMonsterRoundFinished));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAME_LOADED, new EventHandler(OnSaveGameLoaded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.QUESTLOG_SELECTED, new EventHandler(OnQuestLogSelected));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_SELECTED, new EventHandler(OnInventorySelected));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.REQUEST_POPUP_CONFIRM, new EventHandler(OnPreconditionPopupConfirm));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.REQUEST_POPUP_INPUT, new EventHandler(OnPreconditionPopupInput));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.REQUEST_POPUP_WHO_WILL, new EventHandler(OnPreconditionPopupWhoWill));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.REQUEST_POPUP_YES_NO, new EventHandler(OnPreconditionPopupYesNo));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PRECONDITION_SHOW_RESULT, new EventHandler(OnPreconditionResultPopup));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAME_OVER, new EventHandler(OnGameOver));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CONTAINER_SCREEN_OPENED, new EventHandler(OnLootContainerOpen));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CONTAINER_SCREEN_CLOSED, new EventHandler(OnLootContainerClose));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OPEN_LOREBOOK, new EventHandler(OnLorebookSelected));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_REQUEST_BUFF_CANCEL, new EventHandler(OnBuffCancelRequest));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_START_FALLING, new EventHandler(OnPartyStartFalling));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_CAST_SPIRIT_BEACON, new EventHandler(OnCharacterCastSpiritBeacon));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_CAST_IDENTIFY, new EventHandler(OnStartIdentify));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CUTSCENE_STARTED, new EventHandler(HandleCutsceneStartedEvent));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CUTSCENE_STOPPED, new EventHandler(HandleCutsceneStoppedEvent));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SHOW_HINT, new EventHandler(OnShowHint));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CUSTOM_POPUP, new EventHandler(OnCustomPopup));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.REQUEST_POPUP_OBELISK, new EventHandler(OnRequestObelisk));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.ConversationManager.BeginConversation += OnBeginConversation;
			LegacyLogic.Instance.ConversationManager.EndConversation += OnEndConversation;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			m_hudController.Init(party, m_ingameInput);
			if (!LegacyLogic.Instance.WorldManager.IsSaveGame)
			{
				m_bilateralScreen.gameObject.SetActive(true);
				m_bilateralScreen.Init(party);
				m_spellBook.Init(party, this);
			}
			m_journal.Init(this);
			m_journal.Close();
			m_helpScreen.Init();
			m_spellTradingScreen.Init();
			m_identifyScreen.Init();
			NGUITools.SetActiveSelf(m_saveGameMenueController.gameObject, true);
			m_camera.tooltipDelay = Mathf.Abs(ConfigManager.Instance.Options.TooltipDelay);
			m_hudController.ShowScreenSpaceElements(true);
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_INVENTORY, new EventHandler<HotkeyEventArgs>(OnOpenInventory));
			ChangeIngameContext(m_ingameInput);
		}

		private void CleanUp()
		{
			m_ingameInput.CleanUp();
			m_hudController.CleanUp();
			m_bilateralScreen.CleanUp();
			m_journal.Cleanup();
			m_helpScreen.Cleanup();
			m_spellTradingScreen.CleanUp();
			m_identifyScreen.CleanUp();
			m_preconditionConfirm.OnConfirm -= OnPopupConfirmResult;
			m_preconditionConfirm.OnConfirm -= OnPreconditionResultPopupDone;
			m_preconditionInput.OnConfirm -= OnPopupInputResult;
			m_preconditionWhoWill.OnConfirm -= OnPopupWhoWillResult;
			m_preconditionYesNo.OnConfirm -= OnPopupYesNoResult;
			ContextManager.OnContextChanging -= OnContextChanging;
			m_ingameInput.CancelBackClicked -= OnEscapeButton;
			m_ingameInput.OpenMapEvent -= ToggleWorldMap;
			m_ingameInput.OpenAreaMapEvent -= ToggleAreaMap;
			m_ingameInput.OpenSpellbookEvent -= ToggleSpellbook;
			m_ingameInput.OpenSkillsEvent -= ToggleSkillMenu;
			m_ingameInput.OpenJournalEvent -= HandleOpenJournalEvent;
			m_ingameInput.OpenBestiaryEvent -= HandleOpenBestiaryEvent;
			m_ingameInput.OpenLoreEvent -= HandleOpenLoreEvent;
			m_ingameInput.OnStartedQuickSave -= HandleOnQuickSave;
			m_ingameMenu.OpenOptionsEvent -= OnOpenOptionsMenu;
			m_ingameMenu.OpenSaveMenu -= OnOpenSaveMenu;
			m_ingameMenu.OpenLoadMenu -= OnOpenLoadMenu;
			m_ingameMenu.OpenHelpScreenEvent -= OpenHelpScreen;
			m_ingameMenu.CancelBackClicked -= OnEscapeButton;
			m_saveGameMenueController.OnClose -= OnOpenSaveMenu;
			m_saveGameMenueController.OnSave -= HandleOnStartedSave;
			m_bilateralScreen.OccupyScreenSpaceEvent -= OnOccupyScreenSpace;
			m_bilateralScreen.ReleaseScreenSpaceEvent -= OnReleaseScreenSpace;
			m_spellPartySelect.OpenSpellSelectPartyTarget -= OnOpenSpellPartySelect;
			m_spellPartySelect.CloseSpellSelectPartyTarget -= OnCloseSpellPartySelect;
			m_popupRequest.OpenPopupRequest -= OnOpenPopupRequest;
			m_popupRequest.ClosePopupRequest -= OnClosePopupRequest;
			m_spiritBeaconPopup.CloseSpellSpiritBeaconPopup -= CloseSpellSpiritBeaconPopup;
			if (m_canOpenCheats)
			{
				InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CHEAT_MENU, new EventHandler<HotkeyEventArgs>(OnOpenCheatMenu));
			}
			InputManager.UnregisterHotkeyEvent(EHotkeyType.MAKE_SCREENSHOT, new EventHandler<HotkeyEventArgs>(OnMakeSceenShot));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.DISABLE_GUI, new EventHandler<HotkeyEventArgs>(OnDisableGui));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_LOCA_TEST, new EventHandler<HotkeyEventArgs>(OnOpenLocaTest));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_UPLAY_OVERLAY, new EventHandler<HotkeyEventArgs>(OnOpenUplayOverlay));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SAVEGAME_STARTED_SAVE, new EventHandler(HandleOnStartedSave));
			m_hudController.RemoveSelecionListener(m_spellPartySelect);
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_CAST_SPELL_WITH_CHARACTER_SELECTION, new EventHandler(OnOpenSpellPartySelect));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_ROUND_STARTED, new EventHandler(OnMonsterRoundStarted));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_ROUND_FINISHED, new EventHandler(OnMonsterRoundFinished));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAME_LOADED, new EventHandler(OnSaveGameLoaded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.QUESTLOG_SELECTED, new EventHandler(OnQuestLogSelected));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_SELECTED, new EventHandler(OnInventorySelected));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.REQUEST_POPUP_CONFIRM, new EventHandler(OnPreconditionPopupConfirm));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.REQUEST_POPUP_INPUT, new EventHandler(OnPreconditionPopupInput));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.REQUEST_POPUP_WHO_WILL, new EventHandler(OnPreconditionPopupWhoWill));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.REQUEST_POPUP_YES_NO, new EventHandler(OnPreconditionPopupYesNo));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PRECONDITION_SHOW_RESULT, new EventHandler(OnPreconditionResultPopup));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAME_OVER, new EventHandler(OnGameOver));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CONTAINER_SCREEN_OPENED, new EventHandler(OnLootContainerOpen));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CONTAINER_SCREEN_CLOSED, new EventHandler(OnLootContainerClose));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OPEN_LOREBOOK, new EventHandler(OnLorebookSelected));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_REQUEST_BUFF_CANCEL, new EventHandler(OnBuffCancelRequest));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_START_FALLING, new EventHandler(OnPartyStartFalling));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_CAST_SPIRIT_BEACON, new EventHandler(OnCharacterCastSpiritBeacon));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_CAST_IDENTIFY, new EventHandler(OnStartIdentify));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CUTSCENE_STARTED, new EventHandler(HandleCutsceneStartedEvent));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CUTSCENE_STOPPED, new EventHandler(HandleCutsceneStoppedEvent));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SHOW_HINT, new EventHandler(OnShowHint));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CUSTOM_POPUP, new EventHandler(OnCustomPopup));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.REQUEST_POPUP_OBELISK, new EventHandler(OnRequestObelisk));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.ConversationManager.BeginConversation -= OnBeginConversation;
			LegacyLogic.Instance.ConversationManager.EndConversation -= OnEndConversation;
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_INVENTORY, new EventHandler<HotkeyEventArgs>(OnOpenInventory));
			Instance = null;
		}

		private void OnDestroy()
		{
			CleanUp();
		}

		private Boolean IsEventProcessDelayed(Object p_sender, EventArgs p_args, DelayedEventMethod p_delayedEventMethod)
		{
			Boolean flag = m_spellBook.IsOpen && m_spellBook.DelayOtherProcesses(new PopupRequest.RequestCallback(SpellbookCloseCallback));
			Boolean flag2 = m_optionsMenu.IsVisible && m_optionsMenu.DelayOtherProcesses(new PopupRequest.RequestCallback(OptionsCloseCallback));
			if (flag || flag2)
			{
				m_delayedEventMethod = p_delayedEventMethod;
				m_delayedSender = p_sender;
				m_delayedArgs = p_args;
				return true;
			}
			return false;
		}

		private void SpellbookCloseCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				m_spellBook.ConfirmClose();
				m_delayedEventMethod(m_delayedSender, m_delayedArgs);
			}
			m_delayedEventMethod = null;
			m_delayedArgs = null;
			m_delayedSender = null;
		}

		private void OptionsCloseCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				m_optionsMenu.ApplyChanges();
			}
			else
			{
				m_optionsMenu.ReloadConfigs();
			}
			m_optionsMenu.ConfirmClose();
			m_delayedEventMethod(m_delayedSender, m_delayedArgs);
			m_delayedEventMethod = null;
			m_delayedArgs = null;
			m_delayedSender = null;
		}

		private void OnGameOver(Object sender, EventArgs e)
		{
			m_gameOverTimer = 0f;
			m_gameOver = true;
		}

		private void OnOpenSaveMenu(Object sender, EventArgs e)
		{
			if (m_saveGameMenueController.IsVisible || !LegacyLogic.Instance.ConversationManager.IsOpen)
			{
				m_saveGameMenueController.ToggleVisiblity(true);
				m_hudController.ActionBarEnabled = !m_saveGameMenueController.IsVisible;
				if (!m_saveGameMenueController.IsVisible)
				{
					ChangeIngameContext(m_ingameInput);
				}
			}
		}

		private void OnOpenLoadMenu(Object p_sender, EventArgs p_args)
		{
			m_saveGameMenueController.ToggleVisiblity(false);
		}

		private void OnSaveGameLoaded(Object p_sender, EventArgs p_args)
		{
			if (LegacyLogic.Instance.WorldManager.LoadedFromStartMenu)
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				m_bilateralScreen.gameObject.SetActive(true);
				m_bilateralScreen.Init(party);
				m_spellBook.Init(party, this);
				LegacyLogic.Instance.WorldManager.LoadedFromStartMenu = false;
				m_journal.Cleanup();
				m_journal.Init(this);
			}
			else
			{
				ToggleIngameMenu(this, EventArgs.Empty);
				Party party2 = LegacyLogic.Instance.WorldManager.Party;
				m_bilateralScreen.gameObject.SetActive(true);
				m_bilateralScreen.ChangeParty(party2);
				m_spellBook.ChangeParty(party2);
				m_bilateralScreen.gameObject.SetActive(false);
				m_journal.Cleanup();
				m_journal.Init(this);
				ChangeIngameContext(m_ingameInput);
			}
		}

		private void HandleOnStartedSave(Object sender, EventArgs e)
		{
			StartCoroutine(SaveGame());
		}

		private void HandleOnQuickSave(Object sender, EventArgs e)
		{
			StartCoroutine(SaveGame());
		}

		private IEnumerator SaveGame()
		{
			if (LegacyLogic.Instance.WorldManager.CurrentSaveGameType == ESaveGameType.AUTO)
			{
				LegacyLogic.Instance.WorldManager.SaveGameName = LocaManager.GetText("SAVEGAMETYPE_AUTO");
			}
			else if (LegacyLogic.Instance.WorldManager.CurrentSaveGameType == ESaveGameType.QUICK)
			{
				LegacyLogic.Instance.WorldManager.SaveGameName = LocaManager.GetText("SAVEGAMETYPE_QUICK");
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUICK_SAVE, EventArgs.Empty);
				MessageEventArgs args = new MessageEventArgs("GAME_MESSAGE_QUICKSAVE");
				LegacyLogic.Instance.ActionLog.PushEntry(args);
			}
			else
			{
				MessageEventArgs args2 = new MessageEventArgs("GAME_MESSAGE_NORMALSAVE");
				LegacyLogic.Instance.ActionLog.PushEntry(args2);
				LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.GAME_MESSAGE, new GameMessageEventArgs("GAME_MESSAGE_NORMALSAVE", 0f));
			}
			yield return new WaitForEndOfFrame();
			Camera camera = FXMainCamera.Instance.CurrentCamera.camera;
			RenderTexture orginalRT = camera.targetTexture;
			RenderTexture tmpRT = null;
			Texture2D tmpTex = null;
			try
			{
				tmpRT = new RenderTexture(730, 510, 24, RenderTextureFormat.ARGB32);
				camera.targetTexture = tmpRT;
				RenderTexture.active = tmpRT;
				camera.Render();
				tmpTex = new Texture2D(730, 510, TextureFormat.RGB24, false);
				RenderTexture.active = tmpRT;
				tmpTex.ReadPixels(new Rect(0f, 0f, 730f, 510f), 0, 0);
				Byte[] bytes = tmpTex.EncodeToPNG();
				File.WriteAllBytes(Path.Combine(WorldManager.CurrentSaveGameFolder, LegacyLogic.Instance.WorldManager.SaveGameName + ".png"), bytes);
				LegacyLogic.Instance.WorldManager.Save(LegacyLogic.Instance.WorldManager.SaveGameName, bytes);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SAVEGAME_SAVED, EventArgs.Empty);
			}
			finally
			{
				camera.targetTexture = orginalRT;
				RenderTexture.active = null;
				if (tmpRT != null)
				{
					DestroyImmediate(tmpRT);
				}
				if (tmpTex != null)
				{
					DestroyImmediate(tmpTex);
				}
			}
			yield break;
		}

		public void ToggleSpellbook(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(ToggleSpellbook)))
			{
				if (!m_spellBook.IsOpen)
				{
					CloseAllScreens();
				}
				m_spellBook.ToggleSpellview();
				m_hudController.ShowScreenSpaceElements(!IsAnyScreenOpen());
			}
		}

		public void ToggleWorldMap(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(ToggleWorldMap)))
			{
				if (!m_mapController.IsOpen)
				{
					CloseAllScreens();
				}
				m_mapController.ToggleWorldMap();
				m_hudController.ShowScreenSpaceElements(!IsAnyScreenOpen());
			}
		}

		public void ToggleAreaMap(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(ToggleAreaMap)))
			{
				if (!m_mapController.IsOpen)
				{
					CloseAllScreens();
				}
				m_mapController.ToggleAreaMap();
				m_hudController.ShowScreenSpaceElements(!IsAnyScreenOpen());
			}
		}

		public void UpdateLogs()
		{
			m_hudController.ShowScreenSpaceElements(!IsAnyScreenOpen());
		}

		public void OnQuestLogSelected(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(OnQuestLogSelected)) && !m_conversationView.InConversation)
			{
				CloseAllScreens();
				if (!m_journal.IsOpen)
				{
					HandleOpenJournalEvent(this, EventArgs.Empty);
				}
				m_hudController.ShowScreenSpaceElements(false);
				if (p_sender is QuestStep)
				{
					m_journal.GotoQuest((QuestStep)p_sender);
				}
				else
				{
					m_journal.GotoTokens();
				}
			}
		}

		public void OnLorebookSelected(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(OnQuestLogSelected)) && !m_conversationView.InConversation)
			{
				CloseAllScreens();
				if (!m_journal.IsOpen)
				{
					HandleOpenJournalEvent(this, EventArgs.Empty);
				}
				m_hudController.ShowScreenSpaceElements(false);
				m_journal.ToggleLore();
				m_journal.GotoBook((HUDSideInfoBook)p_sender);
			}
		}

		private void OnBuffCancelRequest(Object p_sender, EventArgs p_args)
		{
			PartyBuff partyBuff = p_sender as PartyBuff;
			if (partyBuff != null && m_currentIngameContext == IngameInput)
			{
				String text = LocaManager.GetText(partyBuff.StaticData.Name);
				String text2 = LocaManager.GetText("POPUP_REQUEST_BUFF_CANCEL", text);
				m_requestedCancelBuff = partyBuff;
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.YES_NO, String.Empty, text2, new PopupRequest.RequestCallback(BuffCancelCallback));
			}
		}

		private void OnPartyStartFalling(Object p_sender, EventArgs p_args)
		{
			GameObject currentCamera = FXMainCamera.Instance.CurrentCamera;
			FallingCamera component = currentCamera.transform.parent.GetComponent<FallingCamera>();
			if (component != null)
			{
				component.IsFalling = true;
			}
			FreeRotationCamera component2 = currentCamera.transform.parent.GetComponent<FreeRotationCamera>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
			WalkCameraFX component3 = currentCamera.transform.parent.GetComponent<WalkCameraFX>();
			if (component3 != null)
			{
				component3.enabled = false;
			}
			Int32 num = 0;
			for (Int32 i = 0; i < 4; i++)
			{
				if (LegacyLogic.Instance.WorldManager.Party.GetMember(i).Gender == EGender.MALE)
				{
					num++;
				}
			}
			String text = "fallingDown_";
			if (num > 0)
			{
				text = text + num + "M";
			}
			if (num < 4)
			{
				text = text + (4 - num) + "F";
			}
			AudioManager.Instance.RequestPlayAudioID(text, 100);
		}

		private void BuffCancelCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				LegacyLogic.Instance.WorldManager.Party.Buffs.RemoveBuff(m_requestedCancelBuff.Type);
			}
			m_requestedCancelBuff = null;
		}

		private void OnInventorySelected(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(OnInventorySelected)) && !m_conversationView.InConversation)
			{
				CloseAllScreens();
				if (!m_bilateralScreen.CharacterScreen.IsOpen)
				{
					m_bilateralScreen.ToggleInventory();
					m_bilateralScreen.PartyScreen.OpenInventoryTab();
				}
			}
		}

		public void StartHotkeyInput(IIngameContext p_inputContext)
		{
			ChangeIngameContext(p_inputContext);
		}

		public void StopHotkeyInput()
		{
			ChangeIngameContext(m_ingameInput);
		}

		public void HandleOpenJournalEvent(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(HandleOpenJournalEvent)))
			{
				if (!m_journal.IsOpen)
				{
					CloseAllScreens();
				}
				m_journal.ToggleQuests();
				m_hudController.ShowScreenSpaceElements(!IsAnyScreenOpen());
			}
		}

		public void HandleOpenBestiaryEvent(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(HandleOpenBestiaryEvent)))
			{
				if (!m_journal.IsOpen)
				{
					CloseAllScreens();
				}
				m_journal.ToggleBestiary();
				m_hudController.ShowScreenSpaceElements(!IsAnyScreenOpen());
			}
		}

		public void HandleOpenLoreEvent(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(HandleOpenLoreEvent)))
			{
				if (!m_journal.IsOpen)
				{
					CloseAllScreens();
				}
				m_journal.ToggleLore();
				m_hudController.ShowScreenSpaceElements(!IsAnyScreenOpen());
			}
		}

		public Boolean IsAnyScreenOpen()
		{
			return m_spellBook.IsOpen || m_journal.IsOpen || m_helpScreen.IsOpen || m_mapController.IsOpen || m_bilateralScreen.HasOpenScreens || m_saveGameMenueController.IsVisible || m_optionsMenu.IsVisible || m_ingameMenu.IsVisible || m_identifyScreen.IsActive;
		}

		public Boolean IsAnyIngameMenuSubscreenOpen()
		{
			return m_saveGameMenueController.IsVisible || m_helpScreen.IsOpen || m_optionsMenu.IsVisible;
		}

		public Boolean IsIngameMenuOpen()
		{
			return m_ingameMenu.IsVisible;
		}

		public void CloseAllScreens()
		{
			if (m_spellBook.IsOpen)
			{
				m_spellBook.Close();
			}
			if (m_journal.IsOpen)
			{
				m_journal.OnCloseJournal();
			}
			if (m_mapController.IsOpen)
			{
				m_miniMap.HideAllTooltips();
				m_mapController.Close();
			}
			m_bilateralScreen.CloseScreen();
			if (m_saveGameMenueController.IsVisible)
			{
				m_saveGameMenueController.ToggleVisiblity(false);
				m_hudController.ActionBarEnabled = !m_saveGameMenueController.IsVisible;
			}
			else if (m_optionsMenu.IsVisible)
			{
				m_optionsMenu.Close();
			}
			else if (m_helpScreen.IsOpen)
			{
				m_helpScreen.CloseScreen();
			}
			else if (m_ingameMenu.IsVisible)
			{
				ChangeIngameContext(m_ingameInput);
			}
			else if (m_identifyScreen.IsActive)
			{
				m_identifyScreen.Close(null);
			}
		}

		private void OnMakeSceenShot(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown)
			{
				String text = GamePaths.UserGamePath + "/ScreenShots";
				Directory.CreateDirectory(text);
				DateTime utcNow = DateTime.UtcNow;
				Application.CaptureScreenshot(String.Format("{0}/{1:D04}{2:D02}{3:D02}_{4:D02}{5:D02}{6:D02}_{7}.png", new Object[]
				{
					text,
					utcNow.Year,
					utcNow.Month,
					utcNow.Day,
					utcNow.Hour,
					utcNow.Minute,
					utcNow.Second,
					utcNow.Millisecond
				}));
			}
		}

		private void OnDisableGui(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown)
			{
				EnableGuiCamera(!m_camera.gameObject.activeSelf, true);
			}
		}

		private void OnOpenLocaTest(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown)
			{
				NGUITools.SetActive(m_locaTest.gameObject, !m_locaTest.gameObject.activeSelf);
				m_hudController.ShowScreenSpaceElements(!m_locaTest.gameObject.activeSelf);
			}
		}

		private void OnOpenUplayOverlay(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown)
			{
				LegacyLogic.Instance.ServiceWrapper.ShowOverlay();
			}
		}

		private void OnOpenPopupRequest(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_popupRequest);
		}

		private void OnClosePopupRequest(Object p_sender, EventArgs p_args)
		{
			if (m_currentIngameContext == m_popupRequest)
			{
				ChangeIngameContext(m_previousIngameContext);
			}
		}

		public void EnableGuiCamera(Boolean p_enableGUI, Boolean p_enableInput)
		{
			m_camera.gameObject.SetActive(p_enableGUI);
			if (p_enableInput)
			{
				m_ingameInput.Activate();
			}
			else
			{
				m_ingameInput.Deactivate();
			}
		}

		public void EnableGui(Boolean p_enableGUI, Boolean p_enableInput)
		{
			m_uiRoot.SetActive(p_enableGUI);
			if (p_enableInput)
			{
				m_ingameInput.Activate();
			}
			else
			{
				m_ingameInput.Deactivate();
			}
		}

		private void HandleCutsceneStartedEvent(Object p_sender, EventArgs p_args)
		{
			EnableGuiCamera(false, false);
			EnableGui(false, false);
		}

		private void HandleCutsceneStoppedEvent(Object p_sender, EventArgs p_args)
		{
			Boolean p_enableInput = !(m_currentIngameContext is ConversationView);
			EnableGuiCamera(true, p_enableInput);
			EnableGui(true, p_enableInput);
		}

		public void LockIngameContext()
		{
			m_lockedIngameContext = true;
		}

		public void ReleaseIngameContextLock()
		{
			m_lockedIngameContext = false;
		}

		public void ChangeIngameContext(IIngameContext p_newIngameContext)
		{
			if (p_newIngameContext == null || m_lockedIngameContext)
			{
				return;
			}
			if (p_newIngameContext != m_currentIngameContext)
			{
				m_previousIngameContext = m_currentIngameContext;
			}
			if (m_currentIngameContext != null)
			{
				m_currentIngameContext.Deactivate();
			}
			m_currentIngameContext = p_newIngameContext;
			m_contextChanged = true;
		}

		public void ChangeIngameContextToPrevious()
		{
			ChangeIngameContext(m_previousIngameContext);
		}

		public void Update()
		{
			if (m_gameOver)
			{
				m_gameOverTimer += Time.deltaTime;
				if (m_gameOverTimer > m_gameOverDelay)
				{
					EventHandler callback = delegate(Object sender, EventArgs e)
					{
						FXMainCamera.Instance.gameObject.SetActive(false);
						LegacyLogic.Instance.WorldManager.ClearAndDestroy();
						ContextManager.ChangeContext(EContext.GameOver);
					};
					Overlay.FadeFrontTo(1f, 1f, callback);
					m_gameOver = false;
				}
			}
			if (m_contextChanged && m_canChangeContext)
			{
				m_contextChanged = false;
				m_currentIngameContext.Activate();
			}
			m_ingameInput.Update();
			FXMainCamera instance = FXMainCamera.Instance;
			if (instance != null && instance.CameraRotator != null)
			{
				instance.CameraRotator.EnableMouseEvent = !CheckOpenScreens();
			}
		}

		private void OnLootContainerOpen(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(OnLootContainerOpen)))
			{
				CloseAllScreens();
				if (p_sender is Container)
				{
					m_lootContainerView.SetContainer((Container)p_sender);
					ChangeIngameContext(m_lootContainerView);
					NGUITools.SetActiveSelf(m_inventoryVeil, true);
				}
			}
		}

		public void OnLootContainerClose(Object p_sender, EventArgs p_args)
		{
			if (p_sender is Container)
			{
				ChangeIngameContext(m_ingameInput);
				NGUITools.SetActiveSelf(m_inventoryVeil, false);
			}
		}

		private void OnBeginConversation(Object p_sender, EventArgs p_args)
		{
			CloseAllScreens();
			LegacyLogic.Instance.CommandManager.ClearContiniousQueue();
			m_hudController.ActionBarEnabled = false;
			m_hudController.HideAllTooltips();
			m_conversationView.HandleBeginConversation(this);
			m_miniMap.HideAllTooltips();
		}

		private void OnEndConversation(Object p_sender, EventArgs p_args)
		{
			m_hudController.ActionBarEnabled = true;
			ChangeIngameContext(m_ingameInput);
			m_conversationView.HandleEndConversation(this);
			if (!String.IsNullOrEmpty(LegacyLogic.Instance.ConversationManager.IndoorScene))
			{
				LegacyLogic.Instance.WorldManager.Party.Rotate(2, true);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
			}
		}

		private void OnStartSceneLoad(Object p_sender, EventArgs p_args)
		{
			if (m_bilateralScreen.HasOpenScreens)
			{
				m_bilateralScreen.CloseScreen();
			}
			m_canChangeContext = false;
			m_hudController.HideFlames();
		}

		private void OnFinishSceneLoad(Object p_sender, EventArgs p_args)
		{
			m_canChangeContext = true;
			m_hudController.ShowFlames();
		}

		private void OnMonsterRoundStarted(Object p_sender, EventArgs p_args)
		{
			m_hudController.HideFlames();
		}

		private void OnMonsterRoundFinished(Object p_sender, EventArgs p_args)
		{
			m_hudController.ShowFlames();
		}

		private void OnOpenSpellPartySelect(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(OnOpenSpellPartySelect)))
			{
				if (m_spellBook.IsOpen)
				{
					m_spellBook.Close();
				}
				if (m_bilateralScreen.HasOpenScreens)
				{
					m_bilateralScreen.CloseScreen();
				}
				OpenSpellCharacterSelectionEventArgs openSpellCharacterSelectionEventArgs = (OpenSpellCharacterSelectionEventArgs)p_args;
				m_spellPartySelect.Spell = openSpellCharacterSelectionEventArgs.Spell;
				if (openSpellCharacterSelectionEventArgs.IsScroll)
				{
					m_spellPartySelect.Scroll = openSpellCharacterSelectionEventArgs.Scroll;
				}
				ChangeIngameContext(m_spellPartySelect);
				m_hudController.MovePortraitsInFront();
			}
		}

		private void OnCloseSpellPartySelect(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_ingameInput);
			m_hudController.MovePortraitsBack();
		}

		public void ToggleIngameMenu(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(ToggleIngameMenu)))
			{
				if (!m_ingameMenu.IsVisible)
				{
					LegacyLogic.Instance.CommandManager.ClearContiniousQueue();
					ChangeIngameContext(m_ingameMenu);
				}
				else
				{
					ChangeIngameContext(m_ingameInput);
				}
			}
		}

		public void OnEscapeButton(Object p_sender, EventArgs p_args)
		{
			if (IsAnyScreenOpen())
			{
				CloseAllScreens();
				m_hudController.ShowScreenSpaceElements(true);
			}
			else
			{
				ToggleIngameMenu(this, EventArgs.Empty);
			}
		}

		private void OnOpenOptionsMenu(Object p_sender, EventArgs p_args)
		{
			m_optionsMenu.Open();
		}

		private void OpenHelpScreen(Object p_sender, EventArgs p_args)
		{
			m_helpScreen.Open();
		}

		public void OnOpenCheatMenu(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_cheatController != null && m_ingameInput.Active)
			{
				m_cheatController.ToggleOpenClose();
			}
		}

		public void OnOpenInventory(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_ingameInput.Active)
			{
				ToggleInventory(p_sender, EventArgs.Empty);
			}
		}

		public void ToggleInventory(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(ToggleInventory)))
			{
				if (!m_bilateralScreen.HasOpenScreens)
				{
					CloseAllScreens();
				}
				m_bilateralScreen.ToggleInventory();
			}
		}

		public void OpenAttributePointsWindow(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(OpenAttributePointsWindow)))
			{
				if (!m_bilateralScreen.CharacterScreen.IsOpen)
				{
					if (!m_bilateralScreen.HasOpenScreens)
					{
						CloseAllScreens();
					}
					m_bilateralScreen.ToggleInventory();
				}
				((CharacterHud)p_sender).OnClick();
				m_bilateralScreen.CharacterScreen.OnHudPointsButtonClick();
			}
		}

		public void ToggleSkillMenu(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(ToggleSkillMenu)))
			{
				if (!m_spellBook.IsOpen)
				{
					CloseAllScreens();
				}
				m_spellBook.ToggleSkillview();
			}
		}

		public void OnOccupyScreenSpace(Object p_sender, EventArgs p_args)
		{
			m_hudController.ShowScreenSpaceElements(false);
		}

		public void OnReleaseScreenSpace(Object p_sender, EventArgs p_args)
		{
			m_hudController.ShowScreenSpaceElements(!IsAnyScreenOpen());
		}

		private void OnContextChanging(Object p_sender, ContextChangedEventArgs p_eventArgs)
		{
			if (p_eventArgs.FromContext == EContext.Game)
			{
				Destroy(gameObject);
			}
		}

		private void OnPreconditionPopupConfirm(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_preconditionConfirm);
			m_preconditionConfirm.OnConfirm += OnPopupConfirmResult;
			m_preconditionConfirm.SetArgs(p_args);
		}

		private void OnPopupConfirmResult(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_ingameInput);
			m_preconditionConfirm.OnConfirm -= OnPopupConfirmResult;
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PRECONDITION_GUI_DONE, EventArgs.Empty);
		}

		private void OnPreconditionPopupInput(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_preconditionInput);
			m_preconditionInput.OnConfirm += OnPopupInputResult;
			m_preconditionInput.SetArgs(p_args);
		}

		private void OnPopupInputResult(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_ingameInput);
			m_preconditionInput.OnConfirm -= OnPopupInputResult;
		}

		private void OnPreconditionPopupWhoWill(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_preconditionWhoWill);
			LegacyLogic.Instance.WorldManager.Party.StartTurn();
			m_preconditionWhoWill.OnConfirm += OnPopupWhoWillResult;
			m_preconditionWhoWill.SetArgs(p_args);
			m_hudController.MovePortraitsInFront();
			m_hudController.SetCharacterHudDisabled();
		}

		private void OnPopupWhoWillResult(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_ingameInput);
			m_preconditionWhoWill.OnConfirm -= OnPopupWhoWillResult;
			m_hudController.MovePortraitsBack();
			m_hudController.SetCharacterHudDisabled();
		}

		private void OnPreconditionPopupYesNo(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_preconditionYesNo);
			m_preconditionYesNo.OnConfirm += OnPopupYesNoResult;
			m_preconditionYesNo.SetArgs(p_args);
		}

		private void OnPopupYesNoResult(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_ingameInput);
			m_preconditionYesNo.OnConfirm -= OnPopupYesNoResult;
		}

		private void OnPreconditionResultPopup(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_preconditionConfirm);
			m_preconditionConfirm.OnConfirm += OnPreconditionResultPopupDone;
			m_preconditionConfirm.SetMessageArgs(p_args);
		}

		private void OnPreconditionResultPopupDone(Object p_sender, EventArgs p_args)
		{
			ChangeIngameContext(m_ingameInput);
			m_preconditionConfirm.OnConfirm -= OnPreconditionResultPopupDone;
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PRECONDITION_GUI_DONE, EventArgs.Empty);
		}

		private void OnCustomPopup(Object p_sender, EventArgs p_args)
		{
			CustomPopupEventArgs customPopupEventArgs = (CustomPopupEventArgs)p_args;
			PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CUSTOM_POPUP, customPopupEventArgs.caption, customPopupEventArgs.text, null);
		}

		private void OnCharacterCastSpiritBeacon(Object p_sender, EventArgs p_args)
		{
			if (!IsEventProcessDelayed(p_sender, p_args, new DelayedEventMethod(OnCharacterCastSpiritBeacon)))
			{
				if (m_spellBook.IsOpen)
				{
					m_spellBook.Close();
				}
				if (m_bilateralScreen.HasOpenScreens)
				{
					m_bilateralScreen.CloseScreen();
				}
				SpiritBeaconEventArgs spiritBeaconEventArgs = (SpiritBeaconEventArgs)p_args;
				m_spiritBeaconPopup.Scroll = spiritBeaconEventArgs.Scroll;
				m_spiritBeaconPopup.Spell = (CharacterSpell)p_sender;
				ChangeIngameContext(m_spiritBeaconPopup);
			}
		}

		private void CloseSpellSpiritBeaconPopup(Object sender, EventArgs e)
		{
			ChangeIngameContext(m_ingameInput);
		}

		private void OnStartIdentify(Object sender, EventArgs e)
		{
			if (!(m_currentIngameContext is ConversationView))
			{
				CloseAllScreens();
			}
			IdentifyInventoryController identifyInventoryController = new IdentifyInventoryController(sender);
			identifyInventoryController.StartIdentify();
		}

		private void OnShowHint(Object sender, EventArgs e)
		{
			if (CurrentIngameContext is PopupHint)
			{
				return;
			}
			m_popupHint.SetHint((Hint)sender);
			((Hint)sender).Shown = true;
			ChangeIngameContext(m_popupHint);
		}

		public void HideHint()
		{
			ChangeIngameContext(m_previousIngameContext);
		}

		internal void CloseSavegameMenus()
		{
			if (m_saveGameMenueController.IsVisible)
			{
				m_saveGameMenueController.ToggleVisiblity(false);
			}
			if (m_ingameMenu.IsVisible)
			{
				ChangeIngameContext(m_ingameInput);
			}
		}

		public Boolean CheckOpenScreens()
		{
			return m_bilateralScreen.PartyScreen.IsOpen || m_conversationView.InConversation || m_spellBook.IsOpen || m_journal.IsOpen;
		}

		private void OnRequestObelisk(Object p_sender, EventArgs p_args)
		{
			m_callingObeliskFunction = (ObeliskFunction)p_sender;
			PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.TEXTFIELD_NO_CANCEL, String.Empty, LocaManager.GetText(m_callingObeliskFunction.DialogText), new PopupRequest.RequestCallback(OnObeliskDialogComplete));
		}

		private void OnObeliskDialogComplete(PopupRequest.EResultType p_result, String p_inputString)
		{
			m_callingObeliskFunction.EvaluateUserInput(p_inputString);
		}

		private void OnTokenAdded(Object sender, EventArgs e)
		{
			TokenEventArgs tokenEventArgs = (TokenEventArgs)e;
			if (tokenEventArgs.TokenID == 800)
			{
				LegacyLogic.Instance.WorldManager.SaveCurrentMapData();
				LegacyLogic.Instance.WorldManager.SaveGameName = String.Empty;
				LegacyLogic.Instance.WorldManager.IsSaveGame = false;
				LegacyLogic.Instance.WorldManager.IsShowingDLCEndingSequences = true;
				ContextManager.ChangeContext(EContext.End);
			}
		}

		private delegate void DelayedEventMethod(Object p_sender, EventArgs p_args);
	}
}
