using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.World;
using UnityEngine;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/World/Daytime Dependent Object")]
	public class DaytimeDependentObject : DaytimeDependentBase
	{
		private const Single RANDOM_RANGE = 1f;

		[SerializeField]
		private GameObject m_dawn;

		[SerializeField]
		private GameObject m_day;

		[SerializeField]
		private GameObject m_dusk;

		[SerializeField]
		private GameObject m_night;

		public override void ChangeState(EDayState newState)
		{
			StartCoroutine(UpdateObjects(GetTargetObject(newState)));
		}

		private GameObject GetTargetObject(EDayState state)
		{
			switch (state)
			{
			case EDayState.DAWN:
				return m_dawn;
			case EDayState.DAY:
				return m_day;
			case EDayState.DUSK:
				return m_dusk;
			default:
				return m_night;
			}
		}

		private IEnumerator UpdateObjects(GameObject p_nextObject)
		{
			yield return new WaitForSeconds(UnityEngine.Random.value * 1f);
			UpdateObjects(m_dawn, p_nextObject);
			UpdateObjects(m_day, p_nextObject);
			UpdateObjects(m_dusk, p_nextObject);
			UpdateObjects(m_night, p_nextObject);
			yield break;
		}

		private static void UpdateObjects(GameObject p_setting, GameObject p_nextSetting)
		{
			if (p_setting != null)
			{
				if (p_nextSetting == p_setting)
				{
					if (!p_setting.activeSelf)
					{
						p_setting.SetActive(true);
					}
				}
				else if (p_setting.activeSelf)
				{
					p_setting.SetActive(false);
				}
			}
		}
	}
}
