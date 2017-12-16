using System;
using System.Collections.Generic;

namespace Legacy.Core.Entities.Items
{
	public interface IDescribable
	{
		String GetTypeDescription();

		Dictionary<String, String> GetPropertiesDescription();
	}
}
