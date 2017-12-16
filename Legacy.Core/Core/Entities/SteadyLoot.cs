using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Entities
{
	public struct SteadyLoot
	{
		public readonly EDataType ItemClass;

		public readonly Int32 ItemID;

		public readonly IntRange Amount;

		public readonly Single DropChance;

		public SteadyLoot(EDataType p_itemClass, Int32 p_itemID, Int32 p_min, Int32 p_max, Single p_dropChance)
		{
			ItemClass = p_itemClass;
			ItemID = p_itemID;
			Amount.Min = p_min;
			Amount.Max = p_max;
			DropChance = p_dropChance;
		}
	}
}
