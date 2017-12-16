using System;
using UnityEngine;

public class OnDestroySpawnPrefab : MonoBehaviour
{
	public GameObject Prefab;

	private void OnDestroy()
	{
		Instantiate(Prefab, new Vector3(transform.parent.transform.parent.transform.position.x, transform.parent.transform.position.y - 2.5f, transform.parent.transform.parent.transform.position.z), transform.parent.transform.parent.transform.rotation);
	}
}
