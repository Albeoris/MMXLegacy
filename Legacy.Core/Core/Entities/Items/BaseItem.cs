using System;
using Legacy.Core.Internationalization;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public abstract class BaseItem : ISaveGameObject
	{
		protected Single m_priceMultiplicator = 1f;

		protected abstract BaseItemStaticData BaseData { get; }

		public Single PriceMultiplicator
		{
			get => m_priceMultiplicator;
		    set => m_priceMultiplicator = value;
		}

		public virtual String Name => Localization.Instance.GetText(BaseData.NameKey);

	    public virtual String Icon => BaseData.Icon;

	    public virtual Int32 Price => (Int32)Math.Ceiling(BaseData.Price * m_priceMultiplicator);

	    public virtual Int32 StaticId => BaseData.StaticID;

	    public abstract EDataType GetItemType();

		public abstract void Init(Int32 p_staticID);

		public virtual void Load(SaveGameData p_data)
		{
			Int32 p_staticID = p_data.Get<Int32>("StaticID", 0);
			Init(p_staticID);
			m_priceMultiplicator = p_data.Get<Single>("PriceMultiplicator", 1f);
		}

		public virtual void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("StaticID", StaticId);
			p_data.Set<Single>("PriceMultiplicator", m_priceMultiplicator);
		}
	}
}
