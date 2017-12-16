using System;
using Dumper.Core;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.Quests;

namespace Legacy.Core.StaticData
{
	public class QuestStepStaticData : BaseStaticData
	{
		[CsvColumn("Type")]
		protected EQuestType m_type;

		[CsvColumn("Name")]
		protected String m_name;

		[CsvColumn("FlavorDescription")]
		protected String m_flavorDescription;

		[CsvColumn("ShortDescription")]
		protected String m_shortDescription;

		[CsvColumn("GivenByNPCID")]
		protected Int32 m_givenByNPCID;

		[CsvColumn("Objectives")]
		protected String m_objectives;

		[CsvColumn("RepeatTime")]
		protected Int32 m_repeatTime;

		[CsvColumn("RewardXP")]
		protected Int32 m_rewardXP;

		[CsvColumn("FollowUpStep")]
		protected Int32 m_followupStep;

		[CsvColumn("SteadyLoot")]
		protected SteadyLoot[] m_steadyLoot;

		[CsvColumn("ItemModels")]
		protected ModelProbability[] m_dropModelLevels;

		[CsvColumn("ItemDropChance")]
		protected Single m_dropItemChance;

		[CsvColumn("PrefixChance")]
		protected Single m_dropItemPrefixChance;

		[CsvColumn("SuffixChance")]
		protected Single m_dropItemSuffixChance;

		[CsvColumn("PrefixProbabilities")]
		public EnchantmentProbability[] m_prefixProbabilities;

		[CsvColumn("SuffixProbabilities")]
		public EnchantmentProbability[] m_suffixProbabilities;

		[CsvColumn("ItemSpecificationList")]
		protected EEquipmentType[] m_dropItemSpecificationList;

		[CsvColumn("GoldChance")]
		protected Single m_dropGoldChance;

		[CsvColumn("GoldAmount")]
		protected IntRange m_dropGoldAmount;

		[CsvColumn("TokenID")]
		protected Int32[] m_tokenID;

		public EQuestType Type => m_type;

	    public String Name => m_name;

	    public String FlavorDescription => m_flavorDescription;

	    public String ShortDescription => m_shortDescription;

	    public Int32 GivenByNPCID => m_givenByNPCID;

	    public Int32 RepeatTime => m_repeatTime;

	    public String Objectives => m_objectives;

	    public Int32 RewardXP => m_rewardXP;

	    public Int32 FollowupStep => m_followupStep;

	    public SteadyLoot[] SteadyLoot => m_steadyLoot;

	    public ModelProbability[] DropModelLevels => m_dropModelLevels;

	    public Single DropItemChance => m_dropItemChance;

	    public Single DropItemPrefixChance => m_dropItemPrefixChance;

	    public Single DropItemSuffixChance => m_dropItemSuffixChance;

	    public EnchantmentProbability[] PrefixProbabilities => m_prefixProbabilities;

	    public EnchantmentProbability[] SuffixProbabilities => m_suffixProbabilities;

	    public EEquipmentType[] DropItemSpecificationList => m_dropItemSpecificationList;

	    public Single DropGoldChance => m_dropGoldChance;

	    public IntRange DropGoldAmount => m_dropGoldAmount;

	    public Int32[] TokenID => m_tokenID;

	    public void UpdateFollowupStep(Int32 p_newFollowUp)
		{
			m_followupStep = p_newFollowUp;
		}
	}
}
