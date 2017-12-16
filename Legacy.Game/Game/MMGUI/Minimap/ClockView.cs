using System;
using System.IO;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/Clock View")]
	public class ClockView : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_sprite;

		private Boolean m_isHovered;

		private Boolean m_specialTooltip;

		private void Start()
		{
			OnGameTmeChanged(null, null);
			OnMapChanged(null, new ChangeMapEventArgs(LegacyLogic.Instance.MapLoader.Grid.Name));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAMETIME_TIME_CHANGED, new EventHandler(OnGameTmeChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnDaystateChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHANGE_MAP, new EventHandler(OnMapChanged));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAMETIME_TIME_CHANGED, new EventHandler(OnGameTmeChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnDaystateChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHANGE_MAP, new EventHandler(OnMapChanged));
		}

		private void OnGameTmeChanged(Object sender, EventArgs e)
		{
			Single dayNightBlend = GetDayNightBlend();
			transform.localEulerAngles = new Vector3(0f, 0f, 180f * dayNightBlend);
			if (m_isHovered)
			{
				OnTooltip(m_isHovered);
			}
		}

		private void OnDaystateChanged(Object p_sender, EventArgs p_args)
		{
			if (LegacyLogic.Instance.GameTime.DayState == EDayState.DAWN)
			{
				AudioController.Play("BeginningOfDayJingle");
			}
			else if (LegacyLogic.Instance.GameTime.DayState == EDayState.NIGHT)
			{
				AudioController.Play("BeginningOfNightJingle");
			}
		}

		private void OnMapChanged(Object p_sender, EventArgs p_args)
		{
			String level = ((ChangeMapEventArgs)p_args).Level;
			String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(level);
			if (String.Equals(fileNameWithoutExtension, "exclusive_dungeon_1", StringComparison.InvariantCulture))
			{
				m_specialTooltip = true;
				m_sprite.spriteName = "ICO_daytime_dreamshard";
			}
			else
			{
				m_specialTooltip = false;
				m_sprite.spriteName = "ICO_daytime";
			}
		}

		private Single GetDayNightBlend()
		{
			MMTime time = LegacyLogic.Instance.GameTime.Time;
			Int32 num = ConfigManager.Instance.Game.DawnStartHours * 60;
			Int32 num2 = ConfigManager.Instance.Game.NightStartHours * 60;
			Int32 num3 = time.Hours * 60 + time.Minutes;
			if (num3 >= num && num3 < num2)
			{
				Single num4 = num2 - num;
				return (num3 - num) / num4 - 0.5f;
			}
			Single num5 = 1440 - num2 + num;
			Int32 num6;
			if (num3 >= num2)
			{
				num6 = num3 - num2;
			}
			else
			{
				num6 = 1440 - num2 + num3;
			}
			return num6 / num5 + 0.5f;
		}

		public void OnTooltip(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (p_isOver)
			{
				Vector3 position = transform.position;
				Vector3 p_offset = transform.localScale * 0.5f;
				GameTime gameTime = LegacyLogic.Instance.GameTime;
				String text2;
				String text3;
				String text4;
				String p_captionText;
				if (m_specialTooltip)
				{
					String text = LocaManager.GetText("CALENDAR_WEEKDAY_2");
					text2 = LocaManager.GetText("CALENDAR_DAY_NUMBER_EXTENSION_11");
					text3 = LocaManager.GetText("CALENDAR_MONTH_2");
					text4 = "563";
					p_captionText = text;
				}
				else
				{
					String text = LocaManager.GetText("CALENDAR_WEEKDAY_" + (Int32)(gameTime.Calendar.WeekDay + 1));
					text2 = LocaManager.GetText("CALENDAR_DAY_NUMBER_EXTENSION_" + gameTime.Calendar.Day);
					text3 = LocaManager.GetText("CALENDAR_MONTH_" + (Int32)(gameTime.Calendar.Month + 1));
					text4 = gameTime.Calendar.Year.ToString();
					String arg = gameTime.Time.Hours.ToString("00");
					String arg2 = gameTime.Time.Minutes.ToString("00");
					p_captionText = String.Format("{0}:{1}, {2}", arg, arg2, text);
				}
				String text5 = LocaManager.GetText("CALENDAR_YEAR_SUFFIX");
				String p_tooltipText = String.Format("{0} {1}\n{2} {3}", new Object[]
				{
					text2,
					text3,
					text4,
					text5
				});
				TooltipManager.Instance.Show(this, p_captionText, p_tooltipText, TextTooltip.ESize.BIG, position, p_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
