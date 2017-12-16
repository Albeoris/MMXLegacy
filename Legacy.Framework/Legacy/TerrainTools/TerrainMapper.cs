using System;
using UnityEngine;

namespace Legacy.TerrainTools
{
	[AddComponentMenu("MM Legacy/TerrainTools/TerrainMapper")]
	[RequireComponent(typeof(Terrain))]
	public class TerrainMapper : MonoBehaviour
	{
		public TerrainData mParentTerrainData;

		public Terrain mParentTerrain;

		public BlendDataEntry[] LayerBlendDataArray = new BlendDataEntry[0];

		public Single BlendDelta = 0.1f;

		public Single HeightNoise = 1f;

		public Single HeightNoiseScale = 1f;

		public TerrainData GetTerrainData()
		{
			if (mParentTerrainData == null)
			{
				mParentTerrainData = GetComponent<Terrain>().terrainData;
			}
			return mParentTerrainData;
		}

		public Terrain GetTerrain()
		{
			if (mParentTerrain == null)
			{
				mParentTerrain = GetComponent<Terrain>();
			}
			return mParentTerrain;
		}

		public Single[,,] GenerateAlphaMaps()
		{
			TerrainData terrainData = GetTerrainData();
			Array.Sort<BlendDataEntry>(LayerBlendDataArray, (BlendDataEntry obj1, BlendDataEntry obj2) => obj1.MaxHeight.CompareTo(obj2.MaxHeight));
			for (Int32 i = 0; i < LayerBlendDataArray.Length; i++)
			{
				LayerBlendDataArray[i].MinHeight = ((i <= 0) ? 0f : (LayerBlendDataArray[i - 1].MaxHeight - BlendDelta));
			}
			Single[,,] alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
			for (Int32 j = 0; j < terrainData.alphamapWidth; j++)
			{
				for (Int32 k = 0; k < terrainData.alphamapHeight; k++)
				{
					Single[] layerValuesAtCoordinates = GetLayerValuesAtCoordinates(j, k);
					Int32 num = 0;
					while (num < layerValuesAtCoordinates.Length && num < terrainData.alphamapLayers)
					{
						alphamaps[k, j, num] = layerValuesAtCoordinates[num];
						num++;
					}
				}
			}
			return alphamaps;
		}

		public Single[] GetLayerValuesAtCoordinates(Int32 p_x, Int32 p_y)
		{
			Single num = GetTerrainData().size.x / GetTerrainData().alphamapWidth;
			Single num2 = GetTerrainData().size.z / GetTerrainData().alphamapHeight;
			Vector3 worldPosition = new Vector3(GetTerrain().GetPosition().x + num * p_x, 0f, GetTerrain().GetPosition().z + num2 * p_y);
			Single num3 = GetTerrain().SampleHeight(worldPosition);
			Single num4 = (Mathf.PerlinNoise(worldPosition.x * HeightNoiseScale, worldPosition.z * HeightNoiseScale) * 2f - 1f) * HeightNoise;
			return ResolveLayerValuesByHeight(Mathf.Clamp(num3 + num4, 0f, GetTerrainData().size.y), p_x / (Single)GetTerrainData().alphamapWidth, p_y / (Single)GetTerrainData().alphamapHeight);
		}

		private Single[] ResolveLayerValuesByHeight(Single p_height, Single p_x, Single p_y)
		{
			Single[] array = new Single[GetTerrainData().alphamapLayers];
			for (Int32 i = 0; i < array.Length; i++)
			{
				array[i] = 0f;
			}
			for (Int32 j = 0; j < LayerBlendDataArray.Length; j++)
			{
				if (p_height >= LayerBlendDataArray[j].MinHeight && p_height <= LayerBlendDataArray[j].MaxHeight)
				{
					Single num = 1f;
					if (p_height - LayerBlendDataArray[j].MinHeight < BlendDelta && j > 0)
					{
						num = (p_height - LayerBlendDataArray[j].MinHeight) / BlendDelta;
					}
					else if (LayerBlendDataArray[j].MaxHeight - p_height < BlendDelta && j < LayerBlendDataArray.Length - 1)
					{
						num = (LayerBlendDataArray[j].MaxHeight - p_height) / BlendDelta;
					}
					if (LayerBlendDataArray[j].AltMode != AlternativeLayerBlendMode.NONE)
					{
						Single steepness = GetTerrainData().GetSteepness(p_x, p_y);
						array[LayerBlendDataArray[j].BaseLayerID] += num * LayerBlendDataArray[j].GetBaseLayerStrength(steepness, p_x, p_y);
						array[LayerBlendDataArray[j].AltLayerID] += num * LayerBlendDataArray[j].GetAltLayerStrength(steepness, p_x, p_y);
					}
					else
					{
						array[LayerBlendDataArray[j].BaseLayerID] += num;
					}
				}
			}
			return array;
		}

		private BlendDataEntry GetLayerDataByHeight(Single p_height)
		{
			Int32 num = 0;
			Single num2 = Single.MaxValue;
			for (Int32 i = 0; i < LayerBlendDataArray.Length; i++)
			{
				if (LayerBlendDataArray[i].MaxHeight >= p_height && LayerBlendDataArray[i].MaxHeight < num2)
				{
					num2 = LayerBlendDataArray[i].MaxHeight;
					num = i;
				}
			}
			return LayerBlendDataArray[num];
		}

		public enum AlternativeLayerBlendMode
		{
			NONE,
			NOISE,
			CLIFF_BLEND,
			CLIFF_THRESHOLD
		}

		[Serializable]
		public class BlendDataEntry
		{
			public Int32 BaseLayerID;

			public Int32 AltLayerID;

			public AlternativeLayerBlendMode AltMode;

			public Single SteepnessThreshold;

			public Single NoiseScale;

			public Single NoiseTexIntensity;

			public Single MaxHeight;

			public Single MinHeight;

			public Single GetBaseLayerStrength(Single p_steepness, Single p_x, Single p_y)
			{
				switch (AltMode)
				{
				case AlternativeLayerBlendMode.NOISE:
					return 1f - Mathf.Clamp01(Mathf.PerlinNoise(p_x * NoiseScale, p_y * NoiseScale) * NoiseTexIntensity);
				case AlternativeLayerBlendMode.CLIFF_BLEND:
					return 1f - Mathf.Clamp(p_steepness - SteepnessThreshold, 0f, 80f - SteepnessThreshold) / (80f - SteepnessThreshold);
				case AlternativeLayerBlendMode.CLIFF_THRESHOLD:
					return (p_steepness < SteepnessThreshold) ? 1 : 0;
				default:
					return 1f;
				}
			}

			public Single GetAltLayerStrength(Single p_steepness, Single p_x, Single p_y)
			{
				switch (AltMode)
				{
				case AlternativeLayerBlendMode.NOISE:
					return Mathf.Clamp01(Mathf.PerlinNoise(p_x * NoiseScale, p_y * NoiseScale) * NoiseTexIntensity);
				case AlternativeLayerBlendMode.CLIFF_BLEND:
					return Mathf.Clamp(p_steepness - SteepnessThreshold, 0f, 80f - SteepnessThreshold) / (80f - SteepnessThreshold);
				case AlternativeLayerBlendMode.CLIFF_THRESHOLD:
					return (p_steepness < SteepnessThreshold) ? 0 : 1;
				default:
					return 0f;
				}
			}
		}
	}
}
