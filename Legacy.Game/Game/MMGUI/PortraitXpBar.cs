using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/PortraitXpBar")]
	public class PortraitXpBar : MonoBehaviour
	{
		[SerializeField]
		private Single m_fadeTime = 1f;

		[SerializeField]
		private Single m_showTime = 3f;

		[SerializeField]
		private UIFilledSprite m_xpBarMain;

		[SerializeField]
		private UISprite m_xpBarBackground;

		[SerializeField]
		private UILabel m_xpGainLabel;

		private Queue<XpData> m_xpQueue;

		private Single m_currentShowTime;

		private Boolean m_currentShowedLevelUpMessage;

		private void Awake()
		{
			m_xpQueue = new Queue<XpData>();
			SetEnabledAll(false);
			m_xpBarBackground.alpha = 0f;
			m_xpBarMain.alpha = 0f;
		}

		private void Update()
		{
			if (m_xpQueue.Count == 0)
			{
				SetEnabledAll(false);
			}
			else
			{
				XpData xpData = m_xpQueue.Peek();
				if (xpData.m_startShowingTime == 0f)
				{
					m_currentShowedLevelUpMessage = false;
					m_currentShowTime = m_showTime;
					if (xpData.m_levelUp != 0)
					{
						m_currentShowTime *= 2f;
					}
					xpData.m_startShowingTime = Time.time;
					m_xpBarMain.fillAmount = xpData.m_percentXPFilled;
					m_xpGainLabel.text = "+" + xpData.m_xpWon;
					m_xpGainLabel.color = Color.white;
				}
				Single num = Time.time - xpData.m_startShowingTime;
				if (num <= m_fadeTime)
				{
					Single alpha = num / m_fadeTime;
					m_xpBarBackground.alpha = alpha;
					m_xpBarMain.alpha = alpha;
					m_xpGainLabel.alpha = alpha;
				}
				else if (num <= m_fadeTime + m_currentShowTime)
				{
					m_xpBarBackground.alpha = 1f;
					m_xpBarMain.alpha = 1f;
					m_xpGainLabel.alpha = 1f;
					if (!m_currentShowedLevelUpMessage && xpData.m_levelUp != 0 && num > m_fadeTime + m_showTime)
					{
						m_currentShowedLevelUpMessage = true;
						m_xpGainLabel.color = Color.green;
						m_xpGainLabel.text = LocaManager.GetText("CHARACTER_LEVEL", xpData.m_levelUp);
					}
				}
				else if (num <= m_fadeTime + m_currentShowTime + m_fadeTime)
				{
					Single alpha2 = 1f - (num - (m_fadeTime + m_showTime)) / m_fadeTime;
					m_xpBarBackground.alpha = alpha2;
					m_xpBarMain.alpha = alpha2;
					m_xpGainLabel.alpha = alpha2;
				}
				else
				{
					m_xpBarBackground.alpha = 0f;
					m_xpBarMain.alpha = 0f;
					m_xpGainLabel.alpha = 0f;
					m_xpQueue.Dequeue();
				}
			}
		}

		public void AddXpWon(Int32 xpWon, Int32 levelup, Single percentXPFilled)
		{
			if (!enabled)
			{
				SetEnabledAll(true);
			}
			XpData item = new XpData(xpWon, levelup, percentXPFilled);
			m_xpQueue.Enqueue(item);
		}

		public void Clear()
		{
			m_xpBarBackground.alpha = 0f;
			m_xpBarMain.alpha = 0f;
			m_xpGainLabel.alpha = 0f;
			m_xpQueue.Clear();
		}

		private void SetEnabledAll(Boolean b)
		{
			enabled = b;
			m_xpBarMain.enabled = b;
			m_xpBarBackground.enabled = b;
			m_xpGainLabel.enabled = b;
		}

		public class XpData
		{
			public Int32 m_xpWon;

			public Int32 m_levelUp;

			public Single m_percentXPFilled;

			public Single m_startShowingTime;

			public XpData(Int32 xpWon, Int32 levelUp, Single percentXPFilled)
			{
				m_xpWon = xpWon;
				m_levelUp = levelUp;
				m_percentXPFilled = percentXPFilled;
				m_startShowingTime = 0f;
			}
		}
	}
}
