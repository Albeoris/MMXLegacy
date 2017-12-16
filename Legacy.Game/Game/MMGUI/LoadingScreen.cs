using System;
using System.Collections;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;
using Legacy.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/LoadingScreen")]
	public class LoadingScreen : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_MainGUIRoot;

		[SerializeField]
		private UITexture m_uiTexture;

		[SerializeField]
		private GameObject m_loadingText;

		[SerializeField]
		private GUITextureMonochrome m_monochrome;

		private Single m_updateTime;

		private String m_dots = String.Empty;

		private String m_loadingTextString = "Loading";

		private UILabel m_loadingTextLabel;

		private void Awake()
		{
			m_loadingTextString = LocaManager.GetText("LOADING_SCREEN_LOADING");
			m_loadingTextLabel = m_loadingText.GetComponentInChildren<UILabel>();
			if (LegacyLogic.Instance.MapLoader.IsLoading)
			{
				OnStartLoadScene(null, null);
			}
			else
			{
				OnFinishLoadViews(null, null);
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartLoadScene));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnFinishLoadViews));
		}

		private void Update()
		{
			Single monochromeValue = LegacyLogic.Instance.MapLoader.ViewManagerProgress + LegacyLogic.Instance.MapLoader.SceneLoaderProgress;
			m_monochrome.SetMonochromeValue(monochromeValue);
			m_updateTime += Time.deltaTime;
			if (m_updateTime >= 0.5f)
			{
				m_updateTime -= 0.5f;
				m_dots += ".";
				if (m_dots.Length > 3)
				{
					m_dots = String.Empty;
				}
				m_loadingTextLabel.text = m_loadingTextString + m_dots;
			}
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartLoadScene));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnFinishLoadViews));
		}

		private void OnStartLoadScene(Object sender, EventArgs p_args)
		{
			gameObject.SetActive(true);
			m_loadingText.SetActive(true);
			m_MainGUIRoot.SetActive(false);
			Texture mainTexture = m_uiTexture.mainTexture;
			if (mainTexture != null)
			{
				m_uiTexture.cachedTransform.localScale = new Vector3(mainTexture.width, mainTexture.height);
			}
			if (FXMainCamera.Instance != null)
			{
				FXMainCamera.Instance.CurrentCamera.camera.enabled = false;
			}
			AudioHelper.MuteGUI();
			StartCoroutine(DisableLoadedMainCamera());
		}

		private void OnFinishLoadViews(Object sender, EventArgs p_args)
		{
			StopAllCoroutines();
			if (sender != null || p_args != null)
			{
				AudioHelper.UnmuteGUI();
			}
			gameObject.SetActive(false);
			m_loadingText.SetActive(false);
			m_MainGUIRoot.SetActive(true);
			if (FXMainCamera.Instance != null)
			{
				FXMainCamera.Instance.CurrentCamera.camera.enabled = true;
			}
		}

		private IEnumerator DisableLoadedMainCamera()
		{
			for (;;)
			{
				if (FXMainCamera.Instance != null && FXMainCamera.Instance.CurrentCamera != null)
				{
					FXMainCamera.Instance.CurrentCamera.camera.enabled = false;
				}
				yield return null;
			}
		}
	}
}
