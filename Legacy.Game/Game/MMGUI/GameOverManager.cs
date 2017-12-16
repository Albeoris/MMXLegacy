using System;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Game.Context;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/GameOverManager")]
	public class GameOverManager : MonoBehaviour
	{
		[SerializeField]
		private Texture m_backgroundTexture43;

		[SerializeField]
		private Texture m_backgroundTexture169;

		[SerializeField]
		private SaveGameMenuController m_saveGameMenu;

		[SerializeField]
		private GameObject m_loadLastSaveGameButton;

		[SerializeField]
		private UILabel m_labelPart1;

		[SerializeField]
		private UILabel m_labelPart2;

		[SerializeField]
		private UITexture m_background;

		[SerializeField]
		private Single m_label1FadeTime = 1f;

		[SerializeField]
		private Single m_label1FadeDelay = 1f;

		[SerializeField]
		private Single m_label2FadeTime = 1f;

		[SerializeField]
		private Single m_label2FadeDelay = 1f;

		[SerializeField]
		private String m_AudioID;

		[SerializeField]
		private UIButton m_loadLastButton;

		[SerializeField]
		private UIButton m_loadSaveGameButton;

		[SerializeField]
		private UIButton m_quitButton;

		private Single m_oldLoadGamePosition;

		private Single m_gameOverTime;

		private Boolean m_label1Delayed;

		private Boolean m_label2Delayed;

		private void Awake()
		{
			OnResolutionChange();
			m_saveGameMenu.OnLoadSaveGame += HandleOnLoadSaveGame;
			m_oldLoadGamePosition = m_saveGameMenu.transform.localPosition.z;
			LegacyLogic.Instance.CommandManager.AllowContinuousCommands = true;
			Show();
			NGUITools.SetActive(m_loadLastSaveGameButton, false);
		}

		private void DisableUnnecessaryButtons()
		{
			m_loadLastButton.isEnabled = false;
			m_loadSaveGameButton.isEnabled = false;
			m_quitButton.isEnabled = false;
		}

		private void OnDestroy()
		{
			m_saveGameMenu.OnLoadSaveGame -= HandleOnLoadSaveGame;
		}

		private void OnGameOver(Object sender, EventArgs e)
		{
		}

		public void Show()
		{
			NGUITools.SetActiveSelf(gameObject, true);
			m_gameOverTime = 0f;
			m_labelPart1.alpha = 0f;
			m_labelPart2.alpha = 0f;
			m_label2Delayed = true;
			m_label1Delayed = true;
			AudioController.Play(m_AudioID);
		}

		public void Hide()
		{
		}

		public void Update()
		{
			m_gameOverTime += Time.deltaTime;
			if (m_label1Delayed && m_gameOverTime > m_label1FadeDelay)
			{
				TweenAlpha.Begin(m_labelPart1.gameObject, m_label1FadeTime, 1f);
				m_label1Delayed = false;
			}
			if (m_label2Delayed && m_gameOverTime > m_label2FadeDelay)
			{
				TweenAlpha.Begin(m_labelPart2.gameObject, m_label2FadeTime, 1f);
				m_label2Delayed = false;
			}
		}

		private void OnResolutionChange()
		{
			EAspectRatio easpectRatio = GraphicsConfigManager.GetAspectRatio();
			if (easpectRatio == EAspectRatio.None)
			{
				easpectRatio = EAspectRatio._4_3;
			}
			m_background.mainTexture = ((easpectRatio != EAspectRatio._4_3) ? m_backgroundTexture169 : m_backgroundTexture43);
			m_background.MakePixelPerfect();
		}

		public void OnClickedLoadLastSaveGame(GameObject p_sender)
		{
			LegacyLogic.Instance.WorldManager.LoadLastLoadedSaveGame();
		}

		public void OnClickedLoadGame(GameObject p_sender)
		{
			if (!m_saveGameMenu.IsVisible)
			{
				m_saveGameMenu.ToggleVisiblity(false);
				m_saveGameMenu.Init(true, false);
			}
			m_saveGameMenu.OnClose += SaveGameMenu_OnClose;
		}

		private void HandleOnLoadSaveGame(Object sender, EventArgs e)
		{
			ContextManager.ChangeContext(EContext.Game);
		}

		private void SaveGameMenu_OnClose(Object sender, EventArgs e)
		{
			if (m_saveGameMenu.IsVisible)
			{
				m_saveGameMenu.ToggleVisiblity(false);
			}
			m_saveGameMenu.OnClose -= SaveGameMenu_OnClose;
			m_saveGameMenu.transform.localPosition = new Vector3(m_saveGameMenu.transform.localPosition.x, m_saveGameMenu.transform.localPosition.y, m_oldLoadGamePosition);
		}

		public void OnClickedToMainMenu(GameObject p_sender)
		{
			ContextManager.ChangeContext(EContext.Mainmenu);
		}

		public void OnClickedQuit(GameObject p_sender)
		{
			Main.Instance.QuitGame();
		}
	}
}
