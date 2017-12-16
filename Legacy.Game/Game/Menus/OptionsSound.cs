using System;
using Legacy.Configuration;
using UnityEngine;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/OptionsSound")]
	public class OptionsSound : MonoBehaviour
	{
		[SerializeField]
		private UISlider m_musicSlider;

		[SerializeField]
		private UILabel m_musicLabel;

		[SerializeField]
		private UISlider m_soundSlider;

		[SerializeField]
		private UILabel m_soundLabel;

		[SerializeField]
		private UISlider m_partyBarkSlider;

		[SerializeField]
		private UILabel m_partyBarkLabel;

		[SerializeField]
		private UICheckbox m_enableMonsters;

		[SerializeField]
		private UICheckbox m_enableAmbient;

		private void Awake()
		{
			if (!Helper.Is64BitOperatingSystem())
			{
				NGUITools.SetActiveSelf(m_enableMonsters.gameObject, false);
				NGUITools.SetActiveSelf(m_enableAmbient.gameObject, false);
			}
		}

		private void OnEnable()
		{
			UpdateGUI();
		}

		public void UpdateGUI()
		{
			m_musicSlider.sliderValue = SoundConfigManager.Settings.MusicVolume;
			m_soundSlider.sliderValue = SoundConfigManager.Settings.SFXVolume;
			m_partyBarkSlider.sliderValue = SoundConfigManager.Settings.PartyBarkVolume;
			m_enableMonsters.isChecked = SoundConfigManager.Settings.EnableMonsterSounds;
			m_enableAmbient.isChecked = SoundConfigManager.Settings.EnableAmbientSounds;
			UpdateGUILabels();
		}

		private void UpdateGUILabels()
		{
			m_musicLabel.text = (Int32)(SoundConfigManager.Settings.MusicVolume * 100f) + "%";
			m_soundLabel.text = (Int32)(SoundConfigManager.Settings.SFXVolume * 100f) + "%";
			m_partyBarkLabel.text = (Int32)(SoundConfigManager.Settings.PartyBarkVolume * 100f) + "%";
		}

		private void OnMusicSliderChange(Single p_value)
		{
			SoundConfigManager.Settings.MusicVolume = p_value;
			SoundConfigManager.Settings.Apply();
			UpdateGUILabels();
		}

		private void OnSFXSliderChange(Single p_value)
		{
			SoundConfigManager.Settings.SFXVolume = p_value;
			SoundConfigManager.Settings.Apply();
			UpdateGUILabels();
		}

		private void OnPartyBarkSliderChange(Single p_value)
		{
			SoundConfigManager.Settings.PartyBarkVolume = p_value;
			SoundConfigManager.Settings.Apply();
			UpdateGUILabels();
		}

		private void EnableMonstersStateChanged(Boolean p_state)
		{
			SoundConfigManager.Settings.EnableMonsterSounds = p_state;
		}

		private void EnableAmbientStateChanged(Boolean p_state)
		{
			SoundConfigManager.Settings.EnableAmbientSounds = p_state;
		}
	}
}
