using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public class Jewelry : Equipment, IDescribable
	{
		private JewelryStaticData m_staticData;

		private Dictionary<String, String> m_properties;

		protected override BaseItemStaticData BaseData => m_staticData;

	    protected override EquipmentStaticData BaseEquipmentData => m_staticData;

	    public override EItemSlot ItemSlot
		{
			get
			{
				EEquipmentType type = m_staticData.Type;
				if (type != EEquipmentType.NECKLACE)
				{
					return EItemSlot.ITEM_SLOT_RING;
				}
				return EItemSlot.ITEM_SLOT_NECKLACE;
			}
		}

		public override void Init(Int32 p_staticID)
		{
			m_staticData = StaticDataHandler.GetStaticData<JewelryStaticData>(EDataType.JEWELRY, p_staticID);
			m_identified = m_staticData.Identified;
			InitProperties();
			InitPrefixes();
			InitSuffixes();
		}

		public override void InitFromModel(Int32 p_staticId, Int32 p_prefixId, Int32 p_suffixId)
		{
			EquipmentStaticData staticData = StaticDataHandler.GetStaticData<JewelryStaticData>(EDataType.JEWELRY_MODEL, p_staticId);
			InitFromModel(staticData, p_prefixId, p_suffixId);
		}

		public override void InitFromModel(EquipmentStaticData p_staticData, Int32 p_prefixId, Int32 p_suffixId)
		{
			base.InitFromModel(p_staticData, p_prefixId, p_suffixId);
			m_staticData = (JewelryStaticData)p_staticData;
			InitProperties();
			InitPrefix(p_prefixId);
			InitSuffix(p_suffixId);
		}

		private void InitProperties()
		{
			m_properties = new Dictionary<String, String>();
		}

		public override EDataType GetItemType()
		{
			return EDataType.JEWELRY;
		}

		public override void FillFightValues(Boolean p_offHand, FightValues p_fightValues)
		{
		}

		public override void ModifyProperties()
		{
		}

		public String GetTypeDescription()
		{
			return m_staticData.Type.ToString();
		}

		public Dictionary<String, String> GetPropertiesDescription()
		{
			return m_properties;
		}
	}
}
