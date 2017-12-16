using System;
using Legacy.Utilities;

namespace Legacy.Core.NpcInteraction
{
	public struct NpcEffect
	{
		private ETargetCondition m_targetEffect;

		private EEffectPeriodicity m_effectPeriodicity;

		private Single m_effectValue;

		private Int32 m_effectPrice;

		public NpcEffect(ETargetCondition p_targetEffect, EEffectPeriodicity p_effectType, Single p_effectValue, Int32 p_effectPrice)
		{
			m_targetEffect = p_targetEffect;
			m_effectPeriodicity = p_effectType;
			m_effectValue = p_effectValue;
			m_effectPrice = p_effectPrice;
		}

		public ETargetCondition TargetEffect => m_targetEffect;

	    public EEffectPeriodicity EffectPeriodicity
		{
			get
			{
				if (m_targetEffect == ETargetCondition.NONE)
				{
					LegacyLogger.Log("Properties of empty NpcEffect should not be accessed");
				}
				return m_effectPeriodicity;
			}
		}

		public Single EffectValue
		{
			get
			{
				if (m_targetEffect == ETargetCondition.NONE)
				{
					LegacyLogger.Log("Properties of empty NpcEffect should not be accessed");
				}
				return m_effectValue;
			}
		}

		public Int32 EffectPrice
		{
			get
			{
				if (m_targetEffect == ETargetCondition.NONE)
				{
					LegacyLogger.Log("Properties of empty NpcEffect should not be accessed");
				}
				return m_effectPrice;
			}
		}

		public override String ToString()
		{
			return String.Format("[NpcEffect TargetEffect:{0} EffectType:{1} EffectValue:{2} EffectPrice:{3}]", new Object[]
			{
				Enum.GetName(m_targetEffect.GetType(), m_targetEffect),
				Enum.GetName(m_effectPeriodicity.GetType(), m_effectPeriodicity),
				m_effectValue,
				m_effectPrice
			});
		}
	}
}
