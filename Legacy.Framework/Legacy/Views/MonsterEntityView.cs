using System;
using System.Collections.Generic;
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
	[AddComponentMenu("MM Legacy/Views/Monster Entity View")]
	public class MonsterEntityView : BaseView, IMoveable
	{
		private const Single ANIMATION_DELAY_MAX = 0.25f;

		private const Single BASE_TURN_TIME = 1f;

		private const Single ANIM_MOVEMENT_SPEED_BALANCING = 1f;

		private const Single ANIM_PUSH_TIME = 0.25f;

		[SerializeField]
		private AnimHandler m_AnimationHandler;

		private TimeStateMachine<EState> m_State;

		private Vector3 m_OldPosition;

		private Vector3 m_TargetPosition;

		private Quaternion m_OldRotation = Quaternion.identity;

		private Quaternion m_TargetRotation = Quaternion.identity;

		private EntityPositioning m_CurrentPositioner;

		private EDirection m_CurrentLookDirection;

		private Queue<Action> m_CommandQueue = new Queue<Action>();

		private Boolean m_isBeeingPushed;

		private Vector3 m_pushedTargetPosition;

		private Single m_pushedTime;

		public MonsterEntityView()
		{
			m_State = new TimeStateMachine<EState>();
			m_State.AddState(new TimeState<EState>(EState.IDLE));
			m_State.AddState(new TimeState<EState>(EState.MOVING));
			m_State.AddState(new TimeState<EState>(EState.ROTATING));
			m_State.ChangeState(EState.IDLE);
		}

		void IMoveable.Move(Vector3 targetPosition)
		{
			if (this != null)
			{
				FreeMove(targetPosition, m_CurrentLookDirection, true);
			}
		}

		public Boolean IsMoving => !m_State.IsState(EState.IDLE);

	    public Single MovingTime => m_State.IsState(EState.IDLE) ? 0f : m_State.CurrentStateTime;

	    public Single MovingDuration => m_State.IsState(EState.IDLE) ? 0f : m_State.CurrentStateDuration;

	    public Vector3 TargetPosition
		{
			get => m_TargetPosition;
	        set => m_TargetPosition = value;
	    }

		public Quaternion TargetRotation
		{
			get => m_TargetRotation;
		    set => m_TargetRotation = value;
		}

		public void PushEntityToPosition()
		{
			MovingEntity movingEntity = (MovingEntity)MyController;
			m_CurrentLookDirection = movingEntity.Direction;
			transform.localRotation = Helper.GridDirectionToQuaternion(movingEntity.Direction);
			m_OldPosition = transform.localPosition;
			m_isBeeingPushed = true;
			m_pushedTime = 0f;
			m_pushedTargetPosition = GetVectorPosition(movingEntity.Position, movingEntity.Size, movingEntity.Height);
		}

		public void SetEntityPosition()
		{
			MovingEntity movingEntity = (MovingEntity)MyController;
			m_CurrentLookDirection = movingEntity.Direction;
			transform.localPosition = GetVectorPosition(movingEntity.Position, movingEntity.Size, movingEntity.Height);
			Debug.Log(String.Concat(new Object[]
			{
				"LocalPos: ",
				transform.localPosition.ToString(),
				" - ID: ",
				movingEntity.SpawnerID,
				" - Height: ",
				movingEntity.Height
			}), this);
			transform.localRotation = Helper.GridDirectionToQuaternion(movingEntity.Direction);
			m_TargetRotation = transform.localRotation;
			m_TargetPosition = transform.localPosition;
		}

		public void MoveTo(Vector3 localPosition)
		{
			MoveTo(localPosition, true, m_CurrentLookDirection);
		}

		public virtual void MoveTo(Vector3 localPosition, Boolean delayed, EDirection direction)
		{
			if (localPosition == transform.localPosition)
			{
				return;
			}
			if (delayed)
			{
				m_CommandQueue.Enqueue(delegate
				{
					m_State.ChangeState(EState.IDLE, UnityEngine.Random.Range(0f, 0.25f));
				});
			}
			m_CommandQueue.Enqueue(delegate
			{
				m_OldPosition = transform.localPosition;
				m_OldRotation = transform.localRotation;
				m_TargetPosition = localPosition;
				m_CurrentLookDirection = direction;
				m_TargetRotation = Helper.GridDirectionToQuaternion(m_CurrentLookDirection);
				Single num = Vector3.Distance(m_OldPosition, m_TargetPosition);
				Single speed = num / m_AnimationHandler.Config.MoveSpeed / 1f * 1f;
				PlayAnimation(EAnimType.MOVE, EState.MOVING, 1f, speed);
			});
		}

		public void RotateTo(EDirection direction)
		{
			RotateTo(direction, true);
		}

		public virtual void RotateTo(EDirection direction, Boolean delayed)
		{
			Int32 num = EDirectionFunctions.RotationCount(m_CurrentLookDirection, direction);
			if (num == 0)
			{
				return;
			}
			if (delayed)
			{
				m_CommandQueue.Enqueue(delegate
				{
					m_State.ChangeState(EState.IDLE, UnityEngine.Random.Range(0f, 0.25f));
				});
			}
			if (num < 0)
			{
				m_CommandQueue.Enqueue(delegate
				{
					m_CurrentLookDirection = direction;
					m_TargetRotation = Helper.GridDirectionToQuaternion(m_CurrentLookDirection);
					PlayAnimation(EAnimType.TURN_LEFT, EState.ROTATING, -1f, 2f);
				});
			}
			else
			{
				m_CommandQueue.Enqueue(delegate
				{
					m_CurrentLookDirection = direction;
					m_TargetRotation = Helper.GridDirectionToQuaternion(m_CurrentLookDirection);
					PlayAnimation(EAnimType.TURN_RIGHT, EState.ROTATING, -1f, 2f);
				});
			}
		}

		public void FreeMove(Vector3 targetLocalPosition, EDirection direction, Boolean delayed)
		{
			if (targetLocalPosition == transform.localPosition)
			{
				return;
			}
			Vector3 targetDirection = targetLocalPosition - transform.localPosition;
			targetDirection.y = 0f;
			targetDirection.Normalize();
			m_CommandQueue.Clear();
			if (delayed)
			{
				m_CommandQueue.Enqueue(delegate
				{
					m_State.ChangeState(EState.IDLE, UnityEngine.Random.Range(0f, 0.25f));
				});
			}
			Single num = Vector3.Angle(transform.forward, targetDirection);
			if (num > 50f)
			{
				Int32 leftRight = Helper.AngleDir(transform.forward, targetDirection, Vector3.up);
				m_CommandQueue.Enqueue(delegate
				{
					m_TargetRotation = Quaternion.LookRotation(targetDirection);
					PlayAnimation((leftRight >= 0) ? EAnimType.TURN_RIGHT : EAnimType.TURN_LEFT, EState.ROTATING, -1f, 2f);
				});
			}
			m_CommandQueue.Enqueue(delegate
			{
				m_OldPosition = transform.localPosition;
				m_OldRotation = transform.localRotation;
				m_TargetPosition = targetLocalPosition;
				m_CurrentLookDirection = direction;
				m_TargetRotation = Helper.GridDirectionToQuaternion(m_CurrentLookDirection);
				Single num2 = Vector3.Distance(m_OldPosition, m_TargetPosition);
				Single num3 = num2 / m_AnimationHandler.Config.MoveSpeed;
				PlayAnimation(EAnimType.MOVE, EState.MOVING, num3 / 2f, 2f);
			});
			TriggerLogicFinish();
		}

		public void TriggerLogicFinish()
		{
			m_CommandQueue.Enqueue(delegate
			{
				if (MyController != null)
				{
					((MovingEntity)MyController).MovementDone.Trigger();
					((MovingEntity)MyController).RotationDone.Trigger();
				}
			});
		}

		private void PlayAnimation(EAnimType type, EState changedState, Single duration, Single speed)
		{
			AnimationState state = m_AnimationHandler.GetState(type);
			if (state != null)
			{
				if (duration == -1f)
				{
					duration = state.length / speed;
				}
				m_AnimationHandler.Play(type, duration, speed);
				m_State.ChangeState(changedState, duration);
			}
			else
			{
				m_State.ChangeState(changedState, 0f);
				Debug.LogError("Missing '" + type + "' animation!", this);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (m_AnimationHandler == null)
			{
				m_AnimationHandler = this.GetComponent<AnimHandler>(true);
			}
			m_TargetPosition = transform.localPosition;
			m_TargetRotation = transform.localRotation;
		}

		private void Start()
		{
			m_AnimationHandler.Play(EAnimType.IDLE);
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

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.ROTATE_ENTITY, new EventHandler(OnRotateEntity));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.ROTATE_ENTITY, new EventHandler(OnRotateEntity));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
				SetEntityPosition();
			}
		}

		private void OnMonsterDied(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController && m_CurrentPositioner != null)
			{
				m_CurrentPositioner.RemoveView(this);
				m_CurrentPositioner = null;
			}
		}

		private void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
			if (moveEntityEventArgs != null && p_sender == MyController)
			{
				MovingEntity movingEntity = (MovingEntity)p_sender;
				Vector3 vectorPosition = GetVectorPosition(movingEntity.Position, movingEntity.Size, movingEntity.Height);
				FreeMove(vectorPosition, movingEntity.Direction, true);
			}
		}

		private void OnRotateEntity(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController)
			{
				MovingEntity movingEntity = (MovingEntity)baseObjectEventArgs.Object;
				RotateTo(movingEntity.Direction);
				TriggerLogicFinish();
			}
		}

		private Vector3 GetVectorPosition(Position p_slotPosition, ESize size, Single height)
		{
			ViewManager instance = ViewManager.Instance;
			Int32 p_comeFrom = -1;
			if (m_CurrentPositioner != null)
			{
				p_comeFrom = m_CurrentPositioner.GetSlotIndex(this);
				m_CurrentPositioner.RemoveView(this);
			}
			EntityPositioning currentPositioner = m_CurrentPositioner;
			m_CurrentPositioner = instance.GetEntityPositioning(p_slotPosition);
			Vector3 a;
			m_CurrentPositioner.AddView(this, size, out a, p_comeFrom, currentPositioner);
			return a += Helper.SlotLocalPosition(p_slotPosition, height);
		}

		private void Update()
		{
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
				Helper.Lerp(ref m_OldPosition, ref m_pushedTargetPosition, num, out localPosition);
				transform.localPosition = localPosition;
			}
			else if (m_State.CurrentState != null)
			{
				switch (m_State.CurrentState.Id)
				{
				case EState.IDLE:
					if (m_State.IsStateTimeout && m_CommandQueue.Count > 0)
					{
						Action action = m_CommandQueue.Dequeue();
						action();
					}
					else if (transform.localRotation != m_TargetRotation)
					{
						transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TargetRotation, Time.deltaTime * 4f);
					}
					break;
				case EState.MOVING:
				{
					Single d = Vector3.Distance(m_OldPosition, m_TargetPosition);
					Vector3 vector = m_OldRotation * Vector3.forward * d;
					Vector3 vector2 = m_TargetRotation * Vector3.forward * d;
					Single num2 = m_State.CurrentStateTimePer;
					num2 = num2 * num2 * (3f - 2f * num2);
					Vector3 vector3;
					Helper.Hermite(ref m_OldPosition, ref vector, ref m_TargetPosition, ref vector2, num2, out vector3);
					Vector3 localPosition2 = transform.localPosition;
					Quaternion to = m_TargetRotation;
					if (vector3 != localPosition2)
					{
						to = Quaternion.LookRotation((vector3 - localPosition2).normalized);
					}
					else
					{
						to = transform.rotation;
					}
					transform.localRotation = Quaternion.Slerp(transform.localRotation, to, Time.deltaTime * 8f);
					transform.localPosition = vector3;
					if (m_State.IsStateTimeout)
					{
						m_State.ChangeState(EState.IDLE);
					}
					break;
				}
				case EState.ROTATING:
					if (m_State.IsStateTimeout)
					{
						m_State.ChangeState(EState.IDLE);
					}
					break;
				}
			}
			m_State.Update(Time.deltaTime);
		}

		private enum EState
		{
			IDLE,
			MOVING,
			ROTATING
		}
	}
}
