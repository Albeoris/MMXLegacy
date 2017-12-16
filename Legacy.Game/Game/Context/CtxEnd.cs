using System;
using Legacy.Audio;
using UnityEngine;

namespace Legacy.Game.Context
{
	[AddComponentMenu("MM Legacy/Contexts/CtxEnd")]
	public class CtxEnd : BaseContext
	{
		public CtxEnd() : base(EContext.End)
		{
		}

		protected override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			MusicController.Instance.SetMode(MusicController.Mode.ENDING_SLIDES);
		}
	}
}
