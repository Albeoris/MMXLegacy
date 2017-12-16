using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.EffectEngine.Effects
{
	public class BuffFX : FXBase
	{
		[SerializeField]
		private FXBase m_FadeInFX;

		[SerializeField]
		private FXBase m_LoopFX;

		[SerializeField]
		private FXBase m_FadeOutFX;

		[SerializeField]
		private String m_FadeInAudioID;

		[SerializeField]
		private String m_LoopAudioID;

		[SerializeField]
		private String m_FadeOutAudioID;

		[SerializeField]
		private Single m_DestroyFadeInAfter = -1f;

		private AudioObject m_LoopSound;

		private EEventType m_RemoveEventType;

		private RemoveEventHandler m_IsBuffRemoved;

		private Boolean m_IsStarted;

		private Boolean m_IsFadedIn;

		private Boolean m_IsFinished;

		public override Boolean IsFinished => m_IsFinished;

	    public void SetBuffRemoveCondition(EEventType p_RemoveEvent, RemoveEventHandler p_IsBuffRemoved)
		{
			m_RemoveEventType = p_RemoveEvent;
			m_IsBuffRemoved = p_IsBuffRemoved;
			LegacyLogic.Instance.EventManager.RegisterEvent(m_RemoveEventType, new EventHandler(OnRemoveEvent));
		}

		public override void Begin(Single p_lifetime, FXArgs p_args)
		{
			base.Begin(p_lifetime, p_args);
			if (m_FadeInFX == null && m_LoopFX == null && m_FadeOutFX == null)
			{
				Debug.LogError("BuffFX: Begin: no FX specified!");
			}
			else
			{
				if (m_FadeInFX != null)
				{
					m_FadeInFX = Helper.Instantiate<FXBase>(m_FadeInFX);
				}
				if (m_LoopFX != null)
				{
					m_LoopFX = Helper.Instantiate<FXBase>(m_LoopFX);
				}
				if (m_FadeOutFX != null)
				{
					m_FadeOutFX = Helper.Instantiate<FXBase>(m_FadeOutFX);
				}
			}
			if (m_FadeInFX != null)
			{
				m_FadeInFX.Begin(p_lifetime, p_args);
			}
			if (!String.IsNullOrEmpty(m_FadeInAudioID))
			{
				AudioController.Play(m_FadeInAudioID, EffectArguments.TargetTransform);
			}
			m_IsStarted = true;
			if (m_DestroyFadeInAfter != -1f)
			{
				m_DestroyFadeInAfter += Time.deltaTime;
			}
		}

		public override void Update()
		{
			if (!m_IsFinished)
			{
				base.Update();
				if (m_IsFadedIn)
				{
					if (m_LoopFX != null && !m_LoopFX.IsFinished)
					{
						m_LoopFX.Update();
					}
				}
				else if (m_FadeInFX != null)
				{
					if (m_FadeInFX.IsFinished)
					{
						StartLoopFX();
					}
					else
					{
						m_FadeInFX.Update();
						if (m_DestroyFadeInAfter != -1f)
						{
							m_DestroyFadeInAfter -= Time.deltaTime;
							if (m_DestroyFadeInAfter <= 0f)
							{
								StartLoopFX();
							}
						}
					}
				}
				else
				{
					StartLoopFX();
				}
				if (m_FadeOutFX != null && !m_FadeOutFX.IsFinished)
				{
					m_FadeOutFX.Update();
				}
			}
		}

		public override void Destroy()
		{
			if (!m_IsFinished)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(m_RemoveEventType, new EventHandler(OnRemoveEvent));
			}
			if (m_IsStarted)
			{
				if (!m_IsFinished)
				{
					m_IsFinished = true;
					if (m_FadeInFX != null)
					{
						m_FadeInFX.Destroy();
						m_FadeInFX = null;
					}
					if (m_LoopFX != null)
					{
						m_LoopFX.Destroy();
						m_LoopFX = null;
					}
					StopLoopSound();
					if (!String.IsNullOrEmpty(m_FadeOutAudioID))
					{
						AudioController.Play(m_FadeOutAudioID, EffectArguments.TargetTransform);
					}
					if (m_FadeOutFX != null)
					{
						m_FadeOutFX.Begin(m_Lifetime, EffectArguments);
						m_FadeOutFX.Destroy();
						m_FadeOutFX = null;
					}
				}
			}
			else
			{
				Debug.LogError("BuffFX: Destroy called before Begin! Destroy call skipped!");
			}
			base.Destroy();
		}

		private void OnRemoveEvent(Object p_Sender, EventArgs p_Args)
		{
			if (m_IsBuffRemoved(p_Sender, p_Args))
			{
				Destroy();
			}
		}

		private void StopLoopSound()
		{
			if (m_LoopSound != null && m_LoopSound.IsPlaying())
			{
				m_LoopSound.Stop();
			}
			m_LoopSound = null;
		}

		private void StartLoopFX()
		{
			m_IsFadedIn = true;
			if (m_FadeInFX != null)
			{
				m_FadeInFX.Destroy();
				m_FadeInFX = null;
			}
			if (m_LoopFX != null)
			{
				m_LoopFX.Begin(m_Lifetime, EffectArguments);
			}
			if (!String.IsNullOrEmpty(m_LoopAudioID))
			{
				m_LoopSound = AudioController.Play(m_LoopAudioID, EffectArguments.TargetTransform);
			}
		}

		public delegate Boolean RemoveEventHandler(Object p_sender, EventArgs p_args);
	}
}
