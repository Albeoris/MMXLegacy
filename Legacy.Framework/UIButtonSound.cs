using System;
using Legacy.EffectEngine;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Sound")]
public class UIButtonSound : MonoBehaviour
{
	public AudioClip audioClip;

	public String audioIDHover = String.Empty;

	public String audioIDPress = String.Empty;

	public String audioIDRelease = String.Empty;

	public String audioIDClick = "ButtonClickDefault";

	public Trigger trigger;

	public Single volume = 1f;

	public Single pitch = 1f;

	private void OnHover(Boolean isOver)
	{
		if (enabled && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
		{
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
		if (!String.IsNullOrEmpty(audioIDHover) && AudioController.DoesInstanceExist())
		{
			if (FXMainCamera.Instance != null)
			{
				AudioController.Play(audioIDHover, FXMainCamera.Instance.transform);
			}
			else
			{
				AudioController.Play(audioIDHover);
			}
		}
	}

	private void OnPress(Boolean isPressed)
	{
		if (enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
		{
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
		String text = (!isPressed) ? audioIDRelease : audioIDPress;
		if (!String.IsNullOrEmpty(text) && AudioController.DoesInstanceExist())
		{
			if (FXMainCamera.Instance != null)
			{
				AudioController.Play(text, FXMainCamera.Instance.transform);
			}
			else
			{
				AudioController.Play(text);
			}
		}
	}

	private void OnClick()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
		if (!String.IsNullOrEmpty(audioIDClick) && AudioController.DoesInstanceExist())
		{
			if (FXMainCamera.Instance != null)
			{
				AudioController.Play(audioIDClick, FXMainCamera.Instance.transform);
			}
			else
			{
				AudioController.Play(audioIDClick);
			}
		}
	}

	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease
	}
}
