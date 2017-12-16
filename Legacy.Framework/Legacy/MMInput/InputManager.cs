using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Legacy.MMInput
{
	[AddComponentMenu("MM Legacy/MMInput/InputManager")]
	public class InputManager : MonoBehaviour
	{
		private static Boolean s_HotkeyMapDirty;

		private static HotkeyData[] s_HotkeyMap;

		private static HotkeyData[] s_HotkeyMapSorted;

		private static List<HotkeyData> s_ActiveHotkeys;

		private static Boolean s_LastAnyKey;

		static InputManager()
		{
			EHotkeyType[] array = (EHotkeyType[])Enum.GetValues(typeof(EHotkeyType));
			s_HotkeyMap = new HotkeyData[array.Length];
			s_HotkeyMapSorted = new HotkeyData[array.Length];
			s_ActiveHotkeys = new List<HotkeyData>(array.Length);
		}

		public static Boolean GetHotkey(EHotkeyType type)
		{
			if (type >= EHotkeyType.OPEN_CLOSE_MENU && type < (EHotkeyType)s_HotkeyMap.Length)
			{
				return s_HotkeyMap[(Int32)type].m_Action.IsPressed;
			}
			throw new ArgumentOutOfRangeException("type");
		}

		public static Boolean GetHotkeyDown(EHotkeyType type)
		{
			if (type >= EHotkeyType.OPEN_CLOSE_MENU && type < (EHotkeyType)s_HotkeyMap.Length)
			{
				return s_HotkeyMap[(Int32)type].m_Action.IsDown;
			}
			throw new ArgumentOutOfRangeException("type");
		}

		public static Boolean GetHotkeyUp(EHotkeyType type)
		{
			if (type >= EHotkeyType.OPEN_CLOSE_MENU && type < (EHotkeyType)s_HotkeyMap.Length)
			{
				return s_HotkeyMap[(Int32)type].m_Action.IsUp;
			}
			throw new ArgumentOutOfRangeException("type");
		}

		public static Hotkey GetHotkeyData(EHotkeyType type)
		{
			if (type >= EHotkeyType.OPEN_CLOSE_MENU && type < (EHotkeyType)s_HotkeyMap.Length)
			{
				return s_HotkeyMap[(Int32)type].m_Action;
			}
			throw new ArgumentOutOfRangeException("type");
		}

		public static void SetHotkeyData(Hotkey hotkey)
		{
			if (hotkey.Type >= EHotkeyType.OPEN_CLOSE_MENU && hotkey.Type < (EHotkeyType)s_HotkeyMap.Length)
			{
				s_HotkeyMap[(Int32)hotkey.Type].m_Action = hotkey;
				s_HotkeyMapDirty = true;
				return;
			}
			throw new ArgumentOutOfRangeException("hotkey", "hotkey.Type");
		}

		public static void RegisterHotkeyEvent(EHotkeyType type, EventHandler<HotkeyEventArgs> callback)
		{
			if (type >= EHotkeyType.OPEN_CLOSE_MENU && type < (EHotkeyType)s_HotkeyMap.Length)
			{
				HotkeyData[] array = s_HotkeyMap;
				array[(Int32)type].m_EventHandler = (EventHandler<HotkeyEventArgs>)Delegate.Combine(array[(Int32)type].m_EventHandler, callback);
				s_HotkeyMapDirty = true;
				return;
			}
			throw new ArgumentOutOfRangeException("type");
		}

		public static void UnregisterHotkeyEvent(EHotkeyType type, EventHandler<HotkeyEventArgs> callback)
		{
			if (type >= EHotkeyType.OPEN_CLOSE_MENU && type < (EHotkeyType)s_HotkeyMap.Length)
			{
				HotkeyData[] array = s_HotkeyMap;
				array[(Int32)type].m_EventHandler = (EventHandler<HotkeyEventArgs>)Delegate.Remove(array[(Int32)type].m_EventHandler, callback);
				s_HotkeyMapDirty = true;
				return;
			}
			throw new ArgumentOutOfRangeException("type");
		}

		private void Update()
		{
			Boolean anyKeyDown = Input.anyKeyDown;
			Boolean anyKey = Input.anyKey;
			DirtyCheck();
			if (anyKeyDown && anyKey)
			{
				s_ActiveHotkeys.Clear();
				for (Int32 i = 0; i < s_HotkeyMapSorted.Length; i++)
				{
					HotkeyData hotkeyData = s_HotkeyMapSorted[i];
					if (hotkeyData.m_EventHandler != null && hotkeyData.m_Action.IsDown && AddToList(ref hotkeyData))
					{
						break;
					}
				}
				HandleEvents(true);
			}
			if ((s_LastAnyKey && anyKey) || (s_LastAnyKey && anyKey != s_LastAnyKey))
			{
				s_ActiveHotkeys.Clear();
				for (Int32 j = 0; j < s_HotkeyMapSorted.Length; j++)
				{
					HotkeyData hotkeyData2 = s_HotkeyMapSorted[j];
					if (hotkeyData2.m_EventHandler != null && hotkeyData2.m_Action.IsUp && AddToList(ref hotkeyData2))
					{
						break;
					}
				}
				HandleEvents(false);
			}
			s_LastAnyKey = anyKey;
		}

		private static void DirtyCheck()
		{
			if (s_HotkeyMapDirty)
			{
				s_HotkeyMapDirty = false;
				Array.Clear(s_HotkeyMapSorted, 0, s_HotkeyMapSorted.Length);
				s_HotkeyMap.CopyTo(s_HotkeyMapSorted, 0);
				Array.Sort<HotkeyData>(s_HotkeyMapSorted);
			}
		}

		private static void HandleEvents(Boolean keyDown)
		{
			for (Int32 i = 0; i < s_ActiveHotkeys.Count; i++)
			{
				HotkeyData hotkeyData = s_ActiveHotkeys[i];
				try
				{
					if (hotkeyData.m_EventHandler != null)
					{
						hotkeyData.m_EventHandler(null, new HotkeyEventArgs(hotkeyData.m_Action.Type, keyDown, hotkeyData.m_Action.Key1));
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(String.Concat(new Object[]
					{
						"Exception handle ",
						hotkeyData.m_Action.Type,
						", KeyDown: ",
						keyDown,
						", Key:",
						hotkeyData.m_Action.Key1,
						"\n",
						ex
					}));
				}
			}
		}

		private static Boolean AddToList(ref HotkeyData data)
		{
			if (data.m_Action.IsMouse)
			{
				s_ActiveHotkeys.Add(data);
				return false;
			}
			if (data.m_Action.KeyCount != 1)
			{
				s_ActiveHotkeys.Add(data);
				return true;
			}
			s_ActiveHotkeys.Add(data);
			return false;
		}

		private struct HotkeyData : IComparable<HotkeyData>
		{
			public Hotkey m_Action;

			public EventHandler<HotkeyEventArgs> m_EventHandler;

			public Int32 CompareTo(HotkeyData other)
			{
				if (m_Action.IsMouse || other.m_Action.IsMouse)
				{
					return other.m_Action.IsMouse.CompareTo(m_Action.IsMouse);
				}
				return other.m_Action.KeyCount.CompareTo(m_Action.KeyCount);
			}
		}
	}
}
