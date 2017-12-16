using System;
using Dumper.Core;
using Legacy.Core.StaticData;

namespace Legacy.Core
{
	public class TokenStaticData : BaseStaticData
	{
		[CsvColumn("Icon")]
		protected String m_icon;

		[CsvColumn("Name")]
		protected String m_name;

		[CsvColumn("Description")]
		protected String m_description;

		[CsvColumn("Menupath")]
		protected String m_menupath;

		[CsvColumn("TokenVisible")]
		protected Boolean m_tokenVisible;

		[CsvColumn("SetID")]
		protected Int32 m_setID;

		[CsvColumn("Replacement")]
		protected Int32 m_replacement;

		[CsvColumn("RemoveSilent")]
		protected Boolean m_removeSilent;

		public String Icon => m_icon;

	    public String Name => m_name;

	    public String Description => m_description;

	    public String Menupath => m_menupath;

	    public Boolean TokenVisible => m_tokenVisible;

	    public Int32 SetID => m_setID;

	    public Int32 Replacement => m_replacement;

	    public Boolean RemoveSilent => m_removeSilent;
	}
}
