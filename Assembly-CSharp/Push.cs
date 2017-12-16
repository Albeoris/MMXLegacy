using System;
using UnityEngine;

public class Push : MonoBehaviour
{
	private void Start()
	{
		Transform transform = this.transform.FindChild("CastSpot");
		if (transform != null)
		{
			this.transform.rotation = transform.rotation;
		}
	}
}
