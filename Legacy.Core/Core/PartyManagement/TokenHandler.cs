using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.PartyManagement
{
	public class TokenHandler : ISaveGameObject
	{
		private Party m_myParty;

		private Dictionary<Int32, Int32> m_tokens;

		public TokenHandler(Party myParty)
		{
			if (myParty == null)
			{
				throw new ArgumentNullException("myParty");
			}
			m_myParty = myParty;
			m_tokens = new Dictionary<Int32, Int32>();
		}

		public List<Int32> CollectedTokens
		{
			get
			{
				List<Int32> list = new List<Int32>();
				foreach (KeyValuePair<Int32, Int32> keyValuePair in m_tokens)
				{
					if (keyValuePair.Value > 0)
					{
						list.Add(keyValuePair.Key);
					}
				}
				list.Sort();
				list.Reverse();
				return list;
			}
		}

		public void AddToken(Int32 p_id)
		{
			TokenStaticData tokenData = GetTokenData(p_id);
			if (tokenData == null)
			{
				return;
			}
			Int32 num;
			m_tokens.TryGetValue(p_id, out num);
			m_tokens[p_id] = num + 1;
			if (tokenData.TokenVisible)
			{
				TokenAcquiredEventArgs p_args = new TokenAcquiredEventArgs(tokenData);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			if (p_id >= 11 && p_id <= 22)
			{
				m_myParty.UnlockAdvancedClass((ETokenID)p_id);
			}
			if (p_id <= 6)
			{
				for (Int32 i = 0; i < 4; i++)
				{
					Character member = m_myParty.GetMember(i);
					if (member != null)
					{
						member.CalculateCurrentAttributes();
					}
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.TOKEN_ADDED, new TokenEventArgs(p_id));
			if (tokenData.SetID > 0 && tokenData.Replacement > 0)
			{
				CheckSet(tokenData);
			}
		}

		public void AddTokenIfNull(Int32 p_id)
		{
			Int32 num;
			m_tokens.TryGetValue(p_id, out num);
			if (num == 0)
			{
				AddToken(p_id);
			}
		}

		private void CheckSet(TokenStaticData p_newToken)
		{
			Boolean flag = true;
			List<Int32> list = new List<Int32>();
			foreach (TokenStaticData tokenStaticData in StaticDataHandler.GetIterator<TokenStaticData>(EDataType.TOKEN))
			{
				if (tokenStaticData.SetID == p_newToken.SetID)
				{
					if (GetTokens(tokenStaticData.StaticID) == 0)
					{
						flag = false;
					}
					else
					{
						list.Add(tokenStaticData.StaticID);
					}
				}
			}
			if (flag)
			{
				foreach (Int32 p_id in list)
				{
					RemoveToken(p_id);
				}
				AddToken(p_newToken.Replacement);
			}
		}

		public void RemoveToken(Int32 p_id)
		{
			Int32 num;
			m_tokens.TryGetValue(p_id, out num);
			if (num >= 1)
			{
				m_tokens[p_id] = num - 1;
			}
			TokenStaticData tokenData = GetTokenData(p_id);
			if (tokenData != null)
			{
				if (tokenData.TokenVisible && !tokenData.RemoveSilent)
				{
					TokenRemovedEventArgs p_args = new TokenRemovedEventArgs(tokenData);
					LegacyLogic.Instance.ActionLog.PushEntry(p_args);
				}
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.TOKEN_REMOVED, new TokenEventArgs(p_id));
			}
		}

		public Int32 GetTokens(Int32 p_id)
		{
			Int32 result;
			if (m_tokens.TryGetValue(p_id, out result))
			{
				return result;
			}
			return 0;
		}

		public void RemoveAllTokens(Int32 p_id)
		{
			if (m_tokens.ContainsKey(p_id))
			{
				m_tokens[p_id] = 0;
			}
		}

		public void RemoveSet(Int32 m_setID)
		{
			List<TokenStaticData> list = new List<TokenStaticData>(StaticDataHandler.GetIterator<TokenStaticData>(EDataType.TOKEN));
			for (Int32 i = 0; i < list.Count; i++)
			{
				if (list[i].SetID == m_setID)
				{
					RemoveToken(list[i].StaticID);
				}
			}
		}

		public ETokenID AdvancedClassTokenIdForClass(EClass p_class)
		{
			if (p_class == EClass.MERCENARY)
			{
				return ETokenID.TOKEN_CLASS_WINDSWORD;
			}
			if (p_class == EClass.CRUSADER)
			{
				return ETokenID.TOKEN_CLASS_PALADIN;
			}
			if (p_class == EClass.FREEMAGE)
			{
				return ETokenID.TOKEN_CLASS_ARCHMAGE;
			}
			if (p_class == EClass.BLADEDANCER)
			{
				return ETokenID.TOKEN_CLASS_BLADEMASTER;
			}
			if (p_class == EClass.RANGER)
			{
				return ETokenID.TOKEN_CLASS_WARDEN;
			}
			if (p_class == EClass.DRUID)
			{
				return ETokenID.TOKEN_CLASS_DRUID_ELDER;
			}
			if (p_class == EClass.DEFENDER)
			{
				return ETokenID.TOKEN_CLASS_SHIELD_GUARD;
			}
			if (p_class == EClass.SCOUT)
			{
				return ETokenID.TOKEN_CLASS_PATHFINDER;
			}
			if (p_class == EClass.RUNEPRIEST)
			{
				return ETokenID.TOKEN_CLASS_RUNELORD;
			}
			if (p_class == EClass.BARBARIAN)
			{
				return ETokenID.TOKEN_CLASS_WARMONGER;
			}
			if (p_class == EClass.HUNTER)
			{
				return ETokenID.TOKEN_CLASS_MARAUDER;
			}
			return ETokenID.TOKEN_CLASS_BLOODCALLER;
		}

		public void Load(SaveGameData p_data)
		{
			m_tokens.Clear();
			Int32 num = p_data.Get<Int32>("Count", 0);
			for (Int32 i = 0; i < num; i++)
			{
				Int32 num2 = p_data.Get<Int32>("TokenKey" + i, -1);
				Int32 num3 = p_data.Get<Int32>("TokenValue" + i, -1);
				if (num2 != -1 && num3 != -1)
				{
					m_tokens.Add(num2, num3);
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			Int32 num = 0;
			foreach (KeyValuePair<Int32, Int32> keyValuePair in m_tokens)
			{
				if (keyValuePair.Value > 0)
				{
					p_data.Set<Int32>("TokenKey" + num, keyValuePair.Key);
					p_data.Set<Int32>("TokenValue" + num, keyValuePair.Value);
					num++;
				}
			}
			p_data.Set<Int32>("Count", num);
		}

		public static TokenStaticData GetTokenData(Int32 p_id)
		{
			return StaticDataHandler.GetStaticData<TokenStaticData>(EDataType.TOKEN, p_id);
		}

		public String GetTokenDescription(Int32 p_id)
		{
			TokenStaticData staticData = StaticDataHandler.GetStaticData<TokenStaticData>(EDataType.TOKEN, p_id);
			if (p_id <= 6)
			{
				return Localization.Instance.GetText(staticData.Description, ConfigManager.Instance.Game.ResistancePerBlessing);
			}
			return Localization.Instance.GetText(staticData.Description);
		}

		public ResistanceCollection GetResistances()
		{
			ResistanceCollection resistanceCollection = new ResistanceCollection();
			if (GetTokens(1) > 0)
			{
				resistanceCollection.Add(new Resistance(EDamageType.DARK, ConfigManager.Instance.Game.ResistancePerBlessing));
			}
			if (GetTokens(2) > 0)
			{
				resistanceCollection.Add(new Resistance(EDamageType.LIGHT, ConfigManager.Instance.Game.ResistancePerBlessing));
			}
			if (GetTokens(3) > 0)
			{
				resistanceCollection.Add(new Resistance(EDamageType.FIRE, ConfigManager.Instance.Game.ResistancePerBlessing));
			}
			if (GetTokens(4) > 0)
			{
				resistanceCollection.Add(new Resistance(EDamageType.WATER, ConfigManager.Instance.Game.ResistancePerBlessing));
			}
			if (GetTokens(5) > 0)
			{
				resistanceCollection.Add(new Resistance(EDamageType.AIR, ConfigManager.Instance.Game.ResistancePerBlessing));
			}
			if (GetTokens(6) > 0)
			{
				resistanceCollection.Add(new Resistance(EDamageType.EARTH, ConfigManager.Instance.Game.ResistancePerBlessing));
			}
			return resistanceCollection;
		}
	}
}
