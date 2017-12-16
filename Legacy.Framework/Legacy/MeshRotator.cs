using System;
using UnityEngine;

namespace Legacy
{
	public class MeshRotator : MonoBehaviour
	{
		[SerializeField]
		public Single m_RotationSpeed = 1f;

		[SerializeField]
		public Vector3 m_direction = Vector3.down;

		private void Update()
		{
			transform.Rotate(m_direction * Time.deltaTime * m_RotationSpeed);
		}
	}
}
