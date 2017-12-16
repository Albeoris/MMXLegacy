using System;
using System.Collections.Generic;
using Legacy.Configuration;
using Legacy.Game.MMGUI;
using Legacy.MMInput;
using UnityEngine;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/OptionsInput")]
	public class OptionsInput : MonoBehaviour, IScrollingListener
	{
		private const Int32 VISIBLE_ENTRIES_COUNT = 10;

		[SerializeField]
		private GameObject m_keyConfigPrefab;

		[SerializeField]
		private UIGrid m_keysRoot;

		[SerializeField]
		private UIScrollBar m_scrollBar;

		[SerializeField]
		private Single m_positionTweenSpeed = 0.05f;

		[SerializeField]
		private Int32 m_mouseScrollMultiplicator = 3;

		[SerializeField]
		private UIButton m_buttonKeyDelete;

		[SerializeField]
		private UILabel m_buttonKeyDeleteText;

		[SerializeField]
		private Color m_requestHighlightColor = new Color(255f, 240f, 192f);

		private Int32 m_entriesCount;

		private Vector3 m_startPos;

		private KeyConfigView m_currentKeyView;

		private EHotkeyType m_currentOverrideHotkey;

		private KeyCode m_currentOverrideKeyCode;

		private Dictionary<EHotkeyType, KeyConfigView> m_keyConfigViews;

		private String m_requestHighlightColorHex;

		private void Start()
		{
			m_keyConfigViews = new Dictionary<EHotkeyType, KeyConfigView>();
			m_requestHighlightColorHex = "[" + NGUITools.EncodeColor(m_requestHighlightColor) + "]";
			AddHotkey(EHotkeyType.MOVE_FORWARD);
			AddHotkey(EHotkeyType.MOVE_LEFT);
			AddHotkey(EHotkeyType.MOVE_BACKWARD);
			AddHotkey(EHotkeyType.MOVE_RIGHT);
			AddHotkey(EHotkeyType.ROTATE_LEFT);
			AddHotkey(EHotkeyType.ROTATE_RIGHT);
			AddEmptyLine();
			AddHotkey(EHotkeyType.INTERACT);
			AddHotkey(EHotkeyType.SELECT_NEXT_INTERACTIVE_OBJECT);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_1);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_2);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_3);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_4);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_5);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_6);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_7);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_8);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_9);
			AddHotkey(EHotkeyType.QUICK_ACTION_SLOT_0);
			AddHotkey(EHotkeyType.REST);
			AddEmptyLine();
			AddHotkey(EHotkeyType.OPEN_INVENTORY);
			AddHotkey(EHotkeyType.OPEN_CLOSE_SPELLBOOK);
			AddHotkey(EHotkeyType.OPEN_CLOSE_SKILLS);
			AddHotkey(EHotkeyType.OPEN_CLOSE_JOURNAL);
			AddHotkey(EHotkeyType.OPEN_CLOSE_BESTIARY);
			AddHotkey(EHotkeyType.OPEN_CLOSE_LORE);
			AddHotkey(EHotkeyType.OPEN_MAP);
			AddHotkey(EHotkeyType.OPEN_AREA_MAP);
			AddHotkey(EHotkeyType.OPEN_CLOSE_MENU);
			AddEmptyLine();
			AddHotkey(EHotkeyType.SELECT_PARTY_MEMBER_1);
			AddHotkey(EHotkeyType.SELECT_PARTY_MEMBER_2);
			AddHotkey(EHotkeyType.SELECT_PARTY_MEMBER_3);
			AddHotkey(EHotkeyType.SELECT_PARTY_MEMBER_4);
			AddHotkey(EHotkeyType.QUICKSAVE);
			AddHotkey(EHotkeyType.QUICKLOAD);
			AddHotkey(EHotkeyType.DISABLE_GUI);
			AddHotkey(EHotkeyType.MAKE_SCREENSHOT);
			m_startPos = m_keysRoot.transform.localPosition;
			m_keysRoot.Reposition();
			UpdateScrollbar();
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollbarChange));
			ScrollingHelper.InitScrollListeners(this, gameObject);
			m_buttonKeyDelete.isEnabled = false;
			m_buttonKeyDeleteText.color = Color.gray;
		}

		private void OnDestroy()
		{
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Remove(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollbarChange));
		}

		private void OnEnable()
		{
			UpdateGUI();
		}

		public void UpdateGUI()
		{
			if (m_keyConfigViews != null)
			{
				foreach (KeyConfigView keyConfigView in m_keyConfigViews.Values)
				{
					Hotkey hotkeyData = KeyConfigManager.KeyBindings[keyConfigView.HotkeyType];
					InputManager.SetHotkeyData(hotkeyData);
					keyConfigView.UpdateHotkeys();
				}
			}
		}

		public void RequestKeyBinding(KeyCode p_key)
		{
			foreach (EHotkeyType ehotkeyType in m_keyConfigViews.Keys)
			{
				if (m_currentKeyView.HotkeyType != ehotkeyType)
				{
					Hotkey hotkey = KeyConfigManager.KeyBindings[ehotkeyType];
					if (hotkey.Key1 == p_key || hotkey.AltKey1 == p_key)
					{
						m_currentOverrideHotkey = ehotkeyType;
						m_currentOverrideKeyCode = p_key;
						String arg = m_requestHighlightColorHex + LocaManager.GetText("OPTIONS_INPUT_KEY_" + ehotkeyType) + "[-]";
						String arg2 = m_requestHighlightColorHex + LocaManager.GetText("OPTIONS_INPUT_KEYNAME_" + p_key.ToString().ToUpper()) + "[-]";
						String text = LocaManager.GetText("OPTIONS_INPUT_KEY_REBIND", arg2, arg);
						PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.YES_NO, String.Empty, text, new PopupRequest.RequestCallback(KeyBindRequestCallback));
						return;
					}
				}
			}
			m_currentKeyView.ConfirmKeyBinding();
		}

		public void KeyBindRequestCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				m_currentKeyView.ConfirmKeyBinding();
				Hotkey hotkey = KeyConfigManager.KeyBindings[m_currentOverrideHotkey];
				if (hotkey.Key1 == m_currentOverrideKeyCode)
				{
					hotkey.Key1 = KeyCode.None;
				}
				if (hotkey.AltKey1 == m_currentOverrideKeyCode)
				{
					hotkey.AltKey1 = KeyCode.None;
				}
				KeyConfigManager.KeyBindings[m_currentOverrideHotkey] = hotkey;
				InputManager.SetHotkeyData(hotkey);
				m_keyConfigViews[m_currentOverrideHotkey].UpdateHotkeys();
			}
			else
			{
				m_currentKeyView.CancelKeyBinding();
			}
		}

		private void AddHotkey(EHotkeyType p_hotkeyType)
		{
			GameObject gameObject = NGUITools.AddChild(m_keysRoot.gameObject, m_keyConfigPrefab);
			KeyConfigView component = gameObject.GetComponent<KeyConfigView>();
			component.Init(p_hotkeyType, this);
			m_keyConfigViews[p_hotkeyType] = component;
			m_entriesCount++;
		}

		private void AddEmptyLine()
		{
			GameObject gameObject = NGUITools.AddChild(m_keysRoot.gameObject);
			gameObject.name = "KeyConfigView";
			m_entriesCount++;
		}

		public void StartKeyInput(KeyConfigView p_keyView)
		{
			m_currentKeyView = p_keyView;
			m_buttonKeyDelete.isEnabled = true;
			m_buttonKeyDeleteText.color = Color.white;
		}

		public void EndKeyInput()
		{
			m_buttonKeyDelete.isEnabled = false;
			m_buttonKeyDeleteText.color = Color.gray;
		}

		public void OnKeyBindingDelete()
		{
			m_currentKeyView.DeleteCurrentKeyBinding();
		}

		private void UpdateScrollbar()
		{
			m_scrollBar.barSize = 10f / m_entriesCount;
			m_scrollBar.scrollValue = 0f;
		}

		private void OnScrollbarChange(UIScrollBar p_scrollBar)
		{
			Int32 num = m_entriesCount - 10;
			Int32 num2 = (Int32)Math.Round(p_scrollBar.scrollValue * num);
			Vector3 startPos = m_startPos;
			startPos.y += num2 * m_keysRoot.cellHeight;
			TweenPosition.Begin(m_keysRoot.gameObject, m_positionTweenSpeed, startPos);
		}

		public void OnScroll(Single p_delta)
		{
			Int32 num = m_entriesCount - 10;
			m_scrollBar.scrollValue -= 1f / num * m_mouseScrollMultiplicator * p_delta * 10f;
		}
	}
}
