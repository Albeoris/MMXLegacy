using System;
using UnityEngine;

namespace Legacy.Configuration.Quality
{
	[AddComponentMenu("MM Legacy/Quality/CameraHDR Quality")]
	[RequireComponent(typeof(Camera))]
	internal class CameraHDRQuality : QualityConfigurationBase
	{
		public override void OnQualityConfigutationChanged()
		{
			camera.hdr = GraphicsConfigManager.Settings.HighDynamicRange;
		}
	}
}
