using System;
using System.Collections.Generic;
using Legacy.Animations;
using Legacy.Core.Abilities;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Utilities.StateManagement;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public abstract class CombatViewBase : BaseView
	{
		[SerializeField]
		protected BaseEventHandler m_AnimationEventHandler;

		[SerializeField]
		protected String m_EffectMeleeAttackPath = String.Empty;

		[SerializeField]
		protected String m_EffectAttackRangedPath = "FXDesc/CrossbowMan_Arrow";

		[SerializeField]
		protected String m_EffectAttackRangedBlockedPath = "FXDesc/CrossbowMan_ArrowBlocked";

		protected Queue<Action> m_CommandQueue = new Queue<Action>();

		protected TimeStateMachine<EState> m_State;

		protected FXDefinitions m_Effects;

		protected List<AttacksEventArgs.AttackedTarget> m_reflectedMagicDamage = new List<AttacksEventArgs.AttackedTarget>();

		public CombatViewBase()
		{
			m_State = new TimeStateMachine<EState>();
			m_State.AddState(new TimeState<EState>(EState.IDLE));
			m_State.AddState(new TimeState<EState>(EState.ATTACK));
			m_State.AddState(new TimeState<EState>(EState.ATTACK_RANGED));
			m_State.AddState(new TimeState<EState>(EState.HIT));
			m_State.ChangeState(EState.IDLE);
		}

		public new MovingEntity MyController => (MovingEntity)base.MyController;

	    public abstract void Attack(FXArgs p_fxArgs, Action p_fxFinishedCallback);

		public abstract void AttackRanged(FXArgs p_fxArgs, Action p_fxFinishedCallback, EResultType p_Result);

		public abstract void AttackCritical(FXArgs p_fxArgs, Action p_fxFinishedCallback);

		protected override void OnChangeMyController(BaseObject oldController)
		{
			if (MyController != null && MyController == null)
			{
				throw new NotSupportedException("Only MovingEntity objects\n" + MyController.GetType().FullName);
			}
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
				TriggerControllerDone();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			m_Effects = GetComponent<FXDefinitions>();
			if (m_AnimationEventHandler == null)
			{
				m_AnimationEventHandler = this.GetComponentInChildren< BaseEventHandler>(true);
			}
		}

		protected override void OnDestroy()
		{
			ViewManager.DestroyView(MyController);
			base.OnDestroy();
		}

		protected virtual void Update()
		{
			switch (m_State.CurrentState.Id)
			{
			case EState.IDLE:
				if (m_State.IsStateTimeout && m_CommandQueue.Count > 0)
				{
					Action action = m_CommandQueue.Dequeue();
					action();
				}
				goto IL_F6;
			case EState.ATTACK:
			case EState.ATTACK_RANGED:
				if (m_State.IsStateTimeout)
				{
					Debug.LogWarning(String.Concat(new Object[]
					{
						"State machine of CombatViewBase (",
						name,
						") has timeouted in state (",
						m_State.CurrentState.Id,
						")"
					}));
					m_State.ChangeState(EState.IDLE);
					ChangeStateFromAttackToIdle(null);
				}
				goto IL_F6;
			}
			if (m_State.IsStateTimeout)
			{
				m_State.ChangeState(EState.IDLE);
			}
			IL_F6:
			m_State.Update(Time.deltaTime);
		}

		protected virtual void OnMonsterAttacks(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				AttacksEventArgs attacksEventArgs = p_args as AttacksEventArgs;
				if (attacksEventArgs != null && !attacksEventArgs.Counterattack)
				{
					OnMonsterAttacksGeneric(p_sender, attacksEventArgs, false);
				}
			}
		}

		protected virtual void OnMonsterAttacksRanged(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				AttacksEventArgs attacksEventArgs = p_args as AttacksEventArgs;
				if (attacksEventArgs != null && !attacksEventArgs.Counterattack)
				{
					OnMonsterAttacksGeneric(p_sender, attacksEventArgs, true);
				}
			}
		}

		protected virtual void OnMonsterAttacksGeneric(Object p_sender, AttacksEventArgs p_args, Boolean p_isRanged)
		{
			PlayerEntityView playerEntity = FXHelper.GetPlayerEntity();
			Vector3 p_slotOriginPosition;
			Vector3 p_slotForward;
			Vector3 p_slotLeft;
			Vector3 p_slotTargetPosition;
			ViewManager.GetSlotDatas(MyController.Position, LegacyLogic.Instance.WorldManager.Party.Position, out p_slotOriginPosition, out p_slotForward, out p_slotLeft, out p_slotTargetPosition);
			Boolean flag = false;
			GameObject gameObject = null;
			List<Action> callbacks = new List<Action>(p_args.Attacks.Count);
			EResultType eresultType = EResultType.HIT;
			Int32 num = 0;
			List<Character> targets = new List<Character>();
			AttacksEventArgs.AttackedTarget attack;
			foreach (AttacksEventArgs.AttackedTarget attack2 in p_args.Attacks)
			{
				attack = attack2;
				num++;
				if (attack.AttackTarget is Character)
				{
					Character chara = (Character)attack.AttackTarget;
					AttackResult result = attack.AttackResult;
					targets.Add(chara);
					if (playerEntity != null)
					{
						gameObject = playerEntity.GetMemberGameObject(chara.Index);
					}
					if (gameObject == null)
					{
						Debug.LogError("Could not find target character! Char-Index: " + chara.Index);
					}
					else
					{
						flag |= attack.IsCriticalAttack;
						callbacks.Add(delegate
						{
							if (p_sender is Monster && ((Monster)p_sender).AbilityHandler.HasEntriesForPhase(EExecutionPhase.BEFORE_MONSTER_ATTACK))
							{
								((Monster)p_sender).AbilityHandler.FlushActionLog(EExecutionPhase.BEFORE_MONSTER_ATTACK);
								chara.ConditionHandler.FlushActionLog();
							}
							if (((Monster)p_sender).AbilityHandler.HasEntriesForPhase(EExecutionPhase.AFTER_DAMAGE_CALCULATION))
							{
								((Monster)p_sender).AbilityHandler.FlushActionLog(EExecutionPhase.AFTER_DAMAGE_CALCULATION);
							}
							CombatEntryEventArgs p_args2 = new CombatEntryEventArgs(p_sender, chara, result, attack.BloodMagicEventArgs);
							LegacyLogic.Instance.ActionLog.PushEntry(p_args2);
							if (LegacyLogic.Instance.WorldManager.Party.Buffs.HasBuff(EPartyBuffs.SHADOW_CLOAK) && result.Result == EResultType.EVADE)
							{
								LegacyLogic.Instance.WorldManager.Party.Buffs.RemoveBuff(EPartyBuffs.SHADOW_CLOAK);
							}
							if (LegacyLogic.Instance.WorldManager.Party.Buffs.HasBuff(EPartyBuffs.CELESTIAL_ARMOR))
							{
								if (LegacyLogic.Instance.WorldManager.Party.Buffs.GetBuff(EPartyBuffs.CELESTIAL_ARMOR).IsExpired())
								{
									LegacyLogic.Instance.WorldManager.Party.Buffs.RemoveBuff(EPartyBuffs.CELESTIAL_ARMOR);
								}
								LegacyLogic.Instance.WorldManager.Party.Buffs.FlushActionLog();
							}
							chara.FightHandler.FlushCounterAttackActionLog();
							LegacyLogic.Instance.WorldManager.Party.Buffs.FlushActionLog();
							if (p_sender is Monster && ((Monster)p_sender).AbilityHandler.HasEntriesForPhase(EExecutionPhase.AFTER_MONSTER_ATTACK))
							{
								((Monster)p_sender).AbilityHandler.FlushActionLog(EExecutionPhase.AFTER_MONSTER_ATTACK);
								chara.ConditionHandler.FlushActionLog();
							}
						});
						if (attack.AttackResult.Result == EResultType.BLOCK)
						{
							eresultType = EResultType.BLOCK;
						}
						else if (eresultType != EResultType.BLOCK && attack.AttackResult.Result == EResultType.EVADE)
						{
							eresultType = EResultType.EVADE;
						}
					}
				}
			}
			Action action = delegate
			{
				DelayedEventManager.InvokeEvent(EDelayType.ON_FX_HIT, (!p_isRanged) ? EEventType.MONSTER_ATTACKS : EEventType.MONSTER_ATTACKS_RANGED, p_sender, p_args);
				if (!p_isRanged)
				{
					foreach (AttacksEventArgs.AttackedTarget attackedTarget in p_args.Attacks)
					{
						((Monster)MyController).CombatHandler.TriggerCounterAttacks(attackedTarget.AttackTarget, attackedTarget.AttackResult);
					}
				}
				foreach (Action action2 in callbacks)
				{
					action2();
				}
				foreach (Character character in targets)
				{
					character.ConditionHandler.FlushDelayedActionLog();
				}
			};
			if (gameObject == null)
			{
				gameObject = playerEntity.GetMemberGameObject(UnityEngine.Random.Range(0, 4));
				if (gameObject == null)
				{
					Debug.LogError("No target character could be found! Will skip whole FX! Num of Attacks = " + p_args.Attacks.Count);
					action();
					return;
				}
			}
			FXArgs p_fxArgs = new FXArgs(this.gameObject, gameObject, this.gameObject, gameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition);
			if (p_isRanged)
			{
				AttackRanged(p_fxArgs, action, (!flag) ? eresultType : EResultType.CRITICAL_HIT);
			}
			else if (flag)
			{
				AttackCritical(p_fxArgs, action);
			}
			else
			{
				Attack(p_fxArgs, action);
			}
		}

		protected void CheckForReflectedDamage()
		{
			Monster monster = (Monster)MyController;
			if (monster == null)
			{
				return;
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			List<Object> list = new List<Object>();
			if (LegacyLogic.Instance.MapLoader.Grid.GetMonstersOnFirstSlot(party.Position, party.Direction, 1, list) == 0)
			{
				return;
			}
			Boolean flag = false;
			foreach (Object obj in list)
			{
				if ((Monster)obj == monster)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			if (m_reflectedMagicDamage.Count > 0)
			{
				foreach (AttacksEventArgs.AttackedTarget attackedTarget in m_reflectedMagicDamage)
				{
					if (attackedTarget.AttackTarget == monster)
					{
						this.SendEvent("OnReceivedAttacks", new AttacksUnityEventArgs(this, attackedTarget.AttackResult, true));
					}
				}
			}
			foreach (Character character in party.Members)
			{
				character.EnchantmentHandler.FlushActionLog();
			}
			m_reflectedMagicDamage.Clear();
			list.Clear();
		}

		protected virtual void ChangeStateFromAttackToIdle(Action pOnFXFinished)
		{
			if (pOnFXFinished != null)
			{
				pOnFXFinished();
			}
			Boolean flag = m_State.IsState(EState.ATTACK_RANGED);
			if (m_State.IsState(EState.ATTACK) || m_State.IsState(EState.ATTACK_RANGED))
			{
				m_State.ChangeState(EState.IDLE);
			}
			else
			{
				Debug.LogError("Current state is not ATTACK and not ATTACKRANGED!", this);
			}
			if (MyController != null)
			{
				MyController.AttackingDone.Trigger();
				if (MyController is Monster)
				{
					CheckForReflectedDamage();
					m_reflectedMagicDamage.Clear();
					((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.END_OF_MONSTERS_TURN);
					((Monster)MyController).BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_END_TURN);
					DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, (!flag) ? EEventType.MONSTER_ATTACKS : EEventType.MONSTER_ATTACKS_RANGED, (Monster)MyController, EventArgs.Empty);
				}
			}
		}

		public void HandleAnimationOverrideFX(EAnimType pType, FXArgs pArgs)
		{
			if (m_Effects != null && m_Effects.Contains(pType))
			{
				m_Effects.Load(pType).Configurate(m_AnimationEventHandler, pArgs);
			}
		}

		protected void TriggerControllerDone()
		{
			MyController.AttackingDone.Trigger();
		}

		protected void HandleRangedFX(EResultType pResult, FXArgs fxArgs, Action pOnFXFinished)
		{
			String text;
			if (pResult == EResultType.BLOCK)
			{
				text = m_EffectAttackRangedBlockedPath;
			}
			else
			{
				text = m_EffectAttackRangedPath;
			}
			if (!String.IsNullOrEmpty(text))
			{
				FXDescription fxdescription = Helper.ResourcesLoad<FXDescription>(text, false);
				if (fxdescription != null)
				{
					fxdescription = Helper.Instantiate<FXDescription>(fxdescription);
					fxdescription.Finished += delegate(Object sender, EventArgs e)
					{
						ChangeStateFromAttackToIdle(pOnFXFinished);
					};
					fxdescription.Configurate(m_AnimationEventHandler, fxArgs);
					return;
				}
			}
			ChangeStateFromAttackToIdle(pOnFXFinished);
			Debug.LogError("Ranged FX not found!");
		}

		protected void HandleMeleeFX(FXArgs fxArgs)
		{
			String effectMeleeAttackPath = m_EffectMeleeAttackPath;
			if (!String.IsNullOrEmpty(effectMeleeAttackPath))
			{
				FXDescription fxdescription = Helper.ResourcesLoad<FXDescription>(effectMeleeAttackPath, false);
				if (fxdescription != null)
				{
					fxdescription = Helper.Instantiate<FXDescription>(fxdescription);
					fxdescription.Configurate(m_AnimationEventHandler, fxArgs);
				}
				else
				{
					Debug.LogError("Melee FX not found! '" + m_EffectMeleeAttackPath + "'");
				}
			}
		}

		protected void PlayTakeHitSound(Object p_sender, AttackResult p_attackResult, Boolean p_isRanged)
		{
			if (!p_isRanged)
			{
				Boolean flag = false;
				if (p_sender != null && p_sender is Character)
				{
					Character character = (Character)p_sender;
					BaseItem itemAt = character.Equipment.GetItemAt(EEquipSlots.MAIN_HAND);
					flag = (itemAt != null && itemAt is MagicFocus);
				}
				if (p_attackResult.Result == EResultType.BLOCK)
				{
					AudioController.Play("Blocked_Melee", transform);
				}
				else if (p_attackResult.Result == EResultType.EVADE)
				{
					AudioController.Play("Miss_Attack", transform);
				}
				else if (flag)
				{
					AudioController.Play("Magic_Melee", transform);
				}
				else
				{
					AudioController.Play("Hit_Melee", transform);
				}
			}
		}

		protected void FlushMonsterLogEntries()
		{
			Monster monster = (Monster)MyController;
			monster.AbilityHandler.FlushActionLog(EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_BEFORE_DAMAGE_REDUCTION);
			monster.AbilityHandler.FlushActionLog(EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_AFTER_DAMAGE_REDUCTION);
			monster.AbilityHandler.FlushActionLog(EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_COUNTER_ATTACK);
			monster.AbilityHandler.FlushActionLog(EExecutionPhase.ON_APPLY_MONSTER_BUFF);
			monster.BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE);
		}

		public enum EState
		{
			IDLE,
			HIT,
			ATTACK,
			ATTACK_RANGED
		}
	}
}
