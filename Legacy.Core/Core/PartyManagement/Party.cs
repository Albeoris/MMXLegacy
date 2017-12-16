using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.Pathfinding;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.PartyManagement
{
	public class Party : MovingEntity, ISaveGameObject
	{
		public const Int32 CHARACTER_COUNT = 4;

		public const Int32 ACT_234_PRIVILEGE = 1001;

		public const Int32 ACT_234_TOKEN = 747;

		public const Int32 EXCLUSIVE_DUNGEON_PRIVILEGE = 1002;

		public const Int32 DLC_1_PRIVILEGE = 1003;

		public const Int32 PROMOTION_REWARD_PRIVILEGE = 1005;

		public const Int32 PROMOTION_REWARD_HELMET_ID = 50;

		private Character[] m_members;

		private Int32[] m_order;

		private PartyInventoryController m_inventory;

		private PartyInventoryController m_muleInventory;

		private PartyInventoryController m_activeInventory;

		private HiddenInventoryController[] m_hiddenInventory;

		private PartyBuffHandler m_buffs;

		private Int32 m_currentCharacter;

		private Int32 m_startSpawnerID = -1;

		private Character m_selectedCharacter;

		private Int32 m_gold;

		private Int32 m_supplies;

		private Boolean m_inCombat;

		private Int32 m_restCount;

		private Boolean m_isNpcRest;

		private Int32 m_exploreRange;

		private Boolean m_receivedUplayRelics;

		private Boolean m_receivedPromotionReward;

		private TokenHandler m_tokenHandler;

		private HirelingHandler m_hirelingHandler;

		private CharacterSelectRequest m_characterSelectLock;

		private Monster m_selectedMonster;

		private InteractiveObject m_selectedInteractiveObject;

		private Int32 m_nextSelectableInteractiveObject;

		private Monster[] m_latestSelectedMonster;

		private TriggerHelper m_monsterHitAnimationDone = new TriggerHelper();

		private TriggerHelper m_usedConsumable = new TriggerHelper();

		private TriggerHelper m_restingDone = new TriggerHelper();

		private EMapArea m_currentMapArea;

		private EMapArea m_lastMapArea;

		private List<KeyValuePair<Int32, Position>> m_hiddenMonsterPositions = new List<KeyValuePair<Int32, Position>>();

		private Character m_lastWhoWillCharacter;

		private Object m_lastBlockSubstitute;

		public Party() : this(0)
		{
		}

		public Party(Int32 p_spawnerID) : base(0, EObjectType.PARTY, p_spawnerID)
		{
			m_members = new Character[4];
			m_inventory = new PartyInventoryController(this);
			m_muleInventory = new PartyInventoryController(this);
			m_hiddenInventory = new HiddenInventoryController[5];
			for (Int32 i = 0; i < 5; i++)
			{
				m_hiddenInventory[i] = new HiddenInventoryController(this);
			}
			m_activeInventory = m_inventory;
			m_buffs = new PartyBuffHandler();
			m_tokenHandler = new TokenHandler(this);
			m_hirelingHandler = new HirelingHandler();
			m_latestSelectedMonster = new Monster[4];
			m_exploreRange = ConfigManager.Instance.Game.ExploreRange;
			m_order = new Int32[4];
			for (Int32 j = 0; j < 4; j++)
			{
				m_order[j] = j;
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_BUFF_ADDED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_BUFF_REMOVED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ANNOUNCE_LATE_MONSTER_SPAWN, new EventHandler(OnAnnounceLateMonsterSpawn));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHANGE_MAP, new EventHandler(OnChangeMap));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(OnSpawnMonster));
		}

		public Character LastWhoWillCharacter
		{
			get => m_lastWhoWillCharacter;
		    set => m_lastWhoWillCharacter = value;
		}

		public Object LastBlockSubstitute
		{
			get => m_lastBlockSubstitute;
		    set => m_lastBlockSubstitute = value;
		}

		public EMapArea CurrentMapArea
		{
			get => m_currentMapArea;
		    set
			{
				m_currentMapArea = value;
				if (m_currentMapArea != m_lastMapArea)
				{
					if (m_currentMapArea > EMapArea.SAFE_ZONE)
					{
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GAME_MESSAGE, new MapAreaEventArgs(m_currentMapArea));
					}
					m_lastMapArea = m_currentMapArea;
				}
			}
		}

		public TriggerHelper MonsterHitAnimationDone => m_monsterHitAnimationDone;

	    public TriggerHelper UsedConsumable => m_usedConsumable;

	    public Boolean IsNpcRest
		{
			get => m_isNpcRest;
	        set => m_isNpcRest = value;
	    }

		public TriggerHelper RestingDone => m_restingDone;

	    public Int32 RestCount => m_restCount;

	    public Int32 StartSpawnerID
		{
			get => m_startSpawnerID;
	        set => m_startSpawnerID = value;
	    }

		public PartyInventoryController Inventory => m_inventory;

	    public PartyInventoryController MuleInventory => m_muleInventory;

	    public PartyInventoryController ActiveInventory
		{
			get => m_activeInventory;
	        set => m_activeInventory = value;
	    }

		public HiddenInventoryController GetHiddenInventory(Int32 p_idx)
		{
			return m_hiddenInventory[p_idx];
		}

		public PartyBuffHandler Buffs => m_buffs;

	    public TokenHandler TokenHandler => m_tokenHandler;

	    public Int32 CurrentCharacter => m_currentCharacter;

	    public Character SelectedCharacter => m_selectedCharacter;

	    public Int32 CurrentCharacterIndexByOrder
		{
			get
			{
				for (Int32 i = 0; i < 4; i++)
				{
					if (m_order[i] == m_currentCharacter)
					{
						return i;
					}
				}
				return 0;
			}
		}

		public override ESize Size => ESize.BIG;

	    public Character[] Members => m_members;

	    public HirelingHandler HirelingHandler => m_hirelingHandler;

	    public Boolean InCombat
		{
			get => m_inCombat;
	        set
			{
				if (m_inCombat != value)
				{
					m_inCombat = value;
					if (m_inCombat)
					{
						LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.MELEE);
					}
				}
			}
		}

		public Int32 ExploreRange
		{
			get
			{
				NpcEffect npcEffect;
				if (HirelingHandler.HasEffect(ETargetCondition.HIRE_MMEXPLODISTANCE, out npcEffect) && npcEffect.TargetEffect != ETargetCondition.NONE)
				{
					return (Int32)Math.Round(m_exploreRange + npcEffect.EffectValue, MidpointRounding.AwayFromZero);
				}
				return m_exploreRange;
			}
		}

		public Monster SelectedMonster
		{
			get => m_selectedMonster;
		    set
			{
				if (m_selectedMonster != value)
				{
					m_selectedMonster = value;
					for (Int32 i = 0; i < m_members.Length; i++)
					{
						if (m_members[i] != null)
						{
							m_members[i].SelectedMonster = value;
						}
					}
					if (value != null)
					{
						BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(value, value.Position);
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTER_SELECTED, p_eventArgs);
						m_latestSelectedMonster[(Int32)Direction] = value;
					}
					else
					{
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTER_ALL_SELECTIONS_REMOVED, EventArgs.Empty);
					}
				}
			}
		}

		public void SelectMonsterWithoutEvent(Monster p_monster)
		{
			if (m_selectedMonster != p_monster)
			{
				m_selectedMonster = p_monster;
				for (Int32 i = 0; i < m_members.Length; i++)
				{
					if (m_members[i] != null)
					{
						m_members[i].SelectedMonster = p_monster;
					}
				}
			}
		}

		public InteractiveObject SelectedInteractiveObject
		{
			get => m_selectedInteractiveObject;
		    set
			{
				if (m_selectedInteractiveObject != value)
				{
					m_selectedInteractiveObject = value;
					if (value != null)
					{
						BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(value, value.Position);
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INTERACTIVE_OBJECT_SELECTED, p_eventArgs);
					}
					else
					{
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INTERACTIVE_OBJECT_ALL_SELECTIONS_REMOVED, EventArgs.Empty);
					}
				}
			}
		}

		public Int32 Gold => m_gold;

	    public Int32 Supplies => m_supplies;

	    public Boolean CanRest => m_supplies > 0 && !InCombat && !HasAggro() && !InNpcConversation();

	    public Int32 AttackableCharacterCount
		{
			get
			{
				Int32 num = 0;
				for (Int32 i = 0; i < m_members.Length; i++)
				{
					if (m_members[i] != null && !m_members[i].ConditionHandler.HasOneCondition(ECondition.DEAD))
					{
						num++;
					}
				}
				return num;
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_selectedCharacter = null;
			m_selectedMonster = null;
			m_inventory = null;
			m_muleInventory = null;
			m_activeInventory = null;
			m_hiddenInventory = null;
			if (m_buffs != null)
			{
				m_buffs.RemoveAllBuffs();
			}
			m_buffs = null;
			m_tokenHandler = null;
			for (Int32 i = 0; i < 4; i++)
			{
				if (m_members[i] != null)
				{
					m_members[i].Destroy();
					m_members[i] = null;
				}
			}
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_ADDED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_REMOVED, new EventHandler(OnPartyBuffsChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ANNOUNCE_LATE_MONSTER_SPAWN, new EventHandler(OnAnnounceLateMonsterSpawn));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHANGE_MAP, new EventHandler(OnChangeMap));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(OnSpawnMonster));
		}

		public void ChangeCharacterOrder(Character p_character, Int32 p_newIndex)
		{
			Int32 num = 0;
			for (Int32 i = 0; i < 4; i++)
			{
				if (m_order[i] == p_character.Index)
				{
					num = i;
					break;
				}
			}
			if (num == p_newIndex)
			{
				return;
			}
			m_order[num] = m_order[p_newIndex];
			m_order[p_newIndex] = p_character.Index;
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_ORDER_CHANGED, EventArgs.Empty);
		}

		public override void ApplyDamages(AttackResult p_result, Object p_source)
		{
		}

		public IInventory GetOtherInventory(IInventory p_inventory)
		{
			Boolean flag = HirelingHandler.HasEffect(ETargetCondition.HIRE_MULE);
			if (p_inventory == Inventory && flag)
			{
				return MuleInventory;
			}
			if (p_inventory == MuleInventory)
			{
				return Inventory;
			}
			return p_inventory;
		}

		public void ChangeGold(Int32 p_delta)
		{
			if (p_delta > 0)
			{
				Single sharePricePercent = m_hirelingHandler.SharePricePercent;
				if (sharePricePercent != 0f)
				{
					p_delta -= Math.Max((Int32)(m_hirelingHandler.SharePricePercent * p_delta), 1);
				}
			}
			Int32 num = m_gold + p_delta;
			if (num < 0)
			{
				num = 0;
			}
			if (num != m_gold)
			{
				m_gold = num;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_RESOURCES_CHANGED, new ResourcesChangedEventArgs(ResourcesChangedEventArgs.EResourceType.GOLD));
			}
		}

		public void ConsumeSupply(Int32 p_amount)
		{
			Int32 num = m_supplies - p_amount;
			if (num < 0)
			{
				num = 0;
			}
			if (num != m_supplies)
			{
				m_supplies = num;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_RESOURCES_CHANGED, new ResourcesChangedEventArgs(ResourcesChangedEventArgs.EResourceType.SUPPLIES));
			}
		}

		public void SetSupplies(Int32 p_amount)
		{
			m_supplies = Math.Max(m_supplies, p_amount);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_RESOURCES_CHANGED, new ResourcesChangedEventArgs(ResourcesChangedEventArgs.EResourceType.SUPPLIES));
		}

		public void AddMember(Int32 p_idx, Character p_member)
		{
			if (p_idx < 0 || p_idx >= 4)
			{
				return;
			}
			m_members[p_idx] = p_member;
			p_member.Index = p_idx;
			p_member.FightHandler.BlockSubstitute += OnBlockSubstitute;
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				if (SelectCharacter(i))
				{
					break;
				}
			}
		}

		public void RemoveMember(Int32 p_idx)
		{
			if (p_idx < 0 || p_idx >= 4)
			{
				return;
			}
			m_members[p_idx] = null;
		}

		public Character GetMember(Int32 p_idx)
		{
			return m_members[p_idx];
		}

		public Character GetMemberByOrder(Int32 p_idx)
		{
			return m_members[m_order[p_idx]];
		}

		public Int32 GetMemberIndexByOrder(Int32 p_character)
		{
			for (Int32 i = 0; i < 4; i++)
			{
				if (m_order[i] == p_character)
				{
					return i;
				}
			}
			return 0;
		}

		public Int32 GetOrder(Int32 p_idx)
		{
			return m_order[p_idx];
		}

		public Int32 GetMemberIndex(Character chara)
		{
			if (chara == null)
			{
				return -1;
			}
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				if (m_members[i] == chara)
				{
					return i;
				}
			}
			return -1;
		}

		public void StartTurn()
		{
			AutoSelectMonster();
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				if (m_members[i] != null)
				{
					m_members[i].StartTurn();
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_RESET_FIGHT_ROUND, EventArgs.Empty);
		}

		public Boolean SelectCharacter(Int32 p_idx)
		{
			if (m_characterSelectLock != null)
			{
				if (p_idx != m_currentCharacter)
				{
					m_characterSelectLock(p_idx);
				}
				return false;
			}
			return ConfirmCharacterSelect(p_idx);
		}

		public Boolean ConfirmCharacterSelect(Int32 p_idx)
		{
			if (p_idx < 0 || p_idx >= 4 || m_members[p_idx] == null)
			{
				return false;
			}
			if (m_members[p_idx].ConditionHandler.CantDoAnything() && m_inCombat)
			{
				return false;
			}
			m_currentCharacter = p_idx;
			m_selectedCharacter = m_members[m_currentCharacter];
			LegacyLogic.Instance.EventManager.InvokeEvent(m_selectedCharacter, EEventType.CHARACTER_SELECTED, EventArgs.Empty);
			if (LegacyLogic.Instance.ConversationManager.IsOpen && !LegacyLogic.Instance.ConversationManager.CurrentNpc.TradingInventory.IsTrading)
			{
				LegacyLogic.Instance.ConversationManager.UpdateCurrentDialog();
			}
			return true;
		}

		public void LockCharacterSelect(CharacterSelectRequest m_characterSelectRequestCallback)
		{
			m_characterSelectLock = m_characterSelectRequestCallback;
		}

		public void ReleaseCharacterSelectLock()
		{
			m_characterSelectLock = null;
		}

		public void FinishPartyRound()
		{
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				m_members[i].EndTurn();
			}
		}

		public void FinishCurrentCharacterTurn()
		{
			if (HasAggro())
			{
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.TURNS);
				SelectedCharacter.EndTurn();
				if (FightRoundDone())
				{
					ForceSelectNextCharacter();
				}
				else
				{
					SelectNextAvailableCharacter();
				}
			}
		}

		public List<Monster> GetAllMonstersOnTargetTile(Monster p_target)
		{
			List<Monster> list = new List<Monster>();
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			GridSlot slot = grid.GetSlot(p_target.Position);
			IList<MovingEntity> entities = slot.Entities;
			for (Int32 i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Monster)
				{
					list.Add((Monster)entities[i]);
				}
			}
			return list;
		}

		public void AutoSelectMonster()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			GridSlot slot = grid.GetSlot(Position);
			Position right = slot.Position + Direction;
			if (SelectedMonster == null || SelectedMonster.Position != right || SelectedMonster.CurrentHealth <= 0)
			{
				SelectedMonster = null;
				GetLeftMonsterOnTargetField();
			}
		}

		private void GetLeftMonsterOnTargetField()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			GridSlot gridSlot = null;
			GridSlot slot = grid.GetSlot(Position);
			if (slot != null)
			{
				gridSlot = slot.GetConnection(Direction, false);
				if (gridSlot == null)
				{
					gridSlot = slot.GetConnectionForRanged(Direction, false);
				}
			}
			if (gridSlot == null)
			{
				return;
			}
			IList<MovingEntity> entities = gridSlot.Entities;
			for (Int32 i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Monster)
				{
					if (m_latestSelectedMonster[(Int32)Direction] == (Monster)entities[i])
					{
						SelectedMonster = (Monster)entities[i];
						break;
					}
					SelectedMonster = (Monster)entities[i];
				}
			}
		}

		public void AutoSelectNextMonster()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			GridSlot gridSlot = null;
			GridSlot slot = grid.GetSlot(Position);
			if (slot != null)
			{
				gridSlot = slot.GetConnection(Direction, false);
				if (gridSlot == null)
				{
					gridSlot = slot.GetConnectionForRanged(Direction, false);
				}
			}
			if (gridSlot == null)
			{
				return;
			}
			IList<MovingEntity> entities = gridSlot.Entities;
			Boolean flag = false;
			for (Int32 i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Monster)
				{
					if (flag)
					{
						SelectedMonster = (Monster)entities[i];
						return;
					}
					if (m_latestSelectedMonster[(Int32)Direction] == (Monster)entities[i])
					{
						flag = true;
					}
				}
			}
			for (Int32 j = 0; j < entities.Count; j++)
			{
				if (entities[j] is Monster)
				{
					if (flag)
					{
						SelectedMonster = (Monster)entities[j];
						return;
					}
					if (m_latestSelectedMonster[(Int32)Direction] == (Monster)entities[j])
					{
						return;
					}
				}
			}
		}

		public void SelectNextInteractiveObject()
		{
			SelectedInteractiveObject = null;
			m_nextSelectableInteractiveObject++;
			AutoSelectInteractiveObject();
		}

		public void AutoSelectInteractiveObject()
		{
			if (LegacyLogic.Instance.ConversationManager.IsOpen)
			{
				SelectedInteractiveObject = null;
			}
			else if (SelectedInteractiveObject == null || !SelectedInteractiveObject.IsClickableViewState)
			{
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				GridSlot slot = grid.GetSlot(Position);
				if (slot != null)
				{
					List<InteractiveObject> interactiveObjectsWithClickableViewState = slot.GetInteractiveObjectsWithClickableViewState();
					if (interactiveObjectsWithClickableViewState.Count > 0)
					{
						SelectedInteractiveObject = interactiveObjectsWithClickableViewState[m_nextSelectableInteractiveObject % interactiveObjectsWithClickableViewState.Count];
					}
					else
					{
						SelectedInteractiveObject = null;
					}
				}
				else
				{
					SelectedInteractiveObject = null;
				}
			}
		}

		public Monster GetRandomMonsterInLineOfSight()
		{
			return GetRandomMonsterInLineOfSight(false);
		}

		public Monster GetRandomMonsterInLineOfSight(Boolean p_forRangedAttack)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			if (grid != null)
			{
				GridSlot slot = grid.GetSlot(Position);
				return GetNextMonsterInDirection(slot, p_forRangedAttack);
			}
			return null;
		}

		public Monster GetNextRandomMonsterInLineOfSight(Monster p_monster)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			GridSlot slot = grid.GetSlot(p_monster.Position);
			return GetNextMonsterInDirection(slot, false);
		}

		private Monster GetNextMonsterInDirection(GridSlot p_slot, Boolean p_forRangedAttack)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			GridSlot visitedSlotInDirection = grid.GetVisitedSlotInDirection(p_slot, Direction, p_forRangedAttack);
			if (visitedSlotInDirection != null)
			{
				IList<MovingEntity> entities = visitedSlotInDirection.Entities;
				for (Int32 i = 0; i < entities.Count; i++)
				{
					if (entities[i] is Monster)
					{
					}
				}
				Int32 index = Random.Range(0, entities.Count);
				while (!(entities[index] is Monster))
				{
					index = Random.Range(0, entities.Count);
				}
				return (Monster)entities[index];
			}
			return null;
		}

		internal void SelectNextAvailableCharacter()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < 4; i++)
			{
				if (m_order[i] == CurrentCharacter)
				{
					num = i;
					break;
				}
			}
			for (Int32 j = 1; j <= 4; j++)
			{
				Int32 num2 = m_order[(num + j) % 4];
				if (m_members[num2] != null && !m_members[num2].DoneTurn && !m_members[num2].ConditionHandler.CantDoAnything())
				{
					SelectCharacter(num2);
					if (m_selectedCharacter.HealthPoints / (Single)m_selectedCharacter.MaximumHealthPoints < 0.25f)
					{
						LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.POTIONS);
					}
					if (m_selectedCharacter.ManaPoints / (Single)m_selectedCharacter.MaximumManaPoints < 0.25f)
					{
						LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.POTIONS);
					}
					break;
				}
			}
			AutoSelectMonster();
		}

		internal void ForceSelectNextCharacter()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < 4; i++)
			{
				if (m_order[i] == CurrentCharacter)
				{
					num = i;
					break;
				}
			}
			for (Int32 j = 1; j <= 4; j++)
			{
				Int32 num2 = m_order[(num + j) % 4];
				if (m_members[num2] != null && !m_members[num2].ConditionHandler.CantDoAnything() && SelectCharacter(num2))
				{
					break;
				}
			}
			AutoSelectMonster();
		}

		private void OnCharacterStatusChanged(Object sender, EventArgs e)
		{
			Character character = sender as Character;
			StatusChangedEventArgs statusChangedEventArgs = e as StatusChangedEventArgs;
			if (character != null && character == m_selectedCharacter && statusChangedEventArgs != null && statusChangedEventArgs.BecameUnableToDoAnything)
			{
				for (Int32 i = 0; i < m_members.Length; i++)
				{
					if (SelectCharacter(m_order[i]))
					{
						break;
					}
				}
			}
		}

		public List<Character> GetFightingCharacters()
		{
			List<Character> list = new List<Character>();
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				if (m_members[i] != null && !m_members[i].ConditionHandler.HasOneCondition(ECondition.DEAD) && !m_members[i].ConditionHandler.HasOneCondition(ECondition.UNCONSCIOUS))
				{
					list.Add(m_members[i]);
				}
			}
			return list;
		}

		public List<Character> GetCharactersAlive()
		{
			List<Character> list = new List<Character>();
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				if (m_members[i] != null && !m_members[i].ConditionHandler.HasCondition(ECondition.DEAD))
				{
					list.Add(m_members[i]);
				}
			}
			return list;
		}

		public Int32 CountCharactersWithRangedWeapons()
		{
			List<Character> fightingCharacters = GetFightingCharacters();
			Int32 num = 0;
			for (Int32 i = 0; i < fightingCharacters.Count; i++)
			{
				if (fightingCharacters[i].Equipment.Equipment.GetItemAt(2) != null)
				{
					num++;
				}
			}
			return num;
		}

		public Boolean FightRoundDone()
		{
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				if (m_members[i] != null && !m_members[i].DoneTurn && !m_members[i].ConditionHandler.CantDoAnything())
				{
					return false;
				}
			}
			return true;
		}

		public Boolean FightRoundStarted()
		{
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				if (m_members[i] != null && m_members[i].DoneTurn)
				{
					return true;
				}
			}
			return false;
		}

		public void DoRest()
		{
			Boolean flag = HirelingHandler.FreeRest();
			LegacyLogic.Instance.GameTime.UpdateTime(ConfigManager.Instance.Game.MinutesPerRest, ETimeChangeReason.Resting);
			m_restCount++;
			Int32 num = 0;
			List<Character> list = new List<Character>();
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				if (m_members[i] != null)
				{
					m_members[i].ConditionHandler.RemoveCondition(ECondition.UNCONSCIOUS);
					m_members[i].ConditionHandler.RemoveCondition(ECondition.WEAK);
					m_members[i].ConditionHandler.RemoveCondition(ECondition.PARALYZED);
					m_members[i].ConditionHandler.RemoveCondition(ECondition.CONFUSED);
					m_members[i].ConditionHandler.RemoveCondition(ECondition.SLEEPING);
					m_members[i].ConditionHandler.RemoveCondition(ECondition.STUNNED);
					if (!m_members[i].ConditionHandler.CantDoAnything())
					{
						list.Add(m_members[i]);
					}
				}
			}
			foreach (Character character in list)
			{
				character.DoRest();
			}
			if (!flag && !m_isNpcRest)
			{
				ConsumeSupply(1);
				num++;
			}
			if (HirelingHandler.HasEffect(ETargetCondition.HIRE_RESTWELL))
			{
				m_buffs.AddBuff(EPartyBuffs.WELL_RESTED, 1f);
			}
			m_restingDone.Trigger();
			if (num > 0)
			{
				RestEntryEventArgs p_args = new RestEntryEventArgs(num);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_isNpcRest = false;
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_RESTED, EventArgs.Empty);
		}

		public Boolean InNpcConversation()
		{
			return LegacyLogic.Instance.ConversationManager.CurrentNpc != null;
		}

		public Boolean HasAggro()
		{
			foreach (Monster monster in LegacyLogic.Instance.WorldManager.GetObjectsByTypeIterator<Monster>())
			{
				if (monster.IsAggro)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean IsSensingDanger()
		{
			if (!HasDangerSense())
			{
				return false;
			}
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			GridSlot slot = grid.GetSlot(Position);
			Int32 monsterVisibilityRangeWithDangerSense = ConfigManager.Instance.Game.MonsterVisibilityRangeWithDangerSense;
			Int32 num = monsterVisibilityRangeWithDangerSense * monsterVisibilityRangeWithDangerSense;
			foreach (KeyValuePair<Int32, Position> keyValuePair in m_hiddenMonsterPositions)
			{
				if (Position.DistanceSquared(keyValuePair.Value, Position) <= num)
				{
					return true;
				}
			}
			foreach (Monster monster in LegacyLogic.Instance.WorldManager.GetObjectsByTypeIterator<Monster>())
			{
				GridSlot slot2 = grid.GetSlot(monster.Position);
				Int32 num2 = AStarHelper<GridSlot>.Calculate(slot2, slot, monsterVisibilityRangeWithDangerSense, null, false, null);
				if (num2 > 0)
				{
					return true;
				}
			}
			return false;
		}

		private void OnAnnounceLateMonsterSpawn(Object p_sender, EventArgs p_args)
		{
			AnnounceLateMonsterSpawnArgs announceLateMonsterSpawnArgs = (AnnounceLateMonsterSpawnArgs)p_args;
			m_hiddenMonsterPositions.Add(new KeyValuePair<Int32, Position>(announceLateMonsterSpawnArgs.SpawnerID, announceLateMonsterSpawnArgs.MonsterPosition));
		}

		private void OnChangeMap(Object p_sender, EventArgs p_args)
		{
			m_hiddenMonsterPositions.Clear();
		}

		private void OnSpawnMonster(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			if (baseObjectEventArgs.Object is Monster)
			{
				m_hiddenMonsterPositions.Remove(new KeyValuePair<Int32, Position>(baseObjectEventArgs.Object.SpawnerID, baseObjectEventArgs.Position));
			}
		}

		private void OnMonsterDied(Object sender, EventArgs e)
		{
			foreach (Character character in m_members)
			{
				if (character != null)
				{
					character.ConditionHandler.FlushActionLog();
					character.ConditionHandler.FlushDelayedActionLog();
				}
			}
			if (!HasAggro())
			{
				Boolean flag = false;
				for (Int32 j = 0; j < 4; j++)
				{
					Character memberByOrder = GetMemberByOrder(j);
					if (memberByOrder != null)
					{
						memberByOrder.DoneTurn = true;
						if (!flag && !memberByOrder.ConditionHandler.CantDoAnything())
						{
							SelectCharacter(m_order[j]);
							flag = true;
						}
					}
				}
			}
		}

		private Boolean OnBlockSubstitute()
		{
			Int32 num = Random.Range(0, m_members.Length);
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				Character character = m_members[(i + num) % m_members.Length];
				if (character.SkillHandler.CanSubstituteBlock())
				{
					Boolean flag = character.FightHandler.TestBlock(false, false);
					if (flag)
					{
						m_lastBlockSubstitute = character;
					}
					return flag;
				}
			}
			return false;
		}

		private void OnPartyBuffsChanged(Object sender, EventArgs e)
		{
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				Character member = GetMember(i);
				if (member != null)
				{
					member.CalculateCurrentAttributes();
				}
			}
		}

		public Boolean HasSpotSecrets()
		{
			return m_tokenHandler.GetTokens(1) > 0 || m_buffs.HasBuff(EPartyBuffs.WHISPERING_SHADOWS) || HirelingHandler.HasEffect(ETargetCondition.HIRE_SPOTSECRETS) || HirelingHandler.HasEffect(ETargetCondition.HIRE_ENHSPOTSECRETS);
		}

		public Boolean HasDangerSense()
		{
			return m_tokenHandler.GetTokens(3) > 0 || m_buffs.HasBuff(EPartyBuffs.DANGER_SENSE) || HirelingHandler.HasEffect(ETargetCondition.HIRE_DANGERSENSE);
		}

		public Boolean HasClairvoyance()
		{
			return m_tokenHandler.GetTokens(2) > 0 || m_buffs.HasBuff(EPartyBuffs.CLAIRVOYANCE) || HirelingHandler.HasEffect(ETargetCondition.HIRE_CLAIRVOYANCE) || HirelingHandler.HasEffect(ETargetCondition.HIRE_ENHCLAIRVOYANCE);
		}

		public Boolean HasEnchClairvoyance()
		{
			return HirelingHandler.HasEffect(ETargetCondition.HIRE_ENHCLAIRVOYANCE);
		}

		public Boolean HasEnchSpotSecrets()
		{
			return HirelingHandler.HasEffect(ETargetCondition.HIRE_ENHSPOTSECRETS);
		}

		public override Boolean CanPassTerrain(ETerrainType p_type)
		{
			return (p_type & ETerrainType.BLOCKED) == ETerrainType.NONE && ((p_type & ETerrainType.WATER) == ETerrainType.NONE || m_tokenHandler.GetTokens(4) != 0) && ((p_type & ETerrainType.ROUGH) == ETerrainType.NONE || m_tokenHandler.GetTokens(5) != 0) && ((p_type & ETerrainType.FOREST) == ETerrainType.NONE || m_tokenHandler.GetTokens(6) != 0) && (p_type & ETerrainType.HAZARDOUS) == ETerrainType.NONE && (p_type & ETerrainType.LAVA) == ETerrainType.NONE && (p_type & ETerrainType.SEE_THROUGH) == ETerrainType.NONE && (p_type & ETerrainType.FLY_THROUGH) == ETerrainType.NONE && (p_type & ETerrainType.SHOOT_THROUGH) == ETerrainType.NONE;
		}

		public Potion GetBestPotion(EPotionType p_type, Potion p_previous)
		{
			Potion potion = Inventory.GetBestPotion(p_type);
			Potion bestPotion = MuleInventory.GetBestPotion(p_type);
			if (potion == null || (bestPotion != null && bestPotion.Value > potion.Value))
			{
				potion = bestPotion;
			}
			if (potion != null && (p_previous == null || p_previous.Counter == 0 || potion.Value > p_previous.Value))
			{
				Potion potion2 = ItemFactory.CreateItem<Potion>(potion.StaticId);
				potion2.Counter = GetTotalAmountOfConsumable(potion2);
				return potion2;
			}
			if (p_previous != null)
			{
				p_previous.Counter = GetTotalAmountOfConsumable(p_previous);
			}
			return p_previous;
		}

		public InventorySlotRef GetConsumableSlot(Consumable p_consumable)
		{
			Int32 consumableSlot = Inventory.GetConsumableSlot(p_consumable);
			if (consumableSlot >= 0)
			{
				return new InventorySlotRef(Inventory, consumableSlot);
			}
			consumableSlot = MuleInventory.GetConsumableSlot(p_consumable);
			return new InventorySlotRef(MuleInventory, consumableSlot);
		}

		public Int32 GetTotalAmountOfConsumable(Consumable p_consumable)
		{
			return Inventory.GetTotalAmountOfConsumable(p_consumable) + MuleInventory.GetTotalAmountOfConsumable(p_consumable);
		}

		public void CheckUnlockUPlayPrivilegesRewards()
		{
			if (!m_receivedUplayRelics && LegacyLogic.Instance.ServiceWrapper.IsRewardAvailable(3))
			{
				Int32 num = Inventory.GetMaximumItemCount() - Inventory.GetCurrentItemCount();
				Boolean flag = LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HasEffect(ETargetCondition.HIRE_MULE);
				if (flag)
				{
					num += MuleInventory.GetMaximumItemCount() - MuleInventory.GetCurrentItemCount();
				}
				if (num >= 4)
				{
					m_receivedUplayRelics = true;
					AddItem(ItemFactory.CreateItem<MeleeWeapon>(54));
					AddItem(ItemFactory.CreateItem<MagicFocus>(50));
					AddItem(ItemFactory.CreateItem<Armor>(62));
					AddItem(ItemFactory.CreateItem<Armor>(94));
				}
			}
			if (!m_receivedPromotionReward && LegacyLogic.Instance.ServiceWrapper.IsPrivilegeAvailable(1005))
			{
				Int32 num2 = Inventory.GetMaximumItemCount() - Inventory.GetCurrentItemCount();
				Boolean flag2 = LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HasEffect(ETargetCondition.HIRE_MULE);
				if (flag2)
				{
					num2 += MuleInventory.GetMaximumItemCount() - MuleInventory.GetCurrentItemCount();
				}
				if (num2 >= 1)
				{
					m_receivedPromotionReward = true;
					AddItem(ItemFactory.CreateItem<Armor>(50));
				}
			}
			if (TokenHandler.GetTokens(747) == 0 && LegacyLogic.Instance.ServiceWrapper.IsPrivilegeAvailable(1001))
			{
				TokenHandler.AddToken(747);
			}
		}

		public void AddItem(BaseItem p_item)
		{
			IInventory inventory = Inventory;
			if (!inventory.CanAddItem(p_item))
			{
				inventory = GetOtherInventory(inventory);
			}
			inventory.AddItem(p_item);
		}

		public void UnlockAdvancedClass(ETokenID p_id)
		{
			for (Int32 i = 0; i < m_members.Length; i++)
			{
				if (m_members[i] != null)
				{
					m_members[i].TryUnlockAdvancedClass(p_id);
				}
			}
		}

		internal void PushedOntoHazardousTerrain()
		{
			foreach (Character character in Members)
			{
				character.ConditionHandler.AddCondition(ECondition.DEAD);
				character.ChangeHP(-character.MaximumHealthPoints);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_START_FALLING, EventArgs.Empty);
			LegacyLogic.Instance.UpdateManager.PartyTurnActor.CheckGameOver(false);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("StaticID", StaticID);
			p_data.Set<Int32>("SpawnerID", SpawnerID);
			p_data.Set<Int32>("MemberCount", m_members.Length);
			p_data.Set<Int32>("SelectedCharacter", m_currentCharacter);
			SaveGameData saveGameData = new SaveGameData("Inventory");
			m_inventory.Save(saveGameData);
			SaveGameData saveGameData2 = new SaveGameData("MuleInventory");
			m_muleInventory.Save(saveGameData2);
			for (Int32 i = 0; i < 5; i++)
			{
				SaveGameData saveGameData3 = new SaveGameData("HiddenInventoryChar" + i.ToString());
				m_hiddenInventory[i].Save(saveGameData3);
				p_data.Set<SaveGameData>(saveGameData3.ID, saveGameData3);
			}
			p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			p_data.Set<SaveGameData>(saveGameData2.ID, saveGameData2);
			SaveGameData saveGameData4 = new SaveGameData("Buffs");
			m_buffs.Save(saveGameData4);
			p_data.Set<SaveGameData>(saveGameData4.ID, saveGameData4);
			for (Int32 j = 0; j < 4; j++)
			{
				p_data.Set<Int32>("Order" + j, m_order[j]);
			}
			for (Int32 k = 0; k < m_members.Length; k++)
			{
				p_data.Set<Boolean>("CharacterExists" + k, m_members[k] != null);
				if (m_members[k] != null)
				{
					SaveGameData saveGameData5 = new SaveGameData("Character" + k);
					m_members[k].Save(saveGameData5);
					p_data.Set<SaveGameData>(saveGameData5.ID, saveGameData5);
				}
			}
			p_data.Set<Int32>("Gold", m_gold);
			p_data.Set<Int32>("Supplies", m_supplies);
			p_data.Set<Int32>("PositionX", Position.X);
			p_data.Set<Int32>("PositionY", Position.Y);
			p_data.Set<Int32>("Direction", (Int32)Direction);
			p_data.Set<Single>("Height", Height);
			p_data.Set<Int32>("RestCount", m_restCount);
			p_data.Set<Boolean>("ReceivedUPlayRelics", m_receivedUplayRelics);
			p_data.Set<Boolean>("ReceivedPromotionReward", m_receivedPromotionReward);
			p_data.Set<EMapArea>("CurrentMapArea", m_currentMapArea);
			SaveGameData saveGameData6 = new SaveGameData("Tokens");
			m_tokenHandler.Save(saveGameData6);
			p_data.Set<SaveGameData>(saveGameData6.ID, saveGameData6);
			SaveGameData saveGameData7 = new SaveGameData("Hirelings");
			m_hirelingHandler.Save(saveGameData7);
			p_data.Set<SaveGameData>(saveGameData7.ID, saveGameData7);
		}

		public void Load(SaveGameData p_data)
		{
			Int32 p_staticID = p_data.Get<Int32>("StaticID", 1);
			Int32 p_spawnerID = p_data.Get<Int32>("SpawnerID", 0);
			Int32 num = p_data.Get<Int32>("MemberCount", 0);
			SaveGameData saveGameData = p_data.Get<SaveGameData>("Inventory", null);
			if (saveGameData != null)
			{
				m_inventory.Load(saveGameData);
			}
			SaveGameData saveGameData2 = p_data.Get<SaveGameData>("MuleInventory", null);
			if (saveGameData2 != null)
			{
				m_muleInventory.Load(saveGameData2);
			}
			for (Int32 i = 0; i < 5; i++)
			{
				SaveGameData saveGameData3 = p_data.Get<SaveGameData>("HiddenInventoryChar" + i.ToString(), null);
				if (saveGameData3 != null)
				{
					m_hiddenInventory[i].Load(saveGameData3);
				}
			}
			SaveGameData saveGameData4 = p_data.Get<SaveGameData>("Buffs", null);
			if (saveGameData4 != null)
			{
				m_buffs.Load(saveGameData4);
			}
			for (Int32 j = 0; j < 4; j++)
			{
				m_order[j] = p_data.Get<Int32>("Order" + j, j);
			}
			for (Int32 k = 0; k < num; k++)
			{
				if (!p_data.Get<Boolean>("CharacterExists" + k, false))
				{
					m_members[k] = null;
				}
				else
				{
					Character character = new Character(m_buffs);
					SaveGameData saveGameData5 = p_data.Get<SaveGameData>("Character" + k, null);
					if (saveGameData5 != null)
					{
						character.Load(saveGameData5);
						character.Index = k;
						character.CalculateCurrentAttributes();
						m_members[k] = character;
					}
				}
			}
			m_selectedCharacter = m_members[0];
			m_gold = p_data.Get<Int32>("Gold", 0);
			m_supplies = p_data.Get<Int32>("Supplies", 0);
			Position = new Position(p_data.Get<Int32>("PositionX", 0), p_data.Get<Int32>("PositionY", 0));
			Direction = (EDirection)p_data.Get<Int32>("Direction", 0);
			Height = p_data.Get<Single>("Height", 0f);
			m_restCount = p_data.Get<Int32>("RestCount", 0);
			m_receivedUplayRelics = p_data.Get<Boolean>("ReceivedUPlayRelics", false);
			m_receivedPromotionReward = p_data.Get<Boolean>("ReceivedPromotionReward", false);
			m_currentMapArea = p_data.Get<EMapArea>("CurrentMapArea", EMapArea.NONE);
			m_lastMapArea = m_currentMapArea;
			SaveGameData saveGameData6 = p_data.Get<SaveGameData>("Tokens", null);
			if (saveGameData6 != null)
			{
				m_tokenHandler.Load(saveGameData6);
			}
			SaveGameData saveGameData7 = p_data.Get<SaveGameData>("Hirelings", null);
			if (saveGameData7 != null)
			{
				m_hirelingHandler.Load(saveGameData7);
			}
			InitBaseObject(p_staticID, p_spawnerID);
			SelectCharacter(p_data.Get<Int32>("SelectedCharacter", 0));
		}

		public delegate void CharacterSelectRequest(Int32 p_character);
	}
}
