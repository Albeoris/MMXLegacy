using System;
using Edelweiss.DecalSystem;
using UnityEngine;

public class DS_SkinnedDecals : SkinnedDecals
{
	protected override SkinnedDecalsMeshRenderer AddSkinnedDecalsMeshRendererComponentToGameObject(GameObject a_GameObject)
	{
		return a_GameObject.AddComponent<DS_SkinnedDecalsMeshRenderer>();
	}
}
