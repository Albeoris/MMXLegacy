using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class LoreBookStaticData : BaseStaticData
	{
		[CsvColumn("TitleKey")]
		private String m_titleKey;

		[CsvColumn("TextKey")]
		private String m_textKey;

		[CsvColumn("AuthorKey")]
		private String m_authorKey;

		[CsvColumn("Category")]
		private ELoreBookCategories m_category;

		[CsvColumn("ImagePath")]
		private String m_imageName;

		public LoreBookStaticData()
		{
			m_titleKey = null;
			m_textKey = null;
			m_authorKey = null;
			m_imageName = null;
		}

		public String TitleKey => m_titleKey;

	    public String TextKey => m_textKey;

	    public String AuthorKey => m_authorKey;

	    public ELoreBookCategories Category => m_category;

	    public String ImageName => m_imageName;
	}
}
