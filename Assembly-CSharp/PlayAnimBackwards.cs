using System;
using UnityEngine;

public class PlayAnimBackwards : MonoBehaviour
{
	private void Awake()
	{
		Animation animation = this.animation;
		AnimationState animationState = animation["TwineAnim"];
		animationState.enabled = true;
		animationState.speed = -1f;
		animationState.time = animationState.clip.length;
	}
}
