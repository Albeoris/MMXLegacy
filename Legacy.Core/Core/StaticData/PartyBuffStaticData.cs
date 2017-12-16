using System;
using Dumper.Core;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.StaticData
{
	public class PartyBuffStaticData : BaseStaticData
	{
		[CsvColumn("BuffID")]
		protected EPartyBuffs m_buffId;

		[CsvColumn("Name")]
		protected String m_name;

		[CsvColumn("Description")]
		protected String m_description;

		[CsvColumn("Icon")]
		protected String m_icon;

		[CsvColumn("Duration")]
		protected Int32 m_duration;

		[CsvColumn("DurationIsMinutes")]
		protected Boolean m_durationIsMinutes;

		[CsvColumn("DurationScales")]
		protected Boolean m_durationScales;

		[CsvColumn("Might")]
		protected Single m_might;

		[CsvColumn("Magic")]
		protected Single m_magic;

		[CsvColumn("Perception")]
		protected Single m_perception;

		[CsvColumn("Destiny")]
		protected Single m_destiny;

		[CsvColumn("MeleeAttackValue")]
		protected Single m_meleeAttackValue;

		[CsvColumn("RangedAttackValue")]
		protected Single m_rangedAttackValue;

		[CsvColumn("Armor")]
		protected Single m_armor;

		[CsvColumn("ResistanceFire")]
		protected Single m_resistanceFire;

		[CsvColumn("ResistanceWater")]
		protected Single m_resistanceWater;

		[CsvColumn("ResistanceAir")]
		protected Single m_resistanceAir;

		[CsvColumn("ResistanceEarth")]
		protected Single m_resistanceEarth;

		[CsvColumn("ResistanceLight")]
		protected Single m_resistanceLight;

		[CsvColumn("ResistanceDarkness")]
		protected Single m_resistanceDarkness;

		[CsvColumn("ResistancePrime")]
		protected Single m_resistancePrime;

		[CsvColumn("SpecificValues")]
		protected Single[] m_specificValues;

		[CsvColumn("ValuesPercental")]
		protected Boolean m_valuesPercental;

		[CsvColumn("Gfx")]
		protected String m_gfx;

		public PartyBuffStaticData()
		{
			m_name = String.Empty;
			m_description = String.Empty;
		}

		public EPartyBuffs BuffId => m_buffId;

	    public String Name => m_name;

	    public String Description => m_description;

	    public String Icon => m_icon;

	    public Int32 Duration => m_duration;

	    public Boolean DurationIsMinutes => m_durationIsMinutes;

	    public Boolean DurationScales => m_durationScales;

	    public Single Might => m_might;

	    public Single Magic => m_magic;

	    public Single Perception => m_perception;

	    public Single Destiny => m_destiny;

	    public Single MeleeAttack => m_meleeAttackValue;

	    public Single RangedAttackValue => m_rangedAttackValue;

	    public Single Armor => m_armor;

	    public Single ResistanceFire => m_resistanceFire;

	    public Single ResistanceWater => m_resistanceWater;

	    public Single ResistanceAir => m_resistanceAir;

	    public Single ResistanceEarth => m_resistanceEarth;

	    public Single ResistanceLight => m_resistanceLight;

	    public Single ResistanceDarkness => m_resistanceDarkness;

	    public Single ResistancePrime => m_resistancePrime;

	    public Single[] SpecificValue => m_specificValues;

	    public Boolean ValuesPercental => m_valuesPercental;

	    public String Gfx => m_gfx;
	}
}
