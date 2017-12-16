using System;
using System.Collections;
using System.Threading;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/PopupRequest")]
	public class PopupRequest : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private UIButton m_buttonLeft;

		[SerializeField]
		private UIButton m_buttonCenter;

		[SerializeField]
		private UIButton m_buttonRight;

		[SerializeField]
		private UIButton m_buttonClose;

		[SerializeField]
		private UISprite m_buttonLeftBackground;

		[SerializeField]
		private UISprite m_buttonRightBackground;

		[SerializeField]
		private UISprite m_buttonCenterBackground;

		[SerializeField]
		private UILabel m_buttonLeftText;

		[SerializeField]
		private UILabel m_buttonCenterText;

		[SerializeField]
		private UILabel m_buttonRightText;

		[SerializeField]
		private UILabel m_caption;

		[SerializeField]
		private UILabel m_text;

		[SerializeField]
		private GameObject m_veil;

		[SerializeField]
		private GameObject m_background;

		[SerializeField]
		private GameObject m_border;

		[SerializeField]
		private GameObject m_buttonContainer;

		[SerializeField]
		private PopupItemSplitter m_itemSplitter;

		[SerializeField]
		private UIInput m_textField;

		[SerializeField]
		private UIInput m_textArea;

		[SerializeField]
		private Single m_inputWindowOffset = 50f;

		[SerializeField]
		private Single m_splitItemsWindowOffset = 128f;

		[SerializeField]
		private Single m_textAreaWindowOffset = 140f;

		[SerializeField]
		private String m_buttonLeftBackgroundNormal;

		[SerializeField]
		private String m_buttonRightBackgroundNormal;

		[SerializeField]
		private String m_buttonCenterBackgroundNormal;

		[SerializeField]
		private String m_buttonBlue;

		private Boolean m_isActive;

		private RequestCallback m_callback;

		private Vector3 m_originalBackgroundSize;

		private Vector3 m_originalBorderSize;

		private Vector3 m_originalButtonPosition;

		private ERequestType m_type;

		private static PopupRequest s_instance;

		private Boolean m_ignoreFirstKeyUp;

		public event EventHandler<EventArgs> OpenPopupRequest;

		public event EventHandler<EventArgs> ClosePopupRequest;

		public static PopupRequest Instance => s_instance;

	    public Boolean IsActive => m_isActive;

	    public PopupItemSplitter ItemSplitter => m_itemSplitter;

	    public String InputFieldText
		{
			get => m_textField.text;
	        set => m_textField.text = value;
	    }

		public String InputAreaText
		{
			get => m_textArea.text;
		    set => m_textArea.text = value;
		}

		private void Awake()
		{
			m_originalBackgroundSize = m_background.transform.localScale;
			m_originalBorderSize = m_border.transform.localScale;
			m_originalButtonPosition = m_buttonContainer.transform.localPosition;
			Init();
		}

		private void OnDestroy()
		{
			Destroy();
		}

		public void Init()
		{
			Destroy();
			s_instance = this;
			CloseRequest();
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnHotkeyInteract));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnHotkeyOpenCloseMenu));
			InputManager.RegisterHotkeyEvent(EHotkeyType.CONFIRM_TEXT_INPUT, new EventHandler<HotkeyEventArgs>(OnHotkeyInteract));
		}

		public void Destroy()
		{
			if (s_instance == this)
			{
				s_instance = null;
			}
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnHotkeyInteract));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnHotkeyOpenCloseMenu));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.CONFIRM_TEXT_INPUT, new EventHandler<HotkeyEventArgs>(OnHotkeyInteract));
		}

		public void OpenEntranceRequest(ERequestType p_type, String p_caption, String p_text, RequestCallback p_callback)
		{
			OpenRequest(p_type, p_caption, p_text, p_callback);
			m_veil.SetActive(false);
		}

		public void OpenRequest(ERequestType p_type, String p_caption, String p_text, RequestCallback p_callback)
		{
			if (m_isActive)
			{
				return;
			}
			m_isActive = true;
			m_type = p_type;
			m_callback = p_callback;
			m_text.text = p_text;
			m_textField.text = String.Empty;
			m_textArea.text = String.Empty;
			NGUITools.SetActiveSelf(m_buttonLeft.gameObject, false);
			NGUITools.SetActiveSelf(m_buttonRight.gameObject, false);
			NGUITools.SetActiveSelf(m_buttonCenter.gameObject, false);
			NGUITools.SetActiveSelf(m_buttonClose.gameObject, true);
			NGUITools.SetActiveSelf(m_textField.gameObject, false);
			NGUITools.SetActiveSelf(m_textArea.gameObject, false);
			NGUITools.SetActiveSelf(m_itemSplitter.gameObject, false);
			m_buttonLeftBackground.spriteName = m_buttonLeftBackgroundNormal;
			m_buttonRightBackground.spriteName = m_buttonRightBackgroundNormal;
			m_buttonCenterBackground.spriteName = m_buttonCenterBackgroundNormal;
			NGUITools.SetActiveSelf(gameObject, true);
			switch (p_type)
			{
			case ERequestType.CONFIRM_CANCEL:
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_REQUEST") : p_caption);
				m_buttonLeft.gameObject.SetActive(true);
				m_buttonLeftText.text = LocaManager.GetText("POPUP_REQUEST_CONFIRM");
				m_buttonRight.gameObject.SetActive(true);
				m_buttonRightText.text = LocaManager.GetText("POPUP_REQUEST_CANCEL");
				ButtonWindowSizeOffset(0f);
				break;
			case ERequestType.CONFIRM:
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_CONFIRM") : p_caption);
				m_buttonCenter.gameObject.SetActive(true);
				m_buttonCenterText.text = LocaManager.GetText("POPUP_REQUEST_OK");
				ButtonWindowSizeOffset(0f);
				break;
			case ERequestType.TEXTFIELD_CONFIRM:
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_INPUT") : p_caption);
				m_buttonCenterBackground.spriteName = m_buttonBlue;
				m_buttonCenter.gameObject.SetActive(true);
				m_buttonCenterText.text = LocaManager.GetText("POPUP_REQUEST_CONFIRM");
				m_textField.gameObject.SetActive(true);
				m_textField.selected = true;
				ButtonWindowSizeOffset(m_inputWindowOffset);
				break;
			case ERequestType.SPIRIT_BEACON:
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_REQUEST") : p_caption);
				m_buttonLeftBackground.spriteName = m_buttonBlue;
				m_buttonLeft.gameObject.SetActive(true);
				m_buttonLeftText.text = LocaManager.GetText("POPUP_REQUEST_SPIRIT_BEACON_TRAVEL");
				m_buttonRightBackground.spriteName = m_buttonBlue;
				m_buttonRight.gameObject.SetActive(true);
				m_buttonRightText.text = LocaManager.GetText("POPUP_REQUEST_SPIRIT_BEACON_NEW");
				ButtonWindowSizeOffset(0f);
				break;
			case ERequestType.SPLIT_ITEMS:
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_ITEM_SPLITTER") : p_caption);
				m_buttonLeft.gameObject.SetActive(true);
				m_buttonLeftText.text = LocaManager.GetText("POPUP_REQUEST_CONFIRM");
				m_buttonRight.gameObject.SetActive(true);
				m_buttonRightText.text = LocaManager.GetText("POPUP_REQUEST_CANCEL");
				ButtonWindowSizeOffset(m_splitItemsWindowOffset);
				break;
			case ERequestType.YES_NO:
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_REQUEST") : p_caption);
				m_buttonLeft.gameObject.SetActive(true);
				m_buttonLeftText.text = LocaManager.GetText("POPUP_REQUEST_YES");
				m_buttonRight.gameObject.SetActive(true);
				m_buttonRightText.text = LocaManager.GetText("POPUP_REQUEST_NO");
				ButtonWindowSizeOffset(0f);
				break;
			case ERequestType.APPLY_DISCARD:
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_REQUEST") : p_caption);
				m_buttonLeft.gameObject.SetActive(true);
				m_buttonLeftText.text = LocaManager.GetText("POPUP_REQUEST_APPLY");
				m_buttonRight.gameObject.SetActive(true);
				m_buttonRightText.text = LocaManager.GetText("POPUP_REQUEST_DISCARD");
				ButtonWindowSizeOffset(0f);
				break;
			case ERequestType.MAP_NOTES:
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_MAPNOTE") : p_caption);
				m_textArea.gameObject.SetActive(true);
				m_textArea.selected = true;
				m_buttonLeftBackground.spriteName = m_buttonBlue;
				m_buttonLeft.gameObject.SetActive(true);
				m_buttonLeftText.text = LocaManager.GetText("POPUP_REQUEST_APPLY");
				m_buttonRightBackground.spriteName = m_buttonBlue;
				m_buttonRight.gameObject.SetActive(true);
				m_buttonRightText.text = LocaManager.GetText("POPUP_REQUEST_DELETE");
				ButtonWindowSizeOffset(m_textAreaWindowOffset);
				break;
			case ERequestType.TEXTFIELD_NO_CANCEL:
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_INPUT") : p_caption);
				m_buttonCenterBackground.spriteName = m_buttonBlue;
				m_buttonCenter.gameObject.SetActive(true);
				m_buttonCenterText.text = LocaManager.GetText("POPUP_REQUEST_CONFIRM");
				m_buttonClose.gameObject.SetActive(false);
				m_textField.gameObject.SetActive(true);
				m_textField.selected = false;
				m_textField.StopAllCoroutines();
				m_textField.StartCoroutine(SelectTextFieldDelayed(0.3f));
				ButtonWindowSizeOffset(m_inputWindowOffset);
				break;
			case ERequestType.CUSTOM_POPUP:
				m_ignoreFirstKeyUp = true;
				m_caption.text = ((!(p_caption != String.Empty)) ? LocaManager.GetText("POPUP_CAPTION_CONFIRM") : p_caption);
				m_buttonCenter.gameObject.SetActive(true);
				m_buttonCenterText.text = LocaManager.GetText("POPUP_REQUEST_OK");
				ButtonWindowSizeOffset(0f);
				break;
			}
			if (OpenPopupRequest != null)
			{
				OpenPopupRequest(this, EventArgs.Empty);
			}
			AudioController.Play("SOU_ANNO4_PopUp_Open");
		}

		public void CloseRequest()
		{
			m_isActive = false;
			m_textField.selected = false;
			m_textArea.selected = false;
			m_veil.SetActive(true);
			NGUITools.SetActiveSelf(gameObject, false);
			if (ClosePopupRequest != null)
			{
				ClosePopupRequest(this, EventArgs.Empty);
			}
		}

		private void ButtonWindowSizeOffset(Single p_offset)
		{
			Vector3 localScale = m_originalBackgroundSize;
			localScale.y += p_offset;
			m_background.transform.localScale = localScale;
			localScale = m_originalBorderSize;
			localScale.y += p_offset;
			m_border.transform.localScale = localScale;
			Vector3 originalButtonPosition = m_originalButtonPosition;
			originalButtonPosition.y -= p_offset;
			m_buttonContainer.transform.localPosition = originalButtonPosition;
		}

		private void OnLeftButtonClicked()
		{
			CloseRequest();
			if (m_callback != null)
			{
				if (m_type == ERequestType.SPLIT_ITEMS)
				{
					m_itemSplitter.FinalizeInput();
				}
				m_callback(EResultType.CONFIRMED, String.Empty);
				m_callback = null;
			}
		}

		private void OnCenterButtonClicked()
		{
			CloseRequest();
			if (m_callback != null)
			{
				if (m_type == ERequestType.SPLIT_ITEMS)
				{
					m_itemSplitter.FinalizeInput();
				}
				m_callback(EResultType.CONFIRMED, m_textField.text);
				m_callback = null;
			}
		}

		private void OnRightButtonClicked()
		{
			CloseRequest();
			if (m_callback != null)
			{
				if (m_type == ERequestType.MAP_NOTES)
				{
					m_callback(EResultType.DELETE, m_textField.text);
				}
				else
				{
					m_callback(EResultType.CANCELED, m_textField.text);
				}
				m_callback = null;
			}
		}

		private void OnCloseButtonClicked()
		{
			if (m_type == ERequestType.TEXTFIELD_NO_CANCEL)
			{
				return;
			}
			CloseRequest();
			if (m_callback != null)
			{
				m_callback(EResultType.CANCELED, m_textField.text);
				m_callback = null;
			}
		}

		private void OnHotkeyInteract(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_type == ERequestType.CUSTOM_POPUP && m_ignoreFirstKeyUp)
			{
				m_ignoreFirstKeyUp = false;
				return;
			}
			if (m_isActive && p_args.KeyUp)
			{
				if ((m_type == ERequestType.MAP_NOTES || m_type == ERequestType.TEXTFIELD_CONFIRM || m_type == ERequestType.TEXTFIELD_NO_CANCEL) && (m_textField.selected || m_textArea.selected) && p_args.Action == EHotkeyType.INTERACT)
				{
					return;
				}
				OnCenterButtonClicked();
			}
		}

		private void OnHotkeyOpenCloseMenu(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				OnCloseButtonClicked();
			}
		}

		private IEnumerator SelectTextFieldDelayed(Single waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			m_textField.selected = true;
			yield break;
		}

		public void Activate()
		{
		}

		public void Deactivate()
		{
		}

		public enum ERequestType
		{
			CONFIRM_CANCEL,
			CONFIRM,
			TEXTFIELD_CONFIRM,
			SPIRIT_BEACON,
			SPLIT_ITEMS,
			YES_NO,
			APPLY_DISCARD,
			MAP_NOTES,
			TEXTFIELD_NO_CANCEL,
			CUSTOM_POPUP
		}

		public enum EResultType
		{
			CONFIRMED,
			CANCELED,
			DELETE
		}

		public delegate void RequestCallback(EResultType p_result, String p_inputString);
	}
}
