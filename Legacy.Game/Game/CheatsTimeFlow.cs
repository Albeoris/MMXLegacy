using System;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/Cheats/CheatsTimeFlow")]
	public class CheatsTimeFlow : MonoBehaviour
	{
		private const Single TIME_MIN_SPEED = 0.2f;

		private const Single TIME_MAX_SPEED = 3f;

		[SerializeField]
		private UISlider m_slider;

		public void OnTimeFlowChange(Single p_val)
		{
			Single timeScale;
			if (p_val < 0.5f)
			{
				timeScale = 0.2f + 0.8f * p_val * 2f;
			}
			else if (p_val > 0.5f)
			{
				timeScale = 1f + 2f * (p_val - 0.5f) * 2f;
			}
			else
			{
				timeScale = 1f;
			}
			Time.timeScale = timeScale;
		}

		public void OnPlusButtonClick()
		{
			if (m_slider != null && m_slider.sliderValue < 1f)
			{
				m_slider.sliderValue += 1f / (m_slider.numberOfSteps - 1);
			}
		}

		public void OnMinusButtonClick()
		{
			if (m_slider != null && m_slider.sliderValue > 0f)
			{
				m_slider.sliderValue -= 1f / (m_slider.numberOfSteps - 1);
			}
		}
	}
}
