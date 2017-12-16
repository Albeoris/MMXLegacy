using System;
using UnityEngine;

public class Boltmovement1 : MonoBehaviour
{
	public Single speed = 5f;

	private void FixedUpdate()
	{
		transform.position += transform.forward * speed;
	}
}
