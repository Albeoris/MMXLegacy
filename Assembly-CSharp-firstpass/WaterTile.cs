using System;
using UnityEngine;

[ExecuteInEditMode]
public class WaterTile : MonoBehaviour
{
	public PlanarReflection reflection;

	public WaterBase waterBase;

	public void Start()
	{
		AcquireComponents();
	}

	private void AcquireComponents()
	{
		if (!reflection)
		{
			if (transform.parent)
			{
				reflection = transform.parent.GetComponent<PlanarReflection>();
			}
			else
			{
				reflection = GetComponent<PlanarReflection>();
			}
		}
		if (!waterBase)
		{
			if (transform.parent)
			{
				waterBase = transform.parent.GetComponent<WaterBase>();
			}
			else
			{
				waterBase = GetComponent<WaterBase>();
			}
		}
	}

	public void OnWillRenderObject()
	{
		if (reflection != null && reflection.enabled)
		{
			reflection.WaterTileBeingRendered(Camera.current);
		}
		if (waterBase != null && waterBase.enabled)
		{
			waterBase.WaterTileBeingRendered(Camera.current);
		}
	}
}
