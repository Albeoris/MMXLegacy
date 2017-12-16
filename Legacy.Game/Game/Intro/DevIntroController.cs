using System;
using Legacy.Core.Configuration;
using Legacy.Game.Context;
using Legacy.Game.MMGUI.Videos;
using UnityEngine;

namespace Legacy.Game.Intro
{
	public class DevIntroController : MonoBehaviour
	{
		private Boolean m_SkipableByUser;

		[SerializeField]
		private BinkVideoController m_AVProMovie;

		private void Start()
		{
			m_SkipableByUser = (PlayerPrefs.GetInt("SkipDevIntro", 0) != 0);
			if (!m_SkipableByUser)
			{
				PlayerPrefs.SetInt("SkipDevIntro", 1);
			}
			EVideoDecoder videoDecoder = ConfigManager.Instance.Options.VideoDecoder;
			if (videoDecoder == EVideoDecoder.System)
			{
				m_AVProMovie.enabled = true;
				if (m_AVProMovie.LoadMovie())
				{
					m_AVProMovie.Play();
				}
				else
				{
					ContextManager.ChangeContext(EContext.Mainmenu);
				}
			}
			else
			{
				ContextManager.ChangeContext(EContext.Mainmenu);
			}
		}

		private void Update()
		{
			if (Input.anyKeyDown && m_SkipableByUser)
			{
				m_AVProMovie.Pause();
				m_AVProMovie.View.enabled = false;
				ContextManager.ChangeContext(EContext.Mainmenu);
				enabled = false;
			}
			if (m_AVProMovie.IsError || m_AVProMovie.IsFinishedPlaying)
			{
				ContextManager.ChangeContext(EContext.Mainmenu);
			}
		}
	}
}
