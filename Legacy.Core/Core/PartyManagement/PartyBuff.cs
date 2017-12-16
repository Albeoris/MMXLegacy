using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Internationalization;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.PartyManagement
{
	public class PartyBuff : BuffBase, ISaveGameObject
	{
		private PartyBuffStaticData m_staticData;

		private MMTime m_expireTime;

		private Int32 m_expireTimeTurns;

		private Single m_valueFactor;

		private Boolean m_durationIsMinutes;

		private String m_description;

		private Int32 m_bufferLeft;

		private Int32 m_bufferReduce;

		private Boolean m_infinite;

		private Boolean m_usedThisTurn;

		public EPartyBuffs Type => (EPartyBuffs)m_staticData.StaticID;

	    public PartyBuffStaticData StaticData => m_staticData;

	    public MMTime ExpireTime => m_expireTime;

	    public Int32 ExpireTimeTurns => m_expireTimeTurns;

	    public Single ValueFactor => m_valueFactor;

	    public Boolean DurationIsMinutes => m_durationIsMinutes;

	    public String Description => m_description;

	    public Int32 BufferLeft
		{
			get => m_bufferLeft;
	        set => m_bufferLeft = value;
	    }

		public Boolean Infinite => m_infinite;

	    public void Init(EPartyBuffs p_buffId, Single p_valueFactor)
		{
			InitStaticData(p_buffId);
			m_valueFactor = p_valueFactor;
			m_durationIsMinutes = m_staticData.DurationIsMinutes;
			m_infinite = false;
			Single num = (!m_staticData.DurationScales) ? 1f : p_valueFactor;
			if (m_staticData.DurationIsMinutes)
			{
				m_expireTime = LegacyLogic.Instance.GameTime.Time;
				m_expireTime.AddMinutes((Int32)(m_staticData.Duration * num + 0.5f));
				m_expireTimeTurns = -2;
			}
			else if (m_staticData.Duration > 0)
			{
				m_expireTimeTurns = (Int32)(m_staticData.Duration * num + 0.5f);
			}
			else
			{
				m_infinite = true;
				m_expireTimeTurns = -1;
			}
			m_bufferLeft = 0;
			if (p_buffId == EPartyBuffs.CELESTIAL_ARMOR)
			{
				m_bufferLeft = (Int32)(m_staticData.SpecificValue[0] * p_valueFactor + 0.5f);
			}
			SetDescription();
		}

		public void ExpireNow()
		{
			if (m_durationIsMinutes)
			{
				m_expireTime = LegacyLogic.Instance.GameTime.Time;
			}
			else
			{
				m_infinite = false;
				m_expireTimeTurns = 0;
			}
		}

		public void UpdateExpiration()
		{
			if (!m_infinite)
			{
				m_expireTimeTurns--;
				m_usedThisTurn = false;
			}
		}

		public Boolean IsExpired()
		{
			if (m_infinite)
			{
				return false;
			}
			if (m_durationIsMinutes)
			{
				return LegacyLogic.Instance.GameTime.Time >= m_expireTime;
			}
			return m_expireTimeTurns <= 0;
		}

		private void InitStaticData(EPartyBuffs p_buffId)
		{
			m_staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)p_buffId);
		}

		public Int32 GetHealthPoints(Character p_char)
		{
			if (m_staticData.BuffId == EPartyBuffs.REGENERATION)
			{
				Int32 num = Random.Range((Int32)m_staticData.SpecificValue[0], (Int32)m_staticData.SpecificValue[1] + 1);
				return (Int32)(m_valueFactor * num + 0.5f);
			}
			if (m_staticData.BuffId == EPartyBuffs.NURTURE && !m_usedThisTurn)
			{
				Single num2 = p_char.HealthPoints / (Single)p_char.MaximumHealthPoints;
				Boolean flag = true;
				for (Int32 i = 0; i < 4; i++)
				{
					Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(i);
					if (!member.ConditionHandler.HasCondition(ECondition.DEAD) && member != p_char && member.HealthPoints / (Single)member.MaximumHealthPoints < num2)
					{
						flag = false;
					}
				}
				if (flag)
				{
					m_usedThisTurn = true;
					Int32 num3 = Random.Range((Int32)m_staticData.SpecificValue[0], (Int32)m_staticData.SpecificValue[1] + 1);
					return (Int32)(m_valueFactor * num3 + 0.5f);
				}
			}
			return 0;
		}

		public virtual void ModifyAttack(Attack attack)
		{
			if (Type == EPartyBuffs.LIQUID_MEMBRANE)
			{
				for (Int32 i = 0; i < attack.Damages.Count; i++)
				{
					Damage value = attack.Damages[i];
					value.Value -= (Int32)(value.Value * (ValueFactor * StaticData.SpecificValue[0]) + 0.5f);
					if (value.Value < 0)
					{
						value.Value = 0;
					}
					attack.Damages[i] = value;
				}
			}
			else if (Type == EPartyBuffs.CELESTIAL_ARMOR)
			{
				m_bufferReduce = 0;
				Int32 num = BufferLeft;
				for (Int32 j = 0; j < attack.Damages.Count; j++)
				{
					Damage value2 = attack.Damages[j];
					Int32 num2 = Math.Min(value2.Value, num);
					m_bufferReduce += num2;
					num -= num2;
					value2.Value -= num2;
					if (value2.Value < 0)
					{
						value2.Value = 0;
					}
					DamagePreventedEntryEventArgs p_args = new DamagePreventedEntryEventArgs(this, num2);
					LegacyLogic.Instance.WorldManager.Party.Buffs.AddExternalLogEntry(p_args);
					attack.Damages[j] = value2;
					if (num <= 0)
					{
						break;
					}
				}
			}
		}

		public void AfterAttackResult(AttackResult p_attackResult)
		{
			if ((p_attackResult.Result == EResultType.HIT || p_attackResult.Result == EResultType.CRITICAL_HIT) && Type == EPartyBuffs.CELESTIAL_ARMOR)
			{
				BufferLeft -= m_bufferReduce;
				if (BufferLeft <= 0)
				{
					ExpireNow();
				}
			}
		}

		public void AddAttributes(ref Attributes p_attributes)
		{
			if (!m_staticData.ValuesPercental)
			{
				p_attributes.Might += GetCalculatedValue(m_staticData.Might);
				p_attributes.Magic += GetCalculatedValue(m_staticData.Magic);
				p_attributes.Destiny += GetCalculatedValue(m_staticData.Destiny);
				p_attributes.Perception += GetCalculatedValue(m_staticData.Perception);
			}
		}

		public void AddFightValues(FightValues p_fightValues)
		{
			if (!m_staticData.ValuesPercental)
			{
				p_fightValues.ArmorValue += GetCalculatedValue(m_staticData.Armor);
				p_fightValues.Resistance.Add(EDamageType.FIRE, GetCalculatedValue(m_staticData.ResistanceFire));
				p_fightValues.Resistance.Add(EDamageType.WATER, GetCalculatedValue(m_staticData.ResistanceWater));
				p_fightValues.Resistance.Add(EDamageType.AIR, GetCalculatedValue(m_staticData.ResistanceAir));
				p_fightValues.Resistance.Add(EDamageType.EARTH, GetCalculatedValue(m_staticData.ResistanceEarth));
				p_fightValues.Resistance.Add(EDamageType.LIGHT, GetCalculatedValue(m_staticData.ResistanceLight));
				p_fightValues.Resistance.Add(EDamageType.DARK, GetCalculatedValue(m_staticData.ResistanceDarkness));
				p_fightValues.Resistance.Add(EDamageType.PRIMORDIAL, GetCalculatedValue(m_staticData.ResistancePrime));
				p_fightValues.MainHandAttackValue += GetCalculatedValue(m_staticData.MeleeAttack);
				p_fightValues.OffHandAttackValue += GetCalculatedValue(m_staticData.MeleeAttack);
				p_fightValues.RangedAttackValue += GetCalculatedValue(m_staticData.RangedAttackValue);
				if (m_staticData.StaticID == 15)
				{
					p_fightValues.EvadeValueRangedBonus += GetCalculatedValue(m_staticData.SpecificValue[0]);
				}
				if (m_staticData.StaticID == 36)
				{
					Int32 num = (Int32)(m_staticData.SpecificValue[0] * m_valueFactor + 0.5f);
					Int32 num2 = (Int32)(m_staticData.SpecificValue[1] * m_valueFactor + 0.5f);
					if (p_fightValues.MainHandDamage.GetTotalMaximumDamage() > 0)
					{
						DamageData damageData = p_fightValues.MainHandDamage[EDamageType.LIGHT];
						DamageData value = new DamageData(EDamageType.LIGHT, damageData.Minimum + num, damageData.Maximum + num2);
						p_fightValues.MainHandDamage[EDamageType.LIGHT] = value;
					}
					if (p_fightValues.OffHandDamage.GetTotalMaximumDamage() > 0)
					{
						DamageData damageData2 = p_fightValues.OffHandDamage[EDamageType.LIGHT];
						DamageData value2 = new DamageData(EDamageType.LIGHT, damageData2.Minimum + num, damageData2.Maximum + num2);
						p_fightValues.OffHandDamage[EDamageType.LIGHT] = value2;
					}
					if (p_fightValues.RangeDamage.GetTotalMaximumDamage() > 0)
					{
						DamageData damageData3 = p_fightValues.RangeDamage[EDamageType.LIGHT];
						DamageData value3 = new DamageData(EDamageType.LIGHT, damageData3.Minimum + num, damageData3.Maximum + num2);
						p_fightValues.RangeDamage[EDamageType.LIGHT] = value3;
					}
				}
			}
		}

		public void ModifyAttributes(ref Attributes p_attributes)
		{
			if (m_staticData.ValuesPercental)
			{
				p_attributes.Might = (Int32)Math.Round(p_attributes.Might * (1f + m_staticData.Might), MidpointRounding.AwayFromZero);
				p_attributes.Magic = (Int32)Math.Round(p_attributes.Magic * (1f + m_staticData.Magic), MidpointRounding.AwayFromZero);
				p_attributes.Destiny = (Int32)Math.Round(p_attributes.Destiny * (1f + m_staticData.Destiny), MidpointRounding.AwayFromZero);
				p_attributes.Perception = (Int32)Math.Round(p_attributes.Perception * (1f + m_staticData.Perception), MidpointRounding.AwayFromZero);
			}
		}

		public void ModifyFightValues(FightValues p_fightValues)
		{
			if (m_staticData.ValuesPercental)
			{
				p_fightValues.ArmorValue = (Int32)Math.Round(p_fightValues.ArmorValue * (1f + m_staticData.Armor), MidpointRounding.AwayFromZero);
				p_fightValues.Resistance.Modify(EDamageType.FIRE, 1f + m_staticData.ResistanceFire);
				p_fightValues.Resistance.Modify(EDamageType.WATER, 1f + m_staticData.ResistanceWater);
				p_fightValues.Resistance.Modify(EDamageType.AIR, 1f + m_staticData.ResistanceAir);
				p_fightValues.Resistance.Modify(EDamageType.EARTH, 1f + m_staticData.ResistanceEarth);
				p_fightValues.Resistance.Modify(EDamageType.LIGHT, 1f + m_staticData.ResistanceLight);
				p_fightValues.Resistance.Modify(EDamageType.DARK, 1f + m_staticData.ResistanceDarkness);
				p_fightValues.Resistance.Modify(EDamageType.PRIMORDIAL, 1f + m_staticData.ResistancePrime);
				p_fightValues.MainHandAttackValue *= 1f + m_staticData.MeleeAttack;
				p_fightValues.OffHandAttackValue *= 1f + m_staticData.MeleeAttack;
				p_fightValues.RangedAttackValue *= 1f + m_staticData.RangedAttackValue;
			}
		}

		public Int32 GetCalculatedValue(Single p_baseValue)
		{
			return (Int32)Math.Round(p_baseValue * m_valueFactor, MidpointRounding.AwayFromZero);
		}

		public Int32 GetRangedEvadeBonus()
		{
			if (m_staticData.StaticID == 15)
			{
				return GetCalculatedValue(m_staticData.SpecificValue[0]);
			}
			return 0;
		}

		private void SetDescription()
		{
			if (m_staticData.StaticID == 1 || m_staticData.StaticID == 27)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.ResistanceFire));
			}
			else if (m_staticData.StaticID == 2 || m_staticData.StaticID == 28)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.ResistanceWater));
			}
			else if (m_staticData.StaticID == 3 || m_staticData.StaticID == 29)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.ResistanceAir));
			}
			else if (m_staticData.StaticID == 4 || m_staticData.StaticID == 30)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.ResistanceEarth));
			}
			else if (m_staticData.StaticID == 5 || m_staticData.StaticID == 31)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.ResistanceLight));
			}
			else if (m_staticData.StaticID == 6 || m_staticData.StaticID == 32)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.ResistanceDarkness));
			}
			else if (m_staticData.StaticID == 7 || m_staticData.StaticID == 33)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.ResistanceDarkness));
			}
			else if (m_staticData.StaticID == 11)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.Might));
			}
			else if (m_staticData.StaticID == 12 || m_staticData.StaticID == 34)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.Destiny));
			}
			else if (m_staticData.StaticID == 13)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.Perception));
			}
			else if (m_staticData.StaticID == 14)
			{
				Int32 calculatedValue = GetCalculatedValue(m_staticData.SpecificValue[0]);
				Int32 calculatedValue2 = GetCalculatedValue(m_staticData.SpecificValue[1]);
				String arg;
				if (calculatedValue == calculatedValue2)
				{
					arg = calculatedValue.ToString();
				}
				else
				{
					arg = calculatedValue + " - " + calculatedValue2;
				}
				m_description = Localization.Instance.GetText(m_staticData.Description, arg);
			}
			else if (m_staticData.StaticID == 15)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.SpecificValue[0]));
			}
			else if (m_staticData.StaticID == 16)
			{
				Int32 calculatedValue3 = GetCalculatedValue(m_staticData.SpecificValue[0]);
				Int32 calculatedValue4 = GetCalculatedValue(m_staticData.SpecificValue[1]);
				String arg2;
				if (calculatedValue3 == calculatedValue4)
				{
					arg2 = calculatedValue3.ToString();
				}
				else
				{
					arg2 = calculatedValue3 + " - " + calculatedValue4;
				}
				m_description = Localization.Instance.GetText(m_staticData.Description, arg2);
			}
			else if (m_staticData.StaticID == 37)
			{
				Int32 calculatedValue5 = GetCalculatedValue(m_staticData.SpecificValue[0]);
				Int32 calculatedValue6 = GetCalculatedValue(m_staticData.SpecificValue[1]);
				String arg3;
				if (calculatedValue5 == calculatedValue6)
				{
					arg3 = calculatedValue5.ToString();
				}
				else
				{
					arg3 = calculatedValue5 + " - " + calculatedValue6;
				}
				m_description = Localization.Instance.GetText(m_staticData.Description, arg3);
			}
			else if (m_staticData.StaticID == 36)
			{
				Int32 calculatedValue7 = GetCalculatedValue(m_staticData.SpecificValue[0]);
				Int32 calculatedValue8 = GetCalculatedValue(m_staticData.SpecificValue[1]);
				String arg4;
				if (calculatedValue7 == calculatedValue8)
				{
					arg4 = calculatedValue7.ToString();
				}
				else
				{
					arg4 = calculatedValue7 + " - " + calculatedValue8;
				}
				m_description = Localization.Instance.GetText(m_staticData.Description, arg4);
			}
			else if (m_staticData.StaticID == 17)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.Armor));
			}
			else if (m_staticData.StaticID == 19)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.SpecificValue[0]));
			}
			else if (m_staticData.StaticID == 20 || m_staticData.StaticID == 35)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.MeleeAttack), GetCalculatedValue(m_staticData.Might));
			}
			else if (m_staticData.StaticID == 21)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, (Int32)(m_staticData.Might * 100f + 0.5f));
			}
			else if (m_staticData.StaticID == 25)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.Duration));
			}
			else if (m_staticData.StaticID == 26)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, (Int32)(m_staticData.SpecificValue[0] * m_valueFactor * 100f + 0.5f));
			}
			else if (m_staticData.StaticID == 9 || m_staticData.StaticID == 10 || m_staticData.StaticID == 8)
			{
				m_description = Localization.Instance.GetText(m_staticData.Description, GetCalculatedValue(m_staticData.Duration));
			}
			else
			{
				m_description = Localization.Instance.GetText(m_staticData.Description);
			}
		}

		public void ResetDuration(Single p_valueFactor)
		{
			m_valueFactor = p_valueFactor;
			Single num = (!m_staticData.DurationScales) ? 1f : m_valueFactor;
			if (m_staticData.DurationIsMinutes)
			{
				m_expireTime = LegacyLogic.Instance.GameTime.Time;
				m_expireTime.AddMinutes((Int32)(m_staticData.Duration * num));
				m_expireTimeTurns = -2;
			}
			else
			{
				m_expireTimeTurns = (Int32)(m_staticData.Duration * num + 0.5f);
			}
			if (m_staticData.BuffId == EPartyBuffs.CELESTIAL_ARMOR)
			{
				m_bufferLeft = (Int32)(m_staticData.SpecificValue[0] * p_valueFactor + 0.5f);
			}
			SetDescription();
		}

		public void Load(SaveGameData p_data)
		{
			EPartyBuffs p_buffId = (EPartyBuffs)p_data.Get<Int32>("BuffID", 1);
			InitStaticData(p_buffId);
			m_durationIsMinutes = m_staticData.DurationIsMinutes;
			m_valueFactor = p_data.Get<Single>("ValueFactor", 1f);
			m_bufferLeft = p_data.Get<Int32>("BufferLeft", 0);
			SaveGameData saveGameData = p_data.Get<SaveGameData>("ExpireTime", null);
			if (saveGameData != null)
			{
				m_expireTime.Load(saveGameData);
			}
			m_expireTimeTurns = p_data.Get<Int32>("ExpireTurns", 0);
			if (!m_durationIsMinutes)
			{
				if (m_expireTimeTurns == -1)
				{
					m_infinite = true;
				}
				else
				{
					m_expireTimeTurns++;
				}
			}
			else
			{
				m_expireTimeTurns = -2;
			}
			SetDescription();
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("BuffID", StaticData.StaticID);
			p_data.Set<Single>("ValueFactor", m_valueFactor);
			p_data.Set<Int32>("BufferLeft", m_bufferLeft);
			SaveGameData saveGameData = new SaveGameData("ExpireTime");
			m_expireTime.Save(saveGameData);
			p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			p_data.Set<Int32>("ExpireTurns", m_expireTimeTurns);
		}
	}
}
