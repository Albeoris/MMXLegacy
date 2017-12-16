using System;
using Legacy.Core.Configuration;
using Legacy.Core.Map;
using UnityEngine;

namespace Legacy.Animations
{
	public class AnimatorControlHOMMC : AnimatorControl
	{
		[SerializeField]
		public Single m_SpeedModifierRef = 0.05f;

		[SerializeField]
		private Boolean m_Movement_Function;

		private Single m_SpeedModifier = 1f;

		private Boolean m_Move;

		private EDirection m_Direction = EDirection.CENTER;

		private Single m_DirectionAngle;

		private Boolean m_OptimalAngle;

		private Int32 m_MoveHash;

		protected override void Awake()
		{
			base.Awake();
			m_MoveHash = Animator.StringToHash("MOVE");
		}

		private void CurrentDirection()
		{
			m_DirectionAngle = transform.eulerAngles.y;
			EDirection direction = EDirection.CENTER;
			if (m_DirectionAngle < 30f || m_DirectionAngle > 330f)
			{
				direction = EDirection.NORTH;
			}
			if (m_DirectionAngle < 300f && m_DirectionAngle > 240f)
			{
				direction = EDirection.WEST;
			}
			if (m_DirectionAngle < 120f && m_DirectionAngle > 60f)
			{
				direction = EDirection.EAST;
			}
			if (m_DirectionAngle < 210f && m_DirectionAngle >= 150f)
			{
				direction = EDirection.SOUTH;
			}
			m_Direction = direction;
		}

		private void GotOptimalAngle()
		{
			switch (m_Direction)
			{
			case EDirection.NORTH:
				if (m_DirectionAngle < 1f || m_DirectionAngle > 359f)
				{
					m_OptimalAngle = true;
				}
				else
				{
					m_OptimalAngle = false;
				}
				break;
			case EDirection.EAST:
				if (m_DirectionAngle < 271f || m_DirectionAngle > 269f)
				{
					m_OptimalAngle = true;
				}
				else
				{
					m_OptimalAngle = false;
				}
				break;
			case EDirection.SOUTH:
				if (m_DirectionAngle < 181f || m_DirectionAngle > 179f)
				{
					m_OptimalAngle = true;
				}
				else
				{
					m_OptimalAngle = false;
				}
				break;
			case EDirection.WEST:
				if (m_DirectionAngle < 91f || m_DirectionAngle > 89f)
				{
					m_OptimalAngle = true;
				}
				else
				{
					m_OptimalAngle = false;
				}
				break;
			}
		}

		private void GetCurrentHashName()
		{
			Int32 tagHash = m_Animator.GetCurrentAnimatorStateInfo(0).tagHash;
			if (tagHash == m_MoveHash)
			{
				m_Move = true;
				AdjustSpeed();
			}
			else
			{
				GetNextHashName();
			}
		}

		private void GetNextHashName()
		{
			Int32 tagHash = m_Animator.GetNextAnimatorStateInfo(0).tagHash;
			if (tagHash == m_MoveHash)
			{
				m_Move = true;
				AdjustSpeed();
			}
			else
			{
				m_Move = false;
				AdjustSpeed();
			}
		}

		private void AdjustSpeed()
		{
			Int32 tagHash = m_Animator.GetNextAnimatorStateInfo(0).tagHash;
			Single normalizedTime = m_Animator.GetAnimatorTransitionInfo(0).normalizedTime;
			if (tagHash == m_MoveHash)
			{
				m_SpeedModifier = m_SpeedModifierRef * normalizedTime;
			}
			else
			{
				m_SpeedModifier = (1f - normalizedTime) * m_SpeedModifierRef;
			}
		}

		private void MotionAndRotationHelper()
		{
			GetCurrentHashName();
			if (m_Movement_Function && IsMoving && m_Move)
			{
				if (MoveSpeed > 0.1f)
				{
					Single maxDistanceDelta = Time.deltaTime * (m_SpeedModifier * ConfigManager.Instance.Options.MonsterMovementSpeed);
					Vector3 a = Vector3.MoveTowards(transform.position, TargetPosition, maxDistanceDelta);
					m_DeltaPosition += a - transform.position;
				}
				else
				{
					Single maxDistanceDelta2 = Time.deltaTime * (m_SpeedModifier * ConfigManager.Instance.Options.MonsterMovementSpeed) / 2f;
					Vector3 a2 = Vector3.MoveTowards(transform.position, TargetPosition, maxDistanceDelta2);
					m_DeltaPosition += a2 - transform.position;
				}
			}
			if (IsRotating && !IsMoving)
			{
				GotOptimalAngle();
				if (!m_OptimalAngle)
				{
					switch (m_Direction)
					{
					case EDirection.NORTH:
						if (m_DirectionAngle < 30f)
						{
							transform.Rotate(0f, -0.5f, 0f);
						}
						else
						{
							transform.Rotate(0f, 0.5f, 0f);
						}
						break;
					case EDirection.EAST:
						if (m_DirectionAngle < 300f)
						{
							transform.Rotate(0f, -0.5f, 0f);
						}
						else
						{
							transform.Rotate(0f, 0.5f, 0f);
						}
						break;
					case EDirection.SOUTH:
						if (m_DirectionAngle < 210f)
						{
							transform.Rotate(0f, -0.5f, 0f);
						}
						else
						{
							transform.Rotate(0f, 0.5f, 0f);
						}
						break;
					case EDirection.WEST:
						if (m_DirectionAngle < 120f)
						{
							transform.Rotate(0f, -0.5f, 0f);
						}
						else
						{
							transform.Rotate(0f, 0.5f, 0f);
						}
						break;
					}
				}
			}
		}

		private void RotateOrMove()
		{
			Single num = ConfigManager.Instance.Options.MonsterMovementSpeed;
			Single num2 = 0f;
			if (num == 0f)
			{
				num = 1f;
			}
			if (m_InAttack && m_ResetSpeed)
			{
				m_Animator.speed = m_AnimSpeedRef;
				m_ResetSpeed = false;
			}
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
				target2 = vector.magnitude / m_MinWalkDistance / num;
				direction = vector;
				direction.y = 0f;
				direction.Normalize();
				direction = transform.InverseTransformDirection(direction);
				if (!m_ResetSpeed)
				{
					m_AnimSpeedRef = m_Animator.speed;
				}
				m_Animator.speed = num;
				m_ResetSpeed = true;
				if (Math.Abs(vector.x) < m_MinMoveRadius && Math.Abs(vector.z) < m_MinMoveRadius)
				{
					m_IsMoving.Value = false;
				}
			}
			if (m_IsRotating.RawValue)
			{
				target = m_RotationDirection;
				num2 = Vector3.Angle(transform.forward, m_tmpLookDirection);
				if (!m_ResetSpeed)
				{
					m_AnimSpeedRef = m_Animator.speed;
				}
				m_Animator.speed = num;
				m_ResetSpeed = true;
				if (num2 < m_MinAngle * 5f)
				{
					if (m_RotationDirection > 0)
					{
						target = 0.75f / num;
					}
					else
					{
						target = -0.75f / num;
					}
				}
				if (num2 <= m_MinAngle)
				{
					m_IsRotating.Value = false;
					m_RotationTry = 0;
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
			if (num2 > m_MinAngle)
			{
				m_DeltaRotation = m_Animator.deltaRotation;
			}
			else
			{
				m_DeltaRotation.y = 0f;
			}
			if (!m_IsMoving.RawValue && !m_IsRotating.RawValue)
			{
				m_tmpLookDirection = vector;
				m_tmpLookDirection.y = 0f;
				m_tmpLookDirection.Normalize();
				num2 = Vector3.Angle(transform.forward, m_tmpLookDirection);
				Boolean flag = (Math.Abs(vector.x) > m_MinTurnRadius || Math.Abs(vector.z) > m_MinTurnRadius) && num2 > m_MinAngle;
				if (flag)
				{
					m_IsMoving.Value = false;
					m_IsRotating.Value = true;
					m_RotationDirection = Helper.AngleDir(transform.forward, m_tmpLookDirection, Vector3.up);
					if (m_RotationDirection == 0)
					{
						m_RotationDirection = ((Random.Value >= 0.5f) ? -1 : 1);
					}
				}
				else if (Math.Abs(vector.x) > m_MinMoveRadius || Math.Abs(vector.z) > m_MinMoveRadius)
				{
					m_IsMoving.Value = true;
					m_IsRotating.Value = false;
					m_StartPosition = transform.position;
					m_TargetDistance = Vector3.Distance(m_StartPosition, m_TargetPosition);
				}
				else
				{
					m_tmpLookDirection = m_TargetRotation * Vector3.forward;
					m_tmpLookDirection.y = 0f;
					m_tmpLookDirection.Normalize();
					num2 = Vector3.Angle(transform.forward, m_tmpLookDirection);
					if (num2 > m_MinAngle)
					{
						m_IsMoving.Value = false;
						m_IsRotating.Value = true;
						m_RotationDirection = Helper.AngleDir(transform.forward, m_tmpLookDirection, Vector3.up);
						if (m_RotationDirection == 0)
						{
							m_RotationDirection = ((Random.Value >= 0.5f) ? -1 : 1);
						}
					}
				}
			}
		}

		protected override void Update()
		{
			RotateOrMove();
			MotionAndRotationHelper();
			StateCheck();
		}
	}
}
