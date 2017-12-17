using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.EventManagement;
using Legacy.Core.MapLoading;
using Legacy.Core.Mods;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.ServiceWrapper;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;
using Legacy.Core.Tracking;
using Legacy.Core.UpdateLogic;

namespace Legacy.Core.Api
{
	public class LegacyLogic
	{
		public const String GAME_VERSION = "1.5-16336";

		private EventManager m_eventManager;

		private UpdateManager m_updateManager;

		private MapLoader m_mapLoader;

		private CommandManager m_commandManager;

		private GameTime m_gameTime;

		private WorldManager m_worldManager;

		private ActionLog m_actionLog;

		private ConversationManager m_conversationManager;

		private IServiceWrapperStrategy m_serviceWrapper;

		private CharacterBarkHandler m_characterBarkHandler;

		private TrackingManager m_trackingManager;

		private ModController m_modController;

		private static LegacyLogic s_instance;

		internal LegacyLogic()
		{
			m_eventManager = new EventManager();
			m_mapLoader = new MapLoader();
			m_gameTime = new GameTime();
			m_gameTime.Init(0, 9, 0);
			m_actionLog = new ActionLog();
			m_conversationManager = new ConversationManager();
			m_characterBarkHandler = new CharacterBarkHandler();
			m_commandManager = new CommandManager();
			m_worldManager = new WorldManager(m_eventManager);
			m_serviceWrapper = new DefaultServiceWrapperStrategy();
			m_trackingManager = new TrackingManager();
			m_modController = new ModController();
			m_updateManager = new UpdateManager();
		}

		public static LegacyLogic Instance
		{
			get
			{
				LegacyLogic result;
				if ((result = s_instance) == null)
				{
					result = (s_instance = new LegacyLogic());
				}
				return result;
			}
		}

		public UpdateManager UpdateManager => m_updateManager;

	    public EventManager EventManager => m_eventManager;

	    public MapLoader MapLoader => m_mapLoader;

	    public CommandManager CommandManager => m_commandManager;

	    public IServiceWrapperStrategy ServiceWrapper
		{
			get => m_serviceWrapper;
	        internal set => m_serviceWrapper = value;
	    }

		public TrackingManager TrackingManager => m_trackingManager;

	    public WorldManager WorldManager => m_worldManager;

	    public ActionLog ActionLog => m_actionLog;

	    public CharacterBarkHandler CharacterBarkHandler => m_characterBarkHandler;

	    public GameTime GameTime => m_gameTime;

	    public ConversationManager ConversationManager => m_conversationManager;

	    public ModController ModController => m_modController;

	    internal void StartGame()
		{
			if (WorldManager.IsShowingEndingSequences || WorldManager.IsShowingDLCEndingSequences)
			{
				MapLoader.LoadEndMap();
				WorldManager.IsShowingEndingSequences = false;
				WorldManager.IsShowingDLCEndingSequences = false;
			}
			else if (!WorldManager.IsSaveGame)
			{
				MapLoader.LoadStartMap();
			}
			else
			{
				WorldManager.Load(WorldManager.SaveGameName);
			}
		}

		internal void LoadStaticData(String p_staticDataPath)
		{
            StaticDataHandler.LoadData<RacialSkillsStaticData>(EDataType.RACIAL_SKILLS, p_staticDataPath + "/RacialSkills.csv");
            StaticDataHandler.LoadData<RacialAbilitiesStaticData>(EDataType.RACIAL_ABILITIES, p_staticDataPath + "/RacialAbilities.csv");
			StaticDataHandler.LoadData<ParagonAbilitiesStaticData>(EDataType.PARAGON_ABILITES, p_staticDataPath + "/ParagonAbilities.csv");
			StaticDataHandler.LoadData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, p_staticDataPath + "/CharacterClassStaticData.csv");
			StaticDataHandler.LoadData<MonsterStaticData>(EDataType.MONSTER, p_staticDataPath + "/MonsterStaticData.csv");
			StaticDataHandler.LoadData<InteractiveObjectStaticData>(EDataType.INTERACTIVE_OBJECT, p_staticDataPath + "/InteractiveObjectsStaticData.csv");
			StaticDataHandler.LoadData<InteractiveObjectTooltipStaticData>(EDataType.INTERACTIVE_OBJECT_TOOLTIPS, p_staticDataPath + "/InteractiveObjectTooltips.csv");
			StaticDataHandler.LoadData<ExpStaticData>(EDataType.EXP_TABLE, p_staticDataPath + "/ExpTableStaticData.csv");
			StaticDataHandler.LoadData<BarksCharacterStaticData>(EDataType.BARKS_CHARACTER, p_staticDataPath + "/BarksCharacterStaticData.csv");
			StaticDataHandler.LoadData<BarksPartyStaticData>(EDataType.BARKS_PARTY, p_staticDataPath + "/BarksPartyStaticData.csv");
			StaticDataHandler.LoadData<ChestStaticData>(EDataType.CHEST, p_staticDataPath + "/ChestStaticData.csv");
			StaticDataHandler.LoadData<DungeonEntryStaticData>(EDataType.DUNGEON_ENTRY, p_staticDataPath + "/DungeonEntryStaticData.csv");
			StaticDataHandler.LoadData<ArmorStaticData>(EDataType.ARMOR, p_staticDataPath + "/Armor.csv");
			StaticDataHandler.LoadData<JewelryStaticData>(EDataType.JEWELRY, p_staticDataPath + "/Jewelry.csv");
			StaticDataHandler.LoadData<ShieldStaticData>(EDataType.SHIELD, p_staticDataPath + "/Shields.csv");
			StaticDataHandler.LoadData<MeleeWeaponStaticData>(EDataType.MELEE_WEAPON, p_staticDataPath + "/MeleeWeapons.csv");
			StaticDataHandler.LoadData<MagicFocusStaticData>(EDataType.MAGIC_FOCUS, p_staticDataPath + "/MagicFocus.csv");
			StaticDataHandler.LoadData<RangedWeaponStaticData>(EDataType.RANGED_WEAPON, p_staticDataPath + "/RangedWeapons.csv");
			StaticDataHandler.LoadData<PotionStaticData>(EDataType.POTION, p_staticDataPath + "/Potions.csv");
			StaticDataHandler.LoadData<ScrollStaticData>(EDataType.SCROLL, p_staticDataPath + "/Scrolls.csv");
			StaticDataHandler.LoadData<PrefixStaticData>(EDataType.PREFIX, p_staticDataPath + "/Prefix.csv");
			StaticDataHandler.LoadData<SuffixStaticData>(EDataType.SUFFIX, p_staticDataPath + "/Suffix.csv");
			StaticDataHandler.LoadData<ArmorStaticData>(EDataType.ARMOR_MODEL, p_staticDataPath + "/ArmorModels.csv");
			StaticDataHandler.LoadData<JewelryStaticData>(EDataType.JEWELRY_MODEL, p_staticDataPath + "/JewelryModels.csv");
			StaticDataHandler.LoadData<ShieldStaticData>(EDataType.SHIELD_MODEL, p_staticDataPath + "/ShieldModels.csv");
			StaticDataHandler.LoadData<MeleeWeaponStaticData>(EDataType.MELEE_WEAPON_MODEL, p_staticDataPath + "/MeleeWeaponModels.csv");
			StaticDataHandler.LoadData<MagicFocusStaticData>(EDataType.MAGIC_FOCUS_MODEL, p_staticDataPath + "/MagicFocusModels.csv");
			StaticDataHandler.LoadData<RangedWeaponStaticData>(EDataType.RANGED_WEAPON_MODEL, p_staticDataPath + "/RangedWeaponsModels.csv");
			StaticDataHandler.LoadData<ItemProbabilityStaticData>(EDataType.ITEM_PROBABILITIES, p_staticDataPath + "/ItemProbabilities.csv");
			StaticDataHandler.LoadData<SkillStaticData>(EDataType.SKILL, p_staticDataPath + "/Skills.csv");
			StaticDataHandler.LoadData<SkillEffectStaticData>(EDataType.SKILL_EFFECT, p_staticDataPath + "/SkillEffects.csv");
			StaticDataHandler.LoadData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, p_staticDataPath + "/MonsterBuffStaticData.csv");
			StaticDataHandler.LoadData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, p_staticDataPath + "/PartyBuffs.csv");
			StaticDataHandler.LoadData<MonsterAbilityStaticData>(EDataType.MONSTER_ABILITIES, p_staticDataPath + "/MonsterAbilitiesStaticData.csv");
			StaticDataHandler.LoadData<MonsterSpellStaticData>(EDataType.MONSTER_SPELLS, p_staticDataPath + "/MonsterSpellsStaticData.csv");
			StaticDataHandler.LoadData<CharacterSpellStaticData>(EDataType.CHARACTER_SPELLS, p_staticDataPath + "/CharacterSpellsStaticData.csv");
			StaticDataHandler.LoadData<NpcStaticData>(EDataType.NPC, p_staticDataPath + "/NpcStaticData.csv");
			StaticDataHandler.LoadData<ItemOfferStaticData>(EDataType.ITEM_OFFERS, p_staticDataPath + "/ItemOffers.csv");
			StaticDataHandler.LoadData<SpellOfferStaticData>(EDataType.SPELL_OFFERS, p_staticDataPath + "/SpellOffers.csv");
			StaticDataHandler.LoadData<GeneratedEquipmentStaticData>(EDataType.GENERATED_EQUIPMENT, p_staticDataPath + "/GeneratedEquipment.csv");
			StaticDataHandler.LoadData<GeneratedConsumableStaticData>(EDataType.GENERATED_CONSUMABLES, p_staticDataPath + "/GeneratedConsumables.csv");
			StaticDataHandler.LoadData<QuestStepStaticData>(EDataType.QUEST_STEPS, p_staticDataPath + "/QuestSteps.csv");
			StaticDataHandler.LoadData<QuestObjectiveStaticData>(EDataType.QUEST_OBJECTIVES, p_staticDataPath + "/QuestObjectives.csv");
			StaticDataHandler.LoadData<TokenStaticData>(EDataType.TOKEN, p_staticDataPath + "/Token.csv");
			StaticDataHandler.LoadData<TrapEffectStaticData>(EDataType.TRAP_EFFECT, p_staticDataPath + "/TrapEffects.csv");
			StaticDataHandler.LoadData<WorldMapPointStaticData>(EDataType.WORLD_MAP, p_staticDataPath + "/WorldMapPointsStaticData.csv");
			StaticDataHandler.LoadData<SummonStaticData>(EDataType.SUMMONS, p_staticDataPath + "/SummonStaticData.csv");
			StaticDataHandler.LoadData<LoreBookStaticData>(EDataType.LOREBOOK, p_staticDataPath + "/LoreBookStaticData.csv");
			StaticDataHandler.LoadData<AchievementStaticData>(EDataType.ACHIEVEMENT, p_staticDataPath + "/AchievementsStaticData.csv");
			StaticDataHandler.LoadData<HintStaticData>(EDataType.HINTS, p_staticDataPath + "/Hints.csv");
			StaticDataHandler.LoadData<BarrelStaticData>(EDataType.BARRELS, p_staticDataPath + "/Barrels.csv");
			StaticDataHandler.LoadData<RechargingObjectStaticData>(EDataType.RECHARGING_OBJECTS, p_staticDataPath + "/RechargingObjects.csv");
			StaticDataHandler.LoadData<UnlockableContentStaticData>(EDataType.UNLOCKABLE_CONTENT, p_staticDataPath + "/UnlockableContent.csv");
			StaticDataHandler.LoadData<EndingSlidesStaticData>(EDataType.ENDING_SLIDES, p_staticDataPath + "/EndingSlides.csv");
			StaticDataHandler.LoadData<ShrineStaticData>(EDataType.SHRINES, p_staticDataPath + "/Shrines.csv");
			StaticDataHandler.LoadData<ChallengesStaticData>(EDataType.CHALLENGES, p_staticDataPath + "/TestsChallengesStaticData.csv");
			StaticDataHandler.LoadData<ObelisksStaticData>(EDataType.OBELISKS, p_staticDataPath + "/Obelisks.csv");
			m_worldManager.QuestHandler.Initialize();
			m_worldManager.QuestHandler.LoadDefaultQuestSteps();
			m_worldManager.AchievementManager.Initialize();
			m_worldManager.BestiaryHandler.Initialize();
			m_worldManager.HintManager.Initialize();
			m_worldManager.WorldMapController.Cleanup();
			m_characterBarkHandler.InitializeData();
		}

		internal void SetConversationPath(String p_dialogPath)
		{
			XmlStaticDataHandler<NpcConversationStaticData>.RootPath = p_dialogPath;
		}

		internal void Update()
		{
			m_mapLoader.Update();
			if (!m_mapLoader.IsLoading && m_mapLoader.Grid != null)
			{
				UpdateManager.Update();
			}
		}

		internal void Reset()
		{
			m_conversationManager.Reset();
			m_gameTime.Init(0, 9, 0);
		}

		internal static void NUNIT_Destroy()
		{
			s_instance = new LegacyLogic();
		}
	}
}
