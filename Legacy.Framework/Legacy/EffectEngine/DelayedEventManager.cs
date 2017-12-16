using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.EffectEngine
{
	public static class DelayedEventManager
	{
		private static readonly WaitForEndOfFrame s_EndOfFrame = new WaitForEndOfFrame();

		private static List<EventHandler>[][] s_eventMap;

		private static DelayedEventManagerWorker s_workerGO;

		private static Dictionary<EEventType, Single> s_defaultFixedDelays = new Dictionary<EEventType, Single>();

		private static Dictionary<EEventType, Single> s_fixedDelays = new Dictionary<EEventType, Single>();

		static DelayedEventManager()
		{
			Int32 length = Enum.GetValues(typeof(EDelayType)).Length;
			EEventType[] array = (EEventType[])Enum.GetValues(typeof(EEventType));
			s_eventMap = new List<EventHandler>[length][];
			for (Int32 i = 0; i < s_eventMap.Length; i++)
			{
				s_eventMap[i] = new List<EventHandler>[array.Length];
				for (Int32 j = 0; j < s_eventMap[i].Length; j++)
				{
					s_eventMap[i][j] = new List<EventHandler>();
				}
			}
			s_defaultFixedDelays[EEventType.PARTY_GET_LOOT] = 2f;
			s_defaultFixedDelays[EEventType.CHARACTER_BARK] = 3f;
			s_defaultFixedDelays[EEventType.CHARACTER_XP_GAIN] = 0.85f;
			foreach (KeyValuePair<EEventType, Single> keyValuePair in s_defaultFixedDelays)
			{
				s_fixedDelays.Add(keyValuePair.Key, keyValuePair.Value);
			}
			EEventType[] array2 = array;
			for (Int32 k = 0; k < array2.Length; k++)
			{
				EEventType eeventType = array2[k];
				EEventType eventTypeCopy = eeventType;
				LegacyLogic.Instance.EventManager.RegisterEvent(eeventType, delegate(Object pSender, EventArgs pArgs)
				{
					OnEventManagerEvent(pSender, eventTypeCopy, pArgs);
				});
			}
		}

		private static DelayedEventManagerWorker Worker
		{
			get
			{
				if (s_workerGO == null)
				{
					s_workerGO = new GameObject("DelayedEventManager Worker").AddComponent<DelayedEventManagerWorker>();
					UnityEngine.Object.DontDestroyOnLoad(s_workerGO);
				}
				return s_workerGO;
			}
		}

		public static void RegisterEvent(EDelayType p_delayType, EEventType p_eventType, EventHandler p_delegate)
		{
			if (p_delegate != null)
			{
				s_eventMap[(Int32)p_delayType][(Int32)p_eventType].Add(p_delegate);
			}
		}

		public static void UnregisterEvent(EDelayType p_delayType, EEventType p_eventType, EventHandler p_delegate)
		{
			if (p_delegate != null)
			{
				s_eventMap[(Int32)p_delayType][(Int32)p_eventType].Remove(p_delegate);
			}
		}

		public static void InvokeEvent(EDelayType p_delayType, EEventType p_eventType, Object p_sender, EventArgs p_eventArgs)
		{
			List<EventHandler> list = s_eventMap[(Int32)p_delayType][(Int32)p_eventType];
			for (Int32 i = 0; i < list.Count; i++)
			{
				try
				{
					list[i](p_sender, p_eventArgs);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}

		public static Single GetFixedDelay(EEventType p_eventType)
		{
			Single result;
			if (s_fixedDelays.TryGetValue(p_eventType, out result))
			{
				return result;
			}
			return -1f;
		}

		public static void SetFixedDelay(EEventType p_eventType, Single p_delay)
		{
			s_fixedDelays[p_eventType] = p_delay;
		}

		public static void ResetFixedDelayToDefault(EEventType p_eventType)
		{
			Single value;
			if (s_defaultFixedDelays.TryGetValue(p_eventType, out value))
			{
				s_fixedDelays[p_eventType] = value;
			}
			else
			{
				s_fixedDelays.Remove(p_eventType);
			}
		}

		private static void OnEventManagerEvent(Object p_sender, EEventType p_eventType, EventArgs p_args)
		{
			Worker.StartCoroutine_Auto(InvokeOnFrameEnd(p_sender, p_eventType, p_args));
			Worker.StartCoroutine_Auto(InvokeOnFixedDelay(p_sender, p_eventType, p_args));
		}

		private static IEnumerator InvokeOnFrameEnd(Object p_sender, EEventType p_eventType, EventArgs p_eventArgs)
		{
			yield return s_EndOfFrame;
			InvokeEvent(EDelayType.ON_FRAME_END, p_eventType, p_sender, p_eventArgs);
			yield break;
		}

		private static IEnumerator InvokeOnFixedDelay(Object p_sender, EEventType p_eventType, EventArgs p_eventArgs)
		{
			Single fixedDelay = GetFixedDelay(p_eventType);
			if (fixedDelay > 0f)
			{
				yield return new WaitForSeconds(fixedDelay);
			}
			else
			{
				yield return s_EndOfFrame;
			}
			InvokeEvent(EDelayType.ON_FIXED_DELAY, p_eventType, p_sender, p_eventArgs);
			yield break;
		}
	}
}
