using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[RequireComponent(typeof(Camera))]
	public class CameraPerLayerDistanceCulling : MonoBehaviour
	{
		private Camera m_MyCamera;

		[SerializeField]
		private Single[] m_DistancePerLayer = new Single[32];

		public Single GetLayerDistance(Int32 bitIndex)
		{
			return m_DistancePerLayer[bitIndex];
		}

		public void SetLayerDistance(Int32 bitIndex, Single value)
		{
			if (value < 0f)
			{
				throw new ArgumentOutOfRangeException("value", value + " < 0");
			}
			m_DistancePerLayer[bitIndex] = value;
			if (m_MyCamera != null)
			{
				m_MyCamera.layerCullDistances = m_DistancePerLayer;
			}
		}

		public void LayerDistanceCopyTo(Single[] layers)
		{
			Array.Copy(layers, m_DistancePerLayer, 32);
			if (m_MyCamera != null)
			{
				m_MyCamera.layerCullDistances = m_DistancePerLayer;
			}
		}

		public void LayerDistanceCopyFrom(Single[] layers)
		{
			Array.Copy(m_DistancePerLayer, layers, 32);
			if (m_MyCamera != null)
			{
				m_MyCamera.layerCullDistances = m_DistancePerLayer;
			}
		}

		private void Awake()
		{
			m_MyCamera = this.GetComponent<Camera>(true);
			m_MyCamera.layerCullDistances = m_DistancePerLayer;
		}
	}
}
