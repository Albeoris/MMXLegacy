using System;
using UnityEngine;

public class OnCollisionSpawnSmallerParts : MonoBehaviour
{
	public GameObject Prefab;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "ground" && transform.parent.transform.localScale.x > 1f)
		{
			Vector3 center = renderer.bounds.center;
			GameObject gameObject = (GameObject)Instantiate(Prefab, center, transform.rotation);
			gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x / 4f, gameObject.transform.localScale.y / 4f, gameObject.transform.localScale.z / 2f);
			exploForce component = gameObject.GetComponent<exploForce>();
			component.power = 1000f;
			component.radius = 1.5f;
			if (gameObject.transform.localScale.x < 1f)
			{
				OnCollisionSpawnSmallerParts component2 = gameObject.GetComponent<OnCollisionSpawnSmallerParts>();
				Destroy(component2);
			}
			Destroy(this);
		}
	}
}
