using System;

namespace Legacy.Core.Combat
{
	public class DamageCollection
	{
		private DamageData[] m_Damages = new DamageData[9];

		public DamageCollection()
		{
			Clear();
		}

		public DamageData this[EDamageType type]
		{
			get => m_Damages[(Int32)type];
		    set
			{
				if (type != value.Type)
				{
					throw new InvalidOperationException();
				}
				m_Damages[(Int32)type] = value;
			}
		}

		public void Add(DamageData p_data)
		{
			m_Damages[(Int32)p_data.Type] += p_data;
		}

		public void Clear()
		{
			for (Int32 i = 0; i < m_Damages.Length; i++)
			{
				m_Damages[i] = new DamageData((EDamageType)i, 0, 0);
			}
		}

		public Int32 GetTotalMaximumDamage()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < m_Damages.Length; i++)
			{
				num += m_Damages[i].Maximum;
			}
			return num;
		}
	}
}
