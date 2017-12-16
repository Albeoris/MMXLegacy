using System;

namespace Dumper.Core
{
	public class FieldTypeResolveEventArg : EventArgs
	{
		public FieldTypeResolveEventArg(Type fieldType, Object fieldValue)
		{
			Type = fieldType;
			Value = fieldValue;
		}

		public Type Type { get; private set; }

		public Object Value { get; private set; }

		public String Output { get; set; }
	}
}
