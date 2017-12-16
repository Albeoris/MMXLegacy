using System;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core
{
	public struct MMTime : ISaveGameObject, IEquatable<MMTime>
	{
		public const Int32 HOURS_PER_DAY = 24;

		public const Int32 MINUTES_PER_HOUR = 60;

		public const Int32 MINUTES_PER_DAY = 1440;

		private Int32 m_totalMinutes;

		public MMTime(Int32 p_minutes, Int32 p_hours, Int32 p_days)
		{
			m_totalMinutes = p_minutes + p_hours * 60 + p_days * 1440;
		}

		public MMTime(Int32 p_totalMinutes)
		{
			m_totalMinutes = p_totalMinutes;
		}

		public MMTime(MMTime source)
		{
			m_totalMinutes = source.m_totalMinutes;
		}

		public Int32 TotalMinutes => m_totalMinutes;

	    public Double TotalHours => m_totalMinutes / 60.0;

	    public Double TotalDays => m_totalMinutes / 1440.0;

	    public Int32 Minutes => m_totalMinutes % 60;

	    public Int32 Hours => m_totalMinutes / 60 % 24;

	    public Int32 Days => m_totalMinutes / 1440;

	    public void AddMinutes(Int32 p_minutes)
		{
			m_totalMinutes += p_minutes;
		}

		public void RemoveMinutes(Int32 p_minutes)
		{
			m_totalMinutes -= p_minutes;
		}

		public override Boolean Equals(Object obj)
		{
			return obj is MMTime && Equals((MMTime)obj);
		}

		public Boolean Equals(MMTime other)
		{
			return m_totalMinutes == other.m_totalMinutes;
		}

		public override Int32 GetHashCode()
		{
			return m_totalMinutes.GetHashCode();
		}

		public override String ToString()
		{
			return String.Format("[MMTime: Minutes={0}, Hours={1}, Days={2}]", Minutes, Hours, Days);
		}

		public void Load(SaveGameData p_data)
		{
			m_totalMinutes = p_data.Get<Int32>("TotalMinutes", 0);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("TotalMinutes", m_totalMinutes);
		}

		public static MMTime operator +(MMTime p_t1, MMTime p_t2)
		{
			return new MMTime(p_t1.m_totalMinutes + p_t2.m_totalMinutes);
		}

		public static MMTime operator -(MMTime p_t1, MMTime p_t2)
		{
			return new MMTime(p_t1.m_totalMinutes - p_t2.m_totalMinutes);
		}

		public static Boolean operator !=(MMTime p_t1, MMTime p_t2)
		{
			return !p_t1.Equals(p_t2);
		}

		public static Boolean operator ==(MMTime p_t1, MMTime p_t2)
		{
			return p_t1.Equals(p_t2);
		}

		public static Boolean operator >=(MMTime p_t1, MMTime p_t2)
		{
			return p_t1.Equals(p_t2) || p_t1.m_totalMinutes > p_t2.m_totalMinutes;
		}

		public static Boolean operator <=(MMTime p_t1, MMTime p_t2)
		{
			return p_t1.Equals(p_t2) || p_t1.m_totalMinutes < p_t2.m_totalMinutes;
		}

		public static Boolean operator >(MMTime p_t1, MMTime p_t2)
		{
			return p_t1.m_totalMinutes > p_t2.m_totalMinutes;
		}

		public static Boolean operator <(MMTime p_t1, MMTime p_t2)
		{
			return p_t1.m_totalMinutes < p_t2.m_totalMinutes;
		}
	}
}
