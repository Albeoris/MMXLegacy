using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells
{
	public class SummonSpell : CharacterSpell
	{
		private Summon m_testEntity;

		public SummonSpell(ECharacterSpell type) : base(type)
		{
			m_testEntity = (Summon)EntityFactory.Create(EObjectType.SUMMON, m_staticData.SummonID, 0);
		}

		public override Boolean CheckSpellConditions(Character p_sorcerer)
		{
			if (SpellType == ECharacterSpell.WARFARE_CRIPPLING_TRAP)
			{
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				GridSlot slot = grid.GetSlot(LegacyLogic.Instance.WorldManager.Party.Position);
				foreach (MovingEntity movingEntity in slot.Entities)
				{
					Summon summon = movingEntity as Summon;
					if (summon != null && summon.StaticID == 6)
					{
						return false;
					}
				}
				return true;
			}
			if (SpellType == ECharacterSpell.SPELL_PRIME_TIME_STOP || SpellType == ECharacterSpell.WARFARE_CRIPPLING_TRAP)
			{
				return true;
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Grid grid2 = LegacyLogic.Instance.MapLoader.Grid;
			GridSlot slot2 = grid2.GetSlot(party.Position + party.Direction);
			GridSlot slot3 = grid2.GetSlot(LegacyLogic.Instance.WorldManager.Party.Position);
			return slot2 != null && slot2.IsPassable(m_testEntity, false, true) && slot3.GetTransition(party.Direction).NextState != EGridTransitionType.CLOSED;
		}

		public override Boolean CastSpell(Character p_sorcerer, Boolean p_fromScroll, Int32 p_scrollTier, List<Object> p_targets)
		{
			if (HasResources(p_sorcerer))
			{
				if (SpellType == ECharacterSpell.WARFARE_CRIPPLING_TRAP)
				{
					for (Int32 i = LegacyLogic.Instance.UpdateManager.SummonsActorAfterMonsterTurn.Entities.Count - 1; i >= 0; i--)
					{
						Summon summon = LegacyLogic.Instance.UpdateManager.SummonsActorAfterMonsterTurn.Entities[i] as Summon;
						if (summon != null && summon.CasterId == p_sorcerer.Index)
						{
							summon.Destroy();
						}
					}
				}
				Single magicFactor = GetMagicFactor(p_sorcerer, p_fromScroll, p_scrollTier);
				UseResources(p_sorcerer);
				Party party = LegacyLogic.Instance.WorldManager.Party;
				Summon summon2 = (Summon)EntityFactory.Create(EObjectType.SUMMON, m_staticData.SummonID, 0);
				summon2.CasterId = p_sorcerer.Index;
				if (SpellType == ECharacterSpell.SPELL_FIRE_SEARING_RUNE)
				{
					SummonStaticData staticData = StaticDataHandler.GetStaticData<SummonStaticData>(EDataType.SUMMONS, 5);
					Int32 j = staticData.Range;
					Grid grid = LegacyLogic.Instance.MapLoader.Grid;
					GridSlot slot = grid.GetSlot(LegacyLogic.Instance.WorldManager.Party.Position);
					GridSlot gridSlot = null;
					while (j > 0)
					{
						gridSlot = grid.GetFreeSlotInDirection(slot, party.Direction, j);
						if (gridSlot != null)
						{
							break;
						}
						j--;
					}
					if (gridSlot == null)
					{
						gridSlot = slot;
					}
					summon2.Position = gridSlot.Position;
				}
				else if (SpellType == ECharacterSpell.SPELL_PRIME_TIME_STOP || SpellType == ECharacterSpell.WARFARE_CRIPPLING_TRAP)
				{
					summon2.Position = party.Position;
				}
				else
				{
					summon2.Position = party.Position + party.Direction;
				}
				summon2.SetMagicFactor(magicFactor);
				summon2.SetDamageType(ESkillIDToEDamageType(m_staticData.SkillID));
				summon2.SetIgnoreResistance(p_sorcerer.SkillHandler.GetResistanceIgnoreValue(m_staticData.SkillID));
				summon2.Direction = party.Direction;
				LegacyLogic.Instance.MapLoader.Grid.AddMovingEntity(summon2.Position, summon2);
				LegacyLogic.Instance.WorldManager.SpawnObject(summon2, summon2.Position);
				SpellEventArgs eventArgs = GetEventArgs();
				eventArgs.Result = ESpellResult.OK;
				LegacyLogic.Instance.ActionLog.PushEntry(new CastSpellEntryEventArgs(p_sorcerer, eventArgs));
				return true;
			}
			return false;
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			Int32 staticID = m_staticData.StaticID;
			switch (staticID)
			{
			case 79:
			{
				SummonStaticData staticData = StaticDataHandler.GetStaticData<SummonStaticData>(EDataType.SUMMONS, 4);
				SetDescriptionValue(0, staticData.AILifetime);
				break;
			}
			default:
				if (staticID != 29)
				{
					if (staticID != 30)
					{
						if (staticID == 18)
						{
							SummonStaticData staticData2 = StaticDataHandler.GetStaticData<SummonStaticData>(EDataType.SUMMONS, 3);
							MonsterBuffStaticData staticData3 = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, 6);
							SetDescriptionValue(0, staticData2.AILifetime);
							SetDescriptionValue(1, GetDamageAsStringFromSummonData(staticData2, 0, p_magicFactor));
							SetDescriptionValue(2, (Int32)(staticData3.GetBuffValues(1)[1] * p_magicFactor + 0.5f));
							SetDescriptionValue(3, (Int32)(staticData3.GetBuffValues(1)[0] * p_magicFactor + 0.5f));
						}
					}
					else
					{
						SummonStaticData staticData4 = StaticDataHandler.GetStaticData<SummonStaticData>(EDataType.SUMMONS, 2);
						SetDescriptionValue(0, staticData4.AILifetime);
						SetDescriptionValue(1, GetDamageAsStringFromSummonData(staticData4, 0, p_magicFactor));
					}
				}
				else
				{
					SummonStaticData staticData5 = StaticDataHandler.GetStaticData<SummonStaticData>(EDataType.SUMMONS, 1);
					SetDescriptionValue(0, staticData5.AILifetime);
					SetDescriptionValue(1, GetDamageAsStringFromSummonData(staticData5, 0, p_magicFactor));
				}
				break;
			case 81:
			{
				SummonStaticData staticData6 = StaticDataHandler.GetStaticData<SummonStaticData>(EDataType.SUMMONS, 5);
				SetDescriptionValue(0, staticData6.Range);
				SetDescriptionValue(1, GetDamageAsStringFromSummonData(staticData6, 0, p_magicFactor));
				SetDescriptionValue(2, staticData6.AILifetime);
				break;
			}
			case 84:
			{
				SummonStaticData staticData7 = StaticDataHandler.GetStaticData<SummonStaticData>(EDataType.SUMMONS, 6);
				ParagonAbilitiesStaticData staticData8 = StaticDataHandler.GetStaticData<ParagonAbilitiesStaticData>(EDataType.PARAGON_ABILITES, 8);
				SetDescriptionValue(0, GetDamageAsStringFromSummonData(staticData7, 0, p_magicFactor));
				SetDescriptionValue(1, staticData8.Values[2]);
				break;
			}
			}
		}

		private String GetDamageAsStringFromSummonData(SummonStaticData p_summonData, Int32 p_damageId, Single p_magicFactor)
		{
			Int32 num = (Int32)(p_summonData.DamageData[p_damageId].Minimum * p_magicFactor + 0.5f);
			Int32 num2 = (Int32)(p_summonData.DamageData[p_damageId].Maximum * p_magicFactor + 0.5f);
			if (num == num2)
			{
				return num.ToString();
			}
			return num + " - " + num2;
		}
	}
}
