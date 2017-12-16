using System;
using UnityEngine;

public class DetachChild : MonoBehaviour
{
	private Transform Child;

	private Vector3 pos;

	private Boolean first = true;

	private void Start()
	{
		pos = transform.position;
	}

	private void Update()
	{
		if (first)
		{
			Child = transform.GetChild(0);
			transform.DetachChildren();
			Child.transform.position = pos;
			first = false;
		}
	}
}
