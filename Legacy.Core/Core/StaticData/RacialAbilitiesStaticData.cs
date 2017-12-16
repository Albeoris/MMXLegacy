using System;
using Dumper.Core;
using Legacy.Core.Internationalization;

namespace Legacy.Core.StaticData
{
	public class RacialAbilitiesStaticData : BaseAbilityStaticData
	{
		[CsvColumn("Race")]
		protected ERace m_race;

		[CsvColumn("Value")]
		protected Int32 m_value;

		public ERace Race => m_race;

	    public Int32 Value => m_value;

	    public String GetDescription()
		{
			return Localization.Instance.GetText(m_nameKey + "_INFO", m_value);
		}
	}
}
