using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	internal static class InteractionFactory
	{
		public static BaseInteraction Create(InteractiveObject creator, SpawnCommand command, Int32 spawnerID, Int32 commandIndex)
		{
			switch (command.Type)
			{
			case EInteraction.TOGGLE_DOOR:
				return new ToggleDoorInteraction(command, spawnerID, commandIndex);
			case EInteraction.OPEN_DOOR:
				return new OpenDoorInteraction(command, spawnerID, commandIndex);
			case EInteraction.CLOSE_DOOR:
				return new CloseDoorInteraction(command, spawnerID, commandIndex);
			case EInteraction.TELEPORT:
				return new TeleportInteraction(command, spawnerID, commandIndex);
			case EInteraction.USE_ENTRANCE:
				return new ChangeMapInteraction(command, spawnerID, commandIndex);
			case EInteraction.VIEW_SIGN:
				return new ViewSignInteraction(command, spawnerID, commandIndex);
			case EInteraction.TOGGLE_LEVER:
				return new ToggleLeverInteraction(command, spawnerID, commandIndex);
			case EInteraction.LEVER_UP:
				return new LeverUpInteraction(command, spawnerID, commandIndex);
			case EInteraction.LEVER_DOWN:
				return new LeverDownInteraction(command, spawnerID, commandIndex);
			case EInteraction.TOGGLE_BUTTON:
				return new ToggleButtonInteraction(command, spawnerID, commandIndex);
			case EInteraction.BUTTON_UP:
				return new ButtonUpInteraction(command, spawnerID, commandIndex);
			case EInteraction.BUTTON_DOWN:
				return new ButtonDownInteraction(command, spawnerID, commandIndex);
			case EInteraction.OPEN_CONTAINER:
				return new OpenContainerInteraction(command, spawnerID, commandIndex);
			case EInteraction.START_DIALOGUE:
				return new StartDialogueInteraction(command, spawnerID, commandIndex);
			case EInteraction.SET_DATA:
				return new SetDataInteraction(creator, command, spawnerID, commandIndex);
			case EInteraction.CAST_PARTY_SPELL:
				return new CastCharacterSpellInteraction(command, spawnerID, commandIndex);
			case EInteraction.CAST_MONSTER_SPELL:
				return new CastMonsterSpellInteraction(command, spawnerID, commandIndex);
			case EInteraction.GIVE_XP:
				return new GiveXPInteraction(command, spawnerID, commandIndex);
			case EInteraction.CHANGE_ATTRIBUTE:
				return new IncreaseAttributeInteraction(command, spawnerID, commandIndex);
			case EInteraction.SPAWN_MONSTER:
				return new SpawnMonsterInteraction(command, spawnerID, commandIndex);
			case EInteraction.ADD_TOKEN:
				return new AddTokenInteraction(command, spawnerID, commandIndex);
			case EInteraction.TRIGGER_CUTSCENE:
				return new CutsceneInteraction(command, spawnerID, commandIndex);
			case EInteraction.DISARM_TRAP:
				return new DisarmTrapInteraction(command, spawnerID, commandIndex);
			case EInteraction.REMOVE_TOKEN:
				return new RemoveTokenInteraction(command, spawnerID, commandIndex);
			case EInteraction.TRIGGER_ANIM:
				return new TriggerAnimationInteraction(command, spawnerID, commandIndex);
			case EInteraction.MOVE_OBJECT:
				return new MoveObjectInteraction(command, spawnerID, commandIndex);
			case EInteraction.ROTATE_PARTY:
				return new RotatePartyInteraction(command, spawnerID, commandIndex);
			case EInteraction.START_DEFINED_DIALOG:
				return new StartDefinedNpcDialogInteraction(command, spawnerID, commandIndex);
			case EInteraction.SET_ENABLED:
				return new SetEnabledInteraction(command, spawnerID, commandIndex);
			case EInteraction.GAME_MESSAGE:
				return new GameMessageInteraction(command, spawnerID, commandIndex);
			case EInteraction.TRIGGER_HINT:
				return new TriggerHintInteraction(command, spawnerID, commandIndex);
			case EInteraction.SET_COUNTER:
				return new SetCounterInteraction(command, spawnerID, commandIndex);
			case EInteraction.CHANGE_COUNTER:
				return new ChangeCounterInteraction(command, spawnerID, commandIndex);
			case EInteraction.ADD_LOREBOOK:
				return new AddLorebookInteraction(command, spawnerID, commandIndex);
			case EInteraction.REMOVE_BUFF:
				return new RemoveBuffInteraction(command, spawnerID, commandIndex);
			case EInteraction.UPDATE_SLOT_HEIGHT:
				return new UpdateGridSlotHeightInteraction(command, spawnerID, commandIndex);
			case EInteraction.CHECK_PASSABLE_ON_MOVEMENT:
				return new CheckPassableOnMoveInteraction(command, spawnerID, commandIndex);
			case EInteraction.SET_GRID_HEIGHT:
				return new SetGridHeightInteraction(command, spawnerID, commandIndex);
			case EInteraction.MOVE_PLATFORM:
				return new MovePlatformInteraction(command, spawnerID, commandIndex);
			case EInteraction.RESET_PLATFORM:
				return new ResetPlatformInteraction(command, spawnerID, commandIndex);
			case EInteraction.RESET_GRID_HEIGHT:
				return new ResetGridHeightInteraction(command, spawnerID, commandIndex);
			case EInteraction.CHANGE_TERRAIN:
				return new ChangeTerrainTypeInteraction(command, spawnerID, commandIndex);
			case EInteraction.SET_SLOT_HEIGHT:
				return new SetGridSlotHeightInteraction(command, spawnerID, commandIndex);
			case EInteraction.EXECUTE_INTERACTIVE_OBJECT:
				return new ExecuteInteractiveObjectInteraction(command, spawnerID, commandIndex);
			case EInteraction.UPDATE_QUEST_OBJECTIVE:
				return new UpdateQuestObjectiveInteraction(command, spawnerID, commandIndex);
			case EInteraction.BARREL_INTERACTION:
				return new BarrelInteraction(command, spawnerID, commandIndex);
			case EInteraction.RECHARGER_INTERACTION:
				return new RechargerInteraction(command, spawnerID, commandIndex);
			case EInteraction.SET_STATE:
				return new SetStateInteraction(command, spawnerID, commandIndex);
			case EInteraction.MOVE_MONSTER:
				return new MoveMonsterInteraction(command, spawnerID, commandIndex);
			case EInteraction.SHRINE_INTERACTION:
				return new ShrineInteraction(command, spawnerID, commandIndex);
			case EInteraction.NO_SHOES_INTERACTION:
				return new NoShoesInteraction(command, spawnerID, commandIndex);
			case EInteraction.CHALLENGE_INTERACTION:
				return new ChallengeInteraction(command, spawnerID, commandIndex);
			case EInteraction.DO_DAMAGE_INTERACTION:
				return new DoDamageInteraction(command, spawnerID, commandIndex);
			case EInteraction.ENABLE_MONSTER_SPAWN_INTERACTION:
				return new EnableMonsterSpawnInteraction(command, spawnerID, commandIndex);
			case EInteraction.TRIGGER_BARK:
				return new TriggerBarkInteraction(command, spawnerID, commandIndex);
			case EInteraction.MONSTER_DISAPPEAR:
				return new MonsterDisappearInteraction(command, spawnerID, commandIndex);
			case EInteraction.MOVE_OBJECT_TO_OBJECT:
				return new MoveObjectToTargetInteraction(command, spawnerID, commandIndex);
			case EInteraction.GAME_OVER_INTERACTION:
				return new GameOverInteraction(command, spawnerID, commandIndex);
			case EInteraction.MOVE_HIDDEN_LOOT:
				return new MoveHiddenLootInteraction(command, spawnerID, commandIndex);
			}
			throw new NotImplementedException("Not implemented type " + command.Type);
		}
	}
}
