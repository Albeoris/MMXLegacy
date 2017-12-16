using System;
using Dumper.Core;
using Legacy.Core.Hints;

namespace Legacy.Core.StaticData
{
	public class HintStaticData : BaseStaticData
	{
		[CsvColumn("Type")]
		private EHintType m_type;

		[CsvColumn("Category")]
		private EHintCategory m_category;

		[CsvColumn("Title")]
		private String m_title;

		[CsvColumn("Text")]
		private String m_text;

		public EHintType Type => m_type;

	    public EHintCategory Category => m_category;

	    public String Title => m_title;

	    public String Text => m_text;
	}
}
