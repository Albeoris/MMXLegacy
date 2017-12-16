using System;
using Dumper.Core;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.StaticData.Items
{
	public class ScrollStaticData : BaseItemStaticData
	{
		[CsvColumn("SpellID")]
		protected Int32 m_spellID;

		[CsvColumn("ScrollTier")]
		protected ETier m_scrollTier;

		[CsvColumn("SpellTier")]
		protected ETier m_spellTier;

		[CsvColumn("MagicSchool")]
		protected EDamageType m_magicSchool;

		public ScrollStaticData()
		{
			m_spellID = 0;
			m_scrollTier = ETier.NONE;
		}

		public Int32 SpellID => m_spellID;

	    public ETier ScrollTier => m_scrollTier;

	    public ETier SpellTier => m_spellTier;

	    public EDamageType MagicSchool => m_magicSchool;
	}
}
