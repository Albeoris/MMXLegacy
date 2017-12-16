using System;
using System.Collections.Generic;
using System.Text;
using Legacy.Core;
using Legacy.Core.Abilities;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/BestiaryInfoController")]
	public class BestiaryInfoController : MonoBehaviour
	{
		private const String XP_BAR_NAME = "BAR_line";

		private const String ABILITY_LOCKED = "ICO_ability_locked";

		public const Single BAR_WIDTH = 738f;

		[SerializeField]
		private GameObject m_info1;

		[SerializeField]
		private GameObject m_info2;

		[SerializeField]
		private GameObject m_info3;

		[SerializeField]
		private BestiaryProgressBar m_progressBar;

		[SerializeField]
		private UILabel m_labelRangedMagic;

		[SerializeField]
		private GameObject m_abilityFrame;

		[SerializeField]
		private CharacterStatEntry m_maxHp;

		[SerializeField]
		private CharacterStatEntry m_meleeDmg;

		[SerializeField]
		private CharacterStatEntry m_meleeStrikes;

		[SerializeField]
		private CharacterStatEntry m_rangeDmg;

		[SerializeField]
		private CharacterStatEntry m_rangeStrikes;

		[SerializeField]
		private CharacterStatEntry m_armor;

		[SerializeField]
		private CharacterStatEntry m_meleeAttackValue;

		[SerializeField]
		private CharacterStatEntry m_rangeAttackValue;

		[SerializeField]
		private CharacterStatEntry m_rangeAttackRange;

		[SerializeField]
		private CharacterStatEntry m_blockAttempts;

		[SerializeField]
		private CharacterStatEntry m_critMagic;

		[SerializeField]
		private CharacterStatEntry m_critMelee;

		[SerializeField]
		private CharacterStatEntry m_critRange;

		[SerializeField]
		private CharacterStatEntry m_evade;

		[SerializeField]
		private CharacterStatEntry m_resiPrimordial;

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
		private MonsterStatEntry m_firstSpecial;

		[SerializeField]
		private MonsterStatEntry m_secondSpecial;

		[SerializeField]
		private MonsterStatEntry m_thirdSpecial;

		[SerializeField]
		private MonsterStatEntry m_firstSpell;

		[SerializeField]
		private MonsterStatEntry m_secondSpell;

		[SerializeField]
		private MonsterStatEntry m_thirdSpell;

		[SerializeField]
		private Color m_headLineColor = new Color(0f, 0f, 0f);

		[SerializeField]
		private Color m_textColor = new Color(0f, 0f, 0f);

		private String m_headLineColorHex;

		private String m_textColorHex;

		private List<MonsterStatEntry> m_spells;

		private List<MonsterStatEntry> m_specials;

		private Int32 m_currentAmount;

		private Int32[] m_stages;

		private Dictionary<CharacterStatEntry, String> m_tooltipMapper;

		public void Init()
		{
			m_headLineColorHex = "[" + NGUITools.EncodeColor(m_headLineColor) + "]";
			m_textColorHex = "[" + NGUITools.EncodeColor(m_textColor) + "]";
			m_maxHp.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_HEALTH"));
			m_meleeDmg.Init(LocaManager.GetText("CHARACTER_DAMAGE"));
			m_meleeStrikes.Init(LocaManager.GetText("MONSTER_ATTACK_MELEE_STRIKES"));
			m_rangeDmg.Init(LocaManager.GetText("CHARACTER_DAMAGE"));
			m_rangeStrikes.Init(LocaManager.GetText("MONSTER_ATTACK_RANGED_STRIKES"));
			m_armor.Init(LocaManager.GetText("CHARACTER_DEFENSE_AC"));
			m_meleeAttackValue.Init(LocaManager.GetText("BESTIARY_STAT_ATTACK_VALUE"));
			m_rangeAttackValue.Init(LocaManager.GetText("BESTIARY_STAT_ATTACK_VALUE"));
			m_rangeAttackRange.Init(LocaManager.GetText("MONSTER_ATTACK_RANGE"));
			m_blockAttempts.Init(LocaManager.GetText("CHARACTER_DEFENSE_GENERAL_BLOCK_ATTEMPTS"));
			m_critMagic.Init(LocaManager.GetText("CHARACTER_CRITICAL_DAMAGE"));
			m_critMelee.Init(LocaManager.GetText("CHARACTER_CRITICAL_DAMAGE"));
			m_critRange.Init(LocaManager.GetText("CHARACTER_CRITICAL_DAMAGE"));
			m_evade.Init(LocaManager.GetText("CHARACTER_DEFENSE_EVADE_VALUE"));
			m_resiPrimordial.Init(LocaManager.GetText("CHARACTER_RESISTANCE_PRIMORDIAL"));
			m_resiFire.Init(LocaManager.GetText("CHARACTER_RESISTANCE_FIRE"));
			m_resiWater.Init(LocaManager.GetText("CHARACTER_RESISTANCE_WATER"));
			m_resiEarth.Init(LocaManager.GetText("CHARACTER_RESISTANCE_EARTH"));
			m_resiAir.Init(LocaManager.GetText("CHARACTER_RESISTANCE_AIR"));
			m_resiLight.Init(LocaManager.GetText("CHARACTER_RESISTANCE_LIGHT"));
			m_resiDark.Init(LocaManager.GetText("CHARACTER_RESISTANCE_DARK"));
			m_spells = new List<MonsterStatEntry>();
			m_spells.Add(m_firstSpell);
			m_spells.Add(m_secondSpell);
			m_spells.Add(m_thirdSpell);
			m_specials = new List<MonsterStatEntry>();
			m_specials.Add(m_firstSpecial);
			m_specials.Add(m_secondSpecial);
			m_specials.Add(m_thirdSpecial);
			m_tooltipMapper = new Dictionary<CharacterStatEntry, String>();
			m_tooltipMapper.Add(m_maxHp, "BESTIARY_STAT_ATTRIBUTE_HEALTH_TT");
			m_tooltipMapper.Add(m_meleeDmg, "BESTIARY_STAT_ATTACK_DAMAGE_TT");
			m_tooltipMapper.Add(m_meleeStrikes, "BESTIARY_STAT_STRIKES_TT");
			m_tooltipMapper.Add(m_rangeDmg, "BESTIARY_STAT_ATTACK_DAMAGE_TT");
			m_tooltipMapper.Add(m_rangeStrikes, "BESTIARY_STAT_STRIKES_TT");
			m_tooltipMapper.Add(m_armor, "BESTIARY_STAT_ARMOR_VALUE_TT");
			m_tooltipMapper.Add(m_meleeAttackValue, "BESTIARY_STAT_ATTACK_VALUE_TT");
			m_tooltipMapper.Add(m_rangeAttackValue, "BESTIARY_STAT_ATTACK_VALUE_TT");
			m_tooltipMapper.Add(m_rangeAttackRange, "MONSTER_ATTACK_RANGE_TT");
			m_tooltipMapper.Add(m_blockAttempts, "BESTIARY_STAT_BLOCK_ATTEMPTS_TT");
			m_tooltipMapper.Add(m_critMagic, "BESTIARY_STAT_MAGIC_CRIT_CHANCE_TT");
			m_tooltipMapper.Add(m_critMelee, "BESTIARY_STAT_ATTACK_CRIT_DAMAGE_TT");
			m_tooltipMapper.Add(m_critRange, "BESTIARY_STAT_ATTACK_CRIT_DAMAGE_TT");
			m_tooltipMapper.Add(m_evade, "BESTIARY_STAT_EVADE_VALUE_TT");
			m_tooltipMapper.Add(m_resiPrimordial, "BESTIARY_STAT_RESISTANCE_PRIMORDIAL_TT");
			m_tooltipMapper.Add(m_resiFire, "BESTIARY_STAT_RESISTANCE_FIRE_TT");
			m_tooltipMapper.Add(m_resiWater, "BESTIARY_STAT_RESISTANCE_WATER_TT");
			m_tooltipMapper.Add(m_resiEarth, "BESTIARY_STAT_RESISTANCE_EARTH_TT");
			m_tooltipMapper.Add(m_resiAir, "BESTIARY_STAT_RESISTANCE_AIR_TT");
			m_tooltipMapper.Add(m_resiLight, "BESTIARY_STAT_RESISTANCE_LIGHT_TT");
			m_tooltipMapper.Add(m_resiDark, "BESTIARY_STAT_RESISTANCE_DARK_TT");
			CreateProgressBarSegments();
		}

		private void CreateProgressBarSegments()
		{
			GameObject gameObject = m_progressBar.FillSprite.transform.parent.gameObject;
			Vector3 localPosition = m_progressBar.FillSprite.gameObject.transform.localPosition;
			for (Int32 i = 1; i < 3; i++)
			{
				UISprite uisprite = NGUITools.AddSprite(gameObject, m_progressBar.FillSprite.atlas, "BAR_line");
				Single num = 223.636368f;
				uisprite.layer = 0;
				uisprite.transform.localPosition = new Vector3(localPosition.x + num * i - 1f, localPosition.y, 0f);
				uisprite.transform.localScale = new Vector3(6f, 16f, 1f);
				uisprite.depth = 10;
			}
		}

		public void SetEntry(MonsterStaticData p_data, Int32 p_amountKilled)
		{
			m_stages = p_data.BestiaryThresholds;
			m_currentAmount = p_amountKilled;
			if (p_data.Grade == EMonsterGrade.CHAMPION || p_data.Grade == EMonsterGrade.BOSS)
			{
				m_currentAmount = p_data.BestiaryThresholds[2];
			}
			m_progressBar.SetCurrentAmount(m_currentAmount, p_data.BestiaryThresholds[2]);
			Int32 num;
			if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
			{
				switch (p_data.Grade)
				{
				case EMonsterGrade.CORE:
					num = (Int32)(p_data.MaxHealthpoints * ConfigManager.Instance.Game.MonsterHealthCoreFactor);
					break;
				case EMonsterGrade.ELITE:
					num = (Int32)(p_data.MaxHealthpoints * ConfigManager.Instance.Game.MonsterHealthEliteFactor);
					break;
				case EMonsterGrade.CHAMPION:
					num = (Int32)(p_data.MaxHealthpoints * ConfigManager.Instance.Game.MonsterHealthChampionFactor);
					break;
				default:
					num = p_data.MaxHealthpoints;
					break;
				}
			}
			else
			{
				num = p_data.MaxHealthpoints;
			}
			String p_text = String.Empty;
			if (p_data.MeleeAttackDamageMin == p_data.MeleeAttackDamageMax)
			{
				p_text = p_data.MeleeAttackDamageMin.ToString();
			}
			else
			{
				p_text = p_data.MeleeAttackDamageMin + "-" + p_data.MeleeAttackDamageMax;
			}
			m_maxHp.UpdateLabel(num.ToString(), LocaManager.GetText("BESTIARY_STAT_ATTRIBUTE_HEALTH_TT"));
			m_meleeDmg.UpdateLabel(p_text, LocaManager.GetText("BESTIARY_STAT_ATTACK_DAMAGE_TT"));
			m_meleeStrikes.UpdateLabel(p_data.MeleeAttackStrikesAmount.ToString(), LocaManager.GetText("BESTIARY_STAT_STRIKES_TT"));
			m_critMelee.UpdateLabel(p_data.CriticalDamageMelee.ToString(), LocaManager.GetText("BESTIARY_STAT_ATTACK_CRIT_DAMAGE_TT", p_data.CriticalDamageMelee * 100f));
			m_meleeAttackValue.UpdateLabel(p_data.MeleeAttackValue.ToString(), LocaManager.GetText("BESTIARY_STAT_ATTACK_VALUE_TT"));
			for (Int32 i = 0; i < m_spells.Count; i++)
			{
				m_spells[i].ShowEntry(true);
				if (p_data.Spells.Length > i)
				{
					MonsterSpellStaticData staticData = StaticDataHandler.GetStaticData<MonsterSpellStaticData>(EDataType.MONSTER_SPELLS, p_data.Spells[i].SpellID);
					MonsterSpell monsterSpell = SpellFactory.CreateMonsterSpell((EMonsterSpell)staticData.StaticID, staticData.EffectKey, p_data.Spells[i].SpellProbability);
					m_spells[i].ShowEntry(true);
					m_spells[i].UpdateEntry(staticData.Icon);
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(m_headLineColorHex);
					stringBuilder.Append(LocaManager.GetText(monsterSpell.NameKey));
					stringBuilder.Append("[-]");
					stringBuilder.Append(Environment.NewLine);
					stringBuilder.Append(m_textColorHex);
					stringBuilder.Append(monsterSpell.GetDescriptionForCaster(p_data));
					stringBuilder.Append("[-]");
					m_spells[i].SetTooltip(stringBuilder.ToString());
				}
				else
				{
					m_spells[i].ShowEntry(false);
				}
			}
			if (p_data.AttackRange > 1 && p_data.Spells.Length == 0)
			{
				m_rangeDmg.gameObject.SetActive(true);
				m_rangeStrikes.gameObject.SetActive(true);
				m_rangeAttackValue.gameObject.SetActive(true);
				m_rangeAttackRange.gameObject.SetActive(true);
				m_critRange.gameObject.SetActive(true);
				m_critMagic.gameObject.SetActive(false);
				String p_text2 = String.Empty;
				if (p_data.RangedAttackDamageMin == p_data.RangedAttackDamageMax)
				{
					p_text2 = p_data.RangedAttackDamageMin.ToString();
				}
				else
				{
					p_text2 = p_data.RangedAttackDamageMin + "-" + p_data.RangedAttackDamageMax;
				}
				m_rangeDmg.UpdateLabel(p_text2, LocaManager.GetText("BESTIARY_STAT_ATTACK_DAMAGE_TT", p_data.RangedAttackDamage.Value.ToString()));
				m_rangeStrikes.UpdateLabel(p_data.RangedAttackStrikesAmount.ToString(), LocaManager.GetText("BESTIARY_STAT_STRIKES_TT", p_data.RangedAttackStrikesAmount.ToString()));
				m_rangeAttackValue.UpdateLabel(p_data.RangedAttackValue.ToString(), LocaManager.GetText("BESTIARY_STAT_ATTACK_VALUE_TT", p_data.RangedAttackValue.ToString()));
				m_rangeAttackRange.UpdateLabel(p_data.AttackRange.ToString(), LocaManager.GetText("MONSTER_ATTACK_RANGE_TT", p_data.AttackRange.ToString()));
				m_critRange.UpdateLabel(p_data.CriticalDamageRanged.ToString(), LocaManager.GetText("BESTIARY_STAT_ATTACK_CRIT_DAMAGE_TT", p_data.CriticalDamageRanged * 100f));
				m_labelRangedMagic.text = LocaManager.GetText("GUI_CHARACTER_STATS_RANGED");
				for (Int32 j = 0; j < m_spells.Count; j++)
				{
					m_spells[j].ShowEntry(false);
				}
			}
			else if (p_data.Spells.Length > 0)
			{
				m_rangeDmg.gameObject.SetActive(false);
				m_rangeStrikes.gameObject.SetActive(false);
				m_rangeAttackValue.gameObject.SetActive(false);
				m_rangeAttackRange.gameObject.SetActive(false);
				m_critRange.gameObject.SetActive(false);
				m_critMagic.gameObject.SetActive(true);
				m_labelRangedMagic.text = LocaManager.GetText("GUI_CHARACTER_STATS_MAGIC");
			}
			else
			{
				m_rangeDmg.gameObject.SetActive(true);
				m_rangeStrikes.gameObject.SetActive(true);
				m_rangeAttackValue.gameObject.SetActive(true);
				m_rangeAttackRange.gameObject.SetActive(true);
				m_critRange.gameObject.SetActive(true);
				m_critMagic.gameObject.SetActive(false);
				for (Int32 k = 0; k < m_spells.Count; k++)
				{
					m_spells[k].ShowEntry(false);
				}
				m_labelRangedMagic.text = LocaManager.GetText("GUI_CHARACTER_STATS_RANGED");
				m_rangeDmg.UpdateLabel("-", LocaManager.GetText("BESTIARY_STAT_ATTACK_DAMAGE_TT", "-"));
				m_rangeStrikes.UpdateLabel("-", LocaManager.GetText("BESTIARY_STAT_STRIKES_TT", "-"));
				m_rangeAttackValue.UpdateLabel("-", LocaManager.GetText("BESTIARY_STAT_ATTACK_VALUE_TT", "-"));
				m_rangeAttackRange.UpdateLabel("-", LocaManager.GetText("MONSTER_ATTACK_RANGE_TT", "-"));
				m_critRange.UpdateLabel("-", LocaManager.GetText("BESTIARY_STAT_ATTACK_CRIT_DAMAGE_TT", "-"));
			}
			m_armor.UpdateLabel(p_data.ArmorValue.ToString(), LocaManager.GetText("BESTIARY_STAT_ARMOR_VALUE_TT", p_data.ArmorValue.ToString()));
			m_blockAttempts.UpdateLabel(p_data.GeneralBlockAttemptsPerTurn.ToString(), LocaManager.GetText("BESTIARY_STAT_BLOCK_ATTEMPTS_TT", p_data.GeneralBlockAttemptsPerTurn.ToString()));
			m_evade.UpdateLabel(p_data.EvadeValue.ToString(), LocaManager.GetText("BESTIARY_STAT_EVADE_VALUE_TT", p_data.EvadeValue.ToString()));
			if (p_data.CriticalDamageSpells > 0f)
			{
				m_critMagic.UpdateLabel(p_data.CriticalDamageSpells.ToString(), LocaManager.GetText("BESTIARY_STAT_MAGIC_CRIT_CHANCE_TT", p_data.CriticalDamageSpells * 100f));
			}
			else
			{
				m_critMagic.UpdateLabel("-", LocaManager.GetText("BESTIARY_STAT_MAGIC_CRIT_CHANCE_TT", "-"));
			}
			NGUITools.SetActive(m_abilityFrame, true);
			if (p_data.Abilities.Length == 0)
			{
				NGUITools.SetActive(m_abilityFrame, false);
			}
			for (Int32 l = 0; l < m_specials.Count; l++)
			{
				if (p_data.Abilities.Length > l)
				{
					m_specials[l].ShowEntry(true);
					MonsterAbilityBase monsterAbilityBase = AbilityFactory.CreateMonsterAbility(p_data.Abilities[l].AbilityType, p_data.MagicPower);
					monsterAbilityBase.Level = p_data.Abilities[l].Level;
					m_specials[l].UpdateEntry(monsterAbilityBase.StaticData.Icon);
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.Append(m_headLineColorHex);
					stringBuilder2.Append(LocaManager.GetText(monsterAbilityBase.StaticData.NameKey));
					stringBuilder2.Append("[-]");
					stringBuilder2.Append(Environment.NewLine);
					stringBuilder2.Append(m_textColorHex);
					stringBuilder2.Append(monsterAbilityBase.GetDescription());
					stringBuilder2.Append("[-]");
					m_specials[l].SetTooltip(stringBuilder2.ToString());
				}
				else
				{
					m_specials[l].ShowEntry(false);
				}
			}
			m_resiPrimordial.UpdateLabel("0", LocaManager.GetText("BESTIARY_STAT_RESISTANCE_PRIMORDIAL_TT", 0, 0));
			m_resiFire.UpdateLabel("0", LocaManager.GetText("BESTIARY_STAT_RESISTANCE_FIRE_TT", 0, 0));
			m_resiWater.UpdateLabel("0", LocaManager.GetText("BESTIARY_STAT_RESISTANCE_WATER_TT", 0, 0));
			m_resiEarth.UpdateLabel("0", LocaManager.GetText("BESTIARY_STAT_RESISTANCE_EARTH_TT", 0, 0));
			m_resiAir.UpdateLabel("0", LocaManager.GetText("BESTIARY_STAT_RESISTANCE_AIR_TT", 0, 0));
			m_resiLight.UpdateLabel("0", LocaManager.GetText("BESTIARY_STAT_RESISTANCE_LIGHT_TT", 0, 0));
			m_resiDark.UpdateLabel("0", LocaManager.GetText("BESTIARY_STAT_RESISTANCE_DARK_TT", 0, 0));
			foreach (Resistance resistance in p_data.MagicResistances)
			{
				switch (resistance.Type)
				{
				case EDamageType.AIR:
					m_resiAir.UpdateLabel(resistance.Value.ToString(), LocaManager.GetText("BESTIARY_STAT_RESISTANCE_AIR_TT", resistance.Value * (p_data.EvadeValue / 100), resistance.Value));
					break;
				case EDamageType.EARTH:
					m_resiEarth.UpdateLabel(resistance.Value.ToString(), LocaManager.GetText("BESTIARY_STAT_RESISTANCE_EARTH_TT", resistance.Value * (p_data.EvadeValue / 100), resistance.Value));
					break;
				case EDamageType.FIRE:
					m_resiFire.UpdateLabel(resistance.Value.ToString(), LocaManager.GetText("BESTIARY_STAT_RESISTANCE_FIRE_TT", resistance.Value * (p_data.EvadeValue / 100), resistance.Value));
					break;
				case EDamageType.WATER:
					m_resiWater.UpdateLabel(resistance.Value.ToString(), LocaManager.GetText("BESTIARY_STAT_RESISTANCE_WATER_TT", resistance.Value * (p_data.EvadeValue / 100), resistance.Value));
					break;
				case EDamageType.DARK:
					m_resiDark.UpdateLabel(resistance.Value.ToString(), LocaManager.GetText("BESTIARY_STAT_RESISTANCE_DARK_TT", resistance.Value * (p_data.EvadeValue / 100), resistance.Value));
					break;
				case EDamageType.LIGHT:
					m_resiLight.UpdateLabel(resistance.Value.ToString(), LocaManager.GetText("BESTIARY_STAT_RESISTANCE_LIGHT_TT", resistance.Value * (p_data.EvadeValue / 100), resistance.Value));
					break;
				case EDamageType.PRIMORDIAL:
					m_resiPrimordial.UpdateLabel(resistance.Value.ToString(), LocaManager.GetText("BESTIARY_STAT_RESISTANCE_PRIMORDIAL_TT", resistance.Value * (p_data.EvadeValue / 100), resistance.Value));
					break;
				}
			}
			HideLockedEntries();
		}

		private void HideLockedEntries()
		{
			if (m_currentAmount >= m_stages[0])
			{
				m_info1.SetActive(true);
			}
			else
			{
				CharacterStatEntry[] componentsInChildren = m_info1.GetComponentsInChildren<CharacterStatEntry>();
				foreach (CharacterStatEntry characterStatEntry in componentsInChildren)
				{
					characterStatEntry.UpdateLabel("???", GenerateHiddenEntryTT(characterStatEntry));
				}
				MonsterStatEntry[] componentsInChildren2 = m_info1.GetComponentsInChildren<MonsterStatEntry>();
				foreach (MonsterStatEntry monsterStatEntry in componentsInChildren2)
				{
					monsterStatEntry.UpdateEntry("ICO_ability_locked");
					monsterStatEntry.SetTooltip(LocaManager.GetText("BESTIARY_SPELL_ABILITY_LOCKED", m_stages[0]));
				}
			}
			if (m_currentAmount >= m_stages[1])
			{
				m_info2.SetActive(true);
			}
			else
			{
				CharacterStatEntry[] componentsInChildren3 = m_info2.GetComponentsInChildren<CharacterStatEntry>();
				foreach (CharacterStatEntry characterStatEntry2 in componentsInChildren3)
				{
					characterStatEntry2.UpdateLabel("???", GenerateHiddenEntryTT(characterStatEntry2));
				}
				MonsterStatEntry[] componentsInChildren4 = m_info2.GetComponentsInChildren<MonsterStatEntry>();
				foreach (MonsterStatEntry monsterStatEntry2 in componentsInChildren4)
				{
					monsterStatEntry2.UpdateEntry("ICO_ability_locked");
					monsterStatEntry2.SetTooltip(LocaManager.GetText("BESTIARY_SPELL_ABILITY_LOCKED", m_stages[1]));
				}
			}
			if (m_currentAmount >= m_stages[2])
			{
				m_info3.SetActive(true);
			}
			else
			{
				CharacterStatEntry[] componentsInChildren5 = m_info3.GetComponentsInChildren<CharacterStatEntry>();
				foreach (CharacterStatEntry characterStatEntry3 in componentsInChildren5)
				{
					characterStatEntry3.UpdateLabel("???", GenerateHiddenEntryTT(characterStatEntry3));
				}
				MonsterStatEntry[] componentsInChildren6 = m_info3.GetComponentsInChildren<MonsterStatEntry>();
				foreach (MonsterStatEntry monsterStatEntry3 in componentsInChildren6)
				{
					monsterStatEntry3.UpdateEntry("ICO_ability_locked");
					monsterStatEntry3.SetTooltip(LocaManager.GetText("BESTIARY_SPELL_ABILITY_LOCKED", m_stages[2]));
				}
			}
		}

		private String GenerateHiddenEntryTT(CharacterStatEntry p_affectedEntry)
		{
			String text = "???";
			if (m_tooltipMapper.TryGetValue(p_affectedEntry, out text))
			{
				String text2 = LocaManager.GetText(text);
				if (text2.Contains("{"))
				{
					String[] array = text2.Split("{".ToCharArray());
					for (Int32 i = 1; i < array.Length; i++)
					{
						String str = array[i].Substring(2);
						array[i] = "???" + str;
					}
					StringBuilder stringBuilder = new StringBuilder();
					foreach (String value in array)
					{
						stringBuilder.Append(value);
					}
					text = stringBuilder.ToString();
				}
				else
				{
					text = text2;
				}
			}
			return text;
		}

		public void ShowProgressBar(Boolean p_show)
		{
			NGUITools.SetActive(m_progressBar.gameObject, p_show);
		}

		public void Hide()
		{
			ShowProgressBar(false);
			NGUITools.SetActiveSelf(gameObject, false);
		}

		public void Show()
		{
			NGUITools.SetActiveSelf(gameObject, true);
		}

		public void Cleanup()
		{
		}
	}
}
