using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.World
{
	public class DaytimeDependentController : MonoBehaviour
	{
		private static DaytimeDependentController s_Instance;

		private List<DaytimeDependentBase> m_Objects = new List<DaytimeDependentBase>();

		private EDayState m_state = (EDayState)(-1);

		public static DaytimeDependentController Instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = (FindObjectOfType(typeof(DaytimeDependentController)) as DaytimeDependentController);
					if (s_Instance == null)
					{
						GameObject gameObject = new GameObject("_Singleton " + typeof(DaytimeDependentController).Name);
						s_Instance = gameObject.AddComponent<DaytimeDependentController>();
					}
				}
				return s_Instance;
			}
		}

		public static void Register(DaytimeDependentBase obj)
		{
			Instance.m_Objects.Add(obj);
		}

		public static void Unregister(DaytimeDependentBase obj)
		{
			Instance.m_Objects.Remove(obj);
		}

		protected virtual void Awake()
		{
			if (s_Instance != null && s_Instance != this)
			{
				Destroy(this);
				throw new Exception(typeof(DaytimeDependentController) + "\nInstance already set! by -> " + s_Instance);
			}
			s_Instance = this;
		}

		protected virtual void Start()
		{
			OnGameTimeChanged(null, null);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnGameTimeChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnFinishLoadViews));
		}

		protected virtual void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnGameTimeChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnFinishLoadViews));
			s_Instance = null;
			m_Objects.Clear();
		}

		private void OnFinishLoadViews(Object p_Sender, EventArgs p_Args)
		{
			m_state = (EDayState)(-1);
			OnGameTimeChanged(null, null);
		}

		private void OnGameTimeChanged(Object p_Sender, EventArgs p_Args)
		{
			EDayState dayState = LegacyLogic.Instance.GameTime.DayState;
			if (dayState != m_state)
			{
				m_state = dayState;
				foreach (DaytimeDependentBase daytimeDependentBase in m_Objects)
				{
					if (daytimeDependentBase != null)
					{
						daytimeDependentBase.ChangeState(dayState);
					}
				}
			}
		}
	}
}
