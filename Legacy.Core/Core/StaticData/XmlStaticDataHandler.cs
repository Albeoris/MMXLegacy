using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Legacy.Utilities;

namespace Legacy.Core.StaticData
{
    public interface IXmlStaticData
    {
        void Update(IXmlStaticData additionalData);
    }

    public static class XmlStaticDataHandler<T> where T : class, IXmlStaticData
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
			    T result = null;
				using (FileStream fileStream = File.OpenRead(path))
				{
					T t = (T)s_serializer.Deserialize(fileStream);
					result = t;
				}

			    foreach (String file in Directory.GetFiles(RootPath, p_id + "_*.xml", SearchOption.AllDirectories))
			    {
			        using (FileStream fileStream = File.OpenRead(file))
			            result.Update((T)s_serializer.Deserialize(fileStream));
			    }

                s_staticDataMap.Add(p_id, result);
                return result;
			    

            }
			return null;
		}
	}
}
