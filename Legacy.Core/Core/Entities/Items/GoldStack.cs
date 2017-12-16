using System;
using Legacy.Core.Internationalization;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public class GoldStack : BaseItem
	{
		private Int32 m_amount;

		public GoldStack(Int32 p_amount)
		{
			m_amount = p_amount;
		}

		protected override BaseItemStaticData BaseData => null;

	    public override String Name => Localization.Instance.GetText("GOLD_TT");

	    public override String Icon => "ICO_ressource_gold";

	    public Int32 Amount => m_amount;

	    public override Int32 Price => m_amount;

	    public override Int32 StaticId => 0;

	    public override EDataType GetItemType()
		{
			return EDataType.GOLD_STACK;
		}

		public override void Init(Int32 p_staticID)
		{
		}

		public override void Load(SaveGameData p_data)
		{
			m_amount = p_data.Get<Int32>("Amount", 1);
		}

		public override void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("Amount", m_amount);
		}
	}
}
