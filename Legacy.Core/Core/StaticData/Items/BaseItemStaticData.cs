using System;
using Dumper.Core;

namespace Legacy.Core.StaticData.Items
{
	public class BaseItemStaticData : BaseStaticData
	{
		[CsvColumn("Name")]
		protected String m_nameKey;

		[CsvColumn("Icon")]
		protected String m_icon;

		[CsvColumn("Price")]
		protected Int32 m_price;

		public BaseItemStaticData()
		{
			m_nameKey = String.Empty;
			m_icon = String.Empty;
			m_price = 1;
		}

		public String NameKey => m_nameKey;

	    public String Icon => m_icon;

	    public Int32 Price => m_price;
	}
}
