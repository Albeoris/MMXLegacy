using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Animations
{
	public abstract class BaseEventHandler : MonoBehaviour
	{
		protected AnimEvent[] m_animEvents = new AnimEvent[8];

		public void RegisterAnimationCallback(EAnimEventType p_Type, Action p_Callback)
		{
			AnimEvent animEvent = m_animEvents[(Int32)p_Type];
			if (animEvent == null)
			{
				animEvent = (m_animEvents[(Int32)p_Type] = new AnimEvent());
			}
			animEvent.RegisterCallback(p_Callback);
		}

		public void OnEvent(EAnimEventType p_Type)
		{
			AnimEvent animEvent = m_animEvents[(Int32)p_Type];
			if (animEvent != null)
			{
				animEvent.InvokeCallbacks();
			}
		}

		protected virtual void Update()
		{
			for (Int32 i = 0; i < m_animEvents.Length; i++)
			{
				if (m_animEvents[i] != null && m_animEvents[i].InvokeCallbacksIfTimeOut())
				{
					Debug.LogError(String.Concat(new Object[]
					{
						name,
						"'s ",
						(EAnimEventType)i,
						" Animation Event Missing!"
					}), this);
				}
			}
		}

		public class AnimEvent
		{
			private const Single EVENT_TIME_OUT = 1.5f;

			private List<Action> m_eventCallbacks = new List<Action>();

			private Single m_lastRegCallbackTime;

			public void RegisterCallback(Action pCallback)
			{
				m_eventCallbacks.Add(pCallback);
				m_lastRegCallbackTime = Time.time + 1.5f;
			}

			public Boolean InvokeCallbacksIfTimeOut()
			{
				if (m_eventCallbacks.Count > 0 && m_lastRegCallbackTime < Time.time)
				{
					InvokeCallbacks();
					return true;
				}
				return false;
			}

			public void InvokeCallbacks()
			{
				if (m_eventCallbacks.Count > 0)
				{
					foreach (Action action in m_eventCallbacks)
					{
						try
						{
							action();
						}
						catch (Exception message)
						{
							Debug.LogError(message);
						}
					}
					m_eventCallbacks.Clear();
				}
			}
		}
	}
}
