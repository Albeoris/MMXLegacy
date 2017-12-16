using System;
using System.Threading;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/AttributeChanger")]
	public class AttributeChanger : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_nameLabel;

		[SerializeField]
		private UILabel m_valueLabel;

		[SerializeField]
		private AttributeLevelButton m_levelUpButton;

		[SerializeField]
		private AttributeLevelButton m_levelDownButton;

		private Int32 m_baseValue;

		private Int32 m_addValue;

		private DummyCharacter m_dummyChar;

		private Character m_realChar;

		private EPotionTarget m_attribute;

		private String m_TT;

		public event EventHandler OnAttributeRaised;

		public event EventHandler OnAttributeLowered;

		public void Init(String p_name, String p_TT, EPotionTarget p_attribute, Int32 p_baseValue, Int32 p_addValue, DummyCharacter p_dummyChar, Character p_realChar)
		{
			m_levelUpButton.Init(p_attribute, p_baseValue, p_addValue, AttributeLevelButton.ButtonType.PLUS, p_realChar, p_dummyChar);
			m_levelDownButton.Init(p_attribute, p_baseValue, p_addValue, AttributeLevelButton.ButtonType.MINUS, p_realChar, p_dummyChar);
			m_attribute = p_attribute;
			m_dummyChar = p_dummyChar;
			m_realChar = p_realChar;
			m_baseValue = p_baseValue;
			m_addValue = p_addValue;
			m_TT = p_TT;
			m_nameLabel.text = p_name;
			UpdateValueLabel();
			UpdateButtons();
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public EPotionTarget Attribute => m_attribute;

	    private void UpdateValueLabel()
		{
			String text = (m_baseValue + m_addValue).ToString();
			if (m_addValue > 0)
			{
				if (m_dummyChar != null)
				{
					text = "[007f00]" + text + "[-]";
				}
				else
				{
					text = "[00ff00]" + text + "[-]";
				}
			}
			m_valueLabel.text = text;
		}

		private void UpdateButtons()
		{
			NGUITools.SetActive(m_levelUpButton.gameObject, PointsLeft() > 0);
			NGUITools.SetActive(m_levelDownButton.gameObject, m_addValue > 0);
			m_levelUpButton.UpdateTooltip();
			m_levelDownButton.UpdateTooltip();
		}

		public void OnLevelUpButtonClicked()
		{
			if (PointsLeft() > 0)
			{
				m_addValue++;
			}
			UpdateValueLabel();
			UpdateButtons();
			if (OnAttributeRaised != null)
			{
				OnAttributeRaised(this, EventArgs.Empty);
			}
		}

		private Int32 PointsLeft()
		{
			if (m_dummyChar != null)
			{
				return m_dummyChar.GetAttributesToPickLeft();
			}
			return m_realChar.TemporaryAttributePoints;
		}

		public void OnLevelDownButtonClicked()
		{
			if (m_addValue > 0)
			{
				m_addValue--;
			}
			UpdateValueLabel();
			UpdateButtons();
			if (OnAttributeLowered != null)
			{
				OnAttributeLowered(this, EventArgs.Empty);
			}
		}

		private void OnTooltip(Boolean show)
		{
			if (show)
			{
				TooltipManager.Instance.Show(this, m_TT, m_nameLabel.gameObject.transform.position, Vector3.zero);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		private void OnHover()
		{
		}
	}
}
