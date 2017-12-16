using System;
using UnityEngine;

namespace Legacy
{
	public class AnimShowHideFX : MonoBehaviour
	{
		public void Show()
		{
			if (renderer != null)
			{
				renderer.enabled = true;
			}
			else
			{
				Debug.LogError("AnimShowHideFX: no renderer attached!");
			}
		}

		public void Hide()
		{
			if (renderer != null)
			{
				renderer.enabled = false;
			}
			else
			{
				Debug.LogError("AnimShowHideFX: no renderer attached!");
			}
		}
	}
}
