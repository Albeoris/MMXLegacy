using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class MechanicalDissolveObjectFX : MonoBehaviour
	{
		public Single DissolveTime = 3f;

		public AnimationCurve MoveCurveX;

		public AnimationCurve MoveCurveY;

		public AnimationCurve MoveCurveZ;

		public GameObject TargetObj;

		public Vector3 StartPosition = Vector3.zero;

		public Vector3 TargetPosition = Vector3.zero;

		private Single mTimer;

		private void Start()
		{
			OnEnable();
		}

		private void Update()
		{
			mTimer += Time.deltaTime / DissolveTime;
			TargetObj.transform.localPosition = new Vector3(Mathf.Lerp(StartPosition.x, TargetPosition.x, MoveCurveX.Evaluate(mTimer)), Mathf.Lerp(StartPosition.y, TargetPosition.y, MoveCurveY.Evaluate(mTimer)), Mathf.Lerp(StartPosition.z, TargetPosition.z, MoveCurveZ.Evaluate(mTimer)));
			if (mTimer >= 1f)
			{
				gameObject.SetActive(false);
			}
		}

		private void OnEnable()
		{
			mTimer = 0f;
			if (TargetObj == null)
			{
				gameObject.SetActive(false);
			}
		}
	}
}
