using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public abstract class BaseStaticData
	{
		[CsvColumn("StaticID")]
		protected Int32 m_staticID;

		public Int32 StaticID => m_staticID;

	    public virtual void PostDeserialization()
		{
		}
	}
}
