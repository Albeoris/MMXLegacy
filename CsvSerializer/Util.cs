using System;
using System.Text.RegularExpressions;

namespace Dumper.Core
{
	public static class Util
	{
		private static Char[] sSeperator = new Char[]
		{
			','
		};

		private static Regex sIsCsvNumber = new Regex("[^0-9.,/+/-]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public static Boolean IsCsvNumbers(String inputvalue)
		{
			return !sIsCsvNumber.IsMatch(inputvalue);
		}

		public static Boolean IsUnsignedValueType(this Type type)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.Byte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				return true;
			}
			return false;
		}

		public static String[] Split(String value)
		{
			return Split(value, StringSplitOptions.RemoveEmptyEntries);
		}

		public static String[] Split(String value, StringSplitOptions option)
		{
			return value.Split(sSeperator, option);
		}
	}
}
