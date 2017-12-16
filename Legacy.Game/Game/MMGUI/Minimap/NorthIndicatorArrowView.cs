using System;
using UnityEngine;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/North indicator arrow view")]
	public class NorthIndicatorArrowView : IndicatorArrowView
	{
		private void Update()
		{
			transform.localRotation = SymbolView.s_InverseRotation;
		}
	}
}
