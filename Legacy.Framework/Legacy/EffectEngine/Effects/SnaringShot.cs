using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class SnaringShot : MonoBehaviour
	{
		[SerializeField]
		private Single MAX_Y_FOR_BONES = 1f;

		[SerializeField]
		private Single WEIGHT_FACTOR = 0.7f;

		[SerializeField]
		private Single MOVE_OUTSIDE_SPEED_FATOR = 4f;

		[SerializeField]
		private Single BONE_OFFSET = 0.1f;

		[SerializeField]
		private Single BONE_OFFSET_AXIS_MAX = 1f;

		[SerializeField]
		private Single ROTATION_SPEED = 400f;

		private Single MOVE_OUTSIDE_SPEED = -1f;

		[SerializeField]
		private Single MOVE_UP_SPEED = 5f;

		[SerializeField]
		private Single MOVE_UP_SPEED_RND_OFFSET = 1f;

		[SerializeField]
		private Single MOVE_UP_SPEED_RND_FORCE = 1f;

		[SerializeField]
		private Single POSITION_RND = 1f;

		[SerializeField]
		private Single BUILD_UP_TIME = 1f;

		[SerializeField]
		private Int32 TRAIL_COUNT = 25;

		private Single m_halfTime;

		private Single m_timeLeft = -1f;

		private Single m_rndNum;

		private Transform m_trail;

		private SkinnedMeshRenderer m_targetSmr;

		private Boolean m_isDone;

		private void Start()
		{
			if (MOVE_OUTSIDE_SPEED == -1f)
			{
				if (m_targetSmr != null)
				{
					Transform[] array = new Transform[6];
					Single[] array2 = new Single[6];
					for (Int32 i = 0; i < 6; i++)
					{
						array2[i] = -1f;
					}
					Transform[] bones = m_targetSmr.bones;
					for (Int32 j = 0; j < m_targetSmr.bones.Length; j++)
					{
						Single num = Mathf.Abs(transform.position.y - bones[j].position.y);
						for (Int32 k = 0; k < 6; k++)
						{
							if (array2[k] == -1f || num < array2[k])
							{
								array[k] = bones[j];
								array2[k] = num;
								break;
							}
						}
					}
					Single num2 = 0f;
					for (Int32 l = 0; l < 5; l++)
					{
						if (array[l] != null && array[l + 1] != null)
						{
							num2 += (array[l].position - array[l + 1].position).magnitude;
						}
						else
						{
							num2 += 1f;
						}
					}
					num2 /= 5f;
					MOVE_OUTSIDE_SPEED = num2 * MOVE_OUTSIDE_SPEED_FATOR;
				}
				else
				{
					MOVE_OUTSIDE_SPEED = 5f;
				}
			}
			m_trail = transform.GetChild(0);
			m_halfTime = BUILD_UP_TIME * 0.5f;
			m_rndNum = Random.Value;
			if (m_timeLeft == -1f)
			{
				m_timeLeft = BUILD_UP_TIME;
				for (Int32 m = 0; m < TRAIL_COUNT - 1; m++)
				{
					GameObject gameObject = Helper.Instantiate<GameObject>(this.gameObject, transform.position, transform.rotation);
					SnaringShot component = gameObject.GetComponent<SnaringShot>();
					component.m_timeLeft = BUILD_UP_TIME;
					component.m_targetSmr = m_targetSmr;
					component.MOVE_OUTSIDE_SPEED = MOVE_OUTSIDE_SPEED;
					gameObject.transform.parent = transform.parent;
					gameObject.transform.Rotate(Vector3.up, Random.Value * 360f);
					gameObject.transform.localPosition += UnityEngine.Random.insideUnitSphere * POSITION_RND;
				}
			}
			else
			{
				if (Random.Value > 0.5f)
				{
					ROTATION_SPEED *= -1f;
				}
				MOVE_UP_SPEED += MOVE_UP_SPEED_RND_OFFSET * Random.Value;
			}
		}

		private void Update()
		{
			if (m_isDone)
			{
				return;
			}
			if (m_timeLeft > 0.025)
			{
				Single num = Mathf.Min(Time.deltaTime, 0.035f);
				m_timeLeft -= num;
				Vector3 localEulerAngles = transform.localEulerAngles;
				localEulerAngles.y += ROTATION_SPEED * num;
				transform.localEulerAngles = localEulerAngles;
				Vector3 vector = transform.localPosition;
				vector.y += MOVE_UP_SPEED * num;
				MOVE_UP_SPEED += Mathf.Sign(Mathf.PerlinNoise(num, m_rndNum) - 0.5f) * Mathf.PerlinNoise(transform.localPosition.y, m_rndNum) * MOVE_UP_SPEED_RND_FORCE * num;
				transform.localPosition = vector;
				vector = m_trail.localPosition;
				Vector3 vector2 = vector;
				vector2.y = 0f;
				vector2.Normalize();
				vector2 *= MOVE_OUTSIDE_SPEED * num * (2f - 2f * m_timeLeft / m_halfTime);
				if (vector2.magnitude > 0.1)
				{
					vector -= vector2;
				}
				m_trail.localPosition = vector;
			}
			else
			{
				AdvancedTrailRenderer trail = GetComponentInChildren<AdvancedTrailRenderer>();
				if (trail != null)
				{
					trail.enabled = false;
					if (trail.trailObj != null)
					{
						trail.trailObj.transform.parent = transform;
						if (m_targetSmr != null)
						{
							StartCoroutine(SkinnedMeshRendererGenerator.GenerateSkin(trail.mesh, m_targetSmr, gameObject, trail.instanceMaterial, delegate
							{
								Destroy(trail.trailObj);
							}, BONE_OFFSET, MAX_Y_FOR_BONES, WEIGHT_FACTOR, BONE_OFFSET_AXIS_MAX));
						}
					}
				}
				m_isDone = true;
			}
		}

		private void OnBeginEffect(UnityEventArgs<FXArgs> e)
		{
			m_targetSmr = GetSmr(e.EventArgs.Target);
		}

		private SkinnedMeshRenderer GetSmr(GameObject p_go)
		{
			SkinnedMeshRenderer result = null;
			SkinnedMeshRenderer[] componentsInChildren = p_go.GetComponentsInChildren<SkinnedMeshRenderer>();
			Single num = 0f;
			for (Int32 i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].sharedMesh != null && num < componentsInChildren[i].sharedMesh.vertexCount)
				{
					num = componentsInChildren[i].sharedMesh.vertexCount;
					result = componentsInChildren[i];
				}
			}
			return result;
		}
	}
}
