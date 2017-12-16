using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData
{
	public class ItemOfferStaticData : BaseStaticData
	{
		[CsvColumn("RefreshType")]
		private EOfferRefreshType m_refreshType;

		[CsvColumn("ItemEntries")]
		private ItemOffer[] m_itemOffers;

		public EOfferRefreshType RefreshType => m_refreshType;

	    public ItemOffer[] ItemOffers => m_itemOffers;
	}
}
