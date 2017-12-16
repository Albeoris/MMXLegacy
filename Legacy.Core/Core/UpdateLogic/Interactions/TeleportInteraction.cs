using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class TeleportInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		public const Boolean DEFAULT_IGNORE_TARGET_DIRECTION = true;

		private InteractiveObject m_teleporter;

		private Boolean m_ignoreTargetDirection;

		protected InteractiveObject m_parent;

		public TeleportInteraction()
		{
			m_ignoreTargetDirection = true;
		}

		public TeleportInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			InteractiveObject teleporter = Grid.FindInteractiveObject(m_targetSpawnID);
			m_teleporter = teleporter;
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		public Boolean IgnoreTargetDirection => m_ignoreTargetDirection;

	    public Boolean IsParentTeleporter => m_parent != null && m_parent.Type == EObjectType.TELEPORTER;

	    protected override void DoExecute()
		{
			if (m_teleporter == null)
			{
				throw new InvalidOperationException("Tried to teleport to invalif object");
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (!m_ignoreTargetDirection && m_teleporter.Location != EDirection.CENTER)
			{
				party.Direction = m_teleporter.Location;
			}
			Position position = party.Position;
			if (Grid.AddMovingEntity(m_teleporter.Position, party))
			{
				GridSlot slot = Grid.GetSlot(m_teleporter.Position);
				if (!slot.VisitedByParty)
				{
					slot.VisitedByParty = true;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UNCOVERED_TILES, EventArgs.Empty);
				}
				Grid.GetSlot(position).RemoveEntity(party);
				party.SelectedInteractiveObject = null;
				BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(party, party.Position);
				LegacyLogic.Instance.EventManager.InvokeEvent(party, EEventType.TELEPORT_ENTITY, p_eventArgs);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_TELEPORTER_USED, EventArgs.Empty);
			}
			else
			{
				LegacyLogger.Log(String.Concat(new Object[]
				{
					"Could not move party ",
					m_teleporter.SpawnerID,
					" ",
					m_teleporter.Position.ToString()
				}));
			}
			FinishExecution();
		}

		internal void ParseExtraEditor(String p_extra)
		{
			ParseExtra(p_extra);
		}

		protected override void ParseExtra(String p_extra)
		{
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (array.Length != 1)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					1
				}));
			}
			if (!Boolean.TryParse(array[0], out m_ignoreTargetDirection))
			{
				throw new FormatException("First parameter was not a bool!");
			}
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
	}
}
