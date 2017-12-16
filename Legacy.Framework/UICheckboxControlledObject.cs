using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Object")]
public class UICheckboxControlledObject : MonoBehaviour
{
	public GameObject target;

	public Boolean inverse;

	private void OnEnable()
	{
		UICheckbox component = GetComponent<UICheckbox>();
		if (component != null)
		{
			OnActivate(component.isChecked);
		}
	}

	private void OnActivate(Boolean isActive)
	{
		if (target != null)
		{
			NGUITools.SetActive(target, (!inverse) ? isActive : (!isActive));
			UIPanel uipanel = NGUITools.FindInParents<UIPanel>(target);
			if (uipanel != null)
			{
				uipanel.Refresh();
			}
		}
	}
}
