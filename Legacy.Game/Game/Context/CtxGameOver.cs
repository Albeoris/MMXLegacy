using System;
using Legacy.Audio;
using UnityEngine;

namespace Legacy.Game.Context
{
	[AddComponentMenu("MM Legacy/Contexts/CtxGameOver")]
	public class CtxGameOver : BaseContext
	{
		public CtxGameOver() : base(EContext.GameOver)
		{
		}

		protected override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			MusicController.Instance.SetMode(MusicController.Mode.GAME_OVER);
		}
	}
}
