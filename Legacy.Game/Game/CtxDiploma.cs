using System;
using Legacy.Audio;
using Legacy.Game.Context;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/Contexts/CtxDiploma")]
	public class CtxDiploma : BaseContext
	{
		public CtxDiploma() : base(EContext.Diploma)
		{
		}

		protected override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			MusicController.Instance.SetMode(MusicController.Mode.ENDING_SLIDES);
		}
	}
}
