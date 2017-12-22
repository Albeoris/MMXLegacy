using System;
using System.Collections.Generic;
using Legacy.Core.Abilities;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.AI;
using Legacy.Core.Entities.AI.MonsterBehaviours;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;
using Legacy.Core.StaticData;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.Entities
{
	public class Monster : MovingEntity, ISaveGameObject
	{
		internal StateMachine<EState> m_stateMachine;

		protected TriggerHelper m_startMovement = new TriggerHelper();

		protected TriggerHelper m_turnIdle = new TriggerHelper();

		protected TriggerHelper m_skipMovement = new TriggerHelper();

		protected TriggerHelper m_alignForAttack = new TriggerHelper();

		protected TriggerHelper m_hitAnimationDone = new TriggerHelper();

		private MonsterStaticData m_staticData;

		private Int32 m_intruderAlertAggroRange;

		private Boolean m_alerted;

		private Single m_distanceToParty;

		private Boolean m_isAggro;

		private Boolean m_isAttackable = true;

		private Int32 m_spawnAnim;

		private Boolean m_triggeredLateDieEffects;

		private Int32 m_currentMeleeBlockAttempts;

		private Int32 m_currentGeneralBlockAttempts;

		private Boolean m_rangedAttack;

		private Boolean m_counterAttack;

		private Int32 m_currentHealthPoints;

		private Int32 m_maxHealthPoints;

		private Single m_magicPowerModifier = 1f;

		private Single m_attackValueModifier = 1f;

		private Character m_killingCharacter;

		private MonsterBuffHandler m_buffHandler;

		private MonsterAbilityHandler m_abilityHandler;

		private MonsterAIHandler m_aiHandler;

		private MonsterCombatHandler m_combatHandler;

		private MonsterSpellHandler m_spellHandler;

		private LootHandler m_lootHandler;

		private Boolean m_divideAttacksToPartyCharacters;

		private Boolean m_alwaysTriggerAggro = true;

		private Single m_aggroRange;

		private Boolean m_AggroTriggered;

		private Int32 m_monsterGroupID;

		public Monster() : this(0, 0)
		{
		}

		public Monster(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.MONSTER, p_spawnerID)
		{
		}

		public MonsterStaticData StaticData => m_staticData;

	    public TriggerHelper StartMovement => m_startMovement;

	    public TriggerHelper TurnIdle => m_turnIdle;

	    public TriggerHelper SkipMovement => m_skipMovement;

	    public TriggerHelper HitAnimationDone => m_hitAnimationDone;

	    public EState State => m_stateMachine.CurrentState.Id;

	    public override String Prefab
		{
			get => m_staticData.Prefab;
	        set => throw new NotSupportedException();
	    }

		public override String PrefabAlt
		{
			get => m_staticData.PrefabAlt;
		    set => throw new NotSupportedException();
		}

		public String Name => m_staticData.NameKey;

	    public override ESize Size => m_staticData.Size;

	    public Int32 MovePrio => m_staticData.MovePrio;

	    public Int32 IntruderAlertAggroRange
		{
			get => m_intruderAlertAggroRange;
	        set => m_intruderAlertAggroRange = value;
	    }

		public Int32 MonsterGroupID
		{
			get => m_monsterGroupID;
		    set
			{
				if (m_monsterGroupID != value)
				{
					if (m_monsterGroupID > 0)
					{
						LegacyLogic.Instance.WorldManager.MonsterGroupHandler.RemoveMonsterFromGroup(this);
					}
					m_monsterGroupID = value;
					LegacyLogic.Instance.WorldManager.MonsterGroupHandler.AddMonsterToGroup(this);
				}
			}
		}

		public Single DistanceToParty
		{
			get => m_distanceToParty;
		    set
			{
				m_distanceToParty = value;
				if (m_intruderAlertAggroRange > 0 && m_distanceToParty <= m_intruderAlertAggroRange)
				{
					m_alerted = true;
					m_intruderAlertAggroRange = 0;
				}
				CheckAggroRange();
			}
		}

		public void InitMonsterGroup()
		{
			LegacyLogic.Instance.WorldManager.MonsterGroupHandler.AddMonsterToGroup(this);
		}

		public void CheckSightAggro()
		{
			if (Position.Distance(Position, LegacyLogic.Instance.WorldManager.Party.Position) < ((LegacyLogic.Instance.MapLoader.Grid.Type != EMapType.DUNGEON) ? m_staticData.SightAggroRange : m_staticData.SightAggroRangeDungeon))
			{
				Boolean flag = true;
				List<GridSlot> gridSlotsOnLine = LegacyLogic.Instance.MapLoader.Grid.GetGridSlotsOnLine(Position, LegacyLogic.Instance.WorldManager.Party.Position);
				if (gridSlotsOnLine.Count > 0)
				{
					for (Int32 i = 0; i < gridSlotsOnLine.Count; i++)
					{
						if ((gridSlotsOnLine[i].TerrainType & ETerrainType.BLOCKED) == ETerrainType.BLOCKED && (gridSlotsOnLine[i].TerrainType & ETerrainType.SEE_THROUGH) != ETerrainType.SEE_THROUGH)
						{
							flag = false;
							break;
						}
					}
					if (flag && m_alwaysTriggerAggro)
					{
						IsAggro = true;
					}
				}
			}
		}

		public void CheckAggroRange()
		{
			if (m_distanceToParty == 0f)
			{
				m_distanceToParty = 99f;
				IsAggro = false;
			}
			if (m_distanceToParty <= m_aggroRange - AggroReduce || m_alerted || IsAggro)
			{
				if (m_alwaysTriggerAggro)
				{
					IsAggro = true;
					Grid grid = LegacyLogic.Instance.MapLoader.Grid;
					if (grid != null && grid.Type != EMapType.CITY)
					{
						LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.THREAT);
					}
				}
				else
				{
					IsAggro = false;
				}
			}
			else
			{
				IsAggro = false;
			}
			if (IsAggro && !m_AggroTriggered)
			{
				if (m_staticData.Grade == EMonsterGrade.CHAMPION)
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.BOSS_ENCOUNTER);
					m_AggroTriggered = true;
				}
				else
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.IN_COMBAT);
					m_AggroTriggered = true;
				}
			}
		}

		public void SetAggro(Boolean b)
		{
			m_isAggro = false;
		}

		public Boolean AlwaysTriggerAggro
		{
			get => m_alwaysTriggerAggro;
		    set => m_alwaysTriggerAggro = value;
		}

		public Int32 SpawnAnim
		{
			get => m_spawnAnim;
		    set => m_spawnAnim = value;
		}

		private Int32 AggroReduce
		{
			get
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				NpcEffect npcEffect;
				if (party.HirelingHandler.HasEffect(ETargetCondition.HIRE_AGGROREDUCE, out npcEffect) && npcEffect.TargetEffect != ETargetCondition.NONE)
				{
					return (Int32)npcEffect.EffectValue;
				}
				return 0;
			}
		}

		public Boolean IsAggro
		{
			get => m_isAggro;
		    set
			{
				if (m_isAggro != value)
				{
					m_isAggro = value;
					if (m_isAggro)
					{
						LegacyLogic.Instance.WorldManager.MonsterGroupHandler.Alert(this);
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_HUD_DANGER_SENSE, EventArgs.Empty);
					}
				}
			}
		}

		public Single AggroRange
		{
			get => m_aggroRange;
		    set => m_aggroRange = value;
		}

		public Boolean IsAttackable
		{
			get => m_isAttackable && m_alwaysTriggerAggro;
		    set => m_isAttackable = value;
		}

		public Int32 MaxHealth => m_maxHealthPoints;

	    public Boolean IsFightMusicForced => m_staticData.IsFightMusicForced;

	    public Int32 CurrentHealth => m_currentHealthPoints;

	    public MonsterBuffHandler BuffHandler => m_buffHandler;

	    public MonsterAbilityHandler AbilityHandler => m_abilityHandler;

	    public MonsterAIHandler AiHandler => m_aiHandler;

	    public MonsterCombatHandler CombatHandler => m_combatHandler;

	    public MonsterSpellHandler SpellHandler => m_spellHandler;

	    public LootHandler LootHandler => m_lootHandler;

	    public Boolean RangedAttack
		{
			get => m_rangedAttack;
	        set => m_rangedAttack = value;
	    }

		public Boolean CounterAttack
		{
			get => m_counterAttack;
		    set => m_counterAttack = value;
		}

		public Int32 CurrentMeleeBlockAttempts
		{
			get => m_currentMeleeBlockAttempts;
		    set => m_currentMeleeBlockAttempts = value;
		}

		public Int32 CurrentGeneralBlockAttempts
		{
			get => m_currentGeneralBlockAttempts;
		    set => m_currentGeneralBlockAttempts = value;
		}

		public Int32 MeleeBlockAttempts
		{
			get => m_currentMeleeBlockAttempts;
		    set => m_currentMeleeBlockAttempts = value;
		}

		public Int32 GeneralBlockAttempts
		{
			get => m_currentGeneralBlockAttempts;
		    set => m_currentGeneralBlockAttempts = value;
		}

		public Boolean DivideAttacksToPartyCharacters
		{
			get => m_divideAttacksToPartyCharacters;
		    set => m_divideAttacksToPartyCharacters = value;
		}

		public Single MagicPower => m_staticData.MagicPower * m_magicPowerModifier;

	    public Single MagicPowerModifier
		{
			set => m_magicPowerModifier = value;
		}

		public Single AttackValueModifier
		{
			set => m_attackValueModifier = value;
		}

		public Single MeleeAttackValue => m_staticData.MeleeAttackValue * m_attackValueModifier;

	    public Single RangedAttackValue => m_staticData.RangedAttackValue * m_attackValueModifier;

	    private void InitMonster()
		{
			InitStateMachine();
			InitAbilityHandler();
			InitSpellHandler();
			CreateMonsterAI();
			m_buffHandler = new MonsterBuffHandler(this);
			m_lootHandler = new LootHandler(this);
			m_combatHandler = new MonsterCombatHandler(this)
			{
				MeleeStrikes = m_staticData.MeleeAttackStrikesAmount,
				RangedStrikes = m_staticData.RangedAttackStrikesAmount,
				AttackRange = m_staticData.AttackRange,
				CanCastSpell = true,
				MeleeDamageModifier = 0f,
				RangeDamageModifier = 0f,
				EvadeValue = m_staticData.EvadeValue,
				ArmorValue = m_staticData.ArmorValue,
				IsFleeing = false
			};
			m_magicPowerModifier = 1f;
		}

		public override Boolean CanPassTerrain(ETerrainType p_type)
		{
			return (StaticData.AccessibleTerrains & p_type) != ETerrainType.NONE;
		}

		private void CreateMonsterAI()
		{
			switch (m_staticData.MonsterBehaviour)
			{
			case EMonsterAIBehaviour.MAMUSHI:
				m_aiHandler = new MamushiAIHandler(this);
				break;
			case EMonsterAIBehaviour.IVEN:
				m_aiHandler = new IvenAIHandler(this);
				break;
			case EMonsterAIBehaviour.BRAINLESS_STUPID:
				m_aiHandler = new BrainlessStupidAIHandler(this);
				break;
			case EMonsterAIBehaviour.AVERAGE:
				m_aiHandler = new AverageAIHandler(this);
				break;
			case EMonsterAIBehaviour.CLEVER:
				m_aiHandler = new CleverAIHandler(this);
				break;
			case EMonsterAIBehaviour.GENIAL:
				m_aiHandler = new GenialAIHandler(this);
				break;
			case EMonsterAIBehaviour.TARGET_DUMMY:
				m_aiHandler = new TargetDummyAIHandler(this);
				break;
			case EMonsterAIBehaviour.EREBOS:
				m_aiHandler = new ErebosAIHandler(this);
				break;
			case EMonsterAIBehaviour.TARALETH:
				m_aiHandler = new TaralethAIHandler(this);
				break;
			case EMonsterAIBehaviour.CRYSTAL_SPIDER:
				m_aiHandler = new CrystalSpiderAIHandler(this);
				break;
			case EMonsterAIBehaviour.MARKUS_WOLF:
				m_aiHandler = new MarkusWolfAIBehaviour(this);
				break;
			case EMonsterAIBehaviour.SPIDER_QUEEN:
				m_aiHandler = new SpiderQueenAIBehaviour(this);
				break;
			case EMonsterAIBehaviour.SHALWEND:
				m_aiHandler = new ShalwendAIBehaviour(this);
				break;
			case EMonsterAIBehaviour.STATIONARY:
				m_aiHandler = new StationaryAIHandler(this);
				break;
			case EMonsterAIBehaviour.ADIRA:
				m_aiHandler = new AdiraAIHandler(this);
				break;
			default:
				m_aiHandler = new MonsterAIHandler(this);
				break;
			}
		}

		private void InitStateMachine()
		{
			m_stateMachine = new StateMachine<EState>();
			State<EState> state = new State<EState>(EState.IDLE);
			state.AddTransition(new Transition<EState>(EState.MOVING, m_startMovement));
			state.AddTransition(new Transition<EState>(EState.ACTION_FINISHED, m_skipMovement));
			m_stateMachine.AddState(state);
			State<EState> state2 = new State<EState>(EState.MOVING);
			state2.AddTransition(new Transition<EState>(EState.ACTION_FINISHED, m_movementDone));
			state2.AddTransition(new Transition<EState>(EState.ACTION_FINISHED, m_attackingDone));
			m_stateMachine.AddState(state2);
			State<EState> state3 = new State<EState>(EState.ALIGN_FOR_ATTACK);
			state3.AddTransition(new Transition<EState>(EState.ATTACKING, m_rotationDone)
			{
				ResetTriggerWhenStateEntered = true
			});
			m_stateMachine.AddState(state3);
			State<EState> state4 = new State<EState>(EState.ATTACKING);
			state4.AddTransition(new Transition<EState>(EState.IDLE, m_turnIdle));
			state4.AddTransition(new Transition<EState>(EState.ACTION_FINISHED, m_attackingDone));
			m_stateMachine.AddState(state4);
			State<EState> state5 = new State<EState>(EState.ACTION_FINISHED);
			state5.AddTransition(new Transition<EState>(EState.IDLE, m_turnIdle));
			m_stateMachine.AddState(state5);
			State<EState> p_state = new State<EState>(EState.SPAWNING);
			m_stateMachine.AddState(p_state);
			m_stateMachine.ChangeState(EState.IDLE);
		}

		private void InitAbilityHandler()
		{
			m_abilityHandler = new MonsterAbilityHandler(this);
			if (m_staticData != null && m_staticData.Abilities != null)
			{
				MonsterAbilityID[] abilities = m_staticData.Abilities;
				for (Int32 i = 0; i < abilities.Length; i++)
				{
					MonsterAbilityBase monsterAbilityBase = AbilityFactory.CreateMonsterAbility(abilities[i].AbilityType, MagicPower);
					if (monsterAbilityBase != null)
					{
						monsterAbilityBase.Level = abilities[i].Level;
						m_abilityHandler.Add(monsterAbilityBase);
					}
				}
			}
		}

		private void InitSpellHandler()
		{
			m_spellHandler = new MonsterSpellHandler(this);
			if (m_staticData != null && m_staticData.Spells != null)
			{
				MonsterStaticData.SpellData[] spells = m_staticData.Spells;
				for (Int32 i = 0; i < spells.Length; i++)
				{
					MonsterSpell monsterSpell = SpellFactory.CreateMonsterSpell((EMonsterSpell)spells[i].SpellID, spells[i].AnimationClipName, spells[i].SpellProbability);
					monsterSpell.Level = spells[i].Level;
					if (monsterSpell != null)
					{
						m_spellHandler.AddSpell(monsterSpell);
					}
				}
			}
		}

		protected override void LoadStaticData()
		{
			if (StaticID != 0)
			{
				m_staticData = StaticDataHandler.GetStaticData<MonsterStaticData>(EDataType.MONSTER, StaticID);
				if (m_staticData == null)
				{
					throw new Exception("MonsterStaticData ID=" + StaticID + " not defined!");
				}
				InitMonster();
				if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.NORMAL)
				{
					m_currentHealthPoints = m_staticData.MaxHealthpoints;
					m_maxHealthPoints = m_staticData.MaxHealthpoints;
				}
				else
				{
					GameConfig game = ConfigManager.Instance.Game;
					switch (m_staticData.Grade)
					{
					case EMonsterGrade.CORE:
						m_currentHealthPoints = (Int32)(m_staticData.MaxHealthpoints * game.MonsterHealthCoreFactor);
						m_maxHealthPoints = (Int32)(m_staticData.MaxHealthpoints * game.MonsterHealthCoreFactor);
						break;
					case EMonsterGrade.ELITE:
						m_currentHealthPoints = (Int32)(m_staticData.MaxHealthpoints * game.MonsterHealthEliteFactor);
						m_maxHealthPoints = (Int32)(m_staticData.MaxHealthpoints * game.MonsterHealthEliteFactor);
						break;
					case EMonsterGrade.CHAMPION:
						m_currentHealthPoints = (Int32)(m_staticData.MaxHealthpoints * game.MonsterHealthChampionFactor);
						m_maxHealthPoints = (Int32)(m_staticData.MaxHealthpoints * game.MonsterHealthChampionFactor);
						break;
					default:
						m_maxHealthPoints = m_staticData.MaxHealthpoints;
						m_currentHealthPoints = m_staticData.MaxHealthpoints;
						break;
					}
				}
				m_alwaysTriggerAggro = m_staticData.AlwaysTriggerAggro;
			}
		}

		public void Update()
		{
			if (State == EState.ATTACKING)
			{
				m_stateMachine.ChangeState(EState.MOVING);
				m_combatHandler.DoAttack();
			}
			if (State == EState.SPAWNING && m_appearAnimationDone.IsTriggered)
			{
				m_stateMachine.ChangeState(EState.IDLE);
				m_appearAnimationDone.Reset();
				m_aiHandler.DoTurn(LegacyLogic.Instance.MapLoader.Grid, LegacyLogic.Instance.WorldManager.Party);
			}
			m_stateMachine.Update();
		}

		public void ForceAggro(Boolean p_aggro)
		{
			m_isAggro = p_aggro;
		}

		public void Die()
		{
			Die(false);
		}

		public void Die(Boolean p_forced)
		{
			if (p_forced || m_staticData.CanDie)
			{
				LegacyLogic.Instance.WorldManager.MonsterGroupHandler.RemoveMonsterFromGroup(this);
				LegacyLogic.Instance.WorldManager.DestroyObject(this, Position);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTER_DIED, EventArgs.Empty);
				LegacyLogic.Instance.UpdateManager.PartyTurnActor.CheckInCombat(true);
			}
			else
			{
				ForceAggro(false);
				m_alwaysTriggerAggro = false;
				LegacyLogic.Instance.UpdateManager.PartyTurnActor.CheckInCombat(true);
				LegacyLogic.Instance.UpdateManager.SkipUntilNextInteractionTurnActor = true;
				LegacyLogic.Instance.UpdateManager.CurrentTurnActor.FinishTurn();
				TriggerLateDieEffects();
				Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
				for (Int32 i = 0; i < members.Length; i++)
				{
					if (!members[i].DoneTurn)
					{
						members[i].EndTurn();
					}
				}
			}
		}

		public void Disappear()
		{
			LegacyLogic.Instance.WorldManager.MonsterGroupHandler.RemoveMonsterFromGroup(this);
			LegacyLogic.Instance.WorldManager.DestroyObject(this, Position);
			LegacyLogic.Instance.UpdateManager.PartyTurnActor.CheckInCombat(true);
		}

		public void TriggerLateDieEffects()
		{
			if (!m_triggeredLateDieEffects)
			{
				m_triggeredLateDieEffects = true;
				m_abilityHandler.CancelAllEntries();
				m_buffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_CAST_SPELL);
				m_buffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE);
				foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
				{
					character.FightHandler.FlushDelayedActionLog();
					if (character == m_killingCharacter)
					{
						character.FlushActionLog();
					}
				}
				if (m_staticData.ShowActionLog)
				{
					ConditionChangedEntryEventArgs p_args = new ConditionChangedEntryEventArgs(this, ECondition.DEAD);
					LegacyLogic.Instance.ActionLog.PushEntry(p_args);
				}
				m_lootHandler.DistributeRewards(m_killingCharacter);
				foreach (Character character2 in LegacyLogic.Instance.WorldManager.Party.Members)
				{
					character2.FlushActionLog();
				}
				m_lootHandler.FlushActionLog();
				m_abilityHandler.ExecuteAttackResults(null, null, false, false, EExecutionPhase.MONSTER_DIES);
			}
		}

		public void AddBuff(MonsterBuff p_buff)
		{
			m_buffHandler.AddBuff(p_buff);
			m_abilityHandler.ExecuteAttacks(null, null, false, EExecutionPhase.ON_APPLY_MONSTER_BUFF);
		}

		public void Load(SaveGameData p_data)
		{
			Int32 p_staticID = p_data.Get<Int32>("StaticID", 1);
			Int32 p_spawnerID = p_data.Get<Int32>("SpawnerID", 0);
			Position = new Position(p_data.Get<Int32>("PositionX", 0), p_data.Get<Int32>("PositionY", 0));
			Direction = (EDirection)p_data.Get<Int32>("Direction", 0);
			m_isAggro = p_data.Get<Boolean>("IsAggro", false);
			m_alerted = p_data.Get<Boolean>("Alerted", false);
			m_currentHealthPoints = p_data.Get<Int32>("CurrentHealthPoints", 1);
			SaveGameData saveGameData = new SaveGameData("MonsterBuffs");
			saveGameData = p_data.Get<SaveGameData>("MonsterBuffs", saveGameData);
			m_monsterGroupID = p_data.Get<Int32>("MonsterGroup", 0);
			if (m_buffHandler == null)
			{
				m_buffHandler = new MonsterBuffHandler(this);
			}
			m_buffHandler.Load(saveGameData);
			InitBaseObject(p_staticID, p_spawnerID);
			m_alwaysTriggerAggro = p_data.Get<Boolean>("AlwaysTriggerAggro", true);
			m_aiHandler.Load(p_data);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("StaticID", StaticID);
			p_data.Set<Int32>("SpawnerID", SpawnerID);
			p_data.Set<Int32>("PositionX", Position.X);
			p_data.Set<Int32>("PositionY", Position.Y);
			p_data.Set<Int32>("Direction", (Int32)Direction);
			p_data.Set<Boolean>("IsAggro", m_isAggro);
			p_data.Set<Boolean>("Alerted", m_alerted);
			p_data.Set<Int32>("CurrentHealthPoints", m_currentHealthPoints);
			SaveGameData saveGameData = new SaveGameData("MonsterBuffs");
			m_buffHandler.Save(saveGameData);
			p_data.Set<Int32>("MonsterGroup", m_monsterGroupID);
			p_data.Set<SaveGameData>("MonsterBuffs", saveGameData);
			p_data.Set<Boolean>("AlwaysTriggerAggro", m_alwaysTriggerAggro);
			m_aiHandler.Save(p_data);
		}

		public override void Destroy()
		{
			m_aiHandler.Destroy();
			base.Destroy();
			m_staticData = null;
		}

		public void ResetRoundValues()
		{
			m_currentMeleeBlockAttempts = m_staticData.MeleeBlockAttemptsPerTurn;
			m_currentGeneralBlockAttempts = m_staticData.GeneralBlockAttemptsPerTurn;
			m_abilityHandler.ResetAbilityValues();
			m_spellHandler.StartTurn();
			if (m_alwaysTriggerAggro)
			{
				m_isAttackable = true;
			}
			else
			{
				m_isAttackable = false;
			}
			m_aggroRange = ((LegacyLogic.Instance.MapLoader.Grid.Type != EMapType.DUNGEON) ? m_staticData.AggroRange : m_staticData.AggroRangeDungeon);
		}

		public override void ApplyDamages(AttackResult p_result, Object p_source)
		{
			if (p_result.Result == EResultType.CRITICAL_HIT || p_result.Result == EResultType.HIT)
			{
				m_buffHandler.DoOnGetDamageEffects(p_result.DamageResults);
			}
			Character p_source2 = p_source as Character;
			ChangeHP(-p_result.DamageDone, p_source2);
		}

		public void ChangeHP(Int32 p_deltaHP, Character p_source)
		{
			if (m_currentHealthPoints <= 0)
			{
				return;
			}
			m_currentHealthPoints += p_deltaHP;
			if (m_monsterGroupID > 0)
			{
				LegacyLogic.Instance.WorldManager.MonsterGroupHandler.SyncHP(m_monsterGroupID, m_currentHealthPoints);
			}
			if (m_currentHealthPoints <= 0)
			{
				if (m_staticData.Grade == EMonsterGrade.CHAMPION)
				{
					for (Int32 i = 0; i < 4; i++)
					{
						Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(i);
						if (member.ConditionHandler.Condition != ECondition.DEAD && member.ConditionHandler.Condition != ECondition.UNCONSCIOUS)
						{
							member.BarkHandler.TriggerBark(EBarks.KILLED_BOSS, member);
						}
					}
				}
				else if (p_source != null)
				{
					p_source.BarkHandler.TriggerBark(EBarks.KILLING_STRIKE, p_source);
				}
				m_killingCharacter = p_source;
				Die();
			}
			else if (m_currentHealthPoints > m_maxHealthPoints)
			{
				m_currentHealthPoints = m_maxHealthPoints;
			}
			StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.HEALTH_POINTS);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTER_STATUS_CHANGED, p_eventArgs);
		}

		public void CheckLateDieEffects()
		{
			if (m_currentHealthPoints <= 0)
			{
				TriggerLateDieEffects();
			}
		}

		public void ResetMonsterData()
		{
			m_combatHandler.AttackRange = m_staticData.AttackRange;
			m_combatHandler.MeleeStrikes = m_staticData.MeleeAttackStrikesAmount;
			m_combatHandler.RangedStrikes = m_staticData.RangedAttackStrikesAmount;
			m_combatHandler.CanMove = true;
			m_combatHandler.CanCastSpell = true;
			m_combatHandler.MeleeDamageModifier = 0f;
			m_combatHandler.RangeDamageModifier = 0f;
			m_combatHandler.EvadeValue = m_staticData.EvadeValue;
			m_combatHandler.ArmorValue = m_staticData.ArmorValue;
			m_combatHandler.IsFleeing = false;
			m_magicPowerModifier = 1f;
			m_attackValueModifier = 1f;
			m_aggroRange = ((LegacyLogic.Instance.MapLoader.Grid.Type != EMapType.DUNGEON) ? m_staticData.AggroRange : m_staticData.AggroRangeDungeon);
		}

		public override void EndTurn()
		{
			base.EndTurn();
			if (!m_buffHandler.HasBuff(EMonsterBuffType.TIME_STOP))
			{
				m_buffHandler.UpdateHandler();
			}
			m_combatHandler.PreselectedTarget = null;
			m_combatHandler.CannotBlockThisTurn = false;
			m_abilityHandler.ExecuteAttacks(null, null, false, EExecutionPhase.END_OF_MONSTERS_TURN);
		}

		public enum EState
		{
			IDLE,
			MOVING,
			ATTACKING = 3,
			ALIGN_FOR_ATTACK,
			SPAWNING,
			ACTION_FINISHED = 10
		}
	}
}
