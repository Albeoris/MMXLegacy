using System;
using Legacy.Core.StaticData;

namespace Legacy.Core
{
	public struct ItemOffer
	{
		public static readonly ItemOffer Zero = new ItemOffer(EDataType.NONE, 0, 0);

		public readonly EDataType ItemType;

		public readonly Int32 ItemID;

		public readonly Int32 ItemQuantity;

		public ItemOffer(EDataType p_itemType, Int32 p_itemID, Int32 p_itemQuantity)
		{
			ItemType = p_itemType;
			ItemID = p_itemID;
			ItemQuantity = p_itemQuantity;
		}
	}
}
