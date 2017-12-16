using System;
using Legacy.Core.Configuration;

namespace Legacy.Core.Combat
{
	public abstract class BaseCombatHandler
	{
		public AttackResult AttackEntity(Attack p_attack, Boolean p_isMelee, EDamageType p_damageType)
		{
			return AttackEntity(p_attack, p_isMelee, p_damageType, false, 0, false);
		}

		public abstract AttackResult AttackEntity(Attack p_attack, Boolean p_isMelee, EDamageType p_damageType, Boolean p_skipBlockEvadeTest, Int32 p_resistanceIgnore, Boolean p_skipBlock = false);

		public virtual Boolean TestEvade(Single p_attackValue, Single p_evadeValue, EDamageType p_damageType, Int32 p_ignoreResistance)
		{
			if (p_damageType == EDamageType.PHYSICAL)
			{
				return TestEvadePhysical(p_attackValue, p_evadeValue, p_damageType);
			}
			return TestEvadeSpell(p_damageType, p_ignoreResistance);
		}

		public virtual Boolean TestEvadePhysical(Single p_attackValue, Single p_evadeValue, EDamageType p_damageType)
		{
			Single evadeFactor = ConfigManager.Instance.Game.EvadeFactor;
			Single num;
			if (p_attackValue == 0f)
			{
				num = 0f;
			}
			else if (p_evadeValue <= 0f)
			{
				num = 1f;
			}
			else if (p_attackValue < p_evadeValue)
			{
				num = p_attackValue * evadeFactor / p_evadeValue;
			}
			else
			{
				num = evadeFactor + (1f - evadeFactor) * (1f - (Single)Math.Exp((p_evadeValue - p_attackValue) * evadeFactor / p_evadeValue));
			}
			return Random.Range(0f, 1f) >= num;
		}

		public virtual Boolean TestEvadeSpell(EDamageType p_damageType, Int32 p_ignoreResistance)
		{
			Int32 num = GetResistanceByType(p_damageType).Value;
			if (num > 0)
			{
				num -= p_ignoreResistance;
				if (num < 0)
				{
					num = 0;
				}
			}
			Single num2 = num * 0.01f;
			num2 *= ConfigManager.Instance.Game.MagicEvadeFactor;
			return Random.Range(0f, 1f) >= 1f - num2;
		}

		public virtual Boolean TestCritical(Single p_criticalChance)
		{
			return Random.Range(0f, 1f) < p_criticalChance;
		}

		public virtual Boolean TestBlock(Single p_blockChance, ref Int32 p_blockAttempts, Boolean p_keepBlocksOnFail)
		{
			if (p_blockAttempts > 0)
			{
				Boolean flag = Random.Range(0f, 1f) < p_blockChance;
				if (!p_keepBlocksOnFail || flag)
				{
					p_blockAttempts--;
				}
				return flag;
			}
			return false;
		}

		public abstract Resistance GetResistanceByType(EDamageType p_type);
	}
}
