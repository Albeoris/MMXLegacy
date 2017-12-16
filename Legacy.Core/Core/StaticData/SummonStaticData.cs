using System;
using Dumper.Core;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities.AI;
using Legacy.Core.Entities.AI.Behaviours;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;

namespace Legacy.Core.StaticData
{
	public class SummonStaticData : BaseStaticData
	{
		[CsvColumn("NameKey")]
		public String NameKey;

		[CsvColumn("SummonCount")]
		public Int32 SummonCount;

		[CsvColumn("Damage")]
		public DamageData[] DamageData;

		[CsvColumn("Range")]
		public Int32 Range;

		[CsvColumn("DamageMod")]
		public EDamageMod DamageMod;

		[CsvColumn("TargetType")]
		public ETargetType TargetType;

		[CsvColumn("RemovedConditions")]
		public ECondition[] RemovedConditions;

		[CsvColumn("PartyBuffs")]
		public EPartyBuffs[] PartyBuffs;

		[CsvColumn("MonsterBuffs")]
		public EMonsterBuffType[] MonsterBuffs;

		[CsvColumn("Prefab")]
		public String Prefab;

		[CsvColumn("AIBehaviour")]
		public EAIBehaviour AIBehaviour;

		[CsvColumn("AITargets")]
		public EAITarget AITargets;

		[CsvColumn("AILifetime")]
		public Int32 AILifetime;

		[CsvColumn("DestroyOnTargetContact")]
		public Boolean DestroyOnTargetContact;

		[CsvColumn("AIAggroRange")]
		public Int32 AIRange;

		[CsvColumn("ExecutionOrder")]
		public ESummonExecutionOrder ExecutionOrder;

		public AIData GetAIData()
		{
			return new AIData
			{
				SummonCount = SummonCount,
				DamageData = DamageData,
				Range = Range,
				DamageMod = DamageMod,
				TargetType = TargetType,
				RemovedConditions = RemovedConditions,
				PartyBuffs = PartyBuffs,
				MonsterBuffs = MonsterBuffs,
				Prefab = Prefab,
				AIBehaviour = AIBehaviour,
				AITargets = AITargets,
				DestroyOnTargetContact = DestroyOnTargetContact,
				AILifetime = AILifetime,
				AIRange = AIRange
			};
		}
	}
}
