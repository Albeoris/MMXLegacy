using System;

namespace Dumper.Core
{
	public class ColumnValueResolveEventArg : EventArgs
	{
		public ColumnValueResolveEventArg(Type columnType, String columnValue)
		{
			Type = columnType;
			Value = columnValue;
		}

		public Type Type { get; private set; }

		public String Value { get; private set; }

		public Object Output { get; set; }
	}
}
