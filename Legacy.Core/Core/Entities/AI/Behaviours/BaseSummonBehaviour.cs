using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;

namespace Legacy.Core.Entities.AI.Behaviours
{
	public abstract class BaseSummonBehaviour : BaseBehaviour
	{
		public BaseSummonBehaviour(AIBrain p_brain, Summon p_controller) : base(p_brain, p_controller)
		{
		}

		public new Summon Controller => (Summon)base.Controller;

	    public override void BeginTurn()
		{
			Summon controller = Controller;
			if (controller.DestroyOnTargetContact && HadTargetContact())
			{
				controller.LifeTime = 1;
			}
			if (controller.LifeTime != -1 && controller.LifeTime > 0)
			{
				controller.LifeTime--;
			}
		}

		public override void UpdateTurn()
		{
		}

		public override void EndTurn()
		{
			Summon controller = Controller;
			if (controller.LifeTime == 0)
			{
				controller.Destroy();
			}
		}

		protected Boolean HadTargetContact()
		{
			if (LegacyLogic.Instance.MapLoader.Grid.GetSlot(Controller.Position).HasEntities)
			{
				foreach (MovingEntity movingEntity in LegacyLogic.Instance.MapLoader.Grid.GetSlot(Controller.Position).Entities)
				{
					if (movingEntity != Controller && ((Controller.StaticData.AITargets == EAITarget.Party && movingEntity is Party) || (Controller.StaticData.AITargets == EAITarget.Monster && movingEntity is Monster)))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		protected void GetAllTargetsInRange(List<Object> p_targets, Boolean p_targetsOutOfRange, Boolean p_includeParty)
		{
			Int32 range = Controller.StaticData.Range;
			foreach (Monster monster in LegacyLogic.Instance.WorldManager.GetObjectsByTypeIterator<Monster>())
			{
				if (monster.IsAttackable && monster.CurrentHealth > 0)
				{
					if (p_targetsOutOfRange && Position.Distance(monster.Position, Controller.Position) > range)
					{
						p_targets.Add(monster);
					}
					else if (!p_targetsOutOfRange && Position.Distance(monster.Position, Controller.Position) <= range)
					{
						p_targets.Add(monster);
					}
				}
			}
			if (p_includeParty)
			{
				Position position = LegacyLogic.Instance.WorldManager.Party.Position;
				if (p_targetsOutOfRange && Position.Distance(Controller.Position, position) > range)
				{
					p_targets.AddRange(LegacyLogic.Instance.WorldManager.Party.Members);
				}
				else if (!p_targetsOutOfRange && Position.Distance(Controller.Position, position) <= range)
				{
					p_targets.AddRange(LegacyLogic.Instance.WorldManager.Party.Members);
				}
			}
		}

		protected Boolean Attack(List<Object> attackBuffer)
		{
			if (Controller.StaticID == 5)
			{
				GetAllTargetsInRange(attackBuffer, false, true);
			}
			else
			{
				GetAttackTargets(attackBuffer);
			}
			if (attackBuffer.Count > 0)
			{
				Damage[] array = new Damage[Brain.Data.DamageData.Length];
				for (Int32 i = 0; i < Brain.Data.DamageData.Length; i++)
				{
					DamageData p_data = DamageData.Scale(Brain.Data.DamageData[i], Brain.MagicFactor);
					array[i] = Damage.Create(p_data, 0f);
				}
				Attack p_attack = new Attack(0f, 0f, array);
				Boolean flag = false;
				SummonSpellEventArgs summonSpellEventArgs = new SummonSpellEventArgs();
				foreach (Object obj in attackBuffer)
				{
					BaseCombatHandler baseCombatHandler;
					if (obj is Character)
					{
						flag = true;
						Character character = (Character)obj;
						baseCombatHandler = character.FightHandler;
					}
					else
					{
						if (!(obj is Monster))
						{
							continue;
						}
						flag = true;
						Monster monster = (Monster)obj;
						baseCombatHandler = monster.CombatHandler;
						if (Controller.StaticID == 3 && monster.AbilityHandler.HasAbility(EMonsterAbilityType.UNDEAD))
						{
							AttackResult attackResult = new AttackResult();
							attackResult.Result = EResultType.IMMUNE;
							summonSpellEventArgs.SpellTargets.Add(new AttackedTarget(obj, attackResult));
							continue;
						}
					}
					if (Controller.StaticData.DamageMod != EDamageMod.NONE && Controller.StaticData.DamageMod == EDamageMod.MultiplyByTargets)
					{
						List<Damage> list = new List<Damage>();
						foreach (Damage item in array)
						{
						    var damage = item;
                            damage.Value *= attackBuffer.Count;
							list.Add(damage);
						}
						p_attack = new Attack(0f, 0f, list);
					}
					AttackResult attackResult2 = baseCombatHandler.AttackEntity(p_attack, false, Brain.DamageType, Controller.StaticID == 6, Brain.IgnoreResistance, false);
					summonSpellEventArgs.SpellTargets.Add(new AttackedTarget(obj, attackResult2));
					if (attackResult2.Result != EResultType.EVADE)
					{
						if (obj is Character)
						{
							Character character2 = (Character)obj;
							character2.ApplyDamages(attackResult2, null);
						}
						else if (obj is Monster)
						{
							Monster monster2 = (Monster)obj;
							monster2.ApplyDamages(attackResult2, Controller);
						}
						if (Controller.StaticData.MonsterBuffs[0] != EMonsterBuffType.NONE && obj is Monster)
						{
							Monster monster3 = (Monster)obj;
							foreach (EMonsterBuffType p_type in Controller.StaticData.MonsterBuffs)
							{
								MonsterBuff p_buff = BuffFactory.CreateMonsterBuff(p_type, Brain.MagicFactor);
								monster3.AddBuff(p_buff);
							}
						}
						if (Controller.StaticData.PartyBuffs[0] != EPartyBuffs.NONE && obj is Character)
						{
							foreach (EPartyBuffs p_buffId in Controller.StaticData.PartyBuffs)
							{
								LegacyLogic.Instance.WorldManager.Party.Buffs.AddBuff(p_buffId, 1f);
							}
						}
						if (Controller.StaticData.RemovedConditions[0] != ECondition.NONE && obj is Character)
						{
							Character character3 = (Character)obj;
							foreach (ECondition p_condition in Controller.StaticData.RemovedConditions)
							{
								character3.ConditionHandler.RemoveCondition(p_condition);
							}
						}
					}
				}
				summonSpellEventArgs.Result = ((!flag) ? ESpellResult.NO_TARGET_FOUND : ESpellResult.OK);
				Controller.FeedActionLog(summonSpellEventArgs);
				LegacyLogic.Instance.EventManager.InvokeEvent(Controller, EEventType.SUMMON_CAST_SPELL, summonSpellEventArgs);
				return flag;
			}
			return false;
		}

		protected Boolean Move(List<GridSlot> p_movePath)
		{
			if (p_movePath.Count > 0)
			{
				GridSlot gridSlot = p_movePath[0];
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(Controller.Position, gridSlot.Position);
				if (grid.MoveEntity(Controller, lineOfSightDirection))
				{
					p_movePath.RemoveAt(0);
					return true;
				}
			}
			return false;
		}

		protected void GetAttackTargets(List<Object> targets)
		{
			Int32 range = Brain.Data.Range;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Position position = Controller.Position;
			EDirection direction = Controller.Direction;
			ETargetType targetType = Brain.Data.TargetType;
			if ((targetType & ETargetType.PARTY) == ETargetType.PARTY && LegacyLogic.Instance.WorldManager.Party.Position == position)
			{
				targets.AddRange(LegacyLogic.Instance.WorldManager.Party.Members);
			}
			if ((targetType & ETargetType.SINGLE_MONSTER) == ETargetType.SINGLE_MONSTER)
			{
				Monster randomMonsterInDirection = grid.GetRandomMonsterInDirection(position, direction, range);
				if (randomMonsterInDirection != null && randomMonsterInDirection.CurrentHealth > 0)
				{
					targets.Add(randomMonsterInDirection);
				}
			}
			if ((targetType & ETargetType.ALL_MONSTERS_ON_TARGET_SLOT) == ETargetType.ALL_MONSTERS_ON_TARGET_SLOT)
			{
				Int32 count = targets.Count;
				grid.GetMonstersOnFirstSlot(position, direction, 0, targets);
				for (Int32 i = targets.Count - 1; i >= count; i--)
				{
					if (!((Monster)targets[i]).IsAttackable || ((Monster)targets[i]).CurrentHealth <= 0)
					{
						targets.RemoveAt(i);
					}
				}
			}
			if ((targetType & ETargetType.ALL_MONSTERS_IN_LINE_OF_SIGHT) == ETargetType.ALL_MONSTERS_IN_LINE_OF_SIGHT)
			{
				Int32 count2 = targets.Count;
				grid.GetMonstersInDirection(position, direction, range, targets);
				for (Int32 j = targets.Count - 1; j >= count2; j--)
				{
					if (!((Monster)targets[j]).IsAttackable || ((Monster)targets[j]).CurrentHealth <= 0)
					{
						targets.RemoveAt(j);
					}
				}
			}
			if ((targetType & ETargetType.ALL_ADJACENT_MONSTERS) == ETargetType.ALL_ADJACENT_MONSTERS)
			{
				Int32 count3 = targets.Count;
				grid.GetMonstersInDirection(position, EDirection.NORTH, 1, targets);
				grid.GetMonstersInDirection(position, EDirection.EAST, 1, targets);
				grid.GetMonstersInDirection(position, EDirection.SOUTH, 1, targets);
				grid.GetMonstersInDirection(position, EDirection.WEST, 1, targets);
				for (Int32 k = targets.Count - 1; k >= count3; k--)
				{
					if (!((Monster)targets[k]).IsAttackable || ((Monster)targets[k]).CurrentHealth <= 0)
					{
						targets.RemoveAt(k);
					}
				}
			}
			if ((targetType & ETargetType.ALL_MONSTERS) == ETargetType.ALL_MONSTERS)
			{
				foreach (Monster monster in LegacyLogic.Instance.WorldManager.GetObjectsByTypeIterator<Monster>())
				{
					if (monster.IsAggro && monster.IsAttackable && monster.CurrentHealth > 0)
					{
						targets.Add(monster);
					}
				}
			}
			if (targets.Count > 1)
			{
				HashSet<Object> collection = new HashSet<Object>(targets);
				targets.Clear();
				targets.AddRange(collection);
			}
		}

		protected GridSlot GetMoveTarget(List<GridSlot> p_movePath)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			EObjectType type = (Brain.Data.AITargets != EAITarget.Monster) ? EObjectType.PARTY : EObjectType.MONSTER;
			List<GridSlot> list = new List<GridSlot>();
			IEnumerable<GridSlot> enumerable = grid.SlotIteratorAround(Controller.Position, Brain.Data.AIRange);
			foreach (GridSlot gridSlot in enumerable)
			{
				if (gridSlot.CountEntityTypes(type) > 0)
				{
					list.Add(gridSlot);
				}
			}
			if (list.Count > 1)
			{
				list.Sort(delegate(GridSlot a, GridSlot b)
				{
					Int32 num = Position.DistanceSquared(Controller.Position, a.Position).CompareTo(Position.DistanceSquared(Controller.Position, b.Position));
					if (num == 0)
					{
						return b.CountEntityTypes(type).CompareTo(a.CountEntityTypes(type));
					}
					return num;
				});
			}
			if (list.Count > 0)
			{
				foreach (GridSlot gridSlot2 in list)
				{
					p_movePath.Clear();
					if (grid.CalculateMovablePath(Controller, gridSlot2.Position, p_movePath) <= Brain.Data.AIRange)
					{
						if (p_movePath.Count > 0)
						{
							p_movePath.RemoveAt(0);
						}
						return gridSlot2;
					}
				}
			}
			p_movePath.Clear();
			return null;
		}
	}
}
