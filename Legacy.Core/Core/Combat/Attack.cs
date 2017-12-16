using System;
using System.Collections.Generic;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Combat
{
	public class Attack
	{
		private Single m_attackValue;

		private Single m_criticalChance;

		private List<Damage> m_damages = new List<Damage>();

		public Attack(Single p_attackValue, Single p_criticalChance, Damage p_damages) : this(p_attackValue, p_criticalChance)
		{
			m_damages.Add(p_damages);
		}

		public Attack(Single p_attackValue, Single p_criticalChance, IList<Damage> p_damages) : this(p_attackValue, p_criticalChance)
		{
			m_damages.AddRange(p_damages);
		}

		public Attack(Single p_attackValue, Single p_criticalChance)
		{
			m_attackValue = p_attackValue;
			m_criticalChance = p_criticalChance;
		}

		public List<Damage> Damages => m_damages;

	    public Single CriticalChance
		{
			get => m_criticalChance;
	        set => m_criticalChance = value;
	    }

		public Single AttackValue
		{
			get => m_attackValue;
		    set => m_attackValue = value;
		}

		public EEquipSlots AttackHand { get; set; }
	}
}
