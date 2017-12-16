using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.Quests;
using Legacy.Core.StaticData;

namespace Legacy.Core.Entities
{
	public class LootHandler
	{
		private const Int32 POTION_HEALTH_ID = 1;

		private const Int32 POTION_MANA_ID = 2;

		private Monster m_monster;

		private QuestStep m_quest;

		private Single m_dropGoldChance;

		private IntRange m_dropGoldAmount;

		private ModelProbability[] m_modelProbabilities;

		private Single m_dropItemChance;

		private Single m_dropItemPrefixChance;

		private Single m_dropItemSuffixChance;

		private EnchantmentProbability[] m_dropItemPrefixProbabilities;

		private EnchantmentProbability[] m_dropItemSuffixProbabilities;

		private EEquipmentType[] m_dropItemSpecificationList;

		private SteadyLoot[] m_steadyLoot;

		private Single m_XpReward;

		private Int32[] m_tokenIDs;

		private Boolean m_inventoryFullBarkTriggered;

		private List<LogEntryEventArgs> m_monsterLootEntries;

		public LootHandler(Monster p_monster)
		{
			m_monster = p_monster;
			MonsterStaticData staticData = p_monster.StaticData;
			m_dropGoldChance = staticData.DropGoldChance;
			m_dropGoldAmount = staticData.DropGoldAmount;
			m_modelProbabilities = staticData.DropModelLevels;
			m_dropItemChance = staticData.DropItemChance;
			m_dropItemPrefixChance = staticData.DropItemPrefixChance;
			m_dropItemSuffixChance = staticData.DropItemSuffixChance;
			m_dropItemPrefixProbabilities = staticData.PrefixProbabilities;
			m_dropItemSuffixProbabilities = staticData.SuffixProbabilities;
			m_dropItemSpecificationList = staticData.DropItemSpecificationList;
			m_steadyLoot = staticData.SteadyLoot;
			m_XpReward = staticData.XpReward;
			m_tokenIDs = staticData.DropTokenID;
		}

		public LootHandler(QuestStep p_quest)
		{
			m_quest = p_quest;
			QuestStepStaticData staticData = p_quest.StaticData;
			m_dropGoldChance = staticData.DropGoldChance;
			m_dropGoldAmount = staticData.DropGoldAmount;
			m_modelProbabilities = staticData.DropModelLevels;
			m_dropItemChance = staticData.DropItemChance;
			m_dropItemPrefixChance = staticData.DropItemPrefixChance;
			m_dropItemSuffixChance = staticData.DropItemSuffixChance;
			m_dropItemPrefixProbabilities = staticData.PrefixProbabilities;
			m_dropItemSuffixProbabilities = staticData.SuffixProbabilities;
			m_dropItemSpecificationList = staticData.DropItemSpecificationList;
			m_steadyLoot = staticData.SteadyLoot;
			m_XpReward = staticData.RewardXP;
			m_tokenIDs = staticData.TokenID;
		}

		public void DistributeRewards(Character p_killingCharacter)
		{
			if (LegacyLogic.Instance.WorldManager != null)
			{
				m_inventoryFullBarkTriggered = false;
				m_monsterLootEntries = new List<LogEntryEventArgs>();
				DistributeXpReward(p_killingCharacter);
				DistributeGold();
				DistributeSteadyLootItem();
				DistributeGeneratedItem();
				DistributeToken();
			}
		}

		internal void DistributeGold()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party != null && Random.Value < m_dropGoldChance)
			{
				Int32 num = m_dropGoldAmount.Random();
				if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
				{
					num = (Int32)(num * 0.75f);
				}
				if (num > 0)
				{
					NpcEffect npcEffect;
					if (party.HirelingHandler.HasEffect(ETargetCondition.HIRE_BONUSGF, out npcEffect) && npcEffect.TargetEffect != ETargetCondition.NONE)
					{
						num = (Int32)Math.Round(num * (1f + npcEffect.EffectValue), MidpointRounding.AwayFromZero);
					}
					party.ChangeGold(num);
					if (m_monster != null)
					{
						MonsterLootEntryEventArgs monsterLootEntryEventArgs = new MonsterLootEntryEventArgs(m_monster);
						monsterLootEntryEventArgs.Gold = num;
						m_monsterLootEntries.Add(monsterLootEntryEventArgs);
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_GET_LOOT, monsterLootEntryEventArgs);
					}
					else if (m_quest != null)
					{
						QuestLootEntryEventArgs questLootEntryEventArgs = new QuestLootEntryEventArgs(m_quest);
						questLootEntryEventArgs.Gold = num;
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_GET_LOOT, questLootEntryEventArgs);
					}
				}
			}
		}

		internal void DistributeGeneratedItem()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party != null && Random.Value < m_dropItemChance)
			{
				NpcEffect npcEffect;
				Equipment equipment;
				if (party.HirelingHandler.HasEffect(ETargetCondition.HIRE_BONUSMF, out npcEffect) && npcEffect.TargetEffect != ETargetCondition.NONE)
				{
					equipment = ItemFactory.CreateEquipment(m_modelProbabilities, m_dropItemPrefixChance * (1f + npcEffect.EffectValue), m_dropItemSuffixChance * (1f + npcEffect.EffectValue), m_dropItemPrefixProbabilities, m_dropItemSuffixProbabilities, m_dropItemSpecificationList);
				}
				else
				{
					equipment = ItemFactory.CreateEquipment(m_modelProbabilities, m_dropItemPrefixChance, m_dropItemSuffixChance, m_dropItemPrefixProbabilities, m_dropItemSuffixProbabilities, m_dropItemSpecificationList);
				}
				equipment.InitIdentification();
				DropItem(equipment);
			}
		}

		internal void DistributeSteadyLootItem()
		{
			foreach (SteadyLoot steadyLoot2 in m_steadyLoot)
			{
				if (Random.Value < steadyLoot2.DropChance)
				{
					Int32 num = Random.Range(steadyLoot2.Amount.Min, steadyLoot2.Amount.Max + 1);
					if (num > 0)
					{
						if (steadyLoot2.ItemClass == EDataType.POTION)
						{
							BaseItem baseItem = ItemFactory.CreateItem(steadyLoot2.ItemClass, steadyLoot2.ItemID);
							((Consumable)baseItem).Counter = num;
							DropItem(baseItem);
						}
						else
						{
							for (Int32 j = 0; j < num; j++)
							{
								BaseItem p_item = ItemFactory.CreateItem(steadyLoot2.ItemClass, steadyLoot2.ItemID);
								DropItem(p_item);
							}
						}
					}
				}
			}
		}

		internal void DropItem(BaseItem p_item)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party != null && p_item != null)
			{
				PartyInventoryController partyInventoryController = party.Inventory;
				if (!partyInventoryController.CanAddItem(p_item))
				{
					partyInventoryController = (PartyInventoryController)party.GetOtherInventory(partyInventoryController);
				}
				if (partyInventoryController.CanAddItem(p_item))
				{
					if (p_item is Equipment)
					{
						LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.EQUIPMENT);
					}
					partyInventoryController.AddItem(p_item);
					if (p_item is Equipment && !((Equipment)p_item).Identified && !((Equipment)p_item).IsRelic())
					{
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.UNIDENTIFIED_ITEM);
					}
					if (p_item is Equipment && ((Equipment)p_item).IsRelic())
					{
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.RELIC);
					}
					if (m_monster != null)
					{
						MonsterLootEntryEventArgs monsterLootEntryEventArgs = new MonsterLootEntryEventArgs(m_monster);
						monsterLootEntryEventArgs.Item = p_item;
						m_monsterLootEntries.Add(monsterLootEntryEventArgs);
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_GET_LOOT, monsterLootEntryEventArgs);
					}
					else if (m_quest != null)
					{
						QuestLootEntryEventArgs questLootEntryEventArgs = new QuestLootEntryEventArgs(m_quest);
						questLootEntryEventArgs.Item = p_item;
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_GET_LOOT, questLootEntryEventArgs);
					}
				}
				else if (m_monster != null || m_quest != null)
				{
					if (!m_inventoryFullBarkTriggered)
					{
						m_inventoryFullBarkTriggered = true;
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.INVENTORY_FULL);
					}
					GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position);
					List<InteractiveObject> passiveInteractiveObjects = slot.GetPassiveInteractiveObjects(EDirection.CENTER, false, false, false);
					Container container = null;
					foreach (InteractiveObject interactiveObject in passiveInteractiveObjects)
					{
						Container container2 = interactiveObject as Container;
						if (container2 != null && container2.ContainerType == EContainerType.LOOT_BAG)
						{
							container = container2;
							break;
						}
					}
					if (container == null)
					{
						Int32 nextDynamicSpawnID = LegacyLogic.Instance.WorldManager.GetNextDynamicSpawnID();
						SpawnCommand spawnCommand = new SpawnCommand();
						spawnCommand.Type = EInteraction.OPEN_CONTAINER;
						spawnCommand.Precondition = "NONE";
						spawnCommand.Timing = EInteractionTiming.ON_EXECUTE;
						spawnCommand.RequiredState = EInteractiveObjectState.NONE;
						spawnCommand.ActivateCount = -1;
						spawnCommand.TargetSpawnID = nextDynamicSpawnID;
						container = new Container(8, nextDynamicSpawnID);
						container.Location = EDirection.CENTER;
						container.Position = party.Position;
						container.Prefab = "Prefabs/InteractiveObjects/LootContainer/Lootbag/Lootbag";
						container.Commands.Add(spawnCommand);
						container.ContainerType = EContainerType.LOOT_BAG;
						slot.AddInteractiveObject(container);
						LegacyLogic.Instance.WorldManager.SpawnObject(container, party.Position);
					}
					container.Content.AddItem(p_item);
					if (m_monster != null)
					{
						LegacyLogic.Instance.ActionLog.PushEntry(new MonsterLootEntryEventArgs(m_monster));
					}
					else if (m_quest != null)
					{
						LegacyLogic.Instance.ActionLog.PushEntry(new QuestLootEntryEventArgs(m_quest));
					}
				}
			}
		}

		internal void DistributeXpReward(Character p_killingCharacter)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party != null && m_XpReward > 0f)
			{
				Character[] members = party.Members;
				if (members != null && members.Length > 0)
				{
					Int32 num = (Int32)m_XpReward;
					Boolean flag = false;
					NpcEffect npcEffect;
					if (party.HirelingHandler.HasEffect(ETargetCondition.HIRE_BONUSXP, out npcEffect) && npcEffect.TargetEffect != ETargetCondition.NONE && m_monster != null)
					{
						num = (Int32)Math.Round(num * (1f + npcEffect.EffectValue), MidpointRounding.AwayFromZero);
					}
					if (m_monster == null && m_steadyLoot.Length == 0 && m_dropGoldAmount.Max == 0)
					{
						flag = true;
					}
					foreach (Character character in members)
					{
						if (character != null)
						{
							if (character != p_killingCharacter || m_monster == null)
							{
								if (m_monster == null && p_killingCharacter == null)
								{
									character.AddQuestExp(num);
									if (flag)
									{
										character.FlushRewardsActionLog();
									}
								}
								else
								{
									character.AddExp(num);
								}
							}
							else
							{
								character.AddExp((Int32)Math.Round(num * ConfigManager.Instance.Game.RewardXpMultiplier, MidpointRounding.AwayFromZero));
							}
						}
					}
				}
			}
		}

		private void DistributeToken()
		{
			for (Int32 i = 0; i < m_tokenIDs.Length; i++)
			{
				if (m_tokenIDs[i] > 0)
				{
					Party party = LegacyLogic.Instance.WorldManager.Party;
					party.TokenHandler.AddToken(m_tokenIDs[i]);
				}
			}
		}

		public void FlushActionLog()
		{
			foreach (LogEntryEventArgs p_args in m_monsterLootEntries)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_monsterLootEntries.Clear();
		}
	}
}
