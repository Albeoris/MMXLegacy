using System;
using System.Threading;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Game.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/OptionsMenu")]
	public class OptionsMenu : MonoBehaviour
	{
		[SerializeField]
		private TabController m_tabController;

		[SerializeField]
		private OptionsGame m_optionsGame;

		[SerializeField]
		private OptionsInput m_optionsInput;

		[SerializeField]
		private OptionsGraphics m_optionsGraphics;

		[SerializeField]
		private OptionsSound m_optionsSound;

		private Boolean m_active;

		private ECategory m_lastCategory;

		public event EventHandler CloseEvent;

		public Boolean IsVisible => m_active;

	    private void Awake()
		{
			m_tabController.TabIndexChanged += OnCategoryTabChanged;
		}

		private void OnDestroy()
		{
			m_tabController.TabIndexChanged -= OnCategoryTabChanged;
		}

		public void Open()
		{
			m_active = true;
			NGUITools.SetActiveSelf(gameObject, true);
			m_tabController.SelectTab(0, true);
		}

		public void Close()
		{
			if (HasUnsavedChanges())
			{
				String text = LocaManager.GetText("OPTIONS_CONFIRM_APPLY_CHANGES");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.APPLY_DISCARD, String.Empty, text, new PopupRequest.RequestCallback(PopupRequestCloseCallback));
			}
			else
			{
				ConfirmClose();
			}
		}

		public void ConfirmClose()
		{
			m_active = false;
			NGUITools.SetActiveSelf(gameObject, false);
			if (CloseEvent != null)
			{
				CloseEvent(this, EventArgs.Empty);
			}
		}

		private void PopupRequestCloseCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				ApplyChanges();
			}
			else
			{
				ReloadConfigs();
			}
			ConfirmClose();
		}

		public Boolean DelayOtherProcesses(PopupRequest.RequestCallback p_callbackMethod)
		{
			if (HasUnsavedChanges())
			{
				String text = LocaManager.GetText("OPTIONS_CONFIRM_APPLY_CHANGES");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.APPLY_DISCARD, String.Empty, text, p_callbackMethod);
				return true;
			}
			return false;
		}

		public void ReloadConfigs()
		{
			switch (m_lastCategory)
			{
			case ECategory.GAME:
				ConfigManager.Instance.ReloadGameOptions();
				break;
			case ECategory.INPUT:
				KeyConfigManager.ReloadConfigurations();
				break;
			case ECategory.GRAPHICS:
				GraphicsConfigManager.ReloadConfigurations();
				GraphicsConfigManager.ApplyBrightnessOrGamma();
				break;
			case ECategory.SOUND:
				SoundConfigManager.ReloadConfigurations();
				break;
			}
		}

		public void ApplyChanges()
		{
			switch (m_lastCategory)
			{
			case ECategory.GAME:
				if (ConfigManager.Instance.HasLanguageChanged())
				{
					LegacyLogic.Instance.TrackingManager.TrackLanguangeChange();
				}
				ConfigManager.Instance.WriteConfigurations();
				ApplyGameOptions();
				break;
			case ECategory.INPUT:
				KeyConfigManager.WriteConfigurations();
				LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.INPUT_CHANGED, EventArgs.Empty);
				break;
			case ECategory.GRAPHICS:
				GraphicsConfigManager.WriteConfigurations();
				GraphicsConfigManager.Apply();
				break;
			case ECategory.SOUND:
				SoundConfigManager.WriteConfigurations();
				break;
			}
		}

		private void ApplyGameOptions()
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.OPTIONS_CHANGED, EventArgs.Empty);
			LegacyLogic.Instance.WorldManager.HintManager.SetActive(ConfigManager.Instance.Options.ShowHints);
		}

		private void OnDefaultButtonClick()
		{
			String text = LocaManager.GetText("OPTIONS_CONFIRM_DEFAULT_SETTINGS");
			PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, text, new PopupRequest.RequestCallback(ResetToDefaultRequestCallback));
		}

		private void ResetToDefaultRequestCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				switch (m_lastCategory)
				{
				case ECategory.GAME:
					ConfigManager.Instance.LoadDefaultOptions();
					ApplyGameOptions();
					m_optionsGame.UpdateGUI();
					break;
				case ECategory.INPUT:
					KeyConfigManager.LoadDefaultSettings();
					m_optionsInput.UpdateGUI();
					break;
				case ECategory.GRAPHICS:
					GraphicsConfigManager.LoadDefaultSettings();
					GraphicsConfigManager.InitDefaultResolution();
					GraphicsConfigManager.WriteConfigurations();
					GraphicsConfigManager.Apply();
					GraphicsConfigManager.ApplyBrightnessOrGamma();
					m_optionsGraphics.UpdateGUI();
					break;
				case ECategory.SOUND:
					SoundConfigManager.LoadDefaultSettings();
					m_optionsSound.UpdateGUI();
					break;
				}
				ApplyChanges();
			}
		}

		private void OnCloseButtonClick()
		{
			Close();
		}

		private Boolean HasUnsavedChanges()
		{
			switch (m_lastCategory)
			{
			case ECategory.GAME:
				return ConfigManager.Instance.HasUnsavedChanges();
			case ECategory.INPUT:
				return KeyConfigManager.HasUnsavedChanges();
			case ECategory.GRAPHICS:
				return GraphicsConfigManager.HasUnsavedChanges();
			case ECategory.SOUND:
				return SoundConfigManager.HasUnsavedChanges();
			default:
				return false;
			}
		}

		private void OnCategoryTabChanged(Object p_sender, EventArgs p_args)
		{
			if (HasUnsavedChanges())
			{
				String text = LocaManager.GetText("OPTIONS_CONFIRM_APPLY_CHANGES");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.APPLY_DISCARD, String.Empty, text, new PopupRequest.RequestCallback(PopupRequestTabChangeCallback));
			}
			else
			{
				ChangeTab();
			}
		}

		private void PopupRequestTabChangeCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				ApplyChanges();
			}
			else
			{
				ReloadConfigs();
			}
			ChangeTab();
		}

		private void ChangeTab()
		{
			m_lastCategory = (ECategory)m_tabController.CurrentTabIndex;
			NGUITools.SetActiveSelf(m_optionsGame.gameObject, m_lastCategory == ECategory.GAME);
			NGUITools.SetActiveSelf(m_optionsInput.gameObject, m_lastCategory == ECategory.INPUT);
			NGUITools.SetActiveSelf(m_optionsGraphics.gameObject, m_lastCategory == ECategory.GRAPHICS);
			NGUITools.SetActiveSelf(m_optionsSound.gameObject, m_lastCategory == ECategory.SOUND);
		}

		private void SaveOptions()
		{
			ConfigManager.Instance.WriteConfigurations();
		}

		private enum ECategory
		{
			GAME,
			INPUT,
			GRAPHICS,
			SOUND
		}
	}
}
