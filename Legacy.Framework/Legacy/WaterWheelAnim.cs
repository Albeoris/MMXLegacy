using System;
using UnityEngine;

namespace Legacy
{
	public class WaterWheelAnim : MonoBehaviour
	{
		[SerializeField]
		public Single m_RotationSpeed = 1f;

		private void Update()
		{
			transform.Rotate(Vector3.down * Time.deltaTime * m_RotationSpeed);
		}
	}
}
