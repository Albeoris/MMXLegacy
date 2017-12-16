using System;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/SummarySkill")]
	public class SummarySkill : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private UISprite m_border;

		private Skill m_skillForTT;

		public void Init(SkillStaticData sd, DummyCharacter p_char)
		{
			if (sd != null)
			{
				ETier maxTier = GetMaxTier(p_char, sd);
				m_icon.spriteName = sd.Icon;
				NGUITools.SetActive(m_icon.gameObject, true);
				NGUITools.SetActive(m_border.gameObject, true);
				m_skillForTT = new Skill(sd.StaticID, maxTier);
				m_skillForTT.Level = 1;
			}
			else
			{
				NGUITools.SetActive(m_icon.gameObject, false);
				NGUITools.SetActive(m_border.gameObject, false);
				m_skillForTT = null;
			}
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnTooltip(Boolean show)
		{
			if (show && m_skillForTT != null)
			{
				TooltipManager.Instance.Show(this, m_skillForTT, SkillTooltip.TooltipType.PARTY_CREATION, false, m_icon.gameObject.transform.position, new Vector3(37f, 0f, 0f));
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		private ETier GetMaxTier(DummyCharacter p_char, SkillStaticData p_skill)
		{
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)p_char.Class);
			ETier result = ETier.NONE;
			for (Int32 i = 0; i < staticData.GrandMasterSkills.Length; i++)
			{
				if (staticData.GrandMasterSkills[i] == p_skill.StaticID)
				{
					result = ETier.GRAND_MASTER;
				}
			}
			for (Int32 j = 0; j < staticData.MasterSkills.Length; j++)
			{
				if (staticData.MasterSkills[j] == p_skill.StaticID)
				{
					result = ETier.MASTER;
				}
			}
			for (Int32 k = 0; k < staticData.ExpertSkills.Length; k++)
			{
				if (staticData.ExpertSkills[k] == p_skill.StaticID)
				{
					result = ETier.EXPERT;
				}
			}
			return result;
		}
	}
}
