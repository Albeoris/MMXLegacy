using System;
using System.Collections.Generic;
using Legacy.Utilities;

namespace Legacy.Core.EventManagement
{
	public class EventManager
	{
		private List<EventHandler>[] m_eventMap;

		public EventManager()
		{
			Array values = Enum.GetValues(typeof(EEventType));
			m_eventMap = new List<EventHandler>[values.Length];
			for (Int32 i = 0; i < m_eventMap.Length; i++)
			{
				m_eventMap[i] = new List<EventHandler>();
			}
		}

		public void RegisterEvent(EEventType p_type, EventHandler p_delegate)
		{
			m_eventMap[(Int32)p_type].Add(p_delegate);
		}

		public void UnregisterEvent(EEventType p_type, EventHandler p_delegate)
		{
			m_eventMap[(Int32)p_type].Remove(p_delegate);
		}

		internal void InvokeEvent(Object p_sender, EEventType p_type, EventArgs p_eventArgs)
		{
			List<EventHandler> list = m_eventMap[(Int32)p_type];
			for (Int32 i = 0; i < list.Count; i++)
			{
				try
				{
					list[i](p_sender, p_eventArgs);
				}
				catch (Exception ex)
				{
					LegacyLogger.LogError(String.Concat(new Object[]
					{
						"Event exception '",
						p_type,
						"'\n",
						ex
					}), true);
				}
			}
		}
	}
}
