using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.Internationalization;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using Legacy.Core.UpdateLogic;

namespace Legacy.Core.PartyManagement
{
	public class Character : ISaveGameObject
	{
		private const Int32 MIN_PORTRAIT = 1;

		private const Int32 MAX_PORTRAIT = 2;

		private String m_name = String.Empty;

		private Attributes m_baseAttributes;

		private FightValues m_fightValues = new FightValues();

		private Attributes m_currentAttributes;

		private Int32 m_HealthPoints;

		private Int32 m_ManaPoints;

		private Int32 m_MaxManaReducedByDeficiency;

		private CharacterClass m_class;

		private ResistanceCollection m_resistance;

		private Int32 m_level;

		private Int32 m_exp;

		private Int32 m_expTreshhold;

		private CharacterInventoryController m_equipment;

		private Boolean m_unlockedAdvancedClass;

		private Boolean m_doneTurn;

		private Monster m_selectedMonster;

		private Monster m_CounterAttackMonster;

		private MMTime m_lastRestTime;

		private String m_portrait;

		private Int32 m_portraitID;

		private String m_body;

		private EGender m_gender;

		private EVoiceSetting m_voice;

		private Int32 m_index;

		private Int32 m_skillPoints;

		private Int32 m_attributePoints;

		private Int32 m_temporyrAttributePoints;

		private CharacterFightHandler m_fightHandler;

		private ConditionHandler m_conditionHandler;

		private CharacterSkillHandler m_skillHandler;

		private CharacterSpellHandler m_spellHandler;

		private PartyBuffHandler m_buffs;

		private ItemEnchantmentHandler m_enchantmentHandler;

		private CharacterQuickActions m_quickActions;

		private CharacterAttributeChanger m_attributeChanger;

		private CharacterBarkHandler m_BarkHandler;

		private List<LogEntryEventArgs> m_activeLogEntryList;

		private List<LogEntryEventArgs> m_logEntries;

		private List<LogEntryEventArgs> m_questRewardLogEntries;

		private CastSpellTask m_currentSpellTask;

		public Character(PartyBuffHandler p_buffs)
		{
			m_buffs = p_buffs;
			m_equipment = new CharacterInventoryController(this);
			m_resistance = new ResistanceCollection();
			m_fightHandler = new CharacterFightHandler(this);
			m_conditionHandler = new ConditionHandler(this);
			m_skillHandler = new CharacterSkillHandler(this);
			m_enchantmentHandler = new ItemEnchantmentHandler();
			m_spellHandler = new CharacterSpellHandler(this);
			m_quickActions = new CharacterQuickActions();
			m_attributeChanger = new CharacterAttributeChanger(this);
			m_BarkHandler = new CharacterBarkHandler(this);
			m_logEntries = new List<LogEntryEventArgs>();
			m_questRewardLogEntries = new List<LogEntryEventArgs>();
			m_activeLogEntryList = m_logEntries;
		}

		public Int32 Index
		{
			get => m_index;
		    internal set => m_index = value;
		}

		public MMTime LastRestTime => m_lastRestTime;

	    public String Name
		{
			get => m_name;
	        set => m_name = value;
	    }

		public EGender Gender => m_gender;

	    public EVoiceSetting VoiceSetting => m_voice;

	    public Int32 PortraitID => m_portraitID;

	    public String Portrait => m_portrait;

	    public String Body => m_body;

	    public Attributes BaseAttributes
		{
			get => m_baseAttributes;
	        set => m_baseAttributes = value;
	    }

		public Attributes CurrentAttributes
		{
			get => m_currentAttributes;
		    set => m_currentAttributes = value;
		}

		public FightValues FightValues => m_fightValues;

	    public Int32 MaxManaReducedByDeficiency
		{
			get => m_MaxManaReducedByDeficiency;
	        set => m_MaxManaReducedByDeficiency = Math.Min(value, MaximumManaPoints);
	    }

		public Int32 MaximumHealthPoints => m_currentAttributes.HealthPoints;

	    public Int32 MaximumManaPoints => m_currentAttributes.ManaPoints;

	    public Int32 HealthPoints => m_HealthPoints;

	    public Int32 ManaPoints => m_ManaPoints;

	    public CharacterClass Class => m_class;

	    public ResistanceCollection BaseResistance => m_resistance;

	    public Int32 Level => m_level;

	    public Boolean MaxLevelReached => m_level == StaticDataHandler.GetCount(EDataType.EXP_TABLE) - 1;

	    public Int32 Exp => m_exp;

	    public Boolean UnlockedAdvancedClass => m_unlockedAdvancedClass;

	    public Int32 ExpNeededForNextLevel
		{
			get
			{
				Int32 num = m_expTreshhold - m_exp;
				if (num < 0)
				{
					num = 0;
				}
				return num;
			}
		}

		public Int32 TotalExpNeededForNextLevel
		{
			get
			{
				Int32 num = m_expTreshhold;
				if (num < 0)
				{
					num = 0;
				}
				return num;
			}
		}

		public Boolean DoneTurn
		{
			get => HasDoneTurn();
		    set => m_doneTurn = value;
		}

		public CharacterInventoryController Equipment => m_equipment;

	    public Monster SelectedMonster
		{
			get => m_selectedMonster;
	        set => m_selectedMonster = value;
	    }

		public ConditionHandler ConditionHandler => m_conditionHandler;

	    public CharacterBarkHandler BarkHandler => m_BarkHandler;

	    public CharacterFightHandler FightHandler => m_fightHandler;

	    public CharacterSkillHandler SkillHandler => m_skillHandler;

	    public ItemEnchantmentHandler EnchantmentHandler => m_enchantmentHandler;

	    public CharacterSpellHandler SpellHandler => m_spellHandler;

	    public CharacterQuickActions QuickActions => m_quickActions;

	    public CharacterAttributeChanger AttributeChanger => m_attributeChanger;

	    public Boolean IsDefendModeActive { get; set; }

		public Int32 SkillPoints
		{
			get => m_skillPoints;
		    set
			{
				if (m_skillPoints != value)
				{
					m_skillPoints = value;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_SKILL_POINTS_CHANGED, EventArgs.Empty);
				}
			}
		}

		public Int32 AttributePoints
		{
			get => m_attributePoints;
		    set
			{
				if (m_attributePoints != value)
				{
					m_attributePoints = value;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_ATTRIBUTE_POINTS_CHANGED, EventArgs.Empty);
				}
			}
		}

		public Int32 TemporaryAttributePoints
		{
			get => m_temporyrAttributePoints;
		    set => m_temporyrAttributePoints = value;
		}

		public Boolean CanSpendSkillpoints => m_skillPoints > 0 && !ConditionHandler.HasCondition(ECondition.DEAD);

	    public Monster CounterAttackMonster
		{
			get => m_CounterAttackMonster;
	        set => m_CounterAttackMonster = value;
	    }

		public void CastSpellEvent(CharacterSpell spell, SpellEventArgs spellEventArgs)
		{
			m_currentSpellTask.Spell = spell;
			m_currentSpellTask.SpellArgs = spellEventArgs;
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_CAST_SPELL, spellEventArgs);
		}

		public void FinishCastSpellEvent()
		{
			if (m_currentSpellTask.Spell != null)
			{
				m_currentSpellTask.Spell.FinishCastSpell(this);
			}
			m_currentSpellTask = default(CastSpellTask);
		}

		public void InitStartValues(CharacterClass p_class, Int32 p_portraitId, String p_name, EGender p_gender, EVoiceSetting p_voice, Attributes p_startAttributes)
		{
			if (String.IsNullOrEmpty(p_name))
			{
				String[] defaultNames = p_class.GetDefaultNames(p_gender);
				p_name = Localization.Instance.GetText(defaultNames[Random.Range(0, defaultNames.Length)]);
			}
			if (p_portraitId < 1)
			{
				p_portraitId = 2;
			}
			if (p_portraitId > 2)
			{
				p_portraitId = 1;
			}
			m_name = p_name;
			m_class = p_class;
			m_gender = p_gender;
			m_portraitID = p_portraitId;
			m_voice = p_voice;
			m_baseAttributes = m_class.InitialAttributes;
			m_baseAttributes.Might = m_baseAttributes.Might + p_startAttributes.Might;
			m_baseAttributes.Magic = m_baseAttributes.Magic + p_startAttributes.Magic;
			m_baseAttributes.Destiny = m_baseAttributes.Destiny + p_startAttributes.Destiny;
			m_baseAttributes.Perception = m_baseAttributes.Perception + p_startAttributes.Perception;
			m_baseAttributes.Vitality = m_baseAttributes.Vitality + p_startAttributes.Vitality;
			m_baseAttributes.Spirit = m_baseAttributes.Spirit + p_startAttributes.Spirit;
			m_class.InitialResistance.CopyTo(m_resistance);
			m_HealthPoints = m_baseAttributes.HealthPoints;
			m_ManaPoints = m_baseAttributes.ManaPoints;
			m_level = 1;
			m_exp = 0;
			m_expTreshhold = StaticDataHandler.GetStaticData<ExpStaticData>(EDataType.EXP_TABLE, m_level).DeltaExp;
			m_portrait = "PIC_head_";
			if (p_class.Race == ERace.HUMAN)
			{
				m_portrait += "human";
			}
			else if (p_class.Race == ERace.ELF)
			{
				m_portrait += "elf";
			}
			else if (p_class.Race == ERace.DWARF)
			{
				m_portrait += "dwarf";
			}
			else if (p_class.Race == ERace.ORC)
			{
				m_portrait += "orc";
			}
			m_body = p_class.StaticData.BodyBase;
			if (p_gender == EGender.MALE)
			{
				String portrait = m_portrait;
				m_portrait = String.Concat(new Object[]
				{
					portrait,
					"_male_",
					m_portraitID,
					"_idle"
				});
				m_body += "_male";
			}
			else
			{
				String portrait = m_portrait;
				m_portrait = String.Concat(new Object[]
				{
					portrait,
					"_female_",
					m_portraitID,
					"_idle"
				});
				m_body += "_female";
			}
			m_skillHandler.Init();
			m_enchantmentHandler.Init(this);
			m_spellHandler.Init();
			CalculateCurrentAttributes();
		}

		public String GetPosingTextName()
		{
			String text = m_class.GetPosingTexName();
			if (m_gender == EGender.MALE)
			{
				text = text + "_male_" + m_portraitID;
			}
			else
			{
				text = text + "_female_" + m_portraitID;
			}
			return text;
		}

		public void SetStandardQuickActions()
		{
			m_quickActions[0] = new CharacterQuickActions.Action(EQuickActionType.ATTACK, null, ECharacterSpell.SPELL_FIRE_WARD);
			m_quickActions[1] = new CharacterQuickActions.Action(EQuickActionType.ATTACKRANGED, null, ECharacterSpell.SPELL_FIRE_WARD);
			m_quickActions[2] = new CharacterQuickActions.Action(EQuickActionType.DEFEND, null, ECharacterSpell.SPELL_FIRE_WARD);
			Int32 num = 3;
			for (ESkillID eskillID = ESkillID.SKILL_FIRE_MAGIC; eskillID <= ESkillID.SKILL_PRIMORDIAL_MAGIC; eskillID++)
			{
				Skill skill = m_skillHandler.FindSkill((Int32)eskillID);
				if (skill != null && skill.Tier >= ETier.NOVICE)
				{
					Int32 num2 = -1;
					foreach (SkillEffectStaticData skillEffectStaticData in skill.CurrentlyAvailableEffects)
					{
						if (skillEffectStaticData.Type == ESkillEffectType.LEARN_SPELL)
						{
							num2 = (Int32)skillEffectStaticData.Value;
						}
					}
					if (num2 >= 0)
					{
						m_quickActions[num] = new CharacterQuickActions.Action(EQuickActionType.CAST_SPELL, null, (ECharacterSpell)num2);
						num++;
						if (num >= 8)
						{
							break;
						}
					}
				}
			}
			Skill skill2 = m_skillHandler.FindSkill(11);
			if (skill2 != null && skill2.Tier >= ETier.NOVICE && num < 8)
			{
				if (num < 8)
				{
					m_quickActions[num] = new CharacterQuickActions.Action(EQuickActionType.CAST_SPELL, null, ECharacterSpell.WARFARE_CHALLENGE);
				}
				num++;
				if (num < 8)
				{
					m_quickActions[num] = new CharacterQuickActions.Action(EQuickActionType.CAST_SPELL, null, ECharacterSpell.WARFARE_SHATTER);
				}
			}
			m_quickActions[8] = new CharacterQuickActions.Action(EQuickActionType.USE_BEST_HEALTHPOTION, null, ECharacterSpell.SPELL_FIRE_WARD);
			m_quickActions[9] = new CharacterQuickActions.Action(EQuickActionType.USE_BEST_MANAPOTION, null, ECharacterSpell.SPELL_FIRE_WARD);
		}

		public void GiveStartSetup()
		{
			foreach (EquipmentData equipmentData in m_class.StaticData.StartEquipment)
			{
				Equipment p_item = ItemFactory.CreateItem(equipmentData.Type, equipmentData.StaticId) as Equipment;
				m_equipment.AddItem(p_item);
			}
			AddAddtionalWeapon(ESkillID.SKILL_SWORD);
			AddAddtionalWeapon(ESkillID.SKILL_AXE);
			AddAddtionalWeapon(ESkillID.SKILL_MACE);
			AddAddtionalWeapon(ESkillID.SKILL_DAGGER);
			AddAddtionalWeapon(ESkillID.SKILL_SPEAR);
			AddAddtionalWeapon(ESkillID.SKILL_MAGICAL_FOCUS);
			AddAddtionalWeapon(ESkillID.SKILL_BOW);
			AddAddtionalWeapon(ESkillID.SKILL_CROSSBOW);
			m_HealthPoints = CurrentAttributes.HealthPoints;
			m_ManaPoints = CurrentAttributes.ManaPoints;
		}

		private void AddAddtionalWeapon(ESkillID p_skill)
		{
			if (!m_skillHandler.HasRequiredSkill((Int32)p_skill))
			{
				return;
			}
			BaseItem itemAt = m_equipment.GetItemAt(EEquipSlots.MAIN_HAND);
			if (itemAt is MeleeWeapon && (((itemAt as MeleeWeapon).GetWeaponType() == EEquipmentType.SWORD && p_skill == ESkillID.SKILL_SWORD) || ((itemAt as MeleeWeapon).GetWeaponType() == EEquipmentType.AXE && p_skill == ESkillID.SKILL_AXE) || ((itemAt as MeleeWeapon).GetWeaponType() == EEquipmentType.MACE && p_skill == ESkillID.SKILL_MACE) || ((itemAt as MeleeWeapon).GetWeaponType() == EEquipmentType.SPEAR && p_skill == ESkillID.SKILL_SPEAR) || ((itemAt as MeleeWeapon).GetWeaponType() == EEquipmentType.DAGGER && p_skill == ESkillID.SKILL_DAGGER)))
			{
				return;
			}
			if (itemAt is MagicFocus && p_skill == ESkillID.SKILL_MAGICAL_FOCUS)
			{
				return;
			}
			if (p_skill == ESkillID.SKILL_SWORD)
			{
				AddExtraItem((Equipment)ItemFactory.CreateItem(EDataType.MELEE_WEAPON, 2), itemAt);
			}
			if (p_skill == ESkillID.SKILL_AXE)
			{
				AddExtraItem((Equipment)ItemFactory.CreateItem(EDataType.MELEE_WEAPON, 3), itemAt);
			}
			if (p_skill == ESkillID.SKILL_MACE)
			{
				AddExtraItem((Equipment)ItemFactory.CreateItem(EDataType.MELEE_WEAPON, 9), itemAt);
			}
			if (p_skill == ESkillID.SKILL_DAGGER)
			{
				AddExtraItem((Equipment)ItemFactory.CreateItem(EDataType.MELEE_WEAPON, 1), itemAt);
			}
			if (p_skill == ESkillID.SKILL_SPEAR)
			{
				AddExtraItem((Equipment)ItemFactory.CreateItem(EDataType.MELEE_WEAPON, 5), itemAt);
			}
			if (p_skill == ESkillID.SKILL_MAGICAL_FOCUS)
			{
				AddExtraItem((Equipment)ItemFactory.CreateItem(EDataType.MAGIC_FOCUS, 1), itemAt);
			}
			itemAt = m_equipment.GetItemAt(EEquipSlots.RANGE_WEAPON);
			if (itemAt is RangedWeapon && (((itemAt as RangedWeapon).GetWeaponType() == EEquipmentType.BOW && p_skill == ESkillID.SKILL_BOW) || ((itemAt as RangedWeapon).GetWeaponType() == EEquipmentType.CROSSBOW && p_skill == ESkillID.SKILL_CROSSBOW)))
			{
				return;
			}
			if (p_skill == ESkillID.SKILL_BOW)
			{
				AddExtraItem((Equipment)ItemFactory.CreateItem(EDataType.RANGED_WEAPON, 1), itemAt);
			}
			if (p_skill == ESkillID.SKILL_CROSSBOW)
			{
				AddExtraItem((Equipment)ItemFactory.CreateItem(EDataType.RANGED_WEAPON, 3), itemAt);
			}
		}

		private void AddExtraItem(Equipment newItem, BaseItem currentItem)
		{
			if (currentItem == null)
			{
				m_equipment.AddItem(newItem);
			}
			else
			{
				LegacyLogic.Instance.WorldManager.Party.Inventory.AddItem(newItem);
			}
		}

		public void AddQuestExp(Int32 p_amount)
		{
			m_activeLogEntryList = m_questRewardLogEntries;
			AddExp(p_amount);
		}

		public void AddExpAndFlushActionLog(Int32 p_amount)
		{
			AddExp(p_amount);
			FlushNormalActionLog();
		}

		public void FeedActionLog(LogEntryEventArgs p_args)
		{
			if (p_args != null)
			{
				m_activeLogEntryList.Add(p_args);
			}
		}

		public void AddExp(Int32 p_amount)
		{
			if (p_amount == 0 || ConditionHandler.CantDoAnything())
			{
				return;
			}
			m_equipment.AddExp(p_amount);
			if (MaxLevelReached)
			{
				return;
			}
			m_exp += p_amount;
			ExpEntryEventArgs item = new ExpEntryEventArgs(this, p_amount);
			m_activeLogEntryList.Add(item);
			Boolean flag = false;
			Int32 p_levelUp = 0;
			Single percentExpForNextLevel = GetPercentExpForNextLevel();
			while (m_exp >= m_expTreshhold && !flag)
			{
				flag = ProcessLevelUp();
				p_levelUp = Level;
			}
			XpGainEventArgs p_eventArgs = new XpGainEventArgs(this, p_amount, percentExpForNextLevel, p_levelUp);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_XP_GAIN, p_eventArgs);
			m_activeLogEntryList = m_logEntries;
		}

		public void ChangeHP(Int32 p_deltaHP)
		{
			ChangeHP(p_deltaHP, null);
		}

		public void ChangeHP(Int32 p_deltaHP, Monster p_attacker)
		{
			if (p_deltaHP == 0)
			{
				return;
			}
			if (p_deltaHP >= 0 && m_conditionHandler.HasCondition(ECondition.DEAD))
			{
				return;
			}
			Int32 num = Math.Abs(p_deltaHP);
			Single num2 = m_HealthPoints / 10f * 11f;
			m_HealthPoints += p_deltaHP;
			if (m_HealthPoints > MaximumHealthPoints)
			{
				m_HealthPoints = MaximumHealthPoints;
			}
			if (m_HealthPoints <= 0)
			{
				if (num >= num2 && num >= MaximumHealthPoints / 10 && !m_conditionHandler.HasCondition(ECondition.UNCONSCIOUS) && !m_conditionHandler.HasCondition(ECondition.DEAD))
				{
					BarkHandler.TriggerBark(EBarks.ANNIHILATION, this);
				}
				m_conditionHandler.AddCondition(ECondition.UNCONSCIOUS);
			}
			else if (m_HealthPoints >= 0 && m_conditionHandler.HasCondition(ECondition.UNCONSCIOUS))
			{
				m_conditionHandler.RemoveCondition(ECondition.UNCONSCIOUS);
			}
			Int32 num3 = -ConfigManager.Instance.Game.DeadBaseValue - ConfigManager.Instance.Game.DeadVitalityMultiplicator * m_currentAttributes.Vitality;
			if (m_HealthPoints <= num3)
			{
				if (!m_conditionHandler.HasCondition(ECondition.DEAD))
				{
					if (num >= num2 && num >= MaximumHealthPoints / 10)
					{
						BarkHandler.TriggerBark(EBarks.ANNIHILATION, this);
					}
					if (p_attacker != null)
					{
						LegacyLogic.Instance.TrackingManager.TrackCharacterKilled(this, p_attacker);
					}
				}
				m_conditionHandler.AddCondition(ECondition.DEAD);
			}
			if (m_conditionHandler.HasCondition(ECondition.SLEEPING) && p_deltaHP < -(Int32)(MaximumHealthPoints * ConfigManager.Instance.Game.SleepingWakeupDamage))
			{
				m_conditionHandler.RemoveCondition(ECondition.SLEEPING);
			}
			StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.HEALTH_POINTS);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_STATUS_CHANGED, p_eventArgs);
		}

		public Single GetDeadHealthPercent()
		{
			Int32 num = -ConfigManager.Instance.Game.DeadBaseValue - ConfigManager.Instance.Game.DeadVitalityMultiplicator * m_currentAttributes.Vitality;
			Single num2 = 1f - m_HealthPoints / (Single)num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			return num2;
		}

		public void GetCharacterActiveLeft()
		{
			Int32 num = 0;
			Character p_character = LegacyLogic.Instance.WorldManager.Party.GetMember(0);
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(i);
				if (member.m_conditionHandler.CantDoAnything())
				{
					num++;
				}
				else
				{
					p_character = member;
				}
			}
			if (num > 2)
			{
				BarkHandler.TriggerBark(EBarks.LAST_ONE, p_character);
			}
		}

		public void ChangeMP(Int32 p_deltaMP)
		{
			if (p_deltaMP == 0)
			{
				return;
			}
			m_ManaPoints += p_deltaMP;
			if (m_ManaPoints < 0)
			{
				m_ManaPoints = 0;
			}
			else if (m_ManaPoints > MaximumManaPoints - MaxManaReducedByDeficiency)
			{
				m_ManaPoints = MaximumManaPoints - MaxManaReducedByDeficiency;
			}
			StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.MANA_POINTS);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_STATUS_CHANGED, p_eventArgs);
		}

		public void ApplyDamages(AttackResult result, Monster p_attacker)
		{
			ChangeHP(-result.DamageDone, p_attacker);
		}

		private Boolean ProcessLevelUp()
		{
			ExpStaticData staticData = StaticDataHandler.GetStaticData<ExpStaticData>(EDataType.EXP_TABLE, m_level + 1);
			BarkHandler.TriggerBark(EBarks.LEVEL_UP, this);
			if (staticData != null)
			{
				m_level++;
				SkillPoints += ConfigManager.Instance.Game.SkillPointsPerLevelUp;
				AttributePoints += ConfigManager.Instance.Game.AttributePointsPerLevelUp;
				CalculateCurrentAttributes();
				if (!m_conditionHandler.HasCondition(ECondition.DEAD | ECondition.UNCONSCIOUS))
				{
					m_HealthPoints = CurrentAttributes.HealthPoints;
					m_ManaPoints = CurrentAttributes.ManaPoints;
				}
				m_expTreshhold += staticData.DeltaExp;
				m_activeLogEntryList.Add(new LevelUpEntryEventArgs(this));
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.LEVELUP);
				LegacyLogic.Instance.TrackingManager.TrackLevelUp(this);
				return false;
			}
			return true;
		}

		public Single GetPercentExpForNextLevel()
		{
			ExpStaticData staticData = StaticDataHandler.GetStaticData<ExpStaticData>(EDataType.EXP_TABLE, m_level);
			if (staticData != null)
			{
				Int32 deltaExp = staticData.DeltaExp;
				Single num = 1f - ExpNeededForNextLevel / (Single)deltaExp;
				return (num <= 1f) ? num : 1f;
			}
			return 1f;
		}

		public void CalculateCurrentAttributes()
		{
			Int32 meleeBlockAttempts = FightValues.MeleeBlockAttempts;
			Int32 generalBlockAttempts = FightValues.GeneralBlockAttempts;
			GameConfig game = ConfigManager.Instance.Game;
			m_currentAttributes = m_baseAttributes;
			m_fightValues.ResetValues(m_class.GetEvadeValue());
			m_skillHandler.ResetVirtualSkillLevels();
			m_fightValues.Resistance.Add(BaseResistance);
			m_fightValues.GeneralBlockChance = 0.5f;
			m_fightValues.RangedAttackRange = m_skillHandler.GetRangedAttackRange();
			m_fightValues.MainHandCriticalHitDestinyMultiplier = ConfigManager.Instance.Game.MainHandCritChanceDestinyMod;
			m_fightValues.OffHandCriticalHitDestinyMultiplier = ConfigManager.Instance.Game.OffHandCritChanceDestinyMod;
			m_fightValues.RangedCriticalHitDestinyMultiplier = ConfigManager.Instance.Game.RangedCriticalHitDestinyMod;
			for (EEquipSlots eequipSlots = EEquipSlots.MAIN_HAND; eequipSlots < EEquipSlots.COUNT; eequipSlots++)
			{
				Equipment equipment = m_equipment.GetItemAt(eequipSlots) as Equipment;
				if (equipment != null)
				{
					if (eequipSlots != EEquipSlots.OFF_HAND || equipment.ItemSlot != EItemSlot.ITEM_SLOT_2_HAND)
					{
						equipment.FillFightValues(eequipSlots == EEquipSlots.OFF_HAND, m_fightValues);
						m_enchantmentHandler.ResolveSkillLevelEffects(equipment);
						m_enchantmentHandler.ResolveAttributeEffects(equipment, ref m_currentAttributes);
						m_enchantmentHandler.ResolveFightValueEffects(eequipSlots, equipment, m_fightValues);
					}
				}
			}
			if (!m_conditionHandler.HasCondition(ECondition.DEAD))
			{
				m_buffs.AddAttributes(ref m_currentAttributes);
				m_buffs.AddFightValues(m_fightValues);
			}
			m_conditionHandler.ModifyAttributes(ref m_currentAttributes);
			if (!m_conditionHandler.HasCondition(ECondition.DEAD))
			{
				m_buffs.ModifyAttributes(ref m_currentAttributes);
			}
			Single hpperVitality = m_class.GetHPPerVitality();
			m_currentAttributes.HealthPoints = m_currentAttributes.HealthPoints + (Int32)(game.HealthPerMight * m_currentAttributes.Might + hpperVitality * m_currentAttributes.Vitality);
			m_currentAttributes.ManaPoints = m_currentAttributes.ManaPoints + (Int32)(game.ManaPerMagic * m_currentAttributes.Magic + game.ManaPerSpirit * m_currentAttributes.Spirit);
			m_skillHandler.AddHealthMana(ref m_currentAttributes);
			m_fightValues.EvadeValue += m_currentAttributes.Destiny;
			m_skillHandler.AddFightValues(m_fightValues);
			m_fightValues.MainHandAttackValue += m_fightHandler.CalculateAttackValue(EEquipSlots.MAIN_HAND);
			m_fightValues.OffHandAttackValue += m_fightHandler.CalculateAttackValue(EEquipSlots.OFF_HAND);
			m_fightValues.RangedAttackValue += game.RangeHandAttackValue + CurrentAttributes.Perception;
			m_fightValues.CriticalMainHandHitChance += m_fightHandler.CalculateCriticalHitChance(EEquipSlots.MAIN_HAND);
			m_fightValues.CriticalOffHandHitChance += m_fightHandler.CalculateCriticalHitChance(EEquipSlots.OFF_HAND);
			m_fightValues.CriticalRangeHitChance += m_fightHandler.CalculateCriticalHitChance(EEquipSlots.RANGE_WEAPON);
			m_fightValues.CriticalMagicHitChance += m_fightHandler.CalculateMagicCriticalHitChance();
			m_fightValues.MagicalCriticalDamageMod += m_fightHandler.CalculateMagicCriticalDamageFactor();
			m_fightHandler.ApplyWeaponDamageMods(m_fightValues.MainHandDamage, EEquipSlots.MAIN_HAND);
			m_fightHandler.ApplyWeaponDamageMods(m_fightValues.OffHandDamage, EEquipSlots.OFF_HAND);
			m_fightHandler.ApplyWeaponDamageMods(m_fightValues.RangeDamage, EEquipSlots.RANGE_WEAPON);
			m_fightHandler.CalculateMagicFactors(m_fightValues);
			m_fightValues.Resistance.Add(LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetResistances());
			if (m_fightValues.GeneralBlockAttempts != generalBlockAttempts)
			{
				m_fightHandler.CurrentGeneralBlockAttempts += m_fightValues.GeneralBlockAttempts - generalBlockAttempts;
				if (m_fightHandler.CurrentGeneralBlockAttempts < 0)
				{
					m_fightHandler.CurrentGeneralBlockAttempts = 0;
				}
			}
			if (m_fightValues.MeleeBlockAttempts != meleeBlockAttempts)
			{
				m_fightHandler.CurrentMeleeBlockAttempts += m_fightValues.MeleeBlockAttempts - meleeBlockAttempts;
				if (m_fightHandler.CurrentMeleeBlockAttempts < 0)
				{
					m_fightHandler.CurrentMeleeBlockAttempts = 0;
				}
			}
			m_conditionHandler.ModifyFightValues();
			if (!m_conditionHandler.HasCondition(ECondition.DEAD))
			{
				m_buffs.ModifyFightValues(m_fightValues);
			}
			if (IsDefendModeActive)
			{
				m_fightValues.GeneralBlockChance += ConfigManager.Instance.Game.DefendBlockBonus;
				m_fightValues.EvadeValue *= 1f + ConfigManager.Instance.Game.DefendEvadeBonus;
			}
			if (m_HealthPoints > MaximumHealthPoints)
			{
				m_HealthPoints = MaximumHealthPoints;
			}
			if (m_ManaPoints > MaximumManaPoints)
			{
				m_ManaPoints = MaximumManaPoints;
			}
			StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STATUS);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_STATUS_CHANGED, p_eventArgs);
		}

		public void Load(SaveGameData p_data)
		{
			m_name = p_data.Get<String>("Name", "NoName");
			m_HealthPoints = p_data.Get<Int32>("HealthPoints", 1);
			m_ManaPoints = p_data.Get<Int32>("ManaPoints", 1);
			m_class = new CharacterClass((EClass)p_data.Get<Int32>("Class", 1));
			m_level = p_data.Get<Int32>("Level", 1);
			m_exp = p_data.Get<Int32>("Experience", 1);
			m_portrait = p_data.Get<String>("Portrait", "None");
			m_portraitID = p_data.Get<Int32>("PortraitID", 1);
			m_gender = (EGender)p_data.Get<Int32>("Gender", 1);
			m_voice = (EVoiceSetting)p_data.Get<Int32>("Voice", 1);
			m_skillPoints = p_data.Get<Int32>("Skillpoints", 1);
			m_attributePoints = p_data.Get<Int32>("AttributePoints", 0);
			m_expTreshhold = p_data.Get<Int32>("XPThreshold", 1);
			m_unlockedAdvancedClass = p_data.Get<Boolean>("UnlockedAdvanced", false);
			m_class.IsAdvanced = m_unlockedAdvancedClass;
			m_conditionHandler = new ConditionHandler(this);
			m_conditionHandler.Load(p_data);
			m_body = m_class.StaticData.BodyBase;
			if (m_gender == EGender.MALE)
			{
				m_body += "_male";
			}
			else
			{
				m_body += "_female";
			}
			SaveGameData saveGameData = p_data.Get<SaveGameData>("BaseAttributes", null);
			if (saveGameData != null)
			{
				m_baseAttributes.Load(saveGameData);
			}
			SaveGameData saveGameData2 = p_data.Get<SaveGameData>("Resistances", null);
			if (saveGameData2 != null)
			{
				m_resistance.Load(saveGameData2);
			}
			SaveGameData saveGameData3 = p_data.Get<SaveGameData>("Equipment", null);
			if (saveGameData3 != null)
			{
				m_equipment.Load(saveGameData3);
			}
			SaveGameData saveGameData4 = p_data.Get<SaveGameData>("LastRestTime", null);
			if (saveGameData4 != null)
			{
				m_lastRestTime.Load(saveGameData4);
			}
			SaveGameData saveGameData5 = p_data.Get<SaveGameData>("Skills", null);
			if (saveGameData5 != null)
			{
				m_skillHandler.Init();
				m_skillHandler.Load(saveGameData5);
			}
			SaveGameData saveGameData6 = p_data.Get<SaveGameData>("Spells", null);
			if (saveGameData6 != null)
			{
				m_spellHandler.Init();
				m_spellHandler.Load(saveGameData6);
			}
			SaveGameData saveGameData7 = p_data.Get<SaveGameData>("QuickActions", null);
			if (saveGameData7 != null)
			{
				m_quickActions.Load(saveGameData7);
			}
			m_enchantmentHandler.Init(this);
			CalculateCurrentAttributes();
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<String>("Name", m_name);
			p_data.Set<Int32>("HealthPoints", m_HealthPoints);
			p_data.Set<Int32>("ManaPoints", m_ManaPoints);
			p_data.Set<Int32>("Class", (Int32)m_class.Class);
			p_data.Set<Int32>("Level", m_level);
			p_data.Set<Int32>("Experience", m_exp);
			p_data.Set<String>("Portrait", m_portrait);
			p_data.Set<Int32>("PortraitID", m_portraitID);
			p_data.Set<EGender>("Gender", m_gender);
			p_data.Set<EVoiceSetting>("Voice", m_voice);
			p_data.Set<Int32>("Skillpoints", m_skillPoints);
			p_data.Set<Int32>("AttributePoints", m_attributePoints);
			p_data.Set<Int32>("XPThreshold", m_expTreshhold);
			p_data.Set<Boolean>("UnlockedAdvanced", m_unlockedAdvancedClass);
			SaveGameData saveGameData = new SaveGameData("BaseAttributes");
			m_baseAttributes.Save(saveGameData);
			p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			SaveGameData saveGameData2 = new SaveGameData("Resistances");
			m_resistance.Save(saveGameData2);
			p_data.Set<SaveGameData>(saveGameData2.ID, saveGameData2);
			SaveGameData saveGameData3 = new SaveGameData("Equipment");
			m_equipment.Save(saveGameData3);
			p_data.Set<SaveGameData>(saveGameData3.ID, saveGameData3);
			SaveGameData saveGameData4 = new SaveGameData("LastRestTime");
			m_lastRestTime.Save(saveGameData4);
			p_data.Set<SaveGameData>(saveGameData4.ID, saveGameData4);
			SaveGameData saveGameData5 = new SaveGameData("Skills");
			m_skillHandler.Save(saveGameData5);
			p_data.Set<SaveGameData>(saveGameData5.ID, saveGameData5);
			SaveGameData saveGameData6 = new SaveGameData("Spells");
			m_spellHandler.Save(saveGameData6);
			p_data.Set<SaveGameData>(saveGameData6.ID, saveGameData6);
			SaveGameData saveGameData7 = new SaveGameData("QuickActions");
			m_quickActions.Save(saveGameData7);
			p_data.Set<SaveGameData>(saveGameData7.ID, saveGameData7);
			m_conditionHandler.Save(p_data);
		}

		public void EndTurn()
		{
			DoneTurn = true;
			SkillHandler.EndTurn();
			m_conditionHandler.FlushActionLog();
			if (!m_fightHandler.HasLogEntries)
			{
				FlushNormalActionLog();
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_DONE_TURN_UPDATE, EventArgs.Empty);
		}

		public void EndPartyTurn()
		{
		}

		public void FlushActionLog()
		{
			foreach (LogEntryEventArgs p_args in m_activeLogEntryList)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_activeLogEntryList.Clear();
		}

		public void FlushNormalActionLog()
		{
			m_activeLogEntryList = m_logEntries;
			FlushActionLog();
		}

		public void FlushRewardsActionLog()
		{
			m_activeLogEntryList = m_questRewardLogEntries;
			FlushActionLog();
			m_activeLogEntryList = m_logEntries;
		}

		private Boolean HasDoneTurn()
		{
			if (m_doneTurn && m_CounterAttackMonster != null && !m_CounterAttackMonster.CounterAttack)
			{
				m_CounterAttackMonster = null;
			}
			return m_doneTurn;
		}

		public Boolean InRangedAttackRange(Int32 p_range)
		{
			return m_fightValues.RangedAttackRange >= p_range;
		}

		public void StartTurn()
		{
			DoneTurn = false;
			m_fightHandler.CurrentMeleeBlockAttempts = m_fightValues.MeleeBlockAttempts;
			m_fightHandler.CurrentGeneralBlockAttempts = m_fightValues.GeneralBlockAttempts;
			m_fightHandler.InterceptingCharacter = null;
			m_conditionHandler.StartTurn();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_DONE_TURN_UPDATE, EventArgs.Empty);
			AddHealthFromBuffs();
		}

		private void AddHealthFromBuffs()
		{
			Int32 num = m_buffs.AddHealthPoints(this);
			if (num > 0 && !m_conditionHandler.HasCondition(ECondition.DEAD) && HealthPoints < MaximumHealthPoints)
			{
				ChangeHP(num);
				CharacterSpell p_spell = SpellFactory.CreateCharacterSpell(ECharacterSpell.SPELL_EARTH_REGENERATION);
				PartyBuff buff = m_buffs.GetBuff(EPartyBuffs.REGENERATION);
				SpellEffectEntryEventArgs item = new SpellEffectEntryEventArgs(buff, new SpellEventArgs(p_spell)
				{
					Result = ESpellResult.OK,
					SpellTargets = 
					{
						new HealedTarget(this, num, false)
					}
				});
				m_logEntries.Add(item);
				DamageEventArgs p_eventArgs = new DamageEventArgs(new AttackResult
				{
					Result = EResultType.HEAL,
					DamageResults = 
					{
						new DamageResult(EDamageType.HEAL, num, 0, 1f)
					}
				});
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_HEALS, p_eventArgs);
			}
		}

		public void UseScroll(Scroll p_scroll)
		{
			CharacterSpell characterSpell = SpellFactory.CreateCharacterSpell((ECharacterSpell)p_scroll.SpellID);
			if (characterSpell != null)
			{
				if (characterSpell.TargetType == ETargetType.SINGLE_PARTY_MEMBER)
				{
					OpenSpellCharacterSelectionEventArgs p_eventArgs = new OpenSpellCharacterSelectionEventArgs(characterSpell, p_scroll);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_CAST_SPELL_WITH_CHARACTER_SELECTION, p_eventArgs);
				}
				else if (characterSpell.SpellType == ECharacterSpell.SPELL_PRIME_SPIRIT_BEACON)
				{
					if (characterSpell.CheckSpellConditions(null))
					{
						SpiritBeaconEventArgs p_eventArgs2 = new SpiritBeaconEventArgs(p_scroll);
						LegacyLogic.Instance.EventManager.InvokeEvent(characterSpell, EEventType.CHARACTER_CAST_SPIRIT_BEACON, p_eventArgs2);
					}
				}
				else if (characterSpell.SpellType == ECharacterSpell.SPELL_PRIME_IDENTIFY)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(p_scroll, EEventType.CHARACTER_CAST_IDENTIFY, EventArgs.Empty);
				}
				else
				{
					CastSpellCommand p_command = new CastSpellCommand(characterSpell, null, p_scroll);
					LegacyLogic.Instance.CommandManager.AddCommand(p_command);
				}
			}
		}

		public void UsePotion(Potion p_potion)
		{
			switch (p_potion.Operation)
			{
			case EPotionOperation.INCREASE_ABS:
				PotionIncreaseAbsolute(p_potion);
				break;
			case EPotionOperation.INCREASE_PROZ:
				PotionIncreaseProcentual(p_potion);
				break;
			case EPotionOperation.INCREASE_ABS_PERM:
				ElixirIncreaseAbsolute(p_potion);
				break;
			case EPotionOperation.REMOVE:
				PotionRemove(p_potion);
				break;
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_POTION_USED, EventArgs.Empty);
		}

		internal void PotionIncreaseAbsolute(Potion p_potion)
		{
			SetAttributeByTarget(p_potion, p_potion.Value);
		}

		internal void PotionIncreaseProcentual(Potion p_potion)
		{
			if (p_potion.Target == EPotionTarget.MANA_AND_HP)
			{
				SetAttributeByTarget(EPotionTarget.MANA, GetAttributeByTarget(EPotionTarget.MANA, false) * p_potion.Value / 100);
				SetAttributeByTarget(EPotionTarget.HP, GetAttributeByTarget(EPotionTarget.HP, false) * p_potion.Value / 100);
			}
			else
			{
				SetAttributeByTarget(p_potion, GetAttributeByTarget(p_potion.Target, false) * p_potion.Value / 100);
			}
		}

		internal void PotionRemove(Potion p_potion)
		{
			if (p_potion.Target == EPotionTarget.CONDITION_CONFUSED)
			{
				ConditionHandler.RemoveCondition(ECondition.CONFUSED);
			}
			if (p_potion.Target == EPotionTarget.CONDITION_WEAK)
			{
				ConditionHandler.RemoveCondition(ECondition.WEAK);
			}
			if (p_potion.Target == EPotionTarget.CONDITION_POISONED)
			{
				ConditionHandler.RemoveCondition(ECondition.POISONED);
			}
		}

		internal void ElixirIncreaseAbsolute(Potion p_potion)
		{
			switch (p_potion.Target)
			{
			case EPotionTarget.MIGHT:
				m_baseAttributes.Might = m_baseAttributes.Might + p_potion.Value;
				break;
			case EPotionTarget.MAGIC:
				m_baseAttributes.Magic = m_baseAttributes.Magic + p_potion.Value;
				break;
			case EPotionTarget.PERCEPTION:
				m_baseAttributes.Perception = m_baseAttributes.Perception + p_potion.Value;
				break;
			case EPotionTarget.DESTINY:
				m_baseAttributes.Destiny = m_baseAttributes.Destiny + p_potion.Value;
				break;
			case EPotionTarget.VITALITY:
				m_baseAttributes.Vitality = m_baseAttributes.Vitality + p_potion.Value;
				break;
			case EPotionTarget.SPIRIT:
				m_baseAttributes.Spirit = m_baseAttributes.Spirit + p_potion.Value;
				break;
			case EPotionTarget.BASE_HP:
				m_baseAttributes.HealthPoints = m_baseAttributes.HealthPoints + p_potion.Value;
				break;
			case EPotionTarget.BASE_MANA:
				m_baseAttributes.ManaPoints = m_baseAttributes.ManaPoints + p_potion.Value;
				break;
			case EPotionTarget.ALL_ATTRIBUTES:
			{
				Int32 value = p_potion.Value;
				Attributes right = new Attributes(value, value, value, value, value, value, value, value);
				m_baseAttributes += right;
				break;
			}
			case EPotionTarget.ALL_RESISTANCES:
				for (Int32 i = 1; i <= 7; i++)
				{
					BaseResistance.Add((EDamageType)i, p_potion.Value);
				}
				break;
			case EPotionTarget.FIRE_RESISTANCE:
				BaseResistance.Add(EDamageType.FIRE, p_potion.Value);
				break;
			case EPotionTarget.WATER_RESISTANCE:
				BaseResistance.Add(EDamageType.WATER, p_potion.Value);
				break;
			case EPotionTarget.AIR_RESISTANCE:
				BaseResistance.Add(EDamageType.AIR, p_potion.Value);
				break;
			case EPotionTarget.EARTH_RESISTANCE:
				BaseResistance.Add(EDamageType.EARTH, p_potion.Value);
				break;
			case EPotionTarget.LIGHT_RESISTANCE:
				BaseResistance.Add(EDamageType.LIGHT, p_potion.Value);
				break;
			case EPotionTarget.DARK_RESISTANCE:
				BaseResistance.Add(EDamageType.DARK, p_potion.Value);
				break;
			}
			CalculateCurrentAttributes();
			BroadcastStatsChangedEvent();
			PotionUsedEntryEventArgs p_args = new PotionUsedEntryEventArgs(this, p_potion, p_potion.Value);
			LegacyLogic.Instance.ActionLog.PushEntry(p_args);
		}

		internal Int32 GetAttributeByTarget(EPotionTarget p_targetAttribute, Boolean current)
		{
			if (p_targetAttribute == EPotionTarget.HP)
			{
				return (!current) ? MaximumHealthPoints : HealthPoints;
			}
			if (p_targetAttribute != EPotionTarget.MANA)
			{
				return 0;
			}
			return (!current) ? MaximumManaPoints : ManaPoints;
		}

		internal void SetAttributeByTarget(EPotionTarget p_targetAttribute, Int32 p_value)
		{
			switch (p_targetAttribute)
			{
			case EPotionTarget.HP:
			{
				ChangeHP(p_value);
				DamageEventArgs p_eventArgs = new DamageEventArgs(new AttackResult
				{
					Result = EResultType.HEAL,
					DamageResults = 
					{
						new DamageResult(EDamageType.HEAL, p_value, 0, 1f)
					}
				});
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_HEALS, p_eventArgs);
				break;
			}
			case EPotionTarget.MANA:
				ChangeMP(p_value);
				break;
			case EPotionTarget.MIGHT:
				m_currentAttributes.Might = m_currentAttributes.Might + p_value;
				break;
			case EPotionTarget.MAGIC:
				m_currentAttributes.Magic = m_currentAttributes.Magic + p_value;
				break;
			case EPotionTarget.PERCEPTION:
				m_currentAttributes.Perception = m_currentAttributes.Perception + p_value;
				break;
			case EPotionTarget.DESTINY:
				m_currentAttributes.Destiny = m_currentAttributes.Destiny + p_value;
				break;
			case EPotionTarget.VITALITY:
				m_currentAttributes.Vitality = m_currentAttributes.Vitality + p_value;
				break;
			case EPotionTarget.SPIRIT:
				m_currentAttributes.Spirit = m_currentAttributes.Spirit + p_value;
				break;
			}
			BroadcastStatsChangedEvent();
		}

		internal void SetAttributeByTarget(Potion p_potion, Int32 p_value)
		{
			switch (p_potion.Target)
			{
			case EPotionTarget.HP:
			{
				ChangeHP(p_value);
				DamageEventArgs p_eventArgs = new DamageEventArgs(new AttackResult
				{
					Result = EResultType.HEAL,
					DamageResults = 
					{
						new DamageResult(EDamageType.HEAL, p_value, 0, 1f)
					}
				});
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_HEALS, p_eventArgs);
				break;
			}
			case EPotionTarget.MANA:
				ChangeMP(p_value);
				break;
			case EPotionTarget.MIGHT:
				m_currentAttributes.Might = m_currentAttributes.Might + p_value;
				break;
			case EPotionTarget.MAGIC:
				m_currentAttributes.Magic = m_currentAttributes.Magic + p_value;
				break;
			case EPotionTarget.PERCEPTION:
				m_currentAttributes.Perception = m_currentAttributes.Perception + p_value;
				break;
			case EPotionTarget.DESTINY:
				m_currentAttributes.Destiny = m_currentAttributes.Destiny + p_value;
				break;
			case EPotionTarget.VITALITY:
				m_currentAttributes.Vitality = m_currentAttributes.Vitality + p_value;
				break;
			case EPotionTarget.SPIRIT:
				m_currentAttributes.Spirit = m_currentAttributes.Spirit + p_value;
				break;
			}
			BroadcastStatsChangedEvent();
			PotionUsedEntryEventArgs p_args = new PotionUsedEntryEventArgs(this, p_potion, p_value);
			LegacyLogic.Instance.ActionLog.PushEntry(p_args);
		}

		internal void BroadcastStatsChangedEvent()
		{
			StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STATUS);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_STATUS_CHANGED, p_eventArgs);
		}

		public void DoRest()
		{
			MaxManaReducedByDeficiency = 0;
			if (!m_conditionHandler.HasCondition(ECondition.UNCONSCIOUS | ECondition.PARALYZED | ECondition.STUNNED | ECondition.POISONED))
			{
				ChangeHP(MaximumHealthPoints);
				ChangeMP(MaximumManaPoints);
			}
			m_lastRestTime = LegacyLogic.Instance.GameTime.Time;
			m_conditionHandler.RemoveCondition(ECondition.WEAK);
			m_conditionHandler.RemoveCondition(ECondition.CONFUSED);
			m_conditionHandler.HUDStarved = 0;
		}

		public void ResetRestTime()
		{
			m_lastRestTime = LegacyLogic.Instance.GameTime.Time;
			m_conditionHandler.HUDStarved = 0;
		}

		public void Destroy()
		{
			m_fightValues = null;
			m_fightHandler = null;
			m_conditionHandler = null;
			m_skillHandler = null;
			m_spellHandler = null;
			m_buffs = null;
			m_enchantmentHandler = null;
			m_quickActions = null;
			m_attributeChanger = null;
		}

		public void Resurrect()
		{
			m_HealthPoints = 1;
		}

		public void Advance()
		{
			m_unlockedAdvancedClass = true;
			m_class.IsAdvanced = true;
			CalculateCurrentAttributes();
			LegacyLogic.Instance.TrackingManager.TrackAdvancedClassPromoted(this);
			if (m_class.Class == EClass.CRUSADER)
			{
				m_spellHandler.AddSpell(ECharacterSpell.SPELL_LIGHT_LAY_ON_HANDS);
				m_spellHandler.AddSpell(ECharacterSpell.SPELL_MANDATE_OF_HEAVEN);
			}
			if (m_class.Class == EClass.FREEMAGE)
			{
				m_spellHandler.AddSpell(ECharacterSpell.SPELL_PRIME_TIME_STOP);
			}
			if (m_class.Class == EClass.DRUID)
			{
				m_spellHandler.AddSpell(ECharacterSpell.SPELL_EARTH_HARMONY);
				m_spellHandler.AddSpell(ECharacterSpell.SPELL_EARTH_NURTURE);
			}
			if (m_class.Class == EClass.RUNEPRIEST)
			{
				m_spellHandler.AddSpell(ECharacterSpell.SPELL_FIRE_SEARING_RUNE);
			}
			if (m_class.Class == EClass.RANGER)
			{
				m_spellHandler.AddSpell(ECharacterSpell.WARFARE_POINT_BLANK_SHOT);
			}
			if (m_class.Class == EClass.SCOUT)
			{
				m_spellHandler.AddSpell(ECharacterSpell.WARFARE_SNARING_SHOT);
			}
			if (m_class.Class == EClass.HUNTER)
			{
				m_spellHandler.AddSpell(ECharacterSpell.WARFARE_CRIPPLING_TRAP);
				m_spellHandler.AddSpell(ECharacterSpell.WARFARE_SNATCH);
			}
			if (m_class.Class == EClass.BLADEDANCER)
			{
				m_spellHandler.AddSpell(ECharacterSpell.WARFARE_CARNAGE);
			}
			if (m_class.Class == EClass.MERCENARY)
			{
				m_spellHandler.AddSpell(ECharacterSpell.WARFARE_DASH);
			}
		}

		public void TryUnlockAdvancedClass(ETokenID p_id)
		{
			if (m_class.CanUnlock(p_id))
			{
				Advance();
			}
		}

		public void Respec()
		{
			CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)m_class.Class);
			m_baseAttributes.Might = staticData.BaseMight;
			m_baseAttributes.Magic = staticData.BaseMagic;
			m_baseAttributes.Perception = staticData.BasePerception;
			m_baseAttributes.Destiny = staticData.BaseDestiny;
			m_baseAttributes.Vitality = staticData.BaseVitality;
			m_baseAttributes.Spirit = staticData.BaseSpirit;
			AttributePoints = 5 + ConfigManager.Instance.Game.AttributePointsPerLevelUp * (m_level - 1);
			m_skillHandler.Respec();
			if (m_class.Race == ERace.HUMAN)
			{
				SkillPoints = 4 + ConfigManager.Instance.Game.SkillPointsPerLevelUp * (m_level - 1);
			}
			else
			{
				SkillPoints = 2 + ConfigManager.Instance.Game.SkillPointsPerLevelUp * (m_level - 1);
			}
			CalculateCurrentAttributes();
		}

		private struct CastSpellTask
		{
			public CharacterSpell Spell;

			public SpellEventArgs SpellArgs;
		}
	}
}
