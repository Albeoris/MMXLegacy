using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustomTerrainScriptColorMapUltraU4 : MonoBehaviour
{
	public Material TerrainMaterial;

	public Single SplattingDistance = 600f;

	public Texture2D CustomColorMap;

	public Texture2D TerrainNormalMap;

	public Texture2D SplatMap1;

	public Texture2D SplatMap2;

	public Color SpecularColor = Color.gray;

	public Texture2D customSplatMap1;

	public Texture2D customSplatMap2;

	public Color ColTex1;

	public Color ColTex2;

	public Color ColTex3;

	public Color ColTex4;

	public Color ColTex5;

	public Color ColTex6;

	public Single Elev1 = 1f;

	public Single Elev2 = 1f;

	public Single Elev3 = 1f;

	public Single Elev4 = 1f;

	public Single Elev5 = 1f;

	public Single Elev6 = 1f;

	public Single MultiUv = 0.5f;

	public Boolean BlendMultiUv = true;

	public Single DesMultiUvFac = 0.5f;

	public Single SpecPower = 1f;

	public Single Spec1 = 0.078125f;

	public Single Spec2 = 0.078125f;

	public Single Spec3 = 0.078125f;

	public Single Spec4 = 0.078125f;

	public Single Spec5 = 0.078125f;

	public Single Spec6 = 0.078125f;

	public Boolean TerrainFresnel;

	public Single FresnelIntensity = 1f;

	public Single FresnelPower = 1f;

	public Single FresnelBias = -0.5f;

	public Color ReflectionColor = Color.white;

	public Boolean AdvancedNormalBlending;

	public Texture2D SourceNormal1;

	public Texture2D SourceNormal2;

	public Texture2D SourceNormal3;

	public Texture2D SourceNormal4;

	public Texture2D SourceNormal5;

	public Texture2D SourceNormal6;

	public Texture2D CombinedNormal12;

	public Texture2D CombinedNormal34;

	public Texture2D CombinedNormal56;

	public Single Decal1_ColorCorrectionStrenght = 0.5f;

	public Single Decal2_ColorCorrectionStrenght = 0.5f;

	public Single Decal1_Sharpness = 16f;

	public Single Decal2_Sharpness = 16f;

	[HideInInspector]
	public Boolean showNewInspector = true;

	public List<Material> blendedMaterials = new List<Material>();

	[HideInInspector]
	public Boolean saveMaterial;

	[HideInInspector]
	public Boolean detailsIntro = true;

	[HideInInspector]
	public Boolean detailsBase = true;

	[HideInInspector]
	public Boolean detailsMultiuv;

	[HideInInspector]
	public Boolean detailsImportSplat = true;

	[HideInInspector]
	public Boolean detailsComNormals = true;

	[HideInInspector]
	public Boolean detailsCreateComNormals = true;

	[HideInInspector]
	public Boolean detailsColCor = true;

	[HideInInspector]
	public Boolean detailsSpecVal;

	[HideInInspector]
	public Boolean detailsMeshMat = true;

	private void Awake()
	{
		Terrain terrain = (Terrain)GetComponent(typeof(Terrain));
		terrain.basemapDistance = 100000f;
		updateColormapMaterial();
	}

	private void Start()
	{
		Terrain terrain = (Terrain)GetComponent(typeof(Terrain));
		terrain.basemapDistance = 100000f;
		updateColormapMaterial();
	}

	public void updateColormapMaterial()
	{
		Terrain terrain = (Terrain)GetComponent(typeof(Terrain));
		terrain.materialTemplate = TerrainMaterial;
		Single x = terrain.terrainData.size.x;
		if (TerrainMaterial)
		{
			if (CustomColorMap)
			{
				TerrainMaterial.SetTexture("_CustomColorMap", CustomColorMap);
			}
			if (TerrainNormalMap)
			{
				TerrainMaterial.SetTexture("_TerrainNormalMap", TerrainNormalMap);
			}
			if (SplatMap1)
			{
				TerrainMaterial.SetTexture("_Control", SplatMap1);
			}
			if (SplatMap2)
			{
				TerrainMaterial.SetTexture("_Control2nd", SplatMap2);
			}
			TerrainMaterial.SetColor("_SpecColor", SpecularColor);
			if (CombinedNormal12)
			{
				TerrainMaterial.SetTexture("_CombinedNormal12", CombinedNormal12);
			}
			if (CombinedNormal34)
			{
				TerrainMaterial.SetTexture("_CombinedNormal34", CombinedNormal34);
			}
			if (CombinedNormal56)
			{
				TerrainMaterial.SetTexture("_CombinedNormal56", CombinedNormal56);
			}
			TerrainMaterial.SetVector("_terrainCombinedFloats", new Vector4(MultiUv, DesMultiUvFac, SplattingDistance, SpecPower));
			if (BlendMultiUv)
			{
				Shader.EnableKeyword("USE_BLENDMULTIUV");
				Shader.DisableKeyword("USE_ADDDMULTIUV");
			}
			else
			{
				Shader.DisableKeyword("USE_BLENDMULTIUV");
				Shader.EnableKeyword("USE_ADDDMULTIUV");
			}
			if (TerrainFresnel)
			{
				Shader.EnableKeyword("USE_FRESNEL");
				Shader.DisableKeyword("NO_FRESNEL");
			}
			else
			{
				Shader.EnableKeyword("NO_FRESNEL");
				Shader.DisableKeyword("USE_FRESNEL");
			}
			if (AdvancedNormalBlending)
			{
				Shader.DisableKeyword("USE_STANDARDNORMALBLENDING");
				Shader.EnableKeyword("USE_ADVANCEDNORMALBLENDING");
			}
			else
			{
				Shader.EnableKeyword("USE_STANDARDNORMALBLENDING");
				Shader.DisableKeyword("USE_ADVANCEDNORMALBLENDING");
			}
			TerrainMaterial.SetColor("_ColTex1", ColTex1);
			TerrainMaterial.SetFloat("_Spec1", Spec1);
			TerrainMaterial.SetColor("_ColTex2", ColTex2);
			TerrainMaterial.SetFloat("_Spec2", Spec2);
			TerrainMaterial.SetColor("_ColTex3", ColTex3);
			TerrainMaterial.SetFloat("_Spec3", Spec3);
			TerrainMaterial.SetColor("_ColTex4", ColTex4);
			TerrainMaterial.SetFloat("_Spec4", Spec4);
			TerrainMaterial.SetColor("_ColTex5", ColTex5);
			TerrainMaterial.SetFloat("_Spec5", Spec5);
			TerrainMaterial.SetColor("_ColTex6", ColTex6);
			TerrainMaterial.SetFloat("_Spec6", Spec6);
			TerrainMaterial.SetVector("_Elev", new Vector4(Elev1, Elev2, Elev3, Elev4));
			TerrainMaterial.SetVector("_Elev1", new Vector4(Elev5, Elev6, 1f, 1f));
			TerrainMaterial.SetVector("_Fresnel", new Vector4(FresnelIntensity, FresnelPower, FresnelBias, 0f));
			TerrainMaterial.SetColor("_ReflectionColor", ReflectionColor);
			TerrainMaterial.SetVector("_DecalCombinedFloats", new Vector4(Decal1_ColorCorrectionStrenght, Decal2_ColorCorrectionStrenght, Decal1_Sharpness, Decal2_Sharpness));
			for (Int32 i = 0; i < terrain.terrainData.splatPrototypes.Length; i++)
			{
				TerrainMaterial.SetTexture("_Splat" + i.ToString(), terrain.terrainData.splatPrototypes[i].texture);
				TerrainMaterial.SetFloat("_Splat" + i.ToString() + "Tiling", x / terrain.terrainData.splatPrototypes[i].tileSize.x);
			}
		}
	}
}
