using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Light Sequence")]
	public class LightSequenceScript : MonoBehaviour
	{
		public Single ChangeTime = 1f;

		public Single CrossFadeTime = 1f;

		public Single LightIntensity = 1f;

		public Light[] LightArray;

		private Int32 mCurrentLight;

		private Single mChangeTimer;

		private Single mCrossFadeTimer;

		private Boolean mFadeMode;

		private void Start()
		{
			if (LightArray == null || LightArray.Length < 2)
			{
				enabled = false;
			}
			mChangeTimer = ChangeTime;
			for (Int32 i = 0; i < LightArray.Length; i++)
			{
				LightArray[i].enabled = false;
			}
			LightArray[0].enabled = true;
		}

		private void Update()
		{
			if (!mFadeMode)
			{
				mChangeTimer -= Time.deltaTime;
				if (mChangeTimer <= 0f)
				{
					mFadeMode = true;
					mCrossFadeTimer = CrossFadeTime;
				}
			}
			else
			{
				mCrossFadeTimer -= Time.deltaTime;
				Single num = (CrossFadeTime > 0f) ? (mCrossFadeTimer / CrossFadeTime) : 0f;
				if (LightArray[mCurrentLight] != null)
				{
					LightArray[mCurrentLight].intensity = LightIntensity * num;
					if (LightArray[mCurrentLight].intensity <= 0f)
					{
						LightArray[mCurrentLight].enabled = false;
					}
				}
				if (LightArray[GetNextLightID()] != null)
				{
					LightArray[GetNextLightID()].intensity = LightIntensity * (1f - num);
					if (LightArray[GetNextLightID()].intensity > 0f)
					{
						LightArray[GetNextLightID()].enabled = true;
					}
				}
				if (mCrossFadeTimer <= 0f)
				{
					mFadeMode = false;
					mCurrentLight = GetNextLightID();
					mChangeTimer = ChangeTime;
				}
			}
		}

		private Int32 GetNextLightID()
		{
			if (mCurrentLight + 1 >= LightArray.Length)
			{
				return 0;
			}
			return mCurrentLight + 1;
		}
	}
}
