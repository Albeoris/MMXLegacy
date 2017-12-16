using System;
using Legacy.Core.Achievements;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.ServiceWrapper;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/UPlayOverlay")]
	public class UPlayOverlay : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_background;

		[SerializeField]
		private UILabel m_headline;

		[SerializeField]
		private UILabel m_actionTitle;

		[SerializeField]
		private UILabel m_coinsValue;

		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private GameObject m_container;

		[SerializeField]
		private Single m_fadingTime = 1f;

		[SerializeField]
		private Single m_waitTime = 1f;

		private Boolean m_show;

		private Boolean m_visible;

		private Single m_alpha;

		private Boolean m_showOnline = true;

		private Single m_currentAnimationTime;

		private Single m_currentAnimationDuration;

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ACHIEVEMENT_AQUIRED, new EventHandler(OnAchievementAquired));
			LegacyLogic.Instance.ServiceWrapper.OnActionCompleted += OnActionCompleted;
			Show(true);
			UpdateAlpha();
			Show(false);
		}

		private void Show(Boolean p_visible)
		{
			if (m_showOnline)
			{
				NGUITools.SetActive(m_background.gameObject, p_visible);
				NGUITools.SetActive(m_headline.gameObject, p_visible);
				NGUITools.SetActive(m_actionTitle.gameObject, p_visible);
				NGUITools.SetActive(m_coinsValue.gameObject, p_visible);
				NGUITools.SetActive(m_icon.gameObject, p_visible);
			}
			else
			{
				NGUITools.SetActive(m_background.gameObject, p_visible);
				NGUITools.SetActive(m_headline.gameObject, p_visible);
				NGUITools.SetActive(m_actionTitle.gameObject, false);
				NGUITools.SetActive(m_coinsValue.gameObject, false);
				NGUITools.SetActive(m_icon.gameObject, false);
			}
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
			m_background.alpha = m_alpha;
			m_headline.alpha = m_alpha;
			if (m_showOnline)
			{
				m_actionTitle.alpha = m_alpha;
				m_coinsValue.alpha = m_alpha;
				m_icon.alpha = m_alpha;
			}
		}

		private void OnActionCompleted(Object p_sender, ActionCompletedEventArgs p_args)
		{
			if (LegacyLogic.Instance.WorldManager.AchievementManager.IsActionClaimed(p_args.ActionID - 1))
			{
				return;
			}
			if (p_args.Online)
			{
				ShowOverlayOnline(p_args.ActionID);
			}
			else
			{
				ShowOverlayOffline(p_args.ActionID);
			}
		}

		private void OnAchievementAquired(Object p_sender, EventArgs p_args)
		{
			AchievementEventArgs achievementEventArgs = (AchievementEventArgs)p_args;
			Achievement achievement = achievementEventArgs.Achievement;
			if (achievement.StaticID > 4)
			{
				return;
			}
			if (LegacyLogic.Instance.WorldManager.AchievementManager.IsActionClaimed(achievement.StaticID - 1))
			{
				return;
			}
			if (!LegacyLogic.Instance.ServiceWrapper.IsConnected() || LegacyLogic.Instance.ServiceWrapper.IsOfflineMode())
			{
				ShowOverlayOffline(achievement.StaticID);
			}
		}

		private void ShowOverlayOnline(Int32 p_actionId)
		{
			m_showOnline = true;
			m_headline.transform.localPosition = new Vector3(m_headline.transform.localPosition.x, 30f, m_headline.transform.localPosition.z);
			m_actionTitle.text = LegacyLogic.Instance.ServiceWrapper.GetActionName(p_actionId - 1);
			if (m_actionTitle.text == String.Empty)
			{
				switch (p_actionId)
				{
				case 1:
					m_actionTitle.text = LocaManager.GetText("UPLAY_TITLE_ACTION_1");
					break;
				case 2:
					m_actionTitle.text = LocaManager.GetText("UPLAY_TITLE_ACTION_2");
					break;
				case 3:
					m_actionTitle.text = LocaManager.GetText("UPLAY_TITLE_ACTION_3");
					break;
				case 4:
					m_actionTitle.text = LocaManager.GetText("UPLAY_TITLE_ACTION_4");
					break;
				}
			}
			m_coinsValue.text = "+" + p_actionId * 10;
			Single num = Math.Abs(m_actionTitle.relativeSize.x * m_actionTitle.transform.localScale.x);
			Single num2 = Math.Abs(m_coinsValue.relativeSize.x * m_coinsValue.transform.localScale.x);
			m_coinsValue.transform.localPosition = new Vector3(m_actionTitle.transform.localPosition.x + num + num2 + 25f, m_coinsValue.transform.localPosition.y, m_coinsValue.transform.localPosition.z);
			m_icon.transform.localPosition = new Vector3(m_coinsValue.transform.localPosition.x + m_icon.transform.localScale.x / 2f, m_icon.transform.localPosition.y, m_icon.transform.localPosition.z);
			Single val = m_headline.relativeSize.x * m_headline.transform.localScale.x + 170f;
			Single val2 = Math.Abs(m_headline.transform.localPosition.x) + num + 40f;
			Single val3 = Math.Abs(m_icon.transform.localPosition.x) + m_icon.transform.localScale.x / 2f + 40f;
			Single num3 = Math.Max(val2, val3);
			num3 = Math.Max(num3, val);
			m_background.transform.localScale = new Vector3(num3, m_background.transform.localScale.y, m_background.transform.localScale.z);
			m_container.transform.localPosition = new Vector3(-num3 * 0.5f, 0f, 0f);
			LegacyLogic.Instance.WorldManager.AchievementManager.ClaimAction(p_actionId - 1);
			Show(true);
			FadeIn();
		}

		private void ShowOverlayOffline(Int32 p_actionId)
		{
			m_showOnline = false;
			m_headline.transform.localPosition = new Vector3(m_headline.transform.localPosition.x, 12f, m_headline.transform.localPosition.z);
			Single num = m_headline.relativeSize.x * m_headline.transform.localScale.x;
			Single num2 = Math.Abs(m_headline.transform.localPosition.x) + num + 40f;
			m_background.transform.localScale = new Vector3(num2, m_background.transform.localScale.y, m_background.transform.localScale.z);
			m_container.transform.localPosition = new Vector3(-num2 * 0.5f, 0f, 0f);
			LegacyLogic.Instance.WorldManager.AchievementManager.ClaimAction(p_actionId - 1);
			Show(true);
			FadeIn();
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ACHIEVEMENT_AQUIRED, new EventHandler(OnAchievementAquired));
			LegacyLogic.Instance.ServiceWrapper.OnActionCompleted -= OnActionCompleted;
		}
	}
}
