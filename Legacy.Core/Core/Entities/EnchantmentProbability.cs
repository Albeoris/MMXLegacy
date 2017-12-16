using System;

namespace Legacy.Core.Entities
{
	public struct EnchantmentProbability
	{
		public readonly Int32 ModelLevel;

		public readonly Single Weight;

		public EnchantmentProbability(Int32 p_modelLevel, Single p_weight)
		{
			ModelLevel = p_modelLevel;
			Weight = p_weight;
		}
	}
}
