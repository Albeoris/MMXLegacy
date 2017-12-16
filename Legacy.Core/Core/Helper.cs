using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Legacy.Core
{
	public static class Helper
	{
		public static class Xml
		{
			private static XmlSerializerFactory s_Factory;

			public static Boolean OpenRead<T>(String filePath, XmlRootAttribute root, out T data) where T : class
			{
				if (!File.Exists(filePath))
				{
					throw new FileNotFoundException(filePath);
				}
				Boolean result;
				using (FileStream fileStream = File.OpenRead(filePath))
				{
					data = Deserialize<T>(fileStream, root);
					result = (data != null);
				}
				return result;
			}

			public static void OpenWrite<T>(String filePath, XmlRootAttribute root, T data) where T : class
			{
				using (FileStream fileStream = File.Create(filePath))
				{
					Serialize<T>(fileStream, root, data);
				}
			}

			public static void Serialize<T>(Stream stream, XmlRootAttribute root, T value)
			{
				using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
				{
					Serialize<T>(streamWriter, root, value);
				}
			}

			public static void Serialize<T>(TextWriter writer, XmlRootAttribute root, T value)
			{
				try
				{
					XmlSerializer xmlSerializer = CreateXmlSerializer<T>(root);
					xmlSerializer.Serialize(writer, value);
				}
				finally
				{
					if (writer != null)
					{
						((IDisposable)writer).Dispose();
					}
				}
			}

			public static TOut Deserialize<TOut>(Stream stream, XmlRootAttribute root)
			{
				TOut result;
				using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
				{
					result = Deserialize<TOut>(streamReader, root);
				}
				return result;
			}

			public static TOut Deserialize<TOut>(TextReader reader, XmlRootAttribute root)
			{
				TOut result;
				try
				{
					XmlSerializer xmlSerializer = CreateXmlSerializer<TOut>(root);
					result = (TOut)xmlSerializer.Deserialize(reader);
				}
				finally
				{
					if (reader != null)
					{
						((IDisposable)reader).Dispose();
					}
				}
				return result;
			}

			public static XmlSerializer CreateXmlSerializer<T>(XmlRootAttribute root)
			{
				if (s_Factory == null)
				{
					s_Factory = new XmlSerializerFactory();
				}
				return s_Factory.CreateSerializer(typeof(T), root);
			}
		}
	}
}
