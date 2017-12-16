using System;
using System.Collections.Generic;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/ItemTooltip")]
	public class ItemTooltip : MonoBehaviour
	{
		private const Int32 ENCHANTED = 1;

		private const Int32 DOUBLE_ENCHANTED = 2;

		[SerializeField]
		private TooltipBackground m_background;

		[SerializeField]
		private TooltipGroup m_equipped;

		[SerializeField]
		private TooltipGroup m_name;

		[SerializeField]
		private TooltipGroup m_description;

		[SerializeField]
		private TooltipGroup m_details;

		[SerializeField]
		private TooltipGroup m_requirements;

		[SerializeField]
		private TooltipGroup m_broken;

		[SerializeField]
		private TooltipGroup m_price;

		[SerializeField]
		private TooltipGroup m_action;

		[SerializeField]
		private TooltipGroup m_relicLevelBar;

		[SerializeField]
		private TooltipGroup m_relicDescription;

		[SerializeField]
		private TooltipGroup m_compareCaption;

		[SerializeField]
		private TooltipGroup m_compare;

		[SerializeField]
		private Color m_actionColor = Color.blue;

		[SerializeField]
		private Color m_highlightColor = Color.yellow;

		[SerializeField]
		private Color m_effectColor = Color.green;

		[SerializeField]
		private Color m_brokenColor = Color.red;

		[SerializeField]
		private TooltipItemSlot m_itemSlot;

		[SerializeField]
		private Single m_outerPadding = 5f;

		[SerializeField]
		private Single m_innerPadding = 3f;

		[SerializeField]
		private Boolean m_isCompare;

		[SerializeField]
		private UISprite m_relicBarBackground;

		[SerializeField]
		private UISprite m_relicBarForeground;

		[SerializeField]
		private UISprite m_relicBarShadow;

		[SerializeField]
		private UILabel m_relicXPAmountLabel;

		private static Color m_nameColorPlain = Color.white;

		private static Color m_nameColorEnchanted = Color.green;

		private static Color m_nameColorDoubleEnchanted = new Color(0f, 0.75f, 1f);

		private static Color m_nameColorRelic = new Color(1f, 0.75f, 0f);

		private static Color m_nameColorNotIdentified = Color.red;

		private String m_nameColorPlainHex;

		private String m_nameColorEnchantedHex;

		private String m_nameColorDoubleEnchantedHex;

		private String m_nameColorRelicHex;

		private String m_nameColorNotIdentifiedHex;

		private String m_actionColorHex;

		private String m_highlightColorHex;

		private String m_effectColorHex;

		private String m_brokenColorHex;

		private String m_itemDetails = String.Empty;

		private String m_skillRequirements = String.Empty;

		private String m_itemDescription = String.Empty;

		private String m_itemName = String.Empty;

		private String m_compareText = String.Empty;

		private Vector3 m_scaleWithComparison;

		private String m_relicDescriptionText = String.Empty;

		private Boolean m_initialized;

		public TooltipGroup EquippedGroup => m_equipped;

	    public Vector3 Scale => m_scaleWithComparison;

	    private void Init()
		{
			if (!m_initialized)
			{
				m_highlightColorHex = "[" + NGUITools.EncodeColor(m_highlightColor) + "]";
				m_effectColorHex = "[" + NGUITools.EncodeColor(m_effectColor) + "]";
				m_brokenColorHex = "[" + NGUITools.EncodeColor(m_brokenColor) + "]";
				m_nameColorPlainHex = "[" + NGUITools.EncodeColor(m_nameColorPlain) + "]";
				m_nameColorEnchantedHex = "[" + NGUITools.EncodeColor(m_nameColorEnchanted) + "]";
				m_nameColorDoubleEnchantedHex = "[" + NGUITools.EncodeColor(m_nameColorDoubleEnchanted) + "]";
				m_nameColorRelicHex = "[" + NGUITools.EncodeColor(m_nameColorRelic) + "]";
				m_actionColorHex = "[" + NGUITools.EncodeColor(m_actionColor) + "]";
				m_nameColorNotIdentifiedHex = "[" + NGUITools.EncodeColor(m_nameColorNotIdentified) + "]";
				m_initialized = true;
			}
		}

		public void Hide()
		{
			NGUITools.SetActiveSelf(gameObject, false);
		}

		public void Fill(BaseItem p_item, Equipment p_compare, Equipment p_compareSecondary, IInventory p_inventory)
		{
			Init();
			Boolean flag = !(p_item is Equipment) || ((Equipment)p_item).Identified;
			Boolean flag2 = p_item is Equipment && ((Equipment)p_item).Broken;
			m_itemDescription = String.Empty;
			m_itemDetails = String.Empty;
			m_skillRequirements = String.Empty;
			m_compareText = String.Empty;
			m_relicDescriptionText = String.Empty;
			if (p_item is Equipment)
			{
				((Equipment)p_item).ModifyProperties();
			}
			FillEnchantmentsDescription(p_item);
			FillRelicLevelBar(p_item);
			FillRelicDescription(p_item);
			FillPropertyDescription(p_item);
			FillSkillRequirementDescription(p_item);
			FillItemName(p_item);
			if (p_compare != null)
			{
				FillComparison((Equipment)p_item, p_compare, p_compareSecondary);
			}
			Single num = m_outerPadding + m_innerPadding;
			if (m_isCompare)
			{
				m_equipped.UpdateText(LocaManager.GetText("TOOLTIP_ITEM_CURRENTLY_EQUIPPED"));
				m_equipped.UpdatePositionY(-num);
				num += m_equipped.Size.y + m_innerPadding;
			}
			m_name.UpdateText(m_itemName);
			m_name.UpdatePositionY(-num);
			num += m_name.Size.y + m_innerPadding;
			m_itemSlot.SetItem(p_item);
			m_itemSlot.UpdatePosition(new Vector3(-m_name.Size.x / 2f, -num, 0f));
			m_description.MinHeight = m_itemSlot.Size.y;
			m_description.VerticalAlign = TooltipGroup.Align.CENTER;
			m_description.UpdateText(m_itemDescription);
			m_description.UpdatePositionY(-num);
			num += m_description.Size.y + m_innerPadding;
			if (m_description.Size.y > m_itemSlot.Size.y)
			{
				m_itemSlot.SetHeight(m_description.Size.y);
			}
			else
			{
				m_itemSlot.ResetHeight();
			}
			if (flag)
			{
				m_details.SetVisible(m_itemDetails.Length > 0);
				if (m_itemDetails.Length > 0)
				{
					m_details.UpdateText(m_itemDetails);
					m_details.UpdatePositionY(-num);
					num += m_details.Size.y + m_innerPadding;
				}
			}
			else
			{
				m_details.SetVisible(true);
				m_details.UpdateText(LocaManager.GetText("EQUIPMENT_UNIDENTIFIED_DESCRIPTION"));
				m_details.UpdatePositionY(-num);
				num += m_details.Size.y + m_innerPadding;
			}
			if (m_relicLevelBar.IsVisible)
			{
				m_relicLevelBar.UpdatePositionY(-num);
				num += m_relicLevelBar.Size.y + m_innerPadding;
			}
			m_relicDescription.SetVisible(m_relicDescriptionText.Length > 0);
			if (m_relicDescription.IsVisible)
			{
				m_relicDescription.UpdateText(m_relicDescriptionText);
				m_relicDescription.UpdatePositionY(-num);
				num += m_relicDescription.Size.y + m_innerPadding;
			}
			m_requirements.SetVisible(flag && m_skillRequirements.Length > 0);
			if (flag && m_skillRequirements.Length > 0)
			{
				m_requirements.UpdateText(m_skillRequirements);
				m_requirements.UpdatePositionY(-num);
				num += m_requirements.Size.y + m_innerPadding;
			}
			m_broken.SetVisible(flag2);
			if (flag2)
			{
				String text = m_brokenColorHex + LocaManager.GetText("ITEM_IS_BROKEN") + "[-]";
				text += GetBrokenItemAdditionalText((Equipment)p_item);
				m_broken.UpdateText(text);
				m_broken.UpdatePositionY(-num);
				num += m_broken.Size.y + m_innerPadding;
			}
			Boolean flag3 = !(p_inventory is TradingInventoryController) && !(p_inventory is PartyInventoryController) && !(p_inventory is CharacterInventoryController);
			m_price.SetVisible(true);
			m_price.VerticalAlign = TooltipGroup.Align.CENTER;
			m_price.MinHeight = 54f;
			m_price.Label.color = m_highlightColor;
			if ((p_inventory is TradingInventoryController || flag3) && !m_isCompare)
			{
				m_price.UpdateText(LocaManager.GetText("TOOLTIP_ITEM_PRICE_BUY", m_nameColorPlainHex + p_item.Price + "[-]"));
			}
			else if (flag2)
			{
				m_price.UpdateText(LocaManager.GetText("TOOLTIP_ITEM_PRICE_SELL", m_brokenColorHex + p_item.Price + "[-]"));
			}
			else
			{
				m_price.UpdateText(LocaManager.GetText("TOOLTIP_ITEM_PRICE_SELL", m_nameColorPlainHex + p_item.Price + "[-]"));
			}
			if (p_item is GoldStack)
			{
				m_price.SetVisible(false);
			}
			else
			{
				m_price.UpdatePositionY(-num);
				num += m_price.Size.y + m_innerPadding;
			}
			Boolean flag4 = p_inventory != null && !m_isCompare;
			m_action.SetVisible(flag4);
			if (flag4)
			{
				if (p_inventory is TradingInventoryController)
				{
					m_action.UpdateText(LocaManager.GetText("TOOLTIP_ACTION_BUY", m_actionColorHex));
				}
				else if (p_inventory is CharacterInventoryController)
				{
					m_action.UpdateText(LocaManager.GetText("TOOLTIP_ACTION_UNEQUIP", m_actionColorHex));
				}
				else if (p_inventory is PartyInventoryController)
				{
					ConversationManager conversationManager = LegacyLogic.Instance.ConversationManager;
					if (conversationManager.CurrentNpc != null && conversationManager.CurrentNpc.TradingInventory.IsTrading)
					{
						m_action.UpdateText(LocaManager.GetText("TOOLTIP_ACTION_SELL", m_actionColorHex));
					}
					else if (p_item is Consumable)
					{
						m_action.UpdateText(LocaManager.GetText("TOOLTIP_ACTION_USE", m_actionColorHex));
					}
					else if (p_item is Equipment)
					{
						Equipment equipment = (Equipment)p_item;
						Party party = LegacyLogic.Instance.WorldManager.Party;
						EEquipSlots autoSlot = party.SelectedCharacter.Equipment.GetAutoSlot(equipment);
						Boolean flag5 = party.SelectedCharacter.Equipment.IsItemPlaceableAt(p_item, (Int32)autoSlot);
						if (flag5 && equipment.Identified)
						{
							m_action.UpdateText(LocaManager.GetText("TOOLTIP_ACTION_EQUIP", m_actionColorHex));
						}
						else
						{
							m_action.SetVisible(false);
						}
					}
				}
				else
				{
					m_action.UpdateText(LocaManager.GetText("TOOLTIP_ACTION_LOOT", m_actionColorHex));
				}
			}
			if (m_action.IsVisible)
			{
				m_action.UpdatePositionY(-num);
				num += m_action.Size.y + m_innerPadding;
			}
			m_background.Scale(m_name.Size.x + m_outerPadding * 2f, num - m_innerPadding + m_outerPadding);
			Boolean flag6 = p_compare != null;
			m_compareCaption.SetVisible(false);
			m_compare.SetVisible(false);
			if (flag6)
			{
				m_compareCaption.UpdateText(LocaManager.GetText("TOOLTIP_ITEM_COMPARISON"));
				m_compareCaption.UpdatePositionY(-num);
				num += m_compareCaption.Size.y + m_innerPadding;
				m_compare.UpdateText(m_compareText);
				m_compare.UpdatePositionY(-num);
				num += m_compare.Size.y + m_innerPadding;
			}
			m_scaleWithComparison = m_background.GetScale();
			m_scaleWithComparison.y = num - m_innerPadding + m_outerPadding;
		}

		public void Show()
		{
			NGUITools.SetActiveSelf(gameObject, true);
		}

		public void ShowComparison()
		{
			if (m_compareText != String.Empty)
			{
				m_compareCaption.SetVisible(true);
				m_compare.SetVisible(true);
				m_background.Scale(m_scaleWithComparison.x, m_scaleWithComparison.y);
			}
		}

		private void FillItemName(BaseItem p_item)
		{
			String str = m_nameColorPlainHex;
			Equipment equipment = p_item as Equipment;
			String empty = String.Empty;
			if (equipment != null)
			{
				if (equipment.Identified)
				{
					if (equipment.IsRelic())
					{
						str = m_nameColorRelicHex;
					}
					else
					{
						Int32 num = equipment.Prefixes.Count + equipment.Suffixes.Count;
						Int32 num2 = num;
						if (num2 != 1)
						{
							if (num2 == 2)
							{
								str = m_nameColorDoubleEnchantedHex;
							}
						}
						else
						{
							str = m_nameColorEnchantedHex;
						}
					}
				}
				else
				{
					str = m_nameColorNotIdentifiedHex;
				}
			}
			m_itemName = str + p_item.Name + "[-]" + empty;
		}

		public static Color GetItemColor(BaseItem p_item)
		{
			Equipment equipment = p_item as Equipment;
			if (equipment != null)
			{
				if (!equipment.Identified)
				{
					return m_nameColorNotIdentified;
				}
				if (equipment.IsRelic())
				{
					return m_nameColorRelic;
				}
				Int32 num = equipment.Prefixes.Count + equipment.Suffixes.Count;
				Int32 num2 = num;
				if (num2 == 1)
				{
					return m_nameColorEnchanted;
				}
				if (num2 == 2)
				{
					return m_nameColorDoubleEnchanted;
				}
			}
			return m_nameColorPlain;
		}

		private void FillEnchantmentsDescription(BaseItem p_item)
		{
			Equipment equipment = p_item as Equipment;
			if (equipment != null)
			{
				FillPrefixDescription(equipment);
				FillSuffixDescription(equipment);
			}
		}

		private void FillPrefixDescription(Equipment p_equip)
		{
			foreach (PrefixStaticData prefixStaticData in p_equip.Prefixes)
			{
				Single valueForLevel = prefixStaticData.GetValueForLevel(p_equip.PrefixLevel);
				if (m_itemDetails != String.Empty)
				{
					m_itemDetails += "\n";
				}
				if (valueForLevel > 0f)
				{
					String arg = m_effectColorHex + valueForLevel.ToString() + "[-]";
					m_itemDetails = m_itemDetails + "+" + LocaManager.GetText(prefixStaticData.Description, arg);
				}
				else
				{
					m_itemDetails += LocaManager.GetText(prefixStaticData.Description);
				}
			}
		}

		private void FillSuffixDescription(Equipment p_equip)
		{
			foreach (SuffixStaticData suffixStaticData in p_equip.Suffixes)
			{
				if (m_itemDetails != String.Empty)
				{
					m_itemDetails += "\n";
				}
				m_itemDetails += GetSuffixDescription(suffixStaticData, suffixStaticData.GetValueForLevel(p_equip.SuffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), p_equip, true, false);
			}
		}

		private String GetSuffixDescription(SuffixStaticData suffix, Single value, Equipment p_equip, Boolean p_useColorTags, Boolean p_short)
		{
			String text = suffix.Description;
			if (p_short)
			{
				text += "_SHORT";
			}
			String text2 = LocaManager.GetText(text);
			if (suffix.ValueMode == EValueMode.NONE)
			{
				if (suffix.HasCondition())
				{
					String arg = WrapSuffixColorTags((suffix.GetChanceForLevel(p_equip.SuffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) * 100f).ToString(), p_useColorTags);
					text2 = String.Format(text2, arg);
				}
			}
			else if (suffix.ValueMode == EValueMode.FIXED)
			{
				if (suffix.HasCondition())
				{
					text2 = String.Format(text2, value);
				}
				else
				{
					String arg2 = WrapSuffixColorTags(value.ToString(), p_useColorTags);
					text2 = String.Format(text2, arg2);
				}
			}
			else if (suffix.ValueMode == EValueMode.PERCENT)
			{
				String text3 = WrapSuffixColorTags((value * 100f).ToString(), p_useColorTags);
				if (suffix.HasCondition())
				{
					String arg3 = WrapSuffixColorTags((suffix.GetChanceForLevel(p_equip.SuffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) * 100f).ToString(), p_useColorTags);
					text2 = String.Format(text2, arg3, text3);
				}
				else
				{
					text2 = String.Format(text2, text3);
				}
			}
			else if (suffix.ValueMode == EValueMode.DEBUFF_VALUE_1)
			{
				Single num = suffix.GetChanceForLevel(p_equip.SuffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) * 100f;
				String arg4 = WrapSuffixColorTags(suffix.GetBuffValueForLevel(p_equip.SuffixLevel, 0), p_useColorTags);
				String arg5 = WrapSuffixColorTags(num.ToString(), p_useColorTags);
				text2 = String.Format(text2, arg5, arg4);
			}
			else if (suffix.ValueMode == EValueMode.DEBUFF_VALUE_2)
			{
				Single num2 = suffix.GetChanceForLevel(p_equip.SuffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) * 100f;
				String arg6 = WrapSuffixColorTags(suffix.GetBuffValueForLevel(p_equip.SuffixLevel, 0), p_useColorTags);
				String arg7 = WrapSuffixColorTags(suffix.GetBuffValueForLevel(p_equip.SuffixLevel, 1), p_useColorTags);
				String arg8 = WrapSuffixColorTags(num2.ToString(), p_useColorTags);
				text2 = String.Format(text2, arg8, arg6, arg7);
			}
			else if (suffix.ValueMode == EValueMode.DEBUFF_VALUE_3)
			{
				Single num3 = suffix.GetChanceForLevel(p_equip.SuffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) * 100f;
				String text4 = WrapSuffixColorTags(suffix.GetBuffValueForLevel(p_equip.SuffixLevel, 0), p_useColorTags);
				String text5 = WrapSuffixColorTags(suffix.GetBuffValueForLevel(p_equip.SuffixLevel, 1), p_useColorTags);
				String text6 = WrapSuffixColorTags(suffix.GetBuffValueForLevel(p_equip.SuffixLevel, 2), p_useColorTags);
				String text7 = WrapSuffixColorTags(num3.ToString(), p_useColorTags);
				text2 = String.Format(text2, new Object[]
				{
					text7,
					text4,
					text5,
					text6
				});
			}
			return text2;
		}

		private String WrapSuffixColorTags(String p_text, Boolean p_useColorTags)
		{
			if (p_useColorTags)
			{
				return m_effectColorHex + p_text + "[-]";
			}
			return p_text;
		}

		private void FillRelicLevelBar(BaseItem p_item)
		{
			Equipment equipment = p_item as Equipment;
			if (equipment != null && equipment.IsRelic() && equipment.Identified)
			{
				m_relicLevelBar.SetVisible(true);
				m_relicLevelBar.UpdateText(LocaManager.GetText("ITEM_TOOLTIP_RELIC_LEVEL", equipment.RelicLevel));
				m_relicXPAmountLabel.text = LocaManager.GetText("ITEM_TOOLTIP_RELIC_XP", equipment.CurrentXP, equipment.RequiredXP);
				Vector3 localPosition = m_relicXPAmountLabel.transform.localPosition;
				Vector3 localPosition2 = new Vector3(-m_relicLevelBar.Label.transform.localPosition.x, m_relicLevelBar.Label.transform.localPosition.y, localPosition.z);
				m_relicXPAmountLabel.transform.localPosition = localPosition2;
				Vector3 localPosition3 = m_relicBarForeground.transform.localPosition;
				Vector3 localScale = m_relicBarForeground.transform.localScale;
				Single num = m_relicXPAmountLabel.relativeSize.y * m_relicXPAmountLabel.transform.localScale.y;
				Single num2 = 0f;
				if (equipment.CurrentXP > 0)
				{
					num2 = 1f / (equipment.RequiredXP / (Single)equipment.CurrentXP);
				}
				Single x = m_relicBarBackground.transform.localScale.x * num2;
				m_relicBarForeground.transform.localPosition = new Vector3(localPosition3.x, localPosition2.y - m_innerPadding - localScale.y / 2f - num + 1f, localPosition3.z);
				m_relicBarShadow.transform.localPosition = new Vector3(localPosition3.x, localPosition2.y - m_innerPadding - localScale.y / 2f - num + 1f, m_relicBarShadow.transform.localPosition.z);
				m_relicBarBackground.transform.localPosition = new Vector3(localPosition3.x, localPosition2.y - m_innerPadding - localScale.y / 2f - num + 1f, m_relicBarBackground.transform.localPosition.z);
				m_relicBarForeground.transform.localScale = new Vector3(x, localScale.y, localScale.z);
				Single y = localScale.y + num * 2f;
				Vector3 size = m_relicLevelBar.Size;
				Vector3 p_size = new Vector3(size.x, y, size.z);
				m_relicLevelBar.Resize(p_size);
			}
			else
			{
				m_relicLevelBar.SetVisible(false);
			}
		}

		private void FillRelicDescription(BaseItem p_item)
		{
			Equipment equipment = p_item as Equipment;
			if (equipment != null && equipment.IsRelic() && equipment.Identified)
			{
				String arg = equipment.Description.Substring(0, equipment.Description.Length - 1);
				m_relicDescriptionText += m_highlightColorHex;
				for (Int32 i = 1; i <= equipment.RelicLevel; i++)
				{
					m_relicDescriptionText += LocaManager.GetText(arg + i);
					if (i < equipment.RelicLevel)
					{
						m_relicDescriptionText += "\n\n";
					}
				}
				m_relicDescriptionText += "[-]";
			}
			else
			{
				m_relicDescriptionText = String.Empty;
			}
		}

		private void FillPropertyDescription(BaseItem p_item)
		{
			IDescribable describable = p_item as IDescribable;
			if (describable != null)
			{
				String text = String.Empty;
				text += LocaManager.GetText(describable.GetTypeDescription());
				m_itemDescription = m_itemDescription + m_highlightColorHex + text;
				m_itemDescription += "[-]";
				Boolean flag = p_item is Equipment && ((Equipment)p_item).Broken;
				if (!(p_item is Equipment) || ((Equipment)p_item).Identified)
				{
					Dictionary<String, String> propertiesDescription = describable.GetPropertiesDescription();
					foreach (KeyValuePair<String, String> keyValuePair in propertiesDescription)
					{
						if (p_item.GetItemType() == EDataType.POTION)
						{
							if (m_itemDetails != String.Empty)
							{
								m_itemDetails += "\n";
							}
							String arg = m_effectColorHex + keyValuePair.Value.ToString() + "[-]";
							m_itemDetails += String.Format(LocaManager.GetText(keyValuePair.Key, arg), new Object[0]);
						}
						else if (p_item.GetItemType() == EDataType.SCROLL)
						{
							if (m_itemDetails != String.Empty)
							{
								m_itemDetails += "\n";
							}
							if (keyValuePair.Key == "SCROLL_SPELL_DESCRIPTION")
							{
								m_itemDetails += keyValuePair.Value;
							}
							else
							{
								m_itemDetails += String.Format(LocaManager.GetText(keyValuePair.Key, LocaManager.GetText(keyValuePair.Value)), new Object[0]);
							}
						}
						else
						{
							if (flag && keyValuePair.Key != "MELEE_EFFECT_CRITICAL_DAMAGE")
							{
								String itemDescription = m_itemDescription;
								m_itemDescription = String.Concat(new String[]
								{
									itemDescription,
									"\n",
									m_brokenColorHex,
									keyValuePair.Value,
									"[-] ",
									LocaManager.GetText(keyValuePair.Key)
								});
							}
							else
							{
								String itemDescription = m_itemDescription;
								m_itemDescription = String.Concat(new String[]
								{
									itemDescription,
									"\n",
									keyValuePair.Value,
									" ",
									LocaManager.GetText(keyValuePair.Key)
								});
							}
							if (p_item is Armor)
							{
								Int32 armorPenalty = GetArmorPenalty((Armor)p_item);
								if (armorPenalty > 0)
								{
									String itemDescription = m_itemDescription;
									m_itemDescription = String.Concat(new String[]
									{
										itemDescription,
										"\n",
										m_brokenColorHex,
										"-",
										LocaManager.GetText("ITEM_TOOLTIP_ARMOR_ATTACK_REDUCTION", armorPenalty),
										"[-]"
									});
								}
							}
						}
					}
				}
			}
		}

		private void FillComparison(Equipment p_item, Equipment p_compare, Equipment p_secondaryCompare)
		{
			if (p_item is MeleeWeapon || p_item is MagicFocus)
			{
				MeleeWeapon meleeWeapon = p_item as MeleeWeapon;
				MeleeWeapon meleeWeapon2 = p_compare as MeleeWeapon;
				MeleeWeapon meleeWeapon3 = p_secondaryCompare as MeleeWeapon;
				MagicFocus magicFocus = p_item as MagicFocus;
				MagicFocus magicFocus2 = p_compare as MagicFocus;
				MagicFocus magicFocus3 = p_secondaryCompare as MagicFocus;
				Int32 num = 0;
				num += ((meleeWeapon == null) ? 0 : meleeWeapon.MinDamage);
				num += ((magicFocus == null) ? 0 : magicFocus.MinDamage);
				num -= ((meleeWeapon2 == null) ? 0 : meleeWeapon2.MinDamage);
				num -= ((meleeWeapon3 == null) ? 0 : meleeWeapon3.MinDamage);
				num -= ((magicFocus2 == null) ? 0 : magicFocus2.MinDamage);
				num -= ((magicFocus3 == null) ? 0 : magicFocus3.MinDamage);
				Int32 num2 = 0;
				num2 += ((meleeWeapon == null) ? 0 : meleeWeapon.MaxDamage);
				num2 += ((magicFocus == null) ? 0 : magicFocus.MaxDamage);
				num2 -= ((meleeWeapon2 == null) ? 0 : meleeWeapon2.MaxDamage);
				num2 -= ((meleeWeapon3 == null) ? 0 : meleeWeapon3.MaxDamage);
				num2 -= ((magicFocus2 == null) ? 0 : magicFocus2.MaxDamage);
				num2 -= ((magicFocus3 == null) ? 0 : magicFocus3.MaxDamage);
				AddCompareDamage(num, num2);
				Int32 num3 = 0;
				num3 += ((meleeWeapon == null) ? 0 : meleeWeapon.CriticalDamage);
				num3 -= ((meleeWeapon2 == null) ? 0 : meleeWeapon2.CriticalDamage);
				Int32 num4 = 0;
				if (meleeWeapon3 != null || magicFocus3 != null)
				{
					num4 += ((meleeWeapon == null) ? 0 : meleeWeapon.CriticalDamage);
					num4 -= ((meleeWeapon3 == null) ? 0 : meleeWeapon3.CriticalDamage);
				}
				AddCompareCriticalDamage(num3, num4, "MELEE_EFFECT_CRITICAL_DAMAGE");
				Int32 num5 = 0;
				num5 += ((magicFocus == null) ? 0 : magicFocus.MagicalCriticalDamage);
				num5 -= ((magicFocus2 == null) ? 0 : magicFocus2.MagicalCriticalDamage);
				num5 -= ((magicFocus3 == null) ? 0 : magicFocus3.MagicalCriticalDamage);
				AddCompareCriticalDamage(num5, 0, "MAGIC_FOCUS_EFFECT_INCREASE_CRITICAL_DAMAGE");
			}
			if (p_item is RangedWeapon)
			{
				RangedWeapon rangedWeapon = (RangedWeapon)p_item;
				RangedWeapon rangedWeapon2 = (RangedWeapon)p_compare;
				Int32 p_minDamage = rangedWeapon.MinDamage - rangedWeapon2.MinDamage;
				Int32 p_maxDamage = rangedWeapon.MaxDamage - rangedWeapon2.MaxDamage;
				AddCompareDamage(p_minDamage, p_maxDamage);
				Int32 p_criticalDamage = rangedWeapon.CriticalDamage - rangedWeapon2.CriticalDamage;
				AddCompareCriticalDamage(p_criticalDamage, 0, "RANGED_EFFECT_CRITICAL_DAMAGE");
			}
			if (p_item is Armor)
			{
				Armor armor = (Armor)p_item;
				Armor armor2 = (Armor)p_compare;
				Int32 num6 = armor.ArmorValue - armor2.ArmorValue;
				if (num6 != 0)
				{
					AddCompareText(Math.Abs(num6) + " " + LocaManager.GetText("ARMOR_EFFECT_AC"), num6 > 0);
				}
				Int32 num7 = -GetArmorPenalty(armor) + GetArmorPenalty(armor2);
				if (num7 != 0)
				{
					AddCompareText(LocaManager.GetText("ITEM_TOOLTIP_ARMOR_ATTACK_REDUCTION", Math.Abs(num7)), num7 > 0);
				}
			}
			else if (p_item is Shield)
			{
				Shield shield = (Shield)p_item;
				Shield shield2 = (Shield)p_compare;
				Int32 num8 = shield.ArmorValue - shield2.ArmorValue;
				if (num8 != 0)
				{
					AddCompareText(Math.Abs(num8) + " " + LocaManager.GetText("SHIELD_EFFECT_AC"), num8 > 0);
				}
			}
			FillComparePrefixes(p_item, p_compare, p_secondaryCompare);
			FillCompareSuffixes(p_item, p_compare, p_secondaryCompare);
		}

		private void AddCompareDamage(Int32 p_minDamage, Int32 p_maxDamage)
		{
			if (p_minDamage != 0 || p_maxDamage != 0)
			{
				if ((p_minDamage < 0 && p_maxDamage > 0) || (p_minDamage > 0 && p_maxDamage < 0))
				{
					AddCompareText(Math.Abs(p_minDamage) + " " + LocaManager.GetText("MELEE_EFFECT_DAMAGE_MIN"), p_minDamage > 0);
					AddCompareText(Math.Abs(p_maxDamage) + " " + LocaManager.GetText("MELEE_EFFECT_DAMAGE_MAX"), p_maxDamage > 0);
				}
				else if (p_minDamage <= 0 && p_maxDamage <= 0)
				{
					AddCompareText(String.Concat(new Object[]
					{
						Math.Abs(p_minDamage),
						"-",
						Math.Abs(p_maxDamage),
						" ",
						LocaManager.GetText("MELEE_EFFECT_DAMAGE")
					}), false);
				}
				else
				{
					AddCompareText(String.Concat(new Object[]
					{
						p_minDamage,
						"-",
						p_maxDamage,
						" ",
						LocaManager.GetText("MELEE_EFFECT_DAMAGE")
					}), true);
				}
			}
		}

		private void AddCompareCriticalDamage(Int32 p_criticalDamage1, Int32 p_criticalDamage2, String p_locaKey)
		{
			if (p_criticalDamage1 != 0 && p_criticalDamage2 != 0)
			{
				if (m_compareText != String.Empty)
				{
					m_compareText += "\n";
				}
				m_compareText += ((p_criticalDamage1 <= 0) ? (m_brokenColorHex + "- ") : (m_effectColorHex + "+ "));
				m_compareText = m_compareText + Math.Abs(p_criticalDamage1) + "%";
				if (p_criticalDamage1 > 0 != p_criticalDamage2 > 0)
				{
					m_compareText += "[-]";
				}
				m_compareText += "/";
				m_compareText += ((p_criticalDamage2 <= 0) ? (m_brokenColorHex + "- ") : (m_effectColorHex + "+ "));
				m_compareText = m_compareText + Math.Abs(p_criticalDamage2) + "%";
				if (p_criticalDamage1 > 0 != p_criticalDamage2 > 0)
				{
					m_compareText = m_compareText + "[-] " + LocaManager.GetText("MELEE_EFFECT_CRITICAL_DAMAGE");
				}
				else
				{
					m_compareText = m_compareText + " " + LocaManager.GetText(p_locaKey) + "[-]";
				}
			}
			else if (p_criticalDamage1 != 0)
			{
				AddCompareText(Math.Abs(p_criticalDamage1) + "% " + LocaManager.GetText(p_locaKey), p_criticalDamage1 > 0);
			}
			else if (p_criticalDamage2 != 0)
			{
				AddCompareText(Math.Abs(p_criticalDamage2) + "% " + LocaManager.GetText(p_locaKey), p_criticalDamage2 > 0);
			}
		}

		private void FillComparePrefixes(Equipment p_item, Equipment p_compare, Equipment p_secondaryCompare)
		{
			foreach (PrefixStaticData prefixStaticData in p_item.Prefixes)
			{
				Single num = prefixStaticData.GetValueForLevel(p_item.PrefixLevel);
				foreach (PrefixStaticData prefixStaticData2 in p_compare.Prefixes)
				{
					if (prefixStaticData.Effect == prefixStaticData2.Effect)
					{
						if (prefixStaticData.School == prefixStaticData2.School)
						{
							num -= prefixStaticData2.GetValueForLevel(p_compare.ModelLevel);
						}
						else
						{
							num = prefixStaticData.GetValueForLevel(p_item.ModelLevel);
						}
					}
				}
				if (p_secondaryCompare != null)
				{
					foreach (PrefixStaticData prefixStaticData3 in p_secondaryCompare.Prefixes)
					{
						if (prefixStaticData.Effect == prefixStaticData3.Effect)
						{
							if (prefixStaticData.School == prefixStaticData3.School)
							{
								num -= prefixStaticData3.GetValueForLevel(p_secondaryCompare.ModelLevel);
							}
							else
							{
								num = prefixStaticData.GetValueForLevel(p_item.ModelLevel);
							}
						}
					}
				}
				if (num != 0f)
				{
					AddCompareText(LocaManager.GetText(prefixStaticData.Description, Math.Abs(num)), num > 0f);
				}
			}
			foreach (PrefixStaticData prefixStaticData4 in p_compare.Prefixes)
			{
				Single num2 = prefixStaticData4.GetValueForLevel(p_compare.PrefixLevel);
				Boolean flag = false;
				foreach (PrefixStaticData prefixStaticData5 in p_item.Prefixes)
				{
					if (prefixStaticData4.Effect == prefixStaticData5.Effect && prefixStaticData4.School == prefixStaticData5.School)
					{
						flag = true;
						break;
					}
				}
				if (p_secondaryCompare != null)
				{
					foreach (PrefixStaticData prefixStaticData6 in p_secondaryCompare.Prefixes)
					{
						if (prefixStaticData4.Effect == prefixStaticData6.Effect)
						{
							num2 += prefixStaticData6.GetValueForLevel(p_secondaryCompare.PrefixLevel);
						}
					}
				}
				if (num2 != 0f && !flag)
				{
					AddCompareText(LocaManager.GetText(prefixStaticData4.Description, num2), false);
				}
			}
			if (p_secondaryCompare != null)
			{
				foreach (PrefixStaticData prefixStaticData7 in p_secondaryCompare.Prefixes)
				{
					Single valueForLevel = prefixStaticData7.GetValueForLevel(p_secondaryCompare.PrefixLevel);
					Boolean flag2 = false;
					foreach (PrefixStaticData prefixStaticData8 in p_item.Prefixes)
					{
						if (prefixStaticData7.Effect == prefixStaticData8.Effect && prefixStaticData7.School == prefixStaticData8.School)
						{
							flag2 = true;
							break;
						}
					}
					foreach (PrefixStaticData prefixStaticData9 in p_compare.Prefixes)
					{
						if (prefixStaticData7.Effect == prefixStaticData9.Effect && prefixStaticData7.School == prefixStaticData9.School)
						{
							flag2 = true;
							break;
						}
					}
					if (valueForLevel != 0f && !flag2)
					{
						AddCompareText(LocaManager.GetText(prefixStaticData7.Description, valueForLevel), false);
					}
				}
			}
		}

		private void FillCompareSuffixes(Equipment p_item, Equipment p_compare, Equipment p_secondaryCompare)
		{
			foreach (SuffixStaticData suffixStaticData in p_item.Suffixes)
			{
				Single num = suffixStaticData.GetValueForLevel(p_item.SuffixLevel, p_item.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
				Boolean flag = false;
				using (List<SuffixStaticData>.Enumerator enumerator2 = p_compare.Suffixes.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						SuffixStaticData suffixStaticData2 = enumerator2.Current;
						if (suffixStaticData.CountableDescription)
						{
							if (suffixStaticData.Description == suffixStaticData2.Description)
							{
								num -= suffixStaticData2.GetValueForLevel(p_compare.SuffixLevel, p_item.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
							}
						}
						else if (suffixStaticData.StaticID == suffixStaticData2.StaticID && p_item.SuffixLevel == p_compare.SuffixLevel)
						{
							flag = true;
						}
					}
				}
				if (p_secondaryCompare != null)
				{
					foreach (SuffixStaticData suffixStaticData3 in p_secondaryCompare.Suffixes)
					{
						if (suffixStaticData.CountableDescription)
						{
							if (suffixStaticData.Description == suffixStaticData3.Description)
							{
								num -= suffixStaticData3.GetValueForLevel(p_compare.SuffixLevel, p_item.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
							}
						}
						else if (suffixStaticData.StaticID == suffixStaticData3.StaticID && p_item.SuffixLevel == p_secondaryCompare.SuffixLevel)
						{
							flag = true;
							break;
						}
					}
				}
				if (num != 0f && !flag)
				{
					AddCompareText(GetSuffixDescription(suffixStaticData, Math.Abs(num), p_item, false, true), num > 0f);
				}
			}
			foreach (SuffixStaticData suffixStaticData4 in p_compare.Suffixes)
			{
				Single num2 = suffixStaticData4.GetValueForLevel(p_compare.SuffixLevel, p_compare.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
				Boolean flag2 = false;
				Boolean flag3 = false;
				foreach (SuffixStaticData suffixStaticData5 in p_item.Suffixes)
				{
					if (suffixStaticData4.CountableDescription)
					{
						if (suffixStaticData4.Description == suffixStaticData5.Description)
						{
							flag3 = true;
							break;
						}
					}
					else if (suffixStaticData4.StaticID == suffixStaticData5.StaticID && p_item.SuffixLevel == p_compare.SuffixLevel)
					{
						flag2 = true;
						break;
					}
				}
				if (p_secondaryCompare != null)
				{
					foreach (SuffixStaticData suffixStaticData6 in p_secondaryCompare.Suffixes)
					{
						if (suffixStaticData4.CountableDescription && suffixStaticData4.Description == suffixStaticData6.Description)
						{
							num2 += suffixStaticData6.GetValueForLevel(p_secondaryCompare.SuffixLevel, p_secondaryCompare.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
							break;
						}
					}
				}
				if (num2 != 0f && !flag3 && !flag2)
				{
					AddCompareText(GetSuffixDescription(suffixStaticData4, num2, p_compare, false, true), false);
				}
			}
			if (p_secondaryCompare != null)
			{
				foreach (SuffixStaticData suffixStaticData7 in p_secondaryCompare.Suffixes)
				{
					Single valueForLevel = suffixStaticData7.GetValueForLevel(p_secondaryCompare.SuffixLevel, p_secondaryCompare.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
					Boolean flag4 = false;
					Boolean flag5 = false;
					foreach (SuffixStaticData suffixStaticData8 in p_item.Suffixes)
					{
						if (suffixStaticData7.CountableDescription)
						{
							if (suffixStaticData7.Description == suffixStaticData8.Description)
							{
								flag5 = true;
								break;
							}
						}
						else if (suffixStaticData7.StaticID == suffixStaticData8.StaticID && p_item.SuffixLevel == p_secondaryCompare.SuffixLevel)
						{
							flag4 = true;
							break;
						}
					}
					foreach (SuffixStaticData suffixStaticData9 in p_compare.Suffixes)
					{
						if (suffixStaticData7.CountableDescription)
						{
							if (suffixStaticData7.Description == suffixStaticData9.Description)
							{
								flag5 = true;
								break;
							}
						}
						else if (suffixStaticData7.StaticID == suffixStaticData9.StaticID && p_compare.SuffixLevel == p_secondaryCompare.SuffixLevel)
						{
							if (flag4)
							{
								flag4 = false;
							}
							break;
						}
					}
					if (valueForLevel != 0f && !flag5 && !flag4)
					{
						AddCompareText(GetSuffixDescription(suffixStaticData7, valueForLevel, p_secondaryCompare, false, true), false);
					}
				}
			}
		}

		private void AddCompareText(String p_text, Boolean p_positive)
		{
			if (m_compareText != String.Empty)
			{
				m_compareText += "\n";
			}
			m_compareText += ((!p_positive) ? (m_brokenColorHex + "- ") : (m_effectColorHex + "+ "));
			m_compareText = m_compareText + p_text + "[-]";
		}

		private Int32 GetArmorPenalty(Armor armor)
		{
			Int32 result = 0;
			if (armor != null)
			{
				EEquipmentType subType = armor.GetSubType();
				Int32[] array;
				if (subType == EEquipmentType.LIGHT_ARMOR)
				{
					array = ConfigManager.Instance.Game.AttackValuePenaltiesLightArmor;
				}
				else
				{
					if (subType != EEquipmentType.HEAVY_ARMOR)
					{
						return 0;
					}
					array = ConfigManager.Instance.Game.AttackValuePenaltiesHeavyArmor;
				}
				Int32 num = armor.ModelLevel - 1;
				if (array != null && num >= 0 && num < array.Length)
				{
					result = array[num];
				}
			}
			return result;
		}

		private String GetBrokenItemAdditionalText(Equipment p_equip)
		{
			String text = m_brokenColorHex;
			Single num = (LegacyLogic.Instance.WorldManager.Difficulty != EDifficulty.NORMAL) ? ConfigManager.Instance.Game.BrokenItemMalusHard : ConfigManager.Instance.Game.BrokenItemMalusNormal;
			Single num2 = (Int32)((1f - num) * 100f);
			IDescribable describable = p_equip as IDescribable;
			if (describable != null)
			{
				Dictionary<String, String> propertiesDescription = describable.GetPropertiesDescription();
				foreach (KeyValuePair<String, String> keyValuePair in propertiesDescription)
				{
					if (!(keyValuePair.Key == "MELEE_EFFECT_CRITICAL_DAMAGE"))
					{
						text += "\n";
						String text2 = LocaManager.GetText(keyValuePair.Key);
						text += LocaManager.GetText("ITEM_TOOLTIP_REDUCED_ITEM_PROPERTY", text2, num2);
					}
				}
			}
			text += "[-]";
			return text;
		}

		private void FillSkillRequirementDescription(BaseItem p_item)
		{
			ISkillDependant skillDependant = p_item as ISkillDependant;
			if (skillDependant != null)
			{
				Int32 skillID = skillDependant.GetRequiredSkillID();
				ETier requiredTier = skillDependant.GetRequiredSkillTier();
				FillSkillRequirementDescrption(requiredTier, skillID);
				if (p_item is MeleeWeapon && (p_item as MeleeWeapon).GetSubType() == EEquipmentType.TWOHANDED && (p_item as MeleeWeapon).GetWeaponType() != EEquipmentType.SPEAR)
				{
					skillID = 8;
					requiredTier = ETier.NOVICE;
					FillSkillRequirementDescrption(requiredTier, skillID);
				}
			}
		}

		private void FillSkillRequirementDescrption(ETier requiredTier, Int32 skillID)
		{
			if (skillID > 0)
			{
				SkillStaticData staticData = StaticDataHandler.GetStaticData<SkillStaticData>(EDataType.SKILL, skillID);
				if (staticData != null)
				{
					if (m_skillRequirements != String.Empty)
					{
						m_skillRequirements += "\n";
					}
					Boolean flag = IsSkillRequirementFulfilled(skillID, requiredTier);
					if (!flag)
					{
						m_skillRequirements += m_brokenColorHex;
					}
					m_skillRequirements += GetSkillRequirementDescription(requiredTier, staticData);
					if (!flag)
					{
						m_skillRequirements += "[-]";
					}
				}
			}
		}

		private String GetSkillRequirementDescription(ETier p_requiredTier, SkillStaticData p_skill)
		{
			String result = String.Empty;
			String text = LocaManager.GetText(p_skill.Name);
			if (p_requiredTier == ETier.NONE)
			{
				result = String.Format(LocaManager.GetText("SKILL_REQUIREMENT_TT"), text);
			}
			else
			{
				String text2 = LocaManager.GetText("SKILL_TIER_REQUIREMENT_TT");
				if (p_requiredTier == ETier.NOVICE)
				{
					result = String.Format(text2, text, LocaManager.GetText("SKILL_TIER_1"));
				}
				else if (p_requiredTier == ETier.EXPERT)
				{
					result = String.Format(text2, text, LocaManager.GetText("SKILL_TIER_2"));
				}
				else if (p_requiredTier == ETier.MASTER)
				{
					result = String.Format(text2, text, LocaManager.GetText("SKILL_TIER_3"));
				}
				else if (p_requiredTier == ETier.GRAND_MASTER)
				{
					result = String.Format(text2, text, LocaManager.GetText("SKILL_TIER_4"));
				}
			}
			return result;
		}

		private Boolean IsSkillRequirementFulfilled(Int32 p_skillID, ETier p_tier)
		{
			Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			return selectedCharacter.SkillHandler.HasRequiredSkillTier(p_skillID, p_tier);
		}
	}
}
