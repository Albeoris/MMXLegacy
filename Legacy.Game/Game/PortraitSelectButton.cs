using System;
using System.Threading;
using Legacy.Core;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/PortraitSelectButton")]
	public class PortraitSelectButton : MonoBehaviour
	{
		private const String BACKGROUND_SELECTED_CHARACTER = "BTN_square_152_highlight";

		private const String BACKGROUND_NOT_SELECTED_CHARACTER = "BTN_square_152";

		[SerializeField]
		private UISprite m_BG;

		[SerializeField]
		private UISprite m_Portrait;

		[SerializeField]
		private UISprite m_body;

		[SerializeField]
		private Color m_hoverColor;

		[SerializeField]
		private Color m_selectionColor;

		[SerializeField]
		private Color m_selectionHoverColor;

		private Boolean m_selected = true;

		private Boolean m_isHovered;

		private Color m_pBackgroundNormalColor;

		private EGender m_gender;

		private Int32 m_picIndex;

		public event EventHandler OnPicSelected;

		public void Init(EGender p_gender, Int32 p_index)
		{
			m_pBackgroundNormalColor = m_BG.color;
			m_gender = p_gender;
			m_picIndex = p_index;
		}

		public EGender Gender => m_gender;

	    public Int32 PicIndex => m_picIndex;

	    public void UpdateTexture(ERace p_race, EClass p_class)
		{
			String text;
			if (p_race == ERace.HUMAN)
			{
				text = "PIC_head_human";
			}
			else if (p_race == ERace.ELF)
			{
				text = "PIC_head_elf";
			}
			else if (p_race == ERace.DWARF)
			{
				text = "PIC_head_dwarf";
			}
			else
			{
				text = "PIC_head_orc";
			}
			if (m_gender == EGender.MALE)
			{
				text += "_male_";
			}
			else
			{
				text += "_female_";
			}
			text = text + m_picIndex.ToString() + "_idle";
			m_Portrait.spriteName = text;
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)p_class);
			String text2 = staticData.BodyBase;
			if (m_gender == EGender.MALE)
			{
				text2 += "_male";
			}
			else
			{
				text2 += "_female";
			}
			m_body.spriteName = text2;
		}

		private void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (m_selected)
			{
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!p_isOver) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!p_isOver) ? m_pBackgroundNormalColor : m_hoverColor);
			}
		}

		private void OnClick()
		{
			if (OnPicSelected != null)
			{
				OnPicSelected(this, EventArgs.Empty);
			}
		}

		public void SetSelected(EGender p_gender, Int32 p_index)
		{
			m_selected = (m_gender == p_gender && m_picIndex == p_index);
			if (m_selected)
			{
				m_BG.spriteName = "BTN_square_152_highlight";
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!m_isHovered) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				m_BG.spriteName = "BTN_square_152";
				TweenColor.Begin(m_BG.gameObject, 0.1f, (!m_isHovered) ? m_pBackgroundNormalColor : m_hoverColor);
			}
		}
	}
}
