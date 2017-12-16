using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;
using Legacy.Game.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Cheats
{
	[AddComponentMenu("MM Legacy/Cheats/CheatsItems")]
	public class CheatsItems : MonoBehaviour
	{
		private const String DATA_ARMOR = "Armor";

		private const String DATA_JEWELRY = "Jewelry";

		private const String DATA_SHIELDS = "Shields";

		private const String DATA_MELEE_WEAPON = "Melee Weapons";

		private const String DATA_MAGIC_FOCUS = "Magic Focus";

		private const String DATA_RANGED_WEAPONS = "Ranged Weapons";

		private const String DATA_POTIONS = "Potions";

		private const String DATA_SCROLLS = "Scrolls";

		[SerializeField]
		private UIPopupList m_categoryList;

		[SerializeField]
		private UIPopupList m_itemList;

		[SerializeField]
		private UIPopupList m_prefixList;

		[SerializeField]
		private SelectionList m_suffixList;

		[SerializeField]
		private UICheckbox m_checkboxBroken;

		[SerializeField]
		private UICheckbox m_checkboxUnidentified;

		private List<Int32> m_staticIdList;

		private EDataType m_currentDataType;

		private Boolean m_isEquipment;

		private void OnEnable()
		{
			if (m_categoryList != null)
			{
				m_categoryList.items.Clear();
				m_staticIdList = new List<Int32>();
				m_categoryList.items.Add("Armor");
				m_categoryList.items.Add("Jewelry");
				m_categoryList.items.Add("Shields");
				m_categoryList.items.Add("Melee Weapons");
				m_categoryList.items.Add("Magic Focus");
				m_categoryList.items.Add("Ranged Weapons");
				m_categoryList.items.Add("Potions");
				m_categoryList.items.Add("Scrolls");
			}
			m_prefixList.items.Clear();
			m_suffixList.Clear();
			IEnumerable<PrefixStaticData> iterator = StaticDataHandler.GetIterator<PrefixStaticData>(EDataType.PREFIX);
			IEnumerable<SuffixStaticData> iterator2 = StaticDataHandler.GetIterator<SuffixStaticData>(EDataType.SUFFIX);
			m_prefixList.items.Add("0: NONE");
			foreach (PrefixStaticData prefixStaticData in iterator)
			{
				m_prefixList.items.Add(prefixStaticData.StaticID + ": " + prefixStaticData.Name);
			}
			m_suffixList.AddItem("0: NONE");
			foreach (SuffixStaticData suffixStaticData in iterator2)
			{
				m_suffixList.AddItemWithoutReposition(suffixStaticData.StaticID + ": " + suffixStaticData.Name);
			}
			m_suffixList.ReposItems();
			SelectItemCategory("Armor");
		}

		private void SelectItemCategory(String p_category)
		{
			m_categoryList.selection = p_category;
			if (m_itemList != null)
			{
				m_itemList.items.Clear();
				m_staticIdList.Clear();
				IEnumerable enumerable = null;
				switch (p_category)
				{
				case "Armor":
					enumerable = StaticDataHandler.GetIterator<ArmorStaticData>(EDataType.ARMOR);
					m_currentDataType = EDataType.ARMOR;
					m_isEquipment = true;
					break;
				case "Jewelry":
					enumerable = StaticDataHandler.GetIterator<JewelryStaticData>(EDataType.JEWELRY);
					m_currentDataType = EDataType.JEWELRY;
					m_isEquipment = true;
					break;
				case "Shields":
					enumerable = StaticDataHandler.GetIterator<ShieldStaticData>(EDataType.SHIELD);
					m_currentDataType = EDataType.SHIELD;
					m_isEquipment = true;
					break;
				case "Melee Weapons":
					enumerable = StaticDataHandler.GetIterator<MeleeWeaponStaticData>(EDataType.MELEE_WEAPON);
					m_currentDataType = EDataType.MELEE_WEAPON;
					m_isEquipment = true;
					break;
				case "Magic Focus":
					enumerable = StaticDataHandler.GetIterator<MagicFocusStaticData>(EDataType.MAGIC_FOCUS);
					m_currentDataType = EDataType.MAGIC_FOCUS;
					m_isEquipment = true;
					break;
				case "Ranged Weapons":
					enumerable = StaticDataHandler.GetIterator<RangedWeaponStaticData>(EDataType.RANGED_WEAPON);
					m_currentDataType = EDataType.RANGED_WEAPON;
					m_isEquipment = true;
					break;
				case "Potions":
					enumerable = StaticDataHandler.GetIterator<PotionStaticData>(EDataType.POTION);
					m_currentDataType = EDataType.POTION;
					m_isEquipment = false;
					break;
				case "Scrolls":
					enumerable = StaticDataHandler.GetIterator<ScrollStaticData>(EDataType.SCROLL);
					m_currentDataType = EDataType.SCROLL;
					m_isEquipment = false;
					break;
				}
				IEnumerator enumerator = enumerable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Object obj = enumerator.Current;
						BaseItemStaticData baseItemStaticData = (BaseItemStaticData)obj;
						m_itemList.items.Add(baseItemStaticData.NameKey);
						m_staticIdList.Add(baseItemStaticData.StaticID);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				if (m_itemList.items.Count > 0)
				{
					m_itemList.selection = m_itemList.items[0];
				}
				else
				{
					m_itemList.selection = String.Empty;
				}
				NGUITools.SetActive(m_prefixList.gameObject, m_isEquipment);
				NGUITools.SetActive(m_suffixList.gameObject, m_isEquipment);
			}
		}

		public void OnAddToInventoryButtonClick()
		{
			BaseItem baseItem = ItemFactory.CreateItem(m_currentDataType);
			Int32 index = m_itemList.items.IndexOf(m_itemList.selection);
			baseItem.Init(m_staticIdList[index]);
			if (m_isEquipment)
			{
				Equipment equipment = baseItem as Equipment;
				if (equipment != null)
				{
					Int32 num = Int32.Parse(m_prefixList.selection.Split(new Char[]
					{
						':'
					})[0]);
					if (num != 0)
					{
						PrefixStaticData staticData = StaticDataHandler.GetStaticData<PrefixStaticData>(EDataType.PREFIX, num);
						equipment.Prefixes.Add(staticData);
					}
					Int32 num2 = Int32.Parse(m_suffixList.SelectedItem.Split(new Char[]
					{
						':'
					})[0]);
					if (num2 != 0)
					{
						SuffixStaticData staticData2 = StaticDataHandler.GetStaticData<SuffixStaticData>(EDataType.SUFFIX, num2);
						equipment.Suffixes.Add(staticData2);
					}
				}
				equipment.Broken = m_checkboxBroken.isChecked;
				equipment.Identified = !m_checkboxUnidentified.isChecked;
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.Inventory.AddItem(baseItem);
		}
	}
}
