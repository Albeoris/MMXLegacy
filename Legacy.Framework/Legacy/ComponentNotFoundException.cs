using System;
using System.Runtime.Serialization;

namespace Legacy
{
	[Serializable]
	public class ComponentNotFoundException : Exception
	{
		public ComponentNotFoundException()
		{
		}

		public ComponentNotFoundException(String message) : base(message)
		{
		}

		public ComponentNotFoundException(String message, Exception inner) : base(message, inner)
		{
		}

		protected ComponentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
