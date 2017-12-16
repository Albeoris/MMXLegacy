using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Entities.Items
{
	public abstract class Consumable : BaseItem
	{
		protected Int32 m_counter;

		public Consumable()
		{
			m_counter = 1;
		}

		public Int32 Counter
		{
			get => m_counter;
		    set => m_counter = value;
		}

		public override Int32 Price
		{
			get
			{
				Int32 num = base.Price;
				if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
				{
					num = (Int32)(num * ConfigManager.Instance.Game.ItemConsumablesFactor);
				}
				return num;
			}
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			m_counter = p_data.Get<Int32>("Counter", 1);
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<Int32>("Counter", m_counter);
		}

		public abstract void Consume(InventorySlotRef p_slot, Int32 p_targetCharacter);

		public static Boolean AreSameConsumables(BaseItem p_item1, BaseItem p_item2)
		{
			if (p_item1 == null || p_item2 == null)
			{
				return false;
			}
			Boolean flag = p_item1.GetType() == p_item2.GetType();
			Boolean flag2 = p_item1 is Consumable;
			Boolean flag3 = p_item1.StaticId == p_item2.StaticId;
			return flag && flag2 && flag3;
		}
	}
}
