using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Legacy.Audio;
using Legacy.Configuration;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using Legacy.Game.IngameManagement;
using Legacy.MMGUI;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
    [AddComponentMenu("MM Legacy/MMGUI/NPCConversationView")]
    public class DialogView : MonoBehaviour
    {
        private const String BACKGROUND_43 = "GUI_conversation_background_43";

        private const String BACKGROUND_169 = "GUI_conversation_background_169";

        [SerializeField] private UILabel m_npcName;

        [SerializeField] private MultiTextControl m_text;

        [SerializeField] private UITexture m_textBackground;

        [SerializeField] private UISprite m_npcIcon;

        [SerializeField] private GameObject m_entryList;

        [SerializeField] private GUIMultiSpriteButton m_closeButton;

        [SerializeField] private DialogEntryView[] m_views = Arrays<DialogEntryView>.Empty;

        [SerializeField] private GameObject m_goldRessourceViewGO;

        [SerializeField] private GameObject m_foodRessourceViewGO;

        [SerializeField] private UILabel m_goldRessourceView;

        [SerializeField] private UILabel m_foodRessourceView;

        [LocalResourcePath] [SerializeField] private String m_ConversationBg43;

        [SerializeField] [LocalResourcePath] private String m_ConversationBg169;

        [SerializeField] [LocalResourcePath] private String m_ConversationBg1610;

        private String m_audioIDToPlayAfterLoading;

        private String m_currentVoiceID;

        private AudioRequest m_currentAudioRequest;

        private AudioObject m_currentAudioObject;

        private Int32 m_currentDialogID;

        private Npc m_npc;

        private Boolean m_isHidden;

        private Npc m_fakeNPC;

        private Boolean m_hideBackButton;

        private Boolean m_finishFade;

        private Boolean m_needGold;

        private Boolean m_needFood;

        public Boolean m_skipFade;

        private NpcContainer m_container;

        public NpcContainer CurrentNPCContainer
        {
            get => m_container;
            set => m_container = value;
        }

        public Boolean FinishFade
        {
            get => m_finishFade;
            set => m_finishFade = value;
        }

        private void Awake()
        {
            OnResolutionChange();
            LegacyLogic.Instance.ConversationManager.DialogChanged += HandleDialogChanged;
            InputManager.RegisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_1, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            InputManager.RegisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_2, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            InputManager.RegisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_3, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            InputManager.RegisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_4, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            InputManager.RegisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_5, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnResourceChanged));
            LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnLoadingScreenDeactivates));
        }

        private void OnDestroy()
        {
            LegacyLogic.Instance.ConversationManager.DialogChanged -= HandleDialogChanged;
            InputManager.UnregisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_1, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            InputManager.UnregisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_2, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            InputManager.UnregisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_3, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            InputManager.UnregisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_4, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            InputManager.UnregisterHotkeyEvent(EHotkeyType.DIALOG_OPTION_5, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
            LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnResourceChanged));
            LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnLoadingScreenDeactivates));
            StopVoiceOver();
        }

        private void OnQuickSlotAction(Object sender, HotkeyEventArgs e)
        {
            Npc currentNpc = LegacyLogic.Instance.ConversationManager.CurrentNpc;
            if (currentNpc != null && !m_isHidden && gameObject.activeSelf && !currentNpc.TradingInventory.IsTrading && !currentNpc.TradingSpells.IsTrading && !currentNpc.IdentifyController.IsIdentifying && !currentNpc.RepairController.IsRepairing && e.KeyDown && e.Action >= EHotkeyType.DIALOG_OPTION_1 && e.Action <= EHotkeyType.DIALOG_OPTION_5)
            {
                Int32 num = e.Action - EHotkeyType.DIALOG_OPTION_1;
                if (m_views[num].IsVisible)
                {
                    m_views[num].OnClickedEntry(null);
                }
            }
        }

        public void Init()
        {
            m_npc = null;
            m_currentDialogID = -1;
            m_npc = LegacyLogic.Instance.ConversationManager.CurrentNpc;
            if (m_npc == null)
            {
                return;
            }
            m_currentDialogID = ((LegacyLogic.Instance.ConversationManager.StartDialogID != -1) ? LegacyLogic.Instance.ConversationManager.StartDialogID : m_npc.ConversationData.RootDialog.ID);
            if (LegacyLogic.Instance.ConversationManager.OverrideStartID > 0)
            {
                m_currentDialogID = LegacyLogic.Instance.ConversationManager.OverrideStartID;
                LegacyLogic.Instance.ConversationManager.OverrideStartID = -1;
            }
            Dialog dialog;
            if (m_currentDialogID == -1)
            {
                dialog = m_npc.ConversationData.RootDialog;
            }
            else
            {
                dialog = m_npc.ConversationData[m_currentDialogID];
                m_hideBackButton = dialog.HideBackButton;
            }
            m_fakeNPC = ((dialog.FakeNpcID == 0) ? null : LegacyLogic.Instance.WorldManager.NpcFactory.Get(dialog.FakeNpcID));
            m_npcName.text = ((m_fakeNPC != null) ? LocaManager.GetText(m_fakeNPC.StaticData.NameKey) : LocaManager.GetText(m_npc.StaticData.NameKey));
            dialog.CheckDialog(m_npc);
            m_text.SetInternalText(dialog.Text);
            InitEntries(dialog, true);
            InitVoiceOver(dialog.DialogText.VoiceID);
            m_text.transform.localPosition = new Vector3(m_text.transform.localPosition.x, m_text.transform.localPosition.y, -1f);
            OnResolutionChange();
            m_closeButton.UpdateColor(m_closeButton.IsEnabled, true);
        }

        private void OnResolutionChange()
        {
            Texture2D texture2D;
            switch (GraphicsConfigManager.GetAspectRatio())
            {
                case EAspectRatio._16_9:
                    texture2D = LocalResourcePathAttribute.LoadAsset<Texture2D>(m_ConversationBg169);
                    goto IL_57;
                case EAspectRatio._16_10:
                    texture2D = LocalResourcePathAttribute.LoadAsset<Texture2D>(m_ConversationBg1610);
                    goto IL_57;
            }
            texture2D = LocalResourcePathAttribute.LoadAsset<Texture2D>(m_ConversationBg43);
            IL_57:
            if (m_textBackground.mainTexture != texture2D)
            {
                Texture mainTexture = m_textBackground.mainTexture;
                m_textBackground.mainTexture = texture2D;
                if (mainTexture != null)
                {
                    mainTexture.UnloadAsset();
                }
                m_textBackground.MakePixelPerfect();
            }
        }

        private void OnResourceChanged(Object p_sender, EventArgs p_args)
        {
            m_goldRessourceView.text = LegacyLogic.Instance.WorldManager.Party.Gold.ToString();
        }

        private void OnLoadingScreenDeactivates(Object p_sender, EventArgs p_args)
        {
            if (!String.IsNullOrEmpty(m_audioIDToPlayAfterLoading))
            {
                PlayVoiceOver(m_audioIDToPlayAfterLoading);
                m_audioIDToPlayAfterLoading = null;
            }
        }

        private void HandleDialogChanged(Object p_sender, ChangedDialogEventArgs p_args)
        {
            m_npc = LegacyLogic.Instance.ConversationManager.CurrentNpc;
            m_fakeNPC = ((p_args.Dialog.FakeNpcID == 0) ? null : LegacyLogic.Instance.WorldManager.NpcFactory.Get(p_args.Dialog.FakeNpcID));
            m_npcName.text = ((m_fakeNPC != null) ? LocaManager.GetText(m_fakeNPC.StaticData.NameKey) : LocaManager.GetText(m_npc.StaticData.NameKey));
            m_text.SetInternalText(p_args.Dialog.Text);
            m_currentDialogID = p_args.Dialog.ID;
            m_hideBackButton = p_args.Dialog.HideBackButton;
            InitEntries(p_args.Dialog, m_text.AtEnd());
            InitVoiceOver(p_args.Dialog.DialogText.VoiceID);
            if (!m_text.AtEnd())
            {
                AddNextPageButton();
            }
            if (p_args.Dialog.ID != m_npc.ConversationData.RootDialog.ID)
            {
                UpdateBackButton(m_text.AtEnd());
            }
            else
            {
                UpdateBackButton(false);
            }
        }

        private void AddNextPageButton()
        {
            if (!m_text.AtEnd())
            {
                DialogEntryView dialogEntryView = m_views[0];
                if (dialogEntryView.IsNextPageButton)
                {
                    return;
                }
                dialogEntryView.SetNextPageFunction();
                dialogEntryView.ClickedNextPageButton += OnClickedNextPageButton;
                NGUITools.SetActive(dialogEntryView.gameObject, true);
            }
            else
            {
                for (Int32 i = 0; i < m_views.Length; i++)
                {
                    if (m_views[i].IsNextPageButton)
                    {
                        m_views[i].Reset();
                    }
                }
            }
        }

        private void OnClickedNextPageButton(Object p_sender, EventArgs p_e)
        {
            DialogEntryView dialogEntryView = (DialogEntryView)p_sender;
            dialogEntryView.ClickedNextPageButton -= OnClickedNextPageButton;
            m_text.NextPage();
            dialogEntryView.Reset();
            if (m_text.AtEnd())
            {
                for (Int32 i = 0; i < m_views.Length; i++)
                {
                    m_views[i].Reset();
                }
                BuildEntries(m_npc.ConversationData[m_currentDialogID]);
                for (Int32 j = 0; j < m_views.Length; j++)
                {
                    if (!m_views[j].IsVisible)
                    {
                        if (!m_hideBackButton)
                        {
                            m_views[j].SetBackFunction(m_npc.ConversationData[m_currentDialogID].BackToDialogId);
                            m_views[j].ClickedBackButton += OnClickedBackButton;
                            NGUITools.SetActive(m_views[j].gameObject, true);
                        }
                        else
                        {
                            NGUITools.SetActive(m_views[j].gameObject, false);
                        }
                        break;
                    }
                    NGUITools.SetActive(m_views[j].gameObject, true);
                }
            }
            else
            {
                UpdateBackButton(m_text.AtEnd());
            }
            if (!String.IsNullOrEmpty(m_currentVoiceID))
            {
                PlayVoiceOverLoadingSafe(m_currentVoiceID + "_part" + (m_text.CurrentPage + 1));
            }
            else
            {
                StopVoiceOver();
            }
        }

        private void OnEnable()
        {
            if (m_goldRessourceView != null && m_foodRessourceView != null)
            {
                m_goldRessourceView.text = LegacyLogic.Instance.WorldManager.Party.Gold.ToString();
                m_foodRessourceView.text = LegacyLogic.Instance.WorldManager.Party.Supplies.ToString();
            }
        }

        private void OnDisable()
        {
            StopVoiceOver();
        }

        private void UpdateBackButton(Boolean p_visible)
        {
            if (m_hideBackButton)
            {
                p_visible = false;
            }
            AddNextPageButton();
            if (p_visible)
            {
                Int32 num = 0;
                Boolean flag = false;
                switch (m_npc.ConversationData[m_currentDialogID].Feature.m_type)
                {
                    case EDialogFeature.NONE:
                        for (Int32 i = 0; i < m_views.Length; i++)
                        {
                            if (m_views[0].IsNextPageButton)
                            {
                                num = -1;
                                break;
                            }
                            if (!flag)
                            {
                                if (!m_views[i].IsVisible || m_views[i].IsBackButton)
                                {
                                    num = i;
                                    flag = true;
                                }
                            }
                            else
                            {
                                if (m_views[i].IsBackButton)
                                {
                                    m_views[i].ClickedBackButton -= OnClickedBackButton;
                                }
                                m_views[i].Reset();
                            }
                        }
                        break;
                    case EDialogFeature.TRAINING:
                    case EDialogFeature.CURE:
                    case EDialogFeature.RESURRECT:
                    case EDialogFeature.SPELL:
                    case EDialogFeature.WHO_WILL:
                    case EDialogFeature.RESPEC:
                        num = 4;
                        break;
                    case EDialogFeature.REPAIR:
                    case EDialogFeature.FIRE:
                    case EDialogFeature.REST:
                    case EDialogFeature.SUPPLIES:
                    case EDialogFeature.RESTORE:
                    case EDialogFeature.CAST:
                    case EDialogFeature.TRAVEL:
                    case EDialogFeature.IDENTIFY:
                    case EDialogFeature.BUY_TOKEN:
                    case EDialogFeature.USER_INPUT:
                        num = 1;
                        break;
                    case EDialogFeature.HIRE:
                    case EDialogFeature.KARTHAL_UNDER_SIEGE:
                    {
                        Party party = LegacyLogic.Instance.WorldManager.Party;
                        num = 1;
                        if (!party.HirelingHandler.HasFreeSlot())
                        {
                            if (party.HirelingHandler.Hirelings[0].CanBeFired)
                            {
                                num++;
                            }
                            if (party.HirelingHandler.Hirelings[1].CanBeFired)
                            {
                                num++;
                            }
                        }
                        break;
                    }
                    default:
                        num = m_npc.ConversationData[m_currentDialogID].Entries.Length;
                        break;
                }
                if (num > 4)
                {
                    Debug.LogError("Too many Entries in Dialog");
                    return;
                }
                if (num != -1)
                {
                    DialogEntryView dialogEntryView = m_views[num];
                    dialogEntryView.SetBackFunction(m_npc.ConversationData[m_currentDialogID].BackToDialogId);
                    dialogEntryView.ClickedBackButton += OnClickedBackButton;
                    NGUITools.SetActive(dialogEntryView.gameObject, true);
                }
            }
        }

        public void OnClickedBackButton(Object p_sender, EventArgs p_args)
        {
            ((DialogEntryView)p_sender).ClickedBackButton -= OnClickedBackButton;
            Int32 backToDialogId = ((DialogEntryView)p_sender).BackToDialogId;
            if (backToDialogId == 0)
            {
                LegacyLogic.Instance.ConversationManager.OpenDialog(m_npc);
            }
            else
            {
                LegacyLogic.Instance.ConversationManager.SwitchToDialogId(backToDialogId);
            }
        }

        private void InitEntries(Dialog p_dialog, Boolean p_lastPage)
        {
            m_text.Show();
            NGUITools.SetActive(m_npcName.gameObject, true);
            ClearEntries();
            m_needGold = false;
            m_needFood = false;
            if (m_npc.ConversationData[m_currentDialogID].HideNpcsAndCloseButton)
            {
                m_closeButton.IsEnabled = false;
                LegacyLogic.Instance.ConversationManager._HideNPCs();
            }
            else
            {
                m_closeButton.IsEnabled = true;
                LegacyLogic.Instance.ConversationManager._ShowNPCs();
            }
            if (p_lastPage)
            {
                if (p_dialog.Feature.m_type == EDialogFeature.NONE)
                {
                    if (m_text.AtEnd())
                    {
                        BuildEntries(p_dialog);
                    }
                }
                else
                {
                    InitFeatureDialog(p_dialog);
                }
            }
            m_isHidden = false;
            Show();
        }

        private void BuildEntries(Dialog p_dialog)
        {
            Int32 num = 0;
            if (p_dialog.Feature.m_type == EDialogFeature.NONE)
            {
                for (Int32 i = 0; i < p_dialog.Entries.Length; i++)
                {
                    DialogEntry dialogEntry = p_dialog.Entries[i];
                    if (dialogEntry.State != EDialogState.HIDDEN)
                    {
                        DialogEntryView dialogEntryView = m_views[num];
                        dialogEntryView.SetEntry(dialogEntry, gameObject);
                        num++;
                        if (!m_needGold)
                        {
                            m_needGold = dialogEntry.NeedGold();
                        }
                        if (!m_needFood)
                        {
                            m_needFood = dialogEntry.NeedFood();
                        }
                        dialogEntry.NotifyShow(LocaManager.GetText);
                    }
                }
            }
            else
            {
                InitFeatureDialog(p_dialog);
            }
        }

        private void InitFeatureDialog(Dialog p_dialog)
        {
            switch (p_dialog.Feature.m_type)
            {
                case EDialogFeature.TRAINING:
                    InitTrainingDialog(p_dialog);
                    break;
                case EDialogFeature.REPAIR:
                    InitRepairDialog(p_dialog);
                    break;
                case EDialogFeature.HIRE:
                    InitHireDialog(p_dialog);
                    break;
                case EDialogFeature.FIRE:
                    InitFireDialog(p_dialog);
                    break;
                case EDialogFeature.REST:
                    InitRestDialog(p_dialog);
                    break;
                case EDialogFeature.SUPPLIES:
                    InitSuppliesDialog(p_dialog);
                    break;
                case EDialogFeature.CURE:
                    InitCureDialog(p_dialog);
                    break;
                case EDialogFeature.RESURRECT:
                    InitResurrectDialog(p_dialog);
                    break;
                case EDialogFeature.RESTORE:
                    InitRestoreDialog(p_dialog);
                    break;
                case EDialogFeature.CAST:
                    InitCastDialog(p_dialog);
                    break;
                case EDialogFeature.TRAVEL:
                    InitTravelDialog(p_dialog);
                    break;
                case EDialogFeature.SPELL:
                    InitSpellDialog(p_dialog);
                    break;
                case EDialogFeature.IDENTIFY:
                    InitIdentifyDialog(p_dialog);
                    break;
                case EDialogFeature.WHO_WILL:
                    InitWhoWillDialog(p_dialog);
                    break;
                case EDialogFeature.BUY_TOKEN:
                    InitBuyTokenDialog(p_dialog);
                    break;
                case EDialogFeature.USER_INPUT:
                    InitUserInputDialog(p_dialog);
                    break;
                case EDialogFeature.KARTHAL_UNDER_SIEGE:
                    InitKarthalUnderSiegeDialog(p_dialog);
                    break;
                case EDialogFeature.RESPEC:
                    InitRespecDialog(p_dialog);
                    break;
            }
        }

        private void InitTravelDialog(Dialog p_dialog)
        {
            TravelFunction.RaiseEventShow(LocaManager.GetText, p_dialog.Feature.m_mapName);

            Party party = LegacyLogic.Instance.WorldManager.Party;
            DialogEntryView dialogEntryView = m_views[0];
            Boolean p_enabled = true;
            Int32 num = p_dialog.Feature.m_price;
            if (num == -1)
            {
                num = ConfigManager.Instance.Game.CostTravel;
            }
            Int32 price = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcTravelFactor);
            String text = LocaManager.GetText(p_dialog.Feature.m_optionKey);
            if (party.Gold < price)
            {
                p_enabled = false;
            }
            TravelFunction p_function = new TravelFunction(price, p_dialog.Feature.m_mapName, p_dialog.Feature.m_targetSpawnId);
            dialogEntryView.SetFeature(text, p_enabled, p_function);
            m_needGold = true;
        }

        private void InitBuyTokenDialog(Dialog p_dialog)
        {
            Party party = LegacyLogic.Instance.WorldManager.Party;
            DialogEntryView dialogEntryView = m_views[0];
            Boolean p_enabled = true;
            Int32 price = p_dialog.Feature.m_price;
            String text = LocaManager.GetText(p_dialog.Feature.m_optionKey);
            if (party.Gold < price)
            {
                p_enabled = false;
            }
            BuyTokenFunction p_function = new BuyTokenFunction(LegacyLogic.Instance.ConversationManager.CurrentConversation.RootDialog.ID, price, p_dialog.Feature.m_tokenID);
            dialogEntryView.SetFeature(text, p_enabled, p_function);
            m_needGold = true;
        }

        private void InitIdentifyDialog(Dialog p_dialog)
        {
            IdentifyFunction.RaiseEventShow(LocaManager.GetText);

            Party party = LegacyLogic.Instance.WorldManager.Party;
            DialogEntryView dialogEntryView = m_views[0];
            Boolean p_enabled = true;
            Int32 num = p_dialog.Feature.m_price;
            if (num == -1)
            {
                num = ConfigManager.Instance.Game.CostIdentify;
            }
            Int32 num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcItemIdentifyFactor);
            String text = LocaManager.GetText("DIALOG_OPTION_IDENTIFY");
            Boolean flag = party.HirelingHandler.Hirelings[0] == LegacyLogic.Instance.ConversationManager.CurrentNpc || party.HirelingHandler.Hirelings[1] == LegacyLogic.Instance.ConversationManager.CurrentNpc;
            if (flag)
            {
                if (party.Gold < LegacyLogic.Instance.ConversationManager.CurrentNpc.GetCosts(ETargetCondition.IDENTIFY))
                {
                    p_enabled = false;
                    text = text + " " + LocaManager.GetText("NOT_ENOUGH_GOLD");
                }
            }
            else if (party.Gold < num2)
            {
                p_enabled = false;
                text = text + " " + LocaManager.GetText("NOT_ENOUGH_GOLD");
            }
            if (!party.Inventory.HasUnidentifiedItems() && !party.MuleInventory.HasUnidentifiedItems())
            {
                p_enabled = false;
                text = LocaManager.GetText("CONVERSATION_IDENTIFY_EQUIPMENT_FAILED");
            }
            IdentifyFunction p_function = new IdentifyFunction(LegacyLogic.Instance.ConversationManager.CurrentConversation.RootDialog.ID, num2);
            dialogEntryView.SetFeature(text, p_enabled, p_function);
            m_needGold = true;
        }

        private void InitWhoWillDialog(Dialog p_dialog)
        {
            Party party = LegacyLogic.Instance.WorldManager.Party;
            for (Int32 i = 0; i < party.Members.Length; i++)
            {
                Character memberByOrder = party.GetMemberByOrder(i);
                if (memberByOrder != null)
                {
                    DialogEntryView dialogEntryView = m_views[i];
                    String name = memberByOrder.Name;
                    Boolean p_enabled = true;
                    if (memberByOrder.ConditionHandler.HasOneCondition(ECondition.DEAD | ECondition.UNCONSCIOUS | ECondition.PARALYZED | ECondition.SLEEPING))
                    {
                        p_enabled = false;
                    }
                    WhoWillFunction p_function = new WhoWillFunction(memberByOrder, p_dialog.Feature.m_characterAttribute, p_dialog.Feature.m_minimumValue, p_dialog.Feature.m_successDialogID, p_dialog.Feature.m_failDialogID);
                    dialogEntryView.SetFeature(name, p_enabled, p_function);
                }
            }
            m_needGold = false;
        }

        private void InitUserInputDialog(Dialog p_dialog)
        {
            m_hideBackButton = true;
            String text = p_dialog.Feature.m_popupText;
            if (String.IsNullOrEmpty(text))
            {
                text = "DIALOG_USER_INPUT_POPUP_TEXT_DEFAULT";
            }
            PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.TEXTFIELD_NO_CANCEL, String.Empty, LocaManager.GetText(text), new PopupRequest.RequestCallback(OnUserInputComplete));
        }

        private void OnUserInputComplete(PopupRequest.EResultType p_result, String p_inputString)
        {
            m_hideBackButton = false;
            Dialog dialog = m_npc.ConversationData[m_currentDialogID];
            String[] array = new String[dialog.Feature.m_inputSolutions.Length];
            for (Int32 i = 0; i < dialog.Feature.m_inputSolutions.Length; i++)
            {
                array[i] = LocaManager.GetText(dialog.Feature.m_inputSolutions[i].m_answerLocaKey).ToUpper();
            }
            UserInputFunction userInputFunction = new UserInputFunction(dialog.Feature.m_successDialogID, dialog.Feature.m_failDialogID, array, p_inputString);
            userInputFunction.EvaluateUserInput();
        }

        private void InitRepairDialog(Dialog p_dialog)
        {
            RepairFunction.RaiseEventShow(LocaManager.GetText, p_dialog.Feature.m_repairType);

            Party party = LegacyLogic.Instance.WorldManager.Party;
            Boolean p_enabled = true;
            Int32 brokenItemCount = GetBrokenItemCount(p_dialog.Feature.m_repairType);
            Int32 repairCost = p_dialog.Feature.m_price;
            if (repairCost == -1)
                repairCost = ConfigManager.Instance.Game.CostRepair;

            Int32 repairCostCoeff = CalculatePriceForDifficulty(repairCost, ConfigManager.Instance.Game.NpcItemRepairFactor);
            Int32 totalRepairCost = repairCostCoeff * brokenItemCount;
            DialogEntryView dialogEntryView = m_views[0];
            String text = LocaManager.GetText("CONVERSATION_REPAIRALL_ENTRY_TEXT", totalRepairCost);
            ERepairType repairType = p_dialog.Feature.m_repairType;
            if (repairType != ERepairType.WEAPONS)
            {
                if (repairType == ERepairType.ARMOR_AND_SHIELD)
                {
                    text = LocaManager.GetText("CONVERSATION_REPAIRARMOR_ENTRY_TEXT", totalRepairCost);
                }
            }
            else
            {
                text = LocaManager.GetText("CONVERSATION_REPAIRWEAPONS_ENTRY_TEXT", totalRepairCost);
            }
            Boolean flag = party.HirelingHandler.Hirelings[0] == LegacyLogic.Instance.ConversationManager.CurrentNpc || party.HirelingHandler.Hirelings[1] == LegacyLogic.Instance.ConversationManager.CurrentNpc;
            if (flag)
            {
                if (party.Gold < LegacyLogic.Instance.ConversationManager.CurrentNpc.GetCosts(ETargetCondition.REPAIR))
                {
                    p_enabled = false;
                    text = text + " " + LocaManager.GetText("NOT_ENOUGH_GOLD");
                }
            }
            else if (party.Gold < repairCostCoeff)
            {
                p_enabled = false;
                text = text + " " + LocaManager.GetText("NOT_ENOUGH_GOLD");
            }
            if (!HasBrokenItems(party.Inventory.Inventory, p_dialog.Feature.m_repairType) && !HasBrokenItems(party.MuleInventory.Inventory, p_dialog.Feature.m_repairType))
            {
                Boolean flag2 = false;
                for (Int32 i = 0; i < party.Members.Length; i++)
                {
                    if (HasBrokenItems(party.Members[i].Equipment.Equipment, p_dialog.Feature.m_repairType))
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    p_enabled = false;
                    text = LocaManager.GetText("CONVERSATION_REPAIRWEAPONS_FAILED_NO_BROKEN_ITEMS");
                }
            }
            RepairFunction p_function = new RepairFunction(p_dialog.Feature.m_repairType, (!flag) ? repairCostCoeff : LegacyLogic.Instance.ConversationManager.CurrentNpc.GetCosts(ETargetCondition.REPAIR), LegacyLogic.Instance.ConversationManager.CurrentConversation.RootDialog.ID);
            dialogEntryView.SetFeature(text, p_enabled, p_function);
            m_needGold = true;
        }

        private Boolean HasBrokenItems(Inventory p_inventory, ERepairType p_repairType)
        {
            if (p_repairType == ERepairType.WEAPONS)
            {
                return p_inventory.HasBrokenWeapons();
            }
            if (p_repairType != ERepairType.ARMOR_AND_SHIELD)
            {
                return p_inventory.HasBrokenItems();
            }
            return p_inventory.HasBrokenArmors();
        }

        private Int32 GetBrokenItemCount(ERepairType p_repairType)
        {
            Int32 num = 0;
            Party party = LegacyLogic.Instance.WorldManager.Party;
            num += GetBrokenItemCount(party.Inventory.Inventory, p_repairType);
            num += GetBrokenItemCount(party.MuleInventory.Inventory, p_repairType);
            for (Int32 i = 0; i < party.Members.Length; i++)
            {
                num += GetBrokenItemCount(party.Members[i].Equipment.Equipment, p_repairType);
            }
            return num;
        }

        private Int32 GetBrokenItemCount(Inventory p_inventory, ERepairType p_repairType)
        {
            if (p_repairType == ERepairType.WEAPONS)
            {
                return p_inventory.GetBrokenWeaponsCount();
            }
            if (p_repairType != ERepairType.ARMOR_AND_SHIELD)
            {
                return p_inventory.GetBrokenItemsCount();
            }
            return p_inventory.GetBrokenArmorsCount();
        }

        private void InitFireDialog(Dialog p_dialog)
        {
            Party party = LegacyLogic.Instance.WorldManager.Party;
            Boolean p_enabled = true;
            Npc currentNpc = LegacyLogic.Instance.ConversationManager.CurrentNpc;
            DialogEntryView dialogEntryView = m_views[0];
            String text = LocaManager.GetText("CONVERSATION_FIRE_ENTRY_TEXT", LocaManager.GetText(currentNpc.StaticData.NameKey));
            if (party.HirelingHandler.Hirelings[0] != LegacyLogic.Instance.ConversationManager.CurrentNpc && party.HirelingHandler.Hirelings[1] != LegacyLogic.Instance.ConversationManager.CurrentNpc)
            {
                p_enabled = false;
            }
            HirelingFunction p_function = new HirelingFunction(ETargetCondition.FIRE, p_dialog.Feature.m_dialogID);
            dialogEntryView.SetFeature(text, p_enabled, p_function);
        }

        private void InitHireDialog(Dialog p_dialog)
        {
            Party party = LegacyLogic.Instance.WorldManager.Party;
            Boolean flag = true;
            Npc npc = (p_dialog.Feature.m_npcID != 0) ? LegacyLogic.Instance.WorldManager.NpcFactory.Get(p_dialog.Feature.m_npcID) : LegacyLogic.Instance.ConversationManager.CurrentNpc;
            DialogEntryView dialogEntryView = m_views[0];
            Int32 hirePrice = npc.GetHirePrice();
            String text;
            if (String.IsNullOrEmpty(p_dialog.Feature.m_optionKey))
            {
                text = LocaManager.GetText("CONVERSATION_HIRE_ENTRY_TEXT", LocaManager.GetText(npc.StaticData.NameKey), hirePrice);
            }
            else
            {
                text = LocaManager.GetText(p_dialog.Feature.m_optionKey);
            }
            if (party.Gold < hirePrice)
            {
                flag = false;
                text = text + " " + LocaManager.GetText("NOT_ENOUGH_GOLD");
            }
            if (flag && !party.HirelingHandler.HasFreeSlot())
            {
                flag = false;
                text = text + " " + LocaManager.GetText("NO_FREE_HIRELING_SLOT");
            }
            dialogEntryView.SetFeature(text, flag, new HirelingFunction(ETargetCondition.HIRE, p_dialog.Feature.m_dialogID, hirePrice, p_dialog.Feature.m_sharePrice)
            {
                NpcID = p_dialog.Feature.m_npcID
            });
            if (!party.HirelingHandler.HasFreeSlot())
            {
                Int32 num = 1;
                if (party.HirelingHandler.Hirelings[0].CanBeFired)
                {
                    dialogEntryView = m_views[num];
                    HirelingFunction hirelingFunction = new HirelingFunction(ETargetCondition.FIRE, p_dialog.ID, 0, 0f);
                    hirelingFunction.NpcID = party.HirelingHandler.Hirelings[0].StaticID;
                    dialogEntryView.SetFeature(LocaManager.GetText("CONVERSATION_FIRE_ENTRY_TEXT", LocaManager.GetText(party.HirelingHandler.Hirelings[0].StaticData.NameKey)), true, hirelingFunction);
                    num++;
                }
                if (party.HirelingHandler.Hirelings[1].CanBeFired)
                {
                    dialogEntryView = m_views[num];
                    HirelingFunction hirelingFunction = new HirelingFunction(ETargetCondition.FIRE, p_dialog.ID, 0, 0f);
                    hirelingFunction.NpcID = party.HirelingHandler.Hirelings[1].StaticID;
                    dialogEntryView.SetFeature(LocaManager.GetText("CONVERSATION_FIRE_ENTRY_TEXT", LocaManager.GetText(party.HirelingHandler.Hirelings[1].StaticData.NameKey)), true, hirelingFunction);
                }
            }
        }

        private void InitKarthalUnderSiegeDialog(Dialog p_dialog)
        {
            Party party = LegacyLogic.Instance.WorldManager.Party;
            Boolean flag = true;
            Npc npc = (p_dialog.Feature.m_npcID != 0) ? LegacyLogic.Instance.WorldManager.NpcFactory.Get(p_dialog.Feature.m_npcID) : LegacyLogic.Instance.ConversationManager.CurrentNpc;
            DialogEntryView dialogEntryView = m_views[0];
            String text = LocaManager.GetText(p_dialog.Feature.m_optionKey);
            if (flag && !party.HirelingHandler.HasFreeSlot())
            {
                flag = false;
                text = text + " " + LocaManager.GetText("NO_FREE_HIRELING_SLOT");
            }
            dialogEntryView.SetFeature(text, flag, new HirelingFunction(ETargetCondition.HIRE_AND_CHANGE_MAP, p_dialog.Feature.m_dialogID, 0, 0f)
            {
                NpcID = p_dialog.Feature.m_npcID,
                MapName = p_dialog.Feature.m_mapName,
                TargetSpawnerId = p_dialog.Feature.m_targetSpawnId,
                QuestID = p_dialog.Feature.m_questID
            });
            if (!party.HirelingHandler.HasFreeSlot())
            {
                Int32 num = 1;
                if (party.HirelingHandler.Hirelings[0].CanBeFired)
                {
                    dialogEntryView = m_views[num];
                    HirelingFunction hirelingFunction = new HirelingFunction(ETargetCondition.FIRE, p_dialog.ID, 0, 0f);
                    hirelingFunction.NpcID = party.HirelingHandler.Hirelings[0].StaticID;
                    dialogEntryView.SetFeature(LocaManager.GetText("CONVERSATION_FIRE_ENTRY_TEXT", LocaManager.GetText(party.HirelingHandler.Hirelings[0].StaticData.NameKey)), true, hirelingFunction);
                    num++;
                }
                if (party.HirelingHandler.Hirelings[1].CanBeFired)
                {
                    dialogEntryView = m_views[num];
                    HirelingFunction hirelingFunction = new HirelingFunction(ETargetCondition.FIRE, p_dialog.ID, 0, 0f);
                    hirelingFunction.NpcID = party.HirelingHandler.Hirelings[1].StaticID;
                    dialogEntryView.SetFeature(LocaManager.GetText("CONVERSATION_FIRE_ENTRY_TEXT", LocaManager.GetText(party.HirelingHandler.Hirelings[1].StaticData.NameKey)), true, hirelingFunction);
                }
            }
        }

        private void InitSpellDialog(Dialog p_dialog)
        {
            SpellFunction.RaiseEventShow(LocaManager.GetText);

            Party party = LegacyLogic.Instance.WorldManager.Party;
            m_npc.TradingSpells.UpdateSpells();
            for (Int32 i = 0; i < party.Members.Length; i++)
            {
                Character memberByOrder = party.GetMemberByOrder(i);
                if (memberByOrder != null)
                {
                    DialogEntryView dialogEntryView = m_views[i];
                    String text = LocaManager.GetText("CONVERSATION_STARTSPELL_ENTRY_TEXT", memberByOrder.Name);
                    Boolean flag = true;
                    if (memberByOrder.ConditionHandler.HasOneCondition(ECondition.DEAD | ECondition.UNCONSCIOUS | ECondition.PARALYZED | ECondition.SLEEPING))
                    {
                        text = LocaManager.GetText("CONVERSATION_STARTSPELL_FAILED", memberByOrder.Name);
                        flag = false;
                    }
                    if (flag)
                    {
                        Boolean flag2 = false;
                        for (Int32 j = 0; j < m_npc.TradingSpells.Spells.Count; j++)
                        {
                            if (memberByOrder.SpellHandler.CouldLearnSpell(m_npc.TradingSpells.Spells[j]))
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            flag = false;
                            text = LocaManager.GetText("CONVERSATION_STARTSPELL_FAILED", memberByOrder.Name);
                        }
                    }
                    dialogEntryView.SetFeature(text, flag, new SelectCharacterFunction(party.GetOrder(i), p_dialog.Feature.m_dialogID)
                    {
                        Function = new SpellFunction(p_dialog.Feature.m_dialogID)
                    });
                }
                m_needGold = true;
            }
        }

        private void InitCastDialog(Dialog p_dialog)
        {
            Party party = LegacyLogic.Instance.WorldManager.Party;
            ECharacterSpell[] spells = p_dialog.Feature.m_spells;
            Int32 num = p_dialog.Feature.m_price;
            if (num == -1)
            {
                num = ConfigManager.Instance.Game.CostCast;
            }
            Int32 num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcRestoreFactor);
            Boolean p_enabled = party.Gold > num2;
            String text = LocaManager.GetText("CONVERSATION_CAST_ENTRY_TEXT", num2);
            CastFunction p_function = new CastFunction(num2, spells, p_dialog.Feature.m_dialogID);
            DialogEntryView dialogEntryView = m_views[0];
            dialogEntryView.SetFeature(text, p_enabled, p_function);
            m_needGold = true;
        }

        private void InitRestoreDialog(Dialog p_dialog)
        {
            Party party = LegacyLogic.Instance.WorldManager.Party;
            Int32 num = p_dialog.Feature.m_price;
            if (num == -1)
            {
                num = ConfigManager.Instance.Game.CostRestoration;
            }
            Int32 num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcRestoreFactor);
            String text = LocaManager.GetText("CONVERSATION_RESTORE_DONATE_ENTRY_TEXT", num2);
            DialogEntryView dialogEntryView = m_views[0];
            Boolean flag = true;
            Int32 num3 = 0;
            for (Int32 i = 0; i < party.Members.Length; i++)
            {
                if (!party.Members[i].ConditionHandler.HasCondition(ECondition.DEAD))
                {
                    if (party.Members[i].HealthPoints < party.Members[i].MaximumHealthPoints || party.Members[i].ManaPoints < party.Members[i].MaximumManaPoints)
                    {
                        num3++;
                    }
                }
            }
            if (flag && num2 > party.Gold)
            {
                flag = false;
            }
            RestoreFunction p_function = new RestoreFunction(num2, p_dialog.Feature.m_dialogID);
            dialogEntryView.SetFeature(text, flag, p_function);
        }

        private void InitResurrectDialog(Dialog p_dialog)
        {
            ResurrectFunction.RaiseEventShow(LocaManager.GetText);

            Party party = LegacyLogic.Instance.WorldManager.Party;
            for (Int32 i = 0; i < party.Members.Length; i++)
            {
                Character memberByOrder = party.GetMemberByOrder(i);
                Int32 num = p_dialog.Feature.m_price;
                if (num == -1)
                {
                    num = ConfigManager.Instance.Game.CostResurrect;
                }
                Int32 num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcResurrectFactor);
                String text = LocaManager.GetText("CONVERSATION_RESURRECT_ENTRY_TEXT", memberByOrder.Name, num2);
                Boolean p_enabled = true;
                if (memberByOrder.ConditionHandler.HasCondition(ECondition.DEAD))
                {
                    if (num2 > party.Gold)
                    {
                        p_enabled = false;
                    }
                }
                else
                {
                    text = LocaManager.GetText("CONVERSATION_RESURRECT_FAILED_NOT_DEAD", memberByOrder.Name);
                    p_enabled = false;
                }
                ResurrectFunction p_function = new ResurrectFunction(memberByOrder, num2, p_dialog.Feature.m_dialogID);
                DialogEntryView dialogEntryView = m_views[i];
                dialogEntryView.SetFeature(text, p_enabled, p_function);
            }
            m_needGold = true;
        }

        private void InitCureDialog(Dialog p_dialog)
        {
            CureFunction.RaiseEventShow(LocaManager.GetText);

            Party party = LegacyLogic.Instance.WorldManager.Party;
            for (Int32 i = 0; i < party.Members.Length; i++)
            {
                Character memberByOrder = party.GetMemberByOrder(i);
                Int32 num = p_dialog.Feature.m_price;
                if (num == -1)
                {
                    num = ConfigManager.Instance.Game.CostCure;
                }
                Int32 num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcCureFactor);
                Int32 num3 = 0;
                StringBuilder stringBuilder = new StringBuilder(String.Empty);
                Boolean flag = false;
                for (Int32 j = 0; j < p_dialog.Feature.m_curableConditions.Length; j++)
                {
                    NpcConversationStaticData.CurableCondition curableCondition = p_dialog.Feature.m_curableConditions[j];
                    if (memberByOrder.ConditionHandler.HasCondition(curableCondition.m_type))
                    {
                        flag = true;
                        num3 += num2;
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(", ");
                        }
                        stringBuilder.Append(LocaManager.GetText("CHARACTER_CONDITION_" + curableCondition.m_type.ToString()));
                    }
                }
                String text = LocaManager.GetText("CONVERSATION_CURE_ENTRY_TEXT", memberByOrder.Name, num3);
                if (stringBuilder.Length > 0)
                {
                    text = text + " (" + stringBuilder.ToString() + ")";
                }
                Boolean flag2 = true;
                ECondition[] array = new ECondition[p_dialog.Feature.m_curableConditions.Length];
                for (Int32 k = 0; k < p_dialog.Feature.m_curableConditions.Length; k++)
                {
                    array[k] = p_dialog.Feature.m_curableConditions[k].m_type;
                }
                if (memberByOrder.ConditionHandler.GetVisibleCondition() == ECondition.NONE || !flag)
                {
                    flag2 = false;
                    text = LocaManager.GetText("CONVERSATION_CURE_NEED_NO_CURE", memberByOrder.Name);
                }
                else if (memberByOrder.ConditionHandler.GetVisibleCondition() == ECondition.DEAD)
                {
                    String str = (memberByOrder.Gender != EGender.FEMALE) ? "M" : "F";
                    text = LocaManager.GetText("ACTION_LOG_CONDITION_CHANGED_DEAD_" + str, memberByOrder.Name);
                }
                flag2 = (flag2 && !memberByOrder.ConditionHandler.HasCondition(ECondition.DEAD));
                flag2 = (flag2 && LegacyLogic.Instance.WorldManager.Party.Gold >= num3);
                CureFunction p_function = new CureFunction(memberByOrder, array, num2, p_dialog.Feature.m_dialogID);
                DialogEntryView dialogEntryView = m_views[i];
                dialogEntryView.SetFeature(text, flag2, p_function);
            }
            m_needGold = true;
        }

        private void InitSuppliesDialog(Dialog p_dialog)
        {
            SuppliesFunction.RaiseEventShow(LocaManager.GetText);

            Int32 num = p_dialog.Feature.m_price;
            if (num == -1)
            {
                num = ConfigManager.Instance.Game.CostSupplies;
            }
            Int32 num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcSuppliesFactor);
            num2 = (Int32)(num2 * ((p_dialog.Feature.m_count - LegacyLogic.Instance.WorldManager.Party.Supplies) / (Single)p_dialog.Feature.m_count));
            if (num2 < 0)
            {
                num2 = 0;
            }
            String text = LocaManager.GetText("CONVERSATION_SUPPLIES_ENTRY_TEXT", p_dialog.Feature.m_count, num2);
            Boolean p_enabled = LegacyLogic.Instance.WorldManager.Party.Gold >= num2;
            if (LegacyLogic.Instance.WorldManager.Party.Supplies >= p_dialog.Feature.m_count)
            {
                text = LocaManager.GetText("CONVERSATION_SUPPLIES_FAILED_ENOUGH_SUPPLIES");
                p_enabled = false;
            }
            SuppliesFunction p_function = new SuppliesFunction(p_dialog.Feature.m_count, num2, p_dialog.ID);
            DialogEntryView dialogEntryView = m_views[0];
            dialogEntryView.SetFeature(text, p_enabled, p_function);
            m_needGold = true;
            m_needFood = true;
        }

        private void InitRestDialog(Dialog p_dialog)
        {
            RestFunction.RaiseEventShow(LocaManager.GetText);

            Int32 price = p_dialog.Feature.m_price;
            if (price == -1)
                price = ConfigManager.Instance.Game.CostRest;

            Int32 num2 = CalculatePriceForDifficulty(price, ConfigManager.Instance.Game.NpcRestFactor);
            String text = LocaManager.GetText("CONVERSATION_REST_ENTRY_TEXT", num2);
            Boolean p_enabled = LegacyLogic.Instance.WorldManager.Party.Gold >= num2;
            RestFunction p_function = new RestFunction(p_dialog.Feature.m_wellRested, p_dialog.Feature.m_dialogID, num2);
            DialogEntryView dialogEntryView = m_views[0];
            dialogEntryView.SetFeature(text, p_enabled, p_function);
            m_needGold = true;
        }

        private void InitTrainingDialog(Dialog p_dialog)
        {
            TrainingFunction.RaiseEventShow(LocaManager.GetText, p_dialog.Feature.m_skillID, p_dialog.Feature.m_skillRank);

            Party party = LegacyLogic.Instance.WorldManager.Party;

            for (Int32 i = 0; i < party.Members.Length; i++)
            {
                Character memberByOrder = party.GetMemberByOrder(i);
                if (memberByOrder != null)
                {
                    Int32 price = p_dialog.Feature.m_price;
                    if (price == -1)
                    {
                        if (p_dialog.Feature.m_skillRank == ETier.EXPERT)
                            price = ConfigManager.Instance.Game.SkillExpertPrice;
                        else if (p_dialog.Feature.m_skillRank == ETier.MASTER)
                            price = ConfigManager.Instance.Game.SkillMasterPrice;
                        else
                            price = ConfigManager.Instance.Game.SkillGrandmasterPrice;
                    }

                    price = CalculatePriceForDifficulty(price, ConfigManager.Instance.Game.NpcSkillTrainingFactor);
                    String text = LocaManager.GetText("CONVERSATION_TRAINING_ENTRY_TEXT", memberByOrder.Name, price);

                    Boolean flag = true;
                    if (memberByOrder.ConditionHandler.HasOneCondition(ECondition.DEAD | ECondition.UNCONSCIOUS | ECondition.PARALYZED | ECondition.SLEEPING))
                    {
                        text = LocaManager.GetText("CONVERSATION_TRAINING_FAILED_CONDITION", memberByOrder.Name);
                        flag = false;
                    }

                    Skill skill = memberByOrder.SkillHandler.FindSkill(p_dialog.Feature.m_skillID);
                    if (flag && skill == null)
                    {
                        text = LocaManager.GetText("CONVERSATION_TRAINING_FAILED_NO_ACCESS_TO_SKILL", memberByOrder.Name);
                        flag = false;
                    }
                    else
                    {
                        if (flag && p_dialog.Feature.m_skillRank > skill.MaxTier)
                        {
                            text = LocaManager.GetText("CONVERSATION_TRAINING_FAILED_NO_ACCESS_TO_TIER", memberByOrder.Name);
                            flag = false;
                        }

                        if (flag && p_dialog.Feature.m_skillRank - 1 > skill.Tier)
                        {
                            text = LocaManager.GetText("CONVERSATION_TRAINING_FAILED_RANK_TOO_LOW", memberByOrder.Name);
                            flag = false;
                        }

                        if (flag && skill.Tier >= p_dialog.Feature.m_skillRank)
                        {
                            text = LocaManager.GetText("CONVERSATION_TRAINING_FAILED_CHARACTER_ALREADY_HAS_SKILLRANK", memberByOrder.Name);
                            flag = false;
                        }

                        if (flag && skill.Level < skill.GetRequiredSkillLevel(p_dialog.Feature.m_skillRank))
                        {
                            text = LocaManager.GetText("CONVERSATION_TRAINING_FAILED_SKILLLEVEL_TOO_LOW", memberByOrder.Name);
                            flag = false;
                        }

                        if (flag && p_dialog.Feature.m_skillRank == ETier.GRAND_MASTER && !memberByOrder.UnlockedAdvancedClass)
                        {
                            text = LocaManager.GetText("CONVERSATION_TRAINING_FAILED_ADVANCEDCLASS_NOT_UNLOCKED", memberByOrder.Name);
                            flag = false;
                        }
                    }

                    if (flag && party.Gold < price)
                    {
                        text = LocaManager.GetText("CONVERSATION_TRAINING_FAILED_NOT_ENOUGH_GOLD", memberByOrder.Name, price);
                        text = text + " " + LocaManager.GetText("NOT_ENOUGH_GOLD");
                        flag = false;
                    }

                    TrainingFunction p_function = new TrainingFunction(p_dialog.Feature.m_skillID, p_dialog.Feature.m_skillRank, memberByOrder, (p_dialog.Feature.m_dialogID != -1) ? p_dialog.Feature.m_dialogID : p_dialog.ID, price);
                    DialogEntryView dialogEntryView = m_views[i];
                    dialogEntryView.SetFeature(text, flag, p_function);
                }
                m_needGold = true;
            }
        }

        private void InitRespecDialog(Dialog p_dialog)
        {
            Party party = LegacyLogic.Instance.WorldManager.Party;
            for (Int32 i = 0; i < party.Members.Length; i++)
            {
                Character memberByOrder = party.GetMemberByOrder(i);
                if (memberByOrder != null)
                {
                    Int32 num = p_dialog.Feature.m_price;
                    if (num == -1)
                    {
                        num = ConfigManager.Instance.Game.CostRespec;
                    }
                    Int32 num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcRespecFactor);
                    String text = LocaManager.GetText("CONVERSATION_RESPEC_ENTRY_TEXT", memberByOrder.Name, num2);
                    Boolean flag = true;
                    if (memberByOrder.ConditionHandler.HasOneCondition(ECondition.DEAD | ECondition.UNCONSCIOUS | ECondition.PARALYZED | ECondition.SLEEPING))
                    {
                        text = LocaManager.GetText("CONVERSATION_RESPEC_FAILED_CONDITION", memberByOrder.Name);
                        flag = false;
                    }
                    if (flag && party.Gold < num2)
                    {
                        text = LocaManager.GetText("CONVERSATION_RESPEC_FAILED_NOT_ENOUGH_GOLD", memberByOrder.Name, num2);
                        text = text + " " + LocaManager.GetText("NOT_ENOUGH_GOLD");
                        flag = false;
                    }
                    RespecFunction p_function = new RespecFunction(memberByOrder, num2, p_dialog.Feature.m_dialogID);
                    DialogEntryView dialogEntryView = m_views[i];
                    dialogEntryView.SetFeature(text, flag, p_function);
                }
                m_needGold = true;
            }
        }

        public void HideText()
        {
            m_text.Hide();
            NGUITools.SetActive(m_npcName.gameObject, false);
        }

        public void CloseDialogView(GameObject p_sender)
        {
            if (IngameController.Instance.Overlay.BackAlpha == 0f)
            {
                if (LegacyLogic.Instance.ConversationManager.IsForEntrance)
                {
                    ShowPopup();
                }
                else
                {
                    HideScreen();
                }
            }
            StopVoiceOver();
        }

        public void ShowPopup()
        {
            GridInfo gridInfo = LegacyLogic.Instance.MapLoader.FindGridInfo(LegacyLogic.Instance.ConversationManager.Level.Replace(".xml", String.Empty));
            String text = String.Empty;
            if (gridInfo != null)
            {
                CheckPartyLevel();
                text = LocaManager.GetText(gridInfo.LocationLocaName);
                if (text.IndexOf("@") != -1)
                {
                    text = text.Substring(0, text.IndexOf("@"));
                }
            }
            PopupRequest.Instance.OpenEntranceRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, LocaManager.GetText("REQUEST_GOTO_DUNGEON", text), new PopupRequest.RequestCallback(OnRequestClosed));
        }

        public void CheckPartyLevel()
        {
            String indoorScene = LegacyLogic.Instance.ConversationManager.IndoorScene;
            IEnumerable<DungeonEntryStaticData> iterator = StaticDataHandler.GetIterator<DungeonEntryStaticData>(EDataType.DUNGEON_ENTRY);
            Single num = 0f;
            Single num2 = 0f;
            Single num3 = 0f;
            foreach (DungeonEntryStaticData dungeonEntryStaticData in iterator)
            {
                String dungeonEntryID = dungeonEntryStaticData.DungeonEntryID;
                if (dungeonEntryID == indoorScene)
                {
                    num2 = dungeonEntryStaticData.PartyAverageMinLevel;
                    num3 = dungeonEntryStaticData.PartyAverageMaxLevel;
                }
            }
            for (Int32 i = 0; i < 4; i++)
            {
                Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(i);
                num += member.Level;
            }
            num /= 4f;
            if (num < num2)
            {
                LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.HIGH_LEVEL_DUNGEON);
            }
            if (num > num3)
            {
                LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.LOW_LEVEL_DUNGEON);
            }
        }

        private void OnRequestClosed(PopupRequest.EResultType p_result, String p_text)
        {
            if (p_result == PopupRequest.EResultType.CONFIRMED)
            {
                m_skipFade = true;
                LegacyLogic.Instance.ConversationManager.ChangeMap();
                HideScreen();
            }
            else
            {
                HideScreen();
            }
        }

        private void HideScreen()
        {
            if (LegacyLogic.Instance.ConversationManager.IsOpen && FinishFade)
            {
                Npc currentNpc = LegacyLogic.Instance.ConversationManager.CurrentNpc;
                if (currentNpc != null)
                {
                    if (currentNpc.TradingInventory.IsTrading)
                    {
                        currentNpc.TradingInventory.StopTrading();
                    }
                    if (currentNpc.TradingSpells.IsTrading)
                    {
                        currentNpc.TradingSpells.StopTrading();
                    }
                }
                LegacyLogic.Instance.ConversationManager.CloseNpcContainer(null);
            }
        }

        public void ToggleVisibility()
        {
            m_isHidden = !m_isHidden;
            if (m_isHidden)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void Hide()
        {
            m_fakeNPC = null;
            HideText();
            NGUITools.SetActive(m_entryList, false);
            NGUITools.SetActive(m_textBackground.gameObject, false);
            LegacyLogic.Instance.ConversationManager.CloseDialog();
            NGUITools.SetActive(m_npcIcon.gameObject, false);
            NGUITools.SetActive(m_goldRessourceViewGO, false);
            NGUITools.SetActive(m_foodRessourceViewGO, false);
            StopVoiceOver();
        }

        private void Show()
        {
            if (m_npc != null)
            {
                m_text.Show();
                m_text.SetInternalText(m_npc.ConversationData[m_currentDialogID].Text);
                if (m_fakeNPC != null)
                {
                    m_npcIcon.spriteName = m_fakeNPC.StaticData.PortraitKey;
                }
                else
                {
                    m_npcIcon.spriteName = m_npc.StaticData.PortraitKey;
                }
            }
            NGUITools.SetActive(m_text.gameObject, true);
            NGUITools.SetActive(m_npcName.gameObject, LegacyLogic.Instance.ConversationManager.ShowNameAndPortrait);
            NGUITools.SetActive(m_npcIcon.gameObject, LegacyLogic.Instance.ConversationManager.ShowNameAndPortrait);
            NGUITools.SetActive(m_entryList, true);
            for (Int32 i = 0; i < m_views.Length; i++)
            {
                NGUITools.SetActive(m_views[i].gameObject, m_views[i].IsVisible);
            }
            NGUITools.SetActive(m_textBackground.gameObject, true);
            NGUITools.SetActive(m_closeButton.gameObject, true);
            if (m_npc != null)
            {
                if (m_currentDialogID != m_npc.ConversationData.RootDialog.ID)
                {
                    UpdateBackButton(m_text.AtEnd());
                }
                else
                {
                    UpdateBackButton(false);
                }
            }
            m_goldRessourceView.text = LegacyLogic.Instance.WorldManager.Party.Gold.ToString();
            m_foodRessourceView.text = LegacyLogic.Instance.WorldManager.Party.Supplies.ToString();
            NGUITools.SetActive(m_goldRessourceViewGO, m_needGold);
            NGUITools.SetActive(m_foodRessourceViewGO, m_needFood);
            if (m_needGold)
            {
                if (m_needFood)
                {
                    m_goldRessourceViewGO.transform.localPosition = new Vector3(m_goldRessourceViewGO.transform.localPosition.x, 141f);
                }
                else
                {
                    m_goldRessourceViewGO.transform.localPosition = new Vector3(m_goldRessourceViewGO.transform.localPosition.x, 113f);
                }
            }
            if (LegacyLogic.Instance.ConversationManager.ShowNpcs)
            {
                m_closeButton.IsEnabled = true;
            }
            else if (LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HirelingHired(m_npc.StaticID) && m_npc.ConversationData.RootDialog.ID == m_currentDialogID)
            {
                m_closeButton.IsEnabled = true;
            }
            else
            {
                m_closeButton.IsEnabled = false;
            }
            m_closeButton.UpdateColor(m_closeButton.IsEnabled, true);
        }

        private void ClearEntries()
        {
            if (m_views.Length > 0)
            {
                foreach (DialogEntryView dialogEntryView in m_views)
                {
                    dialogEntryView.Reset();
                }
            }
            m_needGold = false;
            m_needFood = false;
        }

        public void HideForEntrance()
        {
            m_npcName.text = String.Empty;
            NGUITools.SetActive(m_npcIcon.gameObject, false);
            m_text.SetInternalText(String.Empty);
            if (m_views.Length > 0)
            {
                foreach (DialogEntryView dialogEntryView in m_views)
                {
                    NGUITools.SetActive(dialogEntryView.gameObject, false);
                }
            }
            NGUITools.SetActive(m_closeButton.gameObject, false);
            NGUITools.SetActive(m_goldRessourceViewGO, false);
            NGUITools.SetActive(m_foodRessourceViewGO, false);
            NGUITools.SetActive(m_textBackground.gameObject, false);
        }

        private Int32 CalculatePriceForDifficulty(Int32 p_price, Single p_factor)
        {
            if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
            {
                p_price = (Int32)Math.Ceiling(p_price * p_factor);
            }
            return p_price;
        }

        private void InitVoiceOver(String p_voiceID)
        {
            StopVoiceOver();
            m_currentVoiceID = null;
            if (!String.IsNullOrEmpty(p_voiceID))
            {
                if (m_text.PageCount > 1)
                {
                    String text = p_voiceID + "_part" + (m_text.CurrentPage + 1);
                    if (AudioManager.Instance.IsValidAudioID(text))
                    {
                        m_currentVoiceID = p_voiceID;
                        p_voiceID = text;
                    }
                    else
                    {
                        Debug.Log("Voice over part not found. " + text);
                    }
                }
                PlayVoiceOverLoadingSafe(p_voiceID);
            }
        }

        private void PlayVoiceOverLoadingSafe(String p_voiceAudioID)
        {
            if (LegacyLogic.Instance.MapLoader.IsLoading)
            {
                m_audioIDToPlayAfterLoading = p_voiceAudioID;
            }
            else
            {
                PlayVoiceOver(p_voiceAudioID);
            }
        }

        private void PlayVoiceOver(String p_voiceAudioID)
        {
            StopVoiceOver();
            if (!String.IsNullOrEmpty(p_voiceAudioID))
            {
                if (!AudioManager.Instance.IsValidAudioID(p_voiceAudioID))
                {
                    Debug.LogError("Dialog: Unknow voice audioID '" + p_voiceAudioID + "'!");
                    return;
                }
                m_currentAudioRequest = AudioManager.Instance.RequestByAudioID(p_voiceAudioID, 100, delegate(AudioRequest a)
                {
                    if (a.IsDone && a.Controller != null)
                    {
                        m_currentAudioObject = AudioController.Play(p_voiceAudioID);
                        StopAllCoroutines();
                        StartCoroutine(UnloadVoiceOver());
                    }
                });
            }
        }

        private void StopVoiceOver()
        {
            m_audioIDToPlayAfterLoading = null;
            if (m_currentAudioRequest != null && m_currentAudioRequest.IsLoading)
            {
                m_currentAudioRequest.AbortLoad();
            }
            m_currentAudioRequest = null;
            if (m_currentAudioObject != null)
            {
                if (m_currentAudioObject.IsPlaying())
                {
                    m_currentAudioObject.Stop();
                }
                AudioManager.Instance.UnloadByAudioID(m_currentAudioObject.audioID);
                m_currentAudioObject = null;
            }
        }

        private IEnumerator UnloadVoiceOver()
        {
            while (m_currentAudioObject != null)
            {
                if (!m_currentAudioObject.IsPlaying())
                {
                    AudioManager.Instance.UnloadByAudioID(m_currentAudioObject.audioID);
                    m_currentAudioObject = null;
                    break;
                }
                yield return null;
            }
            yield break;
        }
    }
}