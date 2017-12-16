using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Legacy.Utilities
{
	public static class LegacyLogger
	{
		private const Int32 MAX_LOG_COUNT = 200;

		private static List<String> m_NormalList = new List<String>(200);

		private static List<String> m_ErrorList;

		public static readonly IList<String> NormalLog = m_NormalList.AsReadOnly();

		public static readonly IList<String> ErrorLog;

		static LegacyLogger()
		{
			m_ErrorList = new List<String>(200);
			ErrorLog = m_ErrorList.AsReadOnly();
		}

		public static void Clear()
		{
			m_NormalList.Clear();
			m_ErrorList.Clear();
		}

		internal static void Log(Object p_message)
		{
			Log(m_NormalList, p_message, true, 2);
		}

		internal static void Log(Object p_message, Boolean p_useStackTrace)
		{
			Log(m_NormalList, p_message, p_useStackTrace, 2);
		}

		internal static void LogError(Object p_message)
		{
			Log(m_ErrorList, p_message, true, 2);
		}

		internal static void LogError(Object p_message, Boolean p_useStackTrace)
		{
			Log(m_ErrorList, p_message, p_useStackTrace, 2);
		}

		private static void Log(List<String> p_logList, Object p_message, Boolean p_useStackTrace, Int32 p_skipCallStack)
		{
			if (p_logList.Count >= 200)
			{
				p_logList.RemoveAt(0);
			}
			String text;
			if (p_message == null)
			{
				text = "NULL\n";
			}
			else
			{
				text = p_message.ToString();
			}
			if (p_useStackTrace)
			{
				StackTrace stackTrace = new StackTrace(p_skipCallStack, true);
				if (p_message == null)
				{
					text += stackTrace.ToString();
				}
				else
				{
					text = text + "\n" + stackTrace.ToString();
				}
			}
			p_logList.Add(text);
		}
	}
}
