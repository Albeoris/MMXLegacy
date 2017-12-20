using System;
using System.Collections.Generic;
using Legacy.Utilities;

namespace Legacy.Core.EventManagement
{
    public sealed class EventManager<T> where T : EventArgs
    {
        public event Action<T> Event;

        public void Invoke(T args)
        {
            try
            {
                Event?.Invoke(args);
            }
            catch (Exception ex)
            {
                LegacyLogger.LogError(String.Concat("Event exception '", args.GetType().Name, "'\n", ex), true);
            }
        }

        public void TryInvoke(Func<T> args)
        {
            try
            {
                Action<T> evt = Event;
                if (evt == null)
                    return;

                evt(args());
            }
            catch (Exception ex)
            {
                LegacyLogger.LogError(String.Concat("Event exception '", args.GetType().Name, "'\n", ex), true);
            }
        }
    }

    public class EventManager
	{
		private readonly List<EventHandler>[] m_eventMap;
        private readonly Dictionary<Type, Object> m_genericEventMap;

        public EventManager()
		{
			Array values = Enum.GetValues(typeof(EEventType));
			m_eventMap = new List<EventHandler>[values.Length];
		    m_genericEventMap = new Dictionary<Type, Object>();
            for (Int32 i = 0; i < m_eventMap.Length; i++)
				m_eventMap[i] = new List<EventHandler>();
		}

	    public EventManager<T> Get<T>() where T : EventArgs
	    {
	        Type key = TypeCache<T>.Type;

	        if (m_genericEventMap.TryGetValue(key, out var value))
	            return (EventManager<T>)value;

	        EventManager<T> evt = new EventManager<T>();
	        m_genericEventMap.Add(key, evt);
	        return evt;
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
