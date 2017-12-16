using System;
using Legacy.Configuration;
using UnityEngine;
using Object = System.Object;

[RequireComponent(typeof(AntialiasingAsPostEffect))]
public class AntialiasingQuality : MonoBehaviour
{
	private void Awake()
	{
		SettingApplied(null, null);
		GraphicsConfigManager.SettingApplied += SettingApplied;
	}

	private void OnDestroy()
	{
		GraphicsConfigManager.SettingApplied -= SettingApplied;
	}

	private void SettingApplied(Object sender, EventArgs e)
	{
		AntialiasingAsPostEffect component = GetComponent<AntialiasingAsPostEffect>();
		component.enabled = true;
		switch (GraphicsConfigManager.Settings.Antialiasing)
		{
		case EFSAAMode.Off:
			component.enabled = false;
			break;
		case EFSAAMode.FXAA3:
			component.mode = AAMode.FXAA3Console;
			break;
		case EFSAAMode.FXAA2:
			component.mode = AAMode.FXAA2;
			break;
		case EFSAAMode.FXAA1A:
			component.mode = AAMode.FXAA1PresetA;
			break;
		case EFSAAMode.FXAA1B:
			component.mode = AAMode.FXAA1PresetB;
			break;
		case EFSAAMode.NFAA:
			component.mode = AAMode.NFAA;
			break;
		case EFSAAMode.SSAA:
			component.mode = AAMode.SSAA;
			break;
		case EFSAAMode.DLAA:
			component.mode = AAMode.DLAA;
			break;
		}
	}
}
