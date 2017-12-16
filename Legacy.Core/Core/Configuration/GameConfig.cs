using System;
using System.IO;
using Legacy.Core.Utilities.Configuration;

namespace Legacy.Core.Configuration
{
	[Serializable]
	public class GameConfig : IConfigDataContainer
	{
		public const Single CameraVerticalOblique = -0.15f;

		public const Single VolumeScaleSFX = 0.55f;

		public const Single VolumeScaleMusic = 0.55f;

		public const Single VolumeScalePartyBarks = 1f;

		public const Single VolumeScaleVoiceOver2d = 1f;

		public const Int32 MaxSteps = 10;

		public Boolean ChineseVersion;

		public String MapStart;

		public String MapAfterEndingSequences;

		public Int32 SpawnerAfterEndingSequences;

		public Int32 MonsterSpawnRange = 10;

		public Int32 MonsterVisibilityRange = 7;

		public Int32 MonsterVisibilityRangeWithDangerSense = 9;

		public Int32 DawnStartHours = 8;

		public Int32 DayStartHours = 10;

		public Int32 DuskStartHours = 18;

		public Int32 NightStartHours = 20;

		public Int32 MinutesPerTurnOutdoor = 5;

		public Int32 MinutesPerTurnCity = 60;

		public Int32 MinutesPerTurnDungeon = 2;

		public Int32 MinutesPerRest = 480;

		public Int32 MaxLevel = 25;

		public Int32 InventorySize = 50;

		public Single RewardXpMultiplier = 1.05f;

		public Int32 ActionLogMaxEntries = 50;

		public Int32 StartSupplies = 6;

		public Int32 ExploreRange = 3;

		public Single BrokenItemMalusNormal = 0.9f;

		public Single BrokenItemMalusHard = 0.25f;

		public Int32 ItemPriceBrokenOrUnidentified = 5;

		public Int32 ResistancePerBlessing = 5;

		public Single ItemResellMultiplicator = 0.25f;

		public Single ScrollNoviceMagicFactor = 2f;

		public Single ScrollExpertMagicFactor = 4f;

		public Single ScrollMasterMagicFactor = 6f;

		public Single ScrollGrandmasterMagicFactor = 8f;

		public Single RangedAttackMeleeMalus = 0.5f;

	    public Int32 GenerateCompatibleEquipmentRetriesNumber = 0;

        public Int32 HoursDeficiencySyndromesRest = 30;

		public Int32 MinutesDeficiencySyndromesTick = 5;

		public Single PoisonEvadeDecrease = 20f;

		public Single PoisonHealthDamage = 0.1f;

		public Single SleepingWakeupDamage = 0.1f;

		public Single CursedAttribDecrease = 0.7f;

		public Single CursedAttackDecrease = 0.3f;

		public Single WeakAttribDecrease = 0.7f;

		public Single KnockedOutTurnCount = 5f;

		public Single CantDoAnythingTurnCount = 5f;

		public Int32 DeadBaseValue = 30;

		public Int32 DeadVitalityMultiplicator = 3;

		public Int32 SkillPointsPerLevelUp = 3;

		public Int32 AttributePointsPerLevelUp = 4;

		public Int32 RequiredSkillLevelNovice = 1;

		public Int32 RequiredSkillLevelExpert = 7;

		public Int32 RequiredSkillLevelMaster = 15;

		public Int32 RequiredSkillLevelGrandMaster = 25;

		public Int32 SkillExpertPrice = 150;

		public Int32 SkillMasterPrice = 500;

		public Int32 SkillGrandmasterPrice = 1000;

		public Int32[] PartyMembers = new Int32[]
		{
			7,
			10,
			5,
			3
		};

		public Single HealthPerMight = 1f;

		public Single HealthPerVitality = 7f;

		public Single ManaPerMagic = 1f;

		public Single ManaPerSpirit = 3f;

		public Single MainHandAttackValue = 50f;

		public Single OffHandAttackValue = 15f;

		public Single RangeHandAttackValue = 50f;

		public Single MainHandDamage = 0.02f;

		public Single RangedDamage = 0.02f;

		public Single MagicDamage = 0.02f;

		public Single OffHandDamage = 0.004f;

		public Single MainHandCritChanceDestinyMod = 0.25f;

		public Single OffHandCritChanceDestinyMod = 0.05f;

		public Single RangedCriticalHitDestinyMod = 0.25f;

		public Int32[] AttackValuePenaltiesLightArmor = new Int32[]
		{
			5,
			10,
			15,
			20,
			25
		};

		public Int32[] AttackValuePenaltiesHeavyArmor = new Int32[]
		{
			12,
			24,
			36,
			48,
			60
		};

		public Single DefendEvadeBonus = 0.5f;

		public Single DefendBlockBonus = 0.25f;

		public Single MagicCritChance = 0.25f;

		public Single MagicCritFactor = 0.2f;

		public Int32 MonsterCastChance = 80;

		public Single EvadeFactor = 0.85f;

		public Single MagicEvadeFactor = 0.5f;

		public Single HirelingGoldShareFactor = 1.1f;

		public Single ItemSpellsFactor = 1.1f;

		public Single ItemConsumablesFactor = 2f;

		public Single ItemEquipmentFactor = 1.1f;

		public Single NpcSuppliesFactor = 2f;

		public Single NpcItemRepairFactor = 1.5f;

		public Single NpcItemIdentifyFactor = 1.5f;

		public Single NpcRestoreFactor = 2f;

		public Single NpcResurrectFactor = 2f;

		public Single NpcCureFactor = 2f;

		public Single NpcRestFactor = 2f;

		public Single NpcSkillTrainingFactor = 1.1f;

		public Single NpcClassPromotionFactor = 1.1f;

		public Single NpcTravelFactor = 1.1f;

		public Single NpcHirelingCostsFactor = 2f;

		public Single MonsterHealthCoreFactor = 1.5f;

		public Single MonsterHealthEliteFactor = 1f;

		public Single MonsterHealthChampionFactor = 1f;

		public Single ItemResellMultiplicatorHard = 0.1f;

		public Single DamageReceiveFactor = 1.2f;

		public Single NpcRespecFactor = 2f;

		public Int32 TradingInventorySize = 48;

		public Int32 CostCast = 10;

		public Int32 CostCure = 10;

		public Int32 CostIdentify = 10;

		public Int32 CostRepair = 10;

		public Int32 CostRest = 10;

		public Int32 CostRestoration = 10;

		public Int32 CostResurrect = 10;

		public Int32 CostTravel = 100;

		public Int32 CostSupplies = 60;

		public Int32 CostRespec = 1000;

		public Single HirelingBlockChance = 0.3f;

		public Boolean EnableRendezVousLogging;

	    public void Load(FileStream p_stream)
	    {
	        ConfigReader configReader = new ConfigReader();
	        configReader.ReadData(p_stream);

	        var map = configReader["map"];
	        MapStart = map["start"].GetString();
	        MapAfterEndingSequences = map["afterEndingSequences"].GetString();
	        SpawnerAfterEndingSequences = map["spawnerEndingSequences"].GetInt();

	        var grid = configReader["grid"];
	        MonsterSpawnRange = grid["monsterSpawnRange"].GetInt();
	        MonsterVisibilityRange = grid["monsterVisibilityRange"].GetInt();
	        MonsterVisibilityRangeWithDangerSense = grid["monsterVisibilityRangeWithDangerSense"].GetInt();

	        var gameTime = configReader["gametime"];
	        DawnStartHours = gameTime["dawnStartHours"].GetInt();
	        DayStartHours = gameTime["dayStartHours"].GetInt();
	        NightStartHours = gameTime["nightStartHours"].GetInt();
	        DuskStartHours = gameTime["duskStartHours"].GetInt();
	        MinutesPerTurnOutdoor = gameTime["minutesPerTurnOutdoor"].GetInt();
	        MinutesPerTurnCity = gameTime["minutesPerTurnCity"].GetInt();
	        MinutesPerTurnDungeon = gameTime["minutesPerTurnDungeon"].GetInt();
	        MinutesPerRest = gameTime["minutesPerRest"].GetInt();

	        var gamePlay = configReader["gameplay"];
	        MaxLevel = gamePlay["maxLevel"].GetInt();
	        InventorySize = gamePlay["inventorySize"].GetInt();
	        RewardXpMultiplier = gamePlay["rewardXpMultiplier"].GetFloat();
	        ActionLogMaxEntries = gamePlay["actionLogMaxEntries"].GetInt();
	        StartSupplies = gamePlay["startSupplies"].GetInt();
	        ExploreRange = gamePlay["exploreRange"].GetInt();
	        BrokenItemMalusNormal = gamePlay["brokenItemMalusNormal"].GetFloat();
	        BrokenItemMalusHard = gamePlay["brokenItemMalusHard"].GetFloat();
	        ItemPriceBrokenOrUnidentified = gamePlay["itemPriceBrokenOrUnidentified"].GetInt();
	        ResistancePerBlessing = gamePlay["resistancePerBlessing"].GetInt();
	        ItemResellMultiplicator = gamePlay["itemResellMultiplicator"].GetFloat();
	        ScrollNoviceMagicFactor = gamePlay["scrollNoviceMagicFactor"].GetFloat();
	        ScrollExpertMagicFactor = gamePlay["scrollExpertMagicFactor"].GetFloat();
	        ScrollMasterMagicFactor = gamePlay["scrollMasterMagicFactor"].GetFloat();
	        ScrollGrandmasterMagicFactor = gamePlay["scrollGrandmasterMagicFactor"].GetFloat();
	        RangedAttackMeleeMalus = gamePlay["rangedAttackMeleeMalus"].GetFloat();
	        GenerateCompatibleEquipmentRetriesNumber = gamePlay[nameof(GenerateCompatibleEquipmentRetriesNumber)].GetInt(GenerateCompatibleEquipmentRetriesNumber);

	        var conditions = configReader["conditions"];
	        HoursDeficiencySyndromesRest = conditions["hoursDeficiencySyndromesRest"].GetInt();
	        MinutesDeficiencySyndromesTick = conditions["minutesDeficiencySyndromesTick"].GetInt();
	        PoisonEvadeDecrease = conditions["poisonEvadeDecrease"].GetFloat();
	        PoisonHealthDamage = conditions["poisonHealthDamage"].GetFloat();
	        SleepingWakeupDamage = conditions["sleepingWakeupDamage"].GetFloat();
	        CursedAttribDecrease = conditions["cursedAttribDecrease"].GetFloat();
	        CursedAttackDecrease = conditions["cursedAttackDecrease"].GetFloat();
	        WeakAttribDecrease = conditions["weakAttribDecrease"].GetFloat();
	        KnockedOutTurnCount = conditions["knockedOutTurnCount"].GetInt();
	        CantDoAnythingTurnCount = conditions["cantDoAnythingTurnCount"].GetInt();
	        DeadBaseValue = conditions["deadBaseValue"].GetInt();
	        DeadVitalityMultiplicator = conditions["deadVitalityMultiplicator"].GetInt();

	        var skills = configReader["skills"];
	        SkillPointsPerLevelUp = skills["skillPointsPerLevelUp"].GetInt();
	        AttributePointsPerLevelUp = skills["attributePointsPerLevelUp"].GetInt();
	        RequiredSkillLevelNovice = skills["requiredSkillLevelNovice"].GetInt();
	        RequiredSkillLevelExpert = skills["requiredSkillLevelExpert"].GetInt();
	        RequiredSkillLevelMaster = skills["requiredSkillLevelMaster"].GetInt();
	        RequiredSkillLevelGrandMaster = skills["requiredSkillLevelGrandMaster"].GetInt();
	        SkillExpertPrice = skills["skillExpertPrice"].GetInt();
	        SkillMasterPrice = skills["skillMasterPrice"].GetInt();
	        SkillGrandmasterPrice = skills["skillGrandmasterPrice"].GetInt();

	        var party = configReader["party"];
	        PartyMembers = party["members"].GetIntArray();
	        HealthPerMight = party["healthPerMight"].GetFloat();
	        HealthPerVitality = party["healthPerVitality"].GetFloat();
	        ManaPerMagic = party["manaPerMagic"].GetFloat();
	        ManaPerSpirit = party["manaPerSpirit"].GetFloat();

	        var combat = configReader["combat"];
	        MainHandAttackValue = combat["mainHandAttackValue"].GetFloat();
	        OffHandAttackValue = combat["offHandAttackValue"].GetFloat();
	        RangeHandAttackValue = combat["rangeAttackValue"].GetFloat();
	        MainHandDamage = combat["mainHandDamage"].GetFloat();
	        RangedDamage = combat["rangedDamage"].GetFloat();
	        MagicDamage = combat["magicDamage"].GetFloat();
	        OffHandDamage = combat["offHandDamage"].GetFloat();
	        MainHandCritChanceDestinyMod = combat["mainHandCritChanceDestinyMod"].GetFloat();
	        OffHandCritChanceDestinyMod = combat["offHandCritChanceDestinyMod"].GetFloat();
	        RangedCriticalHitDestinyMod = combat["rangedCritChanceDestinyMod"].GetFloat();
	        AttackValuePenaltiesLightArmor = combat["attackValuePenaltiesLightArmor"].GetIntArray();
	        AttackValuePenaltiesHeavyArmor = combat["attackValuePenaltiesHeavyArmor"].GetIntArray();
	        DefendEvadeBonus = combat["defendEvadeBonus"].GetFloat();
	        DefendBlockBonus = combat["defendBlockBonus"].GetFloat();
	        MagicCritChance = combat["magicCritChance"].GetFloat();
	        MagicCritFactor = combat["magicCritFactor"].GetFloat();
	        MonsterCastChance = combat["monsterCastChance"].GetInt();
	        EvadeFactor = combat["evadeFactor"].GetFloat();
	        MagicEvadeFactor = combat["magicEvadeFactor"].GetFloat();

	        var npc = configReader["npc"];
	        TradingInventorySize = npc["tradingInventorySize"].GetInt();
	        CostCast = npc["costCast"].GetInt();
	        CostCure = npc["costCure"].GetInt();
	        CostIdentify = npc["costIdentify"].GetInt();
	        CostRepair = npc["costRepair"].GetInt();
	        CostRest = npc["costRest"].GetInt();
	        CostRestoration = npc["costRestoration"].GetInt();
	        CostResurrect = npc["costResurrect"].GetInt();
	        CostTravel = npc["costTravel"].GetInt();
	        CostSupplies = npc["costSupplies"].GetInt();
	        CostRespec = npc["costRespec"].GetInt();
	        HirelingBlockChance = npc["hirelingBlockChance"].GetFloat();

	        var pricing = configReader["pricing"];
	        HirelingGoldShareFactor = pricing["hirelingGoldShareFactor"].GetFloat();

	        var warriorMode = configReader["warriorMode"];
	        ItemSpellsFactor = warriorMode["itemSpellsFactor"].GetFloat();
	        ItemConsumablesFactor = warriorMode["itemConsumablesFactor"].GetFloat();
	        ItemEquipmentFactor = warriorMode["itemEquipmentFactor"].GetFloat();
	        NpcSuppliesFactor = warriorMode["npcSuppliesFactor"].GetFloat();
	        NpcItemRepairFactor = warriorMode["npcItemRepairFactor"].GetFloat();
	        NpcItemIdentifyFactor = warriorMode["npcItemIdentifyFactor"].GetFloat();
	        NpcRestoreFactor = warriorMode["npcRestoreFactor"].GetFloat();
	        NpcResurrectFactor = warriorMode["npcResurrectFactor"].GetFloat();
	        NpcCureFactor = warriorMode["npcCureFactor"].GetFloat();
	        NpcRestFactor = warriorMode["npcRestFactor"].GetFloat();
	        NpcSkillTrainingFactor = warriorMode["npcSkillTrainingFactor"].GetFloat();
	        NpcClassPromotionFactor = warriorMode["npcClassPromotionFactor"].GetFloat();
	        NpcTravelFactor = warriorMode["npcTravelFactor"].GetFloat();
	        NpcHirelingCostsFactor = warriorMode["npcHirelingCostsFactor"].GetFloat();
	        NpcRespecFactor = warriorMode["npcRespecFactor"].GetFloat();
	        MonsterHealthCoreFactor = warriorMode["monsterHealthCoreFactor"].GetFloat();
	        MonsterHealthEliteFactor = warriorMode["monsterHealthEliteFactor"].GetFloat();
	        MonsterHealthChampionFactor = warriorMode["monsterHealthChampionFactor"].GetFloat();
	        ItemResellMultiplicatorHard = warriorMode["itemResellMultiplicatorHard"].GetFloat();
	        DamageReceiveFactor = warriorMode["damageReceiveFactor"].GetFloat();

	        var online = configReader["online"];
	        EnableRendezVousLogging = online["enableRendezVousLogging"].GetBool();
	    }

	    public void Write(FileStream p_stream)
		{
		}
	}
}
