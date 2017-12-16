using System;
using Legacy.Audio;
using UnityEngine;

namespace Legacy.Game.Context
{
	[AddComponentMenu("MM Legacy/Contexts/CtxIntro")]
	public class CtxIntro : BaseContext
	{
		public CtxIntro() : base(EContext.Intro)
		{
		}

		protected override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			MusicController.Instance.SetMode(MusicController.Mode.MUTE);
		}
	}
}
