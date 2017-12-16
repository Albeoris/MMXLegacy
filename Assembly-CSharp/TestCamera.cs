using System;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
	[SerializeField]
	private Bounds m_Cage = new Bounds(Vector3.zero, new Vector3(10000f, 10000f, 10000f));

	[SerializeField]
	private Single m_MaxXAngle = 80f;

	[SerializeField]
	private Single m_MinXAngle = -80f;

	private Vector3 m_Angle;

	public void Reset()
	{
		m_Angle = transform.localEulerAngles;
		m_Angle.x = Mathf.Clamp(m_Angle.x, m_MinXAngle, m_MaxXAngle);
		m_Angle.z = 0f;
	}

	private void Start()
	{
		Reset();
	}

	private void Update()
	{
		if (Input.GetMouseButton(1))
		{
			Screen.lockCursor = true;
			m_Angle.x = m_Angle.x - Input.GetAxis("Mouse Y");
			m_Angle.x = Mathf.Clamp(m_Angle.x, m_MinXAngle, m_MaxXAngle);
			m_Angle.y = m_Angle.y + Input.GetAxis("Mouse X");
			transform.localEulerAngles = m_Angle;
			Screen.lockCursor = false;
		}
		Single axis = Input.GetAxis("Horizontal");
		Single axis2 = Input.GetAxis("Vertical");
		Single axis3 = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f || axis2 != 0f || axis3 != 0f)
		{
			Vector3 vector = transform.rotation * new Vector3(axis, 0f, axis2);
			vector.y -= axis3 * 2f;
			vector = transform.position + vector;
			if (!m_Cage.Contains(vector))
			{
				vector.x = Mathf.Clamp(vector.x, m_Cage.min.x, m_Cage.max.x);
				vector.y = Mathf.Clamp(vector.y, m_Cage.min.y, m_Cage.max.y);
				vector.z = Mathf.Clamp(vector.z, m_Cage.min.z, m_Cage.max.z);
			}
			transform.position = vector;
		}
	}

	private void OnEnable()
	{
		Reset();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(m_Cage.center, m_Cage.size);
	}
}
