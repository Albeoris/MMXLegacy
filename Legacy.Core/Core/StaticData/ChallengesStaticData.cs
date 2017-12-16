using System;
using Dumper.Core;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Items;
using Legacy.Core.UpdateLogic.Preconditions;

namespace Legacy.Core.StaticData
{
	public class ChallengesStaticData : BaseStaticData
	{
		[CsvColumn("Type")]
		private EPreconditionType m_type;

		[CsvColumn("Attribute")]
		private EPotionTarget m_attribute;

		[CsvColumn("Value")]
		private Int32 m_value;

		[CsvColumn("DamageType")]
		private EDamageType m_damageType;

		[CsvColumn("Damage")]
		private Int32 m_damage;

		[CsvColumn("XPReward")]
		private Int32 m_xpReward;

		[CsvColumn("MenuPath")]
		private String m_menuPath;

		[CsvColumn("WhoWillText")]
		private String m_whoWillText;

		[CsvColumn("SuccessText")]
		private String m_successText;

		[CsvColumn("FailText")]
		private String m_failText;

		[CsvColumn("SingleTarget")]
		private Boolean m_singleTarget;

		public EPreconditionType Type => m_type;

	    public EPotionTarget Attribute => m_attribute;

	    public Int32 Value => m_value;

	    public EDamageType DamageType => m_damageType;

	    public Int32 Damage => m_damage;

	    public Int32 XPReward => m_xpReward;

	    public String MenuPath => m_menuPath;

	    public String WhoWillText => m_whoWillText;

	    public String SuccessText => m_successText;

	    public String FailText => m_failText;

	    public Boolean SingleTarget => m_singleTarget;
	}
}
