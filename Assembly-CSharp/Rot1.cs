using System;
using UnityEngine;

public class Rot1 : MonoBehaviour
{
	private Vector3 startpos;

	private Vector3 currentpos;

	private Vector3 targetpos;

	private Vector3 lasttargetpos;

	private Single timesince;

	private void Awake()
	{
		startpos = transform.position;
		currentpos = transform.position;
		targetpos = transform.position;
		lasttargetpos = transform.position;
	}

	private void Update()
	{
		if (timesince + 0.25f < Time.time)
		{
			lasttargetpos = targetpos;
			targetpos = startpos + new Vector3(UnityEngine.Random.Range(-50, 50) / 10, UnityEngine.Random.Range(-15, 0) / 10, UnityEngine.Random.Range(-50, 50) / 10);
			timesince = Time.time;
		}
		lasttargetpos = Vector3.Lerp(lasttargetpos, targetpos, Time.deltaTime * 0.125f);
		currentpos = Vector3.Lerp(currentpos, lasttargetpos, Time.deltaTime * 0.25f);
		transform.position = currentpos;
	}
}
