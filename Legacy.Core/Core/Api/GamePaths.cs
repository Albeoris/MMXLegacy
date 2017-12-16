using System;
using System.IO;

namespace Legacy.Core.Api
{
	internal static class GamePaths
	{
		private const String MOD_FOLDER = "Mods";

		private const String USER_GAME_FOLDER = "MightAndMagicXLegacy";

		static GamePaths()
		{
			String folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			UserGamePath = Path.Combine(folderPath, "MightAndMagicXLegacy");
			if (!Directory.Exists(UserGamePath))
			{
				Directory.CreateDirectory(UserGamePath);
			}
			ModsRootPath = Path.Combine(UserGamePath, "Mods");
			if (!Directory.Exists(ModsRootPath))
			{
				Directory.CreateDirectory(ModsRootPath);
			}
		}

		public static String UserGamePath { get; private set; }

		public static String ModsRootPath { get; private set; }
	}
}
