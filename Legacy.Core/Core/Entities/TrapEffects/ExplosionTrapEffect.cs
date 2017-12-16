using System;
using Legacy.Core.Combat;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Entities.TrapEffects
{
	public class ExplosionTrapEffect : DamageDealingTrapEffect
	{
		public ExplosionTrapEffect(Int32 p_staticID, InteractiveObject p_parent) : base(p_staticID, p_parent)
		{
		}

		public override void ResolveEffect(Party p_party)
		{
			Damage p_damages = Damage.Create(new DamageData(EDamageType.FIRE, m_staticData.AbsoluteValueMin, m_staticData.AbsoluteValueMax), 1f);
			Attack p_attack = new Attack(1f, 0f, p_damages);
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = p_party.GetMember(i);
				AttackResult attackResult = member.FightHandler.AttackEntity(p_attack, false, EDamageType.FIRE, false, 0, false);
				attackResults[i] = attackResult;
				member.ChangeHP(-attackResult.DamageDone);
			}
			NotifyListeners(p_party);
		}
	}
}
