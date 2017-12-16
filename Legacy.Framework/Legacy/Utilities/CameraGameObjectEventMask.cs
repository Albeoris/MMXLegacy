using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[RequireComponent(typeof(Camera))]
	public class CameraGameObjectEventMask : MonoBehaviour
	{
		[SerializeField]
		private LayerMask m_LayerMask;

		private void Awake()
		{
			camera.eventMask = m_LayerMask;
		}
	}
}
