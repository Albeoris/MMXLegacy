using System;
using Legacy.Game.Context;
using UnityEngine;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/EndGUILogic")]
	public class EndGUILogic : MonoBehaviour
	{
		public void OnStartButtonClick()
		{
			ContextManager.ChangeContext(EContext.Mainmenu);
		}
	}
}
