using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Legacy.Core.Utilities.Configuration
{
	public class ConfigSetting
	{
		private String m_name;

		private String m_value;

		public ConfigSetting(String p_name) : this(p_name, null)
		{
		}

		public ConfigSetting(String p_name, String p_value)
		{
			m_name = p_name;
			m_value = ((p_value != null) ? p_value : String.Empty);
		}

		public String Name => m_name;

	    public String Value
		{
			get => m_value;
	        set => m_value = value;
	    }

		public Int32 GetInt()
		{
			return GetInt(0);
		}

		public Int32 GetInt(Int32 defaultValue)
		{
			Int32 result;
			if (!Int32.TryParse(m_value, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		public Single GetFloat()
		{
			return GetFloat(0f);
		}

		public Single GetFloat(Single defaultValue)
		{
			Single result;
			if (!Single.TryParse(m_value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		public Boolean GetBool()
		{
			return ConvertValueToBool(m_value, false);
		}

		public Boolean GetBool(Boolean defaultValue)
		{
			return ConvertValueToBool(m_value, defaultValue);
		}

		public String GetString()
		{
			return GetString(null);
		}

		public String GetString(String defaultValue)
		{
			if (String.IsNullOrEmpty(m_value) || m_value == "\"\"")
			{
				return defaultValue;
			}
			if (m_value.Length > 1 && m_value.StartsWith("\"") && m_value.EndsWith("\""))
			{
				return m_value.Substring(1, m_value.Length - 2);
			}
			return m_value;
		}

		public T GetEnum<T>() where T : struct
		{
			return GetEnum<T>(default(T));
		}

		public T GetEnum<T>(T defaultValue) where T : struct
		{
			if (String.IsNullOrEmpty(m_value))
			{
				return defaultValue;
			}
			T result;
			try
			{
				result = (T)Enum.Parse(typeof(T), m_value, true);
			}
			catch (ArgumentException)
			{
				result = defaultValue;
			}
			return result;
		}

		public Int32[] GetIntArray()
		{
			String[] array = m_value.Split(new Char[]
			{
				','
			});
			Int32[] array2 = new Int32[array.Length];
			for (Int32 i = 0; i < array.Length; i++)
			{
				array2[i] = Int32.Parse(array[i].Trim(), CultureInfo.InvariantCulture.NumberFormat);
			}
			return array2;
		}

		public Single[] GetFloatArray()
		{
			String[] array = m_value.Split(new Char[]
			{
				','
			});
			Single[] array2 = new Single[array.Length];
			for (Int32 i = 0; i < array.Length; i++)
			{
				array2[i] = Single.Parse(array[i].Trim(), CultureInfo.InvariantCulture.NumberFormat);
			}
			return array2;
		}

		public Boolean[] GetBoolArray()
		{
			String[] array = m_value.Split(new Char[]
			{
				','
			});
			Boolean[] array2 = new Boolean[array.Length];
			for (Int32 i = 0; i < array.Length; i++)
			{
				array2[i] = ConvertValueToBool(array[i].Trim(), false);
			}
			return array2;
		}

		public String[] GetStringArray()
		{
			Match match = Regex.Match(m_value, "[\\\"][^\\\"]*[\\\"][,]*");
			List<String> list = new List<String>();
			while (match.Success)
			{
				String text = match.Value;
				if (text.EndsWith(","))
				{
					text = text.Substring(0, text.Length - 1);
				}
				text = text.Trim();
				text = text.Substring(1, text.Length - 2);
				list.Add(text);
				match = match.NextMatch();
			}
			return list.ToArray();
		}

		public void SetValue(Int32 p_value)
		{
			m_value = p_value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}

		public void SetValue(Single p_value)
		{
			m_value = p_value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}

		public void SetValue(Boolean p_value)
		{
			m_value = p_value.ToString();
		}

		public void SetValue(String p_value)
		{
			p_value = (p_value ?? String.Empty);
			if (!p_value.StartsWith("\""))
			{
				p_value = "\"" + p_value;
			}
			if (!p_value.EndsWith("\""))
			{
				p_value += "\"";
			}
			m_value = p_value;
		}

		public void SetEnumValue<T>(T p_value) where T : struct
		{
			m_value = p_value.ToString();
		}

		public void SetValue(params Int32[] p_values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (Int32 i = 0; i < p_values.Length; i++)
			{
				stringBuilder.Append(p_values[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
				if (i < p_values.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			m_value = stringBuilder.ToString();
		}

		public void SetValue(params Single[] p_values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (Int32 i = 0; i < p_values.Length; i++)
			{
				stringBuilder.Append(p_values[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
				if (i < p_values.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			m_value = stringBuilder.ToString();
		}

		public void SetValue(params Boolean[] p_values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (Int32 i = 0; i < p_values.Length; i++)
			{
				stringBuilder.Append(p_values[i].ToString());
				if (i < p_values.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			m_value = stringBuilder.ToString();
		}

		public void SetValue(params String[] p_values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (Int32 i = 0; i < p_values.Length; i++)
			{
				if (!p_values[i].StartsWith("\""))
				{
					stringBuilder.Append('"');
				}
				stringBuilder.Append(p_values[i]);
				if (!p_values[i].EndsWith("\""))
				{
					stringBuilder.Append('"');
				}
				if (i < p_values.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			m_value = stringBuilder.ToString();
		}

		private static Boolean ConvertValueToBool(String p_value, Boolean defaultValue)
		{
			if (p_value.Equals("true", StringComparison.InvariantCultureIgnoreCase) || p_value.Equals("on", StringComparison.InvariantCultureIgnoreCase) || p_value.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (p_value.Equals("false", StringComparison.InvariantCultureIgnoreCase) || p_value.Equals("off", StringComparison.InvariantCultureIgnoreCase) || p_value.Equals("no", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			Int32 num;
			if (!Int32.TryParse(p_value, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return defaultValue;
			}
			return num != 0;
		}
	}
}
