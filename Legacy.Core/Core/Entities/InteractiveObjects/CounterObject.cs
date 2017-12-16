using System;
using System.IO;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class CounterObject : InteractiveObject
	{
		private Int32 m_currentCount;

		private Int32 m_maxCount;

		private Boolean m_withOverflow;

		public CounterObject() : this(0, 0)
		{
		}

		public CounterObject(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.COUNTING_OBJECT, p_spawnerID)
		{
			m_currentCount = 1;
			m_maxCount = 1;
			m_withOverflow = false;
		}

		public Int32 CurrentCount => m_currentCount;

	    public Int32 MaxCount => m_maxCount;

	    public override EInteractiveObjectState State
		{
			get => base.State;
	        set
			{
				if (value < EInteractiveObjectState.COUNTER_1 || value > EInteractiveObjectState.COUNTER_9)
				{
					throw new ArgumentException("Invalid CounterObject state: " + value);
				}
				base.State = value;
			}
		}

		public void ChangeCounter(Int32 p_delta)
		{
			Int32 currentCount = m_currentCount;
			if (m_withOverflow)
			{
				p_delta %= m_maxCount;
				m_currentCount += p_delta;
				if (m_currentCount > m_maxCount)
				{
					m_currentCount -= m_maxCount;
				}
				if (m_currentCount < 1)
				{
					m_currentCount += m_maxCount;
				}
			}
			else
			{
				m_currentCount += p_delta;
				m_currentCount = Math.Max(1, Math.Min(m_currentCount, m_maxCount));
			}
			UpdateState();
			CounterObjectChangedArgs p_eventArgs = new CounterObjectChangedArgs(currentCount, m_currentCount, p_delta);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.OBJECT_COUNTER_CHANGED, p_eventArgs);
		}

		public void SetCounter(Int32 p_count)
		{
			Int32 currentCount = m_currentCount;
			m_currentCount = Math.Max(1, Math.Min(p_count, m_maxCount));
			UpdateState();
			CounterObjectChangedArgs p_eventArgs = new CounterObjectChangedArgs(currentCount, m_currentCount, 0);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.OBJECT_COUNTER_CHANGED, p_eventArgs);
		}

		public override void SetData(EInteractiveObjectData p_key, String p_value)
		{
			if (p_key == EInteractiveObjectData.COUNTER_OBJECT_DATA)
			{
				String[] array = p_value.Split(new Char[]
				{
					','
				});
				if (array.Length != 3)
				{
					throw new InvalidDataException("Invalid Data for Counter object");
				}
				m_currentCount = Convert.ToInt32(array[0]);
				m_maxCount = Convert.ToInt32(array[1]);
				UpdateState();
				if (String.Compare(array[2], "TRUE", true) == 0)
				{
					m_withOverflow = true;
				}
				else
				{
					m_withOverflow = false;
				}
			}
			else
			{
				base.SetData(p_key, p_value);
			}
		}

		private void UpdateState()
		{
			State = EInteractiveObjectState.CONTAINER_OPENED + m_currentCount;
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			m_currentCount = p_data.Get<Int32>("CurrentCount", 1);
			m_maxCount = p_data.Get<Int32>("MaxCount", 1);
			m_withOverflow = p_data.Get<Boolean>("WithOverflow", true);
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<Int32>("CurrentCount", m_currentCount);
			p_data.Set<Int32>("MaxCount", m_maxCount);
			p_data.Set<Boolean>("WithOverflow", m_withOverflow);
		}
	}
}
