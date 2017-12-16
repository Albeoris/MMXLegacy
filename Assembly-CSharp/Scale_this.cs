using System;
using UnityEngine;

public class Scale_this : MonoBehaviour
{
	public Single speed;

	public Vector3 endscale = new Vector3(0f, 0f, 0f);

	private void Update()
	{
		transform.localScale = Vector3.Lerp(transform.localScale, endscale, Time.deltaTime * speed);
		for (Int32 i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f / transform.localScale.z);
		}
	}
}
