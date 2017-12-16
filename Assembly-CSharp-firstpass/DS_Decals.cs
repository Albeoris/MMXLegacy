using System;
using Edelweiss.DecalSystem;
using UnityEngine;

public class DS_Decals : Decals
{
	protected override DecalsMeshRenderer AddDecalsMeshRendererComponentToGameObject(GameObject a_GameObject)
	{
		return a_GameObject.AddComponent<DS_DecalsMeshRenderer>();
	}
}
