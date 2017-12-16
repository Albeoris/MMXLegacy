using System;

namespace Legacy.Core.MapLoading
{
	public enum EMapLoaderState
	{
		IDLE,
		LOADING_SCENE,
		LOADING_DYNAMIC_OBJECTS,
		REENABLE_MONSTER_SPAWNS,
		CHECK_SENSORS,
		LOAD_VIEWS
	}
}
