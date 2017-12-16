using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	[AddComponentMenu("MM Legacy/HarpoonFX")]
	public class HarpoonFX : MonoBehaviour
	{
		private const Single GROUND_OFFSET = 0.1f;

		private const Single MIN_TRAIL_DIST = 0.1f;

		private const Single MAX_TRAIL_DIST = 7f;

		private const Single PULL_TIME = 1f;

		[SerializeField]
		private AdvancedLineRenderer m_ropeTrail;

		private Vector3[] m_velocities = new Vector3[0];

		private State m_state;

		private Int32 m_currRopeIndex = 1;

		private Single m_pullTime;

		private Single m_slotTargetY;

		private Boolean m_isOnGround;

		private Boolean m_isRippedOff;

		public void Start()
		{
			if (m_ropeTrail == null)
			{
				Debug.LogError("HarpoonFX: AdvancedLineRenderer is null!");
			}
			m_ropeTrail.SetVertexCount(2);
			m_ropeTrail.SetPosition(0, m_ropeTrail.transform.position);
			m_ropeTrail.SetPosition(1, m_ropeTrail.transform.position);
		}

		public void Update()
		{
			switch (m_state)
			{
			case State.MOVE_TO_TARGET:
				MoveToTarget();
				break;
			case State.PULL:
				Pull();
				break;
			case State.FALL_TO_GROUND:
				FallToGround();
				break;
			}
		}

		public void OnBeginEffect(UnityEventArgs p_args)
		{
			FXArgs fxargs = (FXArgs)p_args.EventArgs;
			m_slotTargetY = fxargs.SlotTargetPosition.y + 0.1f;
		}

		public void OnEndEffect()
		{
			AudioController.Play("Harpoon_Pull", transform);
			m_state = State.PULL;
		}

		private void MoveToTarget()
		{
			Single magnitude = (m_ropeTrail.GetPosition(m_currRopeIndex - 1) - m_ropeTrail.GetPosition(m_currRopeIndex)).magnitude;
			if (magnitude >= 0.1f)
			{
				m_currRopeIndex++;
				m_ropeTrail.SetVertexCount(m_ropeTrail.GetVertexCount() + 1);
			}
			m_ropeTrail.SetPosition(m_currRopeIndex, m_ropeTrail.transform.position);
		}

		private void Pull()
		{
			if (m_pullTime < 1f)
			{
				Vector3 position = m_ropeTrail.GetPosition(0);
				Vector3 position2 = m_ropeTrail.transform.position;
				Int32 vertexCount = m_ropeTrail.GetVertexCount();
				for (Int32 i = 1; i < vertexCount; i++)
				{
					m_ropeTrail.SetPosition(i, Vector3.Lerp(position, position2, i / (Single)(vertexCount - 1)));
				}
				m_pullTime += Time.deltaTime;
			}
			else
			{
				m_velocities = new Vector3[m_ropeTrail.GetVertexCount()];
				for (Int32 j = 0; j < m_velocities.Length; j++)
				{
					m_velocities[j] = Vector3.zero;
				}
				m_state = State.FALL_TO_GROUND;
			}
		}

		private void FallToGround()
		{
			Single magnitude = (m_ropeTrail.GetPosition(m_ropeTrail.GetVertexCount() - 1) - m_ropeTrail.GetPosition(m_ropeTrail.GetVertexCount() - 2)).magnitude;
			if (magnitude > 7f)
			{
				m_isRippedOff = true;
				m_isOnGround = false;
			}
			if (!m_isRippedOff)
			{
				m_ropeTrail.SetPosition(m_ropeTrail.GetVertexCount() - 1, m_ropeTrail.transform.position);
			}
			if (!m_isOnGround)
			{
				m_isOnGround = true;
				Int32 vertexCount = m_ropeTrail.GetVertexCount();
				for (Int32 i = 0; i < ((!m_isRippedOff) ? (vertexCount - 1) : vertexCount); i++)
				{
					Vector3 vector = m_ropeTrail.GetPosition(i);
					if (vector.y > m_slotTargetY)
					{
						m_isOnGround = false;
						Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
						insideUnitSphere.y *= 0.1f;
						m_velocities[i] += (Vector3.down * 9.81f + insideUnitSphere * 2f) * Time.deltaTime;
						vector += m_velocities[i] * Time.deltaTime;
						vector.y = Mathf.Max(vector.y, m_slotTargetY);
						m_ropeTrail.SetPosition(i, vector);
					}
				}
			}
		}

		private enum State
		{
			MOVE_TO_TARGET,
			PULL,
			FALL_TO_GROUND
		}
	}
}
