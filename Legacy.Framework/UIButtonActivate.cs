using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Activate")]
public class UIButtonActivate : MonoBehaviour
{
	public GameObject target;

	public Boolean state = true;

	private void OnClick()
	{
		if (target != null)
		{
			NGUITools.SetActive(target, state);
		}
	}
}
