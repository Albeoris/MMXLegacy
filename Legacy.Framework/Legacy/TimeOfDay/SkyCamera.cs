using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	public class SkyCamera : MonoBehaviour
	{
		internal static SkyCamera m_CurrentCamera;

		internal Transform m_CachedTransfrom;

		public Sky sky;

		private void Awake()
		{
			m_CachedTransfrom = transform;
		}

		private void OnEnable()
		{
			if (sky == null)
			{
				Sky x = (Sky)FindObjectOfType(typeof(Sky));
				if (x != null)
				{
					sky = x;
				}
			}
			if (sky == null)
			{
				Debug.LogWarning("Sky instance reference not set. Disabling script.", this);
				enabled = false;
			}
		}

		private void OnDisable()
		{
			if (m_CurrentCamera == this)
			{
				m_CurrentCamera = null;
			}
		}

		protected virtual void OnPreCull()
		{
			m_CurrentCamera = this;
			sky.transform.position = m_CachedTransfrom.position;
		}

		protected virtual void OnPostRender()
		{
			m_CurrentCamera = null;
		}
	}
}
