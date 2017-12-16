using System;
using UnityEngine;

public class ColormapPaintingToolU4 : MonoBehaviour
{
	public Boolean Painting;

	public Boolean Grabbing;

	public Color terrainPaintingColor;

	public Texture2D tempColormap;

	public Texture2D newColormap;

	public String oldFileName;

	public Single BrushSize = 4f;

	public Single Hardness = 0.5f;

	public Single Opacity = 0.5f;

	public String TerrainPainterMessage = String.Empty;

	public String TerrainPainterMessage1 = String.Empty;
}
