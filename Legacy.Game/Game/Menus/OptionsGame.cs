using System;
using System.Collections.Generic;
using System.Reflection;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Game.MMGUI;
using Legacy.Utilities;
using UnityEngine;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/OptionsGame")]
	public class OptionsGame : MonoBehaviour
	{
		private const Single MonsterMaximumSpeed = 3f;

		[SerializeField]
		private UICheckbox m_retroPixelation;

		[SerializeField]
		private UICheckbox m_showHints;

		[SerializeField]
		private UICheckbox m_showFresko;

		[SerializeField]
		private UICheckbox m_lockActionBar;

		[SerializeField]
		private UICheckbox m_showMonsterHPBars;

		[SerializeField]
		private UICheckbox m_showSubtitles;

		[SerializeField]
		private UISlider m_enemyOutlineOpacity;

		[SerializeField]
		private UILabel m_enemyOutlineOpacityLabel;

		[SerializeField]
		private UISlider m_objectOutlineOpacity;

		[SerializeField]
		private UILabel m_objectOutlineOpacityLabel;

		[SerializeField]
		private UIPopupList m_languages;

		[SerializeField]
		private UICheckbox m_showMovementArrows;

		[SerializeField]
		private UICheckbox m_lockCharacterOrder;

		[SerializeField]
		private UICheckbox m_fadeLogsCheckbox;

		[SerializeField]
		private UISlider m_fadeLogsDelay;

		[SerializeField]
		private UILabel m_fadeLogsDelayLabel;

		[SerializeField]
		private UILabel m_fadeLogsDelayValueLabel;

		[SerializeField]
		private Color m_disableColorText = new Color(0.5f, 0.5f, 0.5f);

		[SerializeField]
		private UISprite m_thumbSprite;

		[SerializeField]
		private UISprite m_bgSprite;

		[SerializeField]
		private UISprite m_fgSprite;

		[SerializeField]
		private Color m_disableColorSlider = new Color(0.5f, 0.5f, 0.5f);

		[SerializeField]
		private Color m_enableColorSlider = new Color(0.5f, 0.5f, 0.5f);

		[SerializeField]
		private UICheckbox m_triggerBarks;

		[SerializeField]
		private UICheckbox m_showMessages;

		[SerializeField]
		private UISlider m_logOpacity;

		[SerializeField]
		private UILabel m_logOpacityLabel;

		[SerializeField]
		private UISlider m_movementSpeed;

		[SerializeField]
		private UILabel m_movementSpeedLabel;

		[SerializeField]
		private UISlider m_minimapGridOpacity;

		[SerializeField]
		private UILabel m_minimapGridOpacityLabel;

		[SerializeField]
		private UICheckbox m_showFloatingDamageMonsters;

		[SerializeField]
		private UICheckbox m_showFloatingDamageChars;

		[SerializeField]
		private UICheckbox m_showMinimap;

		[SerializeField]
		private UICheckbox m_viewAlignedMinimap;

		[SerializeField]
		private UISlider m_tooltipDelay;

		[SerializeField]
		private UILabel m_tooltipDelayLabel;

		[SerializeField]
		private GameObject[] m_optionsAfterLanguage;

		[SerializeField]
		private GameObject m_languageGO;

		[SerializeField]
		private Vector2 m_distanceBetweenOptions = new Vector2(380f, 60f);

		private Dictionary<String, String> m_availableLanguages;

		private void Awake()
		{
			m_availableLanguages = new Dictionary<String, String>();
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_ENGLISH"), "en");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_FRENCH"), "fr");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_GERMAN"), "de");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_POLISH"), "pl");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_RUSSIAN"), "ru");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_CZECH"), "cz");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_HUNGARIAN"), "hu");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_ROMANIAN"), "ro");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_ITALIAN"), "it");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_SPANISH"), "es");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_BRAZILIAN"), "br");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_KOREAN"), "kr");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_JAPANESE"), "jp");
			m_availableLanguages.Add(LocaManager.GetText("LANGUAGE_CHINESE"), "cn");
			m_languages.items.Clear();
			foreach (String item in m_availableLanguages.Keys)
			{
				m_languages.items.Add(item);
			}
			if (!Helper.Is64BitOperatingSystem())
			{
				NGUITools.SetActiveSelf(m_triggerBarks.gameObject, false);
			}
			if (ConfigManager.Instance.Game.ChineseVersion)
			{
				NGUITools.SetActiveSelf(m_triggerBarks.gameObject, false);
				NGUITools.SetActiveSelf(m_languageGO, false);
				foreach (GameObject gameObject in m_optionsAfterLanguage)
				{
					Vector3 localPosition = gameObject.transform.localPosition;
					localPosition.y += m_distanceBetweenOptions.y;
					gameObject.transform.localPosition = localPosition;
				}
				Vector3 localPosition2 = m_showFloatingDamageMonsters.transform.localPosition;
				localPosition2.x -= m_distanceBetweenOptions.x;
				m_showFloatingDamageMonsters.transform.localPosition = localPosition2;
			}

            UpdateGUI();
		}

		private void OnEnable()
		{
			UpdateGUI();
		}

		public void UpdateGUI()
		{
			m_retroPixelation.isChecked = ConfigManager.Instance.Options.RetroMode;
			m_showHints.isChecked = LegacyLogic.Instance.WorldManager.HintManager.IsActive;
			m_showFresko.isChecked = ConfigManager.Instance.Options.ShowViewport;
			m_lockActionBar.isChecked = ConfigManager.Instance.Options.LockActionBar;
			m_showMonsterHPBars.isChecked = ConfigManager.Instance.Options.MonsterHPBarsVisible;
			m_showSubtitles.isChecked = ConfigManager.Instance.Options.SubTitles;
			m_enemyOutlineOpacity.sliderValue = ConfigManager.Instance.Options.EnemyOutlineOpacity;
			m_enemyOutlineOpacityLabel.text = (Int32)(100f * m_enemyOutlineOpacity.sliderValue) + "%";
			m_objectOutlineOpacity.sliderValue = ConfigManager.Instance.Options.ObjectOutlineOpacity;
			m_objectOutlineOpacityLabel.text = (Int32)(100f * m_objectOutlineOpacity.sliderValue) + "%";
			m_fadeLogsCheckbox.isChecked = ConfigManager.Instance.Options.FadeLogs;
			m_fadeLogsDelay.sliderValue = ConfigManager.Instance.Options.FadeLogsDelay / 10f;
			m_fadeLogsDelayValueLabel.text = ConfigManager.Instance.Options.FadeLogsDelay.ToString();
			m_fadeLogsDelayLabel.color = ((!m_fadeLogsCheckbox.isChecked) ? m_disableColorText : Color.white);
			m_fadeLogsDelayValueLabel.color = ((!m_fadeLogsCheckbox.isChecked) ? m_disableColorText : Color.white);
			m_fadeLogsDelay.enabled = m_fadeLogsCheckbox.isChecked;
			m_triggerBarks.isChecked = ConfigManager.Instance.Options.TriggerBarks;
			m_showMessages.isChecked = ConfigManager.Instance.Options.ShowGameMessages;
			m_logOpacity.sliderValue = ConfigManager.Instance.Options.LogOpacity;
			m_logOpacityLabel.text = (Int32)(100f * m_logOpacity.sliderValue) + "%";
			m_movementSpeed.sliderValue = (ConfigManager.Instance.Options.MonsterMovementSpeed - 1f) / 2f;
			m_movementSpeedLabel.text = (Int32)(100f * (1f + m_movementSpeed.sliderValue * 2f)) + "%";
			m_showFloatingDamageMonsters.isChecked = ConfigManager.Instance.Options.ShowFloatingDamageMonsters;
			m_showFloatingDamageChars.isChecked = ConfigManager.Instance.Options.ShowFloatingDamageCharacters;
			m_lockCharacterOrder.isChecked = ConfigManager.Instance.Options.LockCharacterOrder;
			foreach (KeyValuePair<String, String> keyValuePair in m_availableLanguages)
			{
				if (keyValuePair.Value == ConfigManager.Instance.Options.Language)
				{
					m_languages.selection = keyValuePair.Key;
				}
			}
			m_showMovementArrows.isChecked = ConfigManager.Instance.Options.IsLeftActionBarShowingArrows;
			m_showMinimap.isChecked = ConfigManager.Instance.Options.ShowMinimap;
			m_viewAlignedMinimap.isChecked = ConfigManager.Instance.Options.ViewAlignedMinimap;
			m_minimapGridOpacity.sliderValue = ConfigManager.Instance.Options.MinimapGirdOpacity;
			m_minimapGridOpacityLabel.text = (Int32)(100f * m_minimapGridOpacity.sliderValue) + "%";
			Single num = (Single)Math.Round(ConfigManager.Instance.Options.TooltipDelay, 1);
			m_tooltipDelayLabel.text = num.ToString();
			ConfigManager.Instance.Options.TooltipDelay = num;
			m_tooltipDelay.sliderValue = num / 2f;
		}

	    private void ViewAlignedMinimapChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.ViewAlignedMinimap = p_state;
		}

		private void RetroPixelationChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.RetroMode = p_state;
		}

		private void ShowHintsStateChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.ShowHints = p_state;
		}

		private void ShowFreskoStateChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.ShowViewport = p_state;
		}

		private void LockActionBarStateChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.LockActionBar = p_state;
		}

		private void ShowMonsterHPBarsStateChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.MonsterHPBarsVisible = p_state;
		}

		private void ShowSubtitlesStateChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.SubTitles = p_state;
		}

		private void TriggerBarkStateChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.TriggerBarks = p_state;
		}

		private void ShowMessagesStateChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.ShowGameMessages = p_state;
		}

		private void ShowMovementArrowsChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.IsLeftActionBarShowingArrows = p_state;
		}

		private void OnEnemyOutlineOpacitySliderChange(Single p_value)
		{
			ConfigManager.Instance.Options.EnemyOutlineOpacity = p_value;
			m_enemyOutlineOpacityLabel.text = (Int32)(100f * p_value) + "%";
		}

		private void OnObjectOutlineOpacitySliderChange(Single p_value)
		{
			ConfigManager.Instance.Options.ObjectOutlineOpacity = p_value;
			m_objectOutlineOpacityLabel.text = (Int32)(100f * p_value) + "%";
		}

		private void OnActionlogOpacitySliderChange(Single p_value)
		{
			ConfigManager.Instance.Options.LogOpacity = p_value;
			m_logOpacityLabel.text = (Int32)(100f * p_value) + "%";
		}

		private void ShowDamageMonstersChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.ShowFloatingDamageMonsters = p_state;
		}

		private void ShowDamageCharsChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.ShowFloatingDamageCharacters = p_state;
		}

		private void LockCharacterOrderChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.LockCharacterOrder = p_state;
		}

		private void OnLanguageChange(String p_item)
		{
			String text = m_availableLanguages[p_item];
			if (ConfigManager.Instance.Options.Language != text)
			{
				String text2 = LocaManager.GetText("OPTIONS_GAME_LANGUAGE_RESTART_INFO_TEXT");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM, String.Empty, text2, null);
			}
			ConfigManager.Instance.Options.Language = text;
		}

		private void FadeLogsStateChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.FadeLogs = p_state;
			m_fadeLogsDelay.enabled = p_state;
			m_thumbSprite.color = ((!p_state) ? m_disableColorSlider : m_enableColorSlider);
			m_fgSprite.color = ((!p_state) ? m_disableColorSlider : m_enableColorSlider);
			m_bgSprite.color = ((!p_state) ? m_disableColorSlider : m_enableColorSlider);
			m_fadeLogsDelayLabel.color = ((!p_state) ? m_disableColorText : Color.white);
			m_fadeLogsDelayValueLabel.color = ((!p_state) ? m_disableColorText : Color.white);
			m_fadeLogsDelayValueLabel.text = ConfigManager.Instance.Options.FadeLogsDelay.ToString();
		}

		private void OnFadeLogsDelaySliderChange(Single p_value)
		{
			if (m_fadeLogsDelay.enabled)
			{
				Double num = Math.Round(p_value, 2) * 10.0;
				m_fadeLogsDelayValueLabel.text = num.ToString();
				ConfigManager.Instance.Options.FadeLogsDelay = (Single)num;
			}
			else
			{
				m_fadeLogsDelay.sliderValue = ConfigManager.Instance.Options.FadeLogsDelay / 10f;
			}
		}

		private void OnMovementSpeedSliderChange(Single p_value)
		{
			Single num = (Single)Math.Round(p_value * 2f, 1) + 1f;
			m_movementSpeedLabel.text = (Int32)(num * 100f) + "%";
			ConfigManager.Instance.Options.MonsterMovementSpeed = num;
			m_movementSpeed.sliderValue = (num - 1f) / 2f;
		}

		private void OnMinimapGirdOpacitySliderChange(Single p_value)
		{
			Single num = (Single)Math.Round(p_value, 3);
			m_minimapGridOpacityLabel.text = (Int32)(num * 100f) + "%";
			ConfigManager.Instance.Options.MinimapGirdOpacity = num;
			m_minimapGridOpacity.sliderValue = num;
		}

		private void OnShowMinimapChanged(Boolean p_state)
		{
			ConfigManager.Instance.Options.ShowMinimap = p_state;
		}

		private void OnToolTipDelaySliderChange(Single p_value)
		{
			Single num = (Single)Math.Round(p_value * 2f, 1);
			m_tooltipDelayLabel.text = num.ToString();
			ConfigManager.Instance.Options.TooltipDelay = num;
			m_tooltipDelay.sliderValue = num / 2f;
		}

	    private sealed class CheckBoxDuplicate
	    {
	        public GameObject Object;
	        public BoxCollider Collider;
	        public UICheckbox Checkbox;
	        public UIButton Button;
	        public UIButtonSound ButtonSound;

	        public CheckBoxDuplicate(GameObject parentPanel, UICheckbox source)
	        {
	            Object = NGUITools.AddChild(parentPanel, source.gameObject);
	            foreach (var component in Object.GetComponents<Component>())
	            {
	                switch (component)
	                {
                        case BoxCollider boxCollider:
                            Collider = boxCollider;
                            break;
                        case UICheckbox checkBox:
                            Checkbox = checkBox;
                            break;
                        case UIButton button:
                            Button = button;
                            break;
                        case UIButtonSound buttonSound:
                            ButtonSound = buttonSound;
                            break;
                    }
	            }
            }

	        public void AddAfter(GameObject lastObject)
	        {
	            Object.transform.localPosition = lastObject.transform.localPosition + Vector3.down * 50f;
	        }

	        public void SetText(String text)
	        {
	            Button.guiText.text = text;
	        }
        }
	}
}
