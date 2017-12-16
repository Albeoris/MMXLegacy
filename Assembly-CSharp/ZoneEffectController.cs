using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.TimeOfDay;
using UnityEngine;
using Object = System.Object;

public class ZoneEffectController : MonoBehaviour
{
	private List<Effect> m_ActiveEffects = new List<Effect>();

	private Effect m_CurrentEffect;

	private Single m_CurrentDayTime = Single.MinValue;

	private Single m_TargetDayTime;

	private EDayState m_TargetDayState;

	private Single m_BlendTime;

	private ZoneEffectSetting.ParameterSet m_SourceSettings = new ZoneEffectSetting.ParameterSet
	{
		VignettingParameter = new ZoneEffectSetting.VignettingParameterSet(),
		BloomParameter = new ZoneEffectSetting.BloomParameters(),
		EdgeDetectionParameter = new ZoneEffectSetting.EdgeDetectionParameters(),
		GlobalFogNearParameter = new ZoneEffectSetting.GlobalFogParameters(),
		GlobalFogFarParameter = new ZoneEffectSetting.GlobalFogParameters(),
		GlobalFogGroundParameter = new ZoneEffectSetting.GlobalFogParameters(),
		SkyParameter = new ZoneEffectSetting.SkyParameterSet
		{
			Clouds = new CloudParameters(),
			Day = new DayParameters(),
			Moon = new MoonParameters(),
			Night = new NightParameters(),
			Sun = new SunParameters()
		},
		SunShaftsParameter = new ZoneEffectSetting.SunShaftsParameters(),
		WaterParameter = new ZoneEffectSetting.WaterParameters()
	};

	private ZoneEffectSetting.ParameterSet m_TargetSettings;

	private Bloom m_BloomController;

	private GlobalFog m_GlobalFogNearController;

	private GlobalFog m_GlobalFogFarController;

	private GlobalFog m_GlobalFogGroundController;

	private SunShafts m_SunShaftsController;

	private Vignetting m_VignettingController;

	private ColorCorrection m_ColorCorrectionController;

	private EdgeDetectionEffect m_EdgeDetectionEffectController;

	private ColorCorrectionEffectBlend m_darkDungeonColorCorrectionController;

	[SerializeField]
	public Sky m_SkyController;

	[SerializeField]
	private WaterBase m_WaterBase;

	[SerializeField]
	private Single m_DefaultSettingBlendTime = 0.5f;

	[SerializeField]
	public ZoneEffectSetting m_DefaultSetting;

	[SerializeField]
	private Single m_TimeOfDayDawn = 7f;

	[SerializeField]
	private Single m_TimeOfDayDay = 14f;

	[SerializeField]
	private Single m_TimeOfDayDusk = 19f;

	[SerializeField]
	private Single m_TimeOfDayNight = 21f;

	public static ZoneEffectController Instance { get; private set; }

	public EDayState CurrentDayState => m_TargetDayState;

    public ZoneEffectSetting CurrentSetting => m_CurrentEffect.Setting;

    public Bloom BloomController
	{
		get => m_BloomController;
        set
		{
			m_BloomController = value;
			ChangeDayState(m_TargetDayState, ETimeChangeReason.None, false);
		}
	}

	public GlobalFog GlobalFogNearController
	{
		get => m_GlobalFogNearController;
	    set
		{
			m_GlobalFogNearController = value;
			ChangeDayState(m_TargetDayState, ETimeChangeReason.None, false);
		}
	}

	public GlobalFog GlobalFogFarController
	{
		get => m_GlobalFogFarController;
	    set
		{
			m_GlobalFogFarController = value;
			ChangeDayState(m_TargetDayState, ETimeChangeReason.None, false);
		}
	}

	public GlobalFog GlobalFogGroundController
	{
		get => m_GlobalFogGroundController;
	    set
		{
			m_GlobalFogGroundController = value;
			ChangeDayState(m_TargetDayState, ETimeChangeReason.None, false);
		}
	}

	public SunShafts SunShaftsController
	{
		get => m_SunShaftsController;
	    set
		{
			m_SunShaftsController = value;
			ChangeDayState(m_TargetDayState, ETimeChangeReason.None, false);
			if (m_SunShaftsController != null && m_SkyController != null && m_SkyController.SunInstance != null)
			{
				m_SunShaftsController.sunTransform = m_SkyController.SunInstance.transform;
			}
		}
	}

	public Vignetting VignettingController
	{
		get => m_VignettingController;
	    set
		{
			m_VignettingController = value;
			ChangeDayState(m_TargetDayState, ETimeChangeReason.None, false);
		}
	}

	public ColorCorrection ColorCorrectionController
	{
		get => m_ColorCorrectionController;
	    set
		{
			m_ColorCorrectionController = value;
			ChangeDayState(m_TargetDayState, ETimeChangeReason.None, false);
		}
	}

	public EdgeDetectionEffect EdgeDetectionEffectController
	{
		get => m_EdgeDetectionEffectController;
	    set
		{
			m_EdgeDetectionEffectController = value;
			ChangeDayState(m_TargetDayState, ETimeChangeReason.None, false);
		}
	}

	public ColorCorrectionEffectBlend ColorCorrectionEffectBlendController
	{
		get => m_darkDungeonColorCorrectionController;
	    set
		{
			m_darkDungeonColorCorrectionController = value;
			CheckBuffs();
		}
	}

	public void Register(Object owner, ZoneEffectSetting setting, Int32 priority, Single blendTime)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (setting == null)
		{
			throw new ArgumentNullException("setting");
		}
		if (m_ActiveEffects.Exists((Effect a) => a.Owner == owner))
		{
			return;
		}
		Int32 num = m_ActiveEffects.FindIndex((Effect a) => a.Priority > priority);
		if (num == -1)
		{
			m_CurrentEffect = new Effect(owner, setting, priority, blendTime);
			m_ActiveEffects.Add(m_CurrentEffect);
		}
		else
		{
			m_ActiveEffects.Insert(num, new Effect(owner, setting, priority, blendTime));
			m_CurrentEffect = m_ActiveEffects[m_ActiveEffects.Count - 1];
		}
		ChangeDayState(m_TargetDayState, ETimeChangeReason.None, true);
		CheckBuffs();
	}

	public void Deregister(Object owner)
	{
		Int32 num = m_ActiveEffects.FindIndex((Effect a) => a.Owner == owner);
		if (num != -1)
		{
			m_ActiveEffects.RemoveAt(num);
			if (m_CurrentEffect.Owner == owner)
			{
				m_CurrentEffect = default(Effect);
				if (m_ActiveEffects.Count > 0)
				{
					m_CurrentEffect = m_ActiveEffects[m_ActiveEffects.Count - 1];
				}
				else if (m_DefaultSetting != null)
				{
					m_CurrentEffect = new Effect(null, m_DefaultSetting, Int32.MinValue, m_DefaultSettingBlendTime);
				}
			}
		}
		ChangeDayState(m_TargetDayState, ETimeChangeReason.None, true);
		CheckBuffs();
	}

	public void ChangeDayState(EDayState dayState, ETimeChangeReason reason, Boolean resetBlendTime = true)
	{
		if (m_CurrentEffect.Setting == null)
		{
			return;
		}
		m_TargetDayState = dayState;
		CopyCurrentSettings();
		if (m_CurrentEffect.Setting.IgnoreDayTime)
		{
			m_TargetSettings = m_CurrentEffect.Setting.Dawn;
		}
		else
		{
			switch (dayState)
			{
			case EDayState.DAWN:
				m_TargetSettings = m_CurrentEffect.Setting.Dawn;
				break;
			case EDayState.DAY:
				m_TargetSettings = m_CurrentEffect.Setting.Day;
				break;
			case EDayState.DUSK:
				m_TargetSettings = m_CurrentEffect.Setting.Dusk;
				break;
			case EDayState.NIGHT:
				m_TargetSettings = m_CurrentEffect.Setting.Night;
				break;
			default:
				throw new NotImplementedException();
			}
		}
		Single num;
		switch (dayState)
		{
		case EDayState.DAWN:
			num = m_TimeOfDayDawn;
			break;
		case EDayState.DAY:
			num = m_TimeOfDayDay;
			break;
		case EDayState.DUSK:
			num = m_TimeOfDayDusk;
			break;
		case EDayState.NIGHT:
			num = m_TimeOfDayNight;
			break;
		default:
			throw new NotImplementedException();
		}
		if (m_CurrentDayTime == -3.40282347E+38f)
		{
			m_CurrentDayTime = num;
		}
		if (m_TargetDayTime > num)
		{
			m_CurrentDayTime -= 24f;
		}
		m_TargetDayTime = num;
		if (resetBlendTime)
		{
			m_BlendTime = 0f;
		}
		if (reason == ETimeChangeReason.Resting)
		{
			BlendValues(1f);
		}
		else
		{
			enabled = true;
		}
	}

	private void Awake()
	{
		Instance = this;
		if (m_DefaultSetting != null)
		{
			m_CurrentEffect = new Effect(null, m_DefaultSetting, Int32.MinValue, m_DefaultSettingBlendTime);
			Sky skyController = m_SkyController;
			if (skyController != null)
			{
				CloudParameters clouds = m_DefaultSetting.Dawn.SkyParameter.Clouds;
				skyController.Clouds.Sharpness = clouds.Sharpness;
				skyController.Clouds.Scale1 = clouds.Scale1;
				skyController.Clouds.Scale2 = clouds.Scale2;
				skyController.Clouds.ShadowProjector = false;
				skyController.Clouds.ShadowStrength = clouds.ShadowStrength;
			}
		}
		if (m_WaterBase != null)
		{
			SpecularLighting component = m_WaterBase.GetComponent<SpecularLighting>();
			if (component != null && m_SkyController != null && m_SkyController.SunInstance != null)
			{
				component.specularLight = m_SkyController.SunInstance.transform;
			}
		}
	}

	private void Start()
	{
		ChangeDayState(LegacyLogic.Instance.GameTime.DayState, ETimeChangeReason.Resting, true);
		LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnDayStateChanged));
		LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_BUFF_ADDED, new EventHandler(OnBuffAdded));
		LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_BUFF_REMOVED, new EventHandler(OnBuffRemoved));
		LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INDOOR_SCENE_CLOSED, new EventHandler(OnSceneClosed));
		CheckBuffs();
	}

	private void OnEnable()
	{
		m_BlendTime = 0f;
	}

	private void OnDestroy()
	{
		LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnDayStateChanged));
		LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_ADDED, new EventHandler(OnBuffAdded));
		LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_REMOVED, new EventHandler(OnBuffRemoved));
		LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INDOOR_SCENE_CLOSED, new EventHandler(OnSceneClosed));
	}

	private void Update()
	{
		if (m_CurrentEffect.Setting == null || m_TargetSettings == null)
		{
			enabled = false;
			return;
		}
		m_BlendTime += Time.deltaTime;
		Single num = (m_CurrentEffect.BlendTime != 0f) ? Mathf.Clamp01(m_BlendTime / m_CurrentEffect.BlendTime) : 1f;
		BlendValues(num);
		enabled = (num < 1f);
	}

	private void BlendValues(Single blend)
	{
		BlendSky(blend);
		BlendBloom(blend);
		BlendVignetting(blend);
		BlendGlobalFog(m_GlobalFogNearController, m_SourceSettings.GlobalFogNearParameter, m_TargetSettings.GlobalFogNearParameter, blend);
		BlendGlobalFog(m_GlobalFogFarController, m_SourceSettings.GlobalFogFarParameter, m_TargetSettings.GlobalFogFarParameter, blend);
		BlendGlobalFog(m_GlobalFogGroundController, m_SourceSettings.GlobalFogGroundParameter, m_TargetSettings.GlobalFogGroundParameter, blend);
		BlendSunShafts(blend);
		BlendWater(blend);
		BlendColorCorrection(blend);
		BlendEdgeDetection(blend);
	}

	private void BlendSky(Single blend)
	{
		Sky skyController = m_SkyController;
		if (skyController == null)
		{
			return;
		}
		ZoneEffectSetting.SkyParameterSet skyParameter = m_SourceSettings.SkyParameter;
		ZoneEffectSetting.SkyParameterSet skyParameter2 = m_TargetSettings.SkyParameter;
		if (m_CurrentEffect.Setting.NoSkyLight)
		{
			for (Int32 i = m_ActiveEffects.Count - 2; i >= 0; i--)
			{
				ZoneEffectSetting setting = m_ActiveEffects[i].Setting;
				if (!setting.NoSkyLight)
				{
					switch (m_TargetDayState)
					{
					case EDayState.DAWN:
						skyParameter2 = setting.Dawn.SkyParameter;
						break;
					case EDayState.DAY:
						skyParameter2 = setting.Day.SkyParameter;
						break;
					case EDayState.DUSK:
						skyParameter2 = setting.Dusk.SkyParameter;
						break;
					case EDayState.NIGHT:
						skyParameter2 = setting.Night.SkyParameter;
						break;
					}
					break;
				}
			}
		}
		skyController.Cycle.TimeOfDay = Mathf.Lerp(m_CurrentDayTime, m_TargetDayTime, blend);
		if (m_CurrentEffect.Setting.NoSkyLight)
		{
			skyController.Day.AdditiveColor = Color.Lerp(skyParameter.Day.AdditiveColor, Color.clear, blend);
		}
		else
		{
			skyController.Day.AdditiveColor = Color.Lerp(skyParameter.Day.AdditiveColor, skyParameter2.Day.AdditiveColor, blend);
		}
		skyController.Day.RayleighMultiplier = Mathf.Lerp(skyParameter.Day.RayleighMultiplier, skyParameter2.Day.RayleighMultiplier, blend);
		skyController.Day.MieMultiplier = Mathf.Lerp(skyParameter.Day.MieMultiplier, skyParameter2.Day.MieMultiplier, blend);
		skyController.Day.Brightness = Mathf.Lerp(skyParameter.Day.Brightness, skyParameter2.Day.Brightness, blend);
		skyController.Day.Directionality = Mathf.Lerp(skyParameter.Day.Directionality, skyParameter2.Day.Directionality, blend);
		skyController.Day.Haziness = Mathf.Lerp(skyParameter.Day.Haziness, skyParameter2.Day.Haziness, blend);
		if (m_CurrentEffect.Setting.NoSkyLight)
		{
			skyController.Night.AdditiveColor = Color.Lerp(skyParameter.Night.AdditiveColor, Color.clear, blend);
		}
		else
		{
			skyController.Night.AdditiveColor = Color.Lerp(skyParameter.Night.AdditiveColor, skyParameter2.Night.AdditiveColor, blend);
		}
		skyController.Night.HazeColor = Color.Lerp(skyParameter.Night.HazeColor, skyParameter2.Night.HazeColor, blend);
		skyController.Night.Haziness = Mathf.Lerp(skyParameter.Night.Haziness, skyParameter2.Night.Haziness, blend);
		skyController.Sun.LightColor = Color.Lerp(skyParameter.Sun.LightColor, skyParameter2.Sun.LightColor, blend);
		if (m_CurrentEffect.Setting.NoSkyLight)
		{
			skyController.Sun.LightIntensity = Mathf.Lerp(skyParameter.Sun.LightIntensity, 0f, blend);
		}
		else
		{
			skyController.Sun.LightIntensity = Mathf.Lerp(skyParameter.Sun.LightIntensity, skyParameter2.Sun.LightIntensity, blend);
		}
		skyController.Sun.Falloff = Mathf.Lerp(skyParameter.Sun.Falloff, skyParameter2.Sun.Falloff, blend);
		skyController.Sun.Coloring = Mathf.Lerp(skyParameter.Sun.Coloring, skyParameter2.Sun.Coloring, blend);
		skyController.Sun.ShadowStrength = Mathf.Lerp(skyParameter.Sun.ShadowStrength, skyParameter2.Sun.ShadowStrength, blend);
		skyController.Moon.LightColor = Color.Lerp(skyParameter.Moon.LightColor, skyParameter2.Moon.LightColor, blend);
		if (m_CurrentEffect.Setting.NoSkyLight)
		{
			skyController.Moon.LightIntensity = Mathf.Lerp(skyParameter.Moon.LightIntensity, 0f, blend);
		}
		else if (LightSpellIsActive() && m_TargetDayState == EDayState.NIGHT)
		{
			skyController.Moon.LightIntensity = Mathf.Lerp(skyParameter.Moon.LightIntensity, 1f, blend);
		}
		else
		{
			skyController.Moon.LightIntensity = Mathf.Lerp(skyParameter.Moon.LightIntensity, skyParameter2.Moon.LightIntensity, blend);
		}
		skyController.Moon.Phase = Mathf.Lerp(skyParameter.Moon.Phase, skyParameter2.Moon.Phase, blend);
		skyController.Moon.Halo = Mathf.Lerp(skyParameter.Moon.Halo, skyParameter2.Moon.Halo, blend);
		skyController.Moon.ShadowStrength = Mathf.Lerp(skyParameter.Moon.ShadowStrength, skyParameter2.Moon.ShadowStrength, blend);
		skyController.Clouds.Tone = Mathf.Lerp(skyParameter.Clouds.Tone, skyParameter2.Clouds.Tone, blend);
		skyController.Clouds.Shading = Mathf.Lerp(skyParameter.Clouds.Shading, skyParameter2.Clouds.Shading, blend);
		skyController.Clouds.Density = Mathf.Lerp(skyParameter.Clouds.Density, skyParameter2.Clouds.Density, blend);
		RenderSettings.ambientLight = Color.Lerp(skyParameter.AmbientColor, skyParameter2.AmbientColor, blend);
	}

	private void BlendVignetting(Single blend)
	{
		Vignetting vignettingController = m_VignettingController;
		if (vignettingController == null)
		{
			return;
		}
		ZoneEffectSetting.VignettingParameterSet vignettingParameter = m_SourceSettings.VignettingParameter;
		ZoneEffectSetting.VignettingParameterSet vignettingParameter2 = m_TargetSettings.VignettingParameter;
		vignettingController.axialAberration = Mathf.Lerp(vignettingParameter.axialAberration, vignettingParameter2.axialAberration, blend);
		vignettingController.blur = Mathf.Lerp(vignettingParameter.blur, vignettingParameter2.blur, blend);
		vignettingController.blurDistance = Mathf.Lerp(vignettingParameter.blurDistance, vignettingParameter2.blurDistance, blend);
		vignettingController.blurSpread = Mathf.Lerp(vignettingParameter.blurSpread, vignettingParameter2.blurSpread, blend);
		vignettingController.chromaticAberration = Mathf.Lerp(vignettingParameter.chromaticAberration, vignettingParameter2.chromaticAberration, blend);
		vignettingController.intensity = Mathf.Lerp(vignettingParameter.intensity, vignettingParameter2.intensity, blend);
		vignettingController.luminanceDependency = Mathf.Lerp(vignettingParameter.luminanceDependency, vignettingParameter2.luminanceDependency, blend);
		vignettingController.enabled = (vignettingController.intensity > 0f);
	}

	private void BlendBloom(Single blend)
	{
		Bloom bloomController = m_BloomController;
		if (bloomController == null)
		{
			return;
		}
		ZoneEffectSetting.BloomParameters bloomParameter = m_SourceSettings.BloomParameter;
		ZoneEffectSetting.BloomParameters bloomParameter2 = m_TargetSettings.BloomParameter;
		bloomController.bloomBlurIterations = (Int32)Mathf.Lerp(bloomParameter.bloomBlurIterations, bloomParameter2.bloomBlurIterations, blend);
		bloomController.bloomIntensity = Mathf.Lerp(bloomParameter.bloomIntensity, bloomParameter2.bloomIntensity, blend);
		bloomController.bloomThreshhold = Mathf.Lerp(bloomParameter.bloomThreshhold, bloomParameter2.bloomThreshhold, blend);
		bloomController.bloomThreshholdColor = Color.Lerp(bloomParameter.bloomThreshholdColor, bloomParameter2.bloomThreshholdColor, blend);
		bloomController.blurWidth = Mathf.Lerp(bloomParameter.blurWidth, bloomParameter2.blurWidth, blend);
		bloomController.flareColorA = Color.Lerp(bloomParameter.flareColorA, bloomParameter2.flareColorA, blend);
		bloomController.flareColorB = Color.Lerp(bloomParameter.flareColorB, bloomParameter2.flareColorB, blend);
		bloomController.flareColorC = Color.Lerp(bloomParameter.flareColorC, bloomParameter2.flareColorC, blend);
		bloomController.flareColorD = Color.Lerp(bloomParameter.flareColorD, bloomParameter2.flareColorD, blend);
		bloomController.flareRotation = Mathf.Lerp(bloomParameter.flareRotation, bloomParameter2.flareRotation, blend);
		bloomController.hollyStretchWidth = Mathf.Lerp(bloomParameter.hollyStretchWidth, bloomParameter2.hollyStretchWidth, blend);
		bloomController.hollywoodFlareBlurIterations = (Int32)Mathf.Lerp(bloomParameter.hollywoodFlareBlurIterations, bloomParameter2.hollywoodFlareBlurIterations, blend);
		bloomController.lensflareIntensity = Mathf.Lerp(bloomParameter.lensflareIntensity, bloomParameter2.lensflareIntensity, blend);
		bloomController.lensFlareSaturation = Mathf.Lerp(bloomParameter.lensFlareSaturation, bloomParameter2.lensFlareSaturation, blend);
		bloomController.lensflareThreshhold = Mathf.Lerp(bloomParameter.lensflareThreshhold, bloomParameter2.lensflareThreshhold, blend);
		bloomController.sepBlurSpread = Mathf.Lerp(bloomParameter.sepBlurSpread, bloomParameter2.sepBlurSpread, blend);
		bloomController.enabled = (bloomController.bloomIntensity > 0f);
	}

	private void BlendGlobalFog(GlobalFog fog, ZoneEffectSetting.GlobalFogParameters source, ZoneEffectSetting.GlobalFogParameters target, Single blend)
	{
		if (fog == null)
		{
			return;
		}
		if (source.globalDensity == 0f)
		{
			fog.globalFogColor = target.globalFogColor;
			fog.height = target.height;
			fog.heightScale = target.heightScale;
			fog.startDistance = target.startDistance;
		}
		else if (target.globalDensity == 0f)
		{
			fog.globalFogColor = source.globalFogColor;
			fog.height = source.height;
			fog.heightScale = source.heightScale;
			fog.startDistance = source.startDistance;
		}
		else
		{
			fog.globalFogColor = Color.Lerp(source.globalFogColor, target.globalFogColor, blend);
			fog.height = Mathf.Lerp(source.height, target.height, blend);
			fog.heightScale = Mathf.Lerp(source.heightScale, target.heightScale, blend);
			fog.startDistance = Mathf.Lerp(source.startDistance, target.startDistance, blend);
		}
		fog.globalDensity = Mathf.Lerp(source.globalDensity, target.globalDensity, blend);
		fog.fogMode = target.mode;
		fog.enabled = (fog.globalDensity > 0f);
	}

	private void BlendSunShafts(Single blend)
	{
		SunShafts sunShaftsController = m_SunShaftsController;
		if (sunShaftsController == null)
		{
			return;
		}
		ZoneEffectSetting.SunShaftsParameters sunShaftsParameter = m_SourceSettings.SunShaftsParameter;
		ZoneEffectSetting.SunShaftsParameters sunShaftsParameter2 = m_TargetSettings.SunShaftsParameter;
		sunShaftsController.sunColor = Color.Lerp(sunShaftsParameter.SunColor, sunShaftsParameter2.SunColor, blend);
		sunShaftsController.sunShaftIntensity = Mathf.Lerp(sunShaftsParameter.SunShaftIntensity, sunShaftsParameter2.SunShaftIntensity, blend);
		sunShaftsController.enabled = (sunShaftsController.sunShaftIntensity > 0f);
	}

	private void BlendColorCorrection(Single blend)
	{
		ColorCorrection colorCorrectionController = m_ColorCorrectionController;
		if (colorCorrectionController == null)
		{
			return;
		}
		colorCorrectionController.BlendLenth = ((blend < 1f) ? m_CurrentEffect.BlendTime : 0f);
		colorCorrectionController.BlendTarget = m_TargetSettings.ColorCorrectionParameter.Texture;
	}

	private void BlendWater(Single blend)
	{
		if (m_WaterBase == null)
		{
			return;
		}
		Material sharedMaterial = m_WaterBase.sharedMaterial;
		if (sharedMaterial == null)
		{
			return;
		}
		ZoneEffectSetting.WaterParameters waterParameter = m_SourceSettings.WaterParameter;
		ZoneEffectSetting.WaterParameters waterParameter2 = m_TargetSettings.WaterParameter;
		sharedMaterial.SetColor("_BaseColor", Color.Lerp(waterParameter.Refaction, waterParameter2.Refaction, blend));
		sharedMaterial.SetColor("_ReflectionColor", Color.Lerp(waterParameter.Reflection, waterParameter2.Reflection, blend));
		sharedMaterial.SetColor("_SpecularColor", Color.Lerp(waterParameter.Specular, waterParameter2.Specular, blend));
		sharedMaterial.SetFloat("_Shininess", Mathf.Lerp(waterParameter.SpecularPower, waterParameter2.SpecularPower, blend));
	}

	private void BlendEdgeDetection(Single blend)
	{
		EdgeDetectionEffect edgeDetectionEffectController = m_EdgeDetectionEffectController;
		if (edgeDetectionEffectController == null)
		{
			return;
		}
		ZoneEffectSetting.EdgeDetectionParameters edgeDetectionParameter = m_SourceSettings.EdgeDetectionParameter;
		ZoneEffectSetting.EdgeDetectionParameters edgeDetectionParameter2 = m_TargetSettings.EdgeDetectionParameter;
		edgeDetectionEffectController.SensitivityDepth = Mathf.Lerp(edgeDetectionParameter.SensitivityDepth, edgeDetectionParameter2.SensitivityDepth, blend);
		edgeDetectionEffectController.SensitivityNormals = Mathf.Lerp(edgeDetectionParameter.SensitivityNormals, edgeDetectionParameter2.SensitivityNormals, blend);
		edgeDetectionEffectController.SampleDist = Mathf.Lerp(edgeDetectionParameter.SampleDist, edgeDetectionParameter2.SampleDist, blend);
		edgeDetectionEffectController.EdgeItensity = Mathf.Lerp(edgeDetectionParameter.EdgeItensity, edgeDetectionParameter2.EdgeItensity, blend);
		edgeDetectionEffectController.CapColor = Color.Lerp(edgeDetectionParameter.CapColor, edgeDetectionParameter2.CapColor, blend);
		edgeDetectionEffectController.ColorScale = Mathf.Lerp(edgeDetectionParameter.ColorScale, edgeDetectionParameter2.ColorScale, blend);
		edgeDetectionEffectController.AddColor = Color.Lerp(edgeDetectionParameter.AddColor, edgeDetectionParameter2.AddColor, blend);
		edgeDetectionEffectController.StartDistance = Mathf.Lerp(edgeDetectionParameter.StartDistance, edgeDetectionParameter2.StartDistance, blend);
		edgeDetectionEffectController.DistanceScale = Mathf.Lerp(edgeDetectionParameter.DistanceScale, edgeDetectionParameter2.DistanceScale, blend);
		edgeDetectionEffectController.Spread = Mathf.Lerp(edgeDetectionParameter.Spread, edgeDetectionParameter2.Spread, blend);
		edgeDetectionEffectController.enabled = (edgeDetectionEffectController.EdgeItensity > 0f);
	}

	private void CopyCurrentSettings()
	{
		if (m_SkyController != null)
		{
			m_CurrentDayTime = m_SkyController.Cycle.TimeOfDay;
			m_SourceSettings.SkyParameter.CopyFrom(m_SkyController.Day);
			m_SourceSettings.SkyParameter.CopyFrom(m_SkyController.Night);
			m_SourceSettings.SkyParameter.CopyFrom(m_SkyController.Sun);
			m_SourceSettings.SkyParameter.CopyFrom(m_SkyController.Moon);
			m_SourceSettings.SkyParameter.CopyFrom(m_SkyController.Clouds);
		}
		if (m_BloomController != null)
		{
			m_SourceSettings.BloomParameter.bloomBlurIterations = m_BloomController.bloomBlurIterations;
			m_SourceSettings.BloomParameter.bloomIntensity = m_BloomController.bloomIntensity;
			m_SourceSettings.BloomParameter.bloomThreshhold = m_BloomController.bloomThreshhold;
			m_SourceSettings.BloomParameter.bloomThreshholdColor = m_BloomController.bloomThreshholdColor;
			m_SourceSettings.BloomParameter.blurWidth = m_BloomController.blurWidth;
			m_SourceSettings.BloomParameter.flareColorA = m_BloomController.flareColorA;
			m_SourceSettings.BloomParameter.flareColorB = m_BloomController.flareColorB;
			m_SourceSettings.BloomParameter.flareColorC = m_BloomController.flareColorC;
			m_SourceSettings.BloomParameter.flareColorD = m_BloomController.flareColorD;
			m_SourceSettings.BloomParameter.flareRotation = m_BloomController.flareRotation;
			m_SourceSettings.BloomParameter.hollyStretchWidth = m_BloomController.hollyStretchWidth;
			m_SourceSettings.BloomParameter.hollywoodFlareBlurIterations = m_BloomController.hollywoodFlareBlurIterations;
			m_SourceSettings.BloomParameter.lensflareIntensity = m_BloomController.lensflareIntensity;
			m_SourceSettings.BloomParameter.lensFlareSaturation = m_BloomController.lensFlareSaturation;
			m_SourceSettings.BloomParameter.lensflareThreshhold = m_BloomController.lensflareThreshhold;
			m_SourceSettings.BloomParameter.sepBlurSpread = m_BloomController.sepBlurSpread;
		}
		if (m_GlobalFogNearController != null)
		{
			m_SourceSettings.GlobalFogNearParameter.globalDensity = m_GlobalFogNearController.globalDensity;
			m_SourceSettings.GlobalFogNearParameter.globalFogColor = m_GlobalFogNearController.globalFogColor;
			m_SourceSettings.GlobalFogNearParameter.height = m_GlobalFogNearController.height;
			m_SourceSettings.GlobalFogNearParameter.heightScale = m_GlobalFogNearController.heightScale;
			m_SourceSettings.GlobalFogNearParameter.startDistance = m_GlobalFogNearController.startDistance;
		}
		if (m_GlobalFogFarController != null)
		{
			m_SourceSettings.GlobalFogFarParameter.globalDensity = m_GlobalFogFarController.globalDensity;
			m_SourceSettings.GlobalFogFarParameter.globalFogColor = m_GlobalFogFarController.globalFogColor;
			m_SourceSettings.GlobalFogFarParameter.height = m_GlobalFogFarController.height;
			m_SourceSettings.GlobalFogFarParameter.heightScale = m_GlobalFogFarController.heightScale;
			m_SourceSettings.GlobalFogFarParameter.startDistance = m_GlobalFogFarController.startDistance;
		}
		if (m_GlobalFogGroundController != null)
		{
			m_SourceSettings.GlobalFogGroundParameter.globalDensity = m_GlobalFogGroundController.globalDensity;
			m_SourceSettings.GlobalFogGroundParameter.globalFogColor = m_GlobalFogGroundController.globalFogColor;
			m_SourceSettings.GlobalFogGroundParameter.height = m_GlobalFogGroundController.height;
			m_SourceSettings.GlobalFogGroundParameter.heightScale = m_GlobalFogGroundController.heightScale;
			m_SourceSettings.GlobalFogGroundParameter.startDistance = m_GlobalFogGroundController.startDistance;
		}
		if (m_SunShaftsController != null)
		{
			m_SourceSettings.SunShaftsParameter.SunColor = m_SunShaftsController.sunColor;
			m_SourceSettings.SunShaftsParameter.SunShaftIntensity = m_SunShaftsController.sunShaftIntensity;
		}
		if (m_VignettingController != null)
		{
			m_SourceSettings.VignettingParameter.axialAberration = m_VignettingController.axialAberration;
			m_SourceSettings.VignettingParameter.blur = m_VignettingController.blur;
			m_SourceSettings.VignettingParameter.blurDistance = m_VignettingController.blurDistance;
			m_SourceSettings.VignettingParameter.blurSpread = m_VignettingController.blurSpread;
			m_SourceSettings.VignettingParameter.chromaticAberration = m_VignettingController.chromaticAberration;
			m_SourceSettings.VignettingParameter.intensity = m_VignettingController.intensity;
			m_SourceSettings.VignettingParameter.luminanceDependency = m_VignettingController.luminanceDependency;
		}
		if (m_EdgeDetectionEffectController != null)
		{
			m_SourceSettings.EdgeDetectionParameter.SensitivityDepth = m_EdgeDetectionEffectController.SensitivityDepth;
			m_SourceSettings.EdgeDetectionParameter.SensitivityNormals = m_EdgeDetectionEffectController.SensitivityNormals;
			m_SourceSettings.EdgeDetectionParameter.SampleDist = m_EdgeDetectionEffectController.SampleDist;
			m_SourceSettings.EdgeDetectionParameter.EdgeItensity = m_EdgeDetectionEffectController.EdgeItensity;
			m_SourceSettings.EdgeDetectionParameter.CapColor = m_EdgeDetectionEffectController.CapColor;
			m_SourceSettings.EdgeDetectionParameter.ColorScale = m_EdgeDetectionEffectController.ColorScale;
			m_SourceSettings.EdgeDetectionParameter.AddColor = m_EdgeDetectionEffectController.AddColor;
			m_SourceSettings.EdgeDetectionParameter.StartDistance = m_EdgeDetectionEffectController.StartDistance;
			m_SourceSettings.EdgeDetectionParameter.DistanceScale = m_EdgeDetectionEffectController.DistanceScale;
			m_SourceSettings.EdgeDetectionParameter.Spread = m_EdgeDetectionEffectController.Spread;
		}
		if (m_WaterBase != null)
		{
			m_SourceSettings.WaterParameter.Refaction = m_WaterBase.sharedMaterial.GetColor("_BaseColor");
			m_SourceSettings.WaterParameter.Reflection = m_WaterBase.sharedMaterial.GetColor("_ReflectionColor");
			m_SourceSettings.WaterParameter.Specular = m_WaterBase.sharedMaterial.GetColor("_SpecularColor");
			m_SourceSettings.WaterParameter.SpecularPower = m_WaterBase.sharedMaterial.GetFloat("_Shininess");
		}
		m_SourceSettings.SkyParameter.AmbientColor = RenderSettings.ambientLight;
	}

	private void OnDayStateChanged(Object sender, EventArgs e)
	{
		GameTimeEventArgs gameTimeEventArgs = (GameTimeEventArgs)e;
		ChangeDayState(gameTimeEventArgs.CurrentDayState, gameTimeEventArgs.Reason, true);
	}

	private void OnBuffAdded(Object sender, EventArgs e)
	{
		if (CurrentSetting == null)
		{
			return;
		}
		PartyBuff partyBuff = (PartyBuff)sender;
		if (partyBuff.Type == EPartyBuffs.LIGHT_ORB || partyBuff.Type == EPartyBuffs.TORCHLIGHT || partyBuff.Type == EPartyBuffs.DARK_VISION)
		{
			if (CurrentSetting.IsDarkDungeonArea && ColorCorrectionEffectBlendController.enabled)
			{
				ColorCorrectionEffectBlendController.Deactivate();
			}
			else if (!CurrentSetting.NoSkyLight && !CurrentSetting.IsDarkDungeonArea && CurrentDayState == EDayState.NIGHT)
			{
				m_SkyController.Moon.LightIntensity = 1f;
			}
		}
	}

	private void OnBuffRemoved(Object sender, EventArgs e)
	{
		PartyBuff partyBuff = sender as PartyBuff;
		if (CurrentSetting == null || partyBuff == null)
		{
			return;
		}
		if (partyBuff.Type == EPartyBuffs.LIGHT_ORB || partyBuff.Type == EPartyBuffs.TORCHLIGHT || partyBuff.Type == EPartyBuffs.DARK_VISION)
		{
			if (CurrentSetting.IsDarkDungeonArea && !ColorCorrectionEffectBlendController.enabled && !LightSpellIsActive())
			{
				ColorCorrectionEffectBlendController.Activate();
			}
			else if (!CurrentSetting.NoSkyLight && !CurrentSetting.IsDarkDungeonArea && CurrentDayState == EDayState.NIGHT && !LightSpellIsActive())
			{
				m_SkyController.Moon.LightIntensity = CurrentSetting.Night.SkyParameter.Moon.LightIntensity;
			}
		}
	}

	private void OnSceneClosed(Object sender, EventArgs e)
	{
		ChangeDayState(CurrentDayState, ETimeChangeReason.Resting, true);
	}

	private void CheckBuffs()
	{
		if (m_darkDungeonColorCorrectionController == null)
		{
			return;
		}
		if (CurrentSetting == null)
		{
			return;
		}
		Boolean flag = LightSpellIsActive();
		if (CurrentSetting.IsDarkDungeonArea && !flag)
		{
			if (!ColorCorrectionEffectBlendController.enabled)
			{
				ColorCorrectionEffectBlendController.Activate();
			}
		}
		else if ((!CurrentSetting.IsDarkDungeonArea || flag) && ColorCorrectionEffectBlendController.enabled)
		{
			ColorCorrectionEffectBlendController.Deactivate();
		}
	}

	private Boolean LightSpellIsActive()
	{
		PartyBuffHandler buffs = LegacyLogic.Instance.WorldManager.Party.Buffs;
		return buffs == null || (buffs.HasBuff(EPartyBuffs.LIGHT_ORB) || buffs.HasBuff(EPartyBuffs.TORCHLIGHT) || buffs.HasBuff(EPartyBuffs.DARK_VISION));
	}

	private struct Effect
	{
		public Object Owner;

		public ZoneEffectSetting Setting;

		public Int32 Priority;

		public Single BlendTime;

		public Effect(Object owner, ZoneEffectSetting setting, Int32 priority, Single blendTime)
		{
			Owner = owner;
			Setting = setting;
			Priority = priority;
			BlendTime = blendTime;
		}
	}
}
