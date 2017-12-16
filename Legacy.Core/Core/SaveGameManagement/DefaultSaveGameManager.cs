using System;
using System.Collections.Generic;
using System.IO;
using Legacy.Core.Api;

namespace Legacy.Core.SaveGameManagement
{
	public class DefaultSaveGameManager : ISaveGameManager
	{
		private ESaveGameError m_lastError;

		public ESaveGameError CheckForError()
		{
			ESaveGameError lastError = m_lastError;
			m_lastError = ESaveGameError.NONE;
			return lastError;
		}

		public Dictionary<String, SaveGameMeta> GetAllSaveGames(Boolean isForSaveMenu)
		{
			Dictionary<String, SaveGameMeta> dictionary = new Dictionary<String, SaveGameMeta>();
			String[] files;
			if (LegacyLogic.Instance.ModController.InModMode)
			{
				files = Directory.GetFiles(WorldManager.CurrentSaveGameFolder, "*.lsg", SearchOption.TopDirectoryOnly);
			}
			else
			{
				files = Directory.GetFiles(GamePaths.UserGamePath, "*.lsg", SearchOption.TopDirectoryOnly);
			}
			for (Int32 i = 0; i < files.Length; i++)
			{
				if (!files[i].Contains("global.lsg") && !files[i].Contains("global3.lsg") && !files[i].Contains("hints.lsg") && !files[i].Contains("CheatSave.lsg"))
				{
					using (FileStream fileStream = new FileStream(files[i], FileMode.Open))
					{
						using (BinaryReader binaryReader = new BinaryReader(fileStream))
						{
							SaveGameMeta value = SaveGame.ReadMetaData(binaryReader, true);
							if (value.Loaded)
							{
								value.Name = Path.GetFileNameWithoutExtension(files[i]);
								if (!isForSaveMenu || value.Type != ESaveGameType.AUTO)
								{
									dictionary.Add(value.Name, value);
								}
							}
						}
					}
				}
			}
			return dictionary;
		}

		public SaveGame LoadSaveGame(String p_file)
		{
			String path = Path.Combine(WorldManager.CurrentSaveGameFolder, p_file + ".lsg");
			if (File.Exists(path))
			{
				SaveGame saveGame = new SaveGame();
				using (FileStream fileStream = File.OpenRead(path))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						saveGame.Read(binaryReader);
					}
				}
				return saveGame;
			}
			m_lastError = ESaveGameError.COULD_NOT_LOAD_SAVEGAME;
			return null;
		}

		public void SaveSaveGame(SaveGame p_saveGame, String p_file, Byte[] p_screenshot)
		{
			String path = Path.Combine(WorldManager.CurrentSaveGameFolder, p_file + ".lsg");
			using (FileStream fileStream = File.Create(path))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					p_saveGame.Write(binaryWriter);
				}
			}
			if (p_screenshot != null)
			{
				File.WriteAllBytes(Path.Combine(WorldManager.CurrentSaveGameFolder, p_file + ".png"), p_screenshot);
			}
		}

		public void LoadSaveGameData(SaveGameData p_saveGameData, String p_file)
		{
			String path = Path.Combine(GamePaths.UserGamePath, p_file);
			if (File.Exists(path))
			{
				using (FileStream fileStream = File.OpenRead(path))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						p_saveGameData.Read(binaryReader);
					}
				}
			}
		}

		public void SaveSaveGameData(SaveGameData p_saveGameData, String p_file)
		{
			using (FileStream fileStream = File.Create(Path.Combine(GamePaths.UserGamePath, p_file)))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					p_saveGameData.Write(binaryWriter);
				}
			}
		}

		public Boolean SaveGameExists(String p_file)
		{
			return File.Exists(Path.Combine(WorldManager.CurrentSaveGameFolder, p_file + ".lsg"));
		}

		public Byte[] GetSaveGameImage(String p_file)
		{
			String path = Path.Combine(WorldManager.CurrentSaveGameFolder, p_file + ".png");
			if (File.Exists(path))
			{
				return File.ReadAllBytes(path);
			}
			return null;
		}

		public void DeleteSaveGame(String p_file)
		{
			String str = Path.Combine(WorldManager.CurrentSaveGameFolder, p_file);
			File.Delete(str + ".lsg");
			File.Delete(str + ".png");
		}
	}
}
