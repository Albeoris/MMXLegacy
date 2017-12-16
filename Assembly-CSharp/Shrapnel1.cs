using System;
using UnityEngine;

public class Shrapnel1 : MonoBehaviour
{
	private void Awake()
	{
		transform.position += new Vector3(UnityEngine.Random.Range(-50, 50) / 10, UnityEngine.Random.Range(-15, 0) / 10, UnityEngine.Random.Range(-50, 50) / 10);
	}
}
