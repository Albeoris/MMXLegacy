using System;
using System.IO;
using System.Xml.Serialization;

namespace Legacy.Core.Map
{
	internal static class GridLoader
	{
		private static XmlSerializer s_Serializer = new XmlSerializer(typeof(Grid));

		public static Grid Load(String p_fileName)
		{
			if (File.Exists(p_fileName))
			{
				using (FileStream fileStream = File.OpenRead(p_fileName))
				{
					Grid grid = (Grid)s_Serializer.Deserialize(fileStream);
					grid.InitConnections();
					return grid;
				}
			}
			return null;
		}
	}
}
