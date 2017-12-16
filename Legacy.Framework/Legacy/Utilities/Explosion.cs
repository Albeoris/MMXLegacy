using System;
using UnityEngine;

namespace Legacy.Utilities
{
	public class Explosion : MonoBehaviour
	{
		[SerializeField]
		private Single m_radius = 5f;

		[SerializeField]
		private Single m_force = 100f;

		[SerializeField]
		private Boolean m_explodeOnStart;

		public void Explode()
		{
			Collider[] array = Physics.OverlapSphere(transform.position, m_radius);
			for (Int32 i = 0; i < array.Length; i++)
			{
				Rigidbody attachedRigidbody = array[i].attachedRigidbody;
				if (attachedRigidbody != null)
				{
					attachedRigidbody.AddExplosionForce(m_force, transform.position, m_radius);
				}
			}
		}

		private void Start()
		{
			if (m_explodeOnStart)
			{
				Explode();
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, m_radius);
		}
	}
}
