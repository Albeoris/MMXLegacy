using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class RechargingObject : InteractiveObject
	{
		private EPartyBuffs m_partyBuff;

		private ERechargerSpecial m_special;

		private MMTime m_lastActivationTime;

		private ERechargerType m_rechargerType;

		public RechargingObject() : this(0, 0)
		{
		}

		public RechargingObject(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.RECHARGING_OBJECT, p_spawnerID)
		{
			State = EInteractiveObjectState.ON;
			m_lastActivationTime = new MMTime(0, 0, 0);
		}

		public override void SetData(EInteractiveObjectData p_key, String p_value)
		{
			if (p_key == EInteractiveObjectData.RECHARGING_OBJECT_DATA)
			{
				Int32 p_staticId;
				Int32.TryParse(p_value, out p_staticId);
				RechargingObjectStaticData staticData = StaticDataHandler.GetStaticData<RechargingObjectStaticData>(EDataType.RECHARGING_OBJECTS, p_staticId);
				if (staticData != null)
				{
					Prefab = staticData.Prefab;
					m_partyBuff = staticData.PartyBuff;
					m_special = staticData.Special;
					m_rechargerType = staticData.RechargerType;
				}
			}
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			m_partyBuff = p_data.Get<EPartyBuffs>("PartyBuff", EPartyBuffs.NONE);
			m_special = p_data.Get<ERechargerSpecial>("Special", ERechargerSpecial.NONE);
			m_rechargerType = p_data.Get<ERechargerType>("RechargerType", ERechargerType.FOUNTAIN);
			SaveGameData saveGameData = p_data.Get<SaveGameData>("LastActivation", null);
			if (saveGameData != null)
			{
				m_lastActivationTime.Load(saveGameData);
			}
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<EPartyBuffs>("PartyBuff", m_partyBuff);
			p_data.Set<ERechargerSpecial>("Special", m_special);
			p_data.Set<ERechargerType>("RechargerType", m_rechargerType);
			SaveGameData saveGameData = new SaveGameData("LastActivation");
			m_lastActivationTime.Save(saveGameData);
			p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
		}

		public void OnCheckRecharge()
		{
			GameTime gameTime = LegacyLogic.Instance.GameTime;
			MMTime p_t = new MMTime(0, 8, gameTime.Time.Days);
			if (gameTime.Time >= p_t && m_lastActivationTime < p_t)
			{
				State = EInteractiveObjectState.ON;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.OBJECT_STATE_CHANGED, EventArgs.Empty);
			}
		}

		public override void OnLevelLoaded(Grid p_grid)
		{
			OnCheckRecharge();
			base.OnLevelLoaded(p_grid);
		}

		public void Interact()
		{
			if (State == EInteractiveObjectState.ON)
			{
				String text = String.Empty;
				State = EInteractiveObjectState.OFF;
				m_lastActivationTime = LegacyLogic.Instance.GameTime.Time;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.OBJECT_STATE_CHANGED, EventArgs.Empty);
				if (m_partyBuff != EPartyBuffs.NONE)
				{
					LegacyLogic.Instance.WorldManager.Party.Buffs.AddBuff(m_partyBuff, 1f);
					text = Localization.Instance.GetText("GAMEMESSAGE_BLESSED");
					GameMessageEventArgs p_eventArgs = new GameMessageEventArgs(text, 0f, false);
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.GAME_MESSAGE, p_eventArgs);
					MessageEventArgs p_args = new MessageEventArgs(text, true);
					LegacyLogic.Instance.ActionLog.PushEntry(p_args);
				}
				else if (m_special == ERechargerSpecial.RESTORE)
				{
					foreach (Character character in LegacyLogic.Instance.WorldManager.Party.GetCharactersAlive())
					{
						if (!character.ConditionHandler.HasCondition(ECondition.DEAD) && (character.HealthPoints != character.MaximumHealthPoints || character.ManaPoints != character.MaximumManaPoints))
						{
							character.ChangeHP(character.MaximumHealthPoints + Math.Abs(character.HealthPoints));
							character.ChangeMP(character.MaximumManaPoints + Math.Abs(character.ManaPoints));
						}
					}
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PARTY_RESTORED, EventArgs.Empty);
					text = Localization.Instance.GetText("GAME_MESSAGE_PARTY_RESTORED");
					MessageEventArgs p_args2 = new MessageEventArgs(text, true);
					LegacyLogic.Instance.ActionLog.PushEntry(p_args2);
				}
				else if (m_special == ERechargerSpecial.CURE)
				{
					for (Int32 i = 0; i < 4; i++)
					{
						Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(i);
						if (member.ConditionHandler.HasCondition(ECondition.DEAD))
						{
							member.ConditionHandler.RemoveCondition(ECondition.DEAD);
							member.Resurrect();
							LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_REVIVED, EventArgs.Empty);
						}
						if (member.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS))
						{
							member.ConditionHandler.RemoveCondition(ECondition.UNCONSCIOUS);
							member.Resurrect();
						}
						member.ConditionHandler.RemoveCondition(ECondition.PARALYZED);
						member.ConditionHandler.RemoveCondition(ECondition.STUNNED);
						member.ConditionHandler.RemoveCondition(ECondition.SLEEPING);
						member.ConditionHandler.RemoveCondition(ECondition.POISONED);
						member.ConditionHandler.RemoveCondition(ECondition.CONFUSED);
						member.ConditionHandler.RemoveCondition(ECondition.WEAK);
						member.ConditionHandler.RemoveCondition(ECondition.CURSED);
					}
					LegacyLogic.Instance.WorldManager.Party.Buffs.RemoveAllBuffs();
					text = Localization.Instance.GetText("GAMEMESSAGE_BLESSED");
					GameMessageEventArgs p_eventArgs2 = new GameMessageEventArgs(text, 0f, false);
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.GAME_MESSAGE, p_eventArgs2);
					MessageEventArgs p_args3 = new MessageEventArgs(text, true);
					LegacyLogic.Instance.ActionLog.PushEntry(p_args3);
				}
			}
			else
			{
				GameMessageEventArgs p_eventArgs3 = new GameMessageEventArgs("GAMEMESSAGE_NOTHING_HAPPENS", 0f, true);
				LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.GAME_MESSAGE, p_eventArgs3);
			}
		}

		public Boolean IsFountain()
		{
			return m_rechargerType == ERechargerType.FOUNTAIN;
		}

		public Boolean IsCrystal()
		{
			return m_rechargerType == ERechargerType.CRYSTAL;
		}
	}
}
