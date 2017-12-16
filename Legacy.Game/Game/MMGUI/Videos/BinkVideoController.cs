using System;
using System.IO;
using Legacy.Bink;
using Legacy.Core.Configuration;
using UnityEngine;

namespace Legacy.Game.MMGUI.Videos
{
	[RequireComponent(typeof(UITexture))]
	[AddComponentMenu("MM Legacy/MMGUI/Bink video controller")]
	public class BinkVideoController : MonoBehaviour
	{
		private Int32 m_ScreenWidth;

		private Int32 m_ScreenHeight;

		private UITexture m_View;

		private BinkMedia m_MediaPlayer;

		private String m_Error;

		[SerializeField]
		private Boolean m_Fullscreen;

		[SerializeField]
		private ScaleMode m_ScaleMode;

		[SerializeField]
		private String m_FilePath;

		[SerializeField]
		private Single m_Volume = 1f;

		[SerializeField]
		public MovieSubtitleController m_subtitles;

		public UITexture View => m_View;

	    public Boolean IsError => m_Error != null;

	    public String Error => m_Error;

	    public String FilePath
		{
			get => m_FilePath;
	        set => m_FilePath = value;
	    }

		public Boolean IsPlaying => m_MediaPlayer != null && m_MediaPlayer.IsPlaying;

	    public Boolean IsFinishedPlaying => m_MediaPlayer != null && m_MediaPlayer.IsFinishedPlaying;

	    public BinkMedia Media => m_MediaPlayer;

	    public Boolean LoadMovie()
		{
			String text = Path.Combine(Application.streamingAssetsPath, m_FilePath);
			if (!File.Exists(text))
			{
				Debug.LogWarning("Bink file not found! " + text, this);
				return false;
			}
			UnloadMovie();
			try
			{
				m_MediaPlayer = new BinkMedia(text, false, false, false);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message, this);
				m_Error = ex.Message;
				return false;
			}
			if (m_View == null)
			{
				m_View = this.GetComponent< UITexture>(true);
			}
			m_View.mainTexture = m_MediaPlayer.OutputTexture;
			if (m_View.hasDynamicMaterial)
			{
				m_View.shader = Helper.FindShader("Unlit/BinkTexture");
			}
			m_ScreenWidth = 0;
			m_ScreenHeight = 0;
			Update();
			return true;
		}

		public void Play()
		{
			if (m_MediaPlayer != null)
			{
				m_MediaPlayer.Play();
			}
		}

		public void Pause()
		{
			if (m_MediaPlayer != null)
			{
				m_MediaPlayer.Pause();
			}
		}

		public void UnloadMovie()
		{
			if (m_MediaPlayer != null)
			{
				m_View.mainTexture = null;
				m_MediaPlayer.Dispose();
				m_MediaPlayer = null;
			}
		}

		protected virtual void Awake()
		{
			m_View = this.GetComponent< UITexture>(true);
		}

		protected virtual void OnRenderObject()
		{
			if (m_MediaPlayer != null)
			{
				m_MediaPlayer.Update();
			}
		}

		protected virtual void Update()
		{
			if (m_MediaPlayer != null)
			{
				m_Volume = Mathf.Clamp01(m_Volume);
				if (m_Volume != m_MediaPlayer.Volume)
				{
					m_MediaPlayer.Volume = m_Volume;
				}
			}
			if (m_subtitles != null && m_MediaPlayer != null && m_MediaPlayer.IsPlaying)
			{
				m_subtitles.ShowSubtitle = ConfigManager.Instance.Options.SubTitles;
				m_subtitles.UpdateSubtitle(m_MediaPlayer.PositionSeconds);
			}
			if (m_Fullscreen && (m_ScreenWidth != Screen.width || m_ScreenHeight != Screen.height))
			{
				Texture mainTexture = m_View.mainTexture;
				if (mainTexture != null)
				{
					m_ScreenWidth = Screen.width;
					m_ScreenHeight = Screen.height;
					switch (m_ScaleMode)
					{
					case ScaleMode.StretchToFill:
						m_View.cachedTransform.localScale = new Vector3(m_ScreenWidth, m_ScreenHeight, 1f);
						break;
					case ScaleMode.ScaleAndCrop:
					case ScaleMode.ScaleToFit:
						if (mainTexture.height < m_ScreenHeight)
						{
							Single num = mainTexture.width / (Single)mainTexture.height;
							m_View.cachedTransform.localScale = new Vector3(Mathf.Round(m_ScreenHeight * num), m_ScreenHeight, 1f);
						}
						else
						{
							Single num2 = mainTexture.height / (Single)mainTexture.width;
							m_View.cachedTransform.localScale = new Vector3(m_ScreenWidth, Mathf.Round(m_ScreenWidth * num2), 1f);
						}
						break;
					}
				}
			}
		}

		protected virtual void OnDestroy()
		{
			UnloadMovie();
		}
	}
}
