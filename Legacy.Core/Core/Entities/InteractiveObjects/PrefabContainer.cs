using System;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class PrefabContainer : InteractiveObject
	{
		public PrefabContainer() : this(0, 0)
		{
		}

		public PrefabContainer(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.PREFAB_CONTAINER, p_spawnerID)
		{
		}

		public String CurrentAnim { get; set; }

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			CurrentAnim = p_data.Get<String>("CurrentAnim", String.Empty);
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<String>("CurrentAnim", (CurrentAnim == null) ? String.Empty : CurrentAnim);
		}
	}
}
