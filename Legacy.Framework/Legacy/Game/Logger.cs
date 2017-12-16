using System;
using Legacy.Utilities;
using UnityEngine;

namespace Legacy.Game
{
	public class Logger : MonoBehaviour
	{
		private void Update()
		{
			if (LegacyLogger.NormalLog.Count > 0 || LegacyLogger.ErrorLog.Count > 0)
			{
				foreach (String message in LegacyLogger.NormalLog)
				{
					Debug.Log(message);
				}
				foreach (String message2 in LegacyLogger.ErrorLog)
				{
					Debug.LogError(message2);
				}
				LegacyLogger.Clear();
			}
		}
	}
}
