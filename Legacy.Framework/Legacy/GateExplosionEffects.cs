using System;
using UnityEngine;

namespace Legacy
{
	public class GateExplosionEffects : MonoBehaviour
	{
		private const Single m_kickAwayForceMin = 250f;

		private const Single m_kickAwayForceMax = 450f;

		[SerializeField]
		private ParticleSystem m_sparksParticles;

		[SerializeField]
		private ParticleSystem m_dustParticles;

		private Boolean m_underwater;

		private Rigidbody m_rigidbody;

		private Single m_delay;

		private Boolean m_dontMove;

		private void Awake()
		{
			m_rigidbody = rigidbody;
		}

		private void OnEnable()
		{
			m_rigidbody.isKinematic = false;
			m_rigidbody.WakeUp();
			m_delay = Time.time + 2f;
		}

		private void OnDisable()
		{
			m_rigidbody.isKinematic = true;
		}

		private void LateUpdate()
		{
			if (m_delay < Time.time && m_rigidbody.IsSleeping())
			{
				m_dontMove = true;
				enabled = false;
			}
			else
			{
				m_dontMove = false;
			}
		}

		private void FixedUpdate()
		{
			if (m_dontMove && !m_rigidbody.isKinematic)
			{
				m_rigidbody.velocity = Vector3.zero;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Water")
			{
				m_underwater = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.tag == "Water")
			{
				m_underwater = false;
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!m_dontMove)
			{
				foreach (ContactPoint contactPoint in collision.contacts)
				{
					if (m_underwater)
					{
						AudioController.Play("MetalOnMud", transform);
					}
					else if (contactPoint.thisCollider.sharedMaterial == contactPoint.otherCollider.sharedMaterial)
					{
						m_sparksParticles.transform.position = contactPoint.point;
						m_sparksParticles.Play(true);
						AudioController.Play("MetalOnMetal", transform);
					}
					else
					{
						m_dustParticles.transform.position = contactPoint.point;
						m_dustParticles.Play(true);
						AudioController.Play("MetalOnDirt", transform);
					}
				}
			}
			else
			{
				KickAway(collision.gameObject, collision.collider);
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			KickAway(collision.gameObject, collision.collider);
		}

		private void KickAway(GameObject p_Model, Collider p_Collider)
		{
			if (p_Model.name == "Model" || p_Model.name.StartsWith("Monster"))
			{
				Vector3 normalized = (transform.position - p_Model.transform.position).normalized;
				normalized.y = 0.5f;
				normalized.Normalize();
				m_rigidbody.AddForce(normalized * UnityEngine.Random.Range(250f, 450f));
				m_dontMove = false;
				m_delay = Time.time + 1f;
				Collider componentInChildren = GetComponentInChildren<Collider>();
				if (componentInChildren != null)
				{
					Physics.IgnoreCollision(componentInChildren, p_Collider);
				}
			}
		}
	}
}
