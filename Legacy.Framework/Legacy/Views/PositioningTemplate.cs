using System;
using UnityEngine;

namespace Legacy.Views
{
	public class PositioningTemplate : ScriptableObject
	{
		[SerializeField]
		private Vector3[] m_SlotPositions = Arrays<Vector3>.Empty;

		public Int32 Length => m_SlotPositions.Length;

	    public Vector3 this[Int32 index]
		{
			get => m_SlotPositions[index];
	        set
			{
				if (m_SlotPositions[index] != value)
				{
					m_SlotPositions[index] = value;
				}
			}
		}
	}
}
