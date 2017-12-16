using System;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities.AI.Behaviours;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;

namespace Legacy.Core.Entities.AI
{
	public struct AIData
	{
		public Int32 SummonCount;

		public DamageData[] DamageData;

		public Int32 Range;

		public EDamageMod DamageMod;

		public ETargetType TargetType;

		public ECondition[] RemovedConditions;

		public EPartyBuffs[] PartyBuffs;

		public EMonsterBuffType[] MonsterBuffs;

		public String Prefab;

		public EAIBehaviour AIBehaviour;

		public EAITarget AITargets;

		public Boolean DestroyOnTargetContact;

		public Int32 AILifetime;

		public Int32 AIRange;
	}
}
