using System;
using System.Xml.Serialization;

namespace Legacy.Core.Map
{
	public class GridInfo
	{
		[XmlElement("Name")]
		public String Name { get; set; }

		[XmlElement("LocationLocaName")]
		public String LocationLocaName { get; set; }
	}
}
