using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class BaseAbilityStaticData : BaseStaticData
	{
		[CsvColumn("NameKey")]
		protected String m_nameKey = String.Empty;

		[CsvColumn("Icon")]
		protected String m_icon = String.Empty;

		public String NameKey => m_nameKey;

	    public String Icon => m_icon;
	}
}
