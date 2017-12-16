using System;
using System.Collections.Generic;
using Legacy.Core.Abilities;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.Entities
{
	public class MonsterCombatHandler : BaseCombatHandler
	{
		private Monster m_owner;

		private Character m_currentTarget;

		private List<Attack> m_attacks;

		private List<AttackResult> m_attackResults;

		private MonsterSpell m_castedSpell;

		private Boolean m_castSpell;

		private List<CombatEntryEventArgs> m_counterAttackArgs;

		private Single m_attackRange;

		private Int32 m_meleeStrikes;

		private Int32 m_rangedStrikes;

		private Boolean m_canMove;

		private Boolean m_canCastSpell;

		private Single m_meleeDamageModifier;

		private Single m_rangeDamageModifier;

		private Single m_evadeValue;

		private Single m_armorValue;

		private Boolean m_isFleeing;

		private Character m_preselectedTarget;

		private Boolean m_cannotBlockThisTurn;

		private Int32 m_meleeStrikesRoundBonus;

		public MonsterCombatHandler(Monster p_owner)
		{
			if (p_owner == null)
			{
				throw new ArgumentNullException("p_owner");
			}
			m_attacks = new List<Attack>();
			m_attackResults = new List<AttackResult>();
			m_counterAttackArgs = new List<CombatEntryEventArgs>();
			m_owner = p_owner;
		}

		public Single AttackRange
		{
			get => m_attackRange;
		    set => m_attackRange = value;
		}

		public Int32 MeleeStrikes
		{
			get => m_meleeStrikes;
		    set => m_meleeStrikes = value;
		}

		public Int32 MeleeStrikesRoundBonus
		{
			get => m_meleeStrikesRoundBonus;
		    set => m_meleeStrikesRoundBonus = value;
		}

		public Int32 RangedStrikes
		{
			get => m_rangedStrikes;
		    set => m_rangedStrikes = value;
		}

		public Boolean CanMove
		{
			get => m_canMove;
		    set => m_canMove = value;
		}

		public Single MeleeDamageModifier
		{
			get => m_meleeDamageModifier;
		    set => m_meleeDamageModifier = value;
		}

		public Single RangeDamageModifier
		{
			get => m_rangeDamageModifier;
		    set => m_rangeDamageModifier = value;
		}

		public Single EvadeValue
		{
			get => m_evadeValue;
		    set => m_evadeValue = value;
		}

		public Boolean CanCastSpell
		{
			get => m_canCastSpell;
		    set => m_canCastSpell = value;
		}

		public Boolean IsFleeing
		{
			get => m_isFleeing;
		    set => m_isFleeing = value;
		}

		public Character PreselectedTarget
		{
			get => m_preselectedTarget;
		    set => m_preselectedTarget = value;
		}

		public Boolean CannotBlockThisTurn
		{
			get => m_cannotBlockThisTurn;
		    set => m_cannotBlockThisTurn = value;
		}

		public Single ArmorValue
		{
			get => m_armorValue;
		    set => m_armorValue = value;
		}

		public MonsterSpell CastedSpell
		{
			get => m_castedSpell;
		    set => m_castedSpell = value;
		}

		public Boolean CastSpell
		{
			get => m_castSpell;
		    set => m_castSpell = value;
		}

		public List<CombatEntryEventArgs> CounterLogEntries => m_counterAttackArgs;

	    public void Attack()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(m_owner.Position, party.Position);
			if (lineOfSightDirection != m_owner.Direction && lineOfSightDirection != EDirection.COUNT && lineOfSightDirection != EDirection.CENTER)
			{
				m_owner.Direction = lineOfSightDirection;
				m_owner.m_stateMachine.ChangeState(Monster.EState.ALIGN_FOR_ATTACK);
				BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(m_owner, m_owner.Position);
				LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.ROTATE_ENTITY, p_eventArgs);
			}
			else
			{
				m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
				DoAttack();
			}
		}

		public void AttackRanged()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(m_owner.Position, party.Position);
			if (lineOfSightDirection != m_owner.Direction && lineOfSightDirection != EDirection.COUNT && lineOfSightDirection != EDirection.CENTER)
			{
				m_owner.Direction = lineOfSightDirection;
				m_owner.RangedAttack = true;
				m_owner.m_stateMachine.ChangeState(Monster.EState.ALIGN_FOR_ATTACK);
				BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(m_owner, m_owner.Position);
				LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.ROTATE_ENTITY, p_eventArgs);
			}
			else
			{
				m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
				m_owner.RangedAttack = true;
				DoAttack();
			}
		}

		public override AttackResult AttackEntity(Attack p_attack, Boolean p_isMelee, EDamageType p_damageType, Boolean p_skipBlockEvadeTest, Int32 p_resistanceIgnore, Boolean p_skipBlock = false)
		{
			AttackResult attackResult = new AttackResult();
			Boolean flag = m_owner.BuffHandler.HasBuff(EMonsterBuffType.MACE_STUN);
			if (!p_skipBlockEvadeTest && !flag && TestEvade(p_attack.AttackValue, m_evadeValue, p_damageType, p_resistanceIgnore))
			{
				attackResult.Result = EResultType.EVADE;
			}
			else if (!p_skipBlockEvadeTest && !m_cannotBlockThisTurn && !flag && (p_isMelee || p_damageType == EDamageType.PHYSICAL) && TestBlock(p_isMelee))
			{
				attackResult.Result = EResultType.BLOCK;
			}
			else
			{
				attackResult.Result = ((!TestCritical(p_attack.CriticalChance)) ? EResultType.HIT : EResultType.CRITICAL_HIT);
				m_owner.BuffHandler.ManipulateDamage(p_attack);
				CalculateDamage(p_attack.Damages, attackResult.Result == EResultType.CRITICAL_HIT, attackResult.DamageResults);
			}
			attackResult.DamagePercentOfHealthPoints = attackResult.DamageDone / (Single)m_owner.MaxHealth;
			m_owner.IsAggro = true;
			return attackResult;
		}

		public AttackResult AttackMonster(Character attacker, Attack p_attack, Boolean p_isMelee, Boolean p_canBeBlockedOrEvaded, EDamageType p_damageType, Boolean p_isMagic, Int32 p_resistanceIgnore)
		{
			AttackResult attackResult = new AttackResult();
			if (p_canBeBlockedOrEvaded && TestEvade(p_attack.AttackValue, m_evadeValue, p_damageType, p_resistanceIgnore))
			{
				attackResult.Result = EResultType.EVADE;
			}
			else if (p_isMelee && m_owner.BuffHandler.HasBuff(EMonsterBuffType.SHADOW_CLOAK))
			{
				attackResult.Result = EResultType.EVADE;
				m_owner.BuffHandler.RemoveBuffByID(23);
			}
			else if (p_canBeBlockedOrEvaded && !m_cannotBlockThisTurn && ((p_isMelee && p_damageType == EDamageType.PHYSICAL) || (!p_isMelee && p_damageType == EDamageType.PHYSICAL)) && TestBlock(p_isMelee) && !attacker.SkillHandler.IgnoreMonsterBlocks(p_attack, p_isMelee))
			{
				if (p_isMelee)
				{
					Equipment equipment = attacker.Equipment.DoMeleeWeaponBreakCheck();
					if (equipment != null)
					{
						attackResult.BrokenItem = equipment;
					}
				}
				attackResult.Result = EResultType.BLOCK;
			}
			else
			{
				attackResult.Result = ((!TestCritical(p_attack.CriticalChance)) ? EResultType.HIT : EResultType.CRITICAL_HIT);
				Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
				if (attackResult.Result == EResultType.CRITICAL_HIT && selectedCharacter != null && m_currentTarget != null)
				{
					m_currentTarget.BarkHandler.TriggerBark(EBarks.CRIT_HIT, selectedCharacter);
				}
				if (m_owner.Name == "MONSTER_EREBOS")
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.RACIAL_6);
				}
				m_owner.BuffHandler.ManipulateDamage(p_attack);
				CalculateDamage(p_attack.Damages, attackResult.Result == EResultType.CRITICAL_HIT, attackResult.DamageResults);
			}
			m_owner.AbilityHandler.ExecuteAttackResult(attacker, attackResult, p_isMagic, !p_isMelee, EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_AFTER_DAMAGE_REDUCTION);
			attackResult.DamagePercentOfHealthPoints = attackResult.DamageDone / (Single)m_owner.MaxHealth;
			m_owner.IsAggro = true;
			return attackResult;
		}

		public void CheckCounterAttack(Character p_attacker)
		{
			if (m_owner.CounterAttack)
			{
				if (m_owner.CurrentHealth > 0)
				{
					ExecuteCounterAttack(p_attacker);
				}
				else
				{
					m_owner.CounterAttack = false;
				}
			}
		}

		public void ExecuteCounterAttack(Character p_target)
		{
			Attack singleMeleeAttack = GetSingleMeleeAttack();
			PartyBuffHandler buffs = LegacyLogic.Instance.WorldManager.Party.Buffs;
			buffs.ModifyAttack(singleMeleeAttack);
			AttackResult attackResult = p_target.FightHandler.AttackEntity(singleMeleeAttack, true, EDamageType.PHYSICAL, true, 0, false);
			AttacksEventArgs attacksEventArgs = new AttacksEventArgs(true);
			buffs.AfterAttackResult(attackResult);
			if (CheckReflectDamageByFireShield(buffs, attackResult, attacksEventArgs))
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(m_currentTarget, EEventType.REFLECTED_MAGIC_DAMAGE, attacksEventArgs);
			}
			p_target.ApplyDamages(attackResult, m_owner);
			m_owner.CounterAttack = false;
			m_counterAttackArgs.Add(new CombatEntryEventArgs(m_owner, p_target, attackResult, null));
			attacksEventArgs.Attacks.Add(new AttacksEventArgs.AttackedTarget(p_target, attackResult));
			LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.MONSTER_ATTACKS, attacksEventArgs);
		}

		public void DoAttack()
		{
			Boolean castSpell = m_castSpell;
			DoDamage();
			if (m_currentTarget != null)
			{
				m_owner.AbilityHandler.ExecuteAttackResults(m_currentTarget, m_attackResults, castSpell, false, EExecutionPhase.AFTER_MONSTER_ATTACK);
				m_owner.AbilityHandler.ExecuteAttackResults(m_currentTarget, m_attackResults, castSpell, false, EExecutionPhase.AFTER_MONSTER_ATTACK_INSTANT);
			}
		}

		private void DoDamage()
		{
			if (!m_owner.RangedAttack)
			{
				m_attacks.Clear();
				m_owner.AbilityHandler.ExecuteAttacks(m_currentTarget, m_attacks, m_castSpell, EExecutionPhase.BEFORE_MONSTER_ATTACK);
				Boolean flag = false;
				if (!m_castSpell)
				{
					List<Damage> list = new List<Damage>();
					for (Int32 i = 0; i < m_meleeStrikes; i++)
					{
						list.Add(m_owner.StaticData.MeleeAttackDamage * (MeleeDamageModifier + 1f));
						Damage meleeAttackElementalDamage = m_owner.StaticData.MeleeAttackElementalDamage;
						if (meleeAttackElementalDamage.Value > 0)
						{
							list.Add(meleeAttackElementalDamage * (MeleeDamageModifier + 1f));
						}
						Attack item = new Attack(m_owner.MeleeAttackValue, m_owner.StaticData.CriticalHitChanceMelee, list);
						m_attacks.Add(item);
						list.Clear();
					}
				}
				else
				{
					if (m_castedSpell == null)
					{
						return;
					}
					if (m_castedSpell.TargetType == ETargetType.PARTY)
					{
						List<Character> aliveCharacters = GetAliveCharacters();
						for (Int32 j = 0; j < aliveCharacters.Count; j++)
						{
							Attack attack = m_castedSpell.GetAttack();
							if (attack != null)
							{
								attack.AttackValue = m_owner.MeleeAttackValue;
								m_attacks.Add(attack);
							}
						}
					}
					else
					{
						Attack attack2 = m_castedSpell.GetAttack();
						if (attack2 != null)
						{
							attack2.AttackValue = m_owner.MeleeAttackValue;
							m_attacks.Add(attack2);
						}
						else
						{
							flag = true;
						}
					}
				}
				m_owner.AbilityHandler.ExecuteAttacks(null, m_attacks, m_castSpell, EExecutionPhase.BEFORE_TARGET_SELECTION);
				m_owner.AbilityHandler.FlushActionLog(EExecutionPhase.BEFORE_TARGET_SELECTION);
				Int32 num = 0;
				if (m_owner.DivideAttacksToPartyCharacters)
				{
					List<Character> aliveCharacters2 = GetAliveCharacters();
					if (aliveCharacters2.Contains(m_preselectedTarget))
					{
						m_currentTarget = m_preselectedTarget;
					}
					else
					{
						m_currentTarget = aliveCharacters2[Random.Range(0, aliveCharacters2.Count)];
					}
					num = aliveCharacters2.IndexOf(m_currentTarget);
				}
				else
				{
					m_currentTarget = GetRandomMemberFromParty();
					if (flag && m_currentTarget.ConditionHandler.HasCondition(ECondition.PARALYZED))
					{
						m_currentTarget = GetRandomConsciousPartyMember();
					}
				}
				if (m_currentTarget != null)
				{
					m_attackResults.Clear();
					Boolean flag2 = false;
					AttacksEventArgs attacksEventArgs = null;
					SpellEventArgs spellEventArgs = null;
					if (m_castSpell)
					{
						spellEventArgs = new SpellEventArgs(m_castedSpell);
						spellEventArgs.DamageType = m_castedSpell.MagicSchool;
						LegacyLogic.Instance.ActionLog.PushEntry(new CastSpellEntryEventArgs(m_owner, spellEventArgs));
					}
					else
					{
						attacksEventArgs = new AttacksEventArgs(false);
					}
					PartyBuffHandler buffs = LegacyLogic.Instance.WorldManager.Party.Buffs;
					if (m_attacks.Count > 0)
					{
						for (Int32 k = 0; k < m_attacks.Count; k++)
						{
							EDamageType p_damageType = EDamageType.PHYSICAL;
							if (m_castSpell)
							{
								p_damageType = m_castedSpell.MagicSchool;
							}
							buffs.ModifyAttack(m_attacks[k]);
							AttackResult attackResult = m_currentTarget.FightHandler.AttackEntity(m_attacks[k], !m_castSpell, p_damageType, false, 0, m_owner.AbilityHandler.HasAbility(EMonsterAbilityType.UNSTOPPABLE_STRIKES));
							if (m_owner.AbilityHandler.HasAbility(EMonsterAbilityType.PIERCING_STRIKES) && (attackResult.Result == EResultType.HIT || attackResult.Result == EResultType.CRITICAL_HIT))
							{
								Int32 p_resistanceIgnore = m_owner.AbilityHandler.CalculateResistanceIgnoreValue(attackResult);
								attackResult = m_currentTarget.FightHandler.RecalculateDamage(attackResult.Result, m_attacks[k], !m_castSpell, p_damageType, p_resistanceIgnore);
							}
							buffs.AfterAttackResult(attackResult);
							m_currentTarget.EnchantmentHandler.ResolveIncomingAttackEffects(attackResult, m_owner, m_attacks[k]);
							m_attackResults.Add(attackResult);
							if (attackResult.DamageDone > 0 && m_castSpell && !m_owner.SpellHandler.SpellTargetList.Contains(m_currentTarget))
							{
								m_owner.SpellHandler.SpellTargetList.Add(m_currentTarget);
							}
							m_owner.AbilityHandler.ExecuteAttackResult(m_currentTarget, attackResult, m_castSpell, false, EExecutionPhase.AFTER_DAMAGE_CALCULATION);
							BloodMagicEventArgs bloodMagicEventArgs = m_currentTarget.FightHandler.ChangeHealth(attackResult, m_owner);
							if (m_castSpell)
							{
								CheckAttackDirection();
								BarkEventArgs[] barkEventArgs = null;
								if (!m_currentTarget.ConditionHandler.HasCondition(ECondition.DEAD) && !m_currentTarget.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS))
								{
									barkEventArgs = new BarkEventArgs[]
									{
										m_currentTarget.BarkHandler.GenerateBarkEventArgs(EBarks.GET_HIT_RANGED),
										m_currentTarget.BarkHandler.GenerateBarkEventArgs(EBarks.GET_HIT)
									};
								}
								spellEventArgs.SpellTargets.Add(new AttackedTarget(m_currentTarget, attackResult));
								spellEventArgs.BarkEventArgs = barkEventArgs;
								spellEventArgs.BloodMagicArgs = bloodMagicEventArgs;
							}
							else
							{
								if (CheckReflectDamageByFireShield(buffs, attackResult, attacksEventArgs))
								{
									flag2 = true;
								}
								BarkEventArgs[] p_barkEventArgs = null;
								CheckAttackDirection();
								if (!m_currentTarget.ConditionHandler.HasCondition(ECondition.DEAD) && !m_currentTarget.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS))
								{
									if (attackResult.Result == EResultType.HIT)
									{
										p_barkEventArgs = new BarkEventArgs[]
										{
											m_currentTarget.BarkHandler.GenerateBarkEventArgs(EBarks.GET_HIT)
										};
									}
									else if (attackResult.Result == EResultType.CRITICAL_HIT)
									{
										p_barkEventArgs = new BarkEventArgs[]
										{
											m_currentTarget.BarkHandler.GenerateBarkEventArgs(EBarks.GET_CRIT)
										};
									}
								}
								attacksEventArgs.Attacks.Add(new AttacksEventArgs.AttackedTarget(m_currentTarget, attackResult, p_barkEventArgs, bloodMagicEventArgs));
							}
							if (m_castSpell && attackResult.Result != EResultType.EVADE)
							{
								m_castedSpell.DoEffect(m_owner, m_currentTarget, spellEventArgs);
							}
							if (m_owner.DivideAttacksToPartyCharacters)
							{
								List<Character> aliveCharacters3 = GetAliveCharacters();
								if (k == 0 && aliveCharacters3.Count > 1)
								{
									num--;
									if (num < 0)
									{
										num = aliveCharacters3.Count - 1;
									}
									if (num < 0)
									{
										num = 0;
									}
								}
								else if (k == 1 && aliveCharacters3.Count > 1)
								{
									num += 2;
									if (num >= aliveCharacters3.Count)
									{
										num -= aliveCharacters3.Count;
									}
								}
								else
								{
									num++;
									if (num >= aliveCharacters3.Count)
									{
										num = 0;
									}
								}
								if (num < 0 || num >= aliveCharacters3.Count)
								{
									m_currentTarget = null;
									break;
								}
								m_currentTarget = aliveCharacters3[num];
							}
						}
					}
					else if (m_castSpell)
					{
						AttackResult attackResult2 = new AttackResult();
						attackResult2.DamagePercentOfHealthPoints = 0f;
						attackResult2.Result = EResultType.HIT;
						spellEventArgs.SpellTargets.Add(new AttackedTarget(m_currentTarget, attackResult2));
						m_castedSpell.DoEffect(m_owner, m_currentTarget, spellEventArgs);
					}
					if (m_castSpell)
					{
						LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.MONSTER_CAST_SPELL, spellEventArgs);
					}
					else
					{
						if (flag2)
						{
							LegacyLogic.Instance.EventManager.InvokeEvent(m_currentTarget, EEventType.REFLECTED_MAGIC_DAMAGE, attacksEventArgs);
						}
						LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.MONSTER_ATTACKS, attacksEventArgs);
					}
				}
				m_owner.DivideAttacksToPartyCharacters = false;
				m_castedSpell = null;
				m_castSpell = false;
			}
			else if (!m_castSpell)
			{
				m_attacks.Clear();
				for (Int32 l = 0; l < m_rangedStrikes; l++)
				{
					List<Damage> list2 = new List<Damage>();
					list2.Add(m_owner.StaticData.RangedAttackDamage * (RangeDamageModifier + 1f));
					Damage rangedAttackElementalDamage = m_owner.StaticData.RangedAttackElementalDamage;
					if (rangedAttackElementalDamage.Value > 0)
					{
						list2.Add(rangedAttackElementalDamage * (MeleeDamageModifier + 1f));
					}
					Attack item2 = new Attack(m_owner.RangedAttackValue, m_owner.StaticData.CriticalHitChanceRanged, list2);
					m_attacks.Add(item2);
				}
				m_currentTarget = GetRandomMemberFromParty();
				if (m_currentTarget != null)
				{
					m_owner.AbilityHandler.ExecuteAttacks(m_currentTarget, m_attacks, false, EExecutionPhase.BEFORE_MONSTER_ATTACK);
					m_attackResults.Clear();
					AttacksEventArgs attacksEventArgs2 = new AttacksEventArgs(false);
					PartyBuffHandler buffs2 = LegacyLogic.Instance.WorldManager.Party.Buffs;
					for (Int32 m = 0; m < m_attacks.Count; m++)
					{
						buffs2.ModifyAttack(m_attacks[m]);
						AttackResult attackResult3 = m_currentTarget.FightHandler.AttackEntity(m_attacks[m], false, EDamageType.PHYSICAL);
						buffs2.AfterAttackResult(attackResult3);
						BarkEventArgs[] p_barkEventArgs2 = null;
						CheckAttackDirection();
						if (!m_currentTarget.ConditionHandler.HasCondition(ECondition.DEAD) && !m_currentTarget.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS))
						{
							p_barkEventArgs2 = new BarkEventArgs[]
							{
								m_currentTarget.BarkHandler.GenerateBarkEventArgs(EBarks.GET_HIT_RANGED),
								m_currentTarget.BarkHandler.GenerateBarkEventArgs(EBarks.GET_HIT)
							};
						}
						BloodMagicEventArgs bloodMagicEventArgs = m_currentTarget.FightHandler.ChangeHealth(attackResult3, m_owner);
						attacksEventArgs2.Attacks.Add(new AttacksEventArgs.AttackedTarget(m_currentTarget, attackResult3, p_barkEventArgs2, bloodMagicEventArgs));
						m_attackResults.Add(attackResult3);
					}
					LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.MONSTER_ATTACKS_RANGED, attacksEventArgs2);
				}
			}
		}

		private Boolean CheckReflectDamageByFireShield(PartyBuffHandler p_partyBuffHandler, AttackResult p_attackResult, AttacksEventArgs p_attackArgs)
		{
			PartyBuff buff = p_partyBuffHandler.GetBuff(EPartyBuffs.FIRE_SHIELD);
			if (buff != null)
			{
				Int32 calculatedValue = buff.GetCalculatedValue(buff.StaticData.SpecificValue[0]);
				Int32 calculatedValue2 = buff.GetCalculatedValue(buff.StaticData.SpecificValue[1]);
				Int32 num = Random.Range(calculatedValue, calculatedValue2 + 1);
				Damage p_damages = new Damage(EDamageType.FIRE, num, 0f, 1f);
				p_attackResult.ReflectedDamage = new Attack(1f, 0f, p_damages);
				p_attackResult.ReflectedDamageSource = buff;
				m_owner.ChangeHP(-p_damages.Value, m_currentTarget);
				AttackResult attackResult = new AttackResult();
				DamageResult item = new DamageResult(EDamageType.FIRE, num, 0, 1f);
				attackResult.DamageResults.Add(item);
				p_attackArgs.Attacks.Add(new AttacksEventArgs.AttackedTarget(m_owner, attackResult));
				return true;
			}
			return false;
		}

		private void CheckAttackDirection()
		{
			EDirection direction = m_owner.Direction;
			EDirection direction2 = LegacyLogic.Instance.WorldManager.Party.Direction;
			Int32 num = DirectionToDegree(direction);
			Int32 num2 = DirectionToDegree(direction2);
			Int32 num3 = num - num2;
			num3 = Math.Abs(num3);
			Int32 num4 = num3;
			if (num4 != 0)
			{
				if (num4 != 90)
				{
					if (num4 != 180)
					{
						if (num4 == 270)
						{
							LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.HIT_SIDE);
						}
					}
				}
				else
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.HIT_SIDE);
				}
			}
			else
			{
				LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.HIT_BACK);
			}
		}

		private Int32 DirectionToDegree(EDirection p_direction)
		{
			Int32 result = 0;
			switch (p_direction)
			{
			case EDirection.NORTH:
				result = 0;
				break;
			case EDirection.EAST:
				result = 90;
				break;
			case EDirection.SOUTH:
				result = 180;
				break;
			case EDirection.WEST:
				result = 270;
				break;
			}
			return result;
		}

		public void TriggerCounterAttacks(Object p_target, AttackResult p_result)
		{
			if (m_currentTarget == null)
			{
				return;
			}
			Character character = p_target as Character;
			if (character != null)
			{
				character.SkillHandler.OnMonsterStrikeMelee(m_owner, p_result);
			}
			List<Attack> list = m_currentTarget.EnchantmentHandler.DealDamageToAttacker(m_owner);
			Boolean flag = false;
			AttacksEventArgs attacksEventArgs = new AttacksEventArgs(false);
			foreach (Attack attack in list)
			{
				foreach (Damage damage in attack.Damages)
				{
					AttackResult attackResult = AttackMonster(m_currentTarget, attack, true, false, damage.Type, true, 0);
					m_owner.ApplyDamages(attackResult, character);
					attacksEventArgs.Attacks.Add(new AttacksEventArgs.AttackedTarget(m_owner, attackResult));
					flag = true;
				}
			}
			if (flag)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(m_currentTarget, EEventType.REFLECTED_MAGIC_DAMAGE, attacksEventArgs);
			}
		}

		private static List<Character> GetAliveCharacters()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			return party.GetCharactersAlive();
		}

		protected virtual Character GetRandomConsciousPartyMember()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			List<Character> fightingCharacters = party.GetFightingCharacters();
			m_owner.AiHandler.FilterPossibleTargets(fightingCharacters);
			for (Int32 i = fightingCharacters.Count - 1; i >= 0; i--)
			{
				if (fightingCharacters[i].ConditionHandler.HasCondition(ECondition.UNCONSCIOUS))
				{
					fightingCharacters.RemoveAt(i);
				}
			}
			if (fightingCharacters.Count <= 0)
			{
				return null;
			}
			if (fightingCharacters.Count == 1)
			{
				return fightingCharacters[0];
			}
			Character character = null;
			if (m_preselectedTarget != null && fightingCharacters.Contains(m_preselectedTarget))
			{
				character = m_preselectedTarget;
			}
			if (character == null)
			{
				character = fightingCharacters[Random.Range(0, fightingCharacters.Count)];
			}
			if (character.FightHandler.InterceptingCharacter != null && !character.FightHandler.InterceptingCharacter.ConditionHandler.CantDoAnything())
			{
				character = character.FightHandler.InterceptingCharacter;
			}
			return character;
		}

		protected virtual Character GetRandomMemberFromParty()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			List<Character> charactersAlive = party.GetCharactersAlive();
			m_owner.AiHandler.FilterPossibleTargets(charactersAlive);
			if (charactersAlive.Count == 0)
			{
				charactersAlive = party.GetCharactersAlive();
			}
			if (charactersAlive.Count <= 0)
			{
				return null;
			}
			if (charactersAlive.Count == 1)
			{
				return charactersAlive[0];
			}
			Character character = null;
			if (m_preselectedTarget != null && charactersAlive.Contains(m_preselectedTarget))
			{
				character = m_preselectedTarget;
			}
			if (character == null)
			{
				character = charactersAlive[Random.Range(0, charactersAlive.Count)];
			}
			if (character.FightHandler.InterceptingCharacter != null && !character.FightHandler.InterceptingCharacter.ConditionHandler.CantDoAnything())
			{
				character = character.FightHandler.InterceptingCharacter;
			}
			return character;
		}

		private Boolean TestBlock(Boolean p_melee)
		{
			Single generalBlockChance = m_owner.StaticData.GeneralBlockChance;
			Int32 num = m_owner.CurrentMeleeBlockAttempts;
			if (p_melee && num > 0)
			{
				Boolean result = TestBlock(generalBlockChance, ref num, false);
				m_owner.CurrentMeleeBlockAttempts = num;
				return result;
			}
			num = m_owner.CurrentGeneralBlockAttempts;
			if (num > 0)
			{
				Boolean result2 = TestBlock(generalBlockChance, ref num, false);
				m_owner.CurrentGeneralBlockAttempts = num;
				return result2;
			}
			return false;
		}

		internal void CalculateDamage(List<Damage> p_damages, Boolean p_critical, List<DamageResult> p_damageResults)
		{
			for (Int32 i = 0; i < p_damages.Count; i++)
			{
				Damage damage = p_damages[i];
				Resistance resistanceByType = GetResistanceByType(damage.Type);
				DamageResult item = DamageResult.Create(damage, resistanceByType);
				if (p_critical)
				{
					item.EffectiveValue = (Int32)Math.Round(item.EffectiveValue * (1f + damage.CriticalBonusMod), MidpointRounding.AwayFromZero);
					item.ResistedValue = (Int32)Math.Round(item.ResistedValue * (1f + damage.CriticalBonusMod), MidpointRounding.AwayFromZero);
				}
				p_damageResults.Add(item);
			}
		}

		public override Resistance GetResistanceByType(EDamageType p_type)
		{
			Int32 num = 0;
			MonsterBuff buff = m_owner.BuffHandler.GetBuff(EMonsterBuffType.CRYSTAL_SHELL);
			if (buff != null)
			{
				num = (Int32)buff.GetBuffValue(1);
			}
			Int32 p_value = 0;
			if (p_type == EDamageType.PHYSICAL)
			{
				p_value = (Int32)m_owner.CombatHandler.ArmorValue;
			}
			for (Int32 i = 0; i < m_owner.StaticData.MagicResistances.Length; i++)
			{
				if (m_owner.StaticData.MagicResistances[i].Type == p_type)
				{
					p_value = m_owner.StaticData.MagicResistances[i].Value + num;
				}
			}
			return new Resistance(p_type, p_value);
		}

		public Attack GetSingleMeleeAttack()
		{
			List<Damage> list = new List<Damage>();
			list.Add(m_owner.StaticData.MeleeAttackDamage * (MeleeDamageModifier + 1f));
			Damage meleeAttackElementalDamage = m_owner.StaticData.MeleeAttackElementalDamage;
			if (meleeAttackElementalDamage.Value > 0)
			{
				list.Add(meleeAttackElementalDamage * (MeleeDamageModifier + 1f));
			}
			return new Attack(m_owner.MeleeAttackValue, m_owner.StaticData.CriticalHitChanceMelee, list);
		}

		private void SendRetaliationEvent()
		{
		}
	}
}
