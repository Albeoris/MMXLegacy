using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Game.Context;
using Legacy.Game.MMGUI;
using Legacy.Game.MMGUI.Videos;
using UnityEngine;

namespace Legacy.Game.Intro
{
	[AddComponentMenu("MM Legacy/MMGUI/IntroController")]
	public class IntroController : MonoBehaviour
	{
		[SerializeField]
		private BinkVideoController m_AVProMovie;

		[SerializeField]
		private Boolean m_Fullscreen;

		[SerializeField]
		private ScaleMode m_ScaleMode;

		[SerializeField]
		private MovieSubtitleController m_subtitles;

		[SerializeField]
		private UITexture m_uiTex;

		private Single m_currentPlaybackTime;

		private void Start()
		{
			if (LegacyLogic.Instance.ModController.InModMode)
			{
				FinishIntro();
				return;
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
					enabled = false;
					FinishIntro();
				}
			}
			else
			{
				enabled = false;
				FinishIntro();
			}
		}

		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.End) || Input.GetKeyUp(KeyCode.KeypadEnter))
			{
				FinishIntro();
			}
			if (m_AVProMovie.IsError || m_AVProMovie.IsFinishedPlaying)
			{
				FinishIntro();
				enabled = false;
			}
		}

		private void FinishIntro()
		{
			ContextManager.ChangeContext(EContext.Game);
		}
	}
}
