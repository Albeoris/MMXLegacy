using System;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.Entities
{
	public struct ModelProbability
	{
		public readonly Int32 ModelLevel;

		public readonly ESubModel SubModel;

		public readonly Single Weight;

		public ModelProbability(Int32 p_modelLevel, ESubModel p_subModel, Single p_weight)
		{
			ModelLevel = p_modelLevel;
			SubModel = p_subModel;
			Weight = p_weight;
		}
	}
}
