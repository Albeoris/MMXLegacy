using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	[AddComponentMenu("MM Legacy/GUI Misc/GUITextureMonochrome")]
	public class GUITextureMonochrome : MonoBehaviour
	{
		[SerializeField]
		private Single m_monochrome = 1f;

		[SerializeField]
		private UITexture m_texture;

		public Single MonochromeValue => m_monochrome;

	    private void Update()
		{
			Single alpha = m_monochrome * 0.5f;
			m_texture.alpha = alpha;
		}

		internal void SetMonochromeValue(Single p_value)
		{
			m_monochrome = p_value;
		}
	}
}
