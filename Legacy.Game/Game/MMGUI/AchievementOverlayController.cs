using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	public class AchievementOverlayController : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_text;

		[SerializeField]
		private UILabel m_title;

		[SerializeField]
		private UISlicedSprite m_background;

		[SerializeField]
		private Single m_fadingTime = 1f;

		[SerializeField]
		private Single m_waitTime = 1f;

		private Boolean m_show;

		private Boolean m_visible;

		private Single m_alpha;

		private Single m_currentAnimationTime;

		private Single m_currentAnimationDuration;

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ACHIEVEMENT_AQUIRED, new EventHandler(OnAchievementAqired));
			Show(true);
			UpdateAlpha();
			Show(false);
		}

		private void Show(Boolean p_visible)
		{
			NGUITools.SetActive(m_background.gameObject, p_visible);
			NGUITools.SetActive(m_title.gameObject, p_visible);
			NGUITools.SetActive(m_text.gameObject, p_visible);
		}

		private void FadeIn()
		{
			m_show = true;
			m_visible = true;
			m_currentAnimationTime = 0f;
			m_currentAnimationDuration = m_fadingTime + m_waitTime;
		}

		private void FadeOut()
		{
			m_show = false;
			m_currentAnimationTime = 0f;
			m_currentAnimationDuration = m_fadingTime;
		}

		private void Update()
		{
			if (m_visible)
			{
				if (m_show)
				{
					m_currentAnimationTime += Time.deltaTime;
					m_alpha = Mathf.Clamp01(m_currentAnimationTime / m_fadingTime);
					if (m_currentAnimationTime >= m_currentAnimationDuration)
					{
						FadeOut();
					}
				}
				else
				{
					m_currentAnimationTime += Time.deltaTime;
					m_alpha = Mathf.Clamp01(1f - m_currentAnimationTime / m_currentAnimationDuration);
					if (m_currentAnimationTime >= m_currentAnimationDuration)
					{
						m_visible = false;
						Show(false);
					}
				}
				UpdateAlpha();
			}
		}

		private void UpdateAlpha()
		{
			m_text.alpha = m_alpha;
			m_title.alpha = m_alpha;
			m_background.alpha = m_alpha;
		}

		private void OnAchievementAqired(Object p_sender, EventArgs p_args)
		{
			AchievementEventArgs achievementEventArgs = (AchievementEventArgs)p_args;
			if (!LegacyLogic.Instance.ServiceWrapper.IsConnected())
			{
				Show(true);
				m_text.text = LocaManager.GetText("ACHIEVEMENT_AQUIRED_TEXT", LocaManager.GetText(achievementEventArgs.Achievement.NameKey));
				FadeIn();
			}
			LegacyLogic.Instance.ServiceWrapper.EarnAchievment(achievementEventArgs.Achievement.StaticID);
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ACHIEVEMENT_AQUIRED, new EventHandler(OnAchievementAqired));
		}
	}
}
