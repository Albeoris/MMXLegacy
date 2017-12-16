using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.Map;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Api
{
	public class GameTime : ISaveGameObject
	{
		private MMTime m_time;

		private MMTime m_lastTime;

		private MMCalendar m_calendar;

		private Int32 m_turn;

		internal GameTime()
		{
		}

		public MMTime Time => m_time;

	    public MMTime LastTime => m_lastTime;

	    public MMCalendar Calendar => m_calendar;

	    public EDayState DayState => GetDayState(m_time.Hours);

	    public EDayState LastDayState => GetDayState(m_lastTime.Hours);

	    public Int32 Turn => m_turn;

	    internal void Init(Int32 p_days, Int32 p_hours, Int32 p_minutes)
		{
			m_lastTime = (m_time = new MMTime(p_minutes, p_hours, p_days));
			m_turn = 0;
			m_calendar = new MMCalendar();
			m_calendar.Init(MMCalendar.EWeekDays.ASHDA, 1, MMCalendar.EMonths.SUN_BLOSSOM, 575);
		}

		internal void UpdateTime(EMapType p_mapType, ETimeChangeReason p_reason)
		{
			switch (p_mapType)
			{
			case EMapType.OUTDOOR:
				UpdateTime(ConfigManager.Instance.Game.MinutesPerTurnOutdoor, p_reason);
				break;
			case EMapType.CITY:
				UpdateTime(ConfigManager.Instance.Game.MinutesPerTurnCity, p_reason);
				break;
			case EMapType.DUNGEON:
				UpdateTime(ConfigManager.Instance.Game.MinutesPerTurnDungeon, p_reason);
				break;
			default:
				throw new NotImplementedException("GameTime: unknow maptype " + p_mapType);
			}
		}

		internal void UpdateTime(Int32 p_minutes, ETimeChangeReason p_reason)
		{
			m_lastTime = m_time;
			m_time += new MMTime(p_minutes);
			if (DayState != LastDayState)
			{
				OnDayStateChanged(p_reason);
			}
			MMTime p_t = new MMTime(0, 8, m_time.Days);
			if (m_time >= p_t && m_lastTime < p_t)
			{
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				foreach (GridSlot gridSlot in grid.SlotIterator())
				{
					foreach (InteractiveObject interactiveObject in gridSlot.GetInteractiveObjectIterator())
					{
						RechargingObject rechargingObject = interactiveObject as RechargingObject;
						if (rechargingObject != null)
						{
							rechargingObject.OnCheckRecharge();
						}
					}
				}
			}
			if (p_reason == ETimeChangeReason.Movement && m_time.Hours == 0)
			{
				if ((p_minutes == 1 && m_time.Minutes == 1) || (p_minutes > 1 && m_time.Minutes > 0 && m_time.Minutes <= p_minutes))
				{
					m_calendar.NextDay();
				}
			}
			else if (p_reason == ETimeChangeReason.Resting)
			{
				m_calendar.SetDateByPassedTime(m_time.Days);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GAMETIME_TIME_CHANGED, new GameTimeEventArgs(DayState, LastDayState, p_reason));
		}

		internal void UpdateIncreaseTurn()
		{
			m_turn++;
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GAMETIME_TURN_CHANGED, EventArgs.Empty);
		}

		public void Load(SaveGameData p_data)
		{
			m_time.Load(p_data);
			m_calendar.SetDateByPassedTime(m_time.Days);
			UpdateTime(0, ETimeChangeReason.None);
		}

		public void Save(SaveGameData p_data)
		{
			m_time.Save(p_data);
		}

		public static EDayState GetDayState(Int32 p_hours)
		{
			if (p_hours >= ConfigManager.Instance.Game.NightStartHours)
			{
				return EDayState.NIGHT;
			}
			if (p_hours >= ConfigManager.Instance.Game.DuskStartHours)
			{
				return EDayState.DUSK;
			}
			if (p_hours >= ConfigManager.Instance.Game.DayStartHours)
			{
				return EDayState.DAY;
			}
			if (p_hours >= ConfigManager.Instance.Game.DawnStartHours)
			{
				return EDayState.DAWN;
			}
			return EDayState.NIGHT;
		}

		private void OnDayStateChanged(ETimeChangeReason p_reason)
		{
			if (DayState == EDayState.DAWN || DayState == EDayState.NIGHT)
			{
				GameTimeEntryEventArgs p_args = new GameTimeEntryEventArgs(DayState);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GAMETIME_DAYSTATE_CHANGED, new GameTimeEventArgs(DayState, LastDayState, p_reason));
			LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.DAY_NIGHT);
		}
	}
}
