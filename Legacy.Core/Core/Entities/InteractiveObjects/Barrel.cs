using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Barrel : InteractiveObject
	{
		private EPotionTarget m_targetAttribute;

		private Int32 m_value;

		public Barrel() : this(0, 0)
		{
		}

		public Barrel(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.BARREL, p_spawnerID)
		{
			State = EInteractiveObjectState.BARREL_CLOSED;
		}

		public override void SetData(EInteractiveObjectData p_key, String p_value)
		{
			if (p_key == EInteractiveObjectData.BARREL_DATA)
			{
				Int32 p_staticId = Convert.ToInt32(p_value);
				BarrelStaticData staticData = StaticDataHandler.GetStaticData<BarrelStaticData>(EDataType.BARRELS, p_staticId);
				Prefab = staticData.Prefab;
				m_targetAttribute = staticData.TargetAttribute;
				m_value = staticData.Value;
			}
		}

		public void Open()
		{
			if (State == EInteractiveObjectState.BARREL_CLOSED)
			{
				State = EInteractiveObjectState.BARREL_OPEN;
				BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(this, Position);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.BARREL_STATE_CHANGE, p_eventArgs);
			}
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			m_targetAttribute = p_data.Get<EPotionTarget>("TargetAttribute", EPotionTarget.MIGHT);
			m_value = p_data.Get<Int32>("Value", 1);
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<EPotionTarget>("TargetAttribute", m_targetAttribute);
			p_data.Set<Int32>("Value", m_value);
		}

		public void GiveBonus(Character p_targetCharacter)
		{
			if (State == EInteractiveObjectState.BARREL_EMPTY)
			{
				return;
			}
			String text = String.Empty;
			switch (m_targetAttribute)
			{
			case EPotionTarget.MIGHT:
			{
				Attributes baseAttributes = p_targetCharacter.BaseAttributes;
				baseAttributes.Might += m_value;
				p_targetCharacter.BaseAttributes = baseAttributes;
				text = "BARREL_DARK_RED_LIQUID_EFFECT_TEXT";
				break;
			}
			case EPotionTarget.MAGIC:
			{
				Attributes baseAttributes = p_targetCharacter.BaseAttributes;
				baseAttributes.Magic += m_value;
				p_targetCharacter.BaseAttributes = baseAttributes;
				text = "BARREL_DARK_BLUE_LIQUID_EFFECT_TEXT";
				break;
			}
			case EPotionTarget.PERCEPTION:
			{
				Attributes baseAttributes = p_targetCharacter.BaseAttributes;
				baseAttributes.Perception += m_value;
				p_targetCharacter.BaseAttributes = baseAttributes;
				text = "BARREL_WHITE_LIQUID_EFFECT_TEXT";
				break;
			}
			case EPotionTarget.DESTINY:
			{
				Attributes baseAttributes = p_targetCharacter.BaseAttributes;
				baseAttributes.Destiny += m_value;
				p_targetCharacter.BaseAttributes = baseAttributes;
				text = "BARREL_PURPLE_LIQUID_EFFECT_TEXT";
				break;
			}
			case EPotionTarget.FIRE_RESISTANCE:
				p_targetCharacter.BaseResistance.Add(EDamageType.FIRE, m_value);
				text = "BARREL_PULSING_RED_LIQUID_EFFECT_TEXT";
				break;
			case EPotionTarget.WATER_RESISTANCE:
				p_targetCharacter.BaseResistance.Add(EDamageType.WATER, m_value);
				text = "BARREL_PULSING_BLUE_LIQUID_EFFECT_TEXT";
				break;
			case EPotionTarget.AIR_RESISTANCE:
				p_targetCharacter.BaseResistance.Add(EDamageType.AIR, m_value);
				text = "BARREL_PULSING_YELLOW_LIQUID_EFFECT_TEXT";
				break;
			case EPotionTarget.EARTH_RESISTANCE:
				p_targetCharacter.BaseResistance.Add(EDamageType.EARTH, m_value);
				text = "BARREL_PULSING_GREEN_LIQUID_EFFECT_TEXT";
				break;
			case EPotionTarget.LIGHT_RESISTANCE:
				p_targetCharacter.BaseResistance.Add(EDamageType.LIGHT, m_value);
				text = "BARREL_PULSING_WHITE_LIQUID_EFFECT_TEXT";
				break;
			case EPotionTarget.DARK_RESISTANCE:
				p_targetCharacter.BaseResistance.Add(EDamageType.DARK, m_value);
				text = "BARREL_PULSING_BLACK_LIQUID_EFFECT_TEXT";
				break;
			case EPotionTarget.PRIME_RESISTANCE:
				p_targetCharacter.BaseResistance.Add(EDamageType.PRIMORDIAL, m_value);
				text = "BARREL_PULSING_PURPLE_LIQUID_EFFECT_TEXT";
				break;
			}
			p_targetCharacter.CalculateCurrentAttributes();
			State = EInteractiveObjectState.BARREL_EMPTY;
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(this, Position);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.BARREL_STATE_CHANGE, p_eventArgs);
			if (p_targetCharacter.Gender == EGender.MALE)
			{
				text += "_M";
			}
			else
			{
				text += "_F";
			}
			String text2 = Localization.Instance.GetText(text, "[00FF00]" + p_targetCharacter.Name + "[-]", m_value);
			GameMessageEventArgs p_eventArgs2 = new GameMessageEventArgs(text2, 0f, false);
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.BARREL_BONUS, p_eventArgs2);
			MessageEventArgs p_args = new MessageEventArgs(text2, true);
			LegacyLogic.Instance.ActionLog.PushEntry(p_args);
		}
	}
}
