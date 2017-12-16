using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ResetPlatformInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 2;

		protected InteractiveObject m_parent;

		public ResetPlatformInteraction()
		{
		}

		public ResetPlatformInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PLATFORM_RESET, null);
			FinishExecution();
		}

		public override void FinishExecution()
		{
			if (m_parent != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_parent.DecreaseActivate(m_commandIndex);
			}
			base.FinishExecution();
		}

		protected override void ParseExtra(String p_extra)
		{
		}

		public void Notify(InteractiveObject p_obj, Position oldPos, Position newPos)
		{
			GridSlot slot = Grid.GetSlot(oldPos);
			slot.TerrainType = ETerrainType.LAVA;
			slot = Grid.GetSlot(newPos);
			slot.TerrainType = ETerrainType.PASSABLE;
			p_obj.Position = newPos;
			Grid.MoveObject(p_obj, newPos);
		}
	}
}
