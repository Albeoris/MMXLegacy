using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[RequireComponent(typeof(Camera))]
	public class CameraEnableOcclusionCulling : MonoBehaviour
	{
		[SerializeField]
		private Boolean m_OcclusionCulling = true;

		public Boolean OcclusionCulling
		{
			get => camera.useOcclusionCulling;
		    set => camera.useOcclusionCulling = value;
		}

		private void Awake()
		{
			camera.useOcclusionCulling = m_OcclusionCulling;
		}
	}
}
