using System;
using Legacy.Audio;

namespace Legacy.Game.Context
{
	public class CtxDevLogos : BaseContext
	{
		public CtxDevLogos() : base(EContext.DevLogos)
		{
		}

		protected override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			MusicController.Instance.SetMode(MusicController.Mode.MUTE);
		}
	}
}
