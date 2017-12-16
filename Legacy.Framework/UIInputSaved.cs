using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Input (Saved)")]
public class UIInputSaved : UIInput
{
	public String playerPrefsField;

	public override String text
	{
		get => base.text;
	    set
		{
			base.text = value;
			SaveToPlayerPrefs(value);
		}
	}

	private void Awake()
	{
		onSubmit += SaveToPlayerPrefs;
		if (!String.IsNullOrEmpty(playerPrefsField) && PlayerPrefs.HasKey(playerPrefsField))
		{
			text = PlayerPrefs.GetString(playerPrefsField);
		}
	}

	private void OnDestroy()
	{
		onSubmit -= SaveToPlayerPrefs;
	}

	private void SaveToPlayerPrefs(String val)
	{
		if (!String.IsNullOrEmpty(playerPrefsField))
		{
			PlayerPrefs.SetString(playerPrefsField, val);
		}
	}

	private void OnApplicationQuit()
	{
		SaveToPlayerPrefs(text);
	}
}
