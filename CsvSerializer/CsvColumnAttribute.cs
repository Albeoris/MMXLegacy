using System;

namespace Dumper.Core
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class CsvColumnAttribute : Attribute
	{
		public readonly String Column;

		public CsvColumnAttribute(String columnName)
		{
			Column = columnName;
		}
	}
}
