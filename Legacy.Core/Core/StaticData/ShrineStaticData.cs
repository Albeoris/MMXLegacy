using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class ShrineStaticData : BaseStaticData
	{
		[CsvColumn("TokenID")]
		private Int32 m_tokenID;

		[CsvColumn("WeekDay")]
		private MMCalendar.EWeekDays m_weekDay;

		[CsvColumn("Caption")]
		private String m_caption;

		[CsvColumn("RightText")]
		private String m_rightText;

		[CsvColumn("WrongText")]
		private String m_wrongText;

		public ShrineStaticData()
		{
			m_tokenID = 0;
			m_weekDay = MMCalendar.EWeekDays.ASHDA;
			m_caption = null;
			m_rightText = null;
			m_wrongText = null;
		}

		public Int32 TokenID => m_tokenID;

	    public MMCalendar.EWeekDays WeekDay => m_weekDay;

	    public String Caption => m_caption;

	    public String RightText => m_rightText;

	    public String WrongText => m_wrongText;
	}
}
