using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.ServiceWrapper
{
	public class UplaySaveGameManager : ISaveGameManager
	{
		private List<UInt32> m_usedSlots = new List<UInt32>();

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
			UPLAY_SAVE_GameList uplay_SAVE_GameList;
			Int32 saveGameList = GetSaveGameList(out uplay_SAVE_GameList);
			if (saveGameList != 0)
			{
				IntPtr list = uplay_SAVE_GameList.list;
				for (Int32 i = 0; i < uplay_SAVE_GameList.count; i++)
				{
					IntPtr ptr = Marshal.ReadIntPtr(list, 0);
					UPLAY_SAVE_Game p_saveGame = (UPLAY_SAVE_Game)Marshal.PtrToStructure(ptr, typeof(UPLAY_SAVE_Game));
					if (p_saveGame.id > 0u)
					{
						using (MemoryStream memoryStream = OpenSaveGame(p_saveGame))
						{
							if (memoryStream == null)
							{
								return dictionary;
							}
							using (BinaryReader binaryReader = new BinaryReader(memoryStream))
							{
								SaveGameMeta value = SaveGame.ReadMetaData(binaryReader, true);
								if (value.Loaded)
								{
									value.Name = p_saveGame.nameUtf8;
									dictionary.Add(value.Name, value);
								}
							}
						}
					}
					list = new IntPtr(list.ToInt32() + IntPtr.Size);
				}
			}
			return dictionary;
		}

		public SaveGame LoadSaveGame(String p_file)
		{
			UPLAY_SAVE_GameList uplay_SAVE_GameList;
			Int32 saveGameList = GetSaveGameList(out uplay_SAVE_GameList);
			if (saveGameList != 0)
			{
				IntPtr list = uplay_SAVE_GameList.list;
				for (Int32 i = 0; i < uplay_SAVE_GameList.count; i++)
				{
					IntPtr ptr = Marshal.ReadIntPtr(list, 0);
					UPLAY_SAVE_Game p_saveGame = (UPLAY_SAVE_Game)Marshal.PtrToStructure(ptr, typeof(UPLAY_SAVE_Game));
					if (p_saveGame.nameUtf8 == p_file)
					{
						using (MemoryStream memoryStream = OpenSaveGame(p_saveGame))
						{
							if (memoryStream == null)
							{
								return null;
							}
							using (BinaryReader binaryReader = new BinaryReader(memoryStream))
							{
								SaveGame saveGame = new SaveGame();
								saveGame.Read(binaryReader);
								return saveGame;
							}
						}
					}
					list = new IntPtr(list.ToInt32() + IntPtr.Size);
				}
			}
			return null;
		}

		public void SaveSaveGame(SaveGame p_saveGame, String p_file, Byte[] p_screenshot)
		{
			UInt32 num = 0u;
			m_usedSlots.Clear();
			UPLAY_SAVE_GameList uplay_SAVE_GameList;
			Int32 num2 = GetSaveGameList(out uplay_SAVE_GameList);
			if (num2 != 0)
			{
				IntPtr list = uplay_SAVE_GameList.list;
				for (Int32 i = 0; i < uplay_SAVE_GameList.count; i++)
				{
					IntPtr ptr = Marshal.ReadIntPtr(list, 0);
					UPLAY_SAVE_Game uplay_SAVE_Game = (UPLAY_SAVE_Game)Marshal.PtrToStructure(ptr, typeof(UPLAY_SAVE_Game));
					if (uplay_SAVE_Game.nameUtf8 == p_file)
					{
						num = uplay_SAVE_Game.id;
						break;
					}
					m_usedSlots.Add(uplay_SAVE_Game.id);
					list = new IntPtr(list.ToInt32() + IntPtr.Size);
				}
			}
			if (num == 0u)
			{
				if (LegacyLogic.Instance.WorldManager.CurrentSaveGameType == ESaveGameType.AUTO)
				{
					num = 1u;
				}
				else if (LegacyLogic.Instance.WorldManager.CurrentSaveGameType == ESaveGameType.QUICK)
				{
					num = 2u;
				}
				else
				{
					UInt32 num3 = 11u;
					while (num != num3)
					{
						num = num3;
						for (Int32 j = 0; j < m_usedSlots.Count; j++)
						{
							if (num == m_usedSlots[j])
							{
								num = 0u;
								num3 += 1u;
								break;
							}
						}
					}
				}
			}
			IntPtr intPtr = CreateOverlapped();
			UInt32 aSaveHandle;
			num2 = UplayInvokes.UPLAY_SAVE_Open(num, UPLAY_SAVE_Mode.UPLAY_SAVE_MODE_Write, out aSaveHandle, intPtr);
			WaitForOverlapped(intPtr);
			UPLAY_OverlappedResult uplay_OverlappedResult;
			UplayInvokes.UPLAY_GetOverlappedOperationResult(intPtr, out uplay_OverlappedResult);
			if (num2 == 0 || uplay_OverlappedResult != UPLAY_OverlappedResult.UPLAY_OverlappedResult_Ok)
			{
				m_lastError = ESaveGameError.COULD_NOT_SAVE_SAVEGAME;
				return;
			}
			UplayInvokes.UPLAY_SAVE_SetName(aSaveHandle, p_file);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					p_saveGame.Write(binaryWriter);
					if (p_screenshot != null)
					{
						binaryWriter.Write(p_screenshot.Length);
						binaryWriter.Write(p_screenshot);
					}
					else
					{
						binaryWriter.Write(0);
					}
				}
				Byte[] array = memoryStream.ToArray();
				UPLAY_DataBlob uplay_DataBlob;
				uplay_DataBlob.data = Marshal.AllocHGlobal(array.Length);
				uplay_DataBlob.numBytes = (UInt32)array.Length;
				IntPtr intPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(uplay_DataBlob));
				Marshal.Copy(array, 0, uplay_DataBlob.data, array.Length);
				Marshal.StructureToPtr(uplay_DataBlob, intPtr2, true);
				num2 = UplayInvokes.UPLAY_SAVE_Write(aSaveHandle, (UInt32)array.Length, intPtr2, intPtr);
				WaitForOverlapped(intPtr);
				UplayInvokes.UPLAY_GetOverlappedOperationResult(intPtr, out uplay_OverlappedResult);
				if (num2 == 0 || uplay_OverlappedResult != UPLAY_OverlappedResult.UPLAY_OverlappedResult_Ok)
				{
					m_lastError = ESaveGameError.COULD_NOT_SAVE_SAVEGAME;
				}
			}
			UplayInvokes.UPLAY_SAVE_Close(aSaveHandle);
			Marshal.FreeHGlobal(intPtr);
		}

		public void LoadSaveGameData(SaveGameData p_saveGameData, String p_file)
		{
			Boolean flag = false;
			UPLAY_SAVE_GameList uplay_SAVE_GameList;
			Int32 saveGameList = GetSaveGameList(out uplay_SAVE_GameList);
			if (saveGameList != 0)
			{
				IntPtr list = uplay_SAVE_GameList.list;
				for (Int32 i = 0; i < uplay_SAVE_GameList.count; i++)
				{
					IntPtr ptr = Marshal.ReadIntPtr(list, 0);
					UPLAY_SAVE_Game p_saveGame = (UPLAY_SAVE_Game)Marshal.PtrToStructure(ptr, typeof(UPLAY_SAVE_Game));
					if (p_saveGame.nameUtf8 == p_file)
					{
						using (MemoryStream memoryStream = OpenSaveGame(p_saveGame))
						{
							if (memoryStream == null)
							{
								return;
							}
							using (BinaryReader binaryReader = new BinaryReader(memoryStream))
							{
								p_saveGameData.Read(binaryReader);
								flag = true;
								SaveSaveGameDataToDisk(p_saveGameData, p_file);
								DeleteSaveGame(p_file);
								break;
							}
						}
					}
					list = new IntPtr(list.ToInt32() + IntPtr.Size);
				}
			}
			if (!flag)
			{
				LoadSaveGameDataFromDisk(p_saveGameData, p_file);
			}
		}

		public void LoadSaveGameDataFromDisk(SaveGameData p_saveGameData, String p_file)
		{
			CreateUserSubPath();
			String path = Path.Combine(Path.Combine(GamePaths.UserGamePath, LegacyLogic.Instance.ServiceWrapper.GetUserName()), p_file);
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
			if (p_file == "global.lsg" || p_file == "global2.lsg" || p_file == "hints.lsg")
			{
				SaveSaveGameDataToDisk(p_saveGameData, p_file);
				return;
			}
		}

		private void SaveSaveGameDataToDisk(SaveGameData p_saveGameData, String p_file)
		{
			CreateUserSubPath();
			using (FileStream fileStream = File.Create(Path.Combine(Path.Combine(GamePaths.UserGamePath, LegacyLogic.Instance.ServiceWrapper.GetUserName()), p_file)))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					p_saveGameData.Write(binaryWriter);
				}
			}
		}

		private void CreateUserSubPath()
		{
			String path = Path.Combine(GamePaths.UserGamePath, LegacyLogic.Instance.ServiceWrapper.GetUserName());
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public Boolean SaveGameExists(String p_file)
		{
			UPLAY_SAVE_GameList uplay_SAVE_GameList;
			Int32 saveGameList = GetSaveGameList(out uplay_SAVE_GameList);
			if (saveGameList != 0)
			{
				IntPtr list = uplay_SAVE_GameList.list;
				for (Int32 i = 0; i < uplay_SAVE_GameList.count; i++)
				{
					IntPtr ptr = Marshal.ReadIntPtr(list, 0);
					if (((UPLAY_SAVE_Game)Marshal.PtrToStructure(ptr, typeof(UPLAY_SAVE_Game))).nameUtf8 == p_file)
					{
						return true;
					}
					list = new IntPtr(list.ToInt32() + IntPtr.Size);
				}
			}
			return false;
		}

		private MemoryStream OpenSaveGame(UPLAY_SAVE_Game p_saveGame)
		{
			IntPtr intPtr = CreateOverlapped();
			UInt32 aSaveHandle;
			Int32 num = UplayInvokes.UPLAY_SAVE_Open(p_saveGame.id, UPLAY_SAVE_Mode.UPLAY_SAVE_MODE_Read, out aSaveHandle, intPtr);
			WaitForOverlapped(intPtr);
			UPLAY_OverlappedResult uplay_OverlappedResult;
			UplayInvokes.UPLAY_GetOverlappedOperationResult(intPtr, out uplay_OverlappedResult);
			if (num == 0 || uplay_OverlappedResult != UPLAY_OverlappedResult.UPLAY_OverlappedResult_Ok)
			{
				m_lastError = ESaveGameError.COULD_NOT_LOAD_SAVEGAME;
				return null;
			}
			Byte[] array = new Byte[p_saveGame.size];
			UPLAY_DataBlob uplay_DataBlob;
			uplay_DataBlob.data = Marshal.AllocHGlobal((Int32)p_saveGame.size);
			uplay_DataBlob.numBytes = p_saveGame.size;
			IntPtr intPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(uplay_DataBlob));
			Marshal.StructureToPtr(uplay_DataBlob, intPtr2, true);
			intPtr = CreateOverlapped();
			UInt32 length;
			num = UplayInvokes.UPLAY_SAVE_Read(aSaveHandle, p_saveGame.size, 0u, intPtr2, out length, intPtr);
			WaitForOverlapped(intPtr);
			UplayInvokes.UPLAY_GetOverlappedOperationResult(intPtr, out uplay_OverlappedResult);
			if (num == 0 || uplay_OverlappedResult != UPLAY_OverlappedResult.UPLAY_OverlappedResult_Ok)
			{
				m_lastError = ESaveGameError.COULD_NOT_LOAD_SAVEGAME;
				return null;
			}
			uplay_DataBlob = (UPLAY_DataBlob)Marshal.PtrToStructure(intPtr2, typeof(UPLAY_DataBlob));
			Marshal.Copy(uplay_DataBlob.data, array, 0, (Int32)length);
			UplayInvokes.UPLAY_SAVE_Close(aSaveHandle);
			Marshal.FreeHGlobal(uplay_DataBlob.data);
			Marshal.FreeHGlobal(intPtr2);
			Marshal.FreeHGlobal(intPtr);
			return new MemoryStream(array);
		}

		public Byte[] GetSaveGameImage(String p_file)
		{
			UPLAY_SAVE_GameList uplay_SAVE_GameList;
			Int32 saveGameList = GetSaveGameList(out uplay_SAVE_GameList);
			if (saveGameList != 0)
			{
				IntPtr list = uplay_SAVE_GameList.list;
				for (Int32 i = 0; i < uplay_SAVE_GameList.count; i++)
				{
					IntPtr ptr = Marshal.ReadIntPtr(list, 0);
					UPLAY_SAVE_Game p_saveGame = (UPLAY_SAVE_Game)Marshal.PtrToStructure(ptr, typeof(UPLAY_SAVE_Game));
					if (p_saveGame.nameUtf8 == p_file)
					{
						using (MemoryStream memoryStream = OpenSaveGame(p_saveGame))
						{
							if (memoryStream == null)
							{
								return null;
							}
							using (BinaryReader binaryReader = new BinaryReader(memoryStream))
							{
								SaveGame saveGame = new SaveGame();
								saveGame.Read(binaryReader);
								Int32 num = binaryReader.ReadInt32();
								if (num > 0)
								{
									return binaryReader.ReadBytes(num);
								}
								return null;
							}
						}
					}
					list = new IntPtr(list.ToInt32() + IntPtr.Size);
				}
			}
			return null;
		}

		public void DeleteSaveGame(String p_file)
		{
			UPLAY_SAVE_GameList uplay_SAVE_GameList;
			Int32 saveGameList = GetSaveGameList(out uplay_SAVE_GameList);
			if (saveGameList != 0)
			{
				IntPtr list = uplay_SAVE_GameList.list;
				for (Int32 i = 0; i < uplay_SAVE_GameList.count; i++)
				{
					IntPtr ptr = Marshal.ReadIntPtr(list, 0);
					UPLAY_SAVE_Game uplay_SAVE_Game = (UPLAY_SAVE_Game)Marshal.PtrToStructure(ptr, typeof(UPLAY_SAVE_Game));
					if (uplay_SAVE_Game.nameUtf8 == p_file)
					{
						IntPtr intPtr = CreateOverlapped();
						UplayInvokes.UPLAY_SAVE_Remove(uplay_SAVE_Game.id, intPtr);
						WaitForOverlapped(intPtr);
						Marshal.FreeHGlobal(intPtr);
						break;
					}
					list = new IntPtr(list.ToInt32() + IntPtr.Size);
				}
			}
		}

		private Int32 GetSaveGameList(out UPLAY_SAVE_GameList p_saveGameList)
		{
			IntPtr intPtr = CreateOverlapped();
			IntPtr ptr;
			Int32 num = UplayInvokes.UPLAY_SAVE_GetSavegames(out ptr, intPtr);
			WaitForOverlapped(intPtr);
			UPLAY_OverlappedResult uplay_OverlappedResult;
			UplayInvokes.UPLAY_GetOverlappedOperationResult(intPtr, out uplay_OverlappedResult);
			if (uplay_OverlappedResult == UPLAY_OverlappedResult.UPLAY_OverlappedResult_Ok && num != 0)
			{
				p_saveGameList = (UPLAY_SAVE_GameList)Marshal.PtrToStructure(ptr, typeof(UPLAY_SAVE_GameList));
			}
			else
			{
				p_saveGameList = default(UPLAY_SAVE_GameList);
				m_lastError = ESaveGameError.COULD_NOT_RECEIVE_SAVEGAMES;
				num = 0;
			}
			Marshal.FreeHGlobal(intPtr);
			return num;
		}

		private void WaitForOverlapped(IntPtr p_overlapped)
		{
			UplayInvokes.UPLAY_Update();
			while (!UplayInvokes.UPLAY_HasOverlappedOperationCompleted(p_overlapped))
			{
				Thread.Sleep(5);
				UplayInvokes.UPLAY_Update();
			}
		}

		private IntPtr CreateOverlapped()
		{
			UPLAY_Overlapped uplay_Overlapped = default(UPLAY_Overlapped);
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uplay_Overlapped));
			Marshal.StructureToPtr(uplay_Overlapped, intPtr, true);
			return intPtr;
		}
	}
}
