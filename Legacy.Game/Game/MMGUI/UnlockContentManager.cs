using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	public class UnlockContentManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_entryPrefab;

		[SerializeField]
		private UILabel m_contentTitle;

		[SerializeField]
		private UILabel m_contentInfo;

		[SerializeField]
		private UITexture m_contentImage;

		[SerializeField]
		private UIGrid m_grid;

		[SerializeField]
		private UIButton m_buyButton;

		[SerializeField]
		private UILabel m_buyButtonText;

		[SerializeField]
		private UIInput m_inputField;

		[SerializeField]
		private LoadingAnim m_loadingAnim;

		[SerializeField]
		private UIDraggablePanel m_dragPanel;

		private List<UnlockContentEntry> m_entries = new List<UnlockContentEntry>();

		private UnlockContentEntry m_selectedEntry;

		private Boolean m_initialised;

		private Boolean m_isUnlockingPrivilege;

		private Boolean m_isVisible;

		public event EventHandler OnClose;

		public Boolean IsVisible => m_isVisible;

	    public void Open()
		{
			NGUITools.SetActiveSelf(this.gameObject, true);
			m_isVisible = true;
			if (!m_initialised)
			{
				IEnumerable<UnlockableContentStaticData> iterator = StaticDataHandler.GetIterator<UnlockableContentStaticData>(EDataType.UNLOCKABLE_CONTENT);
				foreach (UnlockableContentStaticData p_data in iterator)
				{
					GameObject gameObject = NGUITools.AddChild(m_grid.gameObject, m_entryPrefab);
					UnlockContentEntry component = gameObject.GetComponent<UnlockContentEntry>();
					component.Init(p_data);
					component.OnClicked += OnEntryClicked;
					m_entries.Add(component);
				}
				m_initialised = true;
			}
			SelectEntry(m_entries[0]);
		}

		private void OnEntryClicked(Object p_sender, EventArgs p_args)
		{
			UnlockContentEntry unlockContentEntry = p_sender as UnlockContentEntry;
			if (unlockContentEntry != m_selectedEntry)
			{
				SelectEntry(unlockContentEntry);
			}
		}

		private void SelectEntry(UnlockContentEntry p_entry)
		{
			if (m_selectedEntry != null)
			{
				m_selectedEntry.SetSelected(false);
			}
			m_selectedEntry = p_entry;
			m_selectedEntry.SetSelected(true);
			m_contentTitle.text = LocaManager.GetText(m_selectedEntry.StaticData.NameKey);
			m_contentInfo.text = LocaManager.GetText(m_selectedEntry.StaticData.InfoTextKey);
			Boolean flag = m_selectedEntry.StaticData.IsBuyable && !LegacyLogic.Instance.ServiceWrapper.IsPrivilegeAvailable(m_selectedEntry.StaticData.PrivilegeId);
			Texture texture = Helper.ResourcesLoad<Texture>("UnlockContent/" + m_selectedEntry.StaticData.Image);
			if (m_contentImage.mainTexture != texture)
			{
				Texture mainTexture = m_contentImage.mainTexture;
				m_contentImage.mainTexture = texture;
				if (mainTexture != null)
				{
					mainTexture.UnloadAsset();
				}
			}
			m_dragPanel.UpdateScrollbars(true);
		}

		public void Close()
		{
			m_isVisible = false;
			m_loadingAnim.SetVisible(false);
			NGUITools.SetActiveSelf(gameObject, false);
			if (m_isUnlockingPrivilege)
			{
				LegacyLogic.Instance.ServiceWrapper.CancelUnlockPrivilege();
				m_isUnlockingPrivilege = false;
			}
			if (OnClose != null)
			{
				OnClose(this, EventArgs.Empty);
			}
		}

		private void OnCloseClicked()
		{
			Close();
		}

		private void OnRedeemClicked()
		{
			if (LegacyLogic.Instance.ServiceWrapper.IsConnectedToServer())
			{
				LegacyLogic.Instance.ServiceWrapper.UnlockPrivilege(m_inputField.text);
				m_loadingAnim.SetVisible(true);
				m_isUnlockingPrivilege = true;
			}
			else
			{
				String text = LocaManager.GetText("CHEST_PROMOTION_KEY_NO_CONNECTION");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM, String.Empty, text, null);
			}
		}

		private void Update()
		{
			if (m_isUnlockingPrivilege)
			{
				EActivateKeyResult unlockPrivilegeState = LegacyLogic.Instance.ServiceWrapper.GetUnlockPrivilegeState();
				if (unlockPrivilegeState != EActivateKeyResult.ACTIVATE_WAITING)
				{
					Debug.Log("UnlockContentManager: " + unlockPrivilegeState);
					String p_caption = LocaManager.GetText("ERROR_POPUP_MESSAGE_CAPTION");
					String text;
					if (unlockPrivilegeState == EActivateKeyResult.ACTIVATE_SUCCESSFUL)
					{
						LegacyLogic.Instance.ServiceWrapper.UpdatePrivilegesRewards();
						text = LocaManager.GetText("ACTIVATE_KEY_SUCCESS_NEW_CONTENT");
						p_caption = String.Empty;
						foreach (UnlockContentEntry unlockContentEntry in m_entries)
						{
							unlockContentEntry.UpdateUnlockState();
						}
						SelectEntry(m_selectedEntry);
					}
					else if (unlockPrivilegeState == EActivateKeyResult.ACTIVATE_INVALID_KEY || unlockPrivilegeState == EActivateKeyResult.ACTIVATE_WRONG_PRIVILAGE)
					{
						text = LocaManager.GetText("ACTIVATE_KEY_ERROR_INVALID_KEY");
					}
					else if (unlockPrivilegeState == EActivateKeyResult.ACTIVATE_KEY_ALREADY_IN_USE)
					{
						text = LocaManager.GetText("ACTIVATE_KEY_ERROR_KEY_ALREADY_USED");
					}
					else
					{
						text = LocaManager.GetText("CHEST_PROMOTION_KEY_FAIL_TEXT");
					}
					m_isUnlockingPrivilege = false;
					m_loadingAnim.SetVisible(false);
					SelectEntry(m_selectedEntry);
					PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM, p_caption, text, null);
				}
			}
		}

		private void OnBuyClicked()
		{
		}
	}
}
