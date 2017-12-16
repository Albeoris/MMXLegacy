using System;
using System.Collections.Generic;
using Legacy.Configuration;
using Legacy.Game.Context;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/OptionsGraphics")]
	public class OptionsGraphics : MonoBehaviour
	{
		[SerializeField]
		private UIPopupList m_resolution;

		[SerializeField]
		private UIPopupList m_antialiasing;

		[SerializeField]
		private UICheckbox m_fullScreen;

		[SerializeField]
		private UICheckbox m_verticalSync;

		[SerializeField]
		private UICheckbox m_hdr;

		[SerializeField]
		private UILabel m_brightnessLabel;

		[SerializeField]
		private UISlider m_brightnessSlider;

		[SerializeField]
		private UILabel m_gammaLabel;

		[SerializeField]
		private UISlider m_gammaSlider;

		[SerializeField]
		private UILabel m_framerateMaxLabel;

		[SerializeField]
		private UISlider m_framerateMaxSlider;

		[SerializeField]
		private UILabel m_filteringLabel;

		[SerializeField]
		private UISlider m_filteringSlider;

		[SerializeField]
		private UILabel m_textureQualityLabel;

		[SerializeField]
		private UISlider m_textureQualitySlider;

		[SerializeField]
		private UILabel m_shadowQualityLabel;

		[SerializeField]
		private UISlider m_shadowQualitySlider;

		[SerializeField]
		private UILabel m_shadowDistanceLabel;

		[SerializeField]
		private UISlider m_shadowDistanceSlider;

		[SerializeField]
		private UILabel m_terrainQualityLabel;

		[SerializeField]
		private UISlider m_terrainQualitySlider;

		[SerializeField]
		private UILabel m_terrainDetailsLabel;

		[SerializeField]
		private UISlider m_terrainDetailsSlider;

		[SerializeField]
		private UICheckbox m_terrainShadows;

		[SerializeField]
		private UILabel m_effectQualityLabel;

		[SerializeField]
		private UISlider m_effectQualitySlider;

		[SerializeField]
		private UILabel m_viewDistanceLabel;

		[SerializeField]
		private UISlider m_viewDistanceSlider;

		[SerializeField]
		private UILabel m_animationQualityLabel;

		[SerializeField]
		private UISlider m_animationQualitySlider;

		[SerializeField]
		private UILabel m_objectDetailsLabel;

		[SerializeField]
		private UISlider m_objectDetailsSlider;

		private void Awake()
		{
			if (ContextManager.ActiveContextID == EContext.Game)
			{
				UIWidget[] componentsInChildren = m_textureQualitySlider.transform.parent.GetComponentsInChildren<UIWidget>(true);
				foreach (UIWidget uiwidget in componentsInChildren)
				{
					uiwidget.alpha *= 0.5f;
				}
				Collider[] componentsInChildren2 = m_textureQualitySlider.transform.parent.GetComponentsInChildren<Collider>(true);
				foreach (Collider collider in componentsInChildren2)
				{
					collider.enabled = false;
				}
				m_textureQualitySlider.transform.parent.collider.enabled = true;
			}
			m_textureQualitySlider.numberOfSteps = EnumUtil<ETextureQuality>.Length;
			UISlider textureQualitySlider = m_textureQualitySlider;
			textureQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(textureQualitySlider.onValueChange, new UISlider.OnValueChange(OnTextureQualitySliderChange));
			if (!Helper.Is64BitOperatingSystem())
			{
				m_textureQualitySlider.numberOfSteps -= 2;
			}
			m_shadowQualitySlider.numberOfSteps = EnumUtil<EShadowQuality>.Length;
			UISlider shadowQualitySlider = m_shadowQualitySlider;
			shadowQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(shadowQualitySlider.onValueChange, new UISlider.OnValueChange(OnShadowQualitySliderChange));
			m_shadowDistanceSlider.numberOfSteps = 151;
			UISlider shadowDistanceSlider = m_shadowDistanceSlider;
			shadowDistanceSlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(shadowDistanceSlider.onValueChange, new UISlider.OnValueChange(OnShadowDistanceSliderChange));
			m_terrainQualitySlider.numberOfSteps = EnumUtil<ETerrainQuality>.Length;
			UISlider terrainQualitySlider = m_terrainQualitySlider;
			terrainQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(terrainQualitySlider.onValueChange, new UISlider.OnValueChange(OnTerrainQualitySliderChange));
			m_terrainDetailsSlider.numberOfSteps = EnumUtil<ETerrainDetails>.Length;
			UISlider terrainDetailsSlider = m_terrainDetailsSlider;
			terrainDetailsSlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(terrainDetailsSlider.onValueChange, new UISlider.OnValueChange(OnTerrainDetailsSliderChange));
			m_effectQualitySlider.numberOfSteps = EnumUtil<EEffectQuality>.Length;
			UISlider effectQualitySlider = m_effectQualitySlider;
			effectQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(effectQualitySlider.onValueChange, new UISlider.OnValueChange(OnEffectQualitySliderChange));
			m_viewDistanceSlider.numberOfSteps = EnumUtil<EViewDistance>.Length;
			UISlider viewDistanceSlider = m_viewDistanceSlider;
			viewDistanceSlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(viewDistanceSlider.onValueChange, new UISlider.OnValueChange(OnViewDistanceSliderChange));
			m_animationQualitySlider.numberOfSteps = EnumUtil<EAnimationQuality>.Length;
			UISlider animationQualitySlider = m_animationQualitySlider;
			animationQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(animationQualitySlider.onValueChange, new UISlider.OnValueChange(OnAnimationQualitySliderChange));
			m_filteringSlider.numberOfSteps = 17;
			UISlider filteringSlider = m_filteringSlider;
			filteringSlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(filteringSlider.onValueChange, new UISlider.OnValueChange(OnFilteringSliderChange));
			m_framerateMaxSlider.numberOfSteps = 501;
			UISlider framerateMaxSlider = m_framerateMaxSlider;
			framerateMaxSlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(framerateMaxSlider.onValueChange, new UISlider.OnValueChange(OnFramerateMaxSliderChange));
			UISlider brightnessSlider = m_brightnessSlider;
			brightnessSlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(brightnessSlider.onValueChange, new UISlider.OnValueChange(OnBrightnessSliderChange));
			UISlider gammaSlider = m_gammaSlider;
			gammaSlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(gammaSlider.onValueChange, new UISlider.OnValueChange(OnGammaSliderChange));
			UICheckbox fullScreen = m_fullScreen;
			fullScreen.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(fullScreen.onStateChange, new UICheckbox.OnStateChange(OnFullScreenChange));
			UICheckbox verticalSync = m_verticalSync;
			verticalSync.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(verticalSync.onStateChange, new UICheckbox.OnStateChange(OnVSyncChange));
			UICheckbox hdr = m_hdr;
			hdr.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(hdr.onStateChange, new UICheckbox.OnStateChange(OnHDRChange));
			UICheckbox terrainShadows = m_terrainShadows;
			terrainShadows.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(terrainShadows.onStateChange, new UICheckbox.OnStateChange(OnTerrainShadowsChange));
			m_objectDetailsSlider.numberOfSteps = EnumUtil<EObjectDetail>.Length;
			UISlider objectDetailsSlider = m_objectDetailsSlider;
			objectDetailsSlider.onValueChange = (UISlider.OnValueChange)Delegate.Combine(objectDetailsSlider.onValueChange, new UISlider.OnValueChange(OnObjectDetailsSliderChange));
			m_antialiasing.items.Clear();
			m_antialiasing.items.Add(LocaManager.GetText("OPTIONS_GRAPHICS_QUALITY_SETTING_OFF"));
			for (Int32 k = 1; k < EnumUtil<EFSAAMode>.Values.Length; k++)
			{
				m_antialiasing.items.Add(EnumUtil<EFSAAMode>.Values[k].ToString());
			}
			UIPopupList antialiasing = m_antialiasing;
			antialiasing.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Combine(antialiasing.onSelectionChange, new UIPopupList.OnSelectionChange(OnAntialiasingChange));
			m_resolution.items.Clear();
			foreach (Resolution resolution in GraphicsConfigManager.AvailableResolutions)
			{
				if (resolution.refreshRate > 0)
				{
					m_resolution.items.Add(String.Concat(new Object[]
					{
						resolution.width,
						"x",
						resolution.height,
						" ",
						resolution.refreshRate,
						"Hz"
					}));
				}
				else
				{
					m_resolution.items.Add(resolution.width + "x" + resolution.height);
				}
			}
			UIPopupList resolution2 = m_resolution;
			resolution2.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Combine(resolution2.onSelectionChange, new UIPopupList.OnSelectionChange(OnResolutionChanged));
		}

		private void OnDestroy()
		{
			UISlider textureQualitySlider = m_textureQualitySlider;
			textureQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(textureQualitySlider.onValueChange, new UISlider.OnValueChange(OnTextureQualitySliderChange));
			UISlider shadowQualitySlider = m_shadowQualitySlider;
			shadowQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(shadowQualitySlider.onValueChange, new UISlider.OnValueChange(OnShadowQualitySliderChange));
			UISlider terrainQualitySlider = m_terrainQualitySlider;
			terrainQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(terrainQualitySlider.onValueChange, new UISlider.OnValueChange(OnTerrainQualitySliderChange));
			UISlider shadowDistanceSlider = m_shadowDistanceSlider;
			shadowDistanceSlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(shadowDistanceSlider.onValueChange, new UISlider.OnValueChange(OnShadowDistanceSliderChange));
			UISlider terrainDetailsSlider = m_terrainDetailsSlider;
			terrainDetailsSlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(terrainDetailsSlider.onValueChange, new UISlider.OnValueChange(OnTerrainDetailsSliderChange));
			UISlider effectQualitySlider = m_effectQualitySlider;
			effectQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(effectQualitySlider.onValueChange, new UISlider.OnValueChange(OnEffectQualitySliderChange));
			UISlider viewDistanceSlider = m_viewDistanceSlider;
			viewDistanceSlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(viewDistanceSlider.onValueChange, new UISlider.OnValueChange(OnViewDistanceSliderChange));
			UISlider animationQualitySlider = m_animationQualitySlider;
			animationQualitySlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(animationQualitySlider.onValueChange, new UISlider.OnValueChange(OnAnimationQualitySliderChange));
			UISlider filteringSlider = m_filteringSlider;
			filteringSlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(filteringSlider.onValueChange, new UISlider.OnValueChange(OnFilteringSliderChange));
			UISlider framerateMaxSlider = m_framerateMaxSlider;
			framerateMaxSlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(framerateMaxSlider.onValueChange, new UISlider.OnValueChange(OnFramerateMaxSliderChange));
			UISlider brightnessSlider = m_brightnessSlider;
			brightnessSlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(brightnessSlider.onValueChange, new UISlider.OnValueChange(OnBrightnessSliderChange));
			UISlider gammaSlider = m_gammaSlider;
			gammaSlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(gammaSlider.onValueChange, new UISlider.OnValueChange(OnGammaSliderChange));
			UICheckbox fullScreen = m_fullScreen;
			fullScreen.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(fullScreen.onStateChange, new UICheckbox.OnStateChange(OnFullScreenChange));
			UICheckbox verticalSync = m_verticalSync;
			verticalSync.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(verticalSync.onStateChange, new UICheckbox.OnStateChange(OnVSyncChange));
			UICheckbox hdr = m_hdr;
			hdr.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(hdr.onStateChange, new UICheckbox.OnStateChange(OnHDRChange));
			UICheckbox terrainShadows = m_terrainShadows;
			terrainShadows.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(terrainShadows.onStateChange, new UICheckbox.OnStateChange(OnTerrainShadowsChange));
			UISlider objectDetailsSlider = m_objectDetailsSlider;
			objectDetailsSlider.onValueChange = (UISlider.OnValueChange)Delegate.Remove(objectDetailsSlider.onValueChange, new UISlider.OnValueChange(OnObjectDetailsSliderChange));
			UIPopupList antialiasing = m_antialiasing;
			antialiasing.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Remove(antialiasing.onSelectionChange, new UIPopupList.OnSelectionChange(OnAntialiasingChange));
			UIPopupList resolution = m_resolution;
			resolution.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Remove(resolution.onSelectionChange, new UIPopupList.OnSelectionChange(OnResolutionChanged));
		}

		private void OnEnable()
		{
			UpdateGUI();
		}

		public void UpdateGUI()
		{
			m_textureQualitySlider.SetSliderStep((Int32)GraphicsConfigManager.Settings.TextureQuality);
			m_shadowQualitySlider.SetSliderStep((Int32)GraphicsConfigManager.Settings.ShadowQuality);
			m_shadowDistanceSlider.SetSliderStep(GraphicsConfigManager.Settings.ShadowDistance);
			m_terrainQualitySlider.SetSliderStep((Int32)GraphicsConfigManager.Settings.TerrainQuality);
			m_terrainDetailsSlider.SetSliderStep((Int32)GraphicsConfigManager.Settings.TerrainDetails);
			m_effectQualitySlider.SetSliderStep((Int32)GraphicsConfigManager.Settings.EffectQuality);
			m_viewDistanceSlider.SetSliderStep((Int32)GraphicsConfigManager.Settings.ViewDistance);
			m_animationQualitySlider.SetSliderStep((Int32)GraphicsConfigManager.Settings.AnimationQuality);
			m_filteringSlider.SetSliderStep(GraphicsConfigManager.Settings.Filtering);
			m_framerateMaxSlider.SetSliderStep(GraphicsConfigManager.Settings.MaxFramerate);
			Single num = (GraphicsConfigManager.Settings.Brightness - 0.5f) / 1f;
			m_brightnessSlider.sliderValue = (Single)Math.Round(num, 2);
			m_gammaSlider.sliderValue = (GraphicsConfigManager.Settings.Gamma - 0.5f) / 2.5f;
			m_fullScreen.isChecked = GraphicsConfigManager.Settings.FullScreen;
			m_verticalSync.isChecked = GraphicsConfigManager.Settings.VerticalSync;
			m_hdr.isChecked = GraphicsConfigManager.Settings.HighDynamicRange;
			m_terrainShadows.isChecked = GraphicsConfigManager.Settings.TerrainShadows;
			m_objectDetailsSlider.SetSliderStep((Int32)GraphicsConfigManager.Settings.ObjectDetails);
			Int32 antialiasing = (Int32)GraphicsConfigManager.Settings.Antialiasing;
			m_antialiasing.selection = m_antialiasing.items[antialiasing];
			Resolution resolution = GraphicsConfigManager.Settings.Resolution;
			List<Resolution> availableResolutions = GraphicsConfigManager.AvailableResolutions;
			for (Int32 i = 0; i < availableResolutions.Count; i++)
			{
				Resolution resolution2 = availableResolutions[i];
				if (resolution2.width == resolution.width && resolution2.height == resolution.height && resolution2.refreshRate == resolution.refreshRate)
				{
					m_resolution.selection = m_resolution.items[i];
					break;
				}
			}
		}

		private void OnFilteringSliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.Filtering = m_filteringSlider.GetSliderStep();
			m_filteringLabel.text = GraphicsConfigManager.Settings.Filtering.ToString() + "x";
		}

		private void OnFramerateMaxSliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.MaxFramerate = m_framerateMaxSlider.GetSliderStep();
			m_framerateMaxLabel.text = GraphicsConfigManager.Settings.MaxFramerate.ToString();
		}

		private void OnBrightnessSliderChange(Single p_value)
		{
			Single num = 0.5f + m_brightnessSlider.sliderValue * 1f;
			GraphicsConfigManager.Settings.Brightness = (Single)Math.Round(num, 2);
			m_brightnessLabel.text = GraphicsConfigManager.Settings.Brightness.ToString("0.0#");
			GraphicsConfigManager.ApplyBrightnessOrGamma();
		}

		private void OnGammaSliderChange(Single p_value)
		{
			Single num = 0.5f + m_gammaSlider.sliderValue * 2.5f;
			GraphicsConfigManager.Settings.Gamma = (Single)Math.Round(num, 2);
			m_gammaLabel.text = GraphicsConfigManager.Settings.Gamma.ToString("0.0#");
			GraphicsConfigManager.ApplyBrightnessOrGamma();
		}

		private void OnFullScreenChange(Boolean p_state)
		{
			GraphicsConfigManager.Settings.FullScreen = p_state;
		}

		private void OnVSyncChange(Boolean p_state)
		{
			GraphicsConfigManager.Settings.VerticalSync = p_state;
		}

		private void OnHDRChange(Boolean p_state)
		{
			GraphicsConfigManager.Settings.HighDynamicRange = p_state;
		}

		private void OnShadowDistanceSliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.ShadowDistance = m_shadowDistanceSlider.GetSliderStep();
			m_shadowDistanceLabel.text = GraphicsConfigManager.Settings.ShadowDistance.ToString();
		}

		private void OnTerrainShadowsChange(Boolean p_state)
		{
			GraphicsConfigManager.Settings.TerrainShadows = p_state;
		}

		private void OnObjectDetailsSliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.ObjectDetails = (EObjectDetail)m_objectDetailsSlider.GetSliderStep();
			m_objectDetailsLabel.text = LocaManager.GetText("OPTIONS_GRAPHICS_QUALITY_SETTING_" + GraphicsConfigManager.Settings.ObjectDetails);
		}

		private void OnTextureQualitySliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.TextureQuality = (ETextureQuality)m_textureQualitySlider.GetSliderStep();
			m_textureQualityLabel.text = LocaManager.GetText("OPTIONS_GRAPHICS_QUALITY_SETTING_" + GraphicsConfigManager.Settings.TextureQuality);
		}

		private void OnShadowQualitySliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.ShadowQuality = (EShadowQuality)m_shadowQualitySlider.GetSliderStep();
			m_shadowQualityLabel.text = LocaManager.GetText("OPTIONS_GRAPHICS_QUALITY_SETTING_" + GraphicsConfigManager.Settings.ShadowQuality);
		}

		private void OnTerrainQualitySliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.TerrainQuality = (ETerrainQuality)m_terrainQualitySlider.GetSliderStep();
			m_terrainQualityLabel.text = LocaManager.GetText("OPTIONS_GRAPHICS_QUALITY_SETTING_" + GraphicsConfigManager.Settings.TerrainQuality);
		}

		private void OnTerrainDetailsSliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.TerrainDetails = (ETerrainDetails)m_terrainDetailsSlider.GetSliderStep();
			m_terrainDetailsLabel.text = LocaManager.GetText("OPTIONS_GRAPHICS_QUALITY_SETTING_" + GraphicsConfigManager.Settings.TerrainDetails);
		}

		private void OnEffectQualitySliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.EffectQuality = (EEffectQuality)m_effectQualitySlider.GetSliderStep();
			m_effectQualityLabel.text = LocaManager.GetText("OPTIONS_GRAPHICS_QUALITY_SETTING_" + GraphicsConfigManager.Settings.EffectQuality);
		}

		private void OnViewDistanceSliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.ViewDistance = (EViewDistance)m_viewDistanceSlider.GetSliderStep();
			m_viewDistanceLabel.text = LocaManager.GetText("OPTIONS_GRAPHICS_QUALITY_SETTING_" + GraphicsConfigManager.Settings.ViewDistance);
		}

		private void OnAnimationQualitySliderChange(Single p_value)
		{
			GraphicsConfigManager.Settings.AnimationQuality = (EAnimationQuality)m_animationQualitySlider.GetSliderStep();
			m_animationQualityLabel.text = LocaManager.GetText("OPTIONS_GRAPHICS_QUALITY_SETTING_" + GraphicsConfigManager.Settings.AnimationQuality);
		}

		private void OnAntialiasingChange(String p_item)
		{
			EFSAAMode efsaamode = (EFSAAMode)m_antialiasing.items.IndexOf(p_item);
			if (efsaamode >= EFSAAMode.Off)
			{
				GraphicsConfigManager.Settings.Antialiasing = efsaamode;
			}
		}

		private void OnResolutionChanged(String p_item)
		{
			Int32 num = m_resolution.items.IndexOf(p_item);
			if (num >= 0 && num < GraphicsConfigManager.AvailableResolutions.Count)
			{
				GraphicsConfigManager.Settings.Resolution = GraphicsConfigManager.AvailableResolutions[num];
			}
		}
	}
}
