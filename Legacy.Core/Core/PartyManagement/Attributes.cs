using System;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.PartyManagement
{
	public struct Attributes : ISaveGameObject
	{
		public static readonly Attributes Empty = new Attributes(0, 0, 0, 0, 0, 0, 0, 0);

		public Int32 Might;

		public Int32 Magic;

		public Int32 Perception;

		public Int32 Destiny;

		public Int32 Vitality;

		public Int32 Spirit;

		public Int32 HealthPoints;

		public Int32 ManaPoints;

		public Attributes(Int32 p_might, Int32 p_magic, Int32 p_perception, Int32 p_destiny, Int32 p_vitality, Int32 p_spirit, Int32 p_hp, Int32 p_mp)
		{
			Might = p_might;
			Magic = p_magic;
			Perception = p_perception;
			Destiny = p_destiny;
			Vitality = p_vitality;
			Spirit = p_spirit;
			HealthPoints = p_hp;
			ManaPoints = p_mp;
		}

		public override String ToString()
		{
			return String.Format("[Attributes: Might={0}, Magic={1}, Perception={2}, Destiny={3}, Endurance={4}, Aura={5}, HealthPoints={6}, ManaPoints={7}]", new Object[]
			{
				Might,
				Magic,
				Perception,
				Destiny,
				Vitality,
				Spirit,
				HealthPoints,
				ManaPoints
			});
		}

		public void Load(SaveGameData p_data)
		{
			Might = p_data.Get<Int32>("Might", 1);
			Magic = p_data.Get<Int32>("Magic", 1);
			Perception = p_data.Get<Int32>("Perception", 1);
			Destiny = p_data.Get<Int32>("Destiny", 1);
			Vitality = p_data.Get<Int32>("Vitality", 1);
			Spirit = p_data.Get<Int32>("Spirit", 1);
			HealthPoints = p_data.Get<Int32>("HealthPoints", 1);
			ManaPoints = p_data.Get<Int32>("ManaPoints", 1);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("Might", Might);
			p_data.Set<Int32>("Magic", Magic);
			p_data.Set<Int32>("Perception", Perception);
			p_data.Set<Int32>("Destiny", Destiny);
			p_data.Set<Int32>("Vitality", Vitality);
			p_data.Set<Int32>("Spirit", Spirit);
			p_data.Set<Int32>("HealthPoints", HealthPoints);
			p_data.Set<Int32>("ManaPoints", ManaPoints);
		}

		public static Attributes operator +(Attributes left, Attributes right)
		{
			return new Attributes
			{
				Might = left.Might + right.Might,
				Magic = left.Magic + right.Magic,
				Perception = left.Perception + right.Perception,
				Destiny = left.Destiny + right.Destiny,
				Vitality = left.Vitality + right.Vitality,
				Spirit = left.Spirit + right.Spirit,
				HealthPoints = left.HealthPoints + right.HealthPoints,
				ManaPoints = left.ManaPoints + right.ManaPoints
			};
		}
	}
}
