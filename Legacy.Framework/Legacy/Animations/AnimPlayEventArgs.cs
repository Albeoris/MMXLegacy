using System;

namespace Legacy.Animations
{
	public class AnimPlayEventArgs : EventArgs
	{
		public AnimPlayEventArgs(EAnimType p_type, String p_clipName)
		{
			Type = p_type;
			ClipName = p_clipName;
		}

		public EAnimType Type { get; private set; }

		public String ClipName { get; private set; }
	}
}
