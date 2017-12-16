using System;
using UnityEngine;

public class CrushingWeightLanding : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "Ground")
		{
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			ParticleSystem componentInChildren = GetComponentInChildren<ParticleSystem>();
			if (componentInChildren != null)
			{
				AudioController.Play("EarthCrushingWeightStoneOnGround", componentInChildren.transform);
				componentInChildren.transform.parent = null;
				componentInChildren.Play();
				Destroy(componentInChildren.gameObject, 5f);
			}
			Destroy(this);
		}
	}
}
