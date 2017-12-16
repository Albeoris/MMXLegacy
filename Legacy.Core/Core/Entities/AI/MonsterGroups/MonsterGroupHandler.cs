using System;
using System.Collections.Generic;

namespace Legacy.Core.Entities.AI.MonsterGroups
{
	public class MonsterGroupHandler
	{
		public const Int32 CAGE_GROUP = 9999;

		public const Int32 EXPLOSIVE_CAGE_GROUP = 10000;

		private Dictionary<Int32, MonsterGroup> m_monsterGroups;

		public MonsterGroupHandler()
		{
			m_monsterGroups = new Dictionary<Int32, MonsterGroup>();
		}

		public MonsterGroup GetGroup(Int32 p_id)
		{
			if (!m_monsterGroups.ContainsKey(p_id))
			{
				m_monsterGroups.Add(p_id, new MonsterGroup(p_id));
			}
			return m_monsterGroups[p_id];
		}

		public Boolean AddMonsterToGroup(Monster p_monster)
		{
			if (p_monster.MonsterGroupID > 0)
			{
				GetGroup(p_monster.MonsterGroupID).Add(p_monster);
				return true;
			}
			return false;
		}

		public void RemoveMonsterFromGroup(Monster p_monster)
		{
			if (p_monster.MonsterGroupID > 0)
			{
				GetGroup(p_monster.MonsterGroupID).Remove(p_monster);
			}
		}

		public void Alert(Monster p_monster)
		{
			if (p_monster.MonsterGroupID > 0)
			{
				GetGroup(p_monster.MonsterGroupID).Alert();
			}
		}

		public void SyncHP(Int32 p_ID, Int32 newHP)
		{
			if (p_ID != 9999 && p_ID != 10000)
			{
				return;
			}
			GetGroup(p_ID).SyncHP(newHP);
		}

		internal void Clear()
		{
			foreach (MonsterGroup monsterGroup in m_monsterGroups.Values)
			{
				monsterGroup.Clear();
			}
			m_monsterGroups.Clear();
		}
	}
}
