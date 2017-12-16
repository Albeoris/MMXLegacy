using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Core.UpdateLogic.Interactions;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Container : InteractiveObject
	{
		private Inventory m_content;

		private Int32 m_tokenID;

		private Int32 m_loreBookID;

		private EContainerType m_containerType;

		private EInteractiveObjectState m_State;

		private OpenContainerInteraction m_openInteraction;

		private Single m_dropGoldChance;

		private IntRange m_dropGoldAmount;

		private ModelProbability[] m_dropModelLevels;

		private Single m_dropItemChance;

		private Single m_dropItemPrefixChance;

		private EnchantmentProbability[] m_dropItemPrefixProbabilities;

		private EnchantmentProbability[] m_dropItemSuffixProbabilities;

		private Single m_dropItemSuffixChance;

		private EEquipmentType[] m_dropItemSpecificationList;

		private SteadyLoot[] m_dropSteadyItems;

		public Container() : this(0, 0)
		{
		}

		public Container(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.CONTAINER, p_spawnerID)
		{
			m_content = new Inventory(50);
			m_tokenID = 0;
			m_loreBookID = 0;
			m_containerType = EContainerType.CHEST;
		}

		public Boolean GenLootHandler(Int32 p_chest)
		{
			ChestStaticData staticData = StaticDataHandler.GetStaticData<ChestStaticData>(EDataType.CHEST, p_chest);
			m_dropGoldChance = staticData.DropGoldChance;
			m_dropGoldAmount = staticData.DropGoldAmount;
			m_dropModelLevels = staticData.ModelLevels;
			m_dropItemChance = staticData.DropItemChance;
			m_dropItemPrefixChance = staticData.DropItemPrefixChance;
			m_dropItemSuffixChance = staticData.DropItemSuffixChance;
			m_dropItemPrefixProbabilities = staticData.DropItemPrefixProbabilities;
			m_dropItemSuffixProbabilities = staticData.DropItemSuffixProbabilities;
			m_dropItemSpecificationList = staticData.DropItemSpecificationList;
			m_dropSteadyItems = staticData.DropSteadyLoot;
			return true;
		}

		public Inventory Content => m_content;

	    public Int32 TokenID => m_tokenID;

	    public Int32 LoreBookID => m_loreBookID;

	    public EContainerType ContainerType
		{
			get => m_containerType;
	        set => m_containerType = value;
	    }

		public override EInteractiveObjectState State
		{
			get => m_State;
		    set
			{
				if (value != EInteractiveObjectState.NONE && value != EInteractiveObjectState.CONTAINER_OPENED)
				{
					throw new ArgumentException("Invalid Container state: " + value);
				}
				m_State = value;
			}
		}

		public Boolean IsEmpty()
		{
			Boolean flag = false;
			foreach (SpawnCommand spawnCommand in Commands)
			{
				if ((spawnCommand.Type == EInteraction.SPAWN_MONSTER || spawnCommand.Type == EInteraction.CAST_PARTY_SPELL) && spawnCommand.Timing == EInteractionTiming.ON_EXECUTE && (spawnCommand.ActivateCount == -1 || spawnCommand.ActivateCount > 0))
				{
					flag = true;
					break;
				}
			}
			return m_content.GetCurrentItemCount() == 0 && m_tokenID == 0 && !flag && m_loreBookID == 0;
		}

		public Boolean IsEmptyCheckWithoutMonster()
		{
			return m_content.GetCurrentItemCount() == 0 && m_tokenID == 0 && m_loreBookID == 0;
		}

		public Boolean ContainsEquipment()
		{
			for (Int32 i = 0; i < m_content.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = m_content.GetItemAt(i);
				if (itemAt != null && itemAt is Equipment)
				{
					return true;
				}
			}
			return false;
		}

		public override void SetData(EInteractiveObjectData p_key, String p_value)
		{
			switch (p_key)
			{
			case EInteractiveObjectData.CONTAINER_ITEM:
			{
				String[] array = p_value.Split(',');
				EDataType edataType = (EDataType)Enum.Parse(typeof(EDataType), array[0]);
				Int32 p_staticID = Convert.ToInt32(array[1]);
				BaseItem baseItem = ItemFactory.CreateItem(edataType, p_staticID);
				if (edataType == EDataType.GENERATED_EQUIPMENT && baseItem != null)
				{
					((Equipment)baseItem).InitIdentification();
				}
				m_content.AddItem(baseItem);
				return;
			}
			case EInteractiveObjectData.CONTAINER_GOLD:
				m_content.AddItem(new GoldStack(Convert.ToInt32(p_value)));
				return;
			case EInteractiveObjectData.CONTAINER_TOKEN:
				m_tokenID = Convert.ToInt32(p_value);
				return;
			case EInteractiveObjectData.CONTAINER_BOOK:
				m_loreBookID = Convert.ToInt32(p_value);
				return;
			case EInteractiveObjectData.CONTAINER_GENERATED:
			{
				Int32 p_chest = Convert.ToInt32(p_value);
				GenLootHandler(p_chest);
				if (Random.Value < m_dropItemChance)
				{
					Equipment equipment = ItemFactory.CreateEquipment(m_dropModelLevels, m_dropItemPrefixChance, m_dropItemSuffixChance, m_dropItemPrefixProbabilities, m_dropItemSuffixProbabilities, m_dropItemSpecificationList);
					equipment.InitIdentification();
					m_content.AddItem(equipment);
				}
				if (Random.Value < m_dropGoldChance)
				{
					Int32 num = m_dropGoldAmount.Random();
					if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
					{
						num = (Int32)(num * 0.75f);
					}
					if (num > 0)
					{
						m_content.AddItem(new GoldStack(Convert.ToInt32(num)));
					}
				}
				foreach (SteadyLoot steadyLoot in m_dropSteadyItems)
				{
					if (Random.Value < steadyLoot.DropChance)
					{
						Int32 num2 = Random.Range(steadyLoot.Amount.Min, steadyLoot.Amount.Max);
						if (steadyLoot.ItemClass == EDataType.POTION)
						{
							BaseItem baseItem2 = ItemFactory.CreateItem(steadyLoot.ItemClass, steadyLoot.ItemID);
							((Consumable)baseItem2).Counter = num2;
							m_content.AddItem(baseItem2);
						}
						else
						{
							for (Int32 j = 0; j < num2; j++)
							{
								BaseItem p_item = ItemFactory.CreateItem(steadyLoot.ItemClass, steadyLoot.ItemID);
								m_content.AddItem(p_item);
							}
						}
					}
				}
				return;
			}
			}
			base.SetData(p_key, p_value);
		}

		public void InteractionOpenContainer(OpenContainerInteraction interaction)
		{
			if (State != EInteractiveObjectState.CONTAINER_OPENED)
			{
				State = EInteractiveObjectState.CONTAINER_OPENED;
				BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(this, Position);
				LegacyLogic.Instance.EventManager.InvokeEvent(interaction, EEventType.CONTAINER_STATE_CHANGED, p_eventArgs);
			}
			m_openInteraction = interaction;
		}

		public void OpenContainer()
		{
			if (m_tokenID > 0)
			{
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddToken(m_tokenID);
				m_tokenID = 0;
			}
			if (m_loreBookID > 0)
			{
				LegacyLogic.Instance.WorldManager.LoreBookHandler.AddLoreBook(m_loreBookID);
				m_loreBookID = 0;
			}
			foreach (SpawnCommand spawnCommand in Commands)
			{
				if (spawnCommand.Type == EInteraction.SPAWN_MONSTER && spawnCommand.Timing == EInteractionTiming.ON_EXECUTE && (spawnCommand.ActivateCount == -1 || spawnCommand.ActivateCount > 0))
				{
					return;
				}
			}
			if (Content.GetCurrentItemCount() > 0)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CONTAINER_SCREEN_OPENED, EventArgs.Empty);
			}
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(this, Position);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CONTAINER_DONE_LOOTING, p_eventArgs);
		}

		public void CloseContainer()
		{
			if (m_openInteraction != null)
			{
				m_openInteraction.DeactiveContainerIfEmpty();
				m_openInteraction = null;
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CONTAINER_SCREEN_CLOSED, EventArgs.Empty);
		}

		public void LootAll()
		{
			List<BaseItem> list = new List<BaseItem>();
			Int32 num = 0;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Boolean flag = false;
			for (Int32 i = 0; i < m_content.GetMaximumItemCount(); i++)
			{
				BaseItem baseItem = m_content.GetItemAt(i);
				if (baseItem != null)
				{
					IInventory inventory = party.ActiveInventory;
					if (!inventory.CanAddItem(baseItem))
					{
						inventory = party.GetOtherInventory(inventory);
					}
					if (inventory.CanAddItem(baseItem))
					{
						if (baseItem is GoldStack)
						{
							Int32 num2 = ((GoldStack)baseItem).Amount;
							NpcEffect npcEffect;
							if (party.HirelingHandler.HasEffect(ETargetCondition.HIRE_BONUSGF, out npcEffect) && npcEffect.TargetEffect != ETargetCondition.NONE)
							{
								num2 = (Int32)Math.Round(num2 * (1f + npcEffect.EffectValue), MidpointRounding.AwayFromZero);
								baseItem = new GoldStack(num2);
							}
							num += num2;
						}
						else
						{
							list.Add(baseItem);
						}
						inventory.AddItem(baseItem);
						m_content.RemoveItemAt(i);
						if (baseItem is Equipment && !((Equipment)baseItem).Identified && !((Equipment)baseItem).IsRelic())
						{
							LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.UNIDENTIFIED_ITEM);
						}
						if (baseItem is Equipment && ((Equipment)baseItem).IsRelic())
						{
							LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.RELIC);
						}
					}
					else if (!flag)
					{
						flag = true;
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.INVENTORY_FULL);
					}
				}
			}
			DestroyContainerCheck();
			NotifyHUDLoot(list, num);
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(this, Position);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CONTAINER_DONE_LOOTING, p_eventArgs);
		}

		public void NotifyHUDLoot(List<BaseItem> p_lootItems, Int32 p_gold)
		{
			ChestLootEventArgs p_eventArgs = new ChestLootEventArgs(p_lootItems, p_gold);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_GET_LOOT, p_eventArgs);
		}

		public void DestroyContainerCheck()
		{
			if (m_containerType == EContainerType.LOOT_BAG && m_content.GetCurrentItemCount() == 0)
			{
				LegacyLogic.Instance.WorldManager.DestroyObject(this, Position);
			}
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			m_tokenID = p_data.Get<Int32>("TokenID", 0);
			m_loreBookID = p_data.Get<Int32>("LoreBookID", 0);
			m_containerType = p_data.Get<EContainerType>("ContainerType", EContainerType.CHEST);
			SaveGameData saveGameData = p_data.Get<SaveGameData>("Content", null);
			if (saveGameData != null)
			{
				m_content.Load(saveGameData);
			}
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<Int32>("TokenID", m_tokenID);
			p_data.Set<Int32>("LoreBookID", m_loreBookID);
			p_data.Set<Int32>("ContainerType", (Int32)m_containerType);
			SaveGameData saveGameData = new SaveGameData("Content");
			m_content.Save(saveGameData);
			p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
		}

		public void AddItem(BaseItem p_item)
		{
			m_content.AddItem(p_item);
			if (m_openInteraction != null)
			{
				m_openInteraction.IncreaseActivateCount();
			}
			foreach (SpawnCommand spawnCommand in Commands)
			{
				if (spawnCommand.Type == EInteraction.OPEN_CONTAINER)
				{
					spawnCommand.ActivateCount = 1;
				}
			}
		}
	}
}
