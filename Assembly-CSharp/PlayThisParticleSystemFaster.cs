using System;
using UnityEngine;

public class PlayThisParticleSystemFaster : MonoBehaviour
{
	public Single speed = 1f;

	private void Start()
	{
		ParticleSystem[] componentsInChildren = transform.GetComponentsInChildren<ParticleSystem>();
		if (transform.particleSystem != null)
		{
			transform.particleSystem.playbackSpeed = speed;
		}
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.playbackSpeed = speed;
		}
	}
}
