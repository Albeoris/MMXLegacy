using System;
using UnityEngine;

namespace Legacy.Editor
{
	[RequireComponent(typeof(Renderer))]
	[AddComponentMenu("MM Legacy/Effects/DecalBumpTool")]
	public class DecalBumpTool : MonoBehaviour
	{
		public Texture2D BumpTexture;

		public Vector2 TextureTilingScale = Vector2.one;

		public Vector2 TextureOffset = Vector2.zero;

		public Shader ReplacementShader;

		private void Start()
		{
			foreach (Renderer renderer in GetComponents<Renderer>())
			{
				foreach (Material material in renderer.materials)
				{
					Shader replacementShader = ReplacementShader;
					if (replacementShader != null)
					{
						material.shader = replacementShader;
						material.SetTexture("_DecalBumpMap", BumpTexture);
						material.SetTextureScale("_DecalBumpMap", TextureTilingScale);
						material.SetTextureOffset("_DecalBumpMap", TextureOffset);
					}
				}
			}
			enabled = false;
		}
	}
}
