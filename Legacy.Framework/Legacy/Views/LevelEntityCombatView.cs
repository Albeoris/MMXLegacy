using System;
using System.Collections.Generic;
using Legacy.Animations;
using Legacy.Core.Abilities;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Level Entity Combat View")]
	public class LevelEntityCombatView : CombatViewBase
	{
		[SerializeField]
		private Animator m_animator;

		[SerializeField]
		private AnimatorControl m_animatorControl;

		private LevelEntityView m_MainView;

		private Boolean m_InitAggroAnim;

		protected override void Awake()
		{
			base.Awake();
			if (m_animator == null)
			{
				m_animator = this.GetComponent<Animator>(true);
			}
			if (m_animatorControl == null)
			{
				m_animatorControl = this.GetComponent<AnimatorControl>(true);
			}
			m_animator.cullingMode = AnimatorCullingMode.BasedOnRenderers;
			m_MainView = this.GetComponent< LevelEntityView>(true);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.REFLECTED_MAGIC_DAMAGE, new EventHandler(OnReceiveReflectedDamage));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttackOrCastSpell));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttackOrCastSpell));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterAttackOrCastSpell));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SUMMON_CAST_SPELL, new EventHandler(OnCharacterAttackOrCastSpell));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS, new EventHandler(OnEntityTakeHitAttack_OnFxHit));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnEntityTakeHitAttackRanged_OnFxHit));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnEntityTakeHitCastSpell_OnFxHit));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.REFLECTED_MAGIC_DAMAGE, new EventHandler(OnReceiveReflectedDamage));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttackOrCastSpell));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttackOrCastSpell));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterAttackOrCastSpell));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SUMMON_CAST_SPELL, new EventHandler(OnCharacterAttackOrCastSpell));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS, new EventHandler(OnEntityTakeHitAttack_OnFxHit));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnEntityTakeHitAttackRanged_OnFxHit));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnEntityTakeHitCastSpell_OnFxHit));
			}
		}

		public override void Attack(FXArgs p_fxArgs, Action p_fxFinishedCallback)
		{
			m_CommandQueue.Enqueue(delegate
			{
				m_State.ChangeState(EState.IDLE, Random.Range(0f, 0.1f));
			});
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.ATTACK, p_fxArgs);
				HandleMeleeFX(p_fxArgs);
				m_AnimationEventHandler.RegisterAnimationCallback(EAnimEventType.HIT, delegate
				{
					ChangeStateFromAttackToIdle(p_fxFinishedCallback);
				});
				Single length = m_animator.GetCurrentAnimatorStateInfo(0).length;
				m_State.ChangeState(EState.ATTACK, length);
				m_animatorControl.Attack();
			});
		}

		public override void AttackRanged(FXArgs p_fxArgs, Action p_fxFinishedCallback, EResultType p_Result)
		{
			m_CommandQueue.Enqueue(delegate
			{
				m_State.ChangeState(EState.IDLE, Random.Range(0f, 0.1f));
			});
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.ATTACK_RANGED, p_fxArgs);
				HandleRangedFX(p_Result, p_fxArgs, p_fxFinishedCallback);
				Single length = m_animator.GetCurrentAnimatorStateInfo(0).length;
				m_State.ChangeState(EState.ATTACK_RANGED, length);
				m_animatorControl.AttackRange();
			});
		}

		public override void AttackCritical(FXArgs p_fxArgs, Action p_fxFinishedCallback)
		{
			m_CommandQueue.Enqueue(delegate
			{
				m_State.ChangeState(EState.IDLE, Random.Range(0f, 0.1f));
			});
			m_CommandQueue.Enqueue(delegate
			{
				HandleAnimationOverrideFX(EAnimType.ATTACK_CRITICAL, p_fxArgs);
				HandleMeleeFX(p_fxArgs);
				m_AnimationEventHandler.RegisterAnimationCallback(EAnimEventType.HIT, delegate
				{
					ChangeStateFromAttackToIdle(p_fxFinishedCallback);
				});
				Single length = m_animator.GetCurrentAnimatorStateInfo(0).length;
				m_State.ChangeState(EState.ATTACK, length);
				if (m_animatorControl.AttackCriticalMeleeMaxValue != 0)
				{
					m_animatorControl.AttackCritical();
				}
				else
				{
					m_animatorControl.Attack();
				}
			});
		}

		protected override void Update()
		{
			base.Update();
			Monster monster = MyController as Monster;
			if (monster != null)
			{
				m_animatorControl.InCombat = monster.IsAggro;
				if (monster.IsAggro)
				{
					if (!m_InitAggroAnim && ConfigManager.Instance.Options.MonsterMovementSpeed == 1f)
					{
						m_InitAggroAnim = true;
						m_animatorControl.EventSummon(2);
					}
				}
				else
				{
					m_InitAggroAnim = false;
				}
			}
		}

		private void OnReceiveReflectedDamage(Object p_sender, EventArgs p_args)
		{
			if (p_args is AttacksEventArgs)
			{
				Boolean flag = false;
				AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
				foreach (AttacksEventArgs.AttackedTarget item in attacksEventArgs.Attacks)
				{
					if (item.AttackTarget == MyController)
					{
						m_reflectedMagicDamage.Add(item);
						flag = true;
					}
				}
				UpdateManager updateManager = LegacyLogic.Instance.UpdateManager;
				if (updateManager.CurrentTurnActor == updateManager.PartyTurnActor && flag && MyController is Monster)
				{
					CheckForReflectedDamage();
					m_reflectedMagicDamage.Clear();
				}
			}
		}

		private void OnCharacterAttackOrCastSpell(Object p_sender, EventArgs p_args)
		{
			Monster monster = MyController as Monster;
			Int32 num = monster.CurrentHealth;
			Boolean flag = false;
			Boolean flag2 = false;
			if (p_sender is Summon)
			{
				OnEntityTakeHitCastSpell_OnFxHit(p_sender, p_args);
				return;
			}
			if (p_args is AttacksEventArgs)
			{
				AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
				foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
				{
					if (attackedTarget.AttackTarget == MyController)
					{
						EResultType result = attackedTarget.AttackResult.Result;
						if (result == EResultType.EVADE)
						{
							flag2 = true;
						}
						else if (result == EResultType.BLOCK)
						{
							flag = true;
						}
						if (result != EResultType.BLOCK && result != EResultType.EVADE)
						{
							num -= attackedTarget.AttackResult.DamageDone;
						}
					}
				}
				if (flag && num > 0)
				{
					m_animatorControl.Block();
					return;
				}
				if (flag2 && num > 0)
				{
					m_animatorControl.Evade();
					return;
				}
			}
		}

		private void OnEntityTakeHitAttack_OnFxHit(Object p_sender, EventArgs p_args)
		{
			TakeHit(p_sender, p_args, false);
		}

		private void OnEntityTakeHitAttackRanged_OnFxHit(Object p_sender, EventArgs p_args)
		{
			TakeHit(p_sender, p_args, true);
		}

		private void OnEntityTakeHitCastSpell_OnFxHit(Object p_sender, EventArgs p_args)
		{
			TakeHit(p_sender, p_args, false);
		}

		private void TakeHit(Object p_sender, EventArgs p_args, Boolean p_isRanged)
		{
			Int32 tagHash = m_animator.GetCurrentAnimatorStateInfo(0).tagHash;
			Int32 tagHash2 = m_animator.GetNextAnimatorStateInfo(0).tagHash;
			Int32 num = Animator.StringToHash("ATTACK");
			Int32 num2 = Animator.StringToHash("DIE");
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
						if (flag)
						{
							flag = false;
							((Monster)MyController).HitAnimationDone.Trigger();
							FlushMonsterLogEntries();
							DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, (!p_isRanged) ? EEventType.CHARACTER_ATTACKS : EEventType.CHARACTER_ATTACKS_RANGED, p_sender, p_args);
							if (attackedTarget.AttackResult.DamageDone > 0 && num2 != tagHash && num2 != tagHash2 && num != tagHash && num != tagHash2)
							{
								m_animatorControl.Hit();
							}
							PlayTakeHitSound(p_sender, attackResult, p_isRanged);
						}
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
							}
						}
						if (attackResult.ReflectedDamage != null && attackResult.ReflectedDamage.Damages.Count > 0 && MyController is Monster && (!p_isRanged || (p_isRanged && ((Monster)MyController).DistanceToParty < 1.1f)))
						{
							((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_AFTER_DAMAGE_REDUCTION);
							AttackResult attackResult2 = ((Character)p_sender).FightHandler.AttackEntity(attackResult.ReflectedDamage, true, EDamageType.PHYSICAL, true, 0, false);
							((Character)p_sender).ApplyDamages(attackResult2, (Monster)MyController);
							Object p_source = (attackResult2.ReflectedDamageSource != null) ? attackResult2.ReflectedDamageSource : MyController;
							CombatEntryEventArgs p_args3 = new CombatEntryEventArgs(p_source, p_sender, attackResult2, null);
							LegacyLogic.Instance.ActionLog.PushEntry(p_args3);
							DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, EEventType.REFLECTED_MAGIC_DAMAGE, p_sender, new AttacksEventArgs(false)
							{
								Attacks = 
								{
									new AttacksEventArgs.AttackedTarget(p_sender, attackResult2)
								}
							});
						}
						this.SendEvent("OnReceivedAttacks", new AttacksUnityEventArgs(this, attackResult, false));
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
						if (flag)
						{
							flag = false;
							TakeHitDoFinishBySpell(p_sender, p_args);
							if (attackedTarget3.Result.DamageDone > 0 && num2 != tagHash && num2 != tagHash2 && num != tagHash && num != tagHash2)
							{
								m_animatorControl.Hit();
							}
						}
						if (attackedTarget3.Result.ReflectedDamage != null && attackedTarget3.Result.ReflectedDamage.Damages.Count > 0 && MyController is Monster && (!p_isRanged || (p_isRanged && ((Monster)MyController).DistanceToParty < 1.1f)))
						{
							((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_AFTER_DAMAGE_REDUCTION);
							AttackResult attackResult3 = ((Character)p_sender).FightHandler.AttackEntity(attackedTarget3.Result.ReflectedDamage, true, EDamageType.PHYSICAL, true, 0, false);
							((Character)p_sender).ApplyDamages(attackResult3, (Monster)MyController);
							Object p_source2 = (attackResult3.ReflectedDamageSource != null) ? attackResult3.ReflectedDamageSource : MyController;
							CombatEntryEventArgs p_args4 = new CombatEntryEventArgs(p_source2, p_sender, attackResult3, null);
							LegacyLogic.Instance.ActionLog.PushEntry(p_args4);
							DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, EEventType.REFLECTED_MAGIC_DAMAGE, p_sender, new AttacksEventArgs(false)
							{
								Attacks = 
								{
									new AttacksEventArgs.AttackedTarget(p_sender, attackResult3)
								}
							});
						}
						this.SendEvent("OnReceivedAttacks", new AttacksUnityEventArgs(this, attackedTarget3.Result, spellEventArgs.DamageType != EDamageType.PHYSICAL));
					}
				}
				foreach (PushedTarget pushedTarget in spellEventArgs.SpellTargetsOfType<PushedTarget>())
				{
					if (pushedTarget.Target == MyController)
					{
						m_MainView.PushEntityToPosition();
					}
				}
				foreach (MonsterBuffTarget monsterBuffTarget in spellEventArgs.SpellTargetsOfType<MonsterBuffTarget>())
				{
					if (flag && monsterBuffTarget.Target == MyController)
					{
						flag = false;
						TakeHitDoFinishBySpell(p_sender, p_args);
					}
				}
			}
		}

		private void TakeHitDoFinishBySpell(Object p_sender, EventArgs p_args)
		{
			((Monster)MyController).HitAnimationDone.Trigger();
			if (p_sender is Summon)
			{
				((Summon)p_sender).FlushActionLog();
			}
			FlushMonsterLogEntries();
			if (!(p_sender is Summon))
			{
				DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_CAST_SPELL, p_sender, p_args);
			}
		}
	}
}
