using System;
using UnityEngine;

public class BloomC : PostEffectsBaseC
{
	public Single threshhold = 0.25f;

	public Single intensity = 0.75f;

	public Single blurSize = 1f;

	private Resolution resolution;

	public Int32 blurIterations = 1;

	public Int32 blurType;

	public Shader fastBloomShader;

	private Material fastBloomMaterial;

	protected override Boolean CheckResources()
	{
		CheckSupport(false);
		fastBloomMaterial = CheckShaderAndCreateMaterial(fastBloomShader, fastBloomMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	private void OnDisable()
	{
		if (fastBloomMaterial)
		{
			DestroyImmediate(fastBloomMaterial);
		}
	}

	private void Awake()
	{
		fastBloomShader = Shader.Find("Hidden/FastBloom");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		Int32 num = (resolution != Resolution.Low) ? 2 : 4;
		Single num2 = (resolution != Resolution.Low) ? 1f : 0.5f;
		fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * num2, 0f, threshhold, intensity));
		source.filterMode = FilterMode.Bilinear;
		Int32 width = source.width / num;
		Int32 height = source.height / num;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;
		Graphics.Blit(source, renderTexture, fastBloomMaterial, 1);
		Int32 num3 = (blurType != 0) ? 2 : 0;
		for (Int32 i = 0; i < blurIterations; i++)
		{
			fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * num2 + i * 1f, 0f, threshhold, intensity));
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, fastBloomMaterial, 2 + num3);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
			temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, fastBloomMaterial, 3 + num3);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		fastBloomMaterial.SetTexture("_Bloom", renderTexture);
		Graphics.Blit(source, destination, fastBloomMaterial, 0);
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	public enum Resolution
	{
		Low,
		High
	}

	public enum BlurType
	{
		Standard,
		Sgx
	}
}
