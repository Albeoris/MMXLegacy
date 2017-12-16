using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	public class SkyWaterCamera : SkyCamera
	{
		protected override void OnPreCull()
		{
			sky.transform.position = m_CachedTransfrom.position;
		}

		protected override void OnPostRender()
		{
			if (m_CurrentCamera != null)
			{
				sky.transform.position = m_CurrentCamera.m_CachedTransfrom.position;
			}
		}
	}
}
