using System;
using System.Xml.Serialization;
using Legacy.Core.StaticData;
using Legacy.Utilities;

namespace Legacy.Core
{
	public class CreditsData : IXmlStaticData
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

        public void Update(IXmlStaticData additionalData)
        {
            LegacyLogger.LogError("The method Update of the type CreditsData isn't implemented.");
        }
    }
}
