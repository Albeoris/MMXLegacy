using System;

namespace Legacy.Core
{
	public class MMCalendar
	{
		public const Int32 DAYS_PER_WEEK = 7;

		public const Int32 WEEKS_PER_MONTH = 4;

		public const Int32 DAYS_PER_MONTH = 28;

		public const Int32 MONTHS_PER_YEAR = 12;

		public const Int32 DAYS_PER_YEAR = 336;

		public const Int32 START_YEAR = 575;

		public const Int32 START_MONTH = 2;

		private EWeekDays m_weekDay;

		private Int32 m_day;

		private EMonths m_month;

		private Int32 m_year;

		public EWeekDays WeekDay => m_weekDay;

	    public Int32 Day => m_day;

	    public EMonths Month => m_month;

	    public Int32 Year => m_year;

	    public void Init(EWeekDays p_weekDay, Int32 p_day, EMonths p_month, Int32 p_year)
		{
			m_weekDay = p_weekDay;
			m_day = p_day;
			m_month = p_month;
			m_year = p_year;
		}

		public void SetDateByPassedTime(Int32 p_days)
		{
			p_days++;
			p_days += 56;
			Int32 num = p_days % 7 - 1;
			if (num < 0)
			{
				num = 6;
			}
			Int32 num2 = (Int32)Math.Floor(p_days / 336.0);
			p_days -= num2 * 336;
			num2 += 575;
			Int32 num3 = (Int32)Math.Floor(p_days / 28.0);
			p_days -= num3 * 28;
			if (p_days == 0)
			{
				num3--;
				p_days = 28;
			}
			Init((EWeekDays)num, p_days, (EMonths)num3, num2);
		}

		public void NextDay()
		{
			m_day++;
			if (m_day > 28)
			{
				m_day = 1;
				if (m_month == EMonths.WHITE_MAIDEN)
				{
					m_year++;
					m_month = EMonths.MOON_MOTHER;
				}
				else
				{
					m_month++;
				}
			}
			if (m_weekDay == EWeekDays.YLDA)
			{
				m_weekDay = EWeekDays.ASHDA;
			}
			else
			{
				m_weekDay++;
			}
		}

		public override String ToString()
		{
			String format = "[MMCalendar: WeekDay={0}, Day={1}, Month={2}, Year={3}]";
			Object[] array = new Object[4];
			Int32 num = 0;
			Int32 weekDay = (Int32)m_weekDay;
			array[num] = weekDay.ToString();
			array[1] = m_day.ToString();
			Int32 num2 = 2;
			Int32 month = (Int32)m_month;
			array[num2] = month.ToString();
			array[3] = m_year.ToString();
			return String.Format(format, array);
		}

		public enum EMonths
		{
			MOON_MOTHER,
			NIGHT_VEIL,
			SUN_BLOSSOM,
			EMERALD_SONG,
			AZURE_TIDES,
			DANCING_FLAMES,
			LAUGHING_WINDS,
			BLOOD_MOON,
			SHINING_STAR,
			RADIANT_CROWN,
			SPIDER_QUEEN,
			WHITE_MAIDEN
		}

		public enum EWeekDays
		{
			ASHDA,
			MALDA,
			ELDA,
			ARDA,
			SYLADA,
			SHALDA,
			YLDA,
			COUNT
		}
	}
}
