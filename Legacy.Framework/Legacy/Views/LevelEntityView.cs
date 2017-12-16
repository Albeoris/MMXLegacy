using System;
using Legacy.Animations;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.Utilities.StateManagement;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class LevelEntityView : BaseView, IMoveable
	{
		private const Single ANIM_PUSH_TIME = 0.25f;

		private EntityPositioning m_CurrentPositioner;

		private TimeStateMachine<EState> m_State;

		private Single m_IdleRotaCoolDown;

		private Single m_Time;

		[SerializeField]
		protected Animator m_animator;

		[SerializeField]
		protected AnimatorControl m_animatorControl;

		[SerializeField]
		private Int32 m_NumberOfIdleAnimations = 2;

		[SerializeField]
		private Int32 m_SpecialAnimationChance = 20;

		[SerializeField]
		private Int32 m_IdleRotationCoolDownMinValue = 10;

		[SerializeField]
		private Int32 m_IdleRotationCoolDownMaxValue = 16;

		[SerializeField]
		private Boolean m_HOMM6_Creature;

		[SerializeField]
		private Single m_spawnAnimationTime;

		private Boolean m_isBeeingPushed;

		private Vector3 m_oldPosition;

		private Vector3 m_pushedTargetPosition;

		private Single m_pushedTime;

		public LevelEntityView()
		{
			m_State = new TimeStateMachine<EState>();
			m_State.AddState(new TimeState<EState>(EState.IDLE));
			m_State.AddState(new TimeState<EState>(EState.MOVEMENT));
			m_State.AddState(new TimeState<EState>(EState.DONE));
			m_State.ChangeState(EState.IDLE);
		}

		void IMoveable.Move(Vector3 targetPosition)
		{
			if (m_animatorControl != null)
			{
				targetPosition = ViewManager.Instance.GridOrigin.TransformPoint(targetPosition);
				m_animatorControl.MoveTo(targetPosition);
			}
		}

		public new MovingEntity MyController => (MovingEntity)base.MyController;

	    public Int32 NumberOfIdleAnimations
		{
			get => m_NumberOfIdleAnimations;
	        set => m_NumberOfIdleAnimations = value;
	    }

		public virtual void PushEntityToPosition()
		{
			MovingEntity myController = MyController;
			transform.localRotation = Helper.GridDirectionToQuaternion(myController.Direction);
			m_oldPosition = transform.localPosition;
			m_isBeeingPushed = true;
			m_pushedTime = 0f;
			m_pushedTargetPosition = GetVectorPosition(myController.Position, myController.Size, myController.Height);
			m_animatorControl.TargetPosition = ViewManager.Instance.GridOrigin.TransformPoint(m_pushedTargetPosition);
		}

		public virtual void SetEntityPosition()
		{
			MovingEntity myController = MyController;
			Vector3 vectorPosition = GetVectorPosition(myController.Position, myController.Size, myController.Height);
			Quaternion quaternion = Helper.GridDirectionToQuaternion(myController.Direction);
			transform.localPosition = vectorPosition;
			transform.localRotation = quaternion;
			m_animatorControl.TargetPosition = ViewManager.Instance.GridOrigin.TransformPoint(vectorPosition);
			m_animatorControl.TargetRotation = quaternion;
		}

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
			m_animator.applyRootMotion = false;
			m_animator.cullingMode = AnimatorCullingMode.BasedOnRenderers;
			m_Time = Time.time;
			m_IdleRotaCoolDown = Random.Range(m_IdleRotationCoolDownMinValue, m_IdleRotationCoolDownMaxValue);
			m_IdleRotaCoolDown += Time.time;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			if (MyController != null && MyController == null)
			{
				throw new NotSupportedException("Only MovingEntity objects\n" + MyController.GetType().FullName);
			}
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.ROTATE_ENTITY, new EventHandler(OnRotateEntity));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnSetEntityPositionEvent));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DESTROY_BASEOBJECT, new EventHandler(OnDestroyEntity));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.ROTATE_ENTITY, new EventHandler(OnRotateEntity));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnSetEntityPositionEvent));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DESTROY_BASEOBJECT, new EventHandler(OnDestroyEntity));
				SetEntityPosition();
				TriggerControllerDone();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (m_CurrentPositioner != null)
			{
				m_CurrentPositioner.RemoveView(this);
				m_CurrentPositioner = null;
			}
		}

		protected virtual void RandomIdle()
		{
			if (m_NumberOfIdleAnimations != 0 && m_IdleRotaCoolDown < Time.time && !m_State.IsState(EState.MOVEMENT))
			{
				Boolean flag = CheckEntityAggro();
				if (!flag || m_HOMM6_Creature)
				{
					Int32 chance = GetChance();
					if (chance > m_SpecialAnimationChance)
					{
						m_animatorControl.IdleSpecial(1);
						Single num = CoolDownTime();
						m_IdleRotaCoolDown = Time.time + num;
					}
					else if (m_NumberOfIdleAnimations > 1 && !flag)
					{
						m_animatorControl.IdleSpecial(2);
						Single num2 = CoolDownTime();
						m_IdleRotaCoolDown = Time.time + num2;
					}
					else
					{
						m_animatorControl.IdleSpecial(1);
						Single num3 = CoolDownTime();
						m_IdleRotaCoolDown = Time.time + num3;
					}
				}
			}
		}

		protected virtual Int32 CoolDownTime()
		{
			return Random.Range(m_IdleRotationCoolDownMinValue, m_IdleRotationCoolDownMaxValue);
		}

		private Int32 GetChance()
		{
			return Random.Range(1, 101);
		}

		private Boolean CheckEntityAggro()
		{
			Monster monster = MyController as Monster;
			return monster != null && monster.IsAggro;
		}

		protected virtual void Update()
		{
			RandomIdle();
			transform.position += m_animatorControl.DeltaPosition;
			transform.rotation *= m_animatorControl.DeltaRotation;
			m_State.Update(Time.deltaTime);
			if (m_isBeeingPushed)
			{
				m_pushedTime += Time.deltaTime;
				Single num = m_pushedTime / 0.25f;
				if (num >= 1f)
				{
					num = 1f;
					m_isBeeingPushed = false;
				}
				Vector3 localPosition;
				Helper.Lerp(ref m_oldPosition, ref m_pushedTargetPosition, num, out localPosition);
				transform.localPosition = localPosition;
			}
			switch (m_State.CurrentStateID)
			{
			case EState.MOVEMENT:
				if (!m_animatorControl.InMovement && m_Time < Time.time)
				{
					m_State.ChangeState(EState.DONE);
				}
				if (m_State.IsStateTimeout && m_Time < Time.time)
				{
					m_State.ChangeState(EState.DONE);
				}
				break;
			case EState.DONE:
				TriggerControllerDone();
				m_State.ChangeState(EState.IDLE);
				break;
			}
			if (MyController is Monster && ((Monster)MyController).State == Monster.EState.SPAWNING && m_spawnAnimationTime >= 0f)
			{
				m_spawnAnimationTime -= Time.deltaTime;
				if (m_spawnAnimationTime <= 0f)
				{
					((Monster)MyController).AppearAnimationDone.Trigger();
				}
			}
		}

		protected void TriggerControllerDone()
		{
			MyController.MovementDone.Trigger();
			MyController.RotationDone.Trigger();
		}

		protected virtual void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = p_args as MoveEntityEventArgs;
			if (moveEntityEventArgs != null && p_sender == MyController)
			{
				MovingEntity myController = MyController;
				Vector3 position = GetVectorPosition(myController.Position, myController.Size, myController.Height);
				Quaternion lookDirection = Helper.GridDirectionToQuaternion(myController.Direction);
				position = ViewManager.Instance.GridOrigin.TransformPoint(position);
				m_animatorControl.MoveTo(position, lookDirection);
				m_Time = Time.time + 0.5f;
				m_State.ChangeState(EState.MOVEMENT, 7f);
			}
		}

		protected virtual void OnRotateEntity(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController)
			{
				MovingEntity myController = MyController;
				Quaternion lookDirection = Helper.GridDirectionToQuaternion(myController.Direction);
				m_animatorControl.RotateTo(lookDirection);
				m_Time = Time.time + 0.5f;
				m_State.ChangeState(EState.MOVEMENT, 7f);
			}
		}

		protected virtual void OnSetEntityPositionEvent(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController)
			{
				if (m_CurrentPositioner != null)
				{
					m_CurrentPositioner.RemoveView(this);
				}
				SetEntityPosition();
			}
		}

		protected virtual void OnDestroyEntity(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController && m_CurrentPositioner != null)
			{
				m_CurrentPositioner.RemoveView(this);
				m_CurrentPositioner = null;
			}
		}

		protected Vector3 GetVectorPosition(Position p_slotPosition, ESize size, Single height)
		{
			Int32 p_comeFrom = -1;
			if (m_CurrentPositioner != null)
			{
				p_comeFrom = m_CurrentPositioner.GetSlotIndex(this);
				m_CurrentPositioner.RemoveView(this);
			}
			EntityPositioning currentPositioner = m_CurrentPositioner;
			m_CurrentPositioner = ViewManager.Instance.GetEntityPositioning(p_slotPosition);
			Vector3 a;
			m_CurrentPositioner.AddView(this, size, out a, p_comeFrom, currentPositioner);
			return a += Helper.SlotLocalPosition(p_slotPosition, height);
		}

		private enum EState
		{
			IDLE,
			MOVEMENT,
			DONE
		}
	}
}
