using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class AdvancedLineRenderer : MonoBehaviour
	{
		[SerializeField]
		private LineRenderer m_line;

		private Vector3[] m_vertices = new Vector3[0];

		public void Start()
		{
			if (m_line == null)
			{
				Debug.LogError("AdvancedLineRenderer: LineRenderer is null!");
			}
		}

		public void SetPosition(Int32 index, Vector3 position)
		{
			m_vertices[index] = position;
			m_line.SetPosition(index, position);
		}

		public void SetVertexCount(Int32 count)
		{
			Array.Resize<Vector3>(ref m_vertices, count);
			m_line.SetVertexCount(count);
		}

		public Vector3 GetPosition(Int32 index)
		{
			return m_vertices[index];
		}

		public Int32 GetVertexCount()
		{
			return m_vertices.Length;
		}
	}
}
