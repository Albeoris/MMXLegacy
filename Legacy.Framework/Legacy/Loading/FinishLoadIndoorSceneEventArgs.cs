using System;
using Legacy.NpcInteraction;

namespace Legacy.Loading
{
	internal class FinishLoadIndoorSceneEventArgs : EventArgs
	{
		public FinishLoadIndoorSceneEventArgs(IndoorSceneRoot root)
		{
			Root = root;
		}

		public IndoorSceneRoot Root { get; private set; }
	}
}
