using System;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.ActionLogging
{
	public class EnchantmentEffectEventArgs : LogEntryEventArgs
	{
		public EnchantmentEffectEventArgs(SuffixStaticData p_suffixData) : this(p_suffixData, 0f)
		{
		}

		public EnchantmentEffectEventArgs(SuffixStaticData p_suffixData, Single p_effectValue)
		{
			EffectData = p_suffixData;
			EffectValue = p_effectValue;
		}

		public SuffixStaticData EffectData { get; private set; }

		public Single EffectValue { get; private set; }
	}
}
