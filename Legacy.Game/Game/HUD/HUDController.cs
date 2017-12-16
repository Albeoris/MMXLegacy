using System;
using System.Collections.Generic;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic.Actions;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDController")]
	public class HUDController : MonoBehaviour
	{
		[SerializeField]
		private CharacterHud[] m_characters;

		[SerializeField]
		private HUDActionLog m_actionLog;

		[SerializeField]
		private NewHUDQuestLog m_questLog;

		[SerializeField]
		private HUDPartyBuffs m_partyBuffs;

		[SerializeField]
		private GameObject m_characterContainer;

		[SerializeField]
		private ActionBarView m_actionBarView;

		[SerializeField]
		private MovementActionBar m_movementActionBar;

		[SerializeField]
		private HirelingHUD[] m_hirelings;

		private Party m_party;

		private List<ICharacterSelectionListener> m_selectionListeners = new List<ICharacterSelectionListener>();

		private Boolean m_chooseChar;

		public Boolean ActionBarEnabled { get; set; }

		public void Init(Party p_party, IngameInput p_ingameInput)
		{
			ActionBarEnabled = true;
			InitParty(p_party);
			InitHirelings();
			m_actionLog.Init(LegacyLogic.Instance.ActionLog);
			m_questLog.Init(LegacyLogic.Instance.WorldManager.QuestHandler);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAME_LOADED, new EventHandler(OnGameLoaded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnSceneLoaded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_ORDER_CHANGED, new EventHandler(OnPartyOrderChanged));
			m_movementActionBar.Init(p_ingameInput);
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_1, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_2, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_3, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_4, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_5, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_6, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_7, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_8, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_9, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_0, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			DragDropManager.Instance.DropEvent += OnDropEvent;
		}

		public void CleanupBuffs()
		{
			m_partyBuffs.Clear();
		}

		private void OnQuickSlotAction(Object p_sender, HotkeyEventArgs p_args)
		{
			if (!ActionBarEnabled || !IngameController.Instance.IngameInput.Active)
			{
				return;
			}
			if (p_args.KeyUp)
			{
				switch (p_args.Action)
				{
				case EHotkeyType.QUICK_ACTION_SLOT_1:
					m_actionBarView.PressedHotkey(0);
					break;
				case EHotkeyType.QUICK_ACTION_SLOT_2:
					m_actionBarView.PressedHotkey(1);
					break;
				case EHotkeyType.QUICK_ACTION_SLOT_3:
					m_actionBarView.PressedHotkey(2);
					break;
				case EHotkeyType.QUICK_ACTION_SLOT_4:
					m_actionBarView.PressedHotkey(3);
					break;
				case EHotkeyType.QUICK_ACTION_SLOT_5:
					m_actionBarView.PressedHotkey(4);
					break;
				case EHotkeyType.QUICK_ACTION_SLOT_6:
					m_actionBarView.PressedHotkey(5);
					break;
				case EHotkeyType.QUICK_ACTION_SLOT_7:
					m_actionBarView.PressedHotkey(6);
					break;
				case EHotkeyType.QUICK_ACTION_SLOT_8:
					m_actionBarView.PressedHotkey(7);
					break;
				case EHotkeyType.QUICK_ACTION_SLOT_9:
					m_actionBarView.PressedHotkey(8);
					break;
				case EHotkeyType.QUICK_ACTION_SLOT_0:
					m_actionBarView.PressedHotkey(9);
					break;
				}
			}
		}

		public void CleanUp()
		{
			if (m_characters != null)
			{
				for (Int32 i = 0; i < m_characters.Length; i++)
				{
					m_characters[i].OnCharacterClicked -= OnCharacterClicked;
					m_characters[i].Cleanup();
				}
			}
			m_actionLog.CleanUp();
			m_questLog.CleanUp();
			m_actionBarView.CleanUp();
			m_hirelings[0].Cleanup();
			m_hirelings[1].Cleanup();
			m_partyBuffs.Clear();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_ADDED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_REMOVED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAMETIME_TIME_CHANGED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTING, new EventHandler(OnPartyRest));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAME_LOADED, new EventHandler(OnGameLoaded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnSceneLoaded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_ORDER_CHANGED, new EventHandler(OnPartyOrderChanged));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_1, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_2, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_3, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_4, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_5, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_6, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_7, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_8, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_9, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_0, new EventHandler<HotkeyEventArgs>(OnQuickSlotAction));
			DragDropManager.Instance.DropEvent -= OnDropEvent;
		}

		private void OnDropEvent(Object p_sender, EventArgs p_args)
		{
			if (DragDropManager.Instance.DraggedItem is PortraitDragObject)
			{
				for (Int32 i = 0; i < m_characters.Length; i++)
				{
					if (m_characters[i] != null)
					{
						m_characters[i].EndDrag();
					}
				}
			}
		}

		private void OnGameLoaded(Object sender, EventArgs e)
		{
			for (Int32 i = 0; i < m_characters.Length; i++)
			{
				if (m_characters[i] != null)
				{
					m_characters[i].OnCharacterClicked -= OnCharacterClicked;
				}
			}
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_ADDED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_REMOVED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAMETIME_TIME_CHANGED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTING, new EventHandler(OnPartyRest));
			InitParty(LegacyLogic.Instance.WorldManager.Party);
			InitHirelings();
		}

		private void OnSceneLoaded(Object sender, EventArgs e)
		{
			NGUITools.SetActive(m_characterContainer, true);
			NGUITools.SetActive(m_movementActionBar.gameObject, true);
			NGUITools.SetActive(m_partyBuffs.gameObject, true);
			NGUITools.SetActive(m_actionBarView.gameObject, true);
			m_actionLog.SetVisible(true);
			m_questLog.SetVisible(true);
			if (m_hirelings != null)
			{
				for (Int32 i = 0; i < m_hirelings.Length; i++)
				{
					Npc view = m_party.HirelingHandler.Hirelings[i];
					m_hirelings[i].SetView(view);
				}
			}
			for (Int32 j = 0; j < m_characters.Length; j++)
			{
				if (m_characters[j] != null)
				{
					m_characters[j].SetSelected(m_characters[j].Owner == LegacyLogic.Instance.WorldManager.Party.SelectedCharacter);
					m_characters[j].UpdateSkillButton();
					m_characters[j].UpdateAttributeButton();
				}
			}
		}

		private void OnPartyOrderChanged(Object p_sender, EventArgs p_args)
		{
			if (m_characters != null)
			{
				for (Int32 i = 0; i < m_characters.Length; i++)
				{
					Character memberByOrder = m_party.GetMemberByOrder(i);
					m_characters[i].ChangeCharacter(memberByOrder);
				}
			}
		}

		private void InitParty(Party p_party)
		{
			m_party = p_party;
			if (m_characters != null)
			{
				for (Int32 i = 0; i < m_characters.Length; i++)
				{
					Character memberByOrder = m_party.GetMemberByOrder(i);
					if (memberByOrder != null)
					{
						m_characters[i].Init(memberByOrder, i);
						m_characters[i].OnCharacterClicked += OnCharacterClicked;
						m_characters[i].SetSelected(memberByOrder == m_party.SelectedCharacter);
					}
				}
			}
			m_partyBuffs.Clear();
			m_partyBuffs.Init();
			m_partyBuffs.UpdateBuffs(m_party.Buffs.Buffs);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_BUFF_ADDED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_BUFF_REMOVED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAMETIME_TIME_CHANGED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTING, new EventHandler(OnPartyRest));
			m_questLog.CleanUp();
			m_questLog.Init(LegacyLogic.Instance.WorldManager.QuestHandler);
		}

		private void InitHirelings()
		{
			if (m_hirelings != null)
			{
				for (Int32 i = 0; i < m_hirelings.Length; i++)
				{
					m_hirelings[i].Init(i);
				}
			}
		}

		private void OnPartyRest(Object p_sender, EventArgs p_args)
		{
			if (p_sender is RestFunction)
			{
				AudioManager.Instance.RequestPlayAudioID("RestDialog", 0);
			}
			else if (p_sender is RestAction)
			{
				AudioManager.Instance.RequestPlayAudioID("Rest", 0);
			}
			else
			{
				AudioManager.Instance.RequestPlayAudioID("Rest", 0);
			}
			OverlayManager overlay = IngameController.Instance.Overlay;
			EventHandler callback = delegate(Object sender, EventArgs e)
			{
				LegacyLogic.Instance.WorldManager.Party.DoRest();
				overlay.FadeFrontTo(0f);
			};
			overlay.FadeFrontTo(1f, 1f, callback);
		}

		public void OnPartyBuffsChanged(Object p_sender, EventArgs p_args)
		{
			m_partyBuffs.UpdateBuffs(m_party.Buffs.Buffs);
		}

		public void ShowScreenSpaceElements(Boolean p_visible)
		{
			m_actionLog.SetVisible(p_visible);
			m_questLog.SetVisible(p_visible);
		}

		private void OnCharacterClicked(Object p_sender, EventArgs p_args)
		{
			CharacterHud characterHud = p_sender as CharacterHud;
			if (characterHud != null)
			{
				foreach (ICharacterSelectionListener characterSelectionListener in m_selectionListeners)
				{
					if (characterSelectionListener.SelectCharacter(characterHud.Owner.Index))
					{
						return;
					}
				}
				if (m_chooseChar && !characterHud.Enabled)
				{
					return;
				}
				m_party.SelectCharacter(characterHud.Owner.Index);
				AudioManager.Instance.RequestPlayAudioID("PortraitSelect", 0);
			}
		}

		public void OnCharacterPressed(Int32 p_index)
		{
			foreach (ICharacterSelectionListener characterSelectionListener in m_selectionListeners)
			{
				if (characterSelectionListener.SelectCharacter(m_characters[p_index].Owner.Index))
				{
					return;
				}
			}
			if (m_chooseChar && !m_characters[p_index].Enabled)
			{
				return;
			}
			m_party.SelectCharacter(m_characters[p_index].Owner.Index);
			AudioManager.Instance.RequestPlayAudioID("PortraitSelect", 0);
		}

		public void AddSelectionListener(ICharacterSelectionListener p_selectionListener)
		{
			m_selectionListeners.Add(p_selectionListener);
		}

		public void RemoveSelecionListener(ICharacterSelectionListener p_selectionListener)
		{
			m_selectionListeners.Remove(p_selectionListener);
		}

		public void SetCharacterHudDisabled()
		{
			if (m_characters != null)
			{
				for (Int32 i = 0; i < m_characters.Length; i++)
				{
					if (m_chooseChar)
					{
						m_characters[i].SetEnabled(!m_characters[i].Owner.ConditionHandler.CantDoAnything());
					}
					else
					{
						m_characters[i].SetEnabled(true);
					}
				}
			}
		}

		public void HideFlames()
		{
			if (m_characters != null)
			{
				for (Int32 i = 0; i < m_characters.Length; i++)
				{
					m_characters[i].HideFlames();
				}
			}
		}

		public void ShowFlames()
		{
			if (m_characters != null)
			{
				for (Int32 i = 0; i < m_characters.Length; i++)
				{
					m_characters[i].ShowFlames();
				}
			}
		}

		public void MovePortraitsInFront()
		{
			m_chooseChar = true;
			for (Int32 i = 0; i < m_characters.Length; i++)
			{
				m_characters[i].MovePortraitInFront();
			}
		}

		public void MovePortraitsBack()
		{
			m_chooseChar = false;
			for (Int32 i = 0; i < m_characters.Length; i++)
			{
				m_characters[i].MovePortraitBack();
			}
		}

		public void HideAllTooltips()
		{
			BroadcastMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
			BroadcastMessage("OnTooltip", false, SendMessageOptions.DontRequireReceiver);
		}
	}
}
