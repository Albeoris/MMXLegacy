using System;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.PartyCreate
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/PartyCreationSkills")]
	public class PartyCreationSkills : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_nameLabel;

		[SerializeField]
		private UILabel m_raceClassLabel;

		[SerializeField]
		private UISprite m_portrait;

		[SerializeField]
		private UISprite m_body;

		[SerializeField]
		private UILabel m_skillsLeftLabel;

		[SerializeField]
		private SkillSelectButton[] m_weaponBtns;

		[SerializeField]
		private SkillSelectButton[] m_micsBtns;

		[SerializeField]
		private SkillSelectButton[] m_MagicBtns;

		private PartyCreator m_partyCreator;

		public void Init(PartyCreator p_partyCreator)
		{
			m_partyCreator = p_partyCreator;
			Cleanup();
			for (Int32 i = 0; i < m_weaponBtns.Length; i++)
			{
				m_weaponBtns[i].OnSkillSelected += OnSkillSelected;
			}
			for (Int32 j = 0; j < m_MagicBtns.Length; j++)
			{
				m_MagicBtns[j].OnSkillSelected += OnSkillSelected;
			}
			for (Int32 k = 0; k < m_micsBtns.Length; k++)
			{
				m_micsBtns[k].OnSkillSelected += OnSkillSelected;
			}
		}

		public void Cleanup()
		{
			for (Int32 i = 0; i < m_weaponBtns.Length; i++)
			{
				m_weaponBtns[i].OnSkillSelected -= OnSkillSelected;
			}
			for (Int32 j = 0; j < m_MagicBtns.Length; j++)
			{
				m_MagicBtns[j].OnSkillSelected -= OnSkillSelected;
			}
			for (Int32 k = 0; k < m_micsBtns.Length; k++)
			{
				m_micsBtns[k].OnSkillSelected -= OnSkillSelected;
			}
		}

		public void OnAfterActivate()
		{
			UpdateCharacter();
			UpdateSkillSelection();
		}

		public void UndoSelection()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.ResetPickedSkills();
		}

		public void OnResetClick()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.ResetPickedSkills();
			UpdateSkillSelection();
		}

		public void OnDefaultClick()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.SetStartSkills();
			UpdateSkillSelection();
		}

		private void UpdateCharacter()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			if (selectedDummyCharacter.Class != EClass.NONE)
			{
				CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)selectedDummyCharacter.Class);
				m_nameLabel.text = selectedDummyCharacter.Name;
				String text = staticData.NameKey;
				if (selectedDummyCharacter.Gender == EGender.MALE)
				{
					text += "_M";
				}
				else
				{
					text += "_F";
				}
				m_raceClassLabel.text = LocaManager.GetText(selectedDummyCharacter.GetRaceKey()) + " " + LocaManager.GetText(text);
				m_portrait.spriteName = selectedDummyCharacter.GetPortrait();
				m_body.spriteName = selectedDummyCharacter.GetBodySprite();
			}
		}

		private void UpdateSkillSelection()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			String arg = "[000000]";
			if (selectedDummyCharacter.GetSkillsToPickLeft() > 0)
			{
				arg = "[008000]";
			}
			m_skillsLeftLabel.text = LocaManager.GetText("GUI_PARTY_CREATION_POINTS_LEFT", arg, selectedDummyCharacter.GetSkillsToPickLeft());
			if (selectedDummyCharacter.Class != EClass.NONE)
			{
				UpdateButton(m_weaponBtns[0], ESkillID.SKILL_SWORD);
				UpdateButton(m_weaponBtns[1], ESkillID.SKILL_AXE);
				UpdateButton(m_weaponBtns[2], ESkillID.SKILL_MACE);
				UpdateButton(m_weaponBtns[3], ESkillID.SKILL_DAGGER);
				UpdateButton(m_weaponBtns[4], ESkillID.SKILL_SPEAR);
				UpdateButton(m_weaponBtns[5], ESkillID.SKILL_BOW);
				UpdateButton(m_weaponBtns[6], ESkillID.SKILL_CROSSBOW);
				UpdateButton(m_micsBtns[0], ESkillID.SKILL_DUAL_WIELD);
				UpdateButton(m_micsBtns[1], ESkillID.SKILL_TWOHANDED);
				UpdateButton(m_micsBtns[2], ESkillID.SKILL_WARFARE);
				UpdateButton(m_micsBtns[3], ESkillID.SKILL_MEDIUM_ARMOR);
				UpdateButton(m_micsBtns[4], ESkillID.SKILL_HEAVY_ARMOR);
				UpdateButton(m_micsBtns[5], ESkillID.SKILL_SHIELD);
				UpdateButton(m_micsBtns[6], ESkillID.SKILL_ENDURANCE);
				UpdateButton(m_micsBtns[7], ESkillID.SKILL_DODGE);
				UpdateButton(m_micsBtns[8], ESkillID.SKILL_MYSTICISM);
				UpdateButton(m_micsBtns[9], ESkillID.SKILL_ARCANE_DISCIPLINE);
				UpdateButton(m_micsBtns[10], ESkillID.SKILL_MAGICAL_FOCUS);
				UpdateButton(m_MagicBtns[0], ESkillID.SKILL_PRIMORDIAL_MAGIC);
				UpdateButton(m_MagicBtns[1], ESkillID.SKILL_FIRE_MAGIC);
				UpdateButton(m_MagicBtns[2], ESkillID.SKILL_AIR_MAGIC);
				UpdateButton(m_MagicBtns[3], ESkillID.SKILL_LIGHT_MAGIC);
				UpdateButton(m_MagicBtns[4], ESkillID.SKILL_WATER_MAGIC);
				UpdateButton(m_MagicBtns[5], ESkillID.SKILL_EARTH_MAGIC);
				UpdateButton(m_MagicBtns[6], ESkillID.SKILL_DARK_MAGIC);
			}
		}

		private void UpdateButton(SkillSelectButton p_btn, ESkillID p_skill)
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)selectedDummyCharacter.Class);
			ETier etier = ETier.NONE;
			for (Int32 i = 0; i < staticData.GrandMasterSkills.Length; i++)
			{
				if (staticData.GrandMasterSkills[i] == (Int32)p_skill)
				{
					etier = ETier.GRAND_MASTER;
				}
			}
			for (Int32 j = 0; j < staticData.MasterSkills.Length; j++)
			{
				if (staticData.MasterSkills[j] == (Int32)p_skill)
				{
					etier = ETier.MASTER;
				}
			}
			for (Int32 k = 0; k < staticData.ExpertSkills.Length; k++)
			{
				if (staticData.ExpertSkills[k] == (Int32)p_skill)
				{
					etier = ETier.EXPERT;
				}
			}
			SkillStaticData staticData2 = StaticDataHandler.GetStaticData<SkillStaticData>(EDataType.SKILL, (Int32)p_skill);
			p_btn.Init(staticData2, isDefaultSkill(staticData, staticData2), !isStartSkill(staticData, staticData2) && etier != ETier.NONE, selectedDummyCharacter.IsSkillPicked(staticData2.StaticID) || isStartSkill(staticData, staticData2), etier);
		}

		public void OnSkillSelected(Object p_sender, EventArgs p_args)
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			Int32 skillID = ((SkillSelectButton)p_sender).GetSkillID();
			if (selectedDummyCharacter.IsSkillPicked(skillID))
			{
				selectedDummyCharacter.UnPickSkill(skillID);
			}
			else
			{
				selectedDummyCharacter.PickSkill(skillID);
			}
			UpdateSkillSelection();
		}

		private Boolean isDefaultSkill(CharacterClassStaticData sd, SkillStaticData skillData)
		{
			foreach (Int32 num in sd.StartSkills)
			{
				if (num == skillData.StaticID)
				{
					return true;
				}
			}
			return false;
		}

		private Boolean isStartSkill(CharacterClassStaticData sd, SkillStaticData skillData)
		{
			foreach (Int32 num in sd.StartSkills)
			{
				if (num == skillData.StaticID)
				{
					return true;
				}
			}
			return false;
		}
	}
}
