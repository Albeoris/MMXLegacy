using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	public class UnlockContentEntry : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_selectBackground;

		[SerializeField]
		private UILabel m_labelTitle;

		[SerializeField]
		private UILabel m_labelState;

		[SerializeField]
		private Color m_colorRedeemed = new Color(0f, 0.375f, 0f);

		[SerializeField]
		private Color m_colorNotRedeemed = Color.black;

		[SerializeField]
		private Color m_selectionHoverColor = new Color(1f, 1f, 1f, 0.625f);

		[SerializeField]
		private Color m_selectionColor = new Color(1f, 1f, 1f, 0.5f);

		[SerializeField]
		private Color m_hoverColor = new Color(1f, 1f, 1f, 0.25f);

		[SerializeField]
		private Color m_notSelectedColor = new Color(1f, 1f, 1f, 0f);

		private UnlockableContentStaticData m_data;

		private Boolean m_unlockState;

		private Boolean m_isHovered;

		private Boolean m_selected;

		public event EventHandler OnClicked;

		public UnlockableContentStaticData StaticData => m_data;

	    public Boolean UnlockState => m_unlockState;

	    private void OnDisable()
		{
			OnHover(false);
		}

		public void Init(UnlockableContentStaticData p_data)
		{
			m_labelTitle.text = LocaManager.GetText(p_data.NameKey);
			m_data = p_data;
			UpdateUnlockState();
			SetSelected(false);
		}

		public void UpdateUnlockState()
		{
			m_unlockState = LegacyLogic.Instance.ServiceWrapper.IsPrivilegeAvailable(m_data.PrivilegeId);
			if (m_unlockState)
			{
				m_labelState.text = LocaManager.GetText("UNLOCK_CONTENT_STATE_ACTIVE");
				m_labelState.color = m_colorRedeemed;
				m_labelTitle.color = m_colorRedeemed;
			}
			else
			{
				m_labelState.text = LocaManager.GetText("UNLOCK_CONTENT_STATE_NOT_REDEEMED");
				m_labelState.color = m_colorNotRedeemed;
				m_labelTitle.color = m_colorNotRedeemed;
			}
		}

		public void SetSelected(Boolean p_selected)
		{
			m_selected = p_selected;
			if (m_selected)
			{
				TweenColor.Begin(m_selectBackground.gameObject, 0.1f, (!m_isHovered) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				TweenColor.Begin(m_selectBackground.gameObject, 0.1f, (!m_isHovered) ? m_notSelectedColor : m_hoverColor);
			}
		}

		private void OnClick()
		{
			if (OnClicked != null)
			{
				OnClicked(this, EventArgs.Empty);
			}
		}

		private void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (enabled)
			{
				if (m_selected)
				{
					TweenColor.Begin(m_selectBackground.gameObject, 0.1f, (!p_isOver) ? m_selectionColor : m_selectionHoverColor);
				}
				else
				{
					TweenColor.Begin(m_selectBackground.gameObject, 0.1f, (!p_isOver) ? m_notSelectedColor : m_hoverColor);
				}
			}
		}
	}
}
