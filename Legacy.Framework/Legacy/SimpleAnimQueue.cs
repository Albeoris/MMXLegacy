using System;
using UnityEngine;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Animation/Simple Idle Anim Queue")]
	[RequireComponent(typeof(Animation))]
	public class SimpleAnimQueue : MonoBehaviour
	{
		public Single newAnimTimer = 5f;

		public Single newAnimTimerDelta = 2f;

		public AnimEntryConfig BaseAnim;

		public AnimEntryConfig[] AnimArray = Arrays<AnimEntryConfig>.Empty;

		private Single m_randAnimTimer;

		private Int32 m_currentAnim = -1;

		private String baseName;

		private AnimEntryConfig lastConfig;

		private AnimationState lastState;

		private AnimEntryConfig CurrentAnim
		{
			get
			{
				if (m_currentAnim >= 0 && m_currentAnim < AnimArray.Length)
				{
					return AnimArray[m_currentAnim];
				}
				return BaseAnim;
			}
		}

		private Single GetNewAnimTimer()
		{
			return Random.Range(newAnimTimer - newAnimTimerDelta, newAnimTimer + newAnimTimerDelta);
		}

		private Int32 GetRandomAnimation()
		{
			Single num = 0f;
			for (Int32 i = 0; i < AnimArray.Length; i++)
			{
				num += AnimArray[i].Chance;
			}
			Single num2 = Random.Range(0f, Mathf.Max(num, 1f));
			num = 0f;
			for (Int32 j = 0; j < AnimArray.Length; j++)
			{
				num += AnimArray[j].Chance;
				if (num2 <= num)
				{
					return j;
				}
			}
			return Random.Range(0, AnimArray.Length);
		}

		private void StartAnimation(AnimEntryConfig p_newAnim)
		{
			if (p_newAnim.FadeMode == FadeModeType.CrossFade)
			{
				animation.CrossFade(p_newAnim.Clip.name, p_newAnim.fadeInTime);
			}
			else
			{
				animation.Play(p_newAnim.Clip.name, PlayMode.StopAll);
			}
		}

		private void Start()
		{
			if (BaseAnim == null || BaseAnim.Clip == null)
			{
				enabled = false;
				Debug.LogError("Config error!\nBasBaseAnim == null || BaseAnim.Clip == null", this);
				return;
			}
			if (animation.GetClip(BaseAnim.Clip.name) == null)
			{
				animation.AddClip(BaseAnim.Clip, BaseAnim.Clip.name);
			}
			foreach (AnimEntryConfig animEntryConfig in AnimArray)
			{
				if (animation.GetClip(animEntryConfig.Clip.name) == null)
				{
					animation.AddClip(animEntryConfig.Clip, animEntryConfig.Clip.name);
				}
			}
			animation.Play(BaseAnim.Clip.name, PlayMode.StopAll);
			m_randAnimTimer = GetNewAnimTimer();
		}

		private void Update()
		{
			Animation animation = this.animation;
			if (baseName == null)
			{
				baseName = BaseAnim.Clip.name;
			}
			if (animation.isPlaying)
			{
				if (animation.IsPlaying(baseName))
				{
					m_randAnimTimer -= Time.deltaTime;
					if (m_randAnimTimer < 0f)
					{
						m_randAnimTimer = GetNewAnimTimer();
						m_currentAnim = GetRandomAnimation();
						StartAnimation((m_currentAnim != -1) ? CurrentAnim : BaseAnim);
					}
				}
				else
				{
					if (lastConfig != CurrentAnim)
					{
						lastConfig = CurrentAnim;
						lastState = animation[lastConfig.Clip.name];
					}
					if (BaseAnim.FadeMode == FadeModeType.CrossFade)
					{
						if (lastState.time > lastState.length - CurrentAnim.fadeOutTime)
						{
							animation.CrossFade(baseName, BaseAnim.fadeInTime);
						}
					}
					else if (lastState.time + Time.deltaTime > lastState.length)
					{
						animation.Play(baseName, PlayMode.StopAll);
					}
				}
			}
			else
			{
				animation.Play(baseName);
			}
		}

		public enum FadeModeType
		{
			Instant,
			CrossFade
		}

		[Serializable]
		public class AnimEntryConfig
		{
			public AnimationClip Clip;

			public Single Chance = 0.5f;

			public Single fadeInTime = 0.3f;

			public Single fadeOutTime = 0.3f;

			public FadeModeType FadeMode = FadeModeType.CrossFade;
		}
	}
}
