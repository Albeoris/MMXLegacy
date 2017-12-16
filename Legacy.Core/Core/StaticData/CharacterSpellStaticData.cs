using System;
using Dumper.Core;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;

namespace Legacy.Core.StaticData
{
	public class CharacterSpellStaticData : BaseStaticData
	{
		[CsvColumn("Name")]
		public String NameKey;

		[CsvColumn("Description")]
		public String DescriptionKey;

		[CsvColumn("SkillID")]
		public ESkillID SkillID;

		[CsvColumn("ETier")]
		public ETier Tier;

		[CsvColumn("Icon")]
		public String Icon;

		[CsvColumn("Damage")]
		public DamageData[] Damage;

		[CsvColumn("Range")]
		public Int32 Range;

		[CsvColumn("TargetType")]
		public ETargetType TargetType;

		[CsvColumn("RemovedConditions")]
		public ECondition[] RemovedConditions;

		[CsvColumn("PartyBuff")]
		public EPartyBuffs PartyBuff;

		[CsvColumn("MonsterBuffs")]
		public EMonsterBuffType[] MonsterBuffs;

		[CsvColumn("Gfx")]
		public String EffectKey;

		[CsvColumn("ManaCost")]
		public Int32 ManaCost;

		[CsvColumn("GoldPrice")]
		public Int32 GoldPrice;

		[CsvColumn("SummonID")]
		public Int32 SummonID;

		[CsvColumn("ClassOnly")]
		public EClass ClassOnly;

		[CsvColumn("AdditionalValue")]
		public Single AdditionalValue;
	}
}
