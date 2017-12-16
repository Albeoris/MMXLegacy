using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Sound Volume")]
[RequireComponent(typeof(UISlider))]
public class UISoundVolume : MonoBehaviour
{
	private UISlider mSlider;

	private void Awake()
	{
		mSlider = GetComponent<UISlider>();
		mSlider.sliderValue = NGUITools.soundVolume;
		mSlider.eventReceiver = gameObject;
	}

	private void OnSliderChange(Single val)
	{
		NGUITools.soundVolume = val;
	}
}
