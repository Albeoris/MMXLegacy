using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Dumper.Core;
using Legacy.Core.Abilities;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.Quests;
using Legacy.Utilities;

namespace Legacy.Core.StaticData
{
	public static class StaticDataHandler
	{
		private static readonly Dictionary<Int32, BaseStaticData>[] m_staticDataMap = new Dictionary<Int32, BaseStaticData>[71];

	    public static void LoadData<T>(EDataType p_type, String p_filePath) where T : BaseStaticData
	    {
	        T[] array;
	        if (File.Exists(p_filePath))
	        {
	            using (FileStream fileStream = File.OpenRead(p_filePath))
	                array = CSVParser<T>.Deserialize(fileStream);
	        }
            else
	        {
	            array = new T[0];
	        }

	        Dictionary<Int32, T> dic = new Dictionary<Int32, T>(array.Length);
	        foreach (T item in array)
	        {
	            if (!dic.ContainsKey(item.StaticID))
	            {
	                item.PostDeserialization();
	                dic.Add(item.StaticID, item);
	            }
	            else
	            {
	                LegacyLogger.Log(String.Concat(new Object[]
	                {
	                    "StaticData already loaded; SID=",
	                    item.StaticID,
	                    "; Type=",
	                    p_type
	                }));
	            }
	        }

	        String directory = Path.GetDirectoryName(p_filePath);
	        if (directory != null)
	        {
	            String mask = Path.ChangeExtension(Path.GetFileName(p_filePath), null) + "_*.csv";
	            foreach (String filePath in Directory.GetFiles(directory, mask, SearchOption.AllDirectories))
	            {
	                using (FileStream fileStream = File.OpenRead(filePath))
	                    CSVParser<T>.Deserialize(fileStream, dic);
	            }
	        }

	        Dictionary<Int32, BaseStaticData> result = new Dictionary<Int32, BaseStaticData>(dic.Count);
            foreach (KeyValuePair<Int32, T> pair in dic)
            {
                Int32 key = pair.Key;
                T value = pair.Value;
                value.PostDeserialization();
                result.Add(key, value);
            }

	        m_staticDataMap[(Int32)p_type] = result;
	    }

	    public static T GetStaticData<T>(EDataType p_type, Int32 p_staticId) where T : BaseStaticData
		{
			Dictionary<Int32, BaseStaticData> dictionary = m_staticDataMap[(Int32)p_type];
			BaseStaticData baseStaticData = null;
			if (dictionary != null && !dictionary.TryGetValue(p_staticId, out baseStaticData))
			{
				LegacyLogger.Log(String.Concat(new Object[]
				{
					"StaticData not found! Type=",
					p_type,
					"; ID=",
					p_staticId
				}));
			}
			return baseStaticData as T;
		}

		public static IEnumerable<T> GetIterator<T>(EDataType p_type) where T : BaseStaticData
		{
			Dictionary<Int32, BaseStaticData> map = m_staticDataMap[(Int32)p_type];
			if (map != null)
			{
				foreach (BaseStaticData item in map.Values)
				{
					T value = item as T;
					if (value != null)
					{
						yield return value;
					}
				}
			}
			yield break;
		}

		public static Int32 GetCount(EDataType p_type)
		{
			Dictionary<Int32, BaseStaticData> dictionary = m_staticDataMap[(Int32)p_type];
			return (dictionary == null) ? 0 : dictionary.Count;
		}

		public static void Clear()
		{
			for (Int32 i = 0; i < m_staticDataMap.Length; i++)
			{
				if (m_staticDataMap[i] != null)
				{
					m_staticDataMap[i].Clear();
					m_staticDataMap[i] = null;
				}
			}
		}

		private static class CSVParser<T>
		{
			private static CsvSerializer s_Serializer;

			private static Char[] s_Seperator = new Char[]
			{
				','
			};

			static CSVParser()
			{
				s_Serializer = new CsvSerializer(typeof(T[]));
				s_Serializer.ColumnValueResolve += ResolveColumnValue;
			}

			public static T[] Deserialize(Stream stream)
			{
				return (T[])s_Serializer.Deserialize(stream);
			}

		    public static void Deserialize(Stream stream, Dictionary<Int32, T> output)
		    {
		        s_Serializer.Deserialize(stream, output);
		    }

            private static void ResolveColumnValue(Object sender, ColumnValueResolveEventArg e)
			{
				if (e.Type == typeof(Resistance[]))
				{
					String[] array = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					List<Resistance> list = new List<Resistance>(array.Length / 2);
					for (Int32 i = 1; i < array.Length; i += 2)
					{
						Resistance item;
						item.Type = (EDamageType)Enum.Parse(typeof(EDamageType), array[i - 1], true);
						item.Value = Int32.Parse(array[i], CultureInfo.InvariantCulture);
						list.Add(item);
					}
					e.Output = list.ToArray();
				}
				else if (e.Type == typeof(DamageData[]))
				{
					String[] array2 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					List<DamageData> list2 = new List<DamageData>(array2.Length / 3);
					for (Int32 j = 2; j < array2.Length; j += 3)
					{
						EDamageType p_type = (EDamageType)Enum.Parse(typeof(EDamageType), array2[j - 2], true);
						Int32 p_minimum = Int32.Parse(array2[j - 1]);
						Int32 p_maximum = Int32.Parse(array2[j]);
						DamageData item2 = new DamageData(p_type, p_minimum, p_maximum);
						list2.Add(item2);
					}
					e.Output = list2.ToArray();
				}
				else if (e.Type == typeof(NpcEffect[]))
				{
					String[] array3 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					List<NpcEffect> list3 = new List<NpcEffect>(array3.Length / 4);
					for (Int32 k = 3; k < array3.Length; k += 4)
					{
						ETargetCondition p_targetEffect = (ETargetCondition)Enum.Parse(typeof(ETargetCondition), array3[k - 3], true);
						EEffectPeriodicity p_effectType = (EEffectPeriodicity)Enum.Parse(typeof(EEffectPeriodicity), array3[k - 2], true);
						Single p_effectValue = Single.Parse(array3[k - 1]);
						Int32 p_effectPrice = Int32.Parse(array3[k]);
						NpcEffect item3 = new NpcEffect(p_targetEffect, p_effectType, p_effectValue, p_effectPrice);
						list3.Add(item3);
					}
					e.Output = list3.ToArray();
				}
				else if (e.Type == typeof(IntRange))
				{
					String[] array4 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					IntRange zero = IntRange.Zero;
					if (array4.Length == 1)
					{
						zero.Min = (zero.Max = Int32.Parse(array4[0]));
					}
					else if (array4.Length > 1)
					{
						zero.Min = Int32.Parse(array4[0]);
						zero.Max = Int32.Parse(array4[1]);
					}
					e.Output = zero;
				}
				else if (e.Type == typeof(FloatRange))
				{
					String[] array5 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					FloatRange zero2 = FloatRange.Zero;
					if (array5.Length == 1)
					{
						zero2.Min = (zero2.Max = Single.Parse(array5[0]));
					}
					else if (array5.Length > 1)
					{
						zero2.Min = Single.Parse(array5[0]);
						zero2.Max = Single.Parse(array5[1]);
					}
					e.Output = zero2;
				}
				else if (e.Type == typeof(MonsterStaticData.SpellData[]))
				{
					String[] array6 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					List<MonsterStaticData.SpellData> list4 = new List<MonsterStaticData.SpellData>(array6.Length / 4);
					for (Int32 l = 2; l < array6.Length; l += 4)
					{
						String p_animationClipName = array6[l - 2];
						Int32 p_spellID = Int32.Parse(array6[l - 1]);
						Int32 p_spellProbability = Int32.Parse(array6[l]);
						Int32 p_level = Int32.Parse(array6[l + 1]);
						list4.Add(new MonsterStaticData.SpellData(p_animationClipName, p_spellID, p_spellProbability, p_level));
					}
					e.Output = list4.ToArray();
				}
				else if (e.Type == typeof(MonsterStaticData.ExtraDamage))
				{
					String[] array7 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					if (array7.Length > 0)
					{
						MonsterStaticData.ExtraDamage extraDamage = new MonsterStaticData.ExtraDamage((EDamageType)Enum.Parse(typeof(EDamageType), array7[0], true), Int32.Parse(array7[1]));
						e.Output = extraDamage;
					}
				}
				else if (e.Type == typeof(ItemOffer[]))
				{
					String[] array8 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					ItemOffer[] array9 = new ItemOffer[array8.Length / 3];
					for (Int32 m = 0; m < array9.Length; m++)
					{
						EDataType p_itemType = (EDataType)Enum.Parse(typeof(EDataType), array8[m * 3], true);
						Int32 p_itemID = Int32.Parse(array8[m * 3 + 1]);
						Int32 p_itemQuantity = Int32.Parse(array8[m * 3 + 2]);
						array9[m] = new ItemOffer(p_itemType, p_itemID, p_itemQuantity);
					}
					e.Output = array9;
				}
				else if (e.Type == typeof(Position))
				{
					String[] array10 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					Position position = default(Position);
					if (array10.Length == 2)
					{
						position.X = Int32.Parse(array10[0]);
						position.Y = Int32.Parse(array10[1]);
					}
					e.Output = position;
				}
				else if (e.Type == typeof(EquipmentData[]))
				{
					String[] array11 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					EquipmentData[] array12 = new EquipmentData[array11.Length / 2];
					for (Int32 n = 0; n < array12.Length; n++)
					{
						EDataType p_type2 = (EDataType)Enum.Parse(typeof(EDataType), array11[n * 2], true);
						Int32 p_staticId = Int32.Parse(array11[n * 2 + 1]);
						array12[n] = new EquipmentData(p_type2, p_staticId);
					}
					e.Output = array12;
				}
				else if (e.Type == typeof(SteadyLoot[]))
				{
					String[] array13 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					SteadyLoot[] array14 = new SteadyLoot[array13.Length / 5];
					for (Int32 num = 0; num < array14.Length; num++)
					{
						EDataType p_itemClass = (EDataType)Enum.Parse(typeof(EDataType), array13[num * 5], true);
						Int32 p_itemID2 = Int32.Parse(array13[num * 5 + 1]);
						Int32 p_min = Int32.Parse(array13[num * 5 + 2]);
						Int32 p_max = Int32.Parse(array13[num * 5 + 3]);
						Single p_dropChance = Single.Parse(array13[num * 5 + 4]);
						array14[num] = new SteadyLoot(p_itemClass, p_itemID2, p_min, p_max, p_dropChance);
					}
					e.Output = array14;
				}
				else if (e.Type == typeof(ModelProbability[]))
				{
					String[] array15 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					ModelProbability[] array16 = new ModelProbability[array15.Length / 2];
					for (Int32 num2 = 0; num2 < array16.Length; num2++)
					{
						String text = array15[num2 * 2];
						Int32 p_modelLevel = Int32.Parse(text.Substring(0, 1));
						ESubModel p_subModel = (ESubModel)Enum.Parse(typeof(ESubModel), text.Substring(1, 1), true);
						Single p_weight = Single.Parse(array15[num2 * 2 + 1]);
						array16[num2] = new ModelProbability(p_modelLevel, p_subModel, p_weight);
					}
					e.Output = array16;
				}
				else if (e.Type == typeof(EnchantmentProbability[]))
				{
					String[] array17 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					EnchantmentProbability[] array18 = new EnchantmentProbability[array17.Length / 2];
					for (Int32 num3 = 0; num3 < array18.Length; num3++)
					{
						Int32 p_modelLevel2 = Int32.Parse(array17[num3 * 2]);
						Single p_weight2 = Single.Parse(array17[num3 * 2 + 1]);
						array18[num3] = new EnchantmentProbability(p_modelLevel2, p_weight2);
					}
					e.Output = array18;
				}
				else if (e.Type == typeof(StepsOnTerrainData))
				{
					StepsOnTerrainData empty = StepsOnTerrainData.Empty;
					String[] array19 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					if (array19.Length == 2)
					{
						Int32 p_numberOfSteps = Int32.Parse(array19[0]);
						ETerrainType p_type3 = (ETerrainType)Enum.Parse(typeof(ETerrainType), array19[1], true);
						empty = new StepsOnTerrainData(p_numberOfSteps, p_type3);
					}
					e.Output = empty;
				}
				else if (e.Type == typeof(MonsterAbilityID[]))
				{
					String[] array20 = e.Value.Split(s_Seperator, StringSplitOptions.RemoveEmptyEntries);
					MonsterAbilityID[] array21 = new MonsterAbilityID[array20.Length / 2];
					for (Int32 num4 = 0; num4 < array21.Length; num4++)
					{
						MonsterAbilityID monsterAbilityID;
						monsterAbilityID.AbilityType = (EMonsterAbilityType)Enum.Parse(typeof(EMonsterAbilityType), array20[num4 * 2]);
						monsterAbilityID.Level = Int32.Parse(array20[num4 * 2 + 1]);
						array21[num4] = monsterAbilityID;
					}
					e.Output = array21;
				}
			}
		}
	}
}
