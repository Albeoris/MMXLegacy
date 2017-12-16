using System;
using UnityEngine;

public class turn : MonoBehaviour
{
	public Single rotationspeed = 1f;

	private void Update()
	{
		transform.Rotate(0f, 0f, rotationspeed);
	}
}
