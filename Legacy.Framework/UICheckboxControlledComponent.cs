using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Component")]
public class UICheckboxControlledComponent : MonoBehaviour
{
	public MonoBehaviour target;

	public Boolean inverse;

	private Boolean mUsingDelegates;

	private void Start()
	{
		UICheckbox component = GetComponent<UICheckbox>();
		if (component != null)
		{
			mUsingDelegates = true;
			UICheckbox uicheckbox = component;
			uicheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(uicheckbox.onStateChange, new UICheckbox.OnStateChange(OnActivateDelegate));
		}
	}

	private void OnActivateDelegate(Boolean isActive)
	{
		if (enabled && target != null)
		{
			target.enabled = ((!inverse) ? isActive : (!isActive));
		}
	}

	private void OnActivate(Boolean isActive)
	{
		if (!mUsingDelegates)
		{
			OnActivateDelegate(isActive);
		}
	}
}
