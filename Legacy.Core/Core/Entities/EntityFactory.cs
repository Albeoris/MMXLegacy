using System;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.Entities
{
	internal static class EntityFactory
	{
		public static BaseObject Create(EObjectType p_type, Int32 p_staticID, Int32 p_spawnID)
		{
			switch (p_type)
			{
			case EObjectType.MONSTER:
				return new Monster(p_staticID, p_spawnID);
			case EObjectType.SIGN:
				return new Sign(p_staticID, p_spawnID);
			case EObjectType.DOOR:
				return new Door(p_staticID, p_spawnID);
			case EObjectType.ENTRANCE:
				return new Entrance(p_staticID, p_spawnID);
			case EObjectType.TELEPORTER:
				return new Teleporter(p_staticID, p_spawnID);
			case EObjectType.LEVER:
				return new Lever(p_staticID, p_spawnID);
			case EObjectType.BUTTON:
				return new Button(p_staticID, p_spawnID);
			case EObjectType.SENSOR:
				return new Sensor(p_staticID, p_spawnID);
			case EObjectType.CONTAINER:
				return new Container(p_staticID, p_spawnID);
			case EObjectType.NPC_CONTAINER:
				return new NpcContainer(p_staticID, p_spawnID);
			case EObjectType.TRAP:
				return new Trap(p_staticID, p_spawnID);
			case EObjectType.CONDITIONAL_TRIGGER:
				return new ConditionalTrigger(p_staticID, p_spawnID);
			case EObjectType.CUTSCENE:
				return new Cutscene(p_spawnID);
			case EObjectType.TRAP_EFFECT_CONTAINER:
				return new TrapEffectContainer(p_staticID, p_spawnID);
			case EObjectType.SUMMON:
				return new Summon(p_staticID, p_spawnID);
			case EObjectType.PRESSURE_PLATE:
				return new PressurePlate(p_staticID, p_spawnID);
			case EObjectType.PREFAB_CONTAINER:
				return new PrefabContainer(p_staticID, p_spawnID);
			case EObjectType.COUNTING_OBJECT:
				return new CounterObject(p_staticID, p_spawnID);
			case EObjectType.PLATFORM:
				return new Platform(p_staticID, p_spawnID);
			case EObjectType.PLACEHOLDER:
				return new PlaceHolder(p_staticID, p_spawnID);
			case EObjectType.COMMAND_CONTAINER:
				return new CommandContainer(p_staticID, p_spawnID);
			case EObjectType.BARREL:
				return new Barrel(p_staticID, p_spawnID);
			case EObjectType.RECHARGING_OBJECT:
				return new RechargingObject(p_staticID, p_spawnID);
			case EObjectType.SHRINE:
				return new Shrine(p_staticID, p_spawnID);
			}
			throw new NotImplementedException(String.Concat(new Object[]
			{
				"Create unknown, ObjectType: ",
				p_type,
				"; StaticID: ",
				p_staticID,
				"; SpawnID: ",
				p_spawnID
			}));
		}
	}
}
