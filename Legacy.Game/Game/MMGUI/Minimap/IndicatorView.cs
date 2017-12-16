using System;
using UnityEngine;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/IndicatorView")]
	public class IndicatorView : MonoBehaviour
	{
		[SerializeField]
		private AnimationCurve m_RingCurveForm = AnimationCurve.Linear(0f, 1f, 360f, 1f);
	}
}
