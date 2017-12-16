using System;
using Legacy.Utilities;
using UnityEngine;

namespace Legacy.Configuration.Quality
{
	[RequireComponent(typeof(CameraPerLayerDistanceCulling))]
	[AddComponentMenu("MM Legacy/Quality/CameraLayerCullingQuality")]
	internal class CameraLayerCullingQuality : QualityConfigurationBase
	{
		public QualityFloatValue CullingDistanceDefault = new QualityFloatValue();

		public QualityFloatValue CullingDistanceVeryClose = new QualityFloatValue();

		public QualityFloatValue CullingDistanceClose = new QualityFloatValue();

		public QualityFloatValue CullingDistanceMid = new QualityFloatValue();

		public QualityFloatValue CullingDistanceFar = new QualityFloatValue();

		public override void OnQualityConfigutationChanged()
		{
			Int32 viewDistance = (Int32)GraphicsConfigManager.Settings.ViewDistance;
			CullingDistanceDefault.SetQualityValue(viewDistance);
			CullingDistanceVeryClose.SetQualityValue(viewDistance);
			CullingDistanceClose.SetQualityValue(viewDistance);
			CullingDistanceMid.SetQualityValue(viewDistance);
			CullingDistanceFar.SetQualityValue(viewDistance);
			CameraPerLayerDistanceCulling component = GetComponent<CameraPerLayerDistanceCulling>();
			component.SetLayerDistance(0, CullingDistanceDefault.GetQualityValue());
			component.SetLayerDistance(26, CullingDistanceVeryClose.GetQualityValue());
			component.SetLayerDistance(27, CullingDistanceClose.GetQualityValue());
			component.SetLayerDistance(28, CullingDistanceMid.GetQualityValue());
			component.SetLayerDistance(29, CullingDistanceFar.GetQualityValue());
		}
	}
}
