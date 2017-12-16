using System;
using Legacy.Loading;
using UnityEngine;

namespace Legacy.NpcInteraction
{
	internal class IndoorSceneRoot : MonoBehaviour
	{
		[SerializeField]
		private String m_sceneName;

		[SerializeField]
		private Camera m_sceneCamera4x3;

		[SerializeField]
		private Camera m_sceneCamera16x9;

		[SerializeField]
		private String m_enterAudioID = "EnterIndoor";

		[SerializeField]
		private String m_exitAudioID = "ExitIndoor";

		[SerializeField]
		private String m_musicAudioID = String.Empty;

		[SerializeField]
		private Boolean m_isMusicLooped = true;

		[SerializeField]
		private Single m_musicLoopDelay;

		[SerializeField]
		private Color m_ambientColor = Color.black;

		private Color m_lastColor;

		public Color AmbientColor
		{
			get => m_ambientColor;
		    set => m_ambientColor = value;
		}

		public String SceneName
		{
			get => m_sceneName;
		    set => m_sceneName = value;
		}

		public Camera SceneCamera4x3
		{
			get => m_sceneCamera4x3;
		    set => m_sceneCamera4x3 = value;
		}

		public Camera SceneCamera16x9
		{
			get => m_sceneCamera16x9;
		    set => m_sceneCamera16x9 = value;
		}

		public Camera SceneCamera
		{
			get
			{
				Single num = Screen.width / (Single)Screen.height;
				if (Mathf.Abs(num - 1.33333337f) < Mathf.Abs(num - 1.77777779f))
				{
					m_sceneCamera4x3.gameObject.SetActive(true);
					m_sceneCamera16x9.gameObject.SetActive(false);
					return m_sceneCamera4x3;
				}
				m_sceneCamera4x3.gameObject.SetActive(false);
				m_sceneCamera16x9.gameObject.SetActive(true);
				return m_sceneCamera16x9;
			}
		}

		public String EnterAudioID
		{
			get => m_enterAudioID;
		    set => m_enterAudioID = value;
		}

		public String ExitAudioID
		{
			get => m_exitAudioID;
		    set => m_exitAudioID = value;
		}

		public String MusicAudioID
		{
			get => m_musicAudioID;
		    set => m_musicAudioID = value;
		}

		public Boolean IsMusicLooped => m_isMusicLooped;

	    public Single MusicLoopDelay => m_musicLoopDelay;

	    private void Awake()
		{
			if (IndoorSceneLoader.Instance != null)
			{
				IndoorSceneLoader.Instance.IndoorSceneLoaded(this);
			}
		}

		private void OnEnable()
		{
			m_lastColor = RenderSettings.ambientLight;
			RenderSettings.ambientLight = m_ambientColor;
		}

		private void OnDisable()
		{
			RenderSettings.ambientLight = m_lastColor;
		}
	}
}
