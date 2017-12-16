using System;
using Dumper.Core;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.StaticData;

namespace Legacy.Core
{
	public class TrapEffectStaticData : BaseStaticData
	{
		[CsvColumn("NameKey")]
		protected String m_name;

		[CsvColumn("TrapEffect")]
		protected ETrapEffect m_trapEffect;

		[CsvColumn("Gfx")]
		protected String m_gfx;

		[CsvColumn("AbsoluteValueMin")]
		protected Int32 m_absoluteValueMin;

		[CsvColumn("AbsoluteValueMax")]
		protected Int32 m_absoluteValueMax;

		[CsvColumn("PercentageValue")]
		protected Int32 m_PercentageValue;

		[CsvColumn("Refresh")]
		protected ETrapRefresh m_refresh;

		[CsvColumn("MenuPath")]
		protected String m_menuPath;

		public String Name => m_name;

	    public ETrapEffect TrapEffect => m_trapEffect;

	    public String GFX => m_gfx;

	    public Int32 AbsoluteValueMin => m_absoluteValueMin;

	    public Int32 AbsoluteValueMax => m_absoluteValueMax;

	    public Int32 PercentageValue => m_PercentageValue;

	    public ETrapRefresh Refresh => m_refresh;

	    public String MenuPath => m_menuPath;
	}
}
