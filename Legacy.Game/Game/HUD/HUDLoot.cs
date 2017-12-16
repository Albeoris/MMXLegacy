using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDLoot")]
	public class HUDLoot : HUDSideInfoHandlerBase
	{
		[SerializeField]
		private HUDLootEntry m_lootGoldEntry;

		[SerializeField]
		protected GameObject m_ItemEntryPrefab;

		private Queue<LootData> m_lootQueue;

		private AudioObject m_audioObject;

		private Boolean m_isGoldShown;

		private Boolean m_isGoldEntryDisabled;

		protected override void Awake()
		{
			base.Awake();
			m_lootGoldEntry.SetEnabledEntry(false);
			m_lootQueue = new Queue<LootData>();
			DelayedEventManager.RegisterEvent(EDelayType.ON_FIXED_DELAY, EEventType.PARTY_GET_LOOT, new EventHandler(OnGetLoot));
		}

		private void OnDestroy()
		{
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FIXED_DELAY, EEventType.PARTY_GET_LOOT, new EventHandler(OnGetLoot));
		}

		protected override void Update()
		{
			base.Update();
			if (m_lootQueue.Count > 0)
			{
				if (m_lootQueue.Peek().m_gold > 0)
				{
					LootData lootData = m_lootQueue.Dequeue();
					if (ConfigManager.Instance.Options.IsLeftActionBarShowingArrows)
					{
						m_isGoldShown = true;
						m_lootGoldEntry.Init(null, lootData.m_gold);
						if (m_audioObject == null || !m_audioObject.IsPlaying())
						{
							m_audioObject = AudioController.Play("Gold_loot");
						}
					}
				}
				else
				{
					GameObject gameObject = Helper.Instantiate<GameObject>(m_ItemEntryPrefab);
					HUDLootEntry component = gameObject.GetComponent<HUDLootEntry>();
					AddEntry(component);
					LootData lootData2 = m_lootQueue.Dequeue();
					component.Init(lootData2.m_item, 0);
					if (m_audioObject == null || !m_audioObject.IsPlaying())
					{
						m_audioObject = AudioController.Play("item_loot");
					}
				}
			}
			if (m_lootGoldEntry.IsUsed())
			{
				m_lootGoldEntry.UpdateEntry();
			}
			else if (m_isGoldShown)
			{
				m_lootGoldEntry.SetEnabledEntry(false);
				m_isGoldShown = false;
			}
			else if (!ConfigManager.Instance.Options.IsLeftActionBarShowingArrows && !m_isGoldEntryDisabled)
			{
				ENTRIES_ROOT.transform.localPosition = ENTRIES_ROOT.transform.localPosition - Vector3.up * (ENTRY_HEIGHT + ENTRY_HEIGHT_OFFSET);
				m_isGoldEntryDisabled = true;
			}
			else if (ConfigManager.Instance.Options.IsLeftActionBarShowingArrows && m_isGoldEntryDisabled)
			{
				ENTRIES_ROOT.transform.localPosition = ENTRIES_ROOT.transform.localPosition + Vector3.up * (ENTRY_HEIGHT + ENTRY_HEIGHT_OFFSET);
				m_isGoldEntryDisabled = false;
			}
		}

		public void AddLoot(Int32 gold, BaseItem item)
		{
			if (!enabled)
			{
				enabled = true;
			}
			LootData item2 = new LootData(gold, item);
			m_lootQueue.Enqueue(item2);
		}

		private void OnGetLoot(Object sender, EventArgs p_args)
		{
			if (p_args is MonsterLootEntryEventArgs)
			{
				MonsterLootEntryEventArgs monsterLootEntryEventArgs = (MonsterLootEntryEventArgs)p_args;
				AddLoot(monsterLootEntryEventArgs.Gold, monsterLootEntryEventArgs.Item);
			}
			if (p_args is ChestLootEventArgs)
			{
				ChestLootEventArgs chestLootEventArgs = (ChestLootEventArgs)p_args;
				LegacyLogic.Instance.ActionLog.PushEntry(chestLootEventArgs);
				if (chestLootEventArgs.Gold > 0)
				{
					AddLoot(chestLootEventArgs.Gold, null);
				}
				if (chestLootEventArgs.Items != null)
				{
					foreach (BaseItem item in chestLootEventArgs.Items)
					{
						AddLoot(0, item);
					}
				}
			}
			if (p_args is QuestLootEntryEventArgs)
			{
				QuestLootEntryEventArgs questLootEntryEventArgs = (QuestLootEntryEventArgs)p_args;
				LegacyLogic.Instance.ActionLog.PushEntry(questLootEntryEventArgs);
				foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
				{
					if (character != null)
					{
						character.FlushRewardsActionLog();
					}
				}
				if (questLootEntryEventArgs.Gold != 0)
				{
					AddLoot(questLootEntryEventArgs.Gold, null);
				}
				if (questLootEntryEventArgs.Item != null)
				{
					AddLoot(0, questLootEntryEventArgs.Item);
				}
			}
		}

		public class LootData
		{
			public Int32 m_gold;

			public BaseItem m_item;

			public LootData(Int32 gold, BaseItem item)
			{
				m_gold = gold;
				m_item = item;
			}
		}
	}
}
