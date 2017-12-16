using System;
using Dumper.Core;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.StaticData
{
	public class ParagonAbilitiesStaticData : BaseAbilityStaticData
	{
		[CsvColumn("DescKey")]
		protected String m_descKey = String.Empty;

		[CsvColumn("Class")]
		protected EClass m_class;

		[CsvColumn("Values")]
		protected Int32[] m_values;

		[CsvColumn("Passive")]
		protected Boolean m_passive;

		public String DescKey => m_descKey;

	    public EClass Class => m_class;

	    public Int32[] Values => m_values;

	    public Boolean Passive => m_passive;

	    public String GetDescription()
		{
			if (m_values.Length == 1)
			{
				return Localization.Instance.GetText(m_descKey, m_values[0]);
			}
			if (m_values.Length == 2)
			{
				return Localization.Instance.GetText(m_descKey, m_values[0], m_values[1]);
			}
			if (m_values.Length == 3)
			{
				return Localization.Instance.GetText(m_descKey, m_values[0], m_values[1], m_values[2]);
			}
			return Localization.Instance.GetText(m_descKey);
		}
	}
}
