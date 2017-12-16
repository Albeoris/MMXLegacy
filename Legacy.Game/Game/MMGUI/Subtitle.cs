using System;
using System.Xml.Serialization;

namespace Legacy.Game.MMGUI
{
	public struct Subtitle : IComparable<Subtitle>
	{
		[XmlAttribute]
		public Single Time;

		[XmlAttribute]
		public String LocaKey;

		public Int32 CompareTo(Subtitle other)
		{
			return Time.CompareTo(other.Time);
		}
	}
}
