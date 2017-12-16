using System;
using Legacy.Core.Entities;

namespace Legacy.Core.EventManagement
{
	public class MonsterBuffUpdateEventArgs : EventArgs
	{
		private Monster m_monster;

		public MonsterBuffUpdateEventArgs(Monster p_monster)
		{
			m_monster = p_monster;
		}

		public Monster Monster => m_monster;
	}
}
