using System;
using UnityEngine;

namespace Legacy.TerrainTools
{
	[AddComponentMenu("MM Legacy/TerrainTools/TerrainDetailMapper")]
	public class TerrainDetailMapper : TerrainObjectMapperBase
	{
		public DetailLayerData[] DetailLayerConfigs = new DetailLayerData[0];

		public void PlaceDetailLayer(DetailLayerData p_layerData, Boolean p_clear)
		{
			for (Int32 i = 0; i < p_layerData.DetailInstanceData.Length; i++)
			{
				DetailInstanceData detailInstanceData = p_layerData.DetailInstanceData[i];
				Int32[,] detailLayer = mParentTerrainData.GetDetailLayer(0, 0, mParentTerrainData.detailWidth, mParentTerrainData.detailHeight, detailInstanceData.PrototypeID);
				Single num = mParentTerrainData.alphamapWidth / (Single)mParentTerrainData.detailWidth;
				Single num2 = mParentTerrainData.alphamapHeight / (Single)mParentTerrainData.detailHeight;
				Single[,,] alphamaps = mParentTerrainData.GetAlphamaps(0, 0, mParentTerrainData.alphamapWidth, mParentTerrainData.alphamapHeight);
				for (Int32 j = 0; j < mParentTerrainData.detailWidth; j++)
				{
					for (Int32 k = 0; k < mParentTerrainData.detailHeight; k++)
					{
						if (!p_clear)
						{
							Int32 num3 = Mathf.FloorToInt(j * num);
							Int32 num4 = Mathf.FloorToInt(k * num2);
							Single num5 = alphamaps[num3, num4, p_layerData.SplatLayerID];
							if (num5 >= p_layerData.IntensityThreshold)
							{
								Single y = j / (Single)mParentTerrainData.detailWidth;
								Single x = k / (Single)mParentTerrainData.detailHeight;
								if (GetTerrainData().GetSteepness(x, y) <= p_layerData.MaxSteepness)
								{
									Single interpolatedHeight = GetTerrainData().GetInterpolatedHeight(x, y);
									if (interpolatedHeight <= p_layerData.MaxHeight && interpolatedHeight >= p_layerData.MinHeight)
									{
										if (UnityEngine.Random.value <= detailInstanceData.DistributionChance)
										{
											detailLayer[j, k] = Mathf.RoundToInt(detailInstanceData.IntensityDistribution.Evaluate(num5) * detailInstanceData.Number * p_layerData.Scale);
										}
									}
								}
							}
						}
						else
						{
							detailLayer[j, k] = 0;
						}
					}
				}
				mParentTerrainData.SetDetailLayer(0, 0, detailInstanceData.PrototypeID, detailLayer);
			}
		}

		[Serializable]
		public class DetailInstanceData
		{
			public Boolean Active;

			public Int32 PrototypeID;

			public AnimationCurve IntensityDistribution;

			public Single DistributionChance;

			public Int32 Number;

			public DetailInstanceData()
			{
				Active = false;
				PrototypeID = 0;
				IntensityDistribution = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 1f),
					new Keyframe(1f, 1f)
				});
				DistributionChance = 1f;
				Number = 1;
			}
		}

		[Serializable]
		public class DetailLayerData : LayerData
		{
			public DetailInstanceData[] DetailInstanceData;

			public DetailLayerData()
			{
				Active = false;
				Distance = 10f;
				DistanceNoise = 0.8f;
				SplatLayerID = 0;
				DetailInstanceData = new DetailInstanceData[0];
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
