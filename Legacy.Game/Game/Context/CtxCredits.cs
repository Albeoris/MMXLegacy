using System;
using Legacy.Audio;
using UnityEngine;

namespace Legacy.Game.Context
{
	[AddComponentMenu("MM Legacy/Contexts/CtxCredits")]
	public class CtxCredits : BaseContext
	{
		public CtxCredits() : base(EContext.CreditsScreen)
		{
		}

		protected override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			MusicController.Instance.SetMode(MusicController.Mode.CREDITS);
		}
	}
}
