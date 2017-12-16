using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("MM Legacy/TerrainTools/TerrainSingleTreePlacer")]
public class TerrainSingleTreePlacer : MonoBehaviour
{
	public Int32 PrototypeID;

	public Single WidthScale;

	public Single HeightScale;

	public Color Color;

	public Single TreeMinimumDistance = 0.001f;

	[ContextMenu("CleanUpDoubleTrees")]
	private void CleanUp()
	{
		Terrain component = gameObject.GetComponent<Terrain>();
		List<Int32> list = new List<Int32>();
		List<TreeData> list2 = new List<TreeData>();
		Debug.Log("All Trees: " + component.terrainData.treeInstances.Length);
		for (Int32 i = 0; i < component.terrainData.treeInstances.Length; i++)
		{
			if (component.terrainData.treeInstances[i].prototypeIndex == PrototypeID)
			{
				list2.Add(new TreeData
				{
					Tree = component.terrainData.treeInstances[i],
					TreeIndex = i
				});
			}
		}
		Debug.Log("Checking Trees: " + list2.Count);
		for (Int32 j = 0; j < list2.Count; j++)
		{
			for (Int32 k = j; k < list2.Count; k++)
			{
				if (j != k)
				{
					TreeInstance tree = list2[j].Tree;
					TreeInstance tree2 = list2[k].Tree;
					if ((tree.position - tree2.position).sqrMagnitude < TreeMinimumDistance && !list.Contains(list2[k].TreeIndex))
					{
						list.Add(list2[k].TreeIndex);
					}
				}
			}
		}
		list.Sort();
		list.Reverse();
		Debug.Log("Found " + list.Count + " double trees");
		List<TreeInstance> list3 = new List<TreeInstance>();
		for (Int32 l = 0; l < component.terrainData.treeInstances.Length; l++)
		{
			if (!list.Contains(l))
			{
				list3.Add(component.terrainData.treeInstances[l]);
			}
		}
		component.terrainData.treeInstances = list3.ToArray();
	}

	public struct TreeData
	{
		public TreeInstance Tree;

		public Int32 TreeIndex;
	}
}
