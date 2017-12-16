using System;

namespace Legacy.Core.Utilities.Configuration
{
	public class ConfigException : Exception
	{
		public ConfigException(String p_message) : base(p_message)
		{
		}

		public ConfigException(String p_message, Exception p_innerException) : base(p_message, p_innerException)
		{
		}
	}
}
