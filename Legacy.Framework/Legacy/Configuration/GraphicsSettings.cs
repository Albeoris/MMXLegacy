using System;
using Legacy.Core.Utilities.Configuration;
using UnityEngine;

namespace Legacy.Configuration
{
	public class GraphicsSettings
	{
		public const Single MAX_GAMMA = 3f;

		public const Single MIN_GAMMA = 0.5f;

		public const Single MAX_BRIGHTNESS = 1.5f;

		public const Single MIN_BRIGHTNESS = 0.5f;

		public const Int32 MAX_FILTER_LEVEL = 16;

		public const Int32 MAX_FRAMERATE = 500;

		public const Int32 MAX_SHADOW_DISTANCE = 150;

		private Single m_Gamma;

		private Single m_Brightness;

		private Int32 m_Filtering;

		private Int32 m_MaxFramerate;

		private Int32 m_ShadowDistance;

		private ETextureQuality m_TextureQuality;

		public Resolution Resolution { get; set; }

		public EFSAAMode Antialiasing { get; set; }

		public Boolean FullScreen { get; set; }

		public Boolean VerticalSync { get; set; }

		public Boolean HighDynamicRange { get; set; }

		public ETextureQuality TextureQuality
		{
			get => m_TextureQuality;
		    set
			{
				if (value > ETextureQuality.MEDIUM && !Helper.Is64BitOperatingSystem())
				{
					value = ETextureQuality.MEDIUM;
				}
				m_TextureQuality = value;
			}
		}

		public EShadowQuality ShadowQuality { get; set; }

		public ETerrainQuality TerrainQuality { get; set; }

		public ETerrainDetails TerrainDetails { get; set; }

		public EEffectQuality EffectQuality { get; set; }

		public EViewDistance ViewDistance { get; set; }

		public EAnimationQuality AnimationQuality { get; set; }

		public Single Gamma
		{
			get => m_Gamma;
		    set => m_Gamma = Mathf.Clamp(value, 0.5f, 3f);
		}

		public Single Brightness
		{
			get => m_Brightness;
		    set => m_Brightness = Mathf.Clamp(value, 0.5f, 1.5f);
		}

		public Int32 Filtering
		{
			get => m_Filtering;
		    set => m_Filtering = Mathf.Clamp(value, 0, 16);
		}

		public Int32 MaxFramerate
		{
			get => m_MaxFramerate;
		    set => m_MaxFramerate = Mathf.Clamp(value, 0, 500);
		}

		public Int32 ShadowDistance
		{
			get => m_ShadowDistance;
		    set => m_ShadowDistance = Mathf.Clamp(value, 0, 150);
		}

		public EObjectDetail ObjectDetails { get; set; }

		public Boolean TerrainShadows { get; set; }

		public void Load(ConfigReader p_reader)
		{
			Resolution = new Resolution
			{
				width = p_reader["graphics"]["resolutionWidth"].GetInt(),
				height = p_reader["graphics"]["resolutionHeight"].GetInt(),
				refreshRate = p_reader["graphics"]["resolutionRefreshRate"].GetInt()
			};
			FullScreen = p_reader["graphics"]["fullScreen"].GetBool(true);
			VerticalSync = p_reader["graphics"]["vsync"].GetBool(true);
			HighDynamicRange = p_reader["graphics"]["hdr"].GetBool(true);
			Antialiasing = (EFSAAMode)p_reader["graphics"]["fsaaMode"].GetInt();
			TextureQuality = (ETextureQuality)p_reader["graphics"]["textureQuality"].GetInt();
			ShadowQuality = (EShadowQuality)p_reader["graphics"]["shadowQuality"].GetInt();
			TerrainQuality = (ETerrainQuality)p_reader["graphics"]["terrainQuality"].GetInt();
			TerrainDetails = (ETerrainDetails)p_reader["graphics"]["terrainDetails"].GetInt();
			TerrainShadows = p_reader["graphics"]["terrainShadows"].GetBool(true);
			EffectQuality = (EEffectQuality)p_reader["graphics"]["effectQuality"].GetInt();
			ViewDistance = (EViewDistance)p_reader["graphics"]["viewDistance"].GetInt();
			AnimationQuality = (EAnimationQuality)p_reader["graphics"]["animationQuality"].GetInt();
			Gamma = p_reader["graphics"]["gamma"].GetFloat(1f);
			Brightness = p_reader["graphics"]["brightness"].GetFloat(1f);
			Filtering = p_reader["graphics"]["filter"].GetInt(0);
			MaxFramerate = p_reader["graphics"]["maxFramerate"].GetInt(60);
			ShadowDistance = p_reader["graphics"]["shadowDistance"].GetInt(100);
			ObjectDetails = (EObjectDetail)p_reader["graphics"]["objectDetails"].GetInt();
		}

		public void Save(ConfigReader p_reader)
		{
			p_reader["graphics"]["resolutionWidth"].SetValue(Resolution.width);
			p_reader["graphics"]["resolutionHeight"].SetValue(Resolution.height);
			p_reader["graphics"]["resolutionRefreshRate"].SetValue(Resolution.refreshRate);
			p_reader["graphics"]["fullScreen"].SetValue(FullScreen);
			p_reader["graphics"]["vsync"].SetValue(VerticalSync);
			p_reader["graphics"]["hdr"].SetValue(HighDynamicRange);
			p_reader["graphics"]["fsaaMode"].SetValue((Int32)Antialiasing);
			p_reader["graphics"]["textureQuality"].SetValue((Int32)TextureQuality);
			p_reader["graphics"]["shadowQuality"].SetValue((Int32)ShadowQuality);
			p_reader["graphics"]["shadowDistance"].SetValue(ShadowDistance);
			p_reader["graphics"]["terrainQuality"].SetValue((Int32)TerrainQuality);
			p_reader["graphics"]["terrainDetails"].SetValue((Int32)TerrainDetails);
			p_reader["graphics"]["terrainShadows"].SetValue(TerrainShadows);
			p_reader["graphics"]["effectQuality"].SetValue((Int32)EffectQuality);
			p_reader["graphics"]["viewDistance"].SetValue((Int32)ViewDistance);
			p_reader["graphics"]["animationQuality"].SetValue((Int32)AnimationQuality);
			p_reader["graphics"]["gamma"].SetValue(Gamma);
			p_reader["graphics"]["brightness"].SetValue(Brightness);
			p_reader["graphics"]["filter"].SetValue(Filtering);
			p_reader["graphics"]["maxFramerate"].SetValue(MaxFramerate);
			p_reader["graphics"]["objectDetails"].SetValue((Int32)ObjectDetails);
		}

		public Boolean HasUnsavedChanges(ConfigReader p_reader)
		{
			Boolean flag = false;
			flag |= (Resolution.width != p_reader["graphics"]["resolutionWidth"].GetInt());
			flag |= (Resolution.height != p_reader["graphics"]["resolutionHeight"].GetInt());
			flag |= (FullScreen != p_reader["graphics"]["fullScreen"].GetBool());
			flag |= (VerticalSync != p_reader["graphics"]["vsync"].GetBool());
			flag |= (HighDynamicRange != p_reader["graphics"]["hdr"].GetBool());
			flag |= (Antialiasing != (EFSAAMode)p_reader["graphics"]["fsaaMode"].GetInt());
			flag |= (TextureQuality != (ETextureQuality)p_reader["graphics"]["textureQuality"].GetInt());
			flag |= (ShadowQuality != (EShadowQuality)p_reader["graphics"]["shadowQuality"].GetInt());
			flag |= (TerrainQuality != (ETerrainQuality)p_reader["graphics"]["terrainQuality"].GetInt());
			flag |= (TerrainDetails != (ETerrainDetails)p_reader["graphics"]["terrainDetails"].GetInt());
			flag |= (TerrainShadows != p_reader["graphics"]["terrainShadows"].GetBool());
			flag |= (EffectQuality != (EEffectQuality)p_reader["graphics"]["effectQuality"].GetInt());
			flag |= (ViewDistance != (EViewDistance)p_reader["graphics"]["viewDistance"].GetInt());
			flag |= (AnimationQuality != (EAnimationQuality)p_reader["graphics"]["animationQuality"].GetInt());
			flag |= (Gamma != p_reader["graphics"]["gamma"].GetFloat());
			flag |= (Brightness != p_reader["graphics"]["brightness"].GetFloat());
			flag |= (Filtering != p_reader["graphics"]["filter"].GetInt());
			flag |= (MaxFramerate != p_reader["graphics"]["maxFramerate"].GetInt());
			flag |= (ShadowDistance != p_reader["graphics"]["shadowDistance"].GetInt());
			return flag | ObjectDetails != (EObjectDetail)p_reader["graphics"]["objectDetails"].GetInt();
		}
	}
}
