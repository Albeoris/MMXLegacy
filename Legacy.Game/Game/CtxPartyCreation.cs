using System;
using Legacy.Audio;
using Legacy.Game.Context;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/Contexts/CtxPartyCreation")]
	public class CtxPartyCreation : BaseContext
	{
		public CtxPartyCreation() : base(EContext.PartyCreation)
		{
		}

		protected override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			MusicController.Instance.SetMode(MusicController.Mode.MENU);
		}
	}
}
