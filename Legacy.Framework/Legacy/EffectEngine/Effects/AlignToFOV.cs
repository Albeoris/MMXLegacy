using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class AlignToFOV : MonoBehaviour
	{
		public EAlignment Alignment;

		private void Start()
		{
			if (Alignment == EAlignment.LEFT)
			{
				AlignOnX(-1f);
			}
			else
			{
				AlignOnX(1f);
			}
		}

		private void AlignOnX(Single p_relPos)
		{
			Camera main = Camera.main;
			Vector3 vector = main.transform.position;
			vector += main.transform.forward;
			vector += main.transform.right * 1f / (main.projectionMatrix * Vector3.right).x * p_relPos;
			transform.position = vector;
		}

		public enum EAlignment
		{
			LEFT,
			RIGHT
		}
	}
}
