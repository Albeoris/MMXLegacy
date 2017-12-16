using System;
using UnityEngine;

public class exploForce : MonoBehaviour
{
	public Single radius = 3f;

	public Single power = 2500f;

	private void Start()
	{
		Vector3 vector = new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z);
		Collider[] array = Physics.OverlapSphere(vector, radius);
		foreach (Collider collider in array)
		{
			if (collider.rigidbody)
			{
				collider.rigidbody.AddExplosionForce(power, vector, radius, 3f);
			}
		}
	}
}
