using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SkillView")]
	public class SkillView : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_labelName;

		[SerializeField]
		private UILabel m_labelRank;

		[SerializeField]
		private SkillTierView m_tierView1;

		[SerializeField]
		private SkillTierView m_tierView2;

		[SerializeField]
		private SkillTierView m_tierView3;

		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private Color m_inactiveColor = new Color(0.5f, 0.5f, 0.5f);

		[SerializeField]
		private Color m_inactiveColorText = new Color(0.5f, 0.5f, 0.5f);

		[SerializeField]
		private Color m_bonusColor = Color.green;

		[SerializeField]
		private Vector3 m_tooltipOffset;

		[SerializeField]
		private SkillLevelButton m_levelUpButton;

		[SerializeField]
		private SkillLevelButton m_levelDownButton;

		[SerializeField]
		private GameObject m_levelUpButtonBackground;

		[SerializeField]
		private GameObject m_levelDownButtonBackground;

		[SerializeField]
		private SkillDisabledTooltip m_skillDisabledTooltip;

		private Skill m_skill;

		private Color m_labelRankColor;

		private Color m_labelNameColor;

		private Color m_iconColor;

		private String m_bonusColorHex;

		private CharacterSkillHandler m_skillHandler;

		public Skill Skill => m_skill;

	    private void Awake()
		{
			m_labelRankColor = m_labelRank.color;
			m_labelNameColor = m_labelName.color;
			m_bonusColorHex = "[" + NGUITools.EncodeColor(m_bonusColor) + "]";
			m_iconColor = m_icon.color;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_SKILL_CHANGED, new EventHandler(OnSkillChangedEvent));
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_SKILL_CHANGED, new EventHandler(OnSkillChangedEvent));
		}

		public void SetSkill(Skill p_skill, CharacterSkillHandler p_skillHandler)
		{
			NGUITools.SetActiveSelf(gameObject, true);
			m_skill = p_skill;
			m_skillHandler = p_skillHandler;
			m_labelName.text = LocaManager.GetText(m_skill.Name);
			m_levelUpButton.Init(p_skill, SkillLevelButton.ButtonType.PLUS);
			m_levelDownButton.Init(p_skill, SkillLevelButton.ButtonType.MINUS);
			m_skillDisabledTooltip.Init(p_skill);
			UpdateData();
			UpdateLevelUpButtons();
		}

		public void Hide()
		{
			m_skill = null;
			m_skillHandler = null;
			NGUITools.SetActiveSelf(gameObject, false);
		}

		public void UpdateData()
		{
			m_tierView1.SetTierReached(m_skill.Level == Skill.GetMaxLevelByTier(ETier.EXPERT), m_skill.Tier >= ETier.EXPERT);
			m_tierView2.SetTierReached(m_skill.Level == Skill.GetMaxLevelByTier(ETier.MASTER), m_skill.Tier >= ETier.MASTER);
			m_tierView3.SetTierReached(m_skill.Level == Skill.GetMaxLevelByTier(ETier.GRAND_MASTER), m_skill.Tier >= ETier.GRAND_MASTER);
			m_tierView1.SetAvailable(true);
			m_tierView2.SetAvailable(m_skill.Tier >= ETier.EXPERT);
			m_tierView3.SetAvailable(m_skill.Tier >= ETier.MASTER);
			m_tierView2.SetVisible(m_skill.MaxTier >= ETier.MASTER);
			m_tierView3.SetVisible(m_skill.MaxTier >= ETier.GRAND_MASTER);
			m_tierView1.SetDataForTooltip(m_skill, SkillTooltip.TooltipType.EXPERT);
			m_tierView2.SetDataForTooltip(m_skill, SkillTooltip.TooltipType.MASTER);
			m_tierView3.SetDataForTooltip(m_skill, SkillTooltip.TooltipType.GRAND_MASTER);
			if (m_skill.Tier == ETier.NONE)
			{
				m_labelRank.text = LocaManager.GetText("GUI_SKILLS_LEVEL_DESCRIPTION_UNLEARNED");
				m_labelRank.color = m_inactiveColorText;
				m_labelName.color = m_inactiveColorText;
				m_icon.color = m_inactiveColor;
				m_tierView1.SetFillAmount(0f, 0f);
				m_tierView2.SetFillAmount(0f, 0f);
				m_tierView3.SetFillAmount(0f, 0f);
			}
			else
			{
				String arg;
				if (m_skill.VirtualSkillLevel > 0)
				{
					arg = String.Concat(new Object[]
					{
						m_skill.Level,
						" (",
						m_bonusColorHex,
						"+",
						m_skill.VirtualSkillLevel,
						"[-])"
					});
				}
				else
				{
					arg = m_skill.Level.ToString();
				}
				m_labelRank.text = LocaManager.GetText("GUI_SKILLS_LEVEL_DESCRIPTION", GetTierText(m_skill.Tier), arg);
				m_labelRank.color = m_labelRankColor;
				m_labelName.color = m_labelNameColor;
				m_icon.color = m_iconColor;
				Single num = ConfigManager.Instance.Game.RequiredSkillLevelExpert;
				Single num2 = ConfigManager.Instance.Game.RequiredSkillLevelMaster;
				Single num3 = ConfigManager.Instance.Game.RequiredSkillLevelGrandMaster;
				if (m_skill.Tier == ETier.NOVICE)
				{
					m_tierView1.SetFillAmount(m_skill.Level / num, m_skill.TemporaryLevel / num);
					m_tierView2.SetFillAmount(0f, 0f);
					m_tierView3.SetFillAmount(0f, 0f);
				}
				else if (m_skill.Tier == ETier.EXPERT)
				{
					m_tierView1.SetFillAmount(1f, 0f);
					m_tierView2.SetFillAmount((m_skill.Level - num) / (num2 - num), m_skill.TemporaryLevel / (num2 - num));
					m_tierView3.SetFillAmount(0f, 0f);
				}
				else if (m_skill.Tier == ETier.MASTER)
				{
					m_tierView1.SetFillAmount(1f, 0f);
					m_tierView2.SetFillAmount(1f, 0f);
					m_tierView3.SetFillAmount((m_skill.Level - num2) / (num3 - num2), m_skill.TemporaryLevel / (num3 - num2));
				}
				else if (m_skill.Tier == ETier.GRAND_MASTER)
				{
					m_tierView1.SetFillAmount(1f, 0f);
					m_tierView2.SetFillAmount(1f, 0f);
					m_tierView3.SetFillAmount(1f, 0f);
				}
			}
			m_icon.spriteName = m_skill.Icon;
		}

		private void OnTooltip(Boolean show)
		{
			if (show)
			{
				TooltipManager.Instance.Show(this, m_skill, SkillTooltip.TooltipType.BASE, false, m_icon.gameObject.transform.position, m_tooltipOffset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		public void UpdateLevelUpButtons()
		{
			if (m_skillHandler != null)
			{
				m_levelUpButton.SetEnabled(!m_skill.IsTierMaxLevel);
				Boolean state = m_skillHandler.CanSpendSkillpoints && m_skill.Tier < m_skill.MaxTier;
				Boolean state2 = m_skill.TemporaryLevel > 0;
				NGUITools.SetActiveSelf(m_levelUpButton.gameObject, state);
				NGUITools.SetActiveSelf(m_levelUpButtonBackground, state);
				NGUITools.SetActiveSelf(m_levelDownButton.gameObject, state2);
				NGUITools.SetActiveSelf(m_levelDownButtonBackground, state2);
			}
		}

		public void OnSkillChangedEvent(Object p_sender, EventArgs p_args)
		{
			if (p_sender == m_skill)
			{
				UpdateData();
				UpdateLevelUpButtons();
				m_levelUpButton.UpdateTooltip();
				m_levelDownButton.UpdateTooltip();
			}
		}

		public void OnLevelUpButtonClicked()
		{
			if (m_skillHandler.CanSpendSkillpoints && !m_skill.IsTierMaxLevel)
			{
				m_skill.TemporaryLevel++;
				m_skillHandler.SpendSkillPoint();
				m_skillHandler.IncreaseSkillLevel(m_skill.StaticID);
			}
		}

		public void OnLevelDownButtonClicked()
		{
			if (m_skill.TemporaryLevel > 0)
			{
				m_skill.TemporaryLevel--;
				m_skillHandler.RefundSkillPoint();
				m_skillHandler.DecreaseSkillLevel(m_skill.StaticID);
			}
		}

		private Single GetProgressBarValue()
		{
			Single num = ConfigManager.Instance.Game.RequiredSkillLevelExpert;
			Single num2 = ConfigManager.Instance.Game.RequiredSkillLevelMaster;
			Single num3 = ConfigManager.Instance.Game.RequiredSkillLevelGrandMaster;
			switch (m_skill.Tier)
			{
			case ETier.NONE:
				return 0f;
			case ETier.NOVICE:
				return m_skill.Level / num / 3f;
			case ETier.EXPERT:
			{
				Single num4 = (m_skill.Level - num) / (num2 - num);
				return (1f + num4) / 3f;
			}
			case ETier.MASTER:
			{
				Single num5 = (m_skill.Level - num2) / (num3 - num2);
				return (2f + num5) / 3f;
			}
			case ETier.GRAND_MASTER:
				return 1f;
			default:
				return 0f;
			}
		}

		public static String GetTierText(ETier p_tier)
		{
			switch (p_tier)
			{
			case ETier.NOVICE:
				return LocaManager.GetText("SKILL_TIER_1");
			case ETier.EXPERT:
				return LocaManager.GetText("SKILL_TIER_2");
			case ETier.MASTER:
				return LocaManager.GetText("SKILL_TIER_3");
			case ETier.GRAND_MASTER:
				return LocaManager.GetText("SKILL_TIER_4");
			default:
				return String.Empty;
			}
		}
	}
}
