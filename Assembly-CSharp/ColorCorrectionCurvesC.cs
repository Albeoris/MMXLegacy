using System;
using UnityEngine;

[ExecuteInEditMode]
public class ColorCorrectionCurvesC : PostEffectsBaseC
{
	public AnimationCurve redChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve greenChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve blueChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public Boolean useDepthCorrection;

	public AnimationCurve zCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve depthRedChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve depthGreenChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve depthBlueChannel = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private Material ccMaterial;

	private Material ccDepthMaterial;

	private Material selectiveCcMaterial;

	private Texture2D rgbChannelTex;

	private Texture2D rgbDepthChannelTex;

	private Texture2D zCurveTex;

	public Single saturation = 1f;

	public Boolean selectiveCc;

	public Color selectiveFromColor = Color.white;

	public Color selectiveToColor = Color.white;

	public ColorCorrectionMode mode;

	public Boolean updateTextures = true;

	public Shader colorCorrectionCurvesShader;

	public Shader simpleColorCorrectionCurvesShader;

	public Shader colorCorrectionSelectiveShader;

	private Boolean updateTexturesOnStartup = true;

	private void Awake()
	{
		colorCorrectionCurvesShader = Shader.Find("Hidden/ColorCorrectionCurves");
		simpleColorCorrectionCurvesShader = Shader.Find("Hidden/ColorCorrectionCurvesSimple");
		colorCorrectionSelectiveShader = Shader.Find("Hidden/Color Correction Effect");
	}

	protected override void Start()
	{
		base.Start();
		updateTexturesOnStartup = true;
	}

	protected override Boolean CheckResources()
	{
		CheckSupport(mode == ColorCorrectionMode.Advanced);
		ccMaterial = CheckShaderAndCreateMaterial(simpleColorCorrectionCurvesShader, ccMaterial);
		ccDepthMaterial = CheckShaderAndCreateMaterial(colorCorrectionCurvesShader, ccDepthMaterial);
		selectiveCcMaterial = CheckShaderAndCreateMaterial(colorCorrectionSelectiveShader, selectiveCcMaterial);
		if (!rgbChannelTex)
		{
			rgbChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
		}
		if (!rgbDepthChannelTex)
		{
			rgbDepthChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
		}
		if (!zCurveTex)
		{
			zCurveTex = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);
		}
		rgbChannelTex.hideFlags = HideFlags.DontSave;
		rgbDepthChannelTex.hideFlags = HideFlags.DontSave;
		zCurveTex.hideFlags = HideFlags.DontSave;
		rgbChannelTex.wrapMode = TextureWrapMode.Clamp;
		rgbDepthChannelTex.wrapMode = TextureWrapMode.Clamp;
		zCurveTex.wrapMode = TextureWrapMode.Clamp;
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	public void UpdateParameters()
	{
		if (redChannel != null && greenChannel != null && blueChannel != null)
		{
			for (Single num = 0f; num <= 1f; num += 0.003921569f)
			{
				Single num2 = Mathf.Clamp(redChannel.Evaluate(num), 0f, 1f);
				Single num3 = Mathf.Clamp(greenChannel.Evaluate(num), 0f, 1f);
				Single num4 = Mathf.Clamp(blueChannel.Evaluate(num), 0f, 1f);
				rgbChannelTex.SetPixel(Mathf.FloorToInt(num * 255f), 0, new Color(num2, num2, num2));
				rgbChannelTex.SetPixel(Mathf.FloorToInt(num * 255f), 1, new Color(num3, num3, num3));
				rgbChannelTex.SetPixel(Mathf.FloorToInt(num * 255f), 2, new Color(num4, num4, num4));
				Single num5 = Mathf.Clamp(zCurve.Evaluate(num), 0f, 1f);
				zCurveTex.SetPixel(Mathf.FloorToInt(num * 255f), 0, new Color(num5, num5, num5));
				num2 = Mathf.Clamp(depthRedChannel.Evaluate(num), 0f, 1f);
				num3 = Mathf.Clamp(depthGreenChannel.Evaluate(num), 0f, 1f);
				num4 = Mathf.Clamp(depthBlueChannel.Evaluate(num), 0f, 1f);
				rgbDepthChannelTex.SetPixel(Mathf.FloorToInt(num * 255f), 0, new Color(num2, num2, num2));
				rgbDepthChannelTex.SetPixel(Mathf.FloorToInt(num * 255f), 1, new Color(num3, num3, num3));
				rgbDepthChannelTex.SetPixel(Mathf.FloorToInt(num * 255f), 2, new Color(num4, num4, num4));
			}
			rgbChannelTex.Apply();
			rgbDepthChannelTex.Apply();
			zCurveTex.Apply();
		}
	}

	private void UpdateTextures()
	{
		UpdateParameters();
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (updateTexturesOnStartup)
		{
			UpdateParameters();
			updateTexturesOnStartup = false;
		}
		if (useDepthCorrection)
		{
			camera.depthTextureMode |= DepthTextureMode.Depth;
		}
		RenderTexture renderTexture = destination;
		if (selectiveCc)
		{
			renderTexture = RenderTexture.GetTemporary(source.width, source.height);
		}
		if (useDepthCorrection)
		{
			ccDepthMaterial.SetTexture("_RgbTex", rgbChannelTex);
			ccDepthMaterial.SetTexture("_ZCurve", zCurveTex);
			ccDepthMaterial.SetTexture("_RgbDepthTex", rgbDepthChannelTex);
			ccDepthMaterial.SetFloat("_Saturation", saturation);
			Graphics.Blit(source, renderTexture, ccDepthMaterial);
		}
		else
		{
			ccMaterial.SetTexture("_RgbTex", rgbChannelTex);
			ccMaterial.SetFloat("_Saturation", saturation);
			Graphics.Blit(source, renderTexture, ccMaterial);
		}
		if (selectiveCc)
		{
			selectiveCcMaterial.SetColor("selColor", selectiveFromColor);
			selectiveCcMaterial.SetColor("targetColor", selectiveToColor);
			Graphics.Blit(renderTexture, destination, selectiveCcMaterial);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	public enum ColorCorrectionMode
	{
		Simple,
		Advanced
	}
}
