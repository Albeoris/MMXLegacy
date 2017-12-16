using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.MMGUI;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/AttributeLevelButton")]
	public class AttributeLevelButton : MonoBehaviour
	{
		[SerializeField]
		private Vector3 m_offset;

		[SerializeField]
		private Single m_tooltipDelay;

		[SerializeField]
		private GUIMultiSpriteButton m_button;

		private Boolean m_isHovered;

		private Boolean m_tooltipVisible;

		private Single m_tooltipTime;

		private ButtonType m_buttonType;

		private EPotionTarget m_attribute;

		private Int32 m_baseValue;

		private Int32 m_currentIncrease;

		private Character m_character;

		private DummyCharacter m_dummy;

		public void Init(EPotionTarget p_attribute, Int32 p_baseValue, Int32 p_currentIncrease, ButtonType p_buttonType, Character p_character, DummyCharacter p_dummy)
		{
			m_attribute = p_attribute;
			m_buttonType = p_buttonType;
			m_baseValue = p_baseValue;
			m_currentIncrease = p_currentIncrease;
			m_character = p_character;
			m_dummy = p_dummy;
		}

		public void SetEnabled(Boolean p_enabled)
		{
			m_button.IsEnabled = p_enabled;
		}

		public void UpdateTooltip()
		{
			if (m_tooltipVisible && m_button.IsEnabled)
			{
				ShowTooltip(true);
			}
		}

		private void OnHover(Boolean p_isHovered)
		{
			if (p_isHovered)
			{
				m_tooltipTime = Time.realtimeSinceStartup + m_tooltipDelay;
			}
			else if (m_tooltipVisible)
			{
				ShowTooltip(false);
			}
			m_isHovered = p_isHovered;
		}

		private void Update()
		{
			if (m_isHovered && !m_tooltipVisible && m_tooltipTime < Time.realtimeSinceStartup)
			{
				ShowTooltip(true);
			}
		}

		private void ShowTooltip(Boolean p_show)
		{
			if (p_show)
			{
				Vector3 position = gameObject.transform.position;
				AttributeTooltip.TooltipType p_type = (m_buttonType != ButtonType.PLUS) ? AttributeTooltip.TooltipType.CURRENT_EFFECT_PREV : AttributeTooltip.TooltipType.CURRENT_EFFECT_NEXT;
				TooltipManager.Instance.Show(this, p_type, m_attribute, m_baseValue + m_currentIncrease, position, m_offset, m_character, m_dummy);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
			m_tooltipVisible = p_show;
		}

		private void OnDisable()
		{
			OnHover(false);
			TooltipManager.Instance.Hide(this);
		}

		public enum ButtonType
		{
			PLUS,
			MINUS
		}
	}
}
