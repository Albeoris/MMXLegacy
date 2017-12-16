using System;
using Legacy.Core.Entities;
using UnityEngine;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/Stairs Symbol View")]
	public class StairsSymbolView : SimpleSymbolView
	{
		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (MyController != null)
			{
			}
			throw new NotImplementedException();
		}
	}
}
