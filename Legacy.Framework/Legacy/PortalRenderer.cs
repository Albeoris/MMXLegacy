using System;
using UnityEngine;

namespace Legacy
{
	public class PortalRenderer : MonoBehaviour
	{
		public Texture Texture;

		private void Start()
		{
			if (Texture != null)
			{
				renderer.material.SetTexture("_RenderTex", Texture);
				enabled = false;
			}
		}

		private void Update()
		{
			if (Texture != null)
			{
				renderer.material.SetTexture("_RenderTex", Texture);
			}
		}
	}
}
