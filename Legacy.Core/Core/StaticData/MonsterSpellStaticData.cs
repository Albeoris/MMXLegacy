using System;
using Dumper.Core;
using Legacy.Core.Combat;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;

namespace Legacy.Core.StaticData
{
	public class MonsterSpellStaticData : BaseStaticData
	{
		[CsvColumn("KeyName")]
		private String m_nameKey;

		[CsvColumn("Damage1")]
		private DamageData[] m_damage1;

		[CsvColumn("Damage2")]
		private DamageData[] m_damage2;

		[CsvColumn("Damage3")]
		private DamageData[] m_damage3;

		[CsvColumn("TargetType")]
		private ETargetType m_targetType;

		[CsvColumn("Gfx")]
		private String m_effectKey;

		[CsvColumn("InflictedConditions")]
		private ECondition[] m_inflictedConditions;

		[CsvColumn("MagicSchool")]
		private EDamageType m_magicSchool;

		[CsvColumn("Icon")]
		private String m_icon;

		[CsvColumn("AdditionalValue1")]
		private Single m_additionalValue1;

		[CsvColumn("AdditionalValue2")]
		private Single m_additionalValue2;

		[CsvColumn("AdditionalValue3")]
		private Single m_additionalValue3;

		public String NameKey => m_nameKey;

	    public ETargetType TargetType => m_targetType;

	    public String EffectKey => m_effectKey;

	    public ECondition[] InflictedConditions => m_inflictedConditions;

	    public EDamageType MagicSchool => m_magicSchool;

	    public String Icon => m_icon;

	    public DamageData[] GetDamage(Int32 p_level)
		{
			if (p_level == 3)
			{
				return m_damage3;
			}
			if (p_level == 2)
			{
				return m_damage2;
			}
			return m_damage1;
		}

		public Single GetAdditionalValue(Int32 p_level)
		{
			if (p_level == 3)
			{
				return m_additionalValue3;
			}
			if (p_level == 2)
			{
				return m_additionalValue2;
			}
			return m_additionalValue1;
		}
	}
}
