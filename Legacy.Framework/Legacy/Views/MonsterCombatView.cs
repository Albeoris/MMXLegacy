using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Animations;
using Legacy.Core.Abilities;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.AI.MonsterBehaviours;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Monster Combat View")]
	public class MonsterCombatView : CombatViewBase
	{
		[SerializeField]
		private AnimHandler m_AnimationHandler;

		[SerializeField]
		private MonsterDieView m_MonsterDieView;

		private MonsterEntityView m_MainView;

		private Boolean m_IsDead;

		private Int32 m_LastAttackOrSpellFrameCount = -1;

		public Boolean InAction => !m_State.IsState(EState.IDLE);

	    public Single ActionTime => m_State.IsState(EState.IDLE) ? 0f : m_State.CurrentStateTime;

	    public Single ActionDuration => m_State.IsState(EState.IDLE) ? 0f : m_State.CurrentStateDuration;

	    public AnimHandler AnimationHandler => m_AnimationHandler;

	    public override void Attack(FXArgs p_fxArgs, Action p_fxFinishedCallback)
		{
			m_CommandQueue.Enqueue(delegate
			{
				m_State.ChangeState(EState.IDLE, Random.Range(0f, 0.3f));
			});
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.ATTACK, p_fxArgs);
				m_AnimationEventHandler.RegisterAnimationCallback(EAnimEventType.HIT, delegate
				{
					ChangeStateFromAttackToIdle(p_fxFinishedCallback);
				});
				PlayAnimation(EAnimType.ATTACK, EState.ATTACK, -1f, 1f);
			});
		}

		public override void AttackRanged(FXArgs p_fxArgs, Action p_fxFinishedCallback, EResultType p_Result)
		{
			m_CommandQueue.Enqueue(delegate
			{
				m_State.ChangeState(EState.IDLE, Random.Range(0f, 0.3f));
			});
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.ATTACK_RANGED, p_fxArgs);
				HandleRangedFX(p_Result, p_fxArgs, p_fxFinishedCallback);
				PlayAnimation(EAnimType.ATTACK_RANGED, EState.ATTACK_RANGED, -1f, 1f, 2f);
			});
		}

		public override void AttackCritical(FXArgs p_fxArgs, Action p_fxFinishedCallback)
		{
			m_CommandQueue.Enqueue(delegate
			{
				m_State.ChangeState(EState.IDLE, Random.Range(0f, 0.3f));
			});
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.ATTACK_CRITICAL, p_fxArgs);
				m_AnimationEventHandler.RegisterAnimationCallback(EAnimEventType.HIT, delegate
				{
					ChangeStateFromAttackToIdle(p_fxFinishedCallback);
				});
				PlayAnimation(EAnimType.ATTACK_CRITICAL, EState.ATTACK, -1f, 1f);
			});
		}

		public void Hit(FXArgs p_fxArgs)
		{
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.HIT, p_fxArgs);
				PlayAnimation(EAnimType.HIT, EState.HIT, -1f, 1f);
			});
		}

		public void Defend(FXArgs p_fxArgs)
		{
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.DEFEND, p_fxArgs);
				PlayAnimation(EAnimType.DEFEND, EState.HIT, -1f, 1f);
			});
		}

		public void Ability(FXArgs p_fxArgs, Action p_fxFinishedCallback)
		{
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.ABILITY1, p_fxArgs);
				PlayAnimation(EAnimType.ABILITY1, EState.HIT, -1f, 1f);
				m_AnimationEventHandler.RegisterAnimationCallback(EAnimEventType.HIT, delegate
				{
					if (p_fxFinishedCallback != null)
					{
						p_fxFinishedCallback();
					}
				});
			});
		}

		public void PAX_AttackRanged(FXArgs p_fxArgs, Action p_fxFinishedCallback)
		{
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.ATTACK_RANGED, p_fxArgs);
				PlayAnimation(EAnimType.ATTACK_RANGED, EState.IDLE, -1f, 1f);
				m_AnimationEventHandler.RegisterAnimationCallback(EAnimEventType.RANGE_SHOOT, delegate
				{
					if (p_fxFinishedCallback != null)
					{
						p_fxFinishedCallback();
					}
				});
			});
		}

		public void FX_HitNow(FXArgs p_fxArgs)
		{
			HandleAnimationOverrideFX(EAnimType.HIT, p_fxArgs);
			PlayAnimation(EAnimType.HIT, EState.HIT, -1f, 1f);
		}

		private void TakeHit(AttackResult attackResult, Boolean p_isMagical)
		{
			this.SendEvent("OnReceivedAttacks", new AttacksUnityEventArgs(this, attackResult, p_isMagical));
			FlushMonsterLogEntries();
			if (m_IsDead)
			{
				m_MonsterDieView.Die();
			}
			if (attackResult.Result != EResultType.BLOCK && attackResult.Result != EResultType.EVADE)
			{
				PlayAnimation(EAnimType.HIT, EState.HIT, -1f, 1f);
			}
		}

		public Single PlayAnimation(EAnimType type, EState changedState, Single duration, Single speed)
		{
			return PlayAnimation(type, changedState, duration, speed, 0f);
		}

		public Single PlayAnimation(EAnimType type, EState changedState, Single duration, Single speed, Single stateExtraTime)
		{
			if (m_AnimationHandler.IsPlaying(EAnimType.DIE) || (m_IsDead && type != EAnimType.DIE))
			{
				m_State.ChangeState(changedState, 0f);
				return 0f;
			}
			AnimationState state = m_AnimationHandler.GetState(type);
			if (state != null)
			{
				if (duration == -1f)
				{
					duration = state.length / speed;
				}
				m_AnimationHandler.Play(type, duration);
				m_State.ChangeState(changedState, duration + stateExtraTime);
				return duration;
			}
			m_State.ChangeState(changedState, 0f);
			Debug.LogError(String.Concat(new Object[]
			{
				"Missing '",
				type,
				"' animation!\n",
				name
			}), this);
			return 0f;
		}

		protected override void Awake()
		{
			base.Awake();
			if (m_AnimationEventHandler == null)
			{
				m_AnimationEventHandler = this.GetComponentInChildren<BaseEventHandler>(true);
			}
			if (m_AnimationHandler == null)
			{
				m_AnimationHandler = this.GetComponent<AnimHandler>(true);
			}
			m_MainView = this.GetComponent< MonsterEntityView>(true);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.REFLECTED_MAGIC_DAMAGE, new EventHandler(OnReceiveReflectedDamage));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttackOrSpell));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttackOrSpell));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterAttackOrSpell));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SUMMON_CAST_SPELL, new EventHandler(OnCharacterAttackOrSpell));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS, new EventHandler(OnTakeHitAttack_OnFxHit));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnTakeHitAttackRanged_OnFxHit));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnTakeHitSpell_OnFxHit));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.REFLECTED_MAGIC_DAMAGE, new EventHandler(OnReceiveReflectedDamage));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttackOrSpell));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttackOrSpell));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterAttackOrSpell));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SUMMON_CAST_SPELL, new EventHandler(OnCharacterAttackOrSpell));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS, new EventHandler(OnTakeHitAttack_OnFxHit));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnTakeHitAttackRanged_OnFxHit));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnTakeHitSpell_OnFxHit));
			}
		}

		private void OnReceiveReflectedDamage(Object p_sender, EventArgs p_args)
		{
			Debug.LogError("MonsterCombatView: obseletate, should not be used! This function has a bug! Use LevelEntityCombatView instead!");
			if (p_args is AttacksEventArgs)
			{
				AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
				foreach (AttacksEventArgs.AttackedTarget item in attacksEventArgs.Attacks)
				{
					if (item.AttackTarget == MyController)
					{
						m_reflectedMagicDamage.Add(item);
					}
				}
			}
		}

		private void OnTakeHitAttack_OnFxHit(Object p_sender, EventArgs p_args)
		{
			OnTakeHitAttackGeneric(p_sender, p_args, EEventType.CHARACTER_ATTACKS);
		}

		private void OnTakeHitAttackRanged_OnFxHit(Object p_sender, EventArgs p_args)
		{
			OnTakeHitAttackGeneric(p_sender, p_args, EEventType.CHARACTER_ATTACKS_RANGED);
		}

		private void OnTakeHitSpell_OnFxHit(Object p_sender, EventArgs p_args)
		{
			OnTakeHitAttackGeneric(p_sender, p_args, EEventType.CHARACTER_CAST_SPELL);
		}

		private void OnTakeHitAttackGeneric(Object p_sender, EventArgs p_args, EEventType p_eventType)
		{
			Boolean flag = true;
			Boolean flag2 = false;
			if (p_args is AttacksEventArgs)
			{
				AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
				foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
				{
					if (attackedTarget.AttackTarget == MyController)
					{
						AttackResult attackResult = attackedTarget.AttackResult;
						if (!flag2 && attacksEventArgs.PushToParty)
						{
							flag2 = true;
							m_MainView.PushEntityToPosition();
						}
						foreach (AttacksEventArgs.AttackedTarget attackedTarget2 in attacksEventArgs.Attacks)
						{
							if (attackedTarget2.AttackTarget != null && attackedTarget2.AttackTarget == MyController)
							{
								Monster monster = (Monster)MyController;
								FlushMonsterLogEntries();
								monster.CombatHandler.CheckCounterAttack((Character)p_sender);
								List<CombatEntryEventArgs> counterLogEntries = monster.CombatHandler.CounterLogEntries;
								if (counterLogEntries.Count > 0)
								{
									foreach (CombatEntryEventArgs p_args2 in counterLogEntries)
									{
										LegacyLogic.Instance.ActionLog.PushEntry(p_args2);
										if (LegacyLogic.Instance.WorldManager.Party.Buffs.HasBuff(EPartyBuffs.CELESTIAL_ARMOR))
										{
											if (LegacyLogic.Instance.WorldManager.Party.Buffs.GetBuff(EPartyBuffs.CELESTIAL_ARMOR).IsExpired())
											{
												LegacyLogic.Instance.WorldManager.Party.Buffs.RemoveBuff(EPartyBuffs.CELESTIAL_ARMOR);
											}
											LegacyLogic.Instance.WorldManager.Party.Buffs.FlushActionLog();
										}
										((Character)p_sender).ConditionHandler.FlushActionLog();
										((Character)p_sender).ConditionHandler.FlushDelayedActionLog();
									}
								}
								counterLogEntries.Clear();
								if (monster.AiHandler is MamushiAIHandler)
								{
									((Character)p_sender).FightHandler.FlushActionLog();
									((Character)p_sender).FightHandler.FlushDelayedActionLog();
									((Character)p_sender).FlushNormalActionLog();
								}
							}
						}
						if (attackResult.ReflectedDamage != null && attackResult.ReflectedDamage.Damages.Count > 0 && MyController is Monster)
						{
							Monster p_attacker = MyController as Monster;
							AttackResult attackResult2 = ((Character)p_sender).FightHandler.AttackEntity(attackResult.ReflectedDamage, true, EDamageType.PHYSICAL, true, 0, false);
							((Character)p_sender).ApplyDamages(attackResult2, p_attacker);
							Object p_source = (attackResult2.ReflectedDamageSource != null) ? attackResult2.ReflectedDamageSource : MyController;
							CombatEntryEventArgs p_args3 = new CombatEntryEventArgs(p_source, p_sender, attackResult2, null);
							LegacyLogic.Instance.ActionLog.PushEntry(p_args3);
							AttacksEventArgs attacksEventArgs2 = new AttacksEventArgs(false);
							attacksEventArgs2.Attacks.Add(new AttacksEventArgs.AttackedTarget(p_sender, attackResult2));
							DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, EEventType.REFLECTED_MAGIC_DAMAGE, p_sender, attacksEventArgs2);
						}
						if (flag)
						{
							flag = false;
							m_CommandQueue.Enqueue(delegate
							{
								((Monster)MyController).HitAnimationDone.Trigger();
								DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, p_eventType, p_sender, p_args);
							});
							PlayTakeHitSound(p_sender, attackResult, p_eventType == EEventType.CHARACTER_ATTACKS_RANGED);
						}
						TakeHit(attackedTarget.AttackResult, false);
					}
				}
			}
			else if (p_args is SpellEventArgs)
			{
				SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
				foreach (AttackedTarget attackedTarget3 in spellEventArgs.SpellTargetsOfType<AttackedTarget>())
				{
					if (attackedTarget3.Target == MyController)
					{
						if (p_sender is Summon)
						{
							((Summon)p_sender).FlushActionLog();
						}
						FlushMonsterLogEntries();
						if (flag)
						{
							flag = false;
							m_CommandQueue.Enqueue(delegate
							{
								((Monster)MyController).HitAnimationDone.Trigger();
								if (!(p_sender is Summon))
								{
									DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_CAST_SPELL, p_sender, p_args);
								}
							});
						}
						TakeHit(attackedTarget3.Result, spellEventArgs.DamageType != EDamageType.PHYSICAL);
					}
				}
				foreach (PushedTarget pushedTarget in spellEventArgs.SpellTargetsOfType<PushedTarget>())
				{
					if (pushedTarget.Target == MyController)
					{
						m_MainView.PushEntityToPosition();
					}
				}
			}
		}

		private void OnCharacterAttackOrSpell(Object p_sender, EventArgs p_args)
		{
			if (p_args is AttacksEventArgs)
			{
				AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
				foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
				{
					if (attackedTarget.AttackTarget == MyController)
					{
						m_LastAttackOrSpellFrameCount = Time.frameCount;
						EResultType result = attackedTarget.AttackResult.Result;
						if (result == EResultType.BLOCK || result == EResultType.EVADE)
						{
							PlayAnimation(EAnimType.DEFEND, EState.HIT, -1f, 1f);
							break;
						}
					}
				}
			}
			else if (p_args is SpellEventArgs)
			{
				if (p_sender is Summon)
				{
					OnTakeHitSpell_OnFxHit(p_sender, p_args);
					return;
				}
				SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
				foreach (AttackedTarget attackedTarget2 in spellEventArgs.SpellTargetsOfType<AttackedTarget>())
				{
					if (attackedTarget2.Target == MyController)
					{
						m_LastAttackOrSpellFrameCount = Time.frameCount;
						break;
					}
				}
			}
		}

		private void OnMonsterDied(Object p_sender, EventArgs p_args)
		{
			if (p_sender != MyController)
			{
				return;
			}
			if (MyController is Monster)
			{
				((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.ON_APPLY_MONSTER_BUFF);
				((Monster)MyController).BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE);
				((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.MONSTER_DIES);
			}
			m_IsDead = true;
			StartCoroutine_Auto(DieLater());
		}

		private IEnumerator DieLater()
		{
			Boolean dontDie = false;
			for (Int32 i = 0; i < 3; i++)
			{
				dontDie |= (m_LastAttackOrSpellFrameCount == Time.frameCount);
				if (!dontDie)
				{
					yield return new WaitForEndOfFrame();
				}
				dontDie |= (m_LastAttackOrSpellFrameCount == Time.frameCount);
			}
			if (!dontDie)
			{
				m_MonsterDieView.Die();
			}
			yield break;
		}
	}
}
