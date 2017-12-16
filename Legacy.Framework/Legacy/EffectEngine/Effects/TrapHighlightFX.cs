using System;
using System.Collections;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class TrapHighlightFX : MonoBehaviour
	{
		[SerializeField]
		private HighlightFX[] m_FX;

		public void ResetToInvisible()
		{
			foreach (HighlightFX highlightFX in m_FX)
			{
				highlightFX.ResetToInvisible();
			}
		}

		public void FadeIn(Single p_FadeOutAfterDelay)
		{
			FadeIn();
			StopAllCoroutines();
			StartCoroutine(FadeOut(p_FadeOutAfterDelay));
		}

		public void FadeIn()
		{
			foreach (HighlightFX highlightFX in m_FX)
			{
				highlightFX.FadeIn();
			}
			StopAllCoroutines();
		}

		private IEnumerator FadeOut(Single p_Delay)
		{
			yield return new WaitForSeconds(p_Delay);
			FadeOut();
			yield break;
		}

		public void FadeOut()
		{
			foreach (HighlightFX highlightFX in m_FX)
			{
				highlightFX.FadeOut();
			}
		}
	}
}
