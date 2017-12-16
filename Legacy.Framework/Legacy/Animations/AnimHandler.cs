using System;
using System.Threading;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Animations
{
	[AddComponentMenu("MM Legacy/Animation/Animation Handler")]
	[RequireComponent(typeof(Animation))]
	public class AnimHandler : MonoBehaviour
	{
		public const Single ANIM_REWIND_CROSSFADE_TIME = 0.3f;

		public const Single ANIM_TOIDLE_CROSSFADE_TIME = 0.3f;

		[SerializeField]
		private AnimConfig m_Config;

		private Animation m_Animation;

		private Single m_EndTime = -1f;

		private RootMotionComputer m_RootMotionComp;

	    public event EventHandler<AnimPlayEventArgs> PlayingAnimation;

		public AnimConfig Config => m_Config;

	    public Boolean IsPlaying(EAnimType p_type)
		{
			return m_Animation.IsPlaying(m_Config.ClipName(p_type));
		}

		public AnimationState GetState(EAnimType p_type)
		{
			String name = m_Config.ClipName(p_type);
			return m_Animation[name];
		}

		public void Play(EAnimType p_type)
		{
			String p_clipName = m_Config.ClipName(p_type);
			Play(p_type, p_clipName, -1f, 1f);
		}

		public void Play(EAnimType p_type, Single p_duration)
		{
			String p_clipName = m_Config.ClipName(p_type);
			Play(p_type, p_clipName, p_duration, 1f);
		}

		public void Play(EAnimType p_type, Single p_duration, Single p_speed)
		{
			String p_clipName = m_Config.ClipName(p_type);
			Play(p_type, p_clipName, p_duration, p_speed);
		}

		public void Play(String p_clipName)
		{
			Play(EAnimType._MAX_, p_clipName, -1f, 1f);
		}

		public void Play(String p_clipName, Single p_duration)
		{
			Play(EAnimType._MAX_, p_clipName, p_duration, 1f);
		}

		public void Play(String p_clipName, Single p_duration, Single p_speed)
		{
			Play(EAnimType._MAX_, p_clipName, p_duration, p_speed);
		}

		private void Play(EAnimType p_type, String p_clipName, Single p_duration, Single p_speed)
		{
			if (m_Animation == null)
			{
				return;
			}
			AnimationState animationState = m_Animation[p_clipName];
			if (animationState != null)
			{
				if (p_type == EAnimType._MAX_)
				{
					p_type = m_Config.ClipType(p_clipName);
				}
				if (p_type == EAnimType.IDLE)
				{
					m_EndTime = -1f;
				}
				else
				{
					m_EndTime = ((p_duration != -1f) ? p_duration : animationState.length);
					m_EndTime += Time.time;
				}
				if (m_RootMotionComp != null)
				{
					m_RootMotionComp.enabled = (p_type == EAnimType.TURN_LEFT || p_type == EAnimType.TURN_RIGHT);
				}
				if (m_Animation.isPlaying)
				{
					if (m_Animation.IsPlaying(p_clipName))
					{
						AnimationState animationState2 = m_Animation.CrossFadeQueued(p_clipName, 0.3f, QueueMode.PlayNow);
						if (m_RootMotionComp != null)
						{
							m_RootMotionComp.AddAnimInfoToTable(animationState2);
						}
						animationState2.speed = p_speed;
						animationState2.layer += Time.frameCount;
					}
					else
					{
						m_Animation.CrossFade(p_clipName, 0.3f, PlayMode.StopAll);
						animationState.speed = p_speed;
					}
				}
				else
				{
					m_Animation.Play(p_clipName);
					animationState.speed = p_speed;
				}
			}
			else
			{
				Debug.LogError(String.Concat(new Object[]
				{
					"Missing animation ",
					p_type,
					" (",
					p_clipName,
					") !!"
				}));
			}
			if (PlayingAnimation != null)
			{
				PlayingAnimation(this, new AnimPlayEventArgs(p_type, p_clipName));
			}
			this.SendEvent("OnPlayAnimation", new PlayAnimationUnityEventArgs(this, p_type, p_clipName));
		}

		public void Stop()
		{
			if (m_Animation == null)
			{
				return;
			}
			m_Animation.Stop();
		}

		protected virtual void Awake()
		{
			if (m_Config == null)
			{
				throw new ComponentNotFoundException("AnimConfig not defined!");
			}
			if (animation == null)
			{
				throw new ComponentNotFoundException("Animation not defined!");
			}
			m_RootMotionComp = GetComponent<RootMotionComputer>();
			m_Animation = animation;
			m_Animation.playAutomatically = false;
			m_Animation.cullingType = AnimationCullingType.AlwaysAnimate;
			AnimationState state = GetState(EAnimType.DIE);
			if (state != null && state.wrapMode != WrapMode.ClampForever)
			{
				Debug.LogError("Death animation wrapMode of '" + name + "' is not ClampForever!");
			}
		}

		private void OnEnable()
		{
			if (m_RootMotionComp != null)
			{
				m_RootMotionComp.enabled = true;
			}
		}

		private void OnDisable()
		{
			if (m_RootMotionComp != null)
			{
				m_RootMotionComp.enabled = false;
			}
		}

		private void Update()
		{
			if (!IsPlaying(EAnimType.DIE) && !IsPlaying(EAnimType.IDLE) && m_EndTime - 0.3f <= Time.time)
			{
				m_EndTime = -1f;
				String text = m_Config.ClipName(EAnimType.IDLE);
				m_Animation.CrossFade(text, 0.3f);
				this.SendEvent("OnPlayAnimation", new PlayAnimationUnityEventArgs(this, EAnimType.IDLE, text));
			}
		}
	}
}
