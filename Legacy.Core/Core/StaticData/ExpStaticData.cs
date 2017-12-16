using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class ExpStaticData : BaseStaticData
	{
		[CsvColumn("DeltaExp")]
		protected Int32 m_deltaExp;

		public ExpStaticData()
		{
			m_deltaExp = 0;
		}

		public Int32 DeltaExp => m_deltaExp;
	}
}
