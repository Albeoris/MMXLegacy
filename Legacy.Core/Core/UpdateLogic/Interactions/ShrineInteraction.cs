using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ShrineInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		protected InteractiveObject m_parent;

		private Int32 m_shrineID;

		public ShrineInteraction()
		{
		}

		public ShrineInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
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
			m_shrineID = Convert.ToInt32(array[0]);
		}

		protected override void DoExecute()
		{
			ShrineStaticData staticData = StaticDataHandler.GetStaticData<ShrineStaticData>(EDataType.SHRINES, m_shrineID);
			if (staticData != null)
			{
				TokenStaticData staticData2 = StaticDataHandler.GetStaticData<TokenStaticData>(EDataType.TOKEN, staticData.TokenID);
				if (LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(staticData.TokenID) > 0 || (staticData2.Replacement > 0 && LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(staticData2.Replacement) > 0))
				{
					CustomPopupEventArgs p_eventArgs = new CustomPopupEventArgs(Localization.Instance.GetText(staticData.Caption), Localization.Instance.GetText("GAMEMESSAGE_NOTHING_HAPPENS"));
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.CUSTOM_POPUP, p_eventArgs);
					FinishExecution();
				}
				else if (LegacyLogic.Instance.GameTime.Time.Days % 7 == (Int32)staticData.WeekDay)
				{
					m_parent.State = EInteractiveObjectState.ON;
					LegacyLogic.Instance.EventManager.InvokeEvent(m_parent, EEventType.OBJECT_STATE_CHANGED, EventArgs.Empty);
					LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddToken(staticData.TokenID);
					CustomPopupEventArgs p_eventArgs = new CustomPopupEventArgs(Localization.Instance.GetText(staticData.Caption), Localization.Instance.GetText(staticData.RightText));
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.CUSTOM_POPUP, p_eventArgs);
					FinishExecution();
				}
				else
				{
					m_stateMachine.ChangeState(3);
				}
			}
			else
			{
				LegacyLogger.Log("Shrine with ID " + m_shrineID + " doesn't exist");
				FinishExecution();
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
