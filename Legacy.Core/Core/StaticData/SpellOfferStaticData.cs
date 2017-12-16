using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class SpellOfferStaticData : BaseStaticData
	{
		[CsvColumn("SpellIDs")]
		private Int32[] m_spellIDs;

		public Int32[] SpellIDs => m_spellIDs;
	}
}
