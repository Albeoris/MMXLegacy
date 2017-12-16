using System;
using Legacy.TimeOfDay;
using UnityEngine;

public class ZoneEffectSetting : ScriptableObject
{
	public Boolean IsDarkDungeonArea;

	public Boolean IgnoreDayTime;

	public Boolean NoSkyLight;

	public ParameterSet Dawn;

	public ParameterSet Day;

	public ParameterSet Dusk;

	public ParameterSet Night;

	[Serializable]
	public class VignettingParameterSet
	{
		public Single axialAberration = 0.5f;

		public Single blur;

		public Single blurDistance = 2.5f;

		public Single blurSpread = 0.75f;

		public Single chromaticAberration = 0.2f;

		public Single intensity = 0.375f;

		public Single luminanceDependency = 0.25f;

		public void CopyTo(VignettingParameterSet other)
		{
			other.axialAberration = axialAberration;
			other.blur = blur;
			other.blurDistance = blurDistance;
			other.blurSpread = blurSpread;
			other.chromaticAberration = chromaticAberration;
			other.intensity = intensity;
			other.luminanceDependency = luminanceDependency;
		}
	}

	[Serializable]
	public class SkyParameterSet
	{
		public DayParameters Day;

		public NightParameters Night;

		public SunParameters Sun;

		public MoonParameters Moon;

		public CloudParameters Clouds;

		public Color AmbientColor;

		public void CopyTo(SkyParameterSet other)
		{
			other.Day.AdditiveColor = Day.AdditiveColor;
			other.Day.Brightness = Day.Brightness;
			other.Day.Directionality = Day.Directionality;
			other.Day.Haziness = Day.Haziness;
			other.Day.MieMultiplier = Day.MieMultiplier;
			other.Day.RayleighMultiplier = Day.RayleighMultiplier;
			other.Night.AdditiveColor = Night.AdditiveColor;
			other.Night.HazeColor = Night.HazeColor;
			other.Night.Haziness = Night.Haziness;
			other.Sun.Coloring = Sun.Coloring;
			other.Sun.Falloff = Sun.Falloff;
			other.Sun.LightColor = Sun.LightColor;
			other.Sun.LightIntensity = Sun.LightIntensity;
			other.Sun.ShadowStrength = Sun.ShadowStrength;
			other.Moon.Halo = Moon.Halo;
			other.Moon.LightColor = Moon.LightColor;
			other.Moon.LightIntensity = Moon.LightIntensity;
			other.Moon.Phase = Moon.Phase;
			other.Moon.ShadowStrength = Moon.ShadowStrength;
			other.Clouds.Density = Clouds.Density;
			other.Clouds.Scale1 = Clouds.Scale1;
			other.Clouds.Scale2 = Clouds.Scale2;
			other.Clouds.Shading = Clouds.Shading;
			other.Clouds.ShadowStrength = Clouds.ShadowStrength;
			other.Clouds.ShadowProjector = Clouds.ShadowProjector;
			other.Clouds.Sharpness = Clouds.Sharpness;
			other.Clouds.Tone = Clouds.Tone;
			other.AmbientColor = AmbientColor;
		}

		public void CopyFrom(DayParameters other)
		{
			Day.AdditiveColor = other.AdditiveColor;
			Day.Brightness = other.Brightness;
			Day.Directionality = other.Directionality;
			Day.Haziness = other.Haziness;
			Day.MieMultiplier = other.MieMultiplier;
			Day.RayleighMultiplier = other.RayleighMultiplier;
		}

		public void CopyFrom(NightParameters other)
		{
			Night.AdditiveColor = other.AdditiveColor;
			Night.HazeColor = other.HazeColor;
			Night.Haziness = other.Haziness;
		}

		public void CopyFrom(SunParameters other)
		{
			Sun.Coloring = other.Coloring;
			Sun.Falloff = other.Falloff;
			Sun.LightColor = other.LightColor;
			Sun.LightIntensity = other.LightIntensity;
			Sun.ShadowStrength = other.ShadowStrength;
		}

		public void CopyFrom(MoonParameters other)
		{
			Moon.Halo = other.Halo;
			Moon.LightColor = other.LightColor;
			Moon.LightIntensity = other.LightIntensity;
			Moon.Phase = other.Phase;
			Moon.ShadowStrength = other.ShadowStrength;
		}

		public void CopyFrom(CloudParameters other)
		{
			Clouds.Density = other.Density;
			Clouds.Scale1 = other.Scale1;
			Clouds.Scale2 = other.Scale2;
			Clouds.Shading = other.Shading;
			Clouds.ShadowStrength = other.ShadowStrength;
			Clouds.ShadowProjector = other.ShadowProjector;
			Clouds.Sharpness = other.Sharpness;
			Clouds.Tone = other.Tone;
		}
	}

	[Serializable]
	public class BloomParameters
	{
		public Int32 bloomBlurIterations = 2;

		public Single bloomIntensity = 0.5f;

		public Single bloomThreshhold = 0.5f;

		public Color bloomThreshholdColor = Color.white;

		public Single blurWidth = 1f;

		public Color flareColorA = new Color(0.4f, 0.4f, 0.8f, 0.75f);

		public Color flareColorB = new Color(0.4f, 0.8f, 0.8f, 0.75f);

		public Color flareColorC = new Color(0.8f, 0.4f, 0.8f, 0.75f);

		public Color flareColorD = new Color(0.8f, 0.4f, 0f, 0.75f);

		public Single flareRotation;

		public Single hollyStretchWidth = 2.5f;

		public Int32 hollywoodFlareBlurIterations = 2;

		public Single lensflareIntensity;

		public Single lensFlareSaturation = 0.75f;

		public Single lensflareThreshhold = 0.3f;

		public Single sepBlurSpread = 2.5f;

		public void CopyTo(BloomParameters other)
		{
			other.bloomBlurIterations = bloomBlurIterations;
			other.bloomIntensity = bloomIntensity;
			other.bloomThreshhold = bloomThreshhold;
			other.bloomThreshholdColor = bloomThreshholdColor;
			other.blurWidth = blurWidth;
			other.flareColorA = flareColorA;
			other.flareColorB = flareColorB;
			other.flareColorC = flareColorC;
			other.flareColorD = flareColorD;
			other.hollyStretchWidth = hollyStretchWidth;
			other.hollywoodFlareBlurIterations = hollywoodFlareBlurIterations;
			other.lensflareIntensity = lensflareIntensity;
			other.lensFlareSaturation = lensFlareSaturation;
			other.lensflareThreshhold = lensflareThreshhold;
			other.sepBlurSpread = sepBlurSpread;
		}
	}

	[Serializable]
	public class GlobalFogParameters
	{
		public Single globalDensity = 1f;

		public Color globalFogColor = Color.gray;

		public Single height;

		public Single heightScale = 100f;

		public Single startDistance = 200f;

		public GlobalFog.FogMode mode;

		public void CopyTo(GlobalFogParameters other)
		{
			other.globalDensity = globalDensity;
			other.globalFogColor = globalFogColor;
			other.height = height;
			other.heightScale = heightScale;
			other.startDistance = startDistance;
		}
	}

	[Serializable]
	public class SunShaftsParameters
	{
		public Color SunColor = Color.white;

		public Single SunShaftIntensity = 1.15f;

		public void CopyTo(SunShaftsParameters other)
		{
			other.SunColor = SunColor;
			other.SunShaftIntensity = SunShaftIntensity;
		}
	}

	[Serializable]
	public class ColorCorrectionParameters
	{
		public Texture3D Texture;
	}

	[Serializable]
	public class WaterParameters
	{
		public Color Refaction = new Color(0.54f, 0.95f, 0.99f, 0.5f);

		public Color Reflection = new Color(0.54f, 0.95f, 0.99f, 0.5f);

		public Color Specular = new Color(0.72f, 0.72f, 0.72f, 1f);

		public Single SpecularPower = 200f;

		public void CopyTo(WaterParameters other)
		{
			other.Refaction = Refaction;
			other.Reflection = Reflection;
			other.Specular = Specular;
			other.SpecularPower = SpecularPower;
		}
	}

	[Serializable]
	public class EdgeDetectionParameters
	{
		public Single SensitivityDepth = 3f;

		public Single SensitivityNormals = 1f;

		public Single SampleDist = 0.5f;

		public Single EdgeItensity = 3f;

		public Color CapColor = new Color32(5, 5, 5, 5);

		public Single ColorScale = 2f;

		public Color AddColor = new Color32(0, 0, 0, 0);

		public Single StartDistance = 0.01f;

		public Single DistanceScale = 4f;

		public Single Spread = 0.25f;

		public Single SubSample = 1f;

		public void CopyTo(EdgeDetectionParameters other)
		{
			other.EdgeItensity = EdgeItensity;
			other.SensitivityDepth = SensitivityDepth;
			other.SensitivityNormals = SensitivityNormals;
			other.SampleDist = SampleDist;
			other.CapColor = CapColor;
			other.ColorScale = ColorScale;
			other.AddColor = AddColor;
			other.StartDistance = StartDistance;
			other.DistanceScale = DistanceScale;
			other.Spread = Spread;
			other.SubSample = SubSample;
		}
	}

	[Serializable]
	public class ParameterSet
	{
		public SkyParameterSet SkyParameter;

		public VignettingParameterSet VignettingParameter;

		public BloomParameters BloomParameter;

		public GlobalFogParameters GlobalFogNearParameter;

		public GlobalFogParameters GlobalFogFarParameter;

		public GlobalFogParameters GlobalFogGroundParameter;

		public SunShaftsParameters SunShaftsParameter;

		public ColorCorrectionParameters ColorCorrectionParameter;

		public WaterParameters WaterParameter;

		public EdgeDetectionParameters EdgeDetectionParameter;

		public void CopyTo(ParameterSet other)
		{
			SkyParameter.CopyTo(other.SkyParameter);
			VignettingParameter.CopyTo(other.VignettingParameter);
			BloomParameter.CopyTo(other.BloomParameter);
			GlobalFogNearParameter.CopyTo(other.GlobalFogNearParameter);
			GlobalFogFarParameter.CopyTo(other.GlobalFogFarParameter);
			GlobalFogGroundParameter.CopyTo(other.GlobalFogGroundParameter);
			SunShaftsParameter.CopyTo(other.SunShaftsParameter);
			other.ColorCorrectionParameter.Texture = ColorCorrectionParameter.Texture;
			WaterParameter.CopyTo(other.WaterParameter);
			EdgeDetectionParameter.CopyTo(other.EdgeDetectionParameter);
		}
	}
}
