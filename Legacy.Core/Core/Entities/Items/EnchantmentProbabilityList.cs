using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.Entities.Items
{
	public class EnchantmentProbabilityList
	{
		private static Dictionary<EDamageType, Single> s_defaultMultiplicators;

		private List<Enchantment> m_probabilities;

		private Dictionary<EDamageType, Single> m_multiplicators = DefaultMultiplicators;

		public EnchantmentProbabilityList(IEnumerable<PrefixStaticData> p_prefixes, EEquipmentType p_type, EEquipmentType p_subtype)
		{
			m_probabilities = new List<Enchantment>();
			foreach (PrefixStaticData prefixStaticData in p_prefixes)
			{
				Single prefixProbability = GetPrefixProbability(prefixStaticData, p_type, p_subtype);
				if (prefixProbability > 0f)
				{
					Enchantment item = new Enchantment(prefixStaticData.StaticID, prefixProbability, prefixStaticData.School);
					m_probabilities.Add(item);
				}
			}
		}

		public EnchantmentProbabilityList(IEnumerable<SuffixStaticData> p_suffixes, EEquipmentType p_type, EEquipmentType p_subtype)
		{
			m_probabilities = new List<Enchantment>();
			foreach (SuffixStaticData suffixStaticData in p_suffixes)
			{
				Single suffixProbability = GetSuffixProbability(suffixStaticData, p_type, p_subtype);
				if (suffixProbability > 0f)
				{
					Enchantment item = new Enchantment(suffixStaticData.StaticID, suffixProbability, suffixStaticData.MagicSchool);
					m_probabilities.Add(item);
				}
			}
		}

		public static Dictionary<EDamageType, Single> DefaultMultiplicators
		{
			get
			{
				if (s_defaultMultiplicators == null)
				{
					s_defaultMultiplicators = new Dictionary<EDamageType, Single>();
					s_defaultMultiplicators[EDamageType.AIR] = 1f;
					s_defaultMultiplicators[EDamageType.EARTH] = 1f;
					s_defaultMultiplicators[EDamageType.FIRE] = 1f;
					s_defaultMultiplicators[EDamageType.WATER] = 1f;
					s_defaultMultiplicators[EDamageType.LIGHT] = 1f;
					s_defaultMultiplicators[EDamageType.DARK] = 1f;
					s_defaultMultiplicators[EDamageType.PRIMORDIAL] = 1f;
					s_defaultMultiplicators[EDamageType.NONE] = 1f;
				}
				return s_defaultMultiplicators;
			}
		}

		public Dictionary<EDamageType, Single> Multiplicators
		{
			get => m_multiplicators;
		    set => m_multiplicators = value;
		}

		public Int32 GetRandomId()
		{
			Single num = 0f;
			foreach (Enchantment enchantment in m_probabilities)
			{
				num += enchantment.Probability * m_multiplicators[enchantment.School];
			}
			if (num == 0f)
			{
				foreach (Enchantment enchantment2 in m_probabilities)
				{
					num += enchantment2.Probability;
				}
			}
			Single num2 = Random.Range(0f, num);
			Single num3 = 0f;
			foreach (Enchantment enchantment3 in m_probabilities)
			{
				num3 += enchantment3.Probability * m_multiplicators[enchantment3.School];
				if (num2 < num3)
				{
					return enchantment3.ID;
				}
			}
			return 0;
		}

		private Single GetPrefixProbability(PrefixStaticData p_prefix, EEquipmentType p_class, EEquipmentType p_subtype)
		{
			switch (p_class)
			{
			case EEquipmentType.ARMOR:
				switch (p_subtype)
				{
				case EEquipmentType.CLOTHING:
					return p_prefix.ProbabilityCloth;
				case EEquipmentType.LIGHT_ARMOR:
					return p_prefix.ProbabilityLightArmor;
				case EEquipmentType.HEAVY_ARMOR:
					return p_prefix.ProbabilityHeavyArmor;
				case EEquipmentType.ARCANE:
					return p_prefix.ProbabilityArcane;
				case EEquipmentType.MARTIAL:
					return p_prefix.ProbabilityMartial;
				}
				break;
			case EEquipmentType.JEWELRY:
				return p_prefix.ProbabilityJewelry;
			case EEquipmentType.SHIELD:
				return p_prefix.ProbabilityShields;
			case EEquipmentType.MELEE_WEAPON:
				return p_prefix.ProbabilityMelee;
			case EEquipmentType.MAGIC_FOCUS:
				return p_prefix.ProbabilityMagicFocus;
			case EEquipmentType.RANGED_WEAPON:
				return p_prefix.ProbabilityRanged;
			}
			return 0f;
		}

		private Single GetSuffixProbability(SuffixStaticData p_suffix, EEquipmentType p_class, EEquipmentType p_subtype)
		{
			switch (p_class)
			{
			case EEquipmentType.ARMOR:
				switch (p_subtype)
				{
				case EEquipmentType.CLOTHING:
					return p_suffix.ProbabilityCloth;
				case EEquipmentType.LIGHT_ARMOR:
					return p_suffix.ProbabilityLightArmor;
				case EEquipmentType.HEAVY_ARMOR:
					return p_suffix.ProbabilityHeavyArmor;
				case EEquipmentType.ARCANE:
					return p_suffix.ProbabilityArcane;
				case EEquipmentType.MARTIAL:
					return p_suffix.ProbabilityMartial;
				}
				break;
			case EEquipmentType.JEWELRY:
				return p_suffix.ProbabilityJewelry;
			case EEquipmentType.SHIELD:
				return p_suffix.ProbabilityShields;
			case EEquipmentType.MELEE_WEAPON:
				return p_suffix.ProbabilityMelee;
			case EEquipmentType.MAGIC_FOCUS:
				return p_suffix.ProbabilityMagicFocus;
			case EEquipmentType.RANGED_WEAPON:
				return p_suffix.ProbabilityRanged;
			}
			return 0f;
		}

		private struct Enchantment
		{
			public readonly Int32 ID;

			public readonly Single Probability;

			public readonly EDamageType School;

			public Enchantment(Int32 p_id, Single p_probability, EDamageType p_school)
			{
				ID = p_id;
				Probability = p_probability;
				School = p_school;
			}
		}
	}
}
