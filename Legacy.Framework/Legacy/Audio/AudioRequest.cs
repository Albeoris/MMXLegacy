using System;

namespace Legacy.Audio
{
	public class AudioRequest
	{
		public AudioRequest(AudioManager manager, String categoryName, Int32 priority, Boolean isVolumeSetManually)
		{
			Manager = manager;
			CategoryName = categoryName;
			Priority = priority;
			IsVolumeSetManually = isVolumeSetManually;
		}

		public AudioManager Manager { get; private set; }

		public EAudioRequestState State { get; private set; }

		public AudioController Controller { get; private set; }

		public String CategoryName { get; private set; }

		public Int32 Priority { get; private set; }

		public Boolean IsVolumeSetManually { get; private set; }

		public Object Tag { get; set; }

		public Boolean IsLoading => State == EAudioRequestState.Load;

	    public Boolean IsDone => State >= EAudioRequestState.Load;

	    public void AbortLoad()
		{
			if (State == EAudioRequestState.Load)
			{
				State = EAudioRequestState.Abort;
			}
		}

		public void _UpdateReady(AudioController controller)
		{
			if (State != EAudioRequestState.Abort)
			{
				Controller = controller;
				State = EAudioRequestState.Ready;
			}
		}
	}
}
