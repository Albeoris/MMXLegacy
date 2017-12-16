using System;
using Legacy.Audio;
using Legacy.Core.Utilities.Configuration;
using UnityEngine;

namespace Legacy.Configuration
{
	public class SoundSettings
	{
		public Single MusicVolume { get; set; }

		public Single SFXVolume { get; set; }

		public Single PartyBarkVolume { get; set; }

		public Boolean EnableMonsterSounds { get; set; }

		public Boolean EnableAmbientSounds { get; set; }

		public void Load(ConfigReader p_reader)
		{
			MusicVolume = p_reader["sound"]["musicVolume"].GetFloat();
			SFXVolume = p_reader["sound"]["sfxVolume"].GetFloat();
			PartyBarkVolume = p_reader["sound"]["partyBarkVolume"].GetFloat();
			EnableMonsterSounds = p_reader["sound"]["enableMonsterSounds"].GetBool(true);
			EnableAmbientSounds = p_reader["sound"]["enableAmbientSounds"].GetBool(true);
		}

		public Boolean HasUnsavedChanges(ConfigReader p_reader)
		{
			Boolean flag = false;
			flag |= (Mathf.Abs(MusicVolume - p_reader["sound"]["musicVolume"].GetFloat()) > 0.001f);
			flag |= (Mathf.Abs(SFXVolume - p_reader["sound"]["sfxVolume"].GetFloat()) > 0.001f);
			flag |= (Mathf.Abs(PartyBarkVolume - p_reader["sound"]["partyBarkVolume"].GetFloat()) > 0.001f);
			flag |= (EnableMonsterSounds != p_reader["sound"]["enableMonsterSounds"].GetBool());
			return flag | EnableAmbientSounds != p_reader["sound"]["enableAmbientSounds"].GetBool();
		}

		public void Save(ConfigReader p_reader)
		{
			p_reader["sound"]["musicVolume"].SetValue(MusicVolume);
			p_reader["sound"]["sfxVolume"].SetValue(SFXVolume);
			p_reader["sound"]["partyBarkVolume"].SetValue(PartyBarkVolume);
			p_reader["sound"]["enableMonsterSounds"].SetValue(EnableMonsterSounds);
			p_reader["sound"]["enableAmbientSounds"].SetValue(EnableAmbientSounds);
		}

		public void Apply()
		{
			AudioHelper.SetVolume(SFXVolume, PartyBarkVolume, MusicVolume);
		}
	}
}
