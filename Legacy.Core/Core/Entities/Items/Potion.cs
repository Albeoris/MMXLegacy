using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;
using Legacy.Core.UpdateLogic;

namespace Legacy.Core.Entities.Items
{
	public class Potion : Consumable, IDescribable
	{
		private PotionStaticData m_staticData;

		private Dictionary<String, String> m_properties;

		protected override BaseItemStaticData BaseData => m_staticData;

	    public EPotionTarget Target => m_staticData.TargetAttribute;

	    public EPotionOperation Operation => m_staticData.Operation;

	    public Int32 Value => m_staticData.Value;

	    public EPotionType PotionType => m_staticData.Type;

	    public Int32 ModelLevel => m_staticData.ModelLevel;

	    public override void Init(Int32 p_staticID)
		{
			m_staticData = StaticDataHandler.GetStaticData<PotionStaticData>(EDataType.POTION, p_staticID);
			m_properties = new Dictionary<String, String>();
			InitIncreasingProperties();
			InitRemovingProperties();
		}

		private void InitIncreasingProperties()
		{
			if (Operation == EPotionOperation.INCREASE_ABS || Operation == EPotionOperation.INCREASE_ABS_PERM)
			{
				switch (Target)
				{
				case EPotionTarget.HP:
					m_properties["POTION_RESTORE_HEALTH_TT"] = Value.ToString();
					break;
				case EPotionTarget.MANA:
					m_properties["POTION_RESTORE_MANA_TT"] = Value.ToString();
					break;
				case EPotionTarget.MIGHT:
					m_properties["POTION_INCREASE_MIGHT_TT"] = Value.ToString();
					break;
				case EPotionTarget.MAGIC:
					m_properties["POTION_INCREASE_MAGIC_TT"] = Value.ToString();
					break;
				case EPotionTarget.PERCEPTION:
					m_properties["POTION_INCREASE_PERCEPTION_TT"] = Value.ToString();
					break;
				case EPotionTarget.DESTINY:
					m_properties["POTION_INCREASE_DESTINY_TT"] = Value.ToString();
					break;
				case EPotionTarget.VITALITY:
					m_properties["POTION_INCREASE_VITALIY_TT"] = Value.ToString();
					break;
				case EPotionTarget.SPIRIT:
					m_properties["POTION_INCREASE_SPIRIT_TT"] = Value.ToString();
					break;
				case EPotionTarget.BASE_HP:
					m_properties["POTION_INCREASE_HEALTH_TT"] = Value.ToString();
					break;
				case EPotionTarget.BASE_MANA:
					m_properties["POTION_INCREASE_MANA_TT"] = Value.ToString();
					break;
				case EPotionTarget.ALL_ATTRIBUTES:
					m_properties["POTION_INCREASE_ALL_STATS_TT"] = Value.ToString();
					break;
				case EPotionTarget.ALL_RESISTANCES:
					m_properties["POTION_INCREASE_ALL_RESIS_TT"] = Value.ToString();
					break;
				case EPotionTarget.FIRE_RESISTANCE:
					m_properties["POTION_INCREASE_FIRE_TT"] = Value.ToString();
					break;
				case EPotionTarget.WATER_RESISTANCE:
					m_properties["POTION_INCREASE_WATER_TT"] = Value.ToString();
					break;
				case EPotionTarget.AIR_RESISTANCE:
					m_properties["POTION_INCREASE_AIR_TT"] = Value.ToString();
					break;
				case EPotionTarget.EARTH_RESISTANCE:
					m_properties["POTION_INCREASE_EARTH_TT"] = Value.ToString();
					break;
				case EPotionTarget.LIGHT_RESISTANCE:
					m_properties["POTION_INCREASE_LIGHT_TT"] = Value.ToString();
					break;
				case EPotionTarget.DARK_RESISTANCE:
					m_properties["POTION_INCREASE_DARK_TT"] = Value.ToString();
					break;
				}
			}
			if (Operation == EPotionOperation.INCREASE_PROZ)
			{
				EPotionTarget target = Target;
				if (target == EPotionTarget.MANA_AND_HP)
				{
					m_properties["POTION_RESTORATION_TT"] = String.Empty;
				}
			}
		}

		private void InitRemovingProperties()
		{
			if (Operation == EPotionOperation.REMOVE)
			{
				switch (Target)
				{
				case EPotionTarget.CONDITION_CONFUSED:
					m_properties["POTION_REMOVE_CONFUSED_TT"] = String.Empty;
					break;
				case EPotionTarget.CONDITION_POISONED:
					m_properties["POTION_REMOVE_POISONED_TT"] = String.Empty;
					break;
				case EPotionTarget.CONDITION_WEAK:
					m_properties["POTION_REMOVE_WEAK_TT"] = String.Empty;
					break;
				}
			}
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
		}

		public override EDataType GetItemType()
		{
			return EDataType.POTION;
		}

		public override void Consume(InventorySlotRef p_slot, Int32 p_targetCharacter)
		{
			ConsumeCommand p_command = new ConsumeCommand(p_slot, p_targetCharacter);
			LegacyLogic.Instance.CommandManager.AddCommand(p_command);
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
