using System;

namespace Legacy
{
	public static class Define
	{
		public const Int32 SLOT_SIZE = 10;

		public const String COLORED_TEXT_END = "[-]";

		public static class ResourcePaths
		{
			public const String GUI_PREFABS_ROOT = "GuiPrefabs/";

			public const String DIALOG_OK_PREFAB = "GuiPrefabs/Dialog_OK";

			public const String DIALOG_YES_NO_PREFAB = "GuiPrefabs/Dialog_YES_NO";

			public const String HUDDamageText = "GuiPrefabs/HUDDamageText";

			public const String HUDPortaitDamageText = "GuiPrefabs/HUDPortaitDamageText";

			public const String HUDBuff = "GuiPrefabs/HUDBuff";

			public const String GUI_BLOCKER = "GuiPrefabs/GUIBlocker";

			public const String MAPS = "Maps/";

			public const String UNLOCK_CONTENT = "UnlockContent/";

			public const String ENDING_SLIDES = "EndingSlides/";
		}

		public static class GOMessages
		{
			public const String SHOW_DIALOG = "OnShowDialog";

			public const String TAB_BTN_CLICK = "OnTabButtonClick";

			public const String BASE_SCREEN_AFTER_ENABLE = "OnAfterEnable";

			public const String RECEIVED_ATTACKS = "OnReceivedAttacks";

			public const String BEGIN_EFFECT = "OnBeginEffect";

			public const String END_EFFECT = "OnEndEffect";
		}

		public static class EventMessages
		{
			public const String ON_LOCALIZE = "OnLocalize";
		}

		public static class Scenes
		{
			public const String INTRO = "Intro";

			public const String MAINMENU = "Mainmenu";

			public const String GAME = "Game";

			public const String EMPTY = "Empty";
		}

		public static class Tags
		{
			public const String RESPWAN = "Respawn";

			public const String FINISH = "Finish";

			public const String EDITOR_ONLY = "EditorOnly";

			public const String MAIN_CAMERA = "MainCamera";

			public const String PLAYER = "Player";

			public const String GAME_CONTROLLER = "GameController";

			public const String GRID_ORIGIN = "Grid Origin";

			public const String FX_HIT_SPOT = "HitSpot";

			public const String FX_BLOCK_HIT_SPOT = "BlockHitSpot";

			public const String SCENE_CAMERA = "SceneCamera";

			public const String SCENE_SUN = "SceneSun";

			public const String UI_CAMERA = "UICamera";
		}

		public static class Layers
		{
			public const Int32 DEFAULT = 0;

			public const Int32 TRANSPARENT_FX = 1;

			public const Int32 IGNORE_RAYCAST = 2;

			public const Int32 WATER = 8;

			public const Int32 GUI = 256;

			public const Int32 NPC = 512;

			public const Int32 INDOOR = 1024;

			public const Int32 GUI_MINIMAP = 2048;

			public const Int32 DEFAULT_GEOMETRY = 32768;

			public const Int32 CEILING_GEOMETRY = 65536;
		}
	}
}
