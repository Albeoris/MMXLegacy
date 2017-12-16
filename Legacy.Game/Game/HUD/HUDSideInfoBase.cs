using System;
using Legacy.Core.Api;
using UnityEngine;

namespace Legacy.Game.HUD
{
	public abstract class HUDSideInfoBase : MonoBehaviour
	{
		[SerializeField]
		protected Single SPAWN_MOVE_SPEED;

		[SerializeField]
		protected Single SPAWN_SUBSTRACT_AXIS_X;

		[SerializeField]
		protected Single LOOT_SHOW_TIME;

		[SerializeField]
		protected Single LOOT_FADEOUT_SPEED;

		[SerializeField]
		protected Single BACKGROUND_ALPHA = 0.75f;

		[SerializeField]
		protected Single ICON_FADEIN_SPEED = 1f;

		[SerializeField]
		protected Single TEXT_FADEIN_SPEED = 0.5f;

		[SerializeField]
		protected Single TEXT_DELAY = 2f;

		[SerializeField]
		protected UISlicedSprite m_backgroundFill;

		[SerializeField]
		protected UISprite m_itemIcon;

		[SerializeField]
		protected UISprite m_goldIcon;

		[SerializeField]
		protected UILabel m_itemLabel;

		[SerializeField]
		protected UISlicedSprite m_itemBackground;

		[SerializeField]
		protected UISprite m_scrollIcon;

		[SerializeField]
		protected UISprite m_itemIconBackground;

		[SerializeField]
		protected GameObject m_lootFX;

		protected Vector3 m_originalPos;

		protected Single m_currentAlpha;

		protected Boolean m_isUsed;

		protected Single m_insertTime;

		protected Boolean m_isReachedOriginalPos;

		protected Int32 m_currentGoldDisplayed;

		protected Boolean m_isShowingTooltip;

		public Boolean IsUsed()
		{
			return m_isUsed;
		}

		public virtual Boolean IsAlignedToBottom => false;

	    public void SetEnabledEntry(Boolean p_enabled)
		{
			NGUITools.SetActive(gameObject, p_enabled);
		}

		public void SetAlphaEntry(Single newAlphaAll)
		{
			SetAlphaEntry(newAlphaAll, newAlphaAll);
		}

		public void SetAlphaEntry(Single newAlphaIcon, Single newAlphaText)
		{
			m_backgroundFill.alpha = ((newAlphaText <= BACKGROUND_ALPHA) ? newAlphaText : BACKGROUND_ALPHA);
			m_itemLabel.alpha = newAlphaText;
			m_itemIcon.alpha = newAlphaIcon;
			m_goldIcon.alpha = newAlphaIcon;
			m_itemBackground.alpha = newAlphaIcon;
			if (m_itemIconBackground != null)
			{
				m_itemIconBackground.alpha = newAlphaIcon;
			}
			if (m_scrollIcon != null)
			{
				m_scrollIcon.alpha = newAlphaIcon;
			}
		}

		public void Init(String text, String icon, Int32 gold, Color color)
		{
			m_isUsed = true;
			m_insertTime = Time.time;
			m_currentAlpha = 0f;
			m_isReachedOriginalPos = false;
			SetEnabledEntry(true);
			if (!String.IsNullOrEmpty(icon))
			{
				NGUITools.SetActiveSelf(m_goldIcon.gameObject, false);
				m_itemIcon.spriteName = icon;
			}
			else
			{
				NGUITools.SetActiveSelf(m_itemIcon.gameObject, false);
			}
			NGUITools.SetActiveSelf(m_itemLabel.gameObject, true);
			if (m_scrollIcon != null)
			{
				NGUITools.SetActiveSelf(m_scrollIcon.gameObject, false);
			}
			if (m_itemIconBackground != null)
			{
				NGUITools.SetActiveSelf(m_itemIconBackground.gameObject, false);
			}
			if (!String.IsNullOrEmpty(text))
			{
				m_itemLabel.text = text;
			}
			else
			{
				m_currentGoldDisplayed += gold;
				m_itemLabel.text = "+ " + m_currentGoldDisplayed.ToString();
			}
			m_itemLabel.color = color;
			m_originalPos = transform.position;
			Vector3 originalPos = m_originalPos;
			originalPos.x -= SPAWN_SUBSTRACT_AXIS_X;
			transform.position = originalPos;
			if (m_lootFX != null)
			{
				GameObject gameObject = (GameObject)Instantiate(m_lootFX, transform.position, new Quaternion(0f, 0f, 0f, 0f));
				if (gameObject != null)
				{
					gameObject.transform.parent = transform;
					gameObject.transform.localScale = Vector3.one;
				}
			}
			SetAlphaEntry(0f);
		}

		public virtual void UpdateEntry()
		{
			if (!m_isReachedOriginalPos)
			{
				Vector3 position = transform.position;
				position.x += Time.deltaTime * SPAWN_MOVE_SPEED;
				if (position.x > m_originalPos.x)
				{
					position.x = m_originalPos.x;
					m_isReachedOriginalPos = true;
				}
				transform.position = position;
			}
			if (m_currentGoldDisplayed > 0 && m_insertTime + LOOT_SHOW_TIME / 2f <= Time.time)
			{
				m_currentGoldDisplayed = 0;
				m_itemLabel.text = "=" + LegacyLogic.Instance.WorldManager.Party.Gold.ToString();
				m_itemLabel.color = Color.yellow;
			}
			if (m_insertTime + 1f / ICON_FADEIN_SPEED >= Time.time || m_insertTime + TEXT_DELAY + 1f / TEXT_FADEIN_SPEED >= Time.time)
			{
				Single num = Time.time - m_insertTime;
				Single newAlphaIcon = Mathf.Clamp01(num * ICON_FADEIN_SPEED);
				Single newAlphaText = 0f;
				if (num > TEXT_DELAY)
				{
					newAlphaText = Mathf.Clamp01((num - TEXT_DELAY) * TEXT_FADEIN_SPEED);
				}
				SetAlphaEntry(newAlphaIcon, newAlphaText);
			}
			else if (m_insertTime + LOOT_SHOW_TIME > Time.time || m_isShowingTooltip)
			{
				if (m_currentAlpha != 1f)
				{
					m_currentAlpha = 1f;
					SetAlphaEntry(1f);
				}
			}
			else if (m_currentAlpha <= 0f)
			{
				SetEnabledEntry(false);
				m_isUsed = false;
			}
			else
			{
				m_currentAlpha -= Time.deltaTime * LOOT_FADEOUT_SPEED;
				SetAlphaEntry(m_currentAlpha);
			}
		}

		private void OnTooltip(Boolean show)
		{
			if (show && IsTooltipNeeded() && m_isUsed)
			{
				ShowTooltip();
				m_isShowingTooltip = true;
				m_currentAlpha = 1f;
				SetAlphaEntry(m_currentAlpha);
			}
			else
			{
				HideTooltip();
				m_isShowingTooltip = false;
			}
		}

		private void OnMouseUpAsButton()
		{
			OnClick();
		}

		protected abstract Boolean IsTooltipNeeded();

		protected abstract void ShowTooltip();

		protected abstract void HideTooltip();

		protected abstract void OnClick();
	}
}
