using System;
using UnityEngine;

public class SummonDoFadeOut : MonoBehaviour
{
	private Transform parent;

	private void Start()
	{
		parent = transform.parent;
	}

	private void Update()
	{
		if (transform.parent == null || parent != transform.parent)
		{
			particleSystem.enableEmission = false;
			Destroy(gameObject, 5f);
		}
	}
}
