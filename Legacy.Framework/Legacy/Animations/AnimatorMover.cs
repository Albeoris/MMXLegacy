using System;
using Legacy.Core.Map;
using UnityEngine;

namespace Legacy.Animations
{
	public class AnimatorMover : MonoBehaviour
	{
		[SerializeField]
		public AnimatorControl m_anim;

		[SerializeField]
		public Animator m_Animator;

		[SerializeField]
		public Single m_SpeedModifierRef = 0.05f;

		[SerializeField]
		private Boolean m_Movement_Function;

		private Single m_SpeedModifier = 0.05f;

		private Boolean m_Move;

		private EDirection m_Direction = EDirection.CENTER;

		private Single m_DirectionAngle;

		private Boolean m_OptimalAngle;

		private Int32 m_MoveHash;

		private void Start()
		{
			m_MoveHash = Animator.StringToHash("MOVE");
		}

		private void CurrentDirection()
		{
			m_DirectionAngle = transform.localEulerAngles.y;
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
				AdjustSpeedToTransition();
			}
			else
			{
				m_Move = false;
				AdjustSpeedToTransition();
			}
		}

		private void AdjustSpeedToTransition()
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

		private void Update()
		{
			Single moveSpeed = m_anim.MoveSpeed;
			Boolean isMoving = m_anim.IsMoving;
			Boolean isRotating = m_anim.IsRotating;
			if (moveSpeed <= 0.01f || !isMoving)
			{
				m_SpeedModifier /= 2f;
			}
			CurrentDirection();
			GetCurrentHashName();
			if (m_Movement_Function && m_Move && isMoving && moveSpeed > 0f)
			{
				Vector3 position = Vector3.MoveTowards(transform.position, m_anim.TargetPosition, m_SpeedModifier);
				transform.position = position;
			}
			if (!isRotating)
			{
				GotOptimalAngle();
				if (!m_OptimalAngle)
				{
					switch (m_Direction)
					{
					case EDirection.NORTH:
						if (m_DirectionAngle < 30f)
						{
							transform.Rotate(0f, -0.2f, 0f);
						}
						else
						{
							transform.Rotate(0f, 0.2f, 0f);
						}
						break;
					case EDirection.EAST:
						if (m_DirectionAngle < 300f)
						{
							transform.Rotate(0f, -0.2f, 0f);
						}
						else
						{
							transform.Rotate(0f, 0.2f, 0f);
						}
						break;
					case EDirection.SOUTH:
						if (m_DirectionAngle < 210f)
						{
							transform.Rotate(0f, -0.2f, 0f);
							Debug.Log("muhhh");
						}
						else
						{
							transform.Rotate(0f, 0.2f, 0f);
							Debug.Log("muhhh");
						}
						break;
					case EDirection.WEST:
						if (m_DirectionAngle < 120f)
						{
							transform.Rotate(0f, -0.2f, 0f);
						}
						else
						{
							transform.Rotate(0f, 0.2f, 0f);
						}
						break;
					}
				}
			}
		}
	}
}
