using System;
using Dumper.Core;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.StaticData
{
	public class RechargingObjectStaticData : BaseStaticData
	{
		[CsvColumn("Prefab")]
		public String Prefab;

		[CsvColumn("MenuPath")]
		public String MenuPath;

		[CsvColumn("Buff")]
		public EPartyBuffs PartyBuff;

		[CsvColumn("Special")]
		public ERechargerSpecial Special;

		[CsvColumn("RechargerType")]
		public ERechargerType RechargerType;
	}
}
