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
			MapStart = configReader["map"]["start"].GetString();
			MapAfterEndingSequences = configReader["map"]["afterEndingSequences"].GetString();
			SpawnerAfterEndingSequences = configReader["map"]["spawnerEndingSequences"].GetInt();
			MonsterSpawnRange = configReader["grid"]["monsterSpawnRange"].GetInt();
			MonsterVisibilityRange = configReader["grid"]["monsterVisibilityRange"].GetInt();
			MonsterVisibilityRangeWithDangerSense = configReader["grid"]["monsterVisibilityRangeWithDangerSense"].GetInt();
			DawnStartHours = configReader["gametime"]["dawnStartHours"].GetInt();
			DayStartHours = configReader["gametime"]["dayStartHours"].GetInt();
			NightStartHours = configReader["gametime"]["nightStartHours"].GetInt();
			DuskStartHours = configReader["gametime"]["duskStartHours"].GetInt();
			MinutesPerTurnOutdoor = configReader["gametime"]["minutesPerTurnOutdoor"].GetInt();
			MinutesPerTurnCity = configReader["gametime"]["minutesPerTurnCity"].GetInt();
			MinutesPerTurnDungeon = configReader["gametime"]["minutesPerTurnDungeon"].GetInt();
			MinutesPerRest = configReader["gametime"]["minutesPerRest"].GetInt();
			MaxLevel = configReader["gameplay"]["maxLevel"].GetInt();
			InventorySize = configReader["gameplay"]["inventorySize"].GetInt();
			RewardXpMultiplier = configReader["gameplay"]["rewardXpMultiplier"].GetFloat();
			ActionLogMaxEntries = configReader["gameplay"]["actionLogMaxEntries"].GetInt();
			StartSupplies = configReader["gameplay"]["startSupplies"].GetInt();
			ExploreRange = configReader["gameplay"]["exploreRange"].GetInt();
			BrokenItemMalusNormal = configReader["gameplay"]["brokenItemMalusNormal"].GetFloat();
			BrokenItemMalusHard = configReader["gameplay"]["brokenItemMalusHard"].GetFloat();
			ItemPriceBrokenOrUnidentified = configReader["gameplay"]["itemPriceBrokenOrUnidentified"].GetInt();
			ResistancePerBlessing = configReader["gameplay"]["resistancePerBlessing"].GetInt();
			ItemResellMultiplicator = configReader["gameplay"]["itemResellMultiplicator"].GetFloat();
			ScrollNoviceMagicFactor = configReader["gameplay"]["scrollNoviceMagicFactor"].GetFloat();
			ScrollExpertMagicFactor = configReader["gameplay"]["scrollExpertMagicFactor"].GetFloat();
			ScrollMasterMagicFactor = configReader["gameplay"]["scrollMasterMagicFactor"].GetFloat();
			ScrollGrandmasterMagicFactor = configReader["gameplay"]["scrollGrandmasterMagicFactor"].GetFloat();
			RangedAttackMeleeMalus = configReader["gameplay"]["rangedAttackMeleeMalus"].GetFloat();
			HoursDeficiencySyndromesRest = configReader["conditions"]["hoursDeficiencySyndromesRest"].GetInt();
			MinutesDeficiencySyndromesTick = configReader["conditions"]["minutesDeficiencySyndromesTick"].GetInt();
			PoisonEvadeDecrease = configReader["conditions"]["poisonEvadeDecrease"].GetFloat();
			PoisonHealthDamage = configReader["conditions"]["poisonHealthDamage"].GetFloat();
			SleepingWakeupDamage = configReader["conditions"]["sleepingWakeupDamage"].GetFloat();
			CursedAttribDecrease = configReader["conditions"]["cursedAttribDecrease"].GetFloat();
			CursedAttackDecrease = configReader["conditions"]["cursedAttackDecrease"].GetFloat();
			WeakAttribDecrease = configReader["conditions"]["weakAttribDecrease"].GetFloat();
			KnockedOutTurnCount = configReader["conditions"]["knockedOutTurnCount"].GetInt();
			CantDoAnythingTurnCount = configReader["conditions"]["cantDoAnythingTurnCount"].GetInt();
			DeadBaseValue = configReader["conditions"]["deadBaseValue"].GetInt();
			DeadVitalityMultiplicator = configReader["conditions"]["deadVitalityMultiplicator"].GetInt();
			SkillPointsPerLevelUp = configReader["skills"]["skillPointsPerLevelUp"].GetInt();
			AttributePointsPerLevelUp = configReader["skills"]["attributePointsPerLevelUp"].GetInt();
			RequiredSkillLevelNovice = configReader["skills"]["requiredSkillLevelNovice"].GetInt();
			RequiredSkillLevelExpert = configReader["skills"]["requiredSkillLevelExpert"].GetInt();
			RequiredSkillLevelMaster = configReader["skills"]["requiredSkillLevelMaster"].GetInt();
			RequiredSkillLevelGrandMaster = configReader["skills"]["requiredSkillLevelGrandMaster"].GetInt();
			SkillExpertPrice = configReader["skills"]["skillExpertPrice"].GetInt();
			SkillMasterPrice = configReader["skills"]["skillMasterPrice"].GetInt();
			SkillGrandmasterPrice = configReader["skills"]["skillGrandmasterPrice"].GetInt();
			PartyMembers = configReader["party"]["members"].GetIntArray();
			HealthPerMight = configReader["party"]["healthPerMight"].GetFloat();
			HealthPerVitality = configReader["party"]["healthPerVitality"].GetFloat();
			ManaPerMagic = configReader["party"]["manaPerMagic"].GetFloat();
			ManaPerSpirit = configReader["party"]["manaPerSpirit"].GetFloat();
			MainHandAttackValue = configReader["combat"]["mainHandAttackValue"].GetFloat();
			OffHandAttackValue = configReader["combat"]["offHandAttackValue"].GetFloat();
			RangeHandAttackValue = configReader["combat"]["rangeAttackValue"].GetFloat();
			MainHandDamage = configReader["combat"]["mainHandDamage"].GetFloat();
			RangedDamage = configReader["combat"]["rangedDamage"].GetFloat();
			MagicDamage = configReader["combat"]["magicDamage"].GetFloat();
			OffHandDamage = configReader["combat"]["offHandDamage"].GetFloat();
			MainHandCritChanceDestinyMod = configReader["combat"]["mainHandCritChanceDestinyMod"].GetFloat();
			OffHandCritChanceDestinyMod = configReader["combat"]["offHandCritChanceDestinyMod"].GetFloat();
			RangedCriticalHitDestinyMod = configReader["combat"]["rangedCritChanceDestinyMod"].GetFloat();
			AttackValuePenaltiesLightArmor = configReader["combat"]["attackValuePenaltiesLightArmor"].GetIntArray();
			AttackValuePenaltiesHeavyArmor = configReader["combat"]["attackValuePenaltiesHeavyArmor"].GetIntArray();
			DefendEvadeBonus = configReader["combat"]["defendEvadeBonus"].GetFloat();
			DefendBlockBonus = configReader["combat"]["defendBlockBonus"].GetFloat();
			MagicCritChance = configReader["combat"]["magicCritChance"].GetFloat();
			MagicCritFactor = configReader["combat"]["magicCritFactor"].GetFloat();
			MonsterCastChance = configReader["combat"]["monsterCastChance"].GetInt();
			EvadeFactor = configReader["combat"]["evadeFactor"].GetFloat();
			MagicEvadeFactor = configReader["combat"]["magicEvadeFactor"].GetFloat();
			TradingInventorySize = configReader["npc"]["tradingInventorySize"].GetInt();
			CostCast = configReader["npc"]["costCast"].GetInt();
			CostCure = configReader["npc"]["costCure"].GetInt();
			CostIdentify = configReader["npc"]["costIdentify"].GetInt();
			CostRepair = configReader["npc"]["costRepair"].GetInt();
			CostRest = configReader["npc"]["costRest"].GetInt();
			CostRestoration = configReader["npc"]["costRestoration"].GetInt();
			CostResurrect = configReader["npc"]["costResurrect"].GetInt();
			CostTravel = configReader["npc"]["costTravel"].GetInt();
			CostSupplies = configReader["npc"]["costSupplies"].GetInt();
			CostRespec = configReader["npc"]["costRespec"].GetInt();
			HirelingBlockChance = configReader["npc"]["hirelingBlockChance"].GetFloat();
			HirelingGoldShareFactor = configReader["pricing"]["hirelingGoldShareFactor"].GetFloat();
			ItemSpellsFactor = configReader["warriorMode"]["itemSpellsFactor"].GetFloat();
			ItemConsumablesFactor = configReader["warriorMode"]["itemConsumablesFactor"].GetFloat();
			ItemEquipmentFactor = configReader["warriorMode"]["itemEquipmentFactor"].GetFloat();
			NpcSuppliesFactor = configReader["warriorMode"]["npcSuppliesFactor"].GetFloat();
			NpcItemRepairFactor = configReader["warriorMode"]["npcItemRepairFactor"].GetFloat();
			NpcItemIdentifyFactor = configReader["warriorMode"]["npcItemIdentifyFactor"].GetFloat();
			NpcRestoreFactor = configReader["warriorMode"]["npcRestoreFactor"].GetFloat();
			NpcResurrectFactor = configReader["warriorMode"]["npcResurrectFactor"].GetFloat();
			NpcCureFactor = configReader["warriorMode"]["npcCureFactor"].GetFloat();
			NpcRestFactor = configReader["warriorMode"]["npcRestFactor"].GetFloat();
			NpcSkillTrainingFactor = configReader["warriorMode"]["npcSkillTrainingFactor"].GetFloat();
			NpcClassPromotionFactor = configReader["warriorMode"]["npcClassPromotionFactor"].GetFloat();
			NpcTravelFactor = configReader["warriorMode"]["npcTravelFactor"].GetFloat();
			NpcHirelingCostsFactor = configReader["warriorMode"]["npcHirelingCostsFactor"].GetFloat();
			NpcRespecFactor = configReader["warriorMode"]["npcRespecFactor"].GetFloat();
			MonsterHealthCoreFactor = configReader["warriorMode"]["monsterHealthCoreFactor"].GetFloat();
			MonsterHealthEliteFactor = configReader["warriorMode"]["monsterHealthEliteFactor"].GetFloat();
			MonsterHealthChampionFactor = configReader["warriorMode"]["monsterHealthChampionFactor"].GetFloat();
			ItemResellMultiplicatorHard = configReader["warriorMode"]["itemResellMultiplicatorHard"].GetFloat();
			DamageReceiveFactor = configReader["warriorMode"]["damageReceiveFactor"].GetFloat();
			EnableRendezVousLogging = configReader["online"]["enableRendezVousLogging"].GetBool();
		}

		public void Write(FileStream p_stream)
		{
		}
	}
}
