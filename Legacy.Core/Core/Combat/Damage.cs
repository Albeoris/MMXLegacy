using System;

namespace Legacy.Core.Combat
{
	public struct Damage
	{
		public EDamageType Type;

		public Int32 Value;

		public Single CriticalBonusMod;

		public Int32 IgnoreResistance;

		public Single IgnoreResistancePercent;

		public Single percentage;

		public Damage(EDamageType p_type, Int32 p_damageValue, Single p_criticalBonusMod, Single p_percentage)
		{
			Type = p_type;
			Value = p_damageValue;
			CriticalBonusMod = p_criticalBonusMod;
			IgnoreResistance = 0;
			IgnoreResistancePercent = 0f;
			percentage = p_percentage;
		}

		public static Damage Create(DamageData p_data, Single p_criticalBonusMod)
		{
			if (p_data.Minimum > p_data.Maximum)
			{
				throw new ArgumentOutOfRangeException("p_data", "Minimum > Maximum");
			}
			Int32 num = Random.Range(p_data.Minimum, p_data.Maximum + 1);
			Single p_percentage = (num - (p_data.Minimum - 1)) / (Single)(p_data.Maximum - (p_data.Minimum - 1));
			return new Damage(p_data.Type, num, p_criticalBonusMod, p_percentage);
		}

		public static Damage[] Create(DamageData[] p_datas, Single p_criticalBonusMod)
		{
			if (p_datas == null)
			{
				throw new ArgumentNullException("p_datas");
			}
			Damage[] array = new Damage[p_datas.Length];
			for (Int32 i = 0; i < array.Length; i++)
			{
				array[i] = Create(p_datas[i], p_criticalBonusMod);
			}
			return array;
		}

		public static Damage ResistedDamage(Damage left, Resistance resistance)
		{
			if (left.Type != resistance.Type)
			{
				throw new InvalidOperationException();
			}
			Single num = resistance.Value - left.IgnoreResistance;
			if (num > 0f)
			{
				num *= 1f - left.IgnoreResistancePercent;
			}
			if (num > 100f)
			{
				num = 100f;
			}
			return new Damage(left.Type, (Int32)Math.Round(left.Value * num / 100f, MidpointRounding.AwayFromZero), left.CriticalBonusMod, left.percentage);
		}

		public static Damage ResistedDamage(Damage left, Resistance[] resistances)
		{
			Damage result = new Damage(left.Type, 0, left.CriticalBonusMod, left.percentage);
			for (Int32 i = 0; i < resistances.Length; i++)
			{
				if (resistances[i].Type == left.Type)
				{
					result = ResistedDamage(left, resistances[i]);
				}
			}
			return result;
		}

		public static Damage operator *(Damage p_dmg, Single p_factor)
		{
			p_dmg.Value = (Int32)Math.Round(p_dmg.Value * p_factor, MidpointRounding.AwayFromZero);
			return p_dmg;
		}
	}
}
