using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
	public String keyName;

	private UIPopupList mList;

	private UICheckbox mCheck;

	private String key => (!String.IsNullOrEmpty(keyName)) ? keyName : ("NGUI State: " + name);

    private void Awake()
	{
		mList = GetComponent<UIPopupList>();
		mCheck = GetComponent<UICheckbox>();
		if (mList != null)
		{
			UIPopupList uipopupList = mList;
			uipopupList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Combine(uipopupList.onSelectionChange, new UIPopupList.OnSelectionChange(SaveSelection));
		}
		if (mCheck != null)
		{
			UICheckbox uicheckbox = mCheck;
			uicheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(uicheckbox.onStateChange, new UICheckbox.OnStateChange(SaveState));
		}
	}

	private void OnDestroy()
	{
		if (mCheck != null)
		{
			UICheckbox uicheckbox = mCheck;
			uicheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(uicheckbox.onStateChange, new UICheckbox.OnStateChange(SaveState));
		}
		if (mList != null)
		{
			UIPopupList uipopupList = mList;
			uipopupList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Remove(uipopupList.onSelectionChange, new UIPopupList.OnSelectionChange(SaveSelection));
		}
	}

	private void OnEnable()
	{
		if (mList != null)
		{
			String @string = PlayerPrefs.GetString(key);
			if (!String.IsNullOrEmpty(@string))
			{
				mList.selection = @string;
			}
			return;
		}
		if (mCheck != null)
		{
			mCheck.isChecked = (PlayerPrefs.GetInt(key, 1) != 0);
		}
		else
		{
			String string2 = PlayerPrefs.GetString(key);
			UICheckbox[] componentsInChildren = GetComponentsInChildren<UICheckbox>(true);
			Int32 i = 0;
			Int32 num = componentsInChildren.Length;
			while (i < num)
			{
				UICheckbox uicheckbox = componentsInChildren[i];
				uicheckbox.isChecked = (uicheckbox.name == string2);
				i++;
			}
		}
	}

	private void OnDisable()
	{
		if (mCheck == null && mList == null)
		{
			UICheckbox[] componentsInChildren = GetComponentsInChildren<UICheckbox>(true);
			Int32 i = 0;
			Int32 num = componentsInChildren.Length;
			while (i < num)
			{
				UICheckbox uicheckbox = componentsInChildren[i];
				if (uicheckbox.isChecked)
				{
					SaveSelection(uicheckbox.name);
					break;
				}
				i++;
			}
		}
	}

	private void SaveSelection(String selection)
	{
		PlayerPrefs.SetString(key, selection);
	}

	private void SaveState(Boolean state)
	{
		PlayerPrefs.SetInt(key, (!state) ? 0 : 1);
	}
}
