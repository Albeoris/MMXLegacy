using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Configuration.Quality;
using Legacy.Core.Utilities.Configuration;
using UnityEngine;

namespace Legacy.Configuration
{
	public static class GraphicsConfigManager
	{
		private static String m_path;

		private static String m_defaultsPath;

		public static readonly GraphicsSettings Settings = new GraphicsSettings();

		static GraphicsConfigManager()
		{
			Resolution[] resolutions = Screen.resolutions;
			if (resolutions != null)
			{
				foreach (Resolution resolution in resolutions)
				{
					if (IsResolutionSupported(resolution))
					{
						AvailableResolutions.Add(resolution);
					}
				}
			}
		}

	    public static event EventHandler SettingApplied;

	    public static event EventHandler BrightnessOrGammaApplied;

		public static List<Resolution> AvailableResolutions { get; private set; } = new List<Resolution>();

		public static void InitDefaultResolution()
		{
			Settings.Resolution = AvailableResolutions.LastElement<Resolution>();
			Settings.FullScreen = true;
		}

		public static Boolean IsResolutionSupported(Resolution p_resolution)
		{
			EAspectRatio aspectRatio = GetAspectRatio(p_resolution);
			return aspectRatio != EAspectRatio.None;
		}

		public static EAspectRatio GetAspectRatio()
		{
			return GetAspectRatio(new Resolution
			{
				width = Screen.width,
				height = Screen.height
			});
		}

		public static EAspectRatio GetAspectRatio(Int32 p_width, Int32 p_height)
		{
			return GetAspectRatio(new Resolution
			{
				width = p_width,
				height = p_height
			});
		}

		public static EAspectRatio GetAspectRatio(Resolution p_resolution)
		{
			Single num = p_resolution.width / (Single)p_resolution.height;
			if (num >= 1.2375f && num <= 1.2625f)
			{
				return EAspectRatio.None;
			}
			if (num >= 1.32f && num <= 1.34666669f)
			{
				return EAspectRatio._4_3;
			}
			if (num >= 1.76f && num <= 1.79555559f)
			{
				return EAspectRatio._16_9;
			}
			if (num >= 1.584f && num <= 1.616f)
			{
				return EAspectRatio._16_10;
			}
			EAspectRatio result = EAspectRatio.None;
			Single num2 = Single.MaxValue;
			Single num3 = Math.Abs(1.33333337f - num);
			if (num2 > num3)
			{
				num2 = num3;
				result = EAspectRatio._4_3;
			}
			num3 = Math.Abs(1.77777779f - num);
			if (num2 > num3)
			{
				num2 = num3;
				result = EAspectRatio._16_9;
			}
			num3 = Math.Abs(1.6f - num);
			if (num2 > num3)
			{
				result = EAspectRatio._16_10;
			}
			return result;
		}

		public static void SetDefaultsPath(String p_path)
		{
			m_defaultsPath = p_path;
		}

		public static void LoadDefaultSettings()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_defaultsPath);
			Settings.Load(configReader);
		}

		public static void LoadConfigurations(String p_path)
		{
			m_path = p_path;
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(p_path);
			Settings.Load(configReader);
		}

		public static void ReloadConfigurations()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_path);
			Settings.Load(configReader);
		}

		public static void WriteConfigurations()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_path);
			Settings.Save(configReader);
			configReader.WriteData(m_path);
		}

		public static Boolean HasUnsavedChanges()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_path);
			return Settings.HasUnsavedChanges(configReader);
		}

		public static void ApplyBrightnessOrGamma()
		{
			if (BrightnessOrGammaApplied != null)
			{
				BrightnessOrGammaApplied(null, EventArgs.Empty);
			}
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(BrightnessGammaTuneQuality));
			foreach (UnityEngine.Object @object in array)
			{
				((BrightnessGammaTuneQuality)@object).OnQualityConfigutationChanged();
			}
		}

		public static void Apply()
		{
			Int32 num = 3;
			Int32 length = EnumUtil<EShadowQuality>.Length;
			QualitySettings.SetQualityLevel(num + (Int32)Settings.TextureQuality * (Int32)(ETextureQuality)length + (Int32)Settings.ShadowQuality);
			if (Settings.Filtering <= 0)
			{
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
				Texture.SetGlobalAnisotropicFilteringLimits(-1, -1);
			}
			else
			{
				QualitySettings.anisotropicFiltering = ((Settings.Filtering >= 2) ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Enable);
				Texture.SetGlobalAnisotropicFilteringLimits(Settings.Filtering, 16);
			}
			QualitySettings.antiAliasing = 0;
			QualitySettings.vSyncCount = ((!Settings.VerticalSync) ? 0 : 1);
			Application.targetFrameRate = Settings.MaxFramerate;
			QualitySettings.softVegetation = (Settings.TerrainDetails >= ETerrainDetails.HIGH);
			QualitySettings.shadowDistance = ((Settings.ShadowDistance != 0) ? Settings.ShadowDistance : 0.1f);
			switch (Settings.ObjectDetails)
			{
			case EObjectDetail.LOW:
				QualitySettings.lodBias = 0.4f;
				break;
			case EObjectDetail.MEDIUM:
				QualitySettings.lodBias = 0.7f;
				break;
			case EObjectDetail.HIGH:
				QualitySettings.lodBias = 1f;
				break;
			case EObjectDetail.VERY_HIGH:
				QualitySettings.lodBias = 1.5f;
				break;
			case EObjectDetail.ULTRA:
				QualitySettings.lodBias = 2f;
				break;
			}
			switch (Settings.AnimationQuality)
			{
			case EAnimationQuality.LOW:
				QualitySettings.blendWeights = BlendWeights.OneBone;
				break;
			case EAnimationQuality.MEDIUM:
				QualitySettings.blendWeights = BlendWeights.TwoBones;
				break;
			case EAnimationQuality.HIGH:
				QualitySettings.blendWeights = BlendWeights.FourBones;
				break;
			}
			if (!Settings.Resolution.Equals(Screen.currentResolution) || Settings.FullScreen != Screen.fullScreen || !Settings.FullScreen)
			{
				Screen.SetResolution(Settings.Resolution.width, Settings.Resolution.height, Settings.FullScreen);
			}
			if (SettingApplied != null)
			{
				SettingApplied(null, EventArgs.Empty);
			}
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(QualityConfigurationBase));
			foreach (UnityEngine.Object @object in array)
			{
				((QualityConfigurationBase)@object).OnQualityConfigutationChanged();
			}
		}
	}
}
