using System;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Combat
{
	public class ResistanceCollection : ISaveGameObject
	{
		private Resistance[] m_resistance = new Resistance[9];

		public ResistanceCollection()
		{
			Clear();
		}

		public Resistance this[EDamageType type] => m_resistance[(Int32)type];

	    public void Set(EDamageType p_type, Int32 p_value)
		{
			Set(new Resistance(p_type, p_value));
		}

		public void Set(Resistance p_data)
		{
			m_resistance[(Int32)p_data.Type].Value = p_data.Value;
		}

		public void Add(ResistanceCollection p_resistances)
		{
			for (Int32 i = 0; i < 9; i++)
			{
				Add(p_resistances[(EDamageType)i]);
			}
		}

		public void Add(EDamageType p_type, Int32 p_value)
		{
			Add(new Resistance(p_type, p_value));
		}

		public void Add(Resistance p_data)
		{
			m_resistance[(Int32)p_data.Type] += p_data;
		}

		public void Modify(EDamageType p_type, Single p_factor)
		{
			Resistance resistance = m_resistance[(Int32)p_type];
			resistance.Value = (Int32)Math.Round(resistance.Value * p_factor, MidpointRounding.AwayFromZero);
			m_resistance[(Int32)p_type] = resistance;
		}

		public void Clear()
		{
			for (Int32 i = 0; i < m_resistance.Length; i++)
			{
				m_resistance[i] = new Resistance((EDamageType)i, 0);
			}
		}

		public void CopyTo(ResistanceCollection target)
		{
			m_resistance.CopyTo(target.m_resistance, 0);
		}

		public void Load(SaveGameData p_data)
		{
			m_resistance = new Resistance[p_data.Get<Int32>("ResistanceCount", 0)];
			for (Int32 i = 0; i < m_resistance.Length; i++)
			{
				m_resistance[i] = default(Resistance);
				m_resistance[i].Type = p_data.Get<EDamageType>("DamageType" + i, EDamageType.PHYSICAL);
				m_resistance[i].Value = p_data.Get<Int32>("Value" + i, 1);
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("ResistanceCount", m_resistance.Length);
			for (Int32 i = 0; i < m_resistance.Length; i++)
			{
				p_data.Set<Int32>("DamageType" + i, (Int32)m_resistance[i].Type);
				p_data.Set<Int32>("Value" + i, m_resistance[i].Value);
			}
		}
	}
}
