using System;
using System.Collections.Generic;

namespace Legacy.Core.SaveGameManagement
{
	public interface ISaveGameManager
	{
		Dictionary<String, SaveGameMeta> GetAllSaveGames(Boolean isForSaveMenu);

		SaveGame LoadSaveGame(String p_file);

		void SaveSaveGame(SaveGame p_saveGame, String p_file, Byte[] p_screenshot);

		Boolean SaveGameExists(String p_file);

		Byte[] GetSaveGameImage(String p_file);

		void DeleteSaveGame(String p_file);

		void LoadSaveGameData(SaveGameData p_saveGameData, String p_file);

		void SaveSaveGameData(SaveGameData p_saveGameData, String p_file);

		ESaveGameError CheckForError();
	}
}
