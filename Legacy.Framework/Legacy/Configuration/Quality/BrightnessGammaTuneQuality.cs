using System;
using Legacy.EffectEngine.PostEffects;
using UnityEngine;

namespace Legacy.Configuration.Quality
{
	[RequireComponent(typeof(BrightnessGammaTune))]
	[AddComponentMenu("MM Legacy/Quality/BrightnessGammaTune Quality")]
	public class BrightnessGammaTuneQuality : QualityConfigurationBase
	{
		public override void OnQualityConfigutationChanged()
		{
			BrightnessGammaTune component = GetComponent<BrightnessGammaTune>();
			component.BrightnessAmount = GraphicsConfigManager.Settings.Brightness;
			component.GammaAmount = GraphicsConfigManager.Settings.Gamma;
		}
	}
}
