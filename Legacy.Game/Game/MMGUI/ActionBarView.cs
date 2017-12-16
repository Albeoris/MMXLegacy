using System;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ActionBarView")]
	public class ActionBarView : MonoBehaviour
	{
		[SerializeField]
		private ActionButtonView[] m_actionButtons;

		private void Start()
		{
			Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			for (Int32 i = 0; i < m_actionButtons.Length; i++)
			{
				CharacterQuickActions.Action action = selectedCharacter.QuickActions[i];
				m_actionButtons[i].Init(i, action.Type, action.Item, selectedCharacter.SpellHandler.GetSpell(action.Spell));
			}
			UpdateHotkeys();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnSelectedCharacter));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.UPDATE_AVAILABLE_ACTIONS, new EventHandler(OnUpdateAvailableActions));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_SELECTED, new EventHandler(OnUpdateAvailableActions));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INPUT_CHANGED, new EventHandler(OnOptionsChanged));
		}

		public void CleanUp()
		{
			for (Int32 i = 0; i < m_actionButtons.Length; i++)
			{
				m_actionButtons[i].CleanUp();
			}
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnSelectedCharacter));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.UPDATE_AVAILABLE_ACTIONS, new EventHandler(OnUpdateAvailableActions));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_SELECTED, new EventHandler(OnUpdateAvailableActions));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INPUT_CHANGED, new EventHandler(OnOptionsChanged));
		}

		private void OnOptionsChanged(Object p_sender, EventArgs p_args)
		{
			UpdateHotkeys();
		}

		private void UpdateHotkeys()
		{
			for (Int32 i = 0; i < m_actionButtons.Length; i++)
			{
				Hotkey hotkey = KeyConfigManager.KeyBindings[EHotkeyType.QUICK_ACTION_SLOT_1 + i];
				KeyCode keyCode = hotkey.Key1;
				if (keyCode == KeyCode.None)
				{
					keyCode = hotkey.AltKey1;
				}
				else if (keyCode != KeyCode.None)
				{
					String text = LocaManager.GetText("OPTIONS_INPUT_KEYNAME_" + keyCode.ToString().ToUpper());
					if (text.Length > 6)
					{
						text = text.Substring(0, 4) + "..";
					}
					m_actionButtons[i].ButtonHotkey = text;
				}
				else
				{
					m_actionButtons[i].ButtonHotkey = String.Empty;
				}
			}
		}

		private void OnSelectedCharacter(Object p_sender, EventArgs p_args)
		{
			if (DragDropManager.Instance.DraggedItem != null)
			{
				Boolean flag = DragDropManager.Instance.DraggedItem is BasicActionDragObject;
				Boolean flag2 = DragDropManager.Instance.DraggedItem is SpellDragObject;
				Boolean flag3 = DragDropManager.Instance.DraggedItem is QuickActionDragObject;
				Boolean flag4 = DragDropManager.Instance.DraggedItem is ItemDragObject;
				if (flag || flag2 || flag3 || flag4)
				{
					DragDropManager.Instance.CancelDragAction();
				}
			}
			Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			for (Int32 i = 0; i < m_actionButtons.Length; i++)
			{
				CharacterQuickActions.Action action = selectedCharacter.QuickActions[i];
				m_actionButtons[i].Reset();
				m_actionButtons[i].Type = action.Type;
				m_actionButtons[i].Spell = selectedCharacter.SpellHandler.GetSpell(action.Spell);
				m_actionButtons[i].Item = action.Item;
				m_actionButtons[i].UpdateItemCounter();
				m_actionButtons[i].UpdateVisibility();
				m_actionButtons[i].CheckUsability();
				if (action.Type == EQuickActionType.USE_BEST_HEALTHPOTION)
				{
					m_actionButtons[i].GetBestPotion(EPotionType.HEALTH_POTION);
				}
				else if (action.Type == EQuickActionType.USE_BEST_MANAPOTION)
				{
					m_actionButtons[i].GetBestPotion(EPotionType.MANA_POTION);
				}
			}
		}

		private void OnUpdateAvailableActions(Object p_sender, EventArgs p_args)
		{
			for (Int32 i = 0; i < m_actionButtons.Length; i++)
			{
				m_actionButtons[i].CheckUsability();
			}
		}

		public void PressedHotkey(Int32 p_index)
		{
			m_actionButtons[p_index].PressedHotkey();
		}
	}
}
