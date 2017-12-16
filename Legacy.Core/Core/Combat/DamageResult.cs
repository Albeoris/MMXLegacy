using System;

namespace Legacy.Core.Combat
{
	public struct DamageResult
	{
		public EDamageType Type;

		public Int32 EffectiveValue;

		public Int32 ResistedValue;

		public Single Percentage;

		public DamageResult(EDamageType p_type, Int32 p_effectiveValue, Int32 p_resistedValue, Single p_percentage)
		{
			Type = p_type;
			EffectiveValue = p_effectiveValue;
			ResistedValue = p_resistedValue;
			Percentage = p_percentage;
		}

		public static DamageResult Create(Damage damage, Resistance resistance)
		{
			if (damage.Type != resistance.Type)
			{
				throw new InvalidOperationException();
			}
			Damage damage2 = Damage.ResistedDamage(damage, resistance);
			return new DamageResult(damage.Type, damage.Value - damage2.Value, damage2.Value, damage.percentage);
		}

		public static DamageResult Create(Damage damage, Resistance[] resistances)
		{
			Damage damage2 = Damage.ResistedDamage(damage, resistances);
			return new DamageResult(damage.Type, damage.Value - damage2.Value, damage2.Value, damage.percentage);
		}
	}
}
