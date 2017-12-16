using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SpellViewPanel")]
	public class SpellViewPanel : MonoBehaviour
	{
		private const Int32 VIEWS_PER_PAGE = 11;

		[SerializeField]
		private SpellView[] m_spellViews;

		[SerializeField]
		private TabController m_spellTabs;

		[SerializeField]
		private SpellbookPager m_pager;

		[SerializeField]
		private UICheckbox m_checkBox;

		private Dictionary<ECharacterSpell, CharacterSpell> m_spells;

		private Party m_party;

		private Int32 m_currentViewNr;

		private Int32 m_page;

		private Int32 m_pageCount = 1;

		private Boolean m_open;

		public Int32 Page => m_page;

	    public void Init(Party p_party)
		{
			m_party = p_party;
			m_checkBox.isChecked = true;
			UICheckbox checkBox = m_checkBox;
			checkBox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(checkBox.onStateChange, new UICheckbox.OnStateChange(OnCheckboxStateChanged));
			m_spellTabs.TabIndexChanged += OnSpellTabSelected;
			ECharacterSpell[] values = EnumUtil<ECharacterSpell>.Values;
			m_spells = new Dictionary<ECharacterSpell, CharacterSpell>();
			for (Int32 i = 0; i < values.Length - 1; i++)
			{
				try
				{
					m_spells[values[i]] = SpellFactory.CreateCharacterSpell(values[i]);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.UPDATE_AVAILABLE_ACTIONS, new EventHandler(OnUpdateAvailableActions));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.UPDATE_AVAILABLE_ACTIONS, new EventHandler(OnUpdateAvailableActions));
		}

		private void OnSpellTabSelected(Object p_sender, EventArgs p_args)
		{
			UpdatePage(0);
		}

		public void Open()
		{
			m_spellTabs.SelectTab(0, true);
			UpdateTabs();
			m_open = true;
		}

		public void Close()
		{
			m_open = false;
		}

		public void CharacterChanged()
		{
			UpdateTabs();
			if (!m_spellTabs.Tabs[m_spellTabs.CurrentTabIndex].IsEnabled)
			{
				m_spellTabs.SelectTab(0, true);
			}
			else
			{
				UpdatePage(0);
			}
		}

		public void OnUpdateAvailableActions(Object p_sender, EventArgs p_args)
		{
			if (!m_open)
			{
				return;
			}
			for (Int32 i = 0; i < 11; i++)
			{
				m_spellViews[i].UpdateData();
			}
		}

		public void UpdatePage(Int32 p_page)
		{
			m_page = p_page;
			UpdateContent();
			m_pager.UpdatePager(m_page, m_pageCount);
		}

		public void UpdateTabs()
		{
			CharacterSkillHandler skillHandler = m_party.SelectedCharacter.SkillHandler;
			m_spellTabs.Tabs[2].SetEnabled(skillHandler.FindSkill(11) != null);
			m_spellTabs.Tabs[3].SetEnabled(skillHandler.FindSkill(25) != null);
			m_spellTabs.Tabs[4].SetEnabled(skillHandler.FindSkill(19) != null);
			m_spellTabs.Tabs[5].SetEnabled(skillHandler.FindSkill(20) != null);
			m_spellTabs.Tabs[6].SetEnabled(skillHandler.FindSkill(22) != null);
			m_spellTabs.Tabs[7].SetEnabled(skillHandler.FindSkill(21) != null);
			m_spellTabs.Tabs[8].SetEnabled(skillHandler.FindSkill(23) != null);
			m_spellTabs.Tabs[9].SetEnabled(skillHandler.FindSkill(24) != null);
		}

		public void UpdateContent()
		{
			m_currentViewNr = 0;
			switch (m_spellTabs.CurrentTabIndex)
			{
			case 0:
				FillActions();
				FindWarfare(true);
				FillAllSpells();
				break;
			case 1:
				FillActions();
				FindWarfare(true);
				break;
			case 2:
				FindWarfare(false);
				break;
			case 3:
				FillSpellsForSchool(ESkillID.SKILL_PRIMORDIAL_MAGIC);
				break;
			case 4:
				FillSpellsForSchool(ESkillID.SKILL_FIRE_MAGIC);
				break;
			case 5:
				FillSpellsForSchool(ESkillID.SKILL_WATER_MAGIC);
				break;
			case 6:
				FillSpellsForSchool(ESkillID.SKILL_EARTH_MAGIC);
				break;
			case 7:
				FillSpellsForSchool(ESkillID.SKILL_AIR_MAGIC);
				break;
			case 8:
				FillSpellsForSchool(ESkillID.SKILL_LIGHT_MAGIC);
				break;
			case 9:
				FillSpellsForSchool(ESkillID.SKILL_DARK_MAGIC);
				break;
			}
			m_pageCount = (m_currentViewNr - 1) / 11 + 1;
			while (m_currentViewNr < (m_page + 1) * 11)
			{
				Int32 num = m_currentViewNr - m_page * 11;
				if (num >= 0)
				{
					m_spellViews[num].Hide();
				}
				m_currentViewNr++;
			}
		}

		private void FillAllSpells()
		{
			FindWarfare(false);
			FillSpellsForSchool(ESkillID.SKILL_PRIMORDIAL_MAGIC);
			FillSpellsForSchool(ESkillID.SKILL_FIRE_MAGIC);
			FillSpellsForSchool(ESkillID.SKILL_WATER_MAGIC);
			FillSpellsForSchool(ESkillID.SKILL_EARTH_MAGIC);
			FillSpellsForSchool(ESkillID.SKILL_AIR_MAGIC);
			FillSpellsForSchool(ESkillID.SKILL_LIGHT_MAGIC);
			FillSpellsForSchool(ESkillID.SKILL_DARK_MAGIC);
		}

		private void FindWarfare(Boolean p_paragon)
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			List<CharacterSpell> spellsBySchool = selectedCharacter.SpellHandler.GetSpellsBySchool(ESkillID.SKILL_WARFARE);
			if (m_checkBox.isChecked)
			{
				foreach (CharacterSpell characterSpell in spellsBySchool)
				{
					if ((p_paragon && characterSpell.StaticData.ClassOnly == selectedCharacter.Class.Class) || (!p_paragon && characterSpell.StaticData.ClassOnly == EClass.NONE))
					{
						InitSpellView(characterSpell);
					}
				}
			}
			else
			{
				Skill skill = selectedCharacter.SkillHandler.FindSkill(11);
				foreach (KeyValuePair<ECharacterSpell, CharacterSpell> keyValuePair in m_spells)
				{
					CharacterSpellStaticData staticData = keyValuePair.Value.StaticData;
					if (staticData != null)
					{
						Boolean flag;
						if (p_paragon)
						{
							flag = (staticData.SkillID == ESkillID.SKILL_WARFARE && skill != null && staticData.ClassOnly == selectedCharacter.Class.Class);
						}
						else
						{
							flag = (staticData.SkillID == ESkillID.SKILL_WARFARE && skill != null && staticData.Tier <= skill.MaxTier && staticData.ClassOnly == EClass.NONE);
						}
						if (flag)
						{
							InitSpellView(keyValuePair.Value);
						}
					}
				}
			}
		}

		private void FillSpellsForSchool(ESkillID p_school)
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			List<CharacterSpell> spellsBySchool = selectedCharacter.SpellHandler.GetSpellsBySchool(p_school);
			if (m_checkBox.isChecked)
			{
				foreach (CharacterSpell p_spell in spellsBySchool)
				{
					InitSpellView(p_spell);
				}
			}
			else
			{
				Skill skill = selectedCharacter.SkillHandler.FindSkill((Int32)p_school);
				foreach (KeyValuePair<ECharacterSpell, CharacterSpell> keyValuePair in m_spells)
				{
					CharacterSpellStaticData staticData = keyValuePair.Value.StaticData;
					if (staticData != null)
					{
						if (staticData.SkillID == p_school && skill != null && staticData.Tier <= skill.MaxTier && (staticData.ClassOnly == EClass.NONE || staticData.ClassOnly == selectedCharacter.Class.Class))
						{
							InitSpellView(keyValuePair.Value);
						}
					}
				}
			}
		}

		private void FillActions()
		{
			InitSpellView(EQuickActionType.ATTACK);
			InitSpellView(EQuickActionType.ATTACKRANGED);
			InitSpellView(EQuickActionType.DEFEND);
			InitSpellView(EQuickActionType.USE_BEST_HEALTHPOTION);
			InitSpellView(EQuickActionType.USE_BEST_MANAPOTION);
			CharacterClass @class = m_party.SelectedCharacter.Class;
			List<RacialAbilitiesStaticData> racialAbilities = @class.GetRacialAbilities();
			foreach (RacialAbilitiesStaticData p_rsd in racialAbilities)
			{
				InitSpellView(p_rsd);
			}
			List<ParagonAbilitiesStaticData> paragonAbilities = @class.GetParagonAbilities(m_checkBox.isChecked);
			foreach (ParagonAbilitiesStaticData paragonAbilitiesStaticData in paragonAbilities)
			{
				if (paragonAbilitiesStaticData.Passive)
				{
					InitSpellView(paragonAbilitiesStaticData);
				}
			}
		}

		private void InitSpellView(CharacterSpell p_spell)
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			if (m_currentViewNr >= m_page * 11 && m_currentViewNr < (m_page + 1) * 11)
			{
				Int32 num = m_currentViewNr - m_page * 11;
				m_spellViews[num].SetSpell(p_spell, selectedCharacter);
			}
			m_currentViewNr++;
		}

		private void InitSpellView(EQuickActionType p_action)
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			if (m_currentViewNr >= m_page * 11 && m_currentViewNr < (m_page + 1) * 11)
			{
				Int32 num = m_currentViewNr - m_page * 11;
				m_spellViews[num].SetAction(p_action, selectedCharacter);
			}
			m_currentViewNr++;
		}

		private void InitSpellView(RacialAbilitiesStaticData p_rsd)
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			if (m_currentViewNr >= m_page * 11 && m_currentViewNr < (m_page + 1) * 11)
			{
				Int32 num = m_currentViewNr - m_page * 11;
				m_spellViews[num].SetRacialAbility(p_rsd, selectedCharacter);
			}
			m_currentViewNr++;
		}

		private void InitSpellView(ParagonAbilitiesStaticData p_psd)
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			if (m_currentViewNr >= m_page * 11 && m_currentViewNr < (m_page + 1) * 11)
			{
				Int32 num = m_currentViewNr - m_page * 11;
				m_spellViews[num].SetParagonAbility(p_psd, selectedCharacter);
			}
			m_currentViewNr++;
		}

		private void OnCheckboxStateChanged(Boolean p_state)
		{
			UpdatePage(0);
		}

		internal void ChangeParty(Party p_party)
		{
			m_party = p_party;
		}

		private enum ESpellTabs
		{
			ALL,
			ACTIONS,
			WARFARES,
			PRIME,
			FIRE,
			WATER,
			EARTH,
			AIR,
			LIGHT,
			DARK
		}
	}
}
