using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	public class BaseHUDLog : MonoBehaviour
	{
		protected const Single FADE_DELTA_VALUE = 0.05f;

		protected Single m_currentAlpha = 1f;

		protected Single m_configFadeDelay = 1.5f;

		protected Boolean m_configSaysFade;

		protected Single m_initialBackgroundAlpha;

		protected Single m_initialScrollBarAlpha = 0.5f;

		protected Boolean m_isAlreadyFadingIn;

		protected Boolean m_locked;

		protected EFadeState m_fadeState = EFadeState.FADE_OUT;

		protected Single m_waitTime;

		protected Boolean m_isHovered;

		protected void Init()
		{
			m_configSaysFade = ConfigManager.Instance.Options.FadeLogs;
			m_configFadeDelay = ConfigManager.Instance.Options.FadeLogsDelay;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(OnOptionsChanged));
		}

		protected void Cleanup()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(OnOptionsChanged));
		}

		private void OnOptionsChanged(Object sender, EventArgs e)
		{
			m_configSaysFade = ConfigManager.Instance.Options.FadeLogs;
			m_configFadeDelay = ConfigManager.Instance.Options.FadeLogsDelay;
			OnOptionsChanged();
		}

		public virtual void OnOptionsChanged()
		{
		}

		public virtual void FadeComponents(Single p_alpha)
		{
		}

		protected void ResetWaitTime()
		{
			m_waitTime = Time.time + m_configFadeDelay;
		}

		protected virtual void Update()
		{
			if (!m_configSaysFade)
			{
				return;
			}
			if (m_locked)
			{
				return;
			}
			if (m_fadeState == EFadeState.DONE)
			{
				return;
			}
			if (m_fadeState == EFadeState.FADE_IN)
			{
				if (m_currentAlpha < 1f)
				{
					m_currentAlpha += 0.05f;
					FadeComponents(m_currentAlpha);
				}
				else
				{
					if (m_isHovered)
					{
						return;
					}
					m_currentAlpha = 1f;
					FadeComponents(m_currentAlpha);
					m_waitTime = Time.time + m_configFadeDelay;
					m_fadeState = EFadeState.WAITING;
				}
			}
			else if (m_fadeState == EFadeState.FADE_OUT)
			{
				if (m_currentAlpha > 0f)
				{
					m_currentAlpha -= 0.05f;
					FadeComponents(m_currentAlpha);
				}
				else
				{
					m_currentAlpha = 0f;
					FadeComponents(m_currentAlpha);
					m_fadeState = EFadeState.DONE;
				}
			}
			else if (m_fadeState == EFadeState.WAITING && m_waitTime <= Time.time)
			{
				m_fadeState = EFadeState.FADE_OUT;
			}
		}

		public virtual void OnHover(Boolean p_isHovered)
		{
			m_isHovered = p_isHovered;
			if (m_isAlreadyFadingIn || m_locked)
			{
				return;
			}
			if (m_isHovered)
			{
				m_fadeState = EFadeState.FADE_IN;
			}
			else if (m_currentAlpha < 1f)
			{
				m_fadeState = EFadeState.FADE_OUT;
			}
			else
			{
				m_waitTime = Time.time + m_configFadeDelay;
				m_fadeState = EFadeState.WAITING;
			}
		}

		protected enum EFadeState
		{
			FADE_IN,
			FADE_OUT,
			WAITING,
			DONE
		}
	}
}
