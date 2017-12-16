using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Legacy.Utilities;

namespace Legacy.Core.Internationalization
{
	public class Localization
	{
		private static Localization m_instance;

		private static readonly StringBuilder s_TextBuilder = new StringBuilder(128);

		private static Dictionary<String, String> s_LocaContent = new Dictionary<String, String>(StringComparer.InvariantCultureIgnoreCase);

		private static Dictionary<String, String> s_FallBackContent = new Dictionary<String, String>(StringComparer.InvariantCultureIgnoreCase);

		private Localization()
		{
		}

		public static Localization Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new Localization();
				}
				return m_instance;
			}
		}

		public void ClearContent()
		{
			s_LocaContent.Clear();
		}

		public void ClearFallBackContent()
		{
			s_FallBackContent.Clear();
		}

		public void LoadLanguage(String filePath)
		{
			ParseLocaFile(filePath, s_LocaContent);
		}

		public void LoadFallback(String filePath)
		{
			ParseLocaFile(filePath, s_FallBackContent);
		}

		private static void ParseLocaFile(String filePath, Dictionary<String, String> p_target)
		{
			try
			{
				LocaData[] array;
				Helper.Xml.OpenRead<LocaData[]>(filePath, LocaData.XmlRoot, out array);
				if (array != null)
				{
					AddLocaData(filePath, array, p_target);
				}
				else
				{
					LegacyLogger.Log("root Element is null");
				}
			}
			catch (Exception ex)
			{
				LegacyLogger.Log("Error in the " + filePath + ": " + ex.ToString());
			}
		}

		private static void AddLocaData(String fileOrigin, LocaData[] data, Dictionary<String, String> p_target)
		{
			try
			{
				for (Int32 i = 0; i < data.Length; i++)
				{
					if (p_target.ContainsKey(data[i].ID))
					{
						LegacyLogger.Log("Double LocaID '" + data[i].ID + "' in " + fileOrigin);
					}
					else if (data[i].Text != null)
					{
						p_target.Add(data[i].ID, data[i].Text.Trim());
					}
					else
					{
						LegacyLogger.Log("LocaID '" + data[i].ID + "' has no text", false);
					}
				}
			}
			catch (Exception ex)
			{
				LegacyLogger.Log("Error initializing LocaTexts: " + fileOrigin + " " + ex.ToString());
			}
		}

		public Boolean RetriveKey(String id, out String res)
		{
			String text;
			if (!String.IsNullOrEmpty(id) && s_LocaContent.TryGetValue(id, out text))
			{
				res = text;
				return true;
			}
			if (!String.IsNullOrEmpty(id) && s_FallBackContent.TryGetValue(id, out text))
			{
				res = text;
				return true;
			}
			res = id;
			return false;
		}

		public Boolean RetriveFallback(String id, out String res)
		{
			String text;
			if (!String.IsNullOrEmpty(id) && s_FallBackContent.TryGetValue(id, out text))
			{
				res = text;
				return true;
			}
			res = id;
			return false;
		}

		public String GetText(String id, Object arg)
		{
			String format;
			if (RetriveKey(id, out format))
			{
				s_TextBuilder.Length = 0;
				try
				{
					return s_TextBuilder.AppendFormat(format, arg).ToString();
				}
				catch (Exception)
				{
					if (RetriveFallback(id, out format))
					{
						s_TextBuilder.Length = 0;
						return s_TextBuilder.AppendFormat(format, arg).ToString();
					}
				}
			}
			return "404 '" + id + "'";
		}

		public String GetText(String id, Object arg1, Object arg2)
		{
			String format;
			if (RetriveKey(id, out format))
			{
				s_TextBuilder.Length = 0;
				try
				{
					return s_TextBuilder.AppendFormat(format, arg1, arg2).ToString();
				}
				catch (Exception)
				{
					if (RetriveFallback(id, out format))
					{
						s_TextBuilder.Length = 0;
						return s_TextBuilder.AppendFormat(format, arg1, arg2).ToString();
					}
				}
			}
			return "404 '" + id + "'";
		}

		public String GetText(String id, Object arg1, Object arg2, Object arg3)
		{
			String format;
			if (RetriveKey(id, out format))
			{
				s_TextBuilder.Length = 0;
				try
				{
					return s_TextBuilder.AppendFormat(format, arg1, arg2, arg3).ToString();
				}
				catch (Exception)
				{
					if (RetriveFallback(id, out format))
					{
						s_TextBuilder.Length = 0;
						return s_TextBuilder.AppendFormat(format, arg1, arg2, arg3).ToString();
					}
				}
			}
			return "404 '" + id + "'";
		}

		public String GetText(String id, params Object[] args)
		{
			String text;
			if (RetriveKey(id, out text))
			{
				if (args == null || args.Length == 0)
				{
					return text;
				}
				s_TextBuilder.Length = 0;
				try
				{
					return s_TextBuilder.AppendFormat(text, args).ToString();
				}
				catch (Exception)
				{
					if (RetriveFallback(id, out text))
					{
						s_TextBuilder.Length = 0;
						return s_TextBuilder.AppendFormat(text, args).ToString();
					}
				}
			}
			return "404 '" + id + "'";
		}

		public String GetText(String id)
		{
			String result;
			if (RetriveKey(id, out result))
			{
				return result;
			}
			return "404 '" + id + "'";
		}

	    public void SetText(String id, String text)
	    {
	        s_LocaContent[id] = text;
        }

        public void AppendText(StringBuilder builder, String id, Object arg)
		{
			String format;
			if (RetriveKey(id, out format))
			{
				try
				{
					builder.AppendFormat(format, arg).ToString();
				}
				catch (Exception)
				{
					if (RetriveFallback(id, out format))
					{
						builder.AppendFormat(format, arg).ToString();
					}
				}
			}
			else
			{
				builder.Append("404 '" + id + "'");
			}
		}

		public void AppendText(StringBuilder builder, String id, Object arg1, Object arg2)
		{
			String format;
			if (RetriveKey(id, out format))
			{
				try
				{
					builder.AppendFormat(format, arg1, arg2).ToString();
				}
				catch (Exception)
				{
					if (RetriveFallback(id, out format))
					{
						builder.AppendFormat(format, arg1, arg2).ToString();
					}
				}
			}
			else
			{
				builder.Append("404 '" + id + "'");
			}
		}

		public void AppendText(StringBuilder builder, String id, Object arg1, Object arg2, Object arg3)
		{
			String format;
			if (RetriveKey(id, out format))
			{
				try
				{
					builder.AppendFormat(format, arg1, arg2, arg3).ToString();
				}
				catch (Exception)
				{
					if (RetriveFallback(id, out format))
					{
						builder.AppendFormat(format, arg1, arg2, arg3).ToString();
					}
				}
			}
			else
			{
				builder.Append("404 '" + id + "'");
			}
		}

		public void AppendText(StringBuilder builder, String id, params Object[] args)
		{
			String format;
			if (RetriveKey(id, out format))
			{
				if (args == null || args.Length == 0)
				{
					return;
				}
				try
				{
					builder.AppendFormat(format, args).ToString();
				}
				catch (Exception)
				{
					if (RetriveFallback(id, out format))
					{
						builder.AppendFormat(format, args).ToString();
					}
				}
			}
			else
			{
				builder.Append("404 '" + id + "'");
			}
		}

		public void AppendText(StringBuilder builder, String id)
		{
			String value;
			if (RetriveKey(id, out value))
			{
				builder.Append(value);
			}
			else
			{
				builder.Append("404 '" + id + "'");
			}
		}

		public struct LocaData
		{
			public static readonly XmlRootAttribute XmlRoot = new XmlRootAttribute("Localization");

			[XmlAttribute("id")]
			public String ID;

			[XmlText]
			public String Text;
		}
	}
}
