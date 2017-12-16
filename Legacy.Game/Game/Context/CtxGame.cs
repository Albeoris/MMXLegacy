using System;
using Legacy.Audio;
using Legacy.Core.Api;
using UnityEngine;

namespace Legacy.Game.Context
{
	[AddComponentMenu("MM Legacy/Contexts/CtxGame")]
	public class CtxGame : BaseContext
	{
		public CtxGame() : base(EContext.Game)
		{
		}

		public override void OnEnableContext()
		{
			AudioHelper.MuteGUI();
			base.OnEnableContext();
		}

		protected override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			LegacyLogic.Instance.StartGame();
		}
	}
}
