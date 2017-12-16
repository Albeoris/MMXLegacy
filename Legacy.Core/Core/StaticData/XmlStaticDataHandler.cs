using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Legacy.Utilities;

namespace Legacy.Core.StaticData
{
	public static class XmlStaticDataHandler<T> where T : class
	{
		private static readonly XmlSerializer s_serializer = new XmlSerializer(typeof(T));

		private static readonly Dictionary<String, T> s_staticDataMap = new Dictionary<String, T>();

		public static String RootPath { get; set; }

		public static T GetStaticData(String p_id)
		{
			T t;
			if (!s_staticDataMap.TryGetValue(p_id, out t))
			{
				t = LoadData(p_id);
				if (t == null)
				{
					LegacyLogger.Log("XmlStaticData not found! ID=" + p_id);
				}
			}
			return t;
		}

		public static void Clear()
		{
			s_staticDataMap.Clear();
		}

		private static T LoadData(String p_id)
		{
			String path = Path.Combine(RootPath, p_id + ".xml");
			if (File.Exists(path))
			{
				using (FileStream fileStream = File.OpenRead(path))
				{
					T t = (T)s_serializer.Deserialize(fileStream);
					s_staticDataMap.Add(p_id, t);
					return t;
				}
			}
			return null;
		}
	}
}
