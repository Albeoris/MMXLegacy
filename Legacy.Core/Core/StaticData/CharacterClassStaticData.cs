using System;
using Dumper.Core;
using Legacy.Core.Combat;

namespace Legacy.Core.StaticData
{
	public class CharacterClassStaticData : BaseStaticData
	{
		[CsvColumn("NameKey")]
		protected String m_nameKey;

		[CsvColumn("AdvancedNameKey")]
		protected String m_advancedNameKey;

		[CsvColumn("Race")]
		protected ERace m_race;

		[CsvColumn("PosingBase")]
		protected String m_posingBase;

		[CsvColumn("BodyBase")]
		protected String m_bodyBase;

		[CsvColumn("Icon")]
		protected String m_icon;

		[CsvColumn("DefaultFemaleNames")]
		protected String[] m_defaultFemaleNames;

		[CsvColumn("DefaultMaleNames")]
		protected String[] m_defaultMaleNames;

		[CsvColumn("ExpertSkills")]
		protected Int32[] m_expertSkills;

		[CsvColumn("MasterSkills")]
		protected Int32[] m_masterSkills;

		[CsvColumn("GrandMasterSkills")]
		protected Int32[] m_grandMasterSkills;

		[CsvColumn("StartSkills")]
		protected Int32[] m_startSkills;

		[CsvColumn("DefaultSkills")]
		protected Int32[] m_defaultSkills;

		[CsvColumn("BaseMight")]
		protected Int32 m_baseMight;

		[CsvColumn("BaseMagic")]
		protected Int32 m_baseMagic;

		[CsvColumn("BasePerception")]
		protected Int32 m_basePerception;

		[CsvColumn("BaseDestiny")]
		protected Int32 m_baseDestiny;

		[CsvColumn("BaseVitality")]
		protected Int32 m_baseVitality;

		[CsvColumn("BaseSpirit")]
		protected Int32 m_baseSpirit;

		[CsvColumn("BaseHealthPoints")]
		protected Int32 m_baseHP;

		[CsvColumn("BaseManaPoints")]
		protected Int32 m_baseMana;

		[CsvColumn("DefaultMight")]
		protected Int32 m_defaultMight;

		[CsvColumn("DefaultMagic")]
		protected Int32 m_defaultMagic;

		[CsvColumn("DefaultPerception")]
		protected Int32 m_defaultPerception;

		[CsvColumn("DefaultDestiny")]
		protected Int32 m_defaultDestiny;

		[CsvColumn("DefaultVitality")]
		protected Int32 m_defaultVitality;

		[CsvColumn("DefaultSpirit")]
		protected Int32 m_defaultSpirit;

		[CsvColumn("ResistanceAir")]
		protected Int32 m_resAir;

		[CsvColumn("ResistanceEarth")]
		protected Int32 m_resEarth;

		[CsvColumn("ResistanceFire")]
		protected Int32 m_resFire;

		[CsvColumn("ResistanceWater")]
		protected Int32 m_resWater;

		[CsvColumn("ResistanceDark")]
		protected Int32 m_resDark;

		[CsvColumn("ResistanceLight")]
		protected Int32 m_resLight;

		[CsvColumn("ResistancePrimordial")]
		protected Int32 m_resPrimordial;

		[CsvColumn("StartEquipment")]
		protected EquipmentData[] m_startEquipment;

		public CharacterClassStaticData()
		{
			m_nameKey = null;
			m_race = ERace.NONE;
			m_defaultFemaleNames = null;
			m_defaultMaleNames = null;
			m_expertSkills = null;
			m_masterSkills = null;
			m_grandMasterSkills = null;
			m_resAir = 0;
			m_resEarth = 0;
			m_resFire = 0;
			m_resWater = 0;
			m_resDark = 0;
			m_resLight = 0;
			m_resPrimordial = 0;
		}

		public String NameKey => m_nameKey;

	    public String AdvancedNameKey => m_advancedNameKey;

	    public ERace Race => m_race;

	    public String PosingBase => m_posingBase;

	    public String BodyBase => m_bodyBase;

	    public String Icon => m_icon;

	    public String[] DefaultFemaleNames => m_defaultFemaleNames;

	    public String[] DefaultMaleNames => m_defaultMaleNames;

	    public Int32[] ExpertSkills => m_expertSkills;

	    public Int32[] MasterSkills => m_masterSkills;

	    public Int32[] GrandMasterSkills => m_grandMasterSkills;

	    public Int32[] StartSkills => m_startSkills;

	    public Int32[] DefaultSkills => m_defaultSkills;

	    public Int32 BaseMight => m_baseMight;

	    public Int32 BaseMagic => m_baseMagic;

	    public Int32 BasePerception => m_basePerception;

	    public Int32 BaseDestiny => m_baseDestiny;

	    public Int32 BaseVitality => m_baseVitality;

	    public Int32 BaseSpirit => m_baseSpirit;

	    public Int32 BaseHP => m_baseHP;

	    public Int32 BaseMana => m_baseMana;

	    public Int32 DefaultMight => m_defaultMight;

	    public Int32 DefaultMagic => m_defaultMagic;

	    public Int32 DefaultPercetpion => m_defaultPerception;

	    public Int32 DefaultDestiny => m_defaultDestiny;

	    public Int32 DefaultVitality => m_defaultVitality;

	    public Int32 DefaultSpirit => m_defaultSpirit;

	    public Resistance ResAir => new Resistance(EDamageType.AIR, m_resAir);

	    public Resistance ResEarth => new Resistance(EDamageType.EARTH, m_resEarth);

	    public Resistance ResFire => new Resistance(EDamageType.FIRE, m_resFire);

	    public Resistance ResWater => new Resistance(EDamageType.WATER, m_resWater);

	    public Resistance ResDark => new Resistance(EDamageType.DARK, m_resDark);

	    public Resistance ResLight => new Resistance(EDamageType.LIGHT, m_resLight);

	    public Resistance ResPrimordial => new Resistance(EDamageType.PRIMORDIAL, m_resPrimordial);

	    public EquipmentData[] StartEquipment => m_startEquipment;
	}
}
