using System;
using Legacy.Audio;
using UnityEngine;

namespace Legacy.Game.Context
{
	[AddComponentMenu("MM Legacy/Contexts/CtxMainmenu")]
	public class CtxMainmenu : BaseContext
	{
		public CtxMainmenu() : base(EContext.Mainmenu)
		{
		}

		protected override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			MusicController.Instance.SetMode(MusicController.Mode.MENU);
		}
	}
}
