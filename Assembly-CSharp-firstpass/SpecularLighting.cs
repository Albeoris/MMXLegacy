using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(WaterBase))]
public class SpecularLighting : MonoBehaviour
{
	public Transform specularLight;

	private WaterBase waterBase;

	public void Start()
	{
		waterBase = (WaterBase)GetComponent(typeof(WaterBase));
	}

	public void Update()
	{
		if (!waterBase)
		{
			waterBase = (WaterBase)GetComponent(typeof(WaterBase));
		}
		if (specularLight && waterBase.sharedMaterial)
		{
			waterBase.sharedMaterial.SetVector("_WorldLightDir", specularLight.forward);
		}
	}
}
