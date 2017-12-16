using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.TerrainTools
{
	[RequireComponent(typeof(Terrain))]
	public class TerrainObjectMapperBase : MonoBehaviour
	{
		public TerrainData mParentTerrainData;

		public Terrain mParentTerrain;

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

		protected List<Vector2> GetLayerCells(Int32 p_layerID, Single p_intensityThreshold, Single p_distance, Single p_distanceNoise)
		{
			List<Vector2> list = new List<Vector2>();
			Single[,,] alphamaps = GetTerrainData().GetAlphamaps(0, 0, GetTerrainData().alphamapWidth, GetTerrainData().alphamapHeight);
			Single num = GetTerrainData().size.x / GetTerrainData().alphamapWidth;
			Single num2 = GetTerrainData().size.z / GetTerrainData().alphamapHeight;
			for (Single num3 = 0f; num3 < GetTerrainData().size.x; num3 += p_distance)
			{
				for (Single num4 = 0f; num4 < GetTerrainData().size.z; num4 += p_distance)
				{
					Single num5 = Mathf.Clamp(num3 + UnityEngine.Random.Range(-p_distanceNoise, p_distanceNoise) * p_distance, 0f, GetTerrainData().size.x);
					Single num6 = Mathf.Clamp(num4 + UnityEngine.Random.Range(-p_distanceNoise, p_distanceNoise) * p_distance, 0f, GetTerrainData().size.z);
					if (Mathf.RoundToInt(num5 / num) < GetTerrainData().alphamapWidth && Mathf.RoundToInt(num6 / num2) < GetTerrainData().alphamapHeight)
					{
						if (alphamaps[Mathf.RoundToInt(num5 / num), Mathf.RoundToInt(num6 / num2), p_layerID] > p_intensityThreshold)
						{
							list.Add(new Vector2(num5, num6));
						}
					}
				}
			}
			return list;
		}

		[Serializable]
		public class LayerData
		{
			public Boolean Active;

			public Boolean GlobalOptionsActive;

			public Single Distance;

			public Single DistanceNoise;

			public Int32 SplatLayerID;

			public Single MinHeight;

			public Single MaxHeight;

			public Single MaxSteepness;

			public Single IntensityThreshold;

			public Single Scale;

			public Single ScaleVariation;

			public Color ColorHighValues;

			public Color ColorLowValues;

			public LayerData()
			{
				Active = false;
				GlobalOptionsActive = false;
				Distance = 10f;
				DistanceNoise = 0.8f;
				SplatLayerID = 0;
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
