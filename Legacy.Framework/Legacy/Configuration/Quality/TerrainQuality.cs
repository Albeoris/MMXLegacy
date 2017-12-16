using System;
using UnityEngine;

namespace Legacy.Configuration.Quality
{
	[RequireComponent(typeof(Terrain))]
	[AddComponentMenu("MM Legacy/Quality/TerrainQuality")]
	public class TerrainQuality : QualityConfigurationBase
	{
		public QualityFloatValue TerrainPixelError = new QualityFloatValue();

		public QualityFloatValue TerrainBaseMapDistance = new QualityFloatValue();

		public QualityFloatValue DetailDistance = new QualityFloatValue();

		public QualityFloatValue DetailDensity = new QualityFloatValue();

		public QualityFloatValue TreeDistance = new QualityFloatValue();

		public QualityFloatValue TreeBillboardStartDistance = new QualityFloatValue();

		public QualityFloatValue MaxTreeMeshes = new QualityFloatValue();

		public override void OnQualityConfigutationChanged()
		{
			TerrainPixelError.SetQualityValue((Int32)GraphicsConfigManager.Settings.TerrainQuality);
			TerrainBaseMapDistance.SetQualityValue((Int32)GraphicsConfigManager.Settings.TerrainQuality);
			DetailDistance.SetQualityValue((Int32)GraphicsConfigManager.Settings.TerrainDetails);
			DetailDensity.SetQualityValue((Int32)GraphicsConfigManager.Settings.TerrainDetails);
			TreeDistance.SetQualityValue((Int32)GraphicsConfigManager.Settings.TerrainDetails);
			TreeBillboardStartDistance.SetQualityValue((Int32)GraphicsConfigManager.Settings.TerrainDetails);
			MaxTreeMeshes.SetQualityValue((Int32)GraphicsConfigManager.Settings.TerrainDetails);
			Terrain component = GetComponent<Terrain>();
			component.heightmapPixelError = TerrainPixelError.GetQualityValue();
			component.basemapDistance = TerrainBaseMapDistance.GetQualityValue();
			component.detailObjectDistance = DetailDistance.GetQualityValue();
			component.detailObjectDensity = DetailDensity.GetQualityValue();
			component.treeDistance = TreeDistance.GetQualityValue();
			component.treeBillboardDistance = TreeBillboardStartDistance.GetQualityValue();
			component.treeMaximumFullLODCount = (Int32)MaxTreeMeshes.GetQualityValue();
			component.castShadows = GraphicsConfigManager.Settings.TerrainShadows;
		}
	}
}
