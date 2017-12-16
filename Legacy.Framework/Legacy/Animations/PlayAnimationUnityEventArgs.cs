using System;

namespace Legacy.Animations
{
	public class PlayAnimationUnityEventArgs : UnityEventArgs
	{
		public PlayAnimationUnityEventArgs(Object p_sender, EAnimType p_type, String p_clipName) : base(p_sender)
		{
			Type = p_type;
			ClipName = p_clipName;
		}

		public EAnimType Type { get; private set; }

		public String ClipName { get; private set; }
	}
}
