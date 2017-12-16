using System;

public static class SystemTime
{
	private static Double _timeAtLaunch = time;

	public static Double time
	{
		get
		{
			Int64 ticks = DateTime.Now.Ticks;
			return ticks * 1E-07;
		}
	}

	public static Double timeSinceLaunch => time - _timeAtLaunch;
}
