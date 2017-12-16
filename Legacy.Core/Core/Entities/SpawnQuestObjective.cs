using System;
using System.Xml.Serialization;

namespace Legacy.Core.Entities
{
	[Serializable]
	public class SpawnQuestObjective
	{
		private Int32 m_ID;

		[XmlAttribute("ID")]
		public Int32 ID
		{
			get => m_ID;
		    set => m_ID = value;
		}
	}
}
