using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class MagicDissolveObjectFX : MonoBehaviour
	{
		public Single DissolveTime = 3f;

		public AnimationCurve DissCurve;

		public Renderer TargetObj;

		private Single mTimer;

		private void Start()
		{
			OnEnable();
		}

		private void Update()
		{
			mTimer += Time.deltaTime / DissolveTime;
			TargetObj.material.SetFloat("_DissolveFactor", 1f - DissCurve.Evaluate(mTimer));
			if (mTimer >= 1f)
			{
				gameObject.SetActive(false);
			}
		}

		private void OnEnable()
		{
			mTimer = 0f;
			if (TargetObj == null || TargetObj.material == null)
			{
				Debug.Log("Wrong Dissolve Material or no Renderer Invalid");
				gameObject.SetActive(false);
			}
		}
	}
}
