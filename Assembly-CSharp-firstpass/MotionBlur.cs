using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Blur/Motion Blur (Color Accumulation)")]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MotionBlur : ImageEffectBase
{
	public Single blurAmount = 0.8f;

	public Boolean extraBlur;

	private RenderTexture accumTexture;

	protected override void Start()
	{
		if (!SystemInfo.supportsRenderTextures)
		{
			enabled = false;
			return;
		}
		base.Start();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		DestroyImmediate(accumTexture);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (accumTexture == null || accumTexture.width != source.width || accumTexture.height != source.height)
		{
			DestroyImmediate(accumTexture);
			accumTexture = new RenderTexture(source.width, source.height, 0);
			accumTexture.hideFlags = HideFlags.HideAndDontSave;
			Graphics.Blit(source, accumTexture);
		}
		if (extraBlur)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
			accumTexture.MarkRestoreExpected();
			Graphics.Blit(accumTexture, temporary);
			Graphics.Blit(temporary, accumTexture);
			RenderTexture.ReleaseTemporary(temporary);
		}
		blurAmount = Mathf.Clamp(blurAmount, 0f, 0.92f);
		material.SetTexture("_MainTex", accumTexture);
		material.SetFloat("_AccumOrig", 1f - blurAmount);
		accumTexture.MarkRestoreExpected();
		Graphics.Blit(source, accumTexture, material);
		Graphics.Blit(accumTexture, destination);
	}
}
