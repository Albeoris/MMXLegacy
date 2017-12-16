using System;
using Legacy.Core.Abilities;
using Legacy.Core.Entities;

namespace Legacy.Core.EventManagement
{
	public class AbilityEventArgs : EventArgs
	{
		private Monster m_monster;

		private MonsterAbilityBase m_ability;

		public AbilityEventArgs(Monster p_monster, MonsterAbilityBase p_ability)
		{
			m_monster = p_monster;
			m_ability = p_ability;
		}

		public Monster Monster => m_monster;

	    public MonsterAbilityBase Ability => m_ability;
	}
}
