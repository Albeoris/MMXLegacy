using System;
using System.Xml.Serialization;

namespace Legacy.Core
{
	public class CreditsData
	{
		[XmlAttribute("name")]
		public String m_head;

		[XmlElement("credits")]
		public Credits[] m_credits;

		public struct Credits
		{
			[XmlAttribute("header")]
			public String m_creditHeader;

			[XmlElement("section")]
			public CreditSection[] m_sections;
		}

		public struct CreditSection
		{
			[XmlAttribute("header")]
			public String m_sectionHeader;

			[XmlAttribute("subheader")]
			public String m_subHeader;

			[XmlElement("entry")]
			public CreditEntry[] m_entries;
		}

		public struct CreditEntry
		{
			[XmlAttribute("name")]
			public String m_name;
		}
	}
}
