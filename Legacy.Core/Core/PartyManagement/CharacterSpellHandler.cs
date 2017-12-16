using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.PartyManagement
{
	public class CharacterSpellHandler : ISaveGameObject
	{
		private List<CharacterSpell> m_spells;

		private Character m_character;

		public CharacterSpellHandler(Character p_character)
		{
			if (p_character == null)
			{
				throw new ArgumentNullException("p_character");
			}
			m_character = p_character;
			m_spells = new List<CharacterSpell>();
		}

		public Int32 SpellCount => m_spells.Count;

	    public void Init()
		{
			if (m_spells.Count > 0)
			{
				m_spells.Clear();
			}
		}

		public void AddSpell(ECharacterSpell p_spell)
		{
			AddSpell(p_spell, true);
		}

		public void AddSpell(ECharacterSpell p_spell, Boolean p_track)
		{
			if (HasSpell(p_spell))
			{
				return;
			}
			CharacterSpell characterSpell = SpellFactory.CreateCharacterSpell(p_spell);
			if (CanLearnSpell(characterSpell))
			{
				m_spells.Add(characterSpell);
				if (p_track)
				{
					LegacyLogic.Instance.TrackingManager.TrackSpellLearned(characterSpell, m_character);
					LegacyLogic.Instance.EventManager.InvokeEvent(m_character, EEventType.CHARACTER_LEARNED_SPELL, EventArgs.Empty);
				}
			}
		}

		public void RemoveSpell(ECharacterSpell p_spell)
		{
			foreach (CharacterSpell characterSpell in m_spells)
			{
				if (characterSpell.StaticData.StaticID == (Int32)p_spell)
				{
					m_spells.Remove(characterSpell);
					break;
				}
			}
		}

		public Boolean CanLearnSpell(CharacterSpell p_spell)
		{
			if (p_spell.StaticData.ClassOnly != EClass.NONE)
			{
				return true;
			}
			if (p_spell != null && p_spell.StaticData != null && GetSpell(p_spell.SpellType) == null)
			{
				Skill skill = m_character.SkillHandler.FindSkill((Int32)p_spell.StaticData.SkillID);
				if (skill != null && skill.Tier >= p_spell.StaticData.Tier)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean CouldLearnSpell(CharacterSpell p_spell)
		{
			if (p_spell != null && p_spell.StaticData != null && GetSpell(p_spell.SpellType) == null)
			{
				Skill skill = m_character.SkillHandler.FindSkill((Int32)p_spell.StaticData.SkillID);
				if (skill != null && skill.MaxTier >= p_spell.StaticData.Tier)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean HasSpell(ECharacterSpell p_spell)
		{
			foreach (CharacterSpell characterSpell in m_spells)
			{
				if (characterSpell.StaticData.StaticID == (Int32)p_spell)
				{
					return true;
				}
			}
			return false;
		}

		public CharacterSpell GetSpell(ECharacterSpell p_spell)
		{
			foreach (CharacterSpell characterSpell in m_spells)
			{
				if (characterSpell.StaticData.StaticID == (Int32)p_spell)
				{
					return characterSpell;
				}
			}
			return null;
		}

		public List<CharacterSpell> GetSpells()
		{
			return m_spells;
		}

		public List<CharacterSpell> GetSpellsBySchool(ESkillID p_skill_id)
		{
			List<CharacterSpell> list = new List<CharacterSpell>();
			foreach (CharacterSpell characterSpell in m_spells)
			{
				if (characterSpell.StaticData.SkillID == p_skill_id)
				{
					list.Add(characterSpell);
				}
			}
			return list;
		}

		public void Load(SaveGameData p_data)
		{
			List<Int32> list = SaveGame.CreateArrayFromData<Int32>(p_data);
			if (list != null)
			{
				for (Int32 i = 0; i < list.Count; i++)
				{
					AddSpell((ECharacterSpell)list[i], false);
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			for (Int32 i = 0; i < m_spells.Count; i++)
			{
				p_data.Set<Int32>("data_" + i, m_spells[i].StaticID);
			}
		}
	}
}
