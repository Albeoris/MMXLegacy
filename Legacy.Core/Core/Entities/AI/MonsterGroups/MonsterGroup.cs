using System;
using System.Collections.Generic;

namespace Legacy.Core.Entities.AI.MonsterGroups
{
	public class MonsterGroup
	{
		private List<Monster> m_monsters;

		private Boolean m_alerted;

		public MonsterGroup(Int32 p_id)
		{
			m_monsters = new List<Monster>();
		}

		public void Add(Monster p_monster)
		{
			if (m_alerted)
			{
				Alert();
			}
			m_monsters.Add(p_monster);
		}

		public void Remove(Monster p_monster)
		{
			m_monsters.Remove(p_monster);
		}

		public void Alert()
		{
			if (!m_alerted)
			{
				m_alerted = true;
				for (Int32 i = 0; i < m_monsters.Count; i++)
				{
					m_monsters[i].IsAggro = true;
				}
			}
			else
			{
				for (Int32 j = 0; j < m_monsters.Count; j++)
				{
					if (!m_monsters[j].IsAggro)
					{
						m_monsters[j].IsAggro = true;
					}
				}
			}
		}

		public void SyncHP(Int32 p_newHP)
		{
			for (Int32 i = 0; i < m_monsters.Count; i++)
			{
				if (m_monsters[i].CurrentHealth != p_newHP)
				{
					m_monsters[i].ChangeHP(p_newHP - m_monsters[i].CurrentHealth, null);
				}
			}
		}

		internal void Clear()
		{
			m_monsters.Clear();
		}
	}
}
