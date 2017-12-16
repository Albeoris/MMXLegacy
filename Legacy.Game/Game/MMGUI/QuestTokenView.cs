using System;
using Legacy.Core;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/HUD/QuestTokenView")]
	public class QuestTokenView : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private UILabel m_nameLabel;

		[SerializeField]
		private UILabel m_functionLabel;

		[SerializeField]
		private Color m_notUsableHereColor = new Color(0.5f, 0.5f, 0.5f);

		[SerializeField]
		private Color m_notUsableHereColorText = new Color(0.5f, 0.5f, 0.5f);

		private Color m_normalColor;

		private Color m_iconColor;

		private Boolean m_usable;

		private TokenStaticData m_data;

		private Vector3 m_tooltipOffset;

		public Boolean IsUsable => m_usable;

	    private void Awake()
		{
			m_normalColor = m_nameLabel.color;
			m_iconColor = new Color(1f, 1f, 1f);
			m_tooltipOffset = new Vector3(50f, 50f, 0f);
		}

		public void SetTokenData(TokenStaticData p_data, Boolean p_isUsable)
		{
			NGUITools.SetActive(gameObject, true);
			m_data = p_data;
			m_icon.spriteName = p_data.Icon;
			m_nameLabel.text = LocaManager.GetText(p_data.Name);
			m_usable = p_isUsable;
			if (p_isUsable)
			{
				m_icon.color = m_iconColor;
				m_nameLabel.color = m_normalColor;
				m_functionLabel.color = m_normalColor;
			}
			else
			{
				m_icon.color = m_notUsableHereColor;
				m_nameLabel.color = m_notUsableHereColorText;
				m_functionLabel.color = m_notUsableHereColorText;
			}
		}

		public void Hide()
		{
			NGUITools.SetActive(gameObject, false);
		}

		private void OnDisable()
		{
			OnTooltip(false);
		}

		public void OnTooltip(Boolean isOver)
		{
			if (isOver)
			{
				Vector3 position = m_icon.gameObject.transform.position;
				if (m_data.Description != String.Empty)
				{
					TooltipManager.Instance.Show(this, LocaManager.GetText(m_data.Name) + "\n\n" + LocaManager.GetText(m_data.Description), position, m_tooltipOffset);
				}
				else
				{
					Debug.Log(m_data.ToString());
					TooltipManager.Instance.Show(this, LocaManager.GetText(m_data.Name), position, m_tooltipOffset);
				}
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
