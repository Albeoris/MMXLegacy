using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.TerrainTools
{
	[AddComponentMenu("MM Legacy/TerrainTools/TerrainObjectMapper")]
	public class TerrainObjectMapper : TerrainObjectMapperBase
	{
		public ObjectLayerData[] ObjectLayerConfigs = new ObjectLayerData[0];

		public GameObject[] PlaceObjectLayers()
		{
			List<GameObject> list = new List<GameObject>();
			for (Int32 i = 0; i < ObjectLayerConfigs.Length; i++)
			{
				List<Vector2> layerCells = GetLayerCells(ObjectLayerConfigs[i].SplatLayerID, ObjectLayerConfigs[i].IntensityThreshold, ObjectLayerConfigs[i].Distance, ObjectLayerConfigs[i].DistanceNoise);
				List<Vector2> p_positions = layerCells;
				GameObject[] collection = PlaceObjectOnPositions(p_positions, ObjectLayerConfigs[i]);
				list.AddRange(collection);
			}
			return list.ToArray();
		}

		public GameObject[] PlaceObjectLayer(ObjectLayerData p_layerData)
		{
			List<GameObject> list = new List<GameObject>();
			List<Vector2> layerCells = GetLayerCells(p_layerData.SplatLayerID, p_layerData.IntensityThreshold, p_layerData.Distance, p_layerData.DistanceNoise);
			List<Vector2> list2 = layerCells;
			GameObject[] collection = new GameObject[list2.Count];
			collection = PlaceObjectOnPositions(list2, p_layerData);
			list.AddRange(collection);
			return list.ToArray();
		}

		private GameObject[] PlaceObjectOnPositions(List<Vector2> p_positions, ObjectLayerData p_objectLayerConfig)
		{
			List<GameObject> list = new List<GameObject>();
			for (Int32 i = 0; i < p_positions.Count; i++)
			{
				Single num = p_positions[i].y / GetTerrainData().size.z;
				Single num2 = p_positions[i].x / GetTerrainData().size.x;
				Single interpolatedHeight = GetTerrainData().GetInterpolatedHeight(num, num2);
				Vector3 interpolatedNormal = GetTerrainData().GetInterpolatedNormal(num, num2);
				if (interpolatedHeight <= p_objectLayerConfig.MaxHeight && interpolatedHeight >= p_objectLayerConfig.MinHeight)
				{
					Single time = GetTerrainData().GetAlphamaps(Mathf.RoundToInt(num * GetTerrainData().alphamapWidth), Mathf.RoundToInt(num2 * GetTerrainData().alphamapHeight), 1, 1)[0, 0, p_objectLayerConfig.SplatLayerID];
					Single num3 = 0f;
					foreach (ObjectInstanceData objectInstanceData in p_objectLayerConfig.ObjectInstanceArray)
					{
						num3 += objectInstanceData.IntensityDistribution.Evaluate(time);
					}
					if (num3 < 1.0)
					{
						num3 = 1f;
					}
					ObjectInstanceData objectInstanceData2 = null;
					Single num4 = UnityEngine.Random.Range(0f, num3);
					foreach (ObjectInstanceData objectInstanceData3 in p_objectLayerConfig.ObjectInstanceArray)
					{
						Single num5 = objectInstanceData3.IntensityDistribution.Evaluate(time);
						if (num4 < num5)
						{
							objectInstanceData2 = objectInstanceData3;
							break;
						}
						num4 -= num5;
					}
					if (objectInstanceData2 != null)
					{
						Single d = objectInstanceData2.IntensityScale.Evaluate(time);
						if (GetTerrainData().GetSteepness(num, num2) <= p_objectLayerConfig.MaxSteepness)
						{
							GameObject gameObject = (GameObject)Instantiate(objectInstanceData2.ObjectReference);
							gameObject.transform.position = new Vector3(num * GetTerrainData().size.x, interpolatedHeight, num2 * GetTerrainData().size.z) + GetTerrain().transform.position;
							gameObject.transform.localScale = Vector3.one * (p_objectLayerConfig.Scale * (1f + UnityEngine.Random.Range(-p_objectLayerConfig.ScaleVariation, p_objectLayerConfig.ScaleVariation))) * d;
							gameObject.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0f, UnityEngine.Random.Range(-objectInstanceData2.RandRotationValue, objectInstanceData2.RandRotationValue), 0f);
							gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, gameObject.transform.rotation * Quaternion.FromToRotation(gameObject.transform.up, interpolatedNormal), objectInstanceData2.TerrainNormalInfluence);
							gameObject.transform.parent = p_objectLayerConfig.Parent;
							list.Add(gameObject);
						}
					}
				}
			}
			return list.ToArray();
		}

		[Serializable]
		public class ObjectInstanceData
		{
			public Boolean Active;

			public GameObject ObjectReference;

			public AnimationCurve IntensityDistribution;

			public AnimationCurve IntensityScale;

			public Single TerrainNormalInfluence;

			public Single RandRotationValue;

			public ObjectInstanceData()
			{
				Active = false;
				ObjectReference = null;
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
				TerrainNormalInfluence = 0.5f;
				RandRotationValue = 0f;
			}
		}

		[Serializable]
		public class ObjectLayerData : LayerData
		{
			public ObjectInstanceData[] ObjectInstanceArray;

			public Single HeightOffset;

			public Transform Parent;

			public ObjectLayerData()
			{
				Active = false;
				Distance = 10f;
				DistanceNoise = 0.8f;
				SplatLayerID = 0;
				ObjectInstanceArray = new ObjectInstanceData[0];
				MinHeight = 0f;
				MaxHeight = 100f;
				MaxSteepness = 75f;
				IntensityThreshold = 0.2f;
				Scale = 1f;
				ScaleVariation = 0.3f;
				ColorHighValues = Color.white;
				ColorLowValues = Color.gray;
				HeightOffset = 0f;
			}
		}
	}
}
