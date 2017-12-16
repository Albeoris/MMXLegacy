using System;
using Legacy.Core.Configuration;
using UnityEngine;

namespace Legacy.Animations
{
	[RequireComponent(typeof(Animator))]
	public class AnimatorControl : MonoBehaviour
	{
		protected Vector3 m_DeltaPosition;

		protected Quaternion m_DeltaRotation = Quaternion.identity;

		protected Single m_LastDeltaHeight;

		protected Int32 m_RotationDirection;

		protected Vector3 m_TargetPosition;

		protected Vector3 m_StartPosition;

		protected Single m_TargetDistance;

		protected Quaternion m_TargetRotation = Quaternion.identity;

		protected Vector3 m_tmpLookDirection = Vector3.forward;

		protected Animator m_Animator;

		protected AnimatorEx m_AnimatorEx;

		protected Boolean m_IsDead;

		protected Boolean m_InAttack;

		protected Boolean m_InBlock;

		protected Boolean m_InEvade;

		protected Boolean m_InDie;

		protected static Boolean m_CreateHash;

		protected static Int32 m_DieHash;

		protected static Int32 m_EvadeHash;

		protected static Int32 m_BlockHash;

		protected static Int32 m_AttackHash;

		protected static Int32 m_IdleHash;

		protected static Int32 m_IdleSpecialHash;

		protected static Int32 m_IdleCombatHash;

		[SerializeField]
		protected AnimatorEx.FloatProperty m_MoveSpeed = new AnimatorEx.FloatProperty();

		[SerializeField]
		protected AnimatorEx.FloatProperty m_MoveHorizontal = new AnimatorEx.FloatProperty();

		[SerializeField]
		protected AnimatorEx.FloatProperty m_MoveVertical = new AnimatorEx.FloatProperty();

		[SerializeField]
		protected AnimatorEx.FloatProperty m_MoveDirection = new AnimatorEx.FloatProperty();

		[SerializeField]
		protected AnimatorEx.BoolProperty m_InCombat = new AnimatorEx.BoolProperty();

		[SerializeField]
		protected IntTriggerProperty m_AttackTrigger = new IntTriggerProperty();

		[SerializeField]
		protected IntTriggerProperty m_AttackCriticalTrigger = new IntTriggerProperty();

		[SerializeField]
		protected IntTriggerProperty m_AttackRangeTrigger = new IntTriggerProperty();

		[SerializeField]
		protected IntTriggerProperty m_AttackMagicTrigger = new IntTriggerProperty();

		[SerializeField]
		protected IntTriggerProperty m_DieTrigger = new IntTriggerProperty();

		[SerializeField]
		protected IntTriggerProperty m_IdleTrigger = new IntTriggerProperty();

		[SerializeField]
		protected IntTriggerProperty m_EvadeTrigger = new IntTriggerProperty();

		[SerializeField]
		protected IntTriggerProperty m_HitTrigger = new IntTriggerProperty();

		[SerializeField]
		protected IntTriggerProperty m_BlockTrigger = new IntTriggerProperty();

		[SerializeField]
		protected DieProperty m_DieState = new DieProperty();

		[SerializeField]
		protected AnimatorEx.BoolProperty m_IsRotating = new AnimatorEx.BoolProperty();

		[SerializeField]
		protected AnimatorEx.BoolProperty m_IsMoving = new AnimatorEx.BoolProperty();

		[SerializeField]
		protected IntTriggerProperty m_EventTrigger = new IntTriggerProperty();

		[SerializeField]
		protected Single m_MinMoveRadius = 1f;

		[SerializeField]
		protected Single m_MinTurnRadius = 10f;

		[SerializeField]
		protected Single m_MinAngle = 5f;

		[SerializeField]
		protected Single m_MinWalkDistance = 10f;

		[SerializeField]
		protected Single m_Speedfactor = 1f;

		protected Boolean m_ResetSpeed;

		protected Single m_AnimSpeedRef;

		protected Single m_VariableSpeedFactor;

		protected Int32 m_RotationTry;

		private Transform m_CachedTransform;

		private GameObject m_CachedGameObject;

		public new Transform transform
		{
			get
			{
				if (m_CachedTransform == null)
				{
					m_CachedTransform = base.transform;
				}
				return m_CachedTransform;
			}
			private set => m_CachedTransform = value;
		}

		public new GameObject gameObject
		{
			get
			{
				if (m_CachedGameObject == null)
				{
					m_CachedGameObject = base.gameObject;
				}
				return m_CachedGameObject;
			}
			private set => m_CachedGameObject = value;
		}

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

		public Vector3 DeltaPosition => m_DeltaPosition;

	    public Quaternion DeltaRotation => m_DeltaRotation;

	    public Int32 AttackMagicMaxValue => m_AttackMagicTrigger.RandomMax;

	    public Int32 AttackMeleeMaxValue => m_AttackTrigger.RandomMax;

	    public Int32 AttackCriticalMeleeMaxValue => m_AttackCriticalTrigger.RandomMax;

	    public Int32 AttackRangedMaxValue => m_AttackRangeTrigger.RandomMax;

	    public Int32 HitMaxValue => m_HitTrigger.RandomMax;

	    public Int32 DieMaxValue => m_DieTrigger.RandomMax;

	    public Int32 EvadeMaxValue => m_EvadeTrigger.RandomMax;

	    public Int32 BlockMaxValue => m_BlockTrigger.RandomMax;

	    public Int32 IdleMaxValue => m_IdleTrigger.RandomMax;

	    public Int32 EventMaxValue => m_EventTrigger.RandomMax;

	    public Int32 DieState => m_DieState.Value;

	    public Boolean InCombat
		{
			get => m_InCombat.Value;
	        set => m_InCombat.Value = value;
	    }

		public Boolean IsDead => m_IsDead;

	    public Boolean InMovement => IsRotating || IsMoving;

	    public Boolean IsRotating => m_IsRotating.RawValue;

	    public Boolean IsMoving => m_IsMoving.RawValue;

	    public Boolean IsIdle
		{
			get
			{
				if (this == null)
				{
					return false;
				}
				AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
				return currentAnimatorStateInfo.tagHash == m_IdleCombatHash || currentAnimatorStateInfo.tagHash == m_IdleSpecialHash || currentAnimatorStateInfo.tagHash == m_IdleHash;
			}
		}

		public Single Direction => m_MoveDirection.Value;

	    public Single MoveVertical => m_MoveVertical.Value;

	    public Single MoveHorizontal => m_MoveHorizontal.Value;

	    public Single MoveSpeed => m_MoveSpeed.Value;

	    public Single MinTurnAngle => m_MinTurnRadius;

	    public void MoveTo(Vector3 position)
		{
			MoveTo(position, m_TargetRotation);
		}

		public virtual void MoveTo(Vector3 position, Quaternion lookDirection)
		{
			m_TargetPosition = position;
			m_TargetRotation = lookDirection;
			m_StartPosition = transform.position;
			m_LastDeltaHeight = m_StartPosition.y;
		}

		public virtual void RotateTo(Quaternion lookDirection)
		{
			m_TargetRotation = lookDirection;
		}

		public virtual void Attack()
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			ResetValues();
			m_AttackTrigger.Trigger(0);
		}

		public virtual void Attack(Int32 animationID)
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			ResetValues();
			m_AttackTrigger.Trigger(0, animationID, 0);
		}

		public virtual void AttackCritical()
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			ResetValues();
			m_AttackCriticalTrigger.Trigger(0);
		}

		public virtual void AttackCritical(Int32 animationID)
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			ResetValues();
			m_AttackCriticalTrigger.Trigger(0, animationID, 0);
		}

		public virtual void AttackRange()
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			ResetValues();
			m_AttackRangeTrigger.Trigger(0);
		}

		public virtual void AttackRange(Int32 animationID)
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			ResetValues();
			m_AttackRangeTrigger.Trigger(0, animationID, 0);
		}

		public virtual void AttackMagic()
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			ResetValues();
			m_AttackMagicTrigger.Trigger(0);
		}

		public virtual void AttackMagic(Int32 animationID)
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			ResetValues();
			m_AttackMagicTrigger.Trigger(0, animationID, 0);
		}

		public virtual void Die()
		{
			if (this == null)
			{
				return;
			}
			m_IsDead = !m_IsDead;
			if (m_IsDead)
			{
				ResetValues();
				m_DieTrigger.Trigger(0);
				m_DieState.Value = 1;
				m_DieState.TriggerDead(0, 2);
			}
			else
			{
				m_DieState.Value = 3;
				m_DieState.TriggerAlive(0, 0);
			}
		}

		public virtual void Die(Int32 animationID)
		{
			if (this == null)
			{
				return;
			}
			m_IsDead = !m_IsDead;
			if (m_IsDead)
			{
				ResetValues();
				m_DieTrigger.Trigger(0, animationID, 0);
				m_DieState.Value = 1;
				m_DieState.TriggerDead(0, 2);
			}
			else
			{
				m_DieState.Value = 3;
				m_DieState.TriggerAlive(0, 0);
			}
		}

		public virtual void Idle()
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			m_IdleTrigger.Trigger(0);
		}

		public virtual void Evade()
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie || m_InAttack)
			{
				return;
			}
			m_EvadeTrigger.Trigger(0);
		}

		public virtual void Evade(Int32 animationID)
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie || m_InAttack)
			{
				return;
			}
			m_EvadeTrigger.Trigger(0, animationID, 0);
		}

		public virtual void NPCEvent()
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie || m_InAttack)
			{
				return;
			}
			m_EventTrigger.Trigger(0);
		}

		public virtual void Hit()
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie || m_InAttack || m_InBlock || m_InEvade)
			{
				return;
			}
			m_HitTrigger.Trigger(0);
		}

		public virtual void Hit(Int32 animationID)
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie || m_InAttack || m_InBlock || m_InEvade)
			{
				return;
			}
			m_HitTrigger.Trigger(0, animationID, 0);
		}

		public virtual void Block()
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie || m_InAttack)
			{
				return;
			}
			m_BlockTrigger.Trigger(0);
		}

		public virtual void Block(Int32 animationID)
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie || m_InAttack)
			{
				return;
			}
			m_BlockTrigger.Trigger(0, animationID, 0);
		}

		public virtual void EventSummon(Int32 animationID)
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie)
			{
				return;
			}
			m_EventTrigger.Trigger(0, animationID, 0);
		}

		public virtual void IdleSpecial(Int32 animationID)
		{
			if (this == null || m_IsDead || m_DieState.RawValue != 0)
			{
				return;
			}
			StateCheck();
			if (m_InDie || m_InAttack)
			{
				return;
			}
			m_IdleTrigger.Trigger(0, animationID, 0);
		}

		protected virtual void Awake()
		{
			if (!m_CreateHash)
			{
				m_CreateHash = true;
				m_DieHash = Animator.StringToHash("DIE");
				m_AttackHash = Animator.StringToHash("ATTACK");
				m_EvadeHash = Animator.StringToHash("EVADE");
				m_BlockHash = Animator.StringToHash("BLOCK");
				m_IdleCombatHash = Animator.StringToHash("IDLE_COMBAT");
				m_IdleSpecialHash = Animator.StringToHash("IDLE_SPECIAL");
				m_IdleHash = Animator.StringToHash("IDLE");
			}
			transform = base.transform;
			gameObject = base.gameObject;
			m_TargetPosition = transform.position;
			m_TargetRotation = transform.rotation;
			m_StartPosition = m_TargetPosition;
			m_Animator = this.GetComponent<Animator>(true);
			m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			m_Animator.logWarnings = false;
			m_AnimatorEx = new AnimatorEx(m_Animator);
			m_MoveSpeed.Init(m_AnimatorEx, m_Animator);
			m_MoveHorizontal.Init(m_AnimatorEx, m_Animator);
			m_MoveVertical.Init(m_AnimatorEx, m_Animator);
			m_MoveDirection.Init(m_AnimatorEx, m_Animator);
			m_InCombat.Init(m_AnimatorEx, m_Animator);
			m_AttackTrigger.Init(m_AnimatorEx, m_Animator);
			m_AttackCriticalTrigger.Init(m_AnimatorEx, m_Animator);
			m_AttackRangeTrigger.Init(m_AnimatorEx, m_Animator);
			m_AttackMagicTrigger.Init(m_AnimatorEx, m_Animator);
			m_DieTrigger.Init(m_AnimatorEx, m_Animator);
			m_IdleTrigger.Init(m_AnimatorEx, m_Animator);
			m_EvadeTrigger.Init(m_AnimatorEx, m_Animator);
			m_HitTrigger.Init(m_AnimatorEx, m_Animator);
			m_BlockTrigger.Init(m_AnimatorEx, m_Animator);
			m_DieState.Init(m_AnimatorEx, m_Animator);
			m_IsMoving.Init(m_AnimatorEx, m_Animator);
			m_IsRotating.Init(m_AnimatorEx, m_Animator);
			m_EventTrigger.Init(m_AnimatorEx, m_Animator);
		}

		protected virtual void Update()
		{
			Single monsterMovementSpeed = ConfigManager.Instance.Options.MonsterMovementSpeed;
			Single num = 0f;
			StateCheck();
			if (m_LastDeltaHeight == 0f)
			{
				m_LastDeltaHeight = transform.position.y;
			}
			if (m_StartPosition == Vector3.zero)
			{
				m_LastDeltaHeight = transform.position.y;
			}
			Vector3 vector = m_TargetPosition - transform.position;
			Single target = 0f;
			Single target2 = 0f;
			Vector3 direction = default(Vector3);
			if (m_IsMoving.RawValue)
			{
				target2 = vector.magnitude / m_MinWalkDistance;
				direction = vector;
				direction.y = 0f;
				direction.Normalize();
				direction = transform.InverseTransformDirection(direction);
				if (!m_ResetSpeed)
				{
					m_AnimSpeedRef = m_Animator.speed;
				}
				m_Animator.speed = monsterMovementSpeed;
				m_ResetSpeed = true;
				if (Math.Abs(vector.x) <= m_MinMoveRadius && Math.Abs(vector.z) <= m_MinMoveRadius)
				{
					target2 = 0f;
					m_IsMoving.Value = false;
				}
			}
			if (m_IsRotating.RawValue)
			{
				target = m_RotationDirection;
				if (!m_ResetSpeed)
				{
					m_AnimSpeedRef = m_Animator.speed;
				}
				m_Animator.speed = monsterMovementSpeed;
				m_ResetSpeed = true;
				num = Vector3.Angle(transform.forward, m_tmpLookDirection);
				if (num < m_MinAngle * 5f)
				{
					if (m_RotationDirection > 0)
					{
						target = 0.75f / monsterMovementSpeed;
					}
					else
					{
						target = -0.75f / monsterMovementSpeed;
					}
				}
				if (num < m_MinAngle)
				{
					m_IsRotating.Value = false;
				}
			}
			if ((!InMovement && m_ResetSpeed) || (m_InAttack && m_ResetSpeed) || (m_InBlock && m_ResetSpeed) || (m_InDie && m_ResetSpeed) || (m_InEvade && m_ResetSpeed))
			{
				m_Animator.speed = m_AnimSpeedRef;
				m_ResetSpeed = false;
			}
			Single maxDelta = Time.deltaTime * 3f;
			m_MoveSpeed.Value = Mathf.MoveTowards(m_MoveSpeed.Value, target2, maxDelta);
			m_MoveDirection.Value = Mathf.MoveTowards(m_MoveDirection.Value, target, maxDelta);
			m_MoveVertical.Value = Mathf.MoveTowards(m_MoveVertical.Value, direction.z, maxDelta);
			m_MoveHorizontal.Value = Mathf.MoveTowards(m_MoveHorizontal.Value, direction.x, maxDelta);
			m_DeltaPosition = m_Animator.deltaPosition;
			if (num > m_MinAngle)
			{
				m_DeltaRotation = m_Animator.deltaRotation;
			}
			else
			{
				m_DeltaRotation.y = 0f;
			}
			if (m_IsMoving.RawValue)
			{
				Single num2 = Vector3.Distance(m_StartPosition, transform.position);
				Single t = num2 / m_TargetDistance;
				Single num3 = Mathf.Lerp(m_StartPosition.y, m_TargetPosition.y, t);
				Single y = num3 - m_LastDeltaHeight;
				m_LastDeltaHeight = num3;
				m_DeltaPosition *= m_Speedfactor;
				m_DeltaPosition.y = y;
			}
			if (!m_IsMoving.RawValue && !m_IsRotating.RawValue)
			{
				m_tmpLookDirection = m_TargetRotation * Vector3.forward;
				m_tmpLookDirection.y = 0f;
				m_tmpLookDirection.Normalize();
				num = Vector3.Angle(transform.forward, m_tmpLookDirection);
				if (Math.Abs(vector.x) > m_MinMoveRadius || Math.Abs(vector.z) > m_MinMoveRadius || num > m_MinAngle)
				{
					if (num > m_MinAngle)
					{
						m_IsMoving.Value = false;
						m_IsRotating.Value = true;
						m_RotationDirection = Helper.AngleDir(transform.forward, m_tmpLookDirection, Vector3.up);
						if (m_RotationDirection == 0)
						{
							m_RotationDirection = ((Random.Value >= 0.5f) ? -1 : 1);
						}
					}
					else if (num <= m_MinAngle)
					{
						m_IsMoving.Value = true;
						m_IsRotating.Value = false;
						m_StartPosition = transform.position;
						m_TargetDistance = Vector3.Distance(m_StartPosition, m_TargetPosition);
					}
				}
			}
		}

		protected virtual void StateCheck()
		{
			if (m_Animator == null)
			{
				Debug.LogError("Animator is null!");
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
			AnimatorStateInfo nextAnimatorStateInfo = m_Animator.GetNextAnimatorStateInfo(0);
			Int32 tagHash = currentAnimatorStateInfo.tagHash;
			Int32 tagHash2 = nextAnimatorStateInfo.tagHash;
			if (tagHash == m_AttackHash || tagHash2 == m_AttackHash)
			{
				m_InAttack = true;
			}
			else
			{
				m_InAttack = false;
			}
			if (tagHash == m_BlockHash || tagHash2 == m_BlockHash)
			{
				m_InBlock = true;
			}
			else
			{
				m_InBlock = false;
			}
			if (tagHash == m_EvadeHash || tagHash2 == m_EvadeHash)
			{
				m_InEvade = true;
			}
			else
			{
				m_InEvade = false;
			}
			if (tagHash == m_DieHash || tagHash2 == m_DieHash)
			{
				m_InDie = true;
			}
			else
			{
				m_InDie = false;
			}
		}

		protected void ResetValues()
		{
			m_AttackTrigger.Value = 0;
			m_AttackRangeTrigger.Value = 0;
			m_AttackMagicTrigger.Value = 0;
			m_EvadeTrigger.Value = 0;
			m_BlockTrigger.Value = 0;
			m_HitTrigger.Value = 0;
			m_IdleTrigger.Value = 0;
			m_EventTrigger.Value = 0;
		}

		protected virtual void LateUpdate()
		{
			m_AnimatorEx.Update();
		}

		[Serializable]
		public class IntProperty : AnimatorEx.IntProperty
		{
			public String ResetTargetTag;

			public void Trigger(Int32 layer, Int32 value, Int32 resetValue)
			{
				Value = value;
				base.Trigger(layer, ResetTargetTag, ref resetValue);
			}
		}

		[Serializable]
		public class DieProperty : AnimatorEx.IntProperty
		{
			public String ResetDeadTag;

			public String ResetAliveTag;

			public void TriggerDead(Int32 layer, Int32 value)
			{
				Trigger(layer, ResetDeadTag, ref value);
			}

			public void TriggerAlive(Int32 layer, Int32 value)
			{
				Trigger(layer, ResetAliveTag, ref value);
			}
		}

		[Serializable]
		public class IntTriggerProperty : IntProperty
		{
			public Int32 RandomMin;

			public Int32 RandomMax;

			public Int32 RestValue;

			public void Trigger(Int32 layer)
			{
				Value = GetRandomValue();
				base.Trigger(layer, ResetTargetTag, ref RestValue);
			}

			public Int32 GetRandomValue()
			{
				return Random.Range(RandomMin, RandomMax + 1);
			}
		}

		protected enum EDieState
		{
			Alive,
			Dying,
			Dead,
			Revive
		}
	}
}
