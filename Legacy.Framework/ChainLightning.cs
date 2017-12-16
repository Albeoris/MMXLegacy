using System;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
	public List<GameObject> Targets = new List<GameObject>();

	public GameObject Projectile;

	private LineRenderer trail;

	private Int32 reachedTargets;

	private Int32 vertexIndex = 1;

	private Vector3 lastPos;

	public Single speed = 10f;

	public Single stayTime = 1f;

	private List<Int32> m_transformIndices = new List<Int32>();

	private List<Transform> m_transforms = new List<Transform>();

	private Boolean m_isDestroyed;

	private void Start()
	{
		Vector3 origin = transform.position;
		trail = GetComponent<LineRenderer>();
		trail.SetVertexCount(2);
		trail.SetPosition(0, origin);
		trail.SetPosition(1, Projectile.transform.position);
		m_transformIndices.Add(0);
		m_transforms.Add(transform);
		lastPos = origin;
		Targets.Sort(delegate(GameObject a, GameObject b)
		{
			Single num = Vector3.Distance(a.transform.position, origin);
			Single value = Vector3.Distance(b.transform.position, origin);
			return num.CompareTo(value);
		});
	}

	private void Update()
	{
		if (Projectile != null)
		{
			for (Int32 i = 0; i < m_transforms.Count; i++)
			{
				if (m_transforms[i] != null)
				{
					trail.SetPosition(m_transformIndices[i], m_transforms[i].position);
				}
			}
			if (reachedTargets < Targets.Count)
			{
				Transform transform = Projectile.transform;
				trail.SetPosition(vertexIndex, transform.position);
				if (Vector3.Distance(transform.position, lastPos) > 4f && Vector3.Distance(transform.position, Targets[reachedTargets].transform.position) > 3.5f)
				{
					vertexIndex++;
					trail.SetVertexCount(vertexIndex + 1);
					trail.SetPosition(vertexIndex, transform.position);
					lastPos = transform.position;
				}
				transform.LookAt(Targets[reachedTargets].transform.position);
				transform.position += transform.forward * speed;
				if (Vector3.Distance(transform.position, Targets[reachedTargets].transform.position) < 1f)
				{
					m_transformIndices.Add(vertexIndex);
					m_transforms.Add(Targets[reachedTargets].transform);
					if (reachedTargets + 1 < Targets.Count)
					{
						vertexIndex++;
						trail.SetVertexCount(vertexIndex + 1);
						trail.SetPosition(vertexIndex, transform.position);
					}
					reachedTargets++;
				}
			}
			else
			{
				Projectile.transform.position = Targets[reachedTargets - 1].transform.position;
				if (!m_isDestroyed)
				{
					m_isDestroyed = true;
					Destroy(gameObject, stayTime);
				}
			}
		}
	}
}
