using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Dumper.Core
{
	public static class CsvHelper
	{
		private const String COMMENT_FLAG = "#";

		private const Char TEXT_APOSTROPHE = '"';

		private const Char SEPERATOR = ',';

		private static readonly StringBuilder sTextBuilder = new StringBuilder();

		private static readonly Char[] sSeperator = new Char[]
		{
			','
		};

		public static List<String[]> Parse(String fileName)
		{
			List<String[]> result;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				result = Parse(fileStream);
			}
			return result;
		}

		public static List<String[]> Parse(Stream stream)
		{
			return Parse(stream, Encoding.UTF8);
		}

		public static List<String[]> Parse(Stream stream, Encoding encoding)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			List<String> list = new List<String>();
			List<String[]> list2 = new List<String[]>();
			using (StreamReader streamReader = new StreamReader(stream, encoding))
			{
				Predicate<String> match = new Predicate<String>(RemoveEmptyString);
				String text;
				while ((text = streamReader.ReadLine()) != null)
				{
					String[] array;
					if (text.IndexOfAny(sSeperator, 0, 1) == 0)
					{
						array = null;
					}
					else if (text.StartsWith("#", true, invariantCulture) || text.StartsWith('"' + "#", true, invariantCulture))
					{
						array = ParseCsv(text);
						list.AddRange(array);
						list.RemoveAll(match);
						array = list.ToArray();
						list.Clear();
					}
					else
					{
						array = ParseCsv(text);
					}
					list2.Add(array);
				}
			}
			return list2;
		}

		public static String[] ParseCsv(String value)
		{
			sTextBuilder.Length = 0;
			sTextBuilder.Append(value);
			List<Int32> list = new List<Int32>();
			Int32 num = -1;
			while ((num = value.IndexOf('"', num + 1)) != -1)
			{
				list.Add(num);
			}
			List<Int32> list2 = new List<Int32>();
			num = -1;
			while ((num = value.IndexOf(',', num + 1)) != -1)
			{
				Boolean flag = false;
				for (Int32 i = 0; i < list.Count; i += 2)
				{
					Int32 num2 = list[i];
					Int32 num3 = list[i + 1];
					flag = (num > num2 && num < num3);
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					list2.Add(num);
				}
			}
			list2.Add(value.Length);
			num = 0;
			String[] array = new String[list2.Count];
			for (Int32 j = 0; j < array.Length; j++)
			{
				array[j] = sTextBuilder.ToString(num, list2[j] - num).Trim();
				num = list2[j] + 1;
			}
			RemoveApostrophe(array);
			return array;
		}

		public static Boolean ErrorCheck(List<String[]> csvData, out String errorText)
		{
			if (csvData == null)
			{
				errorText = "Array NULL";
				return false;
			}
			if (csvData.Count == 0)
			{
				errorText = "CSV not entries";
				return false;
			}
			if (csvData.Count < 2)
			{
				errorText = "CSV need min. 2 entries";
				return false;
			}
			Int32 num = csvData[0].Length;
			Int32 i = 1;
			Int32 count = csvData.Count;
			while (i < count)
			{
				String[] array = csvData[i];
				if (array != null)
				{
					if (array.Length == 1)
					{
						if (!IsComment(array))
						{
							errorText = "Error at line " + (i + 1) + ", is this a comment line?";
							return false;
						}
					}
					else if (array.Length != num)
					{
						errorText = String.Concat(new Object[]
						{
							"Error at line ",
							i + 1,
							", HeadCount=",
							num,
							", ValueCount=",
							array.Length
						});
						return false;
					}
				}
				i++;
			}
			errorText = null;
			return true;
		}

		public static void ErrorCheck(List<String[]> csvData)
		{
			String message;
			if (!ErrorCheck(csvData, out message))
			{
				throw new Exception(message);
			}
		}

		public static void Cleanup(List<String[]> csvData)
		{
			for (Int32 i = csvData.Count - 1; i >= 0; i--)
			{
				if (IsComment(csvData[i]))
				{
					csvData[i] = null;
				}
			}
		}

		public static Boolean IsComment(String[] data)
		{
			return data != null && data.Length >= 1 && !String.IsNullOrEmpty(data[0]) && (data[0].StartsWith("#", true, CultureInfo.InvariantCulture) || data[0].StartsWith('"' + "#", true, CultureInfo.InvariantCulture));
		}

		private static Boolean RemoveEmptyString(String value)
		{
			return String.IsNullOrEmpty(value);
		}

		private static void RemoveApostrophe(String[] fields)
		{
			for (Int32 i = 0; i < fields.Length; i++)
			{
				sTextBuilder.Length = 0;
				sTextBuilder.Append(fields[i]);
				sTextBuilder.Replace("\"", String.Empty);
				fields[i] = sTextBuilder.ToString();
			}
		}
	}
}
