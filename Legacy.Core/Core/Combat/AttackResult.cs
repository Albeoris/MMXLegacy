using System;
using System.Collections.Generic;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.Combat
{
	public class AttackResult
	{
		private Attack m_reflectedDamage;

		private Object m_reflectedDamageSource;

		private List<DamageResult> m_damageResults;

		private EResultType m_result;

		private Equipment m_brokenItem;

		public AttackResult()
		{
			m_damageResults = new List<DamageResult>();
			m_brokenItem = null;
			DamagePercentOfHealthPoints = 1f;
		}

		public Int32 DamageDone
		{
			get
			{
				Int32 num = 0;
				foreach (DamageResult damageResult in m_damageResults)
				{
					num += damageResult.EffectiveValue;
				}
				return num;
			}
		}

		public Single PercentDamage
		{
			get
			{
				Single num = 0f;
				foreach (DamageResult damageResult in m_damageResults)
				{
					if (damageResult.Percentage > num)
					{
						num = damageResult.Percentage;
					}
				}
				return num;
			}
		}

		public Single DamagePercentOfHealthPoints { get; set; }

		public Attack ReflectedDamage
		{
			get => m_reflectedDamage;
		    set => m_reflectedDamage = value;
		}

		public Object ReflectedDamageSource
		{
			get => m_reflectedDamageSource;
		    set => m_reflectedDamageSource = value;
		}

		public List<DamageResult> DamageResults => m_damageResults;

	    public EResultType Result
		{
			get => m_result;
	        set => m_result = value;
	    }

		public Equipment BrokenItem
		{
			get => m_brokenItem;
		    set => m_brokenItem = value;
		}
	}
}
