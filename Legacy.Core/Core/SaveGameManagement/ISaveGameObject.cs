using System;

namespace Legacy.Core.SaveGameManagement
{
	public interface ISaveGameObject
	{
		void Load(SaveGameData p_data);

		void Save(SaveGameData p_data);
	}
}
