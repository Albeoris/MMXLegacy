using System;
using Dumper.Core;
using Legacy.Core.Entities.Skills;
using Legacy.Core.Internationalization;

namespace Legacy.Core.StaticData
{
    public class RacialSkillsStaticData : BaseAbilityStaticData
    {
        [CsvColumn("Race")]
        protected ERace m_race;

        [CsvColumn("Value")]
        protected Int32 m_value;

        [CsvColumn("ETier")]
        protected ETier m_tier;

        public ERace Race => m_race;

        public Int32 Value => m_value;

        public ETier Tier => m_tier;

        public String GetDescription()
        {
            return Localization.Instance.GetText(m_nameKey + "_INFO", m_value);
        }
    }
}