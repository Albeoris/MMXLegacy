using System;
using UnityEngine;

public class TwineAnimConfig : MonoBehaviour
{
	private void Awake()
	{
		animation["TwineAnimIdle"].layer = 1;
		animation["TwineAnimIdle"].blendMode = AnimationBlendMode.Blend;
		animation["TwineAnimIdle"].weight = 1f;
		animation["TwineAnimIdle"].enabled = true;
	}

	private void Update()
	{
		if (!animation.IsPlaying("TwineAnim"))
		{
			animation["TwineAnimIdle"].enabled = false;
			Destroy(this);
		}
	}
}
