using System;
using Dumper.Core;
using Legacy.Core.Entities;
using Legacy.Core.Quests;
using Legacy.Core.StaticData;

namespace Legacy.Core
{
	public class QuestObjectiveStaticData : BaseStaticData
	{
		[CsvColumn("Description")]
		protected String m_description;

		[CsvColumn("KillMonsterClass")]
		protected EMonsterClass m_monsterClass;

		[CsvColumn("KillMonsterType")]
		protected EMonsterType m_monsterType;

		[CsvColumn("KillMonsterStaticID")]
		protected Int32 m_monsterStaticID;

		[CsvColumn("TokenID")]
		protected Int32 m_tokenID;

		[CsvColumn("NpcID")]
		protected Int32 m_npcID;

		[CsvColumn("IsReturn")]
		protected Boolean m_isReturn;

		[CsvColumn("IsMainObjective")]
		protected Boolean m_isMainObjective;

		[CsvColumn("DaysToPass")]
		protected Int32 m_daysToPass;

		[CsvColumn("StepsOnTerrain")]
		protected StepsOnTerrainData m_stepsOnTerrain;

		[CsvColumn("Location")]
		protected String m_location;

		public String Description => m_description;

	    public EMonsterClass MonsterClass => m_monsterClass;

	    public EMonsterType MonsterType => m_monsterType;

	    public Int32 MonsterStaticID => m_monsterStaticID;

	    public Int32 TokenID => m_tokenID;

	    public Int32 NpcID => m_npcID;

	    public Boolean IsReturn => m_isReturn;

	    public Boolean IsMainObjective => m_isMainObjective;

	    public Int32 DaysToPass => m_daysToPass;

	    public StepsOnTerrainData StepsOnTerrain => m_stepsOnTerrain;

	    public String Location => m_location;

	    public override String ToString()
		{
			return String.Format("[QuestObjectiveStaticData: Description={0}, MonsterClass={1}, MonsterType={2}, MonsterStaticID={3}, TokenID={4}, NpcID={5}, IsReturn={6}, IsMainObjective={7}, DaysToPass={8}, StepsOnTerrain={9}, Location={10}]", new Object[]
			{
				Description,
				MonsterClass,
				MonsterType,
				MonsterStaticID,
				TokenID,
				NpcID,
				IsReturn,
				IsMainObjective,
				DaysToPass,
				StepsOnTerrain,
				Location
			});
		}

		public void SetMainObjective(Boolean p_value)
		{
			m_isMainObjective = p_value;
		}

		public void SetNpcID(Int32 p_value)
		{
			m_npcID = p_value;
		}

		public void SetTokenID(Int32 p_value)
		{
			m_tokenID = p_value;
		}
	}
}
