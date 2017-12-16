using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	internal class MonsterDisappearInteraction : BaseInteraction
	{
		public MonsterDisappearInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
		}

		protected override void ParseExtra(String p_extra)
		{
		}

		protected override void DoExecute()
		{
			Monster monster = LegacyLogic.Instance.WorldManager.FindObjectBySpawnerId<Monster>(m_targetSpawnID);
			if (monster != null)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(monster, EEventType.MONSTER_DISAPPEARED, EventArgs.Empty);
			}
			base.DoExecute();
		}
	}
}
