using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Edge Detection/Edge Detection Legacy")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class EdgeDetectionEffect : PostEffectsBaseC
{
	public Single SensitivityDepth = 3f;

	public Single SensitivityNormals = 0.5f;

	public Single SampleDist = 0.5f;

	public Single EdgeItensity = 0.5f;

	public Color CapColor = new Color32(50, 50, 50, 50);

	public Single ColorScale = 5f;

	public Color AddColor = new Color32(5, 5, 5, 5);

	public Single StartDistance = 0.02f;

	public Single DistanceScale = 30f;

	public Single Spread = 1f;

	public Int32 Softness = 1;

	public Single SubSample = 1f;

	public Shader edgeDetectShader;

	private Material edgeDetectMaterial;

	public Shader blurShader;

	private Material blurMaterial;

	public Shader applyShader;

	private Material applyMaterial;

	protected override Boolean CheckResources()
	{
		CheckSupport(true, false);
		edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMaterial);
		blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
		applyMaterial = CheckShaderAndCreateMaterial(applyShader, applyMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	private void SetCameraFlag()
	{
		camera.depthTextureMode |= DepthTextureMode.DepthNormals;
	}

	private void OnEnable()
	{
		isSupported = true;
		SetCameraFlag();
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		Vector2 vector = new Vector2(SensitivityDepth, SensitivityNormals);
		edgeDetectMaterial.SetVector("_Sensitivity", new Vector4(vector.x, vector.y, 1f, vector.y));
		edgeDetectMaterial.SetFloat("_EdgeItensity", EdgeItensity);
		edgeDetectMaterial.SetFloat("_SampleDistance", SampleDist);
		edgeDetectMaterial.SetFloat("_ColorScale", ColorScale);
		edgeDetectMaterial.SetFloat("_StartDistance", StartDistance);
		edgeDetectMaterial.SetFloat("_DistanceScale", DistanceScale);
		edgeDetectMaterial.SetVector("_CapColor", CapColor);
		edgeDetectMaterial.SetVector("_AddColor", AddColor);
		Int32 width = source.width;
		Int32 height = source.height;
		Single num = 1f * width / (1f * height);
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0);
		Graphics.Blit(source, renderTexture, edgeDetectMaterial);
		for (Int32 i = 0; i < Softness; i++)
		{
			RenderTexture temporary = RenderTexture.GetTemporary((Int32)(width / SubSample), (Int32)(height / SubSample), 0);
			blurMaterial.SetVector("offsets", new Vector4(0f, Spread * 0.001953125f, 0f, 0f));
			Graphics.Blit(renderTexture, temporary, blurMaterial);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
			temporary = RenderTexture.GetTemporary((Int32)(width / SubSample), (Int32)(height / SubSample), 0);
			blurMaterial.SetVector("offsets", new Vector4(Spread * 0.001953125f / num, 0f, 0f, 0f));
			Graphics.Blit(renderTexture, temporary, blurMaterial);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Graphics.Blit(source, destination);
		Graphics.Blit(renderTexture, destination, applyMaterial);
		RenderTexture.ReleaseTemporary(renderTexture);
	}
}
