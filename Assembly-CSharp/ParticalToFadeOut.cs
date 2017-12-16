using System;
using UnityEngine;

public class ParticalToFadeOut : MonoBehaviour
{
	private Single endTime;

	public Single time = 4f;

	private void Start()
	{
		endTime = Time.time + time;
	}

	private void Update()
	{
		if (particleSystem.startColor.a > 0f && Time.time >= endTime)
		{
			particleSystem.startColor = new Color(particleSystem.startColor.r, particleSystem.startColor.g, particleSystem.startColor.b, particleSystem.startColor.a - 10f);
		}
	}
}
