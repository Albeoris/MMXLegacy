using System;
using Dumper.Core;
using Legacy.Core.Map;
using Legacy.Core.StaticData;

namespace Legacy.Core
{
	public class ObelisksStaticData : BaseStaticData
	{
		[CsvColumn("NameKey")]
		private String m_nameKey;

		[CsvColumn("TokenID")]
		private Int32 m_tokenID;

		[CsvColumn("Position")]
		private Position m_Position;

		public ObelisksStaticData()
		{
			m_nameKey = String.Empty;
			m_tokenID = 0;
			m_Position = Position.Zero;
		}

		public String NameKey => m_nameKey;

	    public Int32 TokenID => m_tokenID;

	    public Position Position => m_Position;
	}
}
