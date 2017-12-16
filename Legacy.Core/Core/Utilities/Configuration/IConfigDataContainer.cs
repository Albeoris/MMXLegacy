using System;
using System.IO;

namespace Legacy.Core.Utilities.Configuration
{
	public interface IConfigDataContainer
	{
		void Load(FileStream p_stream);

		void Write(FileStream p_stream);
	}
}
