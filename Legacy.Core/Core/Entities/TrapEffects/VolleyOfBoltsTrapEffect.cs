using System;
using Legacy.Core.Combat;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Entities.TrapEffects
{
	public class VolleyOfBoltsTrapEffect : DamageDealingTrapEffect
	{
		public VolleyOfBoltsTrapEffect(Int32 p_staticID, InteractiveObject p_parent) : base(p_staticID, p_parent)
		{
		}

		public override void ResolveEffect(Party p_party)
		{
			Int32 num = Random.Range(1, 3);
			Attack attack = new Attack(1f, 0f);
			for (Int32 i = 0; i < num; i++)
			{
				Damage item = Damage.Create(new DamageData(EDamageType.PHYSICAL, m_staticData.AbsoluteValueMin, m_staticData.AbsoluteValueMax), 1f);
				attack.Damages.Add(item);
			}
			for (Int32 j = 0; j < 4; j++)
			{
				Character member = p_party.GetMember(j);
				AttackResult attackResult = member.FightHandler.AttackEntity(attack, false, EDamageType.DARK, true, 0, false);
				member.ChangeHP(-attackResult.DamageDone);
				attackResults[j] = attackResult;
			}
			NotifyListeners(p_party);
		}
	}
}
