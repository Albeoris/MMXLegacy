using System;
using System.Threading;
using Legacy.Core.Entities.Skills;
using Legacy.Core.StaticData;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/SkillSelectButton")]
	public class SkillSelectButton : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private UISprite m_tierIcon;

		[SerializeField]
		private UISprite m_tierIconBG;

		[SerializeField]
		private UISprite m_selectionMarker;

		[SerializeField]
		private UISprite m_corners;

		[SerializeField]
		private UISprite m_frame;

		[SerializeField]
		private GameObject m_selectionEffect;

		[SerializeField]
		private Color m_disabledColor;

		[SerializeField]
		private Color m_normalColor;

		[SerializeField]
		private Color m_highLightColor;

		private Boolean m_selected;

		private Boolean m_isEnabled;

		private SkillStaticData m_skill;

		private Skill m_skillForTT;

		private Boolean m_isDefault;

		public event EventHandler OnSkillSelected;

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public void Init(SkillStaticData p_skill, Boolean p_isDefault, Boolean p_isEnabled, Boolean p_selected, ETier p_maxTier)
		{
			m_skill = p_skill;
			gameObject.SetActive(true);
			m_icon.spriteName = p_skill.Icon;
			ETier p_maxTier2 = (p_maxTier <= ETier.NOVICE) ? ETier.NOVICE : p_maxTier;
			m_skillForTT = new Skill(p_skill.StaticID, p_maxTier2);
			m_skillForTT.Level = ((!p_selected) ? 0 : 1);
			m_isDefault = p_isDefault;
			m_isEnabled = p_isEnabled;
			m_selected = p_selected;
			NGUITools.SetActiveSelf(m_selectionEffect, p_selected);
			if (p_maxTier == ETier.GRAND_MASTER)
			{
				m_tierIcon.spriteName = "ICO_skillrank_3";
				NGUITools.SetActive(m_tierIcon.gameObject, true);
				NGUITools.SetActive(m_tierIconBG.gameObject, true);
			}
			else if (p_maxTier == ETier.MASTER)
			{
				m_tierIcon.spriteName = "ICO_skillrank_2";
				NGUITools.SetActive(m_tierIcon.gameObject, true);
				NGUITools.SetActive(m_tierIconBG.gameObject, true);
			}
			else if (p_maxTier == ETier.EXPERT)
			{
				m_tierIcon.spriteName = "ICO_skillrank_1";
				NGUITools.SetActive(m_tierIcon.gameObject, true);
				NGUITools.SetActive(m_tierIconBG.gameObject, true);
			}
			else
			{
				NGUITools.SetActive(m_tierIcon.gameObject, false);
				NGUITools.SetActive(m_tierIconBG.gameObject, false);
			}
			NGUITools.SetActive(m_corners.gameObject, m_isDefault);
			if (m_selected)
			{
				m_icon.color = m_normalColor;
				m_tierIconBG.color = m_highLightColor;
				m_corners.color = m_highLightColor;
				m_frame.color = m_highLightColor;
			}
			else if (!m_isEnabled)
			{
				m_icon.color = m_disabledColor;
				m_tierIconBG.color = m_normalColor;
				m_corners.color = m_normalColor;
				m_frame.color = m_disabledColor;
			}
			else
			{
				m_icon.color = m_normalColor;
				m_tierIconBG.color = m_normalColor;
				m_corners.color = m_normalColor;
				m_frame.color = m_normalColor;
			}
		}

		private void OnClick()
		{
			if (OnSkillSelected != null && m_isEnabled)
			{
				OnSkillSelected(this, EventArgs.Empty);
			}
		}

		private void OnHover(Boolean p_isOver)
		{
			if (!m_isEnabled)
			{
				NGUITools.SetActive(m_selectionMarker.gameObject, false);
				return;
			}
			NGUITools.SetActive(m_selectionMarker.gameObject, p_isOver);
		}

		private void OnTooltip(Boolean show)
		{
			if (show && m_skillForTT != null)
			{
				TooltipManager.Instance.Show(this, m_skillForTT, SkillTooltip.TooltipType.PARTY_CREATION, m_isDefault, m_icon.gameObject.transform.position, new Vector3(75f, 0f, 0f));
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		public Int32 GetSkillID()
		{
			if (m_skill != null)
			{
				return m_skill.StaticID;
			}
			return 0;
		}
	}
}
