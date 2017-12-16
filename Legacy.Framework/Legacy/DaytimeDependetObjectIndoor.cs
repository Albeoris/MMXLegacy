using System;
using Legacy.Core.Api;
using UnityEngine;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/World/Daytime Dependent Object Indoor")]
	public class DaytimeDependetObjectIndoor : MonoBehaviour
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

		protected void Awake()
		{
			ChangeState(LegacyLogic.Instance.GameTime.DayState);
		}

		protected void OnEnable()
		{
			ChangeState(LegacyLogic.Instance.GameTime.DayState);
		}

		public void ChangeState(EDayState newState)
		{
			SetObjectState(m_dawn, EDayState.DAWN, newState);
			SetObjectState(m_day, EDayState.DAY, newState);
			SetObjectState(m_dusk, EDayState.DUSK, newState);
			SetObjectState(m_night, EDayState.NIGHT, newState);
		}

		private void SetObjectState(GameObject p_obj, EDayState p_matchstate, EDayState p_newState)
		{
			if (p_obj != null)
			{
				p_obj.SetActive(p_matchstate == p_newState);
			}
		}
	}
}
