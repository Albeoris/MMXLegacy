using System;
using System.Collections.Generic;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.Entities
{
	public class MonsterSpellHandler
	{
		private List<MonsterSpell> m_spells;

		private Monster m_owner;

		private List<Character> m_spellTargetList;

		public MonsterSpellHandler(Monster p_owner)
		{
			m_spells = new List<MonsterSpell>();
			m_spellTargetList = new List<Character>();
			m_owner = p_owner;
		}

		public MonsterSpell LastCastedSpell { get; set; }

		public List<Character> SpellTargetList => m_spellTargetList;

	    public Int32 Count => m_spells.Count;

	    public MonsterSpell this[Int32 index] => m_spells[index];

	    public void AddSpell(MonsterSpell p_spell)
		{
			if (!m_spells.Contains(p_spell))
			{
				p_spell.CriticalDamage = m_owner.StaticData.CriticalDamageSpells;
				p_spell.MonsterMagicPower = m_owner.MagicPower;
				p_spell.CriticalHitChance = m_owner.StaticData.CriticalHitChanceSpells;
				m_spells.Add(p_spell);
			}
		}

		public void UpdateMagicPower()
		{
			foreach (MonsterSpell monsterSpell in m_spells)
			{
				monsterSpell.MonsterMagicPower = m_owner.MagicPower;
			}
		}

		public void StartTurn()
		{
			m_spellTargetList.Clear();
			LastCastedSpell = null;
			foreach (MonsterSpell monsterSpell in m_spells)
			{
				monsterSpell.StartTurn();
			}
		}

		public MonsterSpell SelectSpellByID(Int32 p_staticID)
		{
			for (Int32 i = 0; i < m_spells.Count; i++)
			{
				if (m_spells[i].StaticID == p_staticID)
				{
					return m_spells[i];
				}
			}
			return null;
		}

		public MonsterSpell SelectSpell()
		{
			Int32 num = 0;
			List<MonsterSpell> list = new List<MonsterSpell>();
			for (Int32 i = 0; i < m_spells.Count; i++)
			{
				if (m_owner.AiHandler.CastSpell(m_spells[i]) && m_spells[i].CheckSpellPreconditions(m_owner))
				{
					list.Add(m_spells[i]);
					num += m_spells[i].CastProbability;
				}
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			if (list.Count > 1)
			{
				Int32 num2 = Random.Range(0, num);
				Int32 num3 = num;
				list.Sort(new Comparison<MonsterSpell>(SortDesc));
				for (Int32 j = 0; j < list.Count; j++)
				{
					num3 -= list[j].CastProbability;
					if (num3 <= num2)
					{
						return list[j];
					}
				}
			}
			return null;
		}

		private static Int32 SortDesc(MonsterSpell p_spellA, MonsterSpell p_spellB)
		{
			return p_spellB.CastProbability.CompareTo(p_spellA.CastProbability);
		}
	}
}
