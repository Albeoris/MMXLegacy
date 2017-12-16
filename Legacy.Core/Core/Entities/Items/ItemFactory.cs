using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public static class ItemFactory
	{
		private static readonly EEquipmentType[] EQUIPMENT_CLASSES = new EEquipmentType[]
		{
			EEquipmentType.ARMOR,
			EEquipmentType.MELEE_WEAPON,
			EEquipmentType.RANGED_WEAPON,
			EEquipmentType.MAGIC_FOCUS,
			EEquipmentType.JEWELRY,
			EEquipmentType.SHIELD
		};

		private static readonly EEquipmentType[] ARMOR_TYPES = new EEquipmentType[]
		{
			EEquipmentType.GARMENT,
			EEquipmentType.GLOVE,
			EEquipmentType.BOOTS,
			EEquipmentType.HEADGEAR
		};

		private static readonly EEquipmentType[] GARMENT_SUBTYPES = new EEquipmentType[]
		{
			EEquipmentType.CLOTHING,
			EEquipmentType.LIGHT_ARMOR,
			EEquipmentType.HEAVY_ARMOR
		};

		private static readonly EEquipmentType[] ARMOR_SUBTYPES = new EEquipmentType[]
		{
			EEquipmentType.ARCANE,
			EEquipmentType.MARTIAL
		};

		private static readonly EEquipmentType[] MELEE_WEAPON_TYPES = new EEquipmentType[]
		{
			EEquipmentType.SWORD,
			EEquipmentType.AXE,
			EEquipmentType.MACE,
			EEquipmentType.DAGGER,
			EEquipmentType.SPEAR
		};

		private static readonly EEquipmentType[] MELEE_WEAPON_SUBTYPES = new EEquipmentType[]
		{
			EEquipmentType.ONEHANDED,
			EEquipmentType.TWOHANDED
		};

		private static readonly EEquipmentType[] RANGED_WEAPON_TYPES = new EEquipmentType[]
		{
			EEquipmentType.BOW,
			EEquipmentType.CROSSBOW
		};

		private static readonly EEquipmentType[] JEWELRY_TYPES = new EEquipmentType[]
		{
			EEquipmentType.NECKLACE,
			EEquipmentType.RING
		};

		private static readonly EEquipmentType[] SHIELD_TYPES = new EEquipmentType[]
		{
			EEquipmentType.SMALL_SHIELD,
			EEquipmentType.BIG_SHIELD
		};

		private static readonly EEquipmentType[] MAGIC_FOCUS_TYPES = new EEquipmentType[]
		{
			EEquipmentType.MAGIC_FOCUS_ONEHANDED,
			EEquipmentType.MAGIC_FOCUS_TWOHANDED
		};

		private static List<Int32> m_potionTempFilter;

		private static List<Int32> m_scrollTempFilter;

		private static List<EEquipmentType> m_itemTypeTempFilter;

		private static Dictionary<EEquipmentType, Single> m_itemProbabilities;

		private static Dictionary<EEquipmentType, EnchantmentProbabilityList> m_prefixProbabilities;

		private static Dictionary<EEquipmentType, EnchantmentProbabilityList> m_suffixProbabilities;

		private static Dictionary<EDamageType, Single> m_currentEnchantmentMultiplicators = EnchantmentProbabilityList.DefaultMultiplicators;

		public static void InitItemProbabilities()
		{
			if (m_itemProbabilities == null)
			{
				m_itemProbabilities = new Dictionary<EEquipmentType, Single>();
			}
			else
			{
				m_itemProbabilities.Clear();
			}
			IEnumerable<ItemProbabilityStaticData> iterator = StaticDataHandler.GetIterator<ItemProbabilityStaticData>(EDataType.ITEM_PROBABILITIES);
			foreach (ItemProbabilityStaticData itemProbabilityStaticData in iterator)
			{
				Single num = 0f;
				for (Int32 i = 0; i < 4; i++)
				{
					num += GetProbabilityForCharacter(itemProbabilityStaticData, LegacyLogic.Instance.WorldManager.Party.GetMember(i));
				}
				m_itemProbabilities.Add(itemProbabilityStaticData.Type, num);
			}
		}

		private static Single GetProbabilityForCharacter(ItemProbabilityStaticData p_data, Character p_character)
		{
			switch (p_character.Class.Class)
			{
			case EClass.MERCENARY:
				return p_data.Mercenary;
			case EClass.CRUSADER:
				return p_data.Crusader;
			case EClass.FREEMAGE:
				return p_data.Freemage;
			case EClass.BLADEDANCER:
				return p_data.Bladedancer;
			case EClass.RANGER:
				return p_data.Ranger;
			case EClass.DRUID:
				return p_data.Druid;
			case EClass.DEFENDER:
				return p_data.Defender;
			case EClass.SCOUT:
				return p_data.Scout;
			case EClass.RUNEPRIEST:
				return p_data.Runepriest;
			case EClass.BARBARIAN:
				return p_data.Barbarian;
			case EClass.HUNTER:
				return p_data.Hunter;
			case EClass.SHAMAN:
				return p_data.Shaman;
			default:
				return 1f;
			}
		}

		public static T CreateItem<T>(Int32 p_staticID) where T : BaseItem, new()
		{
			T result = Activator.CreateInstance<T>();
			result.Init(p_staticID);
			return result;
		}

		public static BaseItem CreateItem(EDataType p_type, Int32 p_staticID)
		{
			BaseItem baseItem;
			if (p_type == EDataType.GENERATED_EQUIPMENT)
			{
				GeneratedEquipmentStaticData staticData = StaticDataHandler.GetStaticData<GeneratedEquipmentStaticData>(p_type, p_staticID);
				m_currentEnchantmentMultiplicators = new Dictionary<EDamageType, Single>();
				m_currentEnchantmentMultiplicators[EDamageType.AIR] = staticData.Air;
				m_currentEnchantmentMultiplicators[EDamageType.EARTH] = staticData.Earth;
				m_currentEnchantmentMultiplicators[EDamageType.FIRE] = staticData.Fire;
				m_currentEnchantmentMultiplicators[EDamageType.WATER] = staticData.Water;
				m_currentEnchantmentMultiplicators[EDamageType.LIGHT] = staticData.Light;
				m_currentEnchantmentMultiplicators[EDamageType.DARK] = staticData.Dark;
				m_currentEnchantmentMultiplicators[EDamageType.PRIMORDIAL] = staticData.Primordial;
				m_currentEnchantmentMultiplicators[EDamageType.NONE] = staticData.None;
				baseItem = CreateEquipment(staticData.ModelLevels, staticData.PrefixChance, staticData.SuffixChance, staticData.PrefixProbabilities, staticData.SuffixProbabilities, staticData.SpecificationList);
				m_currentEnchantmentMultiplicators = EnchantmentProbabilityList.DefaultMultiplicators;
			}
			else if (p_type == EDataType.GENERATED_CONSUMABLES)
			{
				GeneratedConsumableStaticData staticData2 = StaticDataHandler.GetStaticData<GeneratedConsumableStaticData>(p_type, p_staticID);
				m_currentEnchantmentMultiplicators = new Dictionary<EDamageType, Single>();
				m_currentEnchantmentMultiplicators[EDamageType.AIR] = staticData2.Air;
				m_currentEnchantmentMultiplicators[EDamageType.EARTH] = staticData2.Earth;
				m_currentEnchantmentMultiplicators[EDamageType.FIRE] = staticData2.Fire;
				m_currentEnchantmentMultiplicators[EDamageType.WATER] = staticData2.Water;
				m_currentEnchantmentMultiplicators[EDamageType.LIGHT] = staticData2.Light;
				m_currentEnchantmentMultiplicators[EDamageType.DARK] = staticData2.Dark;
				m_currentEnchantmentMultiplicators[EDamageType.PRIMORDIAL] = staticData2.Primordial;
				m_currentEnchantmentMultiplicators[EDamageType.NONE] = 1f;
				baseItem = CreateConsumable(staticData2.ModelRange.Random(), staticData2.SpecificationList);
			}
			else
			{
				baseItem = CreateItem(p_type);
				baseItem.Init(p_staticID);
			}
			return baseItem;
		}

		public static Consumable CreateConsumable(Int32 p_modelLevel, EOfferConsumableType[] p_specificationList)
		{
			EOfferConsumableType eofferConsumableType;
			if (p_specificationList.Length == 0)
			{
				eofferConsumableType = (EOfferConsumableType)Random.Range(0, 4);
			}
			else
			{
				eofferConsumableType = p_specificationList[Random.Range(0, p_specificationList.Length)];
			}
			switch (eofferConsumableType)
			{
			case EOfferConsumableType.HEALTH_POTION:
				return CreatePotion(EPotionType.HEALTH_POTION, p_modelLevel);
			case EOfferConsumableType.MANA_POTION:
				return CreatePotion(EPotionType.MANA_POTION, p_modelLevel);
			case EOfferConsumableType.ELIXIR:
				return CreatePotion(EPotionType.DROPPABLE_ELIXIR, p_modelLevel);
			case EOfferConsumableType.SCROLL:
				return CreateScroll(p_modelLevel);
			default:
				return null;
			}
		}

		public static BaseItem CreateItem(EDataType p_type)
		{
			switch (p_type)
			{
			case EDataType.ARMOR:
				return new Armor();
			case EDataType.JEWELRY:
				return new Jewelry();
			case EDataType.SHIELD:
				return new Shield();
			case EDataType.MELEE_WEAPON:
				return new MeleeWeapon();
			case EDataType.MAGIC_FOCUS:
				return new MagicFocus();
			case EDataType.RANGED_WEAPON:
				return new RangedWeapon();
			case EDataType.POTION:
				return new Potion();
			case EDataType.SCROLL:
				return new Scroll();
			case EDataType.GOLD_STACK:
				return new GoldStack(0);
			}
			throw new NotSupportedException("Unknow type! " + p_type);
		}

		public static Potion CreatePotion(EPotionType p_type, Int32 p_modelLevel)
		{
			List<PotionStaticData> list = new List<PotionStaticData>(StaticDataHandler.GetIterator<PotionStaticData>(EDataType.POTION));
			if (m_potionTempFilter == null)
			{
				m_potionTempFilter = new List<Int32>();
			}
			m_potionTempFilter.Clear();
			for (Int32 i = 0; i < list.Count; i++)
			{
				PotionStaticData potionStaticData = list[i];
				if (potionStaticData.Type == p_type && potionStaticData.ModelLevel == p_modelLevel)
				{
					m_potionTempFilter.Add(i);
				}
			}
			if (m_potionTempFilter.Count > 0)
			{
				Int32 index = Random.Range(0, m_potionTempFilter.Count);
				Int32 index2 = m_potionTempFilter[index];
				return CreateItem<Potion>(list[index2].StaticID);
			}
			throw new NotSupportedException("Potion Data not found! " + p_type);
		}

		public static Scroll CreateScroll(Int32 p_modelLevel)
		{
			ETier etier = ETier.NOVICE;
			switch (p_modelLevel)
			{
			case 3:
				etier = ETier.EXPERT;
				break;
			case 4:
				etier = ETier.MASTER;
				break;
			case 5:
				etier = ETier.GRAND_MASTER;
				break;
			}
			List<ScrollStaticData> list = new List<ScrollStaticData>(StaticDataHandler.GetIterator<ScrollStaticData>(EDataType.SCROLL));
			if (m_scrollTempFilter == null)
			{
				m_scrollTempFilter = new List<Int32>();
			}
			m_scrollTempFilter.Clear();
			ETier etier2 = (ETier)Random.Range(1, (Int32)(etier + 1));
			Single num = 0f;
			for (Int32 i = 0; i < list.Count; i++)
			{
				ScrollStaticData scrollStaticData = list[i];
				if (scrollStaticData.ScrollTier == etier && scrollStaticData.SpellTier == etier2 && scrollStaticData.StaticID < 1000)
				{
					m_scrollTempFilter.Add(i);
					num += m_currentEnchantmentMultiplicators[scrollStaticData.MagicSchool];
				}
			}
			if (m_scrollTempFilter.Count > 0)
			{
				Single num2 = Random.Range(0f, num);
				Single num3 = 0f;
				for (Int32 j = 0; j < m_scrollTempFilter.Count; j++)
				{
					Int32 index = m_scrollTempFilter[j];
					num3 += m_currentEnchantmentMultiplicators[list[index].MagicSchool];
					if (num2 < num3)
					{
						return CreateItem<Scroll>(list[index].StaticID);
					}
				}
			}
			return null;
		}

		public static Equipment CreateEquipment(ModelProbability[] p_modelLevels, Single p_prefixChance, Single p_suffixChance, EnchantmentProbability[] p_prefixProbabilities, EnchantmentProbability[] p_suffixProbabilities, EEquipmentType[] p_specificationList)
		{
			if (m_itemProbabilities == null)
			{
				InitItemProbabilities();
			}
			if (m_prefixProbabilities == null)
			{
				IEnumerable<PrefixStaticData> iterator = StaticDataHandler.GetIterator<PrefixStaticData>(EDataType.PREFIX);
				m_prefixProbabilities = new Dictionary<EEquipmentType, EnchantmentProbabilityList>();
				m_prefixProbabilities[EEquipmentType.MELEE_WEAPON] = new EnchantmentProbabilityList(iterator, EEquipmentType.MELEE_WEAPON, EEquipmentType.NONE);
				m_prefixProbabilities[EEquipmentType.RANGED_WEAPON] = new EnchantmentProbabilityList(iterator, EEquipmentType.RANGED_WEAPON, EEquipmentType.NONE);
				m_prefixProbabilities[EEquipmentType.JEWELRY] = new EnchantmentProbabilityList(iterator, EEquipmentType.JEWELRY, EEquipmentType.NONE);
				m_prefixProbabilities[EEquipmentType.SHIELD] = new EnchantmentProbabilityList(iterator, EEquipmentType.SHIELD, EEquipmentType.NONE);
				m_prefixProbabilities[EEquipmentType.MAGIC_FOCUS] = new EnchantmentProbabilityList(iterator, EEquipmentType.MAGIC_FOCUS, EEquipmentType.NONE);
				m_prefixProbabilities[EEquipmentType.CLOTHING] = new EnchantmentProbabilityList(iterator, EEquipmentType.ARMOR, EEquipmentType.CLOTHING);
				m_prefixProbabilities[EEquipmentType.LIGHT_ARMOR] = new EnchantmentProbabilityList(iterator, EEquipmentType.ARMOR, EEquipmentType.LIGHT_ARMOR);
				m_prefixProbabilities[EEquipmentType.HEAVY_ARMOR] = new EnchantmentProbabilityList(iterator, EEquipmentType.ARMOR, EEquipmentType.HEAVY_ARMOR);
				m_prefixProbabilities[EEquipmentType.ARCANE] = new EnchantmentProbabilityList(iterator, EEquipmentType.ARMOR, EEquipmentType.ARCANE);
				m_prefixProbabilities[EEquipmentType.MARTIAL] = new EnchantmentProbabilityList(iterator, EEquipmentType.ARMOR, EEquipmentType.MARTIAL);
			}
			if (m_suffixProbabilities == null)
			{
				IEnumerable<SuffixStaticData> iterator2 = StaticDataHandler.GetIterator<SuffixStaticData>(EDataType.SUFFIX);
				m_suffixProbabilities = new Dictionary<EEquipmentType, EnchantmentProbabilityList>();
				m_suffixProbabilities[EEquipmentType.MELEE_WEAPON] = new EnchantmentProbabilityList(iterator2, EEquipmentType.MELEE_WEAPON, EEquipmentType.NONE);
				m_suffixProbabilities[EEquipmentType.RANGED_WEAPON] = new EnchantmentProbabilityList(iterator2, EEquipmentType.RANGED_WEAPON, EEquipmentType.NONE);
				m_suffixProbabilities[EEquipmentType.JEWELRY] = new EnchantmentProbabilityList(iterator2, EEquipmentType.JEWELRY, EEquipmentType.NONE);
				m_suffixProbabilities[EEquipmentType.SHIELD] = new EnchantmentProbabilityList(iterator2, EEquipmentType.SHIELD, EEquipmentType.NONE);
				m_suffixProbabilities[EEquipmentType.MAGIC_FOCUS] = new EnchantmentProbabilityList(iterator2, EEquipmentType.MAGIC_FOCUS, EEquipmentType.NONE);
				m_suffixProbabilities[EEquipmentType.CLOTHING] = new EnchantmentProbabilityList(iterator2, EEquipmentType.ARMOR, EEquipmentType.CLOTHING);
				m_suffixProbabilities[EEquipmentType.LIGHT_ARMOR] = new EnchantmentProbabilityList(iterator2, EEquipmentType.ARMOR, EEquipmentType.LIGHT_ARMOR);
				m_suffixProbabilities[EEquipmentType.HEAVY_ARMOR] = new EnchantmentProbabilityList(iterator2, EEquipmentType.ARMOR, EEquipmentType.HEAVY_ARMOR);
				m_suffixProbabilities[EEquipmentType.ARCANE] = new EnchantmentProbabilityList(iterator2, EEquipmentType.ARMOR, EEquipmentType.ARCANE);
				m_suffixProbabilities[EEquipmentType.MARTIAL] = new EnchantmentProbabilityList(iterator2, EEquipmentType.ARMOR, EEquipmentType.MARTIAL);
			}
			EEquipmentType eequipmentType = DetermineType(EQUIPMENT_CLASSES, p_specificationList);
			Boolean flag = Random.Value < p_prefixChance;
			Boolean flag2 = Random.Value < p_suffixChance;
			Int32 p_suffixLevel = -1;
			Int32 p_prefixLevel = -1;
			ModelProbability modelProbability = DetermineModelLevel(p_modelLevels);
			if (flag)
			{
				p_prefixLevel = DetermineEnchantmentLevel(p_prefixProbabilities);
			}
			if (flag2)
			{
				p_suffixLevel = DetermineEnchantmentLevel(p_suffixProbabilities);
			}
			switch (eequipmentType)
			{
			case EEquipmentType.ARMOR:
				return CreateArmor(modelProbability.ModelLevel, modelProbability.SubModel, p_specificationList, p_prefixLevel, p_suffixLevel);
			case EEquipmentType.JEWELRY:
				return CreateJewelry(modelProbability.ModelLevel, modelProbability.SubModel, p_specificationList, p_prefixLevel, p_suffixLevel);
			case EEquipmentType.SHIELD:
				return CreateShield(modelProbability.ModelLevel, modelProbability.SubModel, p_specificationList, p_prefixLevel, p_suffixLevel);
			case EEquipmentType.MELEE_WEAPON:
				return CreateMeleeWeapon(modelProbability.ModelLevel, modelProbability.SubModel, p_specificationList, p_prefixLevel, p_suffixLevel);
			case EEquipmentType.MAGIC_FOCUS:
				return CreateMagicFocus(modelProbability.ModelLevel, modelProbability.SubModel, p_specificationList, p_prefixLevel, p_suffixLevel);
			case EEquipmentType.RANGED_WEAPON:
				return CreateRangedWeapon(modelProbability.ModelLevel, modelProbability.SubModel, p_specificationList, p_prefixLevel, p_suffixLevel);
			default:
				return null;
			}
		}

		private static ModelProbability DetermineModelLevel(ModelProbability[] p_modelLevels)
		{
			Single num = 0f;
			foreach (ModelProbability modelProbability in p_modelLevels)
			{
				num += modelProbability.Weight;
			}
			Single num2 = Random.Range(0f, num);
			Single num3 = 0f;
			foreach (ModelProbability result in p_modelLevels)
			{
				num3 += result.Weight;
				if (num2 < num3)
				{
					return result;
				}
			}
			return new ModelProbability(1, ESubModel.A, 1f);
		}

		private static Int32 DetermineEnchantmentLevel(EnchantmentProbability[] p_probabilities)
		{
			Single num = 0f;
			foreach (EnchantmentProbability enchantmentProbability in p_probabilities)
			{
				num += enchantmentProbability.Weight;
			}
			Single num2 = Random.Range(0f, num);
			Single num3 = 0f;
			foreach (EnchantmentProbability enchantmentProbability2 in p_probabilities)
			{
				num3 += enchantmentProbability2.Weight;
				if (num2 < num3)
				{
					return enchantmentProbability2.ModelLevel;
				}
			}
			return 1;
		}

		private static Equipment CreateArmor(Int32 p_modelLevel, ESubModel p_subModel, EEquipmentType[] p_specificationList, Int32 p_prefixLevel, Int32 p_suffixLevel)
		{
			IEnumerable<ArmorStaticData> iterator = StaticDataHandler.GetIterator<ArmorStaticData>(EDataType.ARMOR_MODEL);
			EEquipmentType eequipmentType = DetermineType(ARMOR_TYPES, p_specificationList);
			EEquipmentType eequipmentType2;
			if (eequipmentType == EEquipmentType.GARMENT)
			{
				eequipmentType2 = DetermineType(GARMENT_SUBTYPES, p_specificationList);
			}
			else
			{
				eequipmentType2 = DetermineType(ARMOR_SUBTYPES, p_specificationList);
			}
			foreach (ArmorStaticData armorStaticData in iterator)
			{
				if (armorStaticData.Type == eequipmentType && armorStaticData.Subtype == eequipmentType2 && armorStaticData.ModelLevel == p_modelLevel && armorStaticData.SubModel == p_subModel)
				{
					Int32 p_suffixId = -1;
					Int32 p_prefixId = -1;
					if (p_prefixLevel > 0)
					{
						m_prefixProbabilities[eequipmentType2].Multiplicators = m_currentEnchantmentMultiplicators;
						p_prefixId = m_prefixProbabilities[eequipmentType2].GetRandomId();
					}
					if (p_suffixLevel > 0)
					{
						m_suffixProbabilities[eequipmentType2].Multiplicators = m_currentEnchantmentMultiplicators;
						p_suffixId = m_suffixProbabilities[eequipmentType2].GetRandomId();
					}
					Armor armor = new Armor();
					armor.InitFromModel(armorStaticData, p_prefixId, p_suffixId);
					armor.PrefixLevel = p_prefixLevel;
					armor.SuffixLevel = p_suffixLevel;
					return armor;
				}
			}
			return null;
		}

		private static Equipment CreateMeleeWeapon(Int32 p_modelLevel, ESubModel p_subModel, EEquipmentType[] p_specificationList, Int32 p_prefixLevel, Int32 p_suffixLevel)
		{
			IEnumerable<MeleeWeaponStaticData> iterator = StaticDataHandler.GetIterator<MeleeWeaponStaticData>(EDataType.MELEE_WEAPON_MODEL);
			EEquipmentType eequipmentType = DetermineType(MELEE_WEAPON_TYPES, p_specificationList);
			EEquipmentType eequipmentType2;
			if (eequipmentType == EEquipmentType.DAGGER)
			{
				eequipmentType2 = EEquipmentType.ONEHANDED;
			}
			else if (eequipmentType == EEquipmentType.SPEAR)
			{
				eequipmentType2 = EEquipmentType.TWOHANDED;
			}
			else
			{
				eequipmentType2 = DetermineType(MELEE_WEAPON_SUBTYPES, p_specificationList);
			}
			foreach (MeleeWeaponStaticData meleeWeaponStaticData in iterator)
			{
				if (meleeWeaponStaticData.Type == eequipmentType && meleeWeaponStaticData.Subtype == eequipmentType2 && meleeWeaponStaticData.ModelLevel == p_modelLevel && meleeWeaponStaticData.SubModel == p_subModel)
				{
					Int32 p_suffixId = -1;
					Int32 p_prefixId = -1;
					if (p_prefixLevel > 0)
					{
						m_prefixProbabilities[EEquipmentType.MELEE_WEAPON].Multiplicators = m_currentEnchantmentMultiplicators;
						p_prefixId = m_prefixProbabilities[EEquipmentType.MELEE_WEAPON].GetRandomId();
					}
					if (p_suffixLevel > 0)
					{
						m_suffixProbabilities[EEquipmentType.MELEE_WEAPON].Multiplicators = m_currentEnchantmentMultiplicators;
						p_suffixId = m_suffixProbabilities[EEquipmentType.MELEE_WEAPON].GetRandomId();
					}
					MeleeWeapon meleeWeapon = new MeleeWeapon();
					meleeWeapon.InitFromModel(meleeWeaponStaticData, p_prefixId, p_suffixId);
					meleeWeapon.PrefixLevel = p_prefixLevel;
					meleeWeapon.SuffixLevel = p_suffixLevel;
					return meleeWeapon;
				}
			}
			return null;
		}

		private static Equipment CreateRangedWeapon(Int32 p_modelLevel, ESubModel p_subModel, EEquipmentType[] p_specificationList, Int32 p_prefixLevel, Int32 p_suffixLevel)
		{
			IEnumerable<RangedWeaponStaticData> iterator = StaticDataHandler.GetIterator<RangedWeaponStaticData>(EDataType.RANGED_WEAPON_MODEL);
			EEquipmentType eequipmentType = DetermineType(RANGED_WEAPON_TYPES, p_specificationList);
			foreach (RangedWeaponStaticData rangedWeaponStaticData in iterator)
			{
				if (rangedWeaponStaticData.Type == eequipmentType && rangedWeaponStaticData.ModelLevel == p_modelLevel && rangedWeaponStaticData.SubModel == p_subModel)
				{
					Int32 p_suffixId = -1;
					Int32 p_prefixId = -1;
					if (p_prefixLevel > 0)
					{
						m_prefixProbabilities[EEquipmentType.RANGED_WEAPON].Multiplicators = m_currentEnchantmentMultiplicators;
						p_prefixId = m_prefixProbabilities[EEquipmentType.RANGED_WEAPON].GetRandomId();
					}
					if (p_suffixLevel > 0)
					{
						m_suffixProbabilities[EEquipmentType.RANGED_WEAPON].Multiplicators = m_currentEnchantmentMultiplicators;
						p_suffixId = m_suffixProbabilities[EEquipmentType.RANGED_WEAPON].GetRandomId();
					}
					RangedWeapon rangedWeapon = new RangedWeapon();
					rangedWeapon.InitFromModel(rangedWeaponStaticData, p_prefixId, p_suffixId);
					rangedWeapon.PrefixLevel = p_prefixLevel;
					rangedWeapon.SuffixLevel = p_suffixLevel;
					return rangedWeapon;
				}
			}
			return null;
		}

		private static Equipment CreateJewelry(Int32 p_modelLevel, ESubModel p_subModel, EEquipmentType[] p_specificationList, Int32 p_prefixLevel, Int32 p_suffixLevel)
		{
			IEnumerable<JewelryStaticData> iterator = StaticDataHandler.GetIterator<JewelryStaticData>(EDataType.JEWELRY_MODEL);
			EEquipmentType eequipmentType = DetermineType(JEWELRY_TYPES, p_specificationList);
			foreach (JewelryStaticData jewelryStaticData in iterator)
			{
				if (jewelryStaticData.Type == eequipmentType && jewelryStaticData.ModelLevel == p_modelLevel && jewelryStaticData.SubModel == p_subModel)
				{
					Int32 p_suffixId = -1;
					Int32 p_prefixId = -1;
					if (p_prefixLevel > 0)
					{
						m_prefixProbabilities[EEquipmentType.JEWELRY].Multiplicators = m_currentEnchantmentMultiplicators;
						p_prefixId = m_prefixProbabilities[EEquipmentType.JEWELRY].GetRandomId();
					}
					if (p_suffixLevel > 0)
					{
						m_suffixProbabilities[EEquipmentType.JEWELRY].Multiplicators = m_currentEnchantmentMultiplicators;
						p_suffixId = m_suffixProbabilities[EEquipmentType.JEWELRY].GetRandomId();
					}
					Jewelry jewelry = new Jewelry();
					jewelry.InitFromModel(jewelryStaticData, p_prefixId, p_suffixId);
					jewelry.PrefixLevel = p_prefixLevel;
					jewelry.SuffixLevel = p_suffixLevel;
					return jewelry;
				}
			}
			return null;
		}

		private static Equipment CreateShield(Int32 p_modelLevel, ESubModel p_subModel, EEquipmentType[] p_specificationList, Int32 p_prefixLevel, Int32 p_suffixLevel)
		{
			IEnumerable<ShieldStaticData> iterator = StaticDataHandler.GetIterator<ShieldStaticData>(EDataType.SHIELD_MODEL);
			EEquipmentType eequipmentType = DetermineType(SHIELD_TYPES, p_specificationList);
			foreach (ShieldStaticData shieldStaticData in iterator)
			{
				if (shieldStaticData.Type == eequipmentType && shieldStaticData.ModelLevel == p_modelLevel && shieldStaticData.SubModel == p_subModel)
				{
					Int32 p_suffixId = -1;
					Int32 p_prefixId = -1;
					if (p_prefixLevel > 0)
					{
						m_prefixProbabilities[EEquipmentType.SHIELD].Multiplicators = m_currentEnchantmentMultiplicators;
						p_prefixId = m_prefixProbabilities[EEquipmentType.SHIELD].GetRandomId();
					}
					if (p_suffixLevel > 0)
					{
						m_suffixProbabilities[EEquipmentType.SHIELD].Multiplicators = m_currentEnchantmentMultiplicators;
						p_suffixId = m_suffixProbabilities[EEquipmentType.SHIELD].GetRandomId();
					}
					Shield shield = new Shield();
					shield.InitFromModel(shieldStaticData, p_prefixId, p_suffixId);
					shield.PrefixLevel = p_prefixLevel;
					shield.SuffixLevel = p_suffixLevel;
					return shield;
				}
			}
			return null;
		}

		private static Equipment CreateMagicFocus(Int32 p_modelLevel, ESubModel p_subModel, EEquipmentType[] p_specificationList, Int32 p_prefixLevel, Int32 p_suffixLevel)
		{
			IEnumerable<MagicFocusStaticData> iterator = StaticDataHandler.GetIterator<MagicFocusStaticData>(EDataType.MAGIC_FOCUS_MODEL);
			EEquipmentType eequipmentType = DetermineType(MAGIC_FOCUS_TYPES, p_specificationList);
			foreach (MagicFocusStaticData magicFocusStaticData in iterator)
			{
				if (magicFocusStaticData.Type == eequipmentType && magicFocusStaticData.ModelLevel == p_modelLevel && magicFocusStaticData.SubModel == p_subModel)
				{
					Int32 p_suffixId = -1;
					Int32 p_prefixId = -1;
					if (p_prefixLevel > 0)
					{
						m_prefixProbabilities[EEquipmentType.MAGIC_FOCUS].Multiplicators = m_currentEnchantmentMultiplicators;
						p_prefixId = m_prefixProbabilities[EEquipmentType.MAGIC_FOCUS].GetRandomId();
					}
					if (p_suffixLevel > 0)
					{
						m_suffixProbabilities[EEquipmentType.MAGIC_FOCUS].Multiplicators = m_currentEnchantmentMultiplicators;
						p_suffixId = m_suffixProbabilities[EEquipmentType.MAGIC_FOCUS].GetRandomId();
					}
					MagicFocus magicFocus = new MagicFocus();
					magicFocus.InitFromModel(magicFocusStaticData, p_prefixId, p_suffixId);
					magicFocus.PrefixLevel = p_prefixLevel;
					magicFocus.SuffixLevel = p_suffixLevel;
					return magicFocus;
				}
			}
			return null;
		}

		private static EEquipmentType DetermineType(EEquipmentType[] p_types, EEquipmentType[] p_specificationList)
		{
			if (m_itemTypeTempFilter == null)
			{
				m_itemTypeTempFilter = new List<EEquipmentType>();
			}
			Single num = 0f;
			Single num2 = 0f;
			m_itemTypeTempFilter.Clear();
			foreach (EEquipmentType eequipmentType in p_types)
			{
				foreach (EEquipmentType eequipmentType2 in p_specificationList)
				{
					if (eequipmentType2 == eequipmentType)
					{
						m_itemTypeTempFilter.Add(eequipmentType);
					}
				}
			}
			if (m_itemTypeTempFilter.Count == 0)
			{
				foreach (EEquipmentType item in p_types)
				{
					m_itemTypeTempFilter.Add(item);
				}
			}
			foreach (EEquipmentType key in m_itemTypeTempFilter)
			{
				num2 += m_itemProbabilities[key];
			}
			Single num3 = Random.Range(0f, num2);
			foreach (EEquipmentType eequipmentType3 in m_itemTypeTempFilter)
			{
				num += m_itemProbabilities[eequipmentType3];
				if (num3 < num)
				{
					return eequipmentType3;
				}
			}
			return EEquipmentType.NONE;
		}
	}
}
