using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Core.UpdateLogic.Preconditions;
using Legacy.Core.WorldMap;
using Legacy.EffectEngine;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/GameMessageController")]
	public class GameMessageController : MonoBehaviour
	{
		private const Single DEFAULT_STAY_TIME = 3f;

		private const Single TERRAIN_MESSAGE_TIMEOUT = 5f;

		private const String MAP_AREA_LOCA_PREFIX = "AREA_NAME_";

		[SerializeField]
		private UILabel m_messageLabel;

		[SerializeField]
		private UILabel m_submessageLabel;

		private Queue<GameMessage> m_queuedMessages;

		private Queue<GameMessage> m_importantMessages;

		private Queue<GameMessage> m_processedQueue;

		private GameMessage m_currentMessage;

		private Boolean m_occupied;

		private Boolean m_showSubLabel;

		private ECondition m_storedCondition;

		private Boolean m_locker;

		private Queue<DelayedMessage> m_delayed;

		private InteractiveObject m_lastInteractiveObject;

		private Single m_terrainMessageTimeOut = -1f;

		public event EventHandler OccupacionChangeEvent;

		public Boolean Occupied => m_occupied;

	    private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(OnGameOptionsChanged));
			OnGameOptionsChanged(null, EventArgs.Empty);
			m_queuedMessages = new Queue<GameMessage>();
			m_importantMessages = new Queue<GameMessage>();
			m_occupied = false;
			m_showSubLabel = false;
			m_storedCondition = ECondition.NONE;
			m_locker = false;
			m_delayed = new Queue<DelayedMessage>();
			Init();
		}

		private void OnDestroy()
		{
			m_queuedMessages.Clear();
			m_importantMessages.Clear();
			m_importantMessages = null;
			m_delayed.Clear();
			UnregisterAll();
			m_storedCondition = ECondition.NONE;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(OnGameOptionsChanged));
		}

		private void RegisterAll()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnMapChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(DelayEverything));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAME_OVER, new EventHandler(OnGameOver));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTORED, new EventHandler(OnPartyRestored));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnPartySuppliesChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_REVIVED, new EventHandler(OnCharacterRevived));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChangedFakeMethod));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_SKILL_TIER_CHANGED, new EventHandler(OnCharacterSkillLevelChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnDaystateChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.WORLDMAP_LOCATION_ADDED, new EventHandler(OnWorldMapLocationAdded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.BESTIARY_ENTRY_CHANGED, new EventHandler(OnBestiaryEntryChangedFakeMethod));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new EventHandler(OnItemStatusChangedFakeMethod));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_RELIC_LEVEL_UP, new EventHandler(OnRelicLevelUp));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggered));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PRECONDITION_EVALUATED, new EventHandler(OnPreconditionEval));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PASSIVE_INTERACTIVE_OBJECT_FOUND, new EventHandler(OnPassiveObject));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.QUICK_SAVE, new EventHandler(OnQuickSave));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.GAME_MESSAGE, new EventHandler(OnExternalMessageCreation));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_FINISH, EEventType.MONSTER_ATTACKS, new EventHandler(HandleDelayedMessages));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(HandleDelayedMessages));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_FINISH, EEventType.MONSTER_DIED, new EventHandler(HandleDelayedMessages));
		}

		private void UnregisterAll()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnMapChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(DelayEverything));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAME_OVER, new EventHandler(OnGameOver));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTORED, new EventHandler(OnPartyRestored));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnPartySuppliesChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_REVIVED, new EventHandler(OnCharacterRevived));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChangedFakeMethod));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_SKILL_TIER_CHANGED, new EventHandler(OnCharacterSkillLevelChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnDaystateChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.WORLDMAP_LOCATION_ADDED, new EventHandler(OnWorldMapLocationAdded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.BESTIARY_ENTRY_CHANGED, new EventHandler(OnBestiaryEntryChangedFakeMethod));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new EventHandler(OnItemStatusChangedFakeMethod));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_RELIC_LEVEL_UP, new EventHandler(OnRelicLevelUp));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggered));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PRECONDITION_EVALUATED, new EventHandler(OnPreconditionEval));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PASSIVE_INTERACTIVE_OBJECT_FOUND, new EventHandler(OnPassiveObject));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.QUICK_SAVE, new EventHandler(OnQuickSave));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.GAME_MESSAGE, new EventHandler(OnExternalMessageCreation));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_FINISH, EEventType.MONSTER_ATTACKS, new EventHandler(HandleDelayedMessages));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(HandleDelayedMessages));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_FINISH, EEventType.MONSTER_DIED, new EventHandler(HandleDelayedMessages));
		}

		public void Init()
		{
			m_messageLabel.text = String.Empty;
			m_submessageLabel.text = String.Empty;
			m_submessageLabel.alpha = 0f;
			m_messageLabel.enabled = true;
			m_submessageLabel.enabled = true;
		}

		private void Update()
		{
			if (m_locker)
			{
				return;
			}
			if (m_terrainMessageTimeOut > 0f)
			{
				m_terrainMessageTimeOut -= Time.deltaTime;
				if (m_terrainMessageTimeOut <= 0f)
				{
					m_terrainMessageTimeOut = -1f;
				}
			}
			if (m_importantMessages.Count > 0)
			{
				m_processedQueue = m_importantMessages;
			}
			else
			{
				m_processedQueue = m_queuedMessages;
			}
			if (m_processedQueue.Count > 0 && !IngameController.Instance.IsAnyScreenOpen() && !m_occupied)
			{
				SetOccupied(true);
				GameMessage gameMessage = m_processedQueue.Dequeue();
				if (String.IsNullOrEmpty(gameMessage.m_message))
				{
					SetOccupied(false);
				}
				else
				{
					m_currentMessage = null;
					gameMessage.m_insertTime = Time.time + gameMessage.m_delay;
					m_currentMessage = gameMessage;
					if (m_processedQueue == m_importantMessages && gameMessage.m_conditionDead)
					{
						m_queuedMessages.Clear();
					}
					SetMessageText(gameMessage.m_message);
				}
			}
			if (m_occupied && m_currentMessage != null)
			{
				if (IngameController.Instance.IsAnyScreenOpen())
				{
					m_messageLabel.enabled = false;
					m_submessageLabel.enabled = false;
				}
				else
				{
					m_messageLabel.enabled = true;
					m_submessageLabel.enabled = true;
				}
				if (String.IsNullOrEmpty(m_submessageLabel.text) && m_currentMessage.m_insertTime < Time.time)
				{
					m_messageLabel.alpha -= 0.05f;
					if (m_messageLabel.alpha < 0.2f)
					{
						m_currentMessage = null;
						m_messageLabel.text = String.Empty;
						m_submessageLabel.text = String.Empty;
						m_messageLabel.alpha = 1f;
						SetOccupied(false);
					}
				}
				else if (m_currentMessage.m_insertTime < Time.time)
				{
					if (!m_showSubLabel)
					{
						if (m_submessageLabel.alpha < 0.9f)
						{
							m_submessageLabel.alpha += 0.05f;
						}
						else
						{
							m_submessageLabel.alpha = 1f;
						}
					}
					if (m_submessageLabel.alpha == 1f)
					{
						m_showSubLabel = true;
						m_currentMessage.m_insertTime = Time.time + 3f;
					}
					if (m_showSubLabel)
					{
						m_messageLabel.alpha -= 0.05f;
						m_submessageLabel.alpha -= 0.05f;
						if (m_messageLabel.alpha < 0.2f)
						{
							m_currentMessage = null;
							m_messageLabel.text = String.Empty;
							m_submessageLabel.text = String.Empty;
							m_messageLabel.alpha = 1f;
							m_submessageLabel.alpha = 0f;
							SetOccupied(false);
							m_showSubLabel = false;
						}
					}
				}
			}
		}

		private void DelayEverything(Object p_sender, EventArgs p_args)
		{
			m_currentMessage = null;
			m_messageLabel.text = String.Empty;
			m_submessageLabel.text = String.Empty;
			m_messageLabel.alpha = 1f;
			SetOccupied(false);
			m_locker = true;
		}

		private void SetOccupied(Boolean p_occupied)
		{
			if (m_occupied != p_occupied)
			{
				m_occupied = p_occupied;
				if (OccupacionChangeEvent != null)
				{
					OccupacionChangeEvent(this, EventArgs.Empty);
				}
			}
		}

		private void SetMessageText(String text)
		{
			if (!ConfigManager.Instance.Options.ShowGameMessages)
			{
				return;
			}
			if (text.Contains("@"))
			{
				String[] array = text.Split(new Char[]
				{
					'@'
				}, 2);
				if (!String.IsNullOrEmpty(array[0]))
				{
					m_messageLabel.text = array[0];
				}
				if (!String.IsNullOrEmpty(array[1]))
				{
					m_submessageLabel.text = array[1];
				}
				m_currentMessage.m_insertTime = Time.time + 1.5f;
				m_currentMessage.m_hasSubMessage = true;
			}
			else
			{
				m_messageLabel.text = text;
				m_currentMessage.m_hasSubMessage = false;
			}
		}

		public Boolean HasSubMessage()
		{
			return m_currentMessage != null && m_currentMessage.m_hasSubMessage;
		}

		public void AddToQueue(String p_messageKey, Single p_delay)
		{
			String text = LocaManager.GetText(p_messageKey);
			GameMessage item = new GameMessage(text, p_delay);
			m_queuedMessages.Enqueue(item);
		}

		private void HandleDelayedMessages(Object p_sender, EventArgs p_args)
		{
			while (m_delayed.Count > 0)
			{
				DelayedMessage delayedMessage = m_delayed.Dequeue();
				switch (delayedMessage.Type)
				{
				case EDelayedMessageType.ITEM_BROKE:
					ItemRepairStatusChanged(delayedMessage.Sender, delayedMessage.Args);
					break;
				case EDelayedMessageType.CHARACTER_STATUS_CHANGED:
					CharacterStatusChanged(delayedMessage.Sender, delayedMessage.Args);
					break;
				case EDelayedMessageType.BESTIARY:
					BestiaryEntryChanged(delayedMessage.Sender, delayedMessage.Args);
					break;
				}
			}
		}

		private void OnExternalMessageCreation(Object p_sender, EventArgs p_args)
		{
			String p_message = null;
			Single p_delay = 3f;
			if (p_args is GameMessageEventArgs)
			{
				GameMessageEventArgs gameMessageEventArgs = p_args as GameMessageEventArgs;
				if (gameMessageEventArgs == null)
				{
					return;
				}
				if (gameMessageEventArgs.IsTerrainMessage)
				{
					if (m_terrainMessageTimeOut >= 0f)
					{
						return;
					}
					m_terrainMessageTimeOut = 5f;
				}
				if (gameMessageEventArgs.isLocaKey)
				{
					if (String.IsNullOrEmpty(gameMessageEventArgs.locaVar))
					{
						p_message = LocaManager.GetText(gameMessageEventArgs.text);
					}
					else
					{
						p_message = LocaManager.GetText(gameMessageEventArgs.text, LocaManager.GetText(gameMessageEventArgs.locaVar));
					}
				}
				else
				{
					p_message = gameMessageEventArgs.text;
				}
				p_delay = gameMessageEventArgs.time;
			}
			else if (p_args is MapAreaEventArgs)
			{
				MapAreaEventArgs mapAreaEventArgs = p_args as MapAreaEventArgs;
				String id = "AREA_NAME_" + mapAreaEventArgs.CurrentArea.ToString();
				p_message = LocaManager.GetText(id);
			}
			GameMessage item = new GameMessage(p_message, p_delay);
			m_queuedMessages.Enqueue(item);
		}

		private void OnGameOptionsChanged(Object p_sender, EventArgs p_args)
		{
			UnregisterAll();
			if (ConfigManager.Instance.Options.ShowGameMessages)
			{
				RegisterAll();
			}
		}

		private void OnPartySuppliesChanged(Object p_sender, EventArgs p_args)
		{
			ResourcesChangedEventArgs resourcesChangedEventArgs = p_args as ResourcesChangedEventArgs;
			if (resourcesChangedEventArgs == null)
			{
				return;
			}
			if (resourcesChangedEventArgs.ResourceType == ResourcesChangedEventArgs.EResourceType.GOLD)
			{
				return;
			}
			String text = LocaManager.GetText("GAME_MESSAGE_SUPPLIES_ADDED", LegacyLogic.Instance.WorldManager.Party.Supplies);
			GameMessage item = new GameMessage(text);
			m_queuedMessages.Enqueue(item);
		}

		private void OnWorldMapLocationAdded(Object p_sender, EventArgs p_args)
		{
			MapPointVisibleEventArgs mapPointVisibleEventArgs = p_args as MapPointVisibleEventArgs;
			if (mapPointVisibleEventArgs == null)
			{
				return;
			}
			String text = LocaManager.GetText(mapPointVisibleEventArgs.Point.StaticData.InfoKey);
			String text2 = LocaManager.GetText("ACTION_LOG_NEW_LOCATION_ADDED_TO_WORLD_MAP", text);
			GameMessage item = new GameMessage(text2);
			m_queuedMessages.Enqueue(item);
		}

		private void OnRelicLevelUp(Object p_sender, EventArgs p_args)
		{
			Equipment equipment = p_sender as Equipment;
			if (equipment == null)
			{
				return;
			}
			String text = LocaManager.GetText("ACTION_LOG_RELICR_LEVEL_UP", equipment.Name);
			GameMessage item = new GameMessage(text);
			m_queuedMessages.Enqueue(item);
		}

		private void OnPartyResourcesChanged(Object p_sender, EventArgs p_args)
		{
			if (p_args is GameMessageEventArgs)
			{
				String text = LocaManager.GetText("GAME_MESSAGE_PARTY_RESOURCES_CHANGED");
				GameMessage item = new GameMessage(text);
				m_queuedMessages.Enqueue(item);
			}
		}

		private void OnPartyRestored(Object p_sender, EventArgs p_args)
		{
			String text = LocaManager.GetText("GAME_MESSAGE_PARTY_RESTORED");
			GameMessage item = new GameMessage(text);
			m_queuedMessages.Enqueue(item);
		}

		private void OnItemStatusChangedFakeMethod(Object p_sender, EventArgs p_args)
		{
			if (LegacyLogic.Instance.WorldManager.Party.HasAggro() && (!(p_args is ItemStatusEventArgs) || !(((ItemStatusEventArgs)p_args).AffectedItem is MeleeWeapon)))
			{
				DelayedMessage delayedMessage = new DelayedMessage();
				delayedMessage.Sender = p_sender;
				delayedMessage.Args = p_args;
				delayedMessage.Type = EDelayedMessageType.ITEM_BROKE;
				m_delayed.Enqueue(delayedMessage);
			}
			else
			{
				ItemRepairStatusChanged(p_sender, p_args);
			}
		}

		private void ItemRepairStatusChanged(Object p_sender, EventArgs p_args)
		{
			String text = String.Empty;
			if (p_args is ItemStatusEventArgs)
			{
				ItemStatusEventArgs itemStatusEventArgs = p_args as ItemStatusEventArgs;
				Character itemOwner = itemStatusEventArgs.ItemOwner;
				if (itemOwner == null)
				{
					return;
				}
				String str = (itemOwner.Gender != EGender.FEMALE) ? "_M" : "_F";
				Equipment equipment = itemStatusEventArgs.AffectedItem as Equipment;
				if (equipment == null)
				{
					return;
				}
				if (equipment is MeleeWeapon)
				{
					text = LocaManager.GetText("GAME_MESSAGE_WEAPON_BROKEN" + str, itemOwner.Name);
				}
				else if (equipment is Shield)
				{
					text = LocaManager.GetText("GAME_MESSAGE_SHIELD_BROKEN" + str, itemOwner.Name);
				}
				else if (equipment is Armor)
				{
					text = LocaManager.GetText("GAME_MESSAGE_ARMOR_BROKEN" + str, itemOwner.Name);
				}
			}
			if (String.IsNullOrEmpty(text))
			{
				return;
			}
			GameMessage item = new GameMessage(text);
			m_queuedMessages.Enqueue(item);
		}

		public void OnMapChanged(Object p_sender, EventArgs p_args)
		{
			m_locker = false;
			m_queuedMessages.Clear();
			if (String.IsNullOrEmpty(LegacyLogic.Instance.MapLoader.Grid.LocationLocaName))
			{
				String gridFileName = LegacyLogic.Instance.MapLoader.GridFileName;
				String text = gridFileName.Replace("_", " ");
				if (text.Contains(".xml"))
				{
					text = gridFileName.Replace(".xml", String.Empty);
				}
				Int32 num = 0;
				if (Int32.TryParse(gridFileName.Substring(gridFileName.Length - 1), out num))
				{
					text = text.Remove(gridFileName.Length - 2);
				}
				GameMessage item;
				if (LegacyLogic.Instance.MapLoader.Grid.Type == EMapType.DUNGEON && num > 0)
				{
					item = new GameMessage(LocaManager.GetText("GAME_MESSAGE_MAP_CHANGE_DUNGEON", text, num.ToString()), 3.5f);
				}
				else
				{
					Position position = LegacyLogic.Instance.WorldManager.Party.Position;
					EMapArea mapArea = LegacyLogic.Instance.MapLoader.Grid.GetSlot(position).MapArea;
					if (mapArea > EMapArea.SAFE_ZONE)
					{
						text = text + "@" + LocaManager.GetText("AREA_NAME_" + mapArea.ToString());
					}
					item = new GameMessage(text, 3.5f);
				}
				m_queuedMessages.Enqueue(item);
			}
			else
			{
				String locationLocaName = LegacyLogic.Instance.MapLoader.Grid.LocationLocaName;
				String text2 = LocaManager.GetText(locationLocaName);
				if (LegacyLogic.Instance.MapLoader.Grid.Type != EMapType.DUNGEON && !text2.Contains("@"))
				{
					Position position2 = LegacyLogic.Instance.WorldManager.Party.Position;
					EMapArea mapArea2 = LegacyLogic.Instance.MapLoader.Grid.GetSlot(position2).MapArea;
					if (mapArea2 > EMapArea.SAFE_ZONE)
					{
						text2 = text2 + "@" + LocaManager.GetText("AREA_NAME_" + mapArea2.ToString());
					}
				}
				GameMessage item2 = new GameMessage(text2);
				m_queuedMessages.Enqueue(item2);
			}
		}

		public void OnHirelingUpdated(Object p_sender, EventArgs p_args)
		{
			HirelingEventArgs hirelingEventArgs = p_args as HirelingEventArgs;
			if (hirelingEventArgs == null)
			{
				return;
			}
			String p_message = String.Empty;
			String text = LocaManager.GetText(hirelingEventArgs.Npc.StaticData.NameKey);
			if (hirelingEventArgs.Condition == ETargetCondition.HIRE)
			{
				String id = "GAME_MESSAGE_HIRELING_JOINED_M";
				p_message = LocaManager.GetText(id, text);
			}
			else
			{
				if (hirelingEventArgs.Condition != ETargetCondition.FIRE)
				{
					return;
				}
				String id2 = "GAME_MESSAGE_HIRELING_LEFT_M";
				p_message = LocaManager.GetText(id2, text);
			}
			GameMessage item = new GameMessage(p_message);
			m_queuedMessages.Enqueue(item);
		}

		public void OnTokenAdded(Object p_sender, EventArgs p_args)
		{
			TokenEventArgs tokenEventArgs = p_args as TokenEventArgs;
			if (tokenEventArgs == null)
			{
				return;
			}
			Int32 tokenID = tokenEventArgs.TokenID;
			TokenStaticData staticData = StaticDataHandler.GetStaticData<TokenStaticData>(EDataType.TOKEN, tokenID);
			if (!staticData.TokenVisible)
			{
				return;
			}
			String text = LocaManager.GetText(staticData.Name);
			String p_message = String.Empty;
			if (tokenID >= 1 && tokenID <= 6)
			{
				p_message = LocaManager.GetText("ACTION_LOG_BLESSING_ACQUIRED", text);
			}
			else if (tokenID >= 7 && tokenID <= 10)
			{
				p_message = text;
			}
			else if (tokenID >= 11 && tokenID <= 22)
			{
				p_message = LocaManager.GetText("ACTION_LOG_CLASS_UNLOCKED", text);
			}
			else
			{
				p_message = LocaManager.GetText("ACTION_LOG_TOKEN_ACQUIRED", text);
			}
			GameMessage item = new GameMessage(p_message);
			m_queuedMessages.Enqueue(item);
		}

		public void OnTrapTriggered(Object p_sender, EventArgs p_args)
		{
			TrapEventArgs trapEventArgs = p_args as TrapEventArgs;
			GameMessage item = new GameMessage(LocaManager.GetText("TRAP_TRIGGERED", LocaManager.GetText(trapEventArgs.TrapEffect.Name)));
			m_queuedMessages.Enqueue(item);
		}

		public void OnDaystateChanged(Object p_sender, EventArgs p_args)
		{
			GameTime gameTime = p_sender as GameTime;
			if (gameTime == null)
			{
				return;
			}
			if (gameTime.DayState == EDayState.DAWN)
			{
				String text = LocaManager.GetText("ACTION_LOG_DAY_BEGINS");
				GameMessage item = new GameMessage(text);
				m_queuedMessages.Enqueue(item);
			}
			else if (gameTime.DayState == EDayState.NIGHT)
			{
				String text2 = LocaManager.GetText("ACTION_LOG_NIGHT_BEGINS");
				GameMessage item2 = new GameMessage(text2);
				m_queuedMessages.Enqueue(item2);
			}
		}

		public void OnPartyRested(Object p_sender, EventArgs p_args)
		{
			Int32 num = ConfigManager.Instance.Game.MinutesPerRest / 60;
			GameMessage item = new GameMessage(LocaManager.GetText("GAME_MESSAGE_PARTY_RESTED", num));
			m_importantMessages.Enqueue(item);
			m_queuedMessages.Clear();
		}

		private void OnCharacterStatusChangedFakeMethod(Object p_sender, EventArgs p_args)
		{
			Boolean flag = false;
			StatusChangedEventArgs statusChangedEventArgs = p_args as StatusChangedEventArgs;
			Character character = p_sender as Character;
			if (statusChangedEventArgs == null || character == null)
			{
				return;
			}
			if (statusChangedEventArgs.Type != StatusChangedEventArgs.EChangeType.CONDITIONS)
			{
				return;
			}
			m_storedCondition = character.ConditionHandler.LastRemovedCondition;
			if (m_storedCondition != ECondition.NONE)
			{
				flag = true;
			}
			if (LegacyLogic.Instance.WorldManager.Party.HasAggro() && !flag && (!character.ConditionHandler.HasCondition(ECondition.POISONED) || !character.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS)))
			{
				DelayedMessage delayedMessage = new DelayedMessage();
				delayedMessage.Sender = p_sender;
				delayedMessage.Args = p_args;
				delayedMessage.Type = EDelayedMessageType.CHARACTER_STATUS_CHANGED;
				m_delayed.Enqueue(delayedMessage);
			}
			else
			{
				CharacterStatusChanged(p_sender, p_args);
			}
		}

		private void CharacterStatusChanged(Object p_sender, EventArgs p_args)
		{
			StatusChangedEventArgs statusChangedEventArgs = p_args as StatusChangedEventArgs;
			Character character = p_sender as Character;
			if (statusChangedEventArgs == null || character == null)
			{
				return;
			}
			if (statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.CONDITIONS)
			{
				String text = String.Empty;
				ECondition storedCondition = m_storedCondition;
				if (storedCondition != ECondition.NONE)
				{
					String text2 = (character.Gender != EGender.FEMALE) ? "M" : "F";
					if (storedCondition == ECondition.STUNNED)
					{
						text = LocaManager.GetText("ACTION_LOG_CONDITION_REMOVED_KNOCKED_OUT_" + text2, character.Name);
					}
					else
					{
						text = LocaManager.GetText("ACTION_LOG_CONDITION_REMOVED_" + storedCondition.ToString() + "_" + text2, character.Name);
					}
				}
				else
				{
					if (character.ConditionHandler.GetVisibleCondition() == ECondition.NONE)
					{
						return;
					}
					String id = String.Empty;
					ECondition econdition;
					if (statusChangedEventArgs.Condition != ECondition.NONE)
					{
						econdition = statusChangedEventArgs.Condition;
					}
					else
					{
						econdition = character.ConditionHandler.GetVisibleCondition();
					}
					if (econdition == ECondition.STUNNED)
					{
						id = "ACTION_LOG_CONDITION_CHANGED_KNOCKED_OUT_" + ((character.Gender != EGender.FEMALE) ? "M" : "F");
					}
					else
					{
						id = String.Concat(new Object[]
						{
							"ACTION_LOG_CONDITION_CHANGED_",
							econdition,
							"_",
							(character.Gender != EGender.FEMALE) ? "M" : "F"
						});
					}
					text = LocaManager.GetText(id, character.Name);
				}
				if (String.IsNullOrEmpty(text))
				{
					return;
				}
				m_storedCondition = ECondition.NONE;
				GameMessage gameMessage = new GameMessage(text);
				if (character.ConditionHandler.GetVisibleCondition() == ECondition.DEAD)
				{
					gameMessage.m_conditionDead = true;
					m_importantMessages.Enqueue(gameMessage);
				}
				else
				{
					m_queuedMessages.Enqueue(gameMessage);
				}
			}
		}

		public void OnCharacterSkillLevelChanged(Object p_sender, EventArgs p_args)
		{
			SkillTierChangedEventArgs skillTierChangedEventArgs = p_args as SkillTierChangedEventArgs;
			if (skillTierChangedEventArgs == null)
			{
				return;
			}
			Int32 tier = (Int32)skillTierChangedEventArgs.ChangedSkill.Tier;
			if (tier == 0)
			{
				return;
			}
			if (IngameController.Instance.IsAnyScreenOpen())
			{
				return;
			}
			String text = LocaManager.GetText("SKILL_TIER_" + tier.ToString());
			String text2 = LocaManager.GetText(skillTierChangedEventArgs.ChangedSkill.Name);
			GameMessage item = new GameMessage(LocaManager.GetText("GAME_MESSAGE_NEW_SKILL_TIER_REACHED", skillTierChangedEventArgs.SkillOwner.Name, text, text2));
			m_queuedMessages.Enqueue(item);
		}

		private void OnCharacterRevived(Object p_sender, EventArgs p_args)
		{
			ResurrectFunction resurrectFunction = p_sender as ResurrectFunction;
			if (resurrectFunction == null)
			{
				return;
			}
			Character chara = resurrectFunction.Chara;
			String str = (chara.Gender != EGender.FEMALE) ? "_M" : "_F";
			String text = LocaManager.GetText("GAME_MESSAGE_CHARACTER_REVIVED" + str, chara.Name);
			GameMessage item = new GameMessage(text);
			m_queuedMessages.Enqueue(item);
		}

		private void OnPreconditionEval(Object p_sender, EventArgs p_args)
		{
			PreconditionEvaluateArgs preconditionEvaluateArgs = p_args as PreconditionEvaluateArgs;
			if (preconditionEvaluateArgs == null)
			{
				return;
			}
			if (preconditionEvaluateArgs.Passed || preconditionEvaluateArgs.Cancelled)
			{
				return;
			}
			String text = String.Empty;
			Boolean flag = false;
			Int32 num = 0;
			PartyCheckPrecondition partyCheckPrecondition = p_sender as PartyCheckPrecondition;
			if (partyCheckPrecondition != null && partyCheckPrecondition.Decision == EPreconditionDecision.NONE && partyCheckPrecondition.RequiredTokenID > 0)
			{
				flag = true;
				num = partyCheckPrecondition.RequiredTokenID;
			}
			Boolean flag2 = false;
			Boolean flag3 = false;
			Boolean flag4 = false;
			Position position = LegacyLogic.Instance.WorldManager.Party.Position;
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(position);
			IEnumerable<InteractiveObject> interactiveObjectIterator = slot.GetInteractiveObjectIterator();
			foreach (InteractiveObject interactiveObject in interactiveObjectIterator)
			{
				if (interactiveObject is Door || interactiveObject is Container || interactiveObject is Lever)
				{
					if (interactiveObject != m_lastInteractiveObject)
					{
						if (interactiveObject is Door)
						{
							Door door = (Door)interactiveObject;
							if (flag && m_lastInteractiveObject == door)
							{
								flag3 = true;
								break;
							}
							if (door.State == EInteractiveObjectState.DOOR_OPEN || !door.Enabled)
							{
								flag3 = true;
								break;
							}
							foreach (SpawnCommand spawnCommand in interactiveObject.Commands)
							{
								if (spawnCommand.Precondition != "NONE" && !spawnCommand.Precondition.Contains("SECRET_CHALLENGE") && !spawnCommand.Precondition.Contains("PLAIN"))
								{
									String[] array = spawnCommand.Precondition.Split(new Char[]
									{
										','
									});
									if (array[4] == String.Empty)
									{
										Int32 num2 = 0;
										if (array[5] != String.Empty && Int32.TryParse(array[5], out num2) && num2 > 0 && num2 != num)
										{
											flag4 = true;
											break;
										}
										if (flag && (m_lastInteractiveObject == null || m_lastInteractiveObject != door))
										{
											m_lastInteractiveObject = door;
										}
										flag2 = true;
										break;
									}
								}
							}
							text = LocaManager.GetText("DOOR_KEY_FAIL_TEXT");
						}
						else if (interactiveObject is Container)
						{
							if (interactiveObject.State == EInteractiveObjectState.CONTAINER_OPENED)
							{
								flag3 = true;
								break;
							}
							foreach (SpawnCommand spawnCommand2 in interactiveObject.Commands)
							{
								if (spawnCommand2.Type == EInteraction.OPEN_CONTAINER && spawnCommand2.Precondition != "NONE" && partyCheckPrecondition.RequiredTokenID > 0)
								{
									String[] array2 = spawnCommand2.Precondition.Split(new Char[]
									{
										','
									});
									if (array2[4] == String.Empty)
									{
										Int32 num3 = 0;
										if (array2[5] != String.Empty && Int32.TryParse(array2[5], out num3) && num3 > 0 && num3 != num)
										{
											flag4 = true;
											break;
										}
									}
									text = LocaManager.GetText("CHEST_KEY_FAIL_TEXT");
									flag3 = false;
									flag2 = true;
									flag4 = false;
									m_lastInteractiveObject = interactiveObject;
									break;
								}
							}
							if (!String.IsNullOrEmpty(text) || flag4)
							{
								break;
							}
						}
						else if (interactiveObject is Lever)
						{
							if (interactiveObject.State == EInteractiveObjectState.LEVER_DOWN)
							{
								flag3 = true;
								break;
							}
							foreach (SpawnCommand spawnCommand3 in interactiveObject.Commands)
							{
								if ((spawnCommand3.Type == EInteraction.LEVER_DOWN || spawnCommand3.Type == EInteraction.LEVER_UP) && spawnCommand3.Precondition != "NONE" && partyCheckPrecondition.RequiredTokenID > 0)
								{
									String[] array3 = spawnCommand3.Precondition.Split(new Char[]
									{
										','
									});
									if (array3[4] == String.Empty)
									{
										Int32 num4 = 0;
										if (array3[5] != String.Empty && Int32.TryParse(array3[5], out num4) && num4 > 0 && num4 != num)
										{
											flag4 = true;
											break;
										}
									}
									text = LocaManager.GetText("LEVER_GEAR_FAIL_TEXT");
									flag3 = false;
									flag2 = true;
									flag4 = false;
									m_lastInteractiveObject = interactiveObject;
									break;
								}
							}
							if (!String.IsNullOrEmpty(text) || flag4)
							{
								break;
							}
						}
					}
				}
			}
			if (flag3)
			{
				return;
			}
			if (!flag2)
			{
				return;
			}
			if (flag4)
			{
				return;
			}
			if (String.IsNullOrEmpty(text))
			{
				return;
			}
			if (m_lastInteractiveObject != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnInteractHotkeyPressed));
			}
			GameMessage item = new GameMessage(text);
			m_importantMessages.Enqueue(item);
		}

		private void OnPassiveObject(Object p_sender, EventArgs p_args)
		{
			InteractiveObjectListEventArgs interactiveObjectListEventArgs = (InteractiveObjectListEventArgs)p_args;
			if (interactiveObjectListEventArgs == null)
			{
				return;
			}
			List<InteractiveObject> objectList = interactiveObjectListEventArgs.ObjectList;
			if (objectList.Count == 0)
			{
				return;
			}
			Boolean flag = false;
			Boolean flag2 = false;
			Boolean flag3 = false;
			for (Int32 i = 0; i < objectList.Count; i++)
			{
				InteractiveObject interactiveObject = objectList[i];
				if (interactiveObject is NpcContainer)
				{
					flag = true;
					break;
				}
				if (!(interactiveObject is Door))
				{
					flag3 = true;
					break;
				}
				Door door = (Door)interactiveObject;
				if (door.State == EInteractiveObjectState.DOOR_OPEN)
				{
					flag2 = true;
					break;
				}
				for (Int32 j = 0; j < door.Commands.Count; j++)
				{
					if (door.Commands[j].Type == EInteraction.TOGGLE_DOOR || door.IsSecret)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (flag || flag2 || flag3)
			{
				return;
			}
			String text = LocaManager.GetText("DOOR_NO_INTERACTION_TEXT");
			GameMessage item = new GameMessage(text);
			m_queuedMessages.Enqueue(item);
		}

		private void OnInteractHotkeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			OnMoveEntity(null, EventArgs.Empty);
		}

		private void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnInteractHotkeyPressed));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
			m_lastInteractiveObject = null;
		}

		private void OnQuickSave(Object sender, EventArgs e)
		{
			GameMessage item = new GameMessage(LocaManager.GetText("GAME_MESSAGE_QUICKSAVE"));
			m_queuedMessages.Enqueue(item);
		}

		private void OnPartyTeleported(Object p_sender, EventArgs p_args)
		{
			GameMessage item = new GameMessage(LocaManager.GetText("GAME_MESSAGE_PARTY_USED_TELEPORTER"));
			m_queuedMessages.Enqueue(item);
		}

		private void OnBestiaryEntryChangedFakeMethod(Object p_sender, EventArgs p_args)
		{
			if (LegacyLogic.Instance.WorldManager.Party.HasAggro())
			{
				DelayedMessage delayedMessage = new DelayedMessage();
				delayedMessage.Sender = p_sender;
				delayedMessage.Args = p_args;
				delayedMessage.Type = EDelayedMessageType.BESTIARY;
				m_delayed.Enqueue(delayedMessage);
			}
			else
			{
				BestiaryEntryChanged(p_sender, p_args);
			}
		}

		private void BestiaryEntryChanged(Object p_sender, EventArgs p_args)
		{
			BestiaryEntryEventArgs bestiaryEntryEventArgs = p_args as BestiaryEntryEventArgs;
			if (bestiaryEntryEventArgs == null)
			{
				return;
			}
			String text = LocaManager.GetText(bestiaryEntryEventArgs.MonsterNameKey);
			String p_message = String.Empty;
			if (bestiaryEntryEventArgs.EntryState == BestiaryEntryEventArgs.EEntryState.ENTRY_NEW)
			{
				p_message = LocaManager.GetText("GAME_MESSAGE_BESTIARY_ENTRY_NEW", text);
			}
			else
			{
				p_message = LocaManager.GetText("GAME_MESSAGE_BESTIARY_ENTRY_CHANGED", text);
			}
			GameMessage item = new GameMessage(p_message);
			m_queuedMessages.Enqueue(item);
		}

		private void OnGameOver(Object p_sender, EventArgs p_args)
		{
			m_currentMessage = null;
			m_messageLabel.text = String.Empty;
			m_submessageLabel.text = String.Empty;
			m_messageLabel.alpha = 1f;
			m_submessageLabel.alpha = 0f;
			m_occupied = false;
			m_showSubLabel = false;
			m_queuedMessages.Clear();
			m_importantMessages.Clear();
		}

		public class GameMessage
		{
			public String m_message;

			public Single m_insertTime;

			public Single m_delay;

			public Boolean m_hasSubMessage;

			public Boolean m_conditionDead;

			public GameMessage(String p_message, Single p_delay)
			{
				m_message = p_message;
				m_delay = ((p_delay <= 0f) ? 3f : p_delay);
			}

			public GameMessage(String p_message)
			{
				m_message = p_message;
				m_delay = 3f;
			}
		}

		private class DelayedMessage
		{
			public Object Sender;

			public EventArgs Args;

			public EDelayedMessageType Type;

			public DelayedMessage()
			{
				Sender = null;
				Args = EventArgs.Empty;
				Type = EDelayedMessageType.NONE;
			}
		}

		private enum EDelayedMessageType
		{
			NONE,
			ITEM_BROKE,
			CHARACTER_STATUS_CHANGED,
			BESTIARY
		}
	}
}
