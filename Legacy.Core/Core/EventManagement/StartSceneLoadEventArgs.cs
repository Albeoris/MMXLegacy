using System;

namespace Legacy.Core.EventManagement
{
	public class StartSceneLoadEventArgs : EventArgs
	{
		public StartSceneLoadEventArgs(String sceneName)
		{
			SceneName = sceneName;
		}

		public String SceneName { get; private set; }
	}
}
