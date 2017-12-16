using System;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
	public Vector3 Axis;

	public Single Amount;

	public Space Space;

	private Transform m_trans;

	private void Awake()
	{
		m_trans = transform;
	}

	private void Update()
	{
		if (Amount != 0f)
		{
			m_trans.Rotate(Axis, Amount * Time.deltaTime, Space);
		}
	}
}
