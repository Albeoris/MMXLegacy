using System;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/CharacterStatCollection")]
	public class CharacterStatCollection : MonoBehaviour
	{
		[SerializeField]
		private CharacterStatEntry m_hp;

		[SerializeField]
		private CharacterStatEntry m_mana;

		[SerializeField]
		private CharacterStatCondition m_unconscious;

		[SerializeField]
		private CharacterStatCondition m_weak;

		[SerializeField]
		private CharacterStatCondition m_confused;

		[SerializeField]
		private CharacterStatCondition m_poisoned;

		[SerializeField]
		private CharacterStatCondition m_cursed;

		[SerializeField]
		private UILabel m_pointsLeft;

		[SerializeField]
		private GUIMultiSpriteButton m_pointsBtn;

		[SerializeField]
		private CharacterStatEntry m_might;

		[SerializeField]
		private CharacterStatEntry m_magic;

		[SerializeField]
		private CharacterStatEntry m_perception;

		[SerializeField]
		private CharacterStatEntry m_desiny;

		[SerializeField]
		private CharacterStatEntry m_vitality;

		[SerializeField]
		private CharacterStatEntry m_spirit;

		[SerializeField]
		private CharacterStatEntry m_mainHandAttack;

		[SerializeField]
		private CharacterStatEntry m_mainHandDamage;

		[SerializeField]
		private CharacterStatEntry m_mainHandCritChance;

		[SerializeField]
		private CharacterStatEntry m_mainHandCritDamage;

		[SerializeField]
		private CharacterStatEntry m_offHandAttack;

		[SerializeField]
		private CharacterStatEntry m_offHandDamage;

		[SerializeField]
		private CharacterStatEntry m_offHandCritChance;

		[SerializeField]
		private CharacterStatEntry m_offHandCritDamage;

		[SerializeField]
		private CharacterStatEntry m_rangedAttack;

		[SerializeField]
		private CharacterStatEntry m_rangeDamage;

		[SerializeField]
		private CharacterStatEntry m_rangedCritChance;

		[SerializeField]
		private CharacterStatEntry m_rangedCritDamage;

		[SerializeField]
		private CharacterStatEntry m_magicCritChance;

		[SerializeField]
		private CharacterStatEntry m_magicCritDamage;

		[SerializeField]
		private CharacterStatEntry m_magicAttackFire;

		[SerializeField]
		private CharacterStatEntry m_magicAttackWater;

		[SerializeField]
		private CharacterStatEntry m_magicAttackAir;

		[SerializeField]
		private CharacterStatEntry m_magicAttackEarth;

		[SerializeField]
		private CharacterStatEntry m_magicAttackLight;

		[SerializeField]
		private CharacterStatEntry m_magicAttackDark;

		[SerializeField]
		private CharacterStatEntry m_magicAttackPrime;

		[SerializeField]
		private CharacterStatEntry m_defAC;

		[SerializeField]
		private CharacterStatEntry m_defEvade;

		[SerializeField]
		private CharacterStatEntry m_defBlockChance;

		[SerializeField]
		private CharacterStatEntry m_defBlocksGeneral;

		[SerializeField]
		private CharacterStatEntry m_defBlocksMelee;

		[SerializeField]
		private CharacterStatEntry m_resiFire;

		[SerializeField]
		private CharacterStatEntry m_resiWater;

		[SerializeField]
		private CharacterStatEntry m_resiEarth;

		[SerializeField]
		private CharacterStatEntry m_resiAir;

		[SerializeField]
		private CharacterStatEntry m_resiLight;

		[SerializeField]
		private CharacterStatEntry m_resiDark;

		[SerializeField]
		private CharacterStatEntry m_resiPrime;

		private Party m_party;

		private Color m_colorRed;

		private Color m_colorGreen;

		private String m_colorRedHex;

		private String m_colorGreenHex;

		public void Init(Party p_party)
		{
			m_party = p_party;
			m_colorRed = new Color(0.75f, 0f, 0f);
			m_colorGreen = new Color(0f, 0.5f, 0f);
			m_colorRedHex = "[" + NGUITools.EncodeColor(m_colorRed) + "]";
			m_colorGreenHex = "[" + NGUITools.EncodeColor(m_colorGreen) + "]";
			m_hp.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_HEALTH"));
			m_mana.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_MANA"));
			m_might.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_MIGHT"));
			m_magic.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_MAGIC"));
			m_perception.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_PERCEPTION"));
			m_desiny.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_DESTINY"));
			m_vitality.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_VITALITY"));
			m_spirit.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_SPIRIT"));
			m_mainHandAttack.Init(LocaManager.GetText("CHARACTER_ATTACK_MELEE_MAIN"));
			m_mainHandDamage.Init(LocaManager.GetText("CHARACTER_DAMAGE"));
			m_mainHandCritChance.Init(LocaManager.GetText("CHARACTER_CRITICAL_CHANCE"));
			m_mainHandCritDamage.Init(LocaManager.GetText("CHARACTER_CRITICAL_DAMAGE"));
			m_offHandAttack.Init(LocaManager.GetText("CHARACTER_ATTACK_MELEE_OFF"));
			m_offHandDamage.Init(LocaManager.GetText("CHARACTER_DAMAGE"));
			m_offHandCritChance.Init(LocaManager.GetText("CHARACTER_CRITICAL_CHANCE"));
			m_offHandCritDamage.Init(LocaManager.GetText("CHARACTER_CRITICAL_DAMAGE"));
			m_rangedAttack.Init(LocaManager.GetText("CHARACTER_ATTACK_RANGED"));
			m_rangeDamage.Init(LocaManager.GetText("CHARACTER_DAMAGE"));
			m_rangedCritChance.Init(LocaManager.GetText("CHARACTER_CRITICAL_CHANCE"));
			m_rangedCritDamage.Init(LocaManager.GetText("CHARACTER_CRITICAL_DAMAGE"));
			m_magicCritChance.Init(LocaManager.GetText("CHARACTER_CRITICAL_CHANCE"));
			m_magicCritDamage.Init(LocaManager.GetText("CHARACTER_CRITICAL_DAMAGE"));
			m_magicAttackFire.Init(LocaManager.GetText("CHARACTER_RESISTANCE_FIRE"));
			m_magicAttackWater.Init(LocaManager.GetText("CHARACTER_RESISTANCE_WATER"));
			m_magicAttackAir.Init(LocaManager.GetText("CHARACTER_RESISTANCE_AIR"));
			m_magicAttackEarth.Init(LocaManager.GetText("CHARACTER_RESISTANCE_EARTH"));
			m_magicAttackLight.Init(LocaManager.GetText("CHARACTER_RESISTANCE_LIGHT"));
			m_magicAttackDark.Init(LocaManager.GetText("CHARACTER_RESISTANCE_DARK"));
			m_magicAttackPrime.Init(LocaManager.GetText("CHARACTER_RESISTANCE_PRIMORDIAL"));
			m_defAC.Init(LocaManager.GetText("CHARACTER_DEFENSE_AC"));
			m_defEvade.Init(LocaManager.GetText("CHARACTER_DEFENSE_EVADE_VALUE"));
			m_defBlockChance.Init(LocaManager.GetText("CHARACTER_DEFENSE_BLOCK_CHANCE"));
			m_defBlocksGeneral.Init(LocaManager.GetText("CHARACTER_DEFENSE_GENERAL_BLOCK_ATTEMPTS"));
			m_defBlocksMelee.Init(LocaManager.GetText("CHARACTER_DEFENSE_MELEE_BLOCK_ATTEMPTS"));
			m_resiFire.Init(LocaManager.GetText("CHARACTER_RESISTANCE_FIRE"));
			m_resiWater.Init(LocaManager.GetText("CHARACTER_RESISTANCE_WATER"));
			m_resiEarth.Init(LocaManager.GetText("CHARACTER_RESISTANCE_EARTH"));
			m_resiAir.Init(LocaManager.GetText("CHARACTER_RESISTANCE_AIR"));
			m_resiLight.Init(LocaManager.GetText("CHARACTER_RESISTANCE_LIGHT"));
			m_resiDark.Init(LocaManager.GetText("CHARACTER_RESISTANCE_DARK"));
			m_resiPrime.Init(LocaManager.GetText("CHARACTER_RESISTANCE_PRIMORDIAL"));
		}

		public void UpdateContent()
		{
			UpdateHPMana();
			UpdateConditions();
			UpdateAttributes();
			UpdateMelee();
			UpdateRanged();
			UpdateMagic();
			UpdateDefense();
			UpdateResis();
		}

		private void UpdateHPMana()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			GameConfig game = ConfigManager.Instance.Game;
			Int32 num = member.BaseAttributes.HealthPoints + (Int32)(game.HealthPerMight * member.BaseAttributes.Might + member.Class.GetHPPerVitality() * member.BaseAttributes.Vitality);
			Int32 num2 = member.BaseAttributes.ManaPoints + (Int32)(game.ManaPerMagic * member.BaseAttributes.Magic + game.ManaPerSpirit * member.BaseAttributes.Spirit);
			String text = member.CurrentAttributes.HealthPoints.ToString();
			if (num < member.CurrentAttributes.HealthPoints)
			{
				text = m_colorGreenHex + text + "[-]";
			}
			else if (num > member.CurrentAttributes.HealthPoints)
			{
				text = m_colorRedHex + text + "[-]";
			}
			String p_tt = LocaManager.GetText("CHARACTER_ATTRIBUTE_HEALTH_TT") + "\n\n" + LocaManager.GetText("GUI_STATS_X_OF_Y", member.HealthPoints, text);
			String text2 = member.CurrentAttributes.ManaPoints.ToString();
			if (num2 < member.CurrentAttributes.ManaPoints)
			{
				text2 = m_colorGreenHex + text2 + "[-]";
			}
			else if (num2 > member.CurrentAttributes.HealthPoints)
			{
				text2 = m_colorRedHex + text2 + "[-]";
			}
			String p_tt2 = LocaManager.GetText("CHARACTER_ATTRIBUTE_MANA_TT") + "\n\n" + LocaManager.GetText("GUI_STATS_X_OF_Y", member.ManaPoints, text2);
			m_hp.UpdateLabel(LocaManager.GetText("GUI_STATS_X_OF_Y", member.HealthPoints, text), p_tt);
			m_mana.UpdateLabel(LocaManager.GetText("GUI_STATS_X_OF_Y", member.ManaPoints, text2), p_tt2);
		}

		private void UpdateConditions()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			if (member.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS))
			{
				m_unconscious.SetCondition(ECondition.UNCONSCIOUS, member);
			}
			else if (member.ConditionHandler.HasCondition(ECondition.PARALYZED))
			{
				m_unconscious.SetCondition(ECondition.PARALYZED, member);
			}
			else if (member.ConditionHandler.HasCondition(ECondition.STUNNED))
			{
				m_unconscious.SetCondition(ECondition.STUNNED, member);
			}
			else if (member.ConditionHandler.HasCondition(ECondition.SLEEPING))
			{
				m_unconscious.SetCondition(ECondition.SLEEPING, member);
			}
			else
			{
				m_unconscious.SetCondition(ECondition.SLEEPING, member);
			}
			m_weak.SetCondition(ECondition.WEAK, member);
			m_confused.SetCondition(ECondition.CONFUSED, member);
			m_poisoned.SetCondition(ECondition.POISONED, member);
			m_cursed.SetCondition(ECondition.CURSED, member);
		}

		private void UpdateAttributes()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			GameConfig game = ConfigManager.Instance.Game;
			String arg = "[000000]";
			if (member.AttributePoints > 0)
			{
				arg = "[008000]";
			}
			m_pointsLeft.text = LocaManager.GetText("GUI_POINTS_LEFT", arg, member.AttributePoints);
			NGUITools.SetActive(m_pointsBtn.gameObject, member.AttributePoints > 0);
			Int32 p_currentValue = member.CurrentAttributes.Might;
			Int32 p_baseValue = member.BaseAttributes.Might;
			m_might.UpdateLabel(GetColoredValue(p_currentValue, p_baseValue, true), LocaManager.GetText("CHARACTER_ATTRIBUTE_MIGHT_TT", game.HealthPerMight, GetColoredValue(p_currentValue, p_baseValue, false)));
			p_currentValue = member.CurrentAttributes.Magic;
			p_baseValue = member.BaseAttributes.Magic;
			m_magic.UpdateLabel(GetColoredValue(p_currentValue, p_baseValue, true), LocaManager.GetText("CHARACTER_ATTRIBUTE_MAGIC_TT", game.ManaPerMagic, GetColoredValue(p_currentValue, p_baseValue, false)));
			p_currentValue = member.CurrentAttributes.Perception;
			p_baseValue = member.BaseAttributes.Perception;
			m_perception.UpdateLabel(GetColoredValue(p_currentValue, p_baseValue, true), LocaManager.GetText("CHARACTER_ATTRIBUTE_PERCEPTION_TT", GetColoredValue(p_currentValue, p_baseValue, false)));
			p_currentValue = member.CurrentAttributes.Destiny;
			p_baseValue = member.BaseAttributes.Destiny;
			m_desiny.UpdateLabel(GetColoredValue(p_currentValue, p_baseValue, true), LocaManager.GetText("CHARACTER_ATTRIBUTE_DESTINY_TT", GetColoredValue(p_currentValue, p_baseValue, false)));
			p_currentValue = member.CurrentAttributes.Vitality;
			p_baseValue = member.BaseAttributes.Vitality;
			m_vitality.UpdateLabel(GetColoredValue(p_currentValue, p_baseValue, true), LocaManager.GetText("CHARACTER_ATTRIBUTE_VITALITY_TT", member.Class.GetHPPerVitality(), GetColoredValue(p_currentValue, p_baseValue, false)));
			p_currentValue = member.CurrentAttributes.Spirit;
			p_baseValue = member.BaseAttributes.Spirit;
			m_spirit.UpdateLabel(GetColoredValue(p_currentValue, p_baseValue, true), LocaManager.GetText("CHARACTER_ATTRIBUTE_SPIRIT_TT", game.ManaPerSpirit, GetColoredValue(p_currentValue, p_baseValue, false)));
		}

		private void UpdateMelee()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			Boolean flag = false;
			BaseItem itemAt = member.Equipment.GetItemAt(EEquipSlots.MAIN_HAND);
			if (itemAt is MeleeWeapon && (itemAt as MeleeWeapon).GetSubType() == EEquipmentType.TWOHANDED)
			{
				flag = true;
			}
			String text = String.Empty;
			String text2 = String.Empty;
			String text3 = String.Empty;
			Armor armor = member.Equipment.GetItemAt(EEquipSlots.BODY) as Armor;
			Int32 armorPenalty = member.FightHandler.GetArmorPenalty();
			Int32 attackValuePenaltyReduction = member.FightValues.AttackValuePenaltyReduction;
			Int32 num = Math.Max(armorPenalty - attackValuePenaltyReduction, 0);
			if (armor != null && armor.GetSubType() == EEquipmentType.HEAVY_ARMOR && num > 0)
			{
				text3 = LocaManager.GetText("CHARACTER_ATTACK_REDUCED_HEAVY", num);
			}
			else if (armor != null && armor.GetSubType() == EEquipmentType.LIGHT_ARMOR && num > 0)
			{
				text3 = LocaManager.GetText("CHARACTER_ATTACK_REDUCED_MEDIUM", num);
			}
			if (flag)
			{
				String text4 = ((Int32)(member.FightValues.MainHandAttackValue + 0.5f)).ToString();
				if (itemAt is MagicFocus)
				{
					text4 = "-";
				}
				text2 = LocaManager.GetText("CHARACTER_ATTACK_TWONHAND_TT", text4);
				if (text3 != String.Empty)
				{
					text2 = text2 + "\n" + text3;
				}
				m_mainHandAttack.UpdateLabel(text4, text2);
				Int32 num2 = 0;
				Int32 num3 = 0;
				for (EDamageType edamageType = EDamageType.PHYSICAL; edamageType < EDamageType._MAX_; edamageType++)
				{
					num2 += member.FightValues.MainHandDamage[edamageType].Minimum;
					num3 += member.FightValues.MainHandDamage[edamageType].Maximum;
				}
				text = num2 + "-" + num3;
				m_mainHandDamage.UpdateLabel(text, LocaManager.GetText("CHARACTER_DAMAGE_WEAPON", text));
				String text5 = LocaManager.GetText("CHARACTER_TWOHAND_CRIT_CHANCE_TT", (Int32)(member.FightValues.CriticalMainHandHitChance * 100f + 0.5f));
				String text6 = LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_TT", (Int32)(member.FightValues.MainHandCriticalDamageMod * 100f + 0.5f));
				m_mainHandCritChance.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_CHANCE_INFO", (Int32)(member.FightValues.CriticalMainHandHitChance * 100f + 0.5f)), text5);
				m_mainHandCritDamage.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_INFO", (Int32)(member.FightValues.MainHandCriticalDamageMod * 100f + 0.5f)), text6);
				m_offHandAttack.UpdateLabel("0", String.Empty);
				m_offHandDamage.UpdateLabel("0", String.Empty);
				m_offHandCritChance.UpdateLabel("0", String.Empty);
				m_offHandCritDamage.UpdateLabel("0", String.Empty);
			}
			else
			{
				String text7 = ((Int32)(member.FightValues.MainHandAttackValue + 0.5f)).ToString();
				if (itemAt is MagicFocus)
				{
					text7 = "-";
				}
				text2 = LocaManager.GetText("CHARACTER_ATTACK_MAINHAND_TT", text7);
				if (text3 != String.Empty)
				{
					text2 = text2 + "\n" + text3;
				}
				m_mainHandAttack.UpdateLabel(text7, text2);
				Int32 num4 = 0;
				Int32 num5 = 0;
				for (EDamageType edamageType2 = EDamageType.PHYSICAL; edamageType2 < EDamageType._MAX_; edamageType2++)
				{
					num4 += member.FightValues.MainHandDamage[edamageType2].Minimum;
					num5 += member.FightValues.MainHandDamage[edamageType2].Maximum;
				}
				text = num4 + "-" + num5;
				m_mainHandDamage.UpdateLabel(text, LocaManager.GetText("CHARACTER_DAMAGE_WEAPON", text));
				String text8 = LocaManager.GetText("CHARACTER_MAINHAND_CRIT_CHANCE_TT", (Int32)(member.FightValues.CriticalMainHandHitChance * 100f + 0.5f));
				String text9 = LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_TT", (Int32)(member.FightValues.MainHandCriticalDamageMod * 100f + 0.5f));
				m_mainHandCritChance.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_CHANCE_INFO", (Int32)(member.FightValues.CriticalMainHandHitChance * 100f + 0.5f)), text8);
				m_mainHandCritDamage.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_INFO", (Int32)(member.FightValues.MainHandCriticalDamageMod * 100f + 0.5f)), text9);
				BaseItem itemAt2 = member.Equipment.GetItemAt(EEquipSlots.OFF_HAND);
				if (itemAt2 != null && !(itemAt2 is Shield))
				{
					text7 = ((Int32)(member.FightValues.OffHandAttackValue + 0.5f)).ToString();
					if (itemAt2 is MagicFocus)
					{
						text7 = "-";
					}
					text2 = LocaManager.GetText("CHARACTER_ATTACK_OFFHAND_TT", text7);
					if (text3 != String.Empty)
					{
						text2 = text2 + "\n" + text3;
					}
					m_offHandAttack.UpdateLabel(text7, text2);
					num4 = 0;
					num5 = 0;
					for (EDamageType edamageType3 = EDamageType.PHYSICAL; edamageType3 < EDamageType._MAX_; edamageType3++)
					{
						num4 += member.FightValues.OffHandDamage[edamageType3].Minimum;
						num5 += member.FightValues.OffHandDamage[edamageType3].Maximum;
					}
					text = num4 + "-" + num5;
					m_offHandDamage.UpdateLabel(text, LocaManager.GetText("CHARACTER_DAMAGE_WEAPON", text));
					String text10 = LocaManager.GetText("CHARACTER_OFFHAND_CRIT_CHANCE_TT", (Int32)(member.FightValues.CriticalOffHandHitChance * 100f + 0.5f));
					String text11 = LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_TT", (Int32)(member.FightValues.OffHandCriticalDamageMod * 100f + 0.5f));
					m_offHandCritChance.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_CHANCE_INFO", (Int32)(member.FightValues.CriticalOffHandHitChance * 100f + 0.5f)), text10);
					m_offHandCritDamage.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_INFO", (Int32)(member.FightValues.OffHandCriticalDamageMod * 100f + 0.5f)), text11);
				}
				else
				{
					m_offHandAttack.UpdateLabel("0", String.Empty);
					m_offHandDamage.UpdateLabel("0", String.Empty);
					m_offHandCritChance.UpdateLabel("0", String.Empty);
					m_offHandCritDamage.UpdateLabel("0", String.Empty);
				}
			}
		}

		private void UpdateRanged()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			BaseItem itemAt = member.Equipment.GetItemAt(EEquipSlots.RANGE_WEAPON);
			if (itemAt != null)
			{
				Int32 num = (Int32)(member.FightValues.RangedAttackValue + 0.5f);
				m_rangedAttack.UpdateLabel(num.ToString(), LocaManager.GetText("CHARACTER_ATTACK_RANGED_TT", num));
				Int32 num2 = 0;
				Int32 num3 = 0;
				for (EDamageType edamageType = EDamageType.PHYSICAL; edamageType < EDamageType._MAX_; edamageType++)
				{
					num2 += member.FightValues.RangeDamage[edamageType].Minimum;
					num3 += member.FightValues.RangeDamage[edamageType].Maximum;
				}
				String text = num2 + "-" + num3;
				m_rangeDamage.UpdateLabel(text, LocaManager.GetText("CHARACTER_DAMAGE_WEAPON", text));
				String text2 = LocaManager.GetText("CHARACTER_RANGED_CRIT_CHANCE_TT", (Int32)(member.FightValues.CriticalRangeHitChance * 100f + 0.5f));
				String text3 = LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_TT", (Int32)(member.FightValues.RangeCriticalDamageMod * 100f + 0.5f));
				m_rangedCritChance.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_CHANCE_INFO", (Int32)(member.FightValues.CriticalRangeHitChance * 100f + 0.5f)), text2);
				m_rangedCritDamage.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_INFO", (Int32)(member.FightValues.RangeCriticalDamageMod * 100f + 0.5f)), text3);
			}
			else
			{
				m_rangedAttack.UpdateLabel("0", String.Empty);
				m_rangeDamage.UpdateLabel("0", String.Empty);
				m_rangedCritChance.UpdateLabel("0", String.Empty);
				m_rangedCritDamage.UpdateLabel("0", String.Empty);
			}
		}

		private void UpdateMagic()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			String p_text = String.Format("{0:0.0}", member.FightValues.MagicPowers[ESkillID.SKILL_FIRE_MAGIC]);
			m_magicAttackFire.UpdateLabel(p_text, LocaManager.GetText("CHARACTER_MAGIC_POWER_FIRE"));
			p_text = String.Format("{0:0.0}", member.FightValues.MagicPowers[ESkillID.SKILL_WATER_MAGIC]);
			m_magicAttackWater.UpdateLabel(p_text, LocaManager.GetText("CHARACTER_MAGIC_POWER_WATER"));
			p_text = String.Format("{0:0.0}", member.FightValues.MagicPowers[ESkillID.SKILL_AIR_MAGIC]);
			m_magicAttackAir.UpdateLabel(p_text, LocaManager.GetText("CHARACTER_MAGIC_POWER_AIR"));
			p_text = String.Format("{0:0.0}", member.FightValues.MagicPowers[ESkillID.SKILL_EARTH_MAGIC]);
			m_magicAttackEarth.UpdateLabel(p_text, LocaManager.GetText("CHARACTER_MAGIC_POWER_EARTH"));
			p_text = String.Format("{0:0.0}", member.FightValues.MagicPowers[ESkillID.SKILL_LIGHT_MAGIC]);
			m_magicAttackLight.UpdateLabel(p_text, LocaManager.GetText("CHARACTER_MAGIC_POWER_LIGHT"));
			p_text = String.Format("{0:0.0}", member.FightValues.MagicPowers[ESkillID.SKILL_DARK_MAGIC]);
			m_magicAttackDark.UpdateLabel(p_text, LocaManager.GetText("CHARACTER_MAGIC_POWER_DARK"));
			p_text = String.Format("{0:0.0}", member.FightValues.MagicPowers[ESkillID.SKILL_PRIMORDIAL_MAGIC]);
			m_magicAttackPrime.UpdateLabel(p_text, LocaManager.GetText("CHARACTER_MAGIC_POWER_PRIME"));
			String text = LocaManager.GetText("CHARACTER_MAGIC_CRIT_CHANCE_TT", (Int32)(member.FightValues.CriticalMagicHitChance * 100f + 0.5f));
			String text2 = LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_TT", (Int32)((member.FightValues.MagicalCriticalDamageMod - 1f) * 100f + 0.5f));
			m_magicCritChance.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_CHANCE_INFO", (Int32)(member.FightValues.CriticalMagicHitChance * 100f + 0.5f)), text);
			m_magicCritDamage.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_DAMAGE_INFO", (Int32)((member.FightValues.MagicalCriticalDamageMod - 1f) * 100f + 0.5f)), text2);
		}

		private void UpdateDefense()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			Int32 num = (Int32)member.FightValues.EvadeValue;
			Int32 num2 = (Int32)(member.FightValues.GeneralBlockChance * 100f + 0.5f);
			String text = num.ToString();
			if (member.ConditionHandler.HasCondition(ECondition.POISONED))
			{
				text = m_colorRedHex + num + "[-]";
			}
			m_defAC.UpdateLabel(member.FightValues.ArmorValue.ToString(), LocaManager.GetText("CHARACTER_DEFENSE_AC_TT", member.FightValues.ArmorValue));
			PartyBuff buff = m_party.Buffs.GetBuff(EPartyBuffs.WIND_SHIELD);
			if (buff != null)
			{
				String text2 = String.Concat(new Object[]
				{
					text,
					"/",
					m_colorGreenHex,
					num + buff.GetRangedEvadeBonus(),
					"[-]"
				});
				m_defEvade.UpdateLabel(text2, LocaManager.GetText("CHARACTER_DEFENSE_EVADE_VALUE_TT", text2));
			}
			else
			{
				m_defEvade.UpdateLabel(text, LocaManager.GetText("CHARACTER_DEFENSE_EVADE_VALUE_TT", num));
			}
			m_defBlockChance.UpdateLabel(LocaManager.GetText("CHARACTER_ATTACK_CRIT_CHANCE_INFO", num2.ToString()), LocaManager.GetText("CHARACTER_DEFENSE_BLOCK_CHANCE_TT", num2));
			m_defBlocksGeneral.UpdateLabel(member.FightHandler.CurrentGeneralBlockAttempts.ToString(), LocaManager.GetText("CHARACTER_DEFENSE_GENERAL_BLOCK_ATTEMPTS_TT", member.FightHandler.CurrentGeneralBlockAttempts));
			m_defBlocksMelee.UpdateLabel(member.FightHandler.CurrentMeleeBlockAttempts.ToString(), LocaManager.GetText("CHARACTER_DEFENSE_MELEE_BLOCK_ATTEMPTS_TT", member.FightHandler.CurrentMeleeBlockAttempts));
		}

		private void UpdateResis()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			GameConfig game = ConfigManager.Instance.Game;
			Int32 value = member.FightValues.Resistance[EDamageType.FIRE].Value;
			Int32 value2 = member.BaseResistance[EDamageType.FIRE].Value;
			m_resiFire.UpdateLabel(GetColoredValue(value, value2, true), LocaManager.GetText("CHARACTER_RESISTANCE_FIRE_TT", value * game.MagicEvadeFactor, value, value));
			value = member.FightValues.Resistance[EDamageType.WATER].Value;
			value2 = member.BaseResistance[EDamageType.WATER].Value;
			m_resiWater.UpdateLabel(GetColoredValue(value, value2, true), LocaManager.GetText("CHARACTER_RESISTANCE_WATER_TT", value * game.MagicEvadeFactor, value, value));
			value = member.FightValues.Resistance[EDamageType.EARTH].Value;
			value2 = member.BaseResistance[EDamageType.EARTH].Value;
			m_resiEarth.UpdateLabel(GetColoredValue(value, value2, true), LocaManager.GetText("CHARACTER_RESISTANCE_EARTH_TT", value * game.MagicEvadeFactor, value, value));
			value = member.FightValues.Resistance[EDamageType.AIR].Value;
			value2 = member.BaseResistance[EDamageType.AIR].Value;
			m_resiAir.UpdateLabel(GetColoredValue(value, value2, true), LocaManager.GetText("CHARACTER_RESISTANCE_AIR_TT", value * game.MagicEvadeFactor, value, value));
			value = member.FightValues.Resistance[EDamageType.LIGHT].Value;
			value2 = member.BaseResistance[EDamageType.LIGHT].Value;
			m_resiLight.UpdateLabel(GetColoredValue(value, value2, true), LocaManager.GetText("CHARACTER_RESISTANCE_LIGHT_TT", value * game.MagicEvadeFactor, value, value));
			value = member.FightValues.Resistance[EDamageType.DARK].Value;
			value2 = member.BaseResistance[EDamageType.DARK].Value;
			m_resiDark.UpdateLabel(GetColoredValue(value, value2, true), LocaManager.GetText("CHARACTER_RESISTANCE_DARK_TT", value * game.MagicEvadeFactor, value, value));
			value = member.FightValues.Resistance[EDamageType.PRIMORDIAL].Value;
			value2 = member.BaseResistance[EDamageType.PRIMORDIAL].Value;
			m_resiPrime.UpdateLabel(GetColoredValue(value, value2, true), LocaManager.GetText("CHARACTER_RESISTANCE_PRIMORDIAL_TT", value * game.MagicEvadeFactor, value, value));
		}

		private String GetColoredValue(Int32 p_currentValue, Int32 p_baseValue, Boolean showAsTotal)
		{
			String text;
			if (showAsTotal)
			{
				text = p_currentValue.ToString();
				if (p_currentValue > p_baseValue)
				{
					text = m_colorGreenHex + text + "[-]";
				}
				else if (p_currentValue < p_baseValue)
				{
					text = m_colorRedHex + text + "[-]";
				}
			}
			else
			{
				text = p_baseValue.ToString();
				if (p_currentValue > p_baseValue)
				{
					text = String.Concat(new String[]
					{
						p_baseValue.ToString(),
						m_colorGreenHex,
						"+",
						(p_currentValue - p_baseValue).ToString(),
						"[-]"
					});
				}
				else if (p_currentValue < p_baseValue)
				{
					text = String.Concat(new String[]
					{
						p_baseValue.ToString(),
						m_colorRedHex,
						"-",
						(p_baseValue - p_currentValue).ToString(),
						"[-]"
					});
				}
			}
			return text;
		}
	}
}
