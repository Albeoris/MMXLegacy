using System;
using UnityEngine;

public class MeshTerrainBlendingTool : MonoBehaviour
{
	public Boolean Painting;

	public Boolean hideWireframe;

	public Boolean bakeForStaticBatching;

	public String oldFileName;

	public Single BrushSize = 4f;

	public Single Opacity = 0.5f;

	public Single Strength = 0.5f;

	public Boolean TextureBlend = true;

	public Boolean NormalBlend = true;

	public String vertexPainterMessage = String.Empty;

	public String vertexPainterMessage1 = String.Empty;
}
