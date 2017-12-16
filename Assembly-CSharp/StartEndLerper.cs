using System;
using UnityEngine;

public class StartEndLerper : MonoBehaviour
{
	public GameObject StartGO;

	public GameObject EndGO;

	public GameObject TargetGO;

	public Single Duration = 1f;

	private Single m_startTime = -1f;

	private void Start()
	{
		m_startTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup - m_startTime < Duration)
		{
			TargetGO.transform.position = Vector3.Lerp(StartGO.transform.position, EndGO.transform.position, (Time.realtimeSinceStartup - m_startTime) / Duration);
			TargetGO.transform.LookAt(EndGO.transform.position);
		}
	}
}
