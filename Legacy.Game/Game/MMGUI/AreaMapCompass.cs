using System;
using Legacy.Game.MMGUI.Minimap;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/AreaMap Compass")]
	public class AreaMapCompass : MonoBehaviour
	{
		private void Update()
		{
			transform.localRotation = Quaternion.Inverse(SymbolView.s_InverseRotation);
		}
	}
}
