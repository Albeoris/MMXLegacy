using System;
using UnityEngine;

public class Stop_Emit_Timed : MonoBehaviour
{
	public Single Stop_in = 3f;

	private Single time;

	private void Start()
	{
		time = Time.time;
	}

	private void Update()
	{
		if (Time.time > time + Stop_in && particleSystem.enableEmission)
		{
			particleSystem.enableEmission = false;
		}
	}
}
