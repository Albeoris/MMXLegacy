using System;
using Legacy.Core.Entities.Items;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/TooltipItemSlot")]
	public class TooltipItemSlot : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_background;

		[SerializeField]
		private UISprite m_itemTexture;

		[SerializeField]
		private UISprite m_itemBackground;

		[SerializeField]
		private UISprite m_scrollTexture;

		[SerializeField]
		private UISprite m_brokenIcon;

		[SerializeField]
		private UISprite m_spellTexture;

		private Vector3 m_originalSize;

		private Vector3 m_itemTextureOriginalPos;

		private Vector3 m_itemBackgroundOriginalPos;

		private Vector3 m_scrollTextureOriginalPos;

		private Vector3 m_brokenIconOriginalPos;

		private Vector3 m_spellTextureOriginalPos;

		private Boolean m_initialized;

		public Vector3 Size
		{
			get
			{
				if (m_originalSize == Vector3.zero)
				{
					m_originalSize = m_background.transform.localScale;
				}
				return m_originalSize;
			}
		}

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			if (!m_initialized)
			{
				m_initialized = true;
				m_originalSize = m_background.transform.localScale;
				if (m_itemTexture != null)
				{
					m_itemTextureOriginalPos = m_itemTexture.transform.localPosition;
				}
				if (m_itemBackground != null)
				{
					m_itemBackgroundOriginalPos = m_itemBackground.transform.localPosition;
				}
				if (m_scrollTexture != null)
				{
					m_scrollTextureOriginalPos = m_scrollTexture.transform.localPosition;
				}
				if (m_brokenIcon != null)
				{
					m_brokenIconOriginalPos = m_brokenIcon.transform.localPosition;
				}
				if (m_spellTexture != null)
				{
					m_spellTextureOriginalPos = m_spellTexture.transform.localPosition;
				}
			}
		}

		public void UpdatePosition(Vector3 pos)
		{
			gameObject.transform.localPosition = pos;
		}

		public void SetHeight(Single p_height)
		{
			Vector3 originalSize = m_originalSize;
			originalSize.y = p_height;
			m_background.transform.localScale = originalSize;
			UpdateIconPosition(m_itemTexture, m_itemTextureOriginalPos);
			UpdateIconPosition(m_itemBackground, m_itemBackgroundOriginalPos);
			UpdateIconPosition(m_scrollTexture, m_scrollTextureOriginalPos);
			UpdateIconPosition(m_brokenIcon, m_brokenIconOriginalPos);
			UpdateIconPosition(m_spellTexture, m_spellTextureOriginalPos);
		}

		public void ResetHeight()
		{
			m_background.transform.localScale = m_originalSize;
			m_itemTexture.transform.localPosition = m_itemTextureOriginalPos;
			m_itemBackground.transform.localPosition = m_itemBackgroundOriginalPos;
			m_scrollTexture.transform.localPosition = m_scrollTextureOriginalPos;
			m_brokenIcon.transform.localPosition = m_brokenIconOriginalPos;
		}

		private void UpdateIconPosition(UISprite p_icon, Vector3 p_originalPos)
		{
			if (p_icon != null)
			{
				Single num = (m_background.transform.localScale.y - m_originalSize.y) * 0.5f;
				Vector3 localPosition = p_originalPos;
				localPosition.y -= num;
				p_icon.transform.localPosition = localPosition;
			}
		}

		public void SetVisible(Boolean p_visible)
		{
			NGUITools.SetActiveSelf(gameObject, p_visible);
		}

		public void SetItem(BaseItem p_item)
		{
			Initialize();
			if (p_item == null)
			{
				SetItem(String.Empty);
			}
			else if (p_item is Scroll)
			{
				SetItem("ITM_consumable_scroll");
				m_scrollTexture.spriteName = p_item.Icon;
				NGUITools.SetActiveSelf(m_scrollTexture.gameObject, true);
				NGUITools.SetActiveSelf(m_brokenIcon.gameObject, false);
				NGUITools.SetActiveSelf(m_itemBackground.gameObject, false);
			}
			else
			{
				SetItem(p_item.Icon);
				NGUITools.SetActiveSelf(m_scrollTexture.gameObject, false);
				if (p_item is Equipment)
				{
					Equipment equipment = (Equipment)p_item;
					NGUITools.SetActiveSelf(m_brokenIcon.gameObject, equipment.Broken);
					ItemSlot.UpdateItemBackground(m_itemBackground, p_item);
				}
				else
				{
					NGUITools.SetActiveSelf(m_brokenIcon.gameObject, false);
					NGUITools.SetActiveSelf(m_itemBackground.gameObject, false);
				}
			}
		}

		public void SetSpell(String p_icon)
		{
			Initialize();
			if (m_spellTexture != null)
			{
				if (p_icon != String.Empty)
				{
					m_spellTexture.spriteName = p_icon;
				}
				else
				{
					m_spellTexture.spriteName = "ITM_missing";
				}
			}
		}

		public void SetItem(String p_icon)
		{
			Initialize();
			NGUITools.SetActiveSelf(m_itemTexture.gameObject, true);
			if (m_itemTexture != null)
			{
				if (p_icon != String.Empty)
				{
					m_itemTexture.spriteName = p_icon;
				}
				else
				{
					m_itemTexture.spriteName = "ITM_missing";
				}
			}
		}

		public void HideItem()
		{
			NGUITools.SetActiveSelf(m_itemTexture.gameObject, false);
		}
	}
}
