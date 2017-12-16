using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	[ExecuteInEditMode]
	public class Sky : MonoBehaviour
	{
		private const Single lambda_r1 = 6.5E-07f;

		private const Single lambda_r2 = 4.225E-13f;

		private const Single lambda_r4 = 1.78506256E-25f;

		private const Single lambda_g1 = 5.5E-07f;

		private const Single lambda_g2 = 3.02499968E-13f;

		private const Single lambda_g4 = 9.150623E-26f;

		private const Single lambda_b1 = 4.5E-07f;

		private const Single lambda_b2 = 2.02499991E-13f;

		private const Single lambda_b4 = 4.10062471E-26f;

		private const Single pi = 3.14159274f;

		private const Single pi2 = 9.869605f;

		private const Single pi3 = 31.006279f;

		private const Single pi4 = 97.4091f;

		public GameObject SunInstance;

		public GameObject MoonInstance;

		public Light MoonLightInstance;

		public GameObject AtmosphereInstance;

		public GameObject CloudInstance;

		public GameObject SpaceInstance;

		public GameObject ProjectorInstance;

		public Boolean LinearLighting;

		public CycleParameters Cycle;

		public DayParameters Day;

		public NightParameters Night;

		public SunParameters Sun;

		public MoonParameters Moon;

		public CloudParameters Clouds;

		public WorldParameters World;

		internal Transform DomeTransform;

		internal Material SpaceShader;

		internal Material AtmosphereShader;

		internal Material CloudShader;

		internal Projector ShadowProjector;

		internal Transform SunTransform;

		internal Renderer SunRenderer;

		internal Material SunShader;

		internal WrapperLight SunLight;

		internal Transform MoonTransform;

		internal Renderer MoonRenderer;

		internal Material MoonShader;

		internal Material MoonHalo;

		internal WrapperLight MoonLight;

		internal SkyAnimation Animation;

		private Transform m_CachedTransform;

		private Vector3 betaRayleigh;

		private Vector3 betaRayleighTheta;

		private Vector3 betaMie;

		private Vector3 betaMieTheta;

		private Vector3 henyeyGreenstein;

		private Vector3 sunDirection;

		private Vector3 moonDirection;

		public new Transform transform
		{
			get
			{
				if (m_CachedTransform == null)
				{
					m_CachedTransform = base.transform;
				}
				return m_CachedTransform;
			}
			private set => m_CachedTransform = value;
		}

		internal Boolean IsDay => SunLight.enabled;

	    internal Boolean IsNight => MoonLight.enabled;

	    internal Single Radius => DomeTransform.localScale.x;

	    internal Single Gamma => (!LinearLighting) ? 2.2f : 1f;

	    internal Single OneOverGamma => (!LinearLighting) ? 0.454545438f : 1f;

	    internal Single SunZenith { get; private set; }

		internal Single MoonZenith { get; private set; }

		protected virtual void Awake()
		{
			transform = base.transform;
		}

		protected void OnEnable()
		{
			DomeTransform = transform;
			Animation = GetComponent<SkyAnimation>();
			if (SpaceInstance)
			{
				SpaceShader = SpaceInstance.renderer.sharedMaterial;
			}
			if (AtmosphereInstance)
			{
				AtmosphereShader = AtmosphereInstance.renderer.sharedMaterial;
			}
			if (CloudInstance)
			{
				CloudShader = CloudInstance.renderer.sharedMaterial;
			}
			if (ProjectorInstance)
			{
				ProjectorInstance.gameObject.SetActive(false);
			}
			if (SunInstance)
			{
				SunTransform = SunInstance.transform;
				SunRenderer = SunInstance.renderer;
				SunShader = SunRenderer.sharedMaterial;
				SunLight = new WrapperLight(SunInstance.light);
			}
			else
			{
				Debug.LogError("Sun instance reference not set. Disabling script.");
				enabled = false;
			}
			if (MoonInstance)
			{
				MoonTransform = MoonInstance.transform;
				MoonRenderer = MoonInstance.renderer;
				if (MoonRenderer != null)
				{
					MoonShader = MoonRenderer.sharedMaterial;
				}
				MoonLight = new WrapperLight((!(MoonInstance.light == null)) ? MoonInstance.light : MoonLightInstance);
				if (MoonRenderer != null && MoonRenderer.sharedMaterials.Length > 1)
				{
					MoonHalo = MoonRenderer.sharedMaterials[1];
				}
			}
			else
			{
				Debug.LogError("Moon instance reference not set. Disabling script.");
				enabled = false;
			}
		}

		protected void Update()
		{
			ValidateScriptParameters();
			SetupSunAndMoon();
			SetupRayleighScattering();
			SetupMieScattering();
			SetupHenyeyGreensteinPhaseFunction();
			SetupShaderParameters();
		}

		private void SetupShaderParameters()
		{
			Color color = ColorPow(SunLight.color, Gamma);
			Color a = ColorPow(MoonLight.color, Gamma);
			Color color2 = ColorPow(Day.AdditiveColor, Gamma);
			Color color3 = ColorPow(Night.HazeColor * Night.HazeColor.a, Gamma);
			if (World.SetFogColor)
			{
				RenderSettings.fogColor = FogColor();
			}
			if (World.SetAmbientLight)
			{
				RenderSettings.ambientLight = AmbientColor();
			}
			if (AtmosphereShader != null)
			{
				AtmosphereShader.SetColor("_SunColor", color);
				AtmosphereShader.SetVector("_SunDirection", sunDirection);
				AtmosphereShader.SetFloat("_OneOverGamma", OneOverGamma);
				AtmosphereShader.SetFloat("_DayBrightness", Day.Brightness);
				AtmosphereShader.SetFloat("_DayHaziness", Day.Haziness);
				AtmosphereShader.SetFloat("_NightHaziness", Night.Haziness);
				AtmosphereShader.SetColor("_DayColor", color2);
				AtmosphereShader.SetColor("_NightHazeColor", color3);
				AtmosphereShader.SetVector("_BetaRayleigh", betaRayleigh);
				AtmosphereShader.SetVector("_BetaRayleighTheta", betaRayleighTheta);
				AtmosphereShader.SetVector("_BetaMie", betaMie);
				AtmosphereShader.SetVector("_BetaMieTheta", betaMieTheta);
				AtmosphereShader.SetVector("_HenyeyGreenstein", henyeyGreenstein);
			}
			if (CloudShader != null)
			{
				Color color4 = Color.Lerp(a * 0.25f, color, color.a);
				CloudShader.SetColor("_LightColor", color4);
				Vector3 v = Vector3.Lerp(moonDirection, sunDirection, color.a);
				CloudShader.SetVector("_LightDirection", v);
				CloudShader.SetFloat("_OneOverGamma", OneOverGamma);
				CloudShader.SetFloat("_CloudTone", Clouds.Tone);
				CloudShader.SetFloat("_CloudDensity", Clouds.Density);
				CloudShader.SetFloat("_CloudSharpness", Clouds.Sharpness);
				CloudShader.SetFloat("_CloudShading", Clouds.Shading);
				CloudShader.SetFloat("_CloudScale1", Clouds.Scale1);
				CloudShader.SetFloat("_CloudScale2", Clouds.Scale2);
				CloudShader.SetVector("_CloudUV", Animation.CloudUV);
			}
			if (SpaceShader != null)
			{
				SpaceShader.SetColor("_Color", Night.AdditiveColor);
			}
			if (SunShader != null)
			{
				SunShader.SetColor("_Color", Sun.LightColor);
			}
			if (SunLight.Light != null)
			{
				SunLight.Light.shadowStrength = Sun.ShadowStrength;
			}
			if (MoonShader != null)
			{
				MoonShader.SetFloat("_Phase", Moon.Phase);
			}
			if (MoonLight.Light != null)
			{
				MoonLight.Light.shadowStrength = Moon.ShadowStrength;
			}
			if (MoonHalo != null)
			{
				MoonHalo.SetFloat("_Phase", Moon.Phase);
				MoonHalo.SetColor("_Color", MoonLight.color * Moon.Halo);
			}
		}

		private void SetupHenyeyGreensteinPhaseFunction()
		{
			Single directionality = Day.Directionality;
			henyeyGreenstein.x = 1f - directionality * directionality;
			henyeyGreenstein.y = 1f + directionality * directionality;
			henyeyGreenstein.z = 2f * directionality;
		}

		private void SetupRayleighScattering()
		{
			Single rayleighMultiplier = Day.RayleighMultiplier;
			betaRayleigh.x = rayleighMultiplier * 8f * 31.006279f * 0.0006002188f * 0.0006002188f / 13.628952f;
			betaRayleigh.y = rayleighMultiplier * 8f * 31.006279f * 0.0006002188f * 0.0006002188f / 6.98650074f;
			betaRayleigh.z = rayleighMultiplier * 8f * 31.006279f * 0.0006002188f * 0.0006002188f / 3.130827f;
			betaRayleighTheta.x = rayleighMultiplier * 9.869605f * 0.0006002188f * 0.0006002188f / 9.085968f;
			betaRayleighTheta.y = rayleighMultiplier * 9.869605f * 0.0006002188f * 0.0006002188f / 4.657667f;
			betaRayleighTheta.z = rayleighMultiplier * 9.869605f * 0.0006002188f * 0.0006002188f / 2.087218f;
		}

		private void SetupMieScattering()
		{
			Single mieMultiplier = Day.MieMultiplier;
			betaMie.x = mieMultiplier * 0.434f * 3.39996823E-19f * 4f * 9.869605f / 4.225E-13f * 0.687112f * 3.14159274f;
			betaMie.y = mieMultiplier * 0.434f * 3.39996823E-19f * 4f * 9.869605f / 3.02499968E-13f * 0.679802f * 3.14159274f;
			betaMie.z = mieMultiplier * 0.434f * 3.39996823E-19f * 4f * 9.869605f / 2.02499991E-13f * 0.665996f * 3.14159274f;
			betaMieTheta.x = mieMultiplier * 0.434f * 3.39996823E-19f * 4f * 9.869605f / 4.225E-13f * 0.5f;
			betaMieTheta.y = mieMultiplier * 0.434f * 3.39996823E-19f * 4f * 9.869605f / 3.02499968E-13f * 0.5f;
			betaMieTheta.z = mieMultiplier * 0.434f * 3.39996823E-19f * 4f * 9.869605f / 2.02499991E-13f * 0.5f;
		}

		private void SetupSunAndMoon()
		{
			Single f = 0.0174532924f * Cycle.Latitude;
			Single num = Mathf.Sin(f);
			Single num2 = Mathf.Cos(f);
			Single longitude = Cycle.Longitude;
			Single julianDate = Cycle.JulianDate;
			Single num3 = Cycle.TimeOfDay - Cycle.UTC;
			Single num4 = 23.4393f - 3.563E-07f * julianDate;
			Single f2 = 0.0174532924f * num4;
			Single num5 = Mathf.Sin(f2);
			Single num6 = Mathf.Cos(f2);
			Single num7 = 282.9404f + 4.70935E-05f * julianDate;
			Single num8 = 0.016709f - 1.151E-09f * julianDate;
			Single num9 = 356.047f + 0.985600233f * julianDate;
			Single f3 = 0.0174532924f * num9;
			Single num10 = Mathf.Sin(f3);
			Single num11 = Mathf.Cos(f3);
			Single num12 = num9 + num8 * 57.29578f * num10 * (1f + num8 * num11);
			Single f4 = 0.0174532924f * num12;
			Single num13 = Mathf.Sin(f4);
			Single num14 = Mathf.Cos(f4);
			Single num15 = num14 - num8;
			Single num16 = num13 * Mathf.Sqrt(1f - num8 * num8);
			Single num17 = 57.29578f * Mathf.Atan2(num16, num15);
			Single num18 = Mathf.Sqrt(num15 * num15 + num16 * num16);
			Single num19 = num17 + num7;
			Single f5 = 0.0174532924f * num19;
			Single num20 = Mathf.Sin(f5);
			Single num21 = Mathf.Cos(f5);
			Single num22 = num18 * num21;
			Single num23 = num18 * num20;
			Single num24 = num22;
			Single num25 = num23 * num6;
			Single y = num23 * num5;
			Single num26 = Mathf.Atan2(num25, num24);
			Single num27 = 57.29578f * num26;
			Single f6 = Mathf.Atan2(y, Mathf.Sqrt(num24 * num24 + num25 * num25));
			Single num28 = Mathf.Sin(f6);
			Single num29 = Mathf.Cos(f6);
			Single num30 = num17 + num7 + 180f;
			Single num31 = num30 + num3 * 15f;
			Single num32 = num31 + longitude;
			Single num33 = num32 - num27;
			Single f7 = 0.0174532924f * num33;
			Single num34 = Mathf.Sin(f7);
			Single num35 = Mathf.Cos(f7);
			Single num36 = num35 * num29;
			Single num37 = num34 * num29;
			Single num38 = num28;
			Single num39 = num36 * num - num38 * num2;
			Single num40 = num37;
			Single y2 = num36 * num2 + num38 * num;
			Single num41 = Mathf.Atan2(num40, num39) + 3.14159274f;
			Single num42 = Mathf.Atan2(y2, Mathf.Sqrt(num39 * num39 + num40 * num40));
			Single num43 = 1.57079637f - num42;
			Single phi = num41;
			SunZenith = 57.29578f * num43;
			MoonZenith = Mathf.PingPong(SunZenith + 180f, 180f);
			SunTransform.position = DomeTransform.position + DomeTransform.rotation * OrbitalToUnity(Radius, num43, phi);
			SunTransform.LookAt(DomeTransform.position);
			MoonTransform.position = DomeTransform.position + DomeTransform.rotation * OrbitalToUnity(Radius, num43 + 3.14159274f, phi);
			MoonTransform.LookAt(DomeTransform.position);
			SunRenderer.enabled = (SunTransform.localPosition.y > -0.1f);
			if (MoonRenderer != null)
			{
				MoonRenderer.enabled = (MoonTransform.localPosition.y > -0.1f);
			}
			sunDirection = DomeTransform.InverseTransformDirection(SunTransform.forward);
			moonDirection = DomeTransform.InverseTransformDirection(MoonTransform.forward);
			SetupLightIntensity(num42);
			SetupLightColor(num43);
		}

		private void SetupLightIntensity(Single altitude)
		{
			Single num = Mathf.Abs(altitude);
			Single num2 = 0.17453292f;
			if (altitude > 0f)
			{
				MoonLight.enabled = false;
				MoonLight.intensity = 0f;
				Single lightIntensity = Sun.LightIntensity;
				if (num < num2)
				{
					SunLight.intensity = Mathf.Lerp(0f, lightIntensity, num / num2);
				}
				else
				{
					SunLight.intensity = lightIntensity;
				}
				SunLight.enabled = (SunLight.intensity > 0.05f);
			}
			else
			{
				SunLight.enabled = false;
				SunLight.intensity = 0f;
				Single num3 = Moon.LightIntensity * Mathf.Clamp01(1f - Mathf.Abs(Moon.Phase));
				if (num < num2)
				{
					MoonLight.intensity = Mathf.Lerp(0f, num3, num / num2);
				}
				else
				{
					MoonLight.intensity = num3;
				}
				MoonLight.enabled = (MoonLight.intensity > 0.05f);
			}
		}

		private void SetupLightColor(Single theta)
		{
			Single num = Mathf.Cos(theta * Mathf.Pow(Mathf.Abs(theta / 3.14159274f), 0.5f / Sun.Falloff));
			Single num2 = Mathf.Sqrt(501264f * num * num + 1416f + 1f) - 708f * num;
			Single num3 = Sun.LightColor.r;
			Single num4 = Sun.LightColor.g;
			Single num5 = Sun.LightColor.b;
			Single a = Sun.LightColor.a;
			num3 *= Mathf.Exp(-0.008735f * Mathf.Pow(0.65f, -4.08f * num2));
			num4 *= Mathf.Exp(-0.008735f * Mathf.Pow(0.549999952f, -4.08f * num2));
			num5 *= Mathf.Exp(-0.008735f * Mathf.Pow(0.45f, -4.08f * num2));
			num3 = ((num3 <= 0f) ? 0f : num3);
			num4 = ((num4 <= 0f) ? 0f : num4);
			num5 = ((num5 <= 0f) ? 0f : num5);
			SunLight.color = Color.Lerp(new Color(a, a, a, a), new Color(num3, num4, num5, a), Sun.Coloring);
			MoonLight.color = MoonLight.intensity * Moon.LightColor;
		}

		private Vector3 OrbitalToUnity(Single radius, Single theta, Single phi)
		{
			Single num = Mathf.Sin(theta);
			Single num2 = Mathf.Cos(theta);
			Single num3 = Mathf.Sin(phi);
			Single num4 = Mathf.Cos(phi);
			Vector3 result;
			result.z = radius * num * num4;
			result.y = radius * num2;
			result.x = radius * num * num3;
			return result;
		}

		private Color ColorPow(Color c, Single p)
		{
			return new Color(Mathf.Pow(c.r, p), Mathf.Pow(c.g, p), Mathf.Pow(c.b, p), Mathf.Pow(c.a, p));
		}

		private Color FogColor()
		{
			Color color = SunLight.color;
			Color color2 = Day.AdditiveColor.a * Day.AdditiveColor;
			Color color3 = Night.HazeColor.a * Night.HazeColor;
			Single num = Mathf.Pow(0.001f, Day.Haziness);
			Single num2 = Mathf.Pow(0.001f, Night.Haziness);
			Single num3 = 190000f - num * (360000f - num * 190000f);
			Single num4 = 190000f - num * (372000f - num * 190000f);
			Single t = Mathf.Exp(-(num3 * betaRayleigh.x + num4 * betaMie.x));
			Single t2 = Mathf.Exp(-(num3 * betaRayleigh.y + num4 * betaMie.y));
			Single t3 = Mathf.Exp(-(num3 * betaRayleigh.z + num4 * betaMie.z));
			Vector3 vector = betaRayleighTheta * 2f + betaMieTheta * henyeyGreenstein.x / Mathf.Pow(henyeyGreenstein.y, 1.5f);
			Vector3 vector2 = betaRayleigh + betaMie;
			Single r = color2.r;
			Single g = color2.g;
			Single b = color2.b;
			Single from = color.r * Day.Brightness * vector.x / vector2.x;
			Single from2 = color.g * Day.Brightness * vector.y / vector2.y;
			Single from3 = color.b * Day.Brightness * vector.z / vector2.z;
			Single r2 = Mathf.Lerp(from, r, t);
			Single g2 = Mathf.Lerp(from2, g, t2);
			Single b2 = Mathf.Lerp(from3, b, t3);
			Color b3 = new Color(r2, g2, b2, 1f);
			Single r3 = color3.r;
			Single g3 = color3.g;
			Single b4 = color3.b;
			Color a = new Color(r3, g3, b4, 1f);
			Color a2 = Color.Lerp(a, b3, color.a);
			a2.a = 1f - (1f - color.a) * num2;
			return Mathf.Sqrt(a2.a) * a2;
		}

		private Color AmbientColor()
		{
			Single r = SunLight.color.r;
			Single g = SunLight.color.g;
			Single b = SunLight.color.b;
			Color b2 = new Color(r, g, b, 1f);
			Single r2 = MoonLight.color.r;
			Single g2 = MoonLight.color.g;
			Single b3 = MoonLight.color.b;
			Color a = new Color(r2, g2, b3, 1f);
			Color a2 = Color.Lerp(a, b2, SunLight.color.a);
			return World.AmbientIntensity * a2;
		}

		private Vector4 CloudShadowUV()
		{
			Vector3 normalized = SunTransform.localPosition.normalized;
			Vector2 vector = new Vector2(normalized.x, normalized.z) / Mathf.Max(0.05f, normalized.y) / 2f;
			Vector4 result;
			result.x = 0.5f + vector.x + Animation.CloudUV.x / Clouds.Scale1;
			result.y = 0.5f + vector.y + Animation.CloudUV.y / Clouds.Scale1;
			result.z = 0.5f + vector.x + Animation.CloudUV.x / Clouds.Scale2;
			result.w = 0.5f + vector.y + Animation.CloudUV.y / Clouds.Scale2;
			return result;
		}

		private void ValidateScriptParameters()
		{
			Cycle.TimeOfDay = Mathf.Repeat(Cycle.TimeOfDay, 24f);
			Cycle.JulianDate = Mathf.Repeat(Cycle.JulianDate - 1f, 365f) + 1f;
			Cycle.Longitude = Mathf.Clamp(Cycle.Longitude, -180f, 180f);
			Cycle.Latitude = Mathf.Clamp(Cycle.Latitude, -90f, 90f);
			Day.MieMultiplier = Mathf.Max(0f, Day.MieMultiplier);
			Day.RayleighMultiplier = Mathf.Max(0f, Day.RayleighMultiplier);
			Day.Brightness = Mathf.Max(0f, Day.Brightness);
			Day.Haziness = Mathf.Max(0f, Day.Haziness);
			Day.Directionality = Mathf.Clamp01(Day.Directionality);
			Night.Haziness = Mathf.Max(0f, Night.Haziness);
			Sun.LightIntensity = Mathf.Clamp01(Sun.LightIntensity);
			Sun.ShadowStrength = Mathf.Clamp01(Sun.ShadowStrength);
			Sun.Falloff = Mathf.Clamp01(Sun.Falloff);
			Sun.Coloring = Mathf.Clamp01(Sun.Coloring);
			Moon.LightIntensity = Mathf.Clamp01(Moon.LightIntensity);
			Moon.ShadowStrength = Mathf.Clamp01(Moon.ShadowStrength);
			Moon.Halo = Mathf.Clamp01(Moon.Halo);
			Moon.Phase = Mathf.Clamp(Moon.Phase, -1f, 1f);
			Clouds.Tone = Mathf.Max(0f, Clouds.Tone);
			Clouds.Shading = Mathf.Max(0f, Clouds.Shading);
			Clouds.Density = Mathf.Max(0f, Clouds.Density);
			Clouds.Sharpness = Mathf.Max(0f, Clouds.Sharpness);
			Clouds.ShadowStrength = Mathf.Clamp01(Clouds.ShadowStrength);
			World.AmbientIntensity = Mathf.Clamp01(World.AmbientIntensity);
		}

		internal struct WrapperLight
		{
			private Boolean m_enabled;

			private Color m_color;

			private Single m_intensity;

			public Light Light;

			public WrapperLight(Light light)
			{
				m_enabled = false;
				m_color = Color.clear;
				m_intensity = 0f;
				Light = light;
			}

			public Boolean enabled
			{
				get => (!(Light != null)) ? m_enabled : Light.enabled;
			    set
				{
					if (Light != null)
					{
						Light.enabled = value;
					}
					m_enabled = value;
				}
			}

			public Color color
			{
				get => (!(Light != null)) ? m_color : Light.color;
			    set
				{
					if (Light != null)
					{
						Light.color = value;
					}
					m_color = value;
				}
			}

			public Single intensity
			{
				get => (!(Light != null)) ? m_intensity : Light.intensity;
			    set
				{
					if (Light != null)
					{
						Light.intensity = value;
					}
					m_intensity = value;
				}
			}
		}
	}
}
