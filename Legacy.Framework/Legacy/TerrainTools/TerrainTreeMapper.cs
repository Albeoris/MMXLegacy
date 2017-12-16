using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.TerrainTools
{
	[AddComponentMenu("MM Legacy/TerrainTools/TerrainTreeMapper")]
	public class TerrainTreeMapper : TerrainObjectMapperBase
	{
		public TreeLayerData[] TreeLayerConfigs = new TreeLayerData[0];

		public TreeInstance[] PlaceTreeLayers()
		{
			List<TreeInstance> list = new List<TreeInstance>();
			for (Int32 i = 0; i < TreeLayerConfigs.Length; i++)
			{
				List<Vector2> layerCells = GetLayerCells(TreeLayerConfigs[i].SplatLayerID, TreeLayerConfigs[i].IntensityThreshold, TreeLayerConfigs[i].Distance, TreeLayerConfigs[i].DistanceNoise);
				List<Vector2> p_positions = layerCells;
				TreeInstance[] collection = PlaceTreeOnPositions(p_positions, TreeLayerConfigs[i]);
				list.AddRange(collection);
			}
			return list.ToArray();
		}

		public TreeInstance[] PlaceTreeLayer(TreeLayerData p_layerData)
		{
			List<TreeInstance> list = new List<TreeInstance>();
			List<Vector2> layerCells = GetLayerCells(p_layerData.SplatLayerID, p_layerData.IntensityThreshold, p_layerData.Distance, p_layerData.DistanceNoise);
			List<Vector2> list2 = layerCells;
			TreeInstance[] collection = new TreeInstance[list2.Count];
			collection = PlaceTreeOnPositions(list2, p_layerData);
			list.AddRange(collection);
			return list.ToArray();
		}

		private TreeInstance[] PlaceTreeOnPositions(List<Vector2> p_positions, TreeLayerData p_treeLayerConfig)
		{
			List<TreeInstance> list = new List<TreeInstance>();
			for (Int32 i = 0; i < p_positions.Count; i++)
			{
				TreeInstance item = default(TreeInstance);
				Single num = p_positions[i].y / GetTerrainData().size.z;
				Single num2 = p_positions[i].x / GetTerrainData().size.x;
				Single interpolatedHeight = GetTerrainData().GetInterpolatedHeight(num, num2);
				Single y = interpolatedHeight / GetTerrainData().size.y;
				if (interpolatedHeight <= p_treeLayerConfig.MaxHeight && interpolatedHeight >= p_treeLayerConfig.MinHeight)
				{
					Single time = GetTerrainData().GetAlphamaps(Mathf.RoundToInt(num * GetTerrainData().alphamapWidth), Mathf.RoundToInt(num2 * GetTerrainData().alphamapHeight), 1, 1)[0, 0, p_treeLayerConfig.SplatLayerID];
					Single num3 = 0f;
					foreach (TreeInstanceData treeInstanceData2 in p_treeLayerConfig.TreeInstanceData)
					{
						num3 += treeInstanceData2.IntensityDistribution.Evaluate(time);
					}
					if (num3 < 1.0)
					{
						num3 = 1f;
					}
					TreeInstanceData treeInstanceData3 = null;
					Single num4 = UnityEngine.Random.Range(0f, num3);
					foreach (TreeInstanceData treeInstanceData5 in p_treeLayerConfig.TreeInstanceData)
					{
						Single num5 = treeInstanceData5.IntensityDistribution.Evaluate(time);
						if (num4 < num5)
						{
							treeInstanceData3 = treeInstanceData5;
							break;
						}
						num4 -= num5;
					}
					if (treeInstanceData3 != null)
					{
						Single num6 = treeInstanceData3.IntensityScale.Evaluate(time);
						if (GetTerrainData().GetSteepness(num, num2) <= p_treeLayerConfig.MaxSteepness)
						{
							item.position = new Vector3(num, y, num2);
							item.prototypeIndex = treeInstanceData3.PrototypeID;
							Single num7 = p_treeLayerConfig.Scale * (1f + UnityEngine.Random.Range(-p_treeLayerConfig.ScaleVariation, p_treeLayerConfig.ScaleVariation)) * num6;
							item.heightScale = num7;
							item.widthScale = num7;
							item.color = Color.Lerp(p_treeLayerConfig.ColorHighValues, p_treeLayerConfig.ColorLowValues, UnityEngine.Random.value);
							item.lightmapColor = Color.white;
							list.Add(item);
						}
					}
				}
			}
			return list.ToArray();
		}

		[Serializable]
		public class TreeInstanceData
		{
			public Boolean Active;

			public Int32 PrototypeID;

			public AnimationCurve IntensityDistribution;

			public AnimationCurve IntensityScale;

			public TreeInstanceData()
			{
				Active = false;
				PrototypeID = 0;
				IntensityDistribution = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 1f),
					new Keyframe(1f, 1f)
				});
				IntensityScale = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 1f),
					new Keyframe(1f, 1f)
				});
			}
		}

		[Serializable]
		public class TreeLayerData : LayerData
		{
			public TreeInstanceData[] TreeInstanceData;

			public TreeLayerData()
			{
				Active = false;
				Distance = 10f;
				DistanceNoise = 0.8f;
				SplatLayerID = 0;
				TreeInstanceData = new TreeInstanceData[0];
				MinHeight = 0f;
				MaxHeight = 100f;
				MaxSteepness = 75f;
				IntensityThreshold = 0.2f;
				Scale = 1f;
				ScaleVariation = 0.3f;
				ColorHighValues = Color.white;
				ColorLowValues = Color.gray;
			}
		}
	}
}
