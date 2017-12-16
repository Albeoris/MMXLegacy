using System;
using Dumper.Core;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.StaticData
{
	public class SkillEffectStaticData : BaseStaticData
	{
		[CsvColumn("GeneralDescription")]
		protected String m_generalDescription;

		[CsvColumn("GeneralDescriptionTarget")]
		protected String m_generalDescriptionTarget;

		[CsvColumn("DynamicDescription")]
		protected String m_dynamicDescription;

		[CsvColumn("ContainsPercent")]
		protected Boolean m_containsPercent;

		[CsvColumn("ShowInTooltip")]
		protected Boolean m_showInTooltip;

		[CsvColumn("Condition")]
		protected ESkillEffectCondition m_condition;

		[CsvColumn("Type")]
		protected ESkillEffectType m_type;

		[CsvColumn("Value")]
		protected Single m_value;

		[CsvColumn("Mode")]
		protected ESkillEffectMode m_mode;

		public String GeneralDescription => m_generalDescription;

	    public String GeneralDescriptionTarget => m_generalDescriptionTarget;

	    public String DynamicDescription => m_dynamicDescription;

	    public Boolean ContainsPercent => m_containsPercent;

	    public Boolean ShowInTooltip => m_showInTooltip;

	    public ESkillEffectCondition Condition => m_condition;

	    public ESkillEffectType Type => m_type;

	    public Single Value => m_value;

	    public ESkillEffectMode Mode => m_mode;
	}
}
