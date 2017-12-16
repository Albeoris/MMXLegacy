using System;
using UnityEngine;

namespace Legacy.Animations
{
	public class AnimatorRotater : MonoBehaviour
	{
		public AnimatorControl anim;

		private void Update()
		{
			Boolean isRotating = anim.IsRotating;
			Single direction = anim.Direction;
			Single num;
			if (direction < 0.5 && direction > -0.5f)
			{
				num = 0.1f;
			}
			else
			{
				num = 3f;
			}
			if (direction != 0f & isRotating)
			{
				if (direction < 0f)
				{
					transform.Rotate(0f, -1f * num, 0f);
				}
				else
				{
					transform.Rotate(0f, 1f * num, 0f);
				}
			}
		}
	}
}
