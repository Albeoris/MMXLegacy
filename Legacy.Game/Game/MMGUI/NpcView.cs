using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.NpcInteraction;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/NpcView")]
	public class NpcView : MonoBehaviour
	{
		private const String BACKGROUND_SELECTED_CHARACTER = "BTN_square_152_highlight";

		private const String BACKGROUND_NOT_SELECTED_CHARACTER = "BTN_square_152";

		private Npc m_npc;

		[SerializeField]
		private UISprite m_portrait;

		[SerializeField]
		private UISprite m_background;

		[SerializeField]
		private UISprite m_seperationBar;

		[SerializeField]
		private UISprite[] m_bookmarkIcons;

		[SerializeField]
		private GameObject[] m_bookmarkButtons;

		[SerializeField]
		private ButtonHighlight m_buttonHighlight;

		public event EventHandler ClickedNpcView;

		public void Init(Npc p_npc)
		{
			if (p_npc == null)
			{
				throw new ArgumentNullException("m_npc");
			}
			m_npc = p_npc;
			m_portrait.spriteName = m_npc.StaticData.PortraitKey;
			NGUITools.SetActive(m_bookmarkButtons[0], false);
			NGUITools.SetActive(m_bookmarkButtons[1], false);
			if (m_npc.ConversationData.Bookmarks != null && m_npc.ConversationData.Bookmarks.Length > 0)
			{
				for (Int32 i = 0; i < m_npc.ConversationData.Bookmarks.Length; i++)
				{
					if (i >= 2)
					{
						break;
					}
					m_bookmarkIcons[i].spriteName = "ICO_npc_" + m_npc.ConversationData.Bookmarks[i].m_function;
					NGUITools.SetActive(m_bookmarkButtons[i], true);
				}
				if (m_npc.ConversationData.Bookmarks.Length == 1)
				{
					m_bookmarkButtons[0].transform.localPosition = new Vector3(0f, m_bookmarkButtons[0].transform.localPosition.y);
				}
				else
				{
					m_bookmarkButtons[0].transform.localPosition = new Vector3(-37f, m_bookmarkButtons[0].transform.localPosition.y);
				}
			}
		}

		public void SetSeperationBarVisible(Boolean p_visible)
		{
			NGUITools.SetActiveSelf(m_seperationBar.gameObject, p_visible);
		}

		public void OnClickedBookmark(GameObject p_sender)
		{
			GameObject gameObject = p_sender.transform.parent.gameObject;
			if (gameObject == m_bookmarkButtons[0])
			{
				LegacyLogic.Instance.ConversationManager.OpenBookmark(m_npc, m_npc.ConversationData.Bookmarks[0]);
			}
			else if (gameObject == m_bookmarkButtons[1])
			{
				LegacyLogic.Instance.ConversationManager.OpenBookmark(m_npc, m_npc.ConversationData.Bookmarks[1]);
			}
		}

		public void OnNpcClicked(GameObject p_sender)
		{
			if (m_npc == null)
			{
				throw new NullReferenceException("m_npc is null!!! Call Init Method first");
			}
			ConversationManager conversationManager = LegacyLogic.Instance.ConversationManager;
			if (conversationManager.CurrentNpc != m_npc)
			{
				LegacyLogic.Instance.ConversationManager.OpenDialog(m_npc);
			}
			if (ClickedNpcView != null)
			{
				ClickedNpcView(this, EventArgs.Empty);
			}
			SetSelected(true);
		}

		public void SetSelected(Boolean p_selected)
		{
			m_background.spriteName = ((!p_selected) ? "BTN_square_152" : "BTN_square_152_highlight");
			m_buttonHighlight.SetSelected(p_selected);
		}
	}
}
