using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Preconditions
{
	public class DisarmTrapPrecondition : BasePrecondition
	{
		private String m_successText;

		private String m_failText;

		private EPotionTarget m_attribute;

		private Int32 m_requiredValue;

		private Int32 m_damage;

		public DisarmTrapPrecondition() : base(EPreconditionType.DISARM_TRAP)
		{
		}

		public String SuccessText
		{
			get => m_successText;
		    set => m_successText = value;
		}

		public String FailText
		{
			get => m_failText;
		    set => m_failText = value;
		}

		public EPotionTarget Attribute
		{
			get => m_attribute;
		    set => m_attribute = value;
		}

		public Int32 RequiredValue
		{
			get => m_requiredValue;
		    set => m_requiredValue = value;
		}

		public Int32 Damage
		{
			get => m_damage;
		    set => m_damage = value;
		}

		public override void Trigger()
		{
			if (LegacyLogic.Instance.WorldManager.Party.HasClairvoyance())
			{
				if (Decision == EPreconditionDecision.NONE)
				{
					throw new Exception("Precontion NONE makes no sense here");
				}
				if (Decision == EPreconditionDecision.TEXT_INPUT)
				{
					throw new Exception("Precontion TEXT_INPUT makes no sense here");
				}
				if (Decision == EPreconditionDecision.YES_NO)
				{
					throw new Exception("Precontion YES_NO makes no sense here");
				}
				if (Attribute != EPotionTarget.MIGHT && Attribute != EPotionTarget.MAGIC && Attribute != EPotionTarget.PERCEPTION)
				{
					throw new Exception("Wrong attribute for a test-precondition");
				}
				base.Trigger();
			}
			else
			{
				m_result = false;
				m_cancelled = false;
				base.OnResult(null, EventArgs.Empty);
			}
		}

		public override void OnResult(Object p_sender, EventArgs p_eventArgs)
		{
			m_cancelled = ((PreconditionResultWhoWillArgs)p_eventArgs).m_cancelled;
			if (!m_cancelled)
			{
				m_result = Evaluate((PreconditionResultWhoWillArgs)p_eventArgs);
			}
			base.OnResult(p_sender, p_eventArgs);
		}

		public Boolean Evaluate(PreconditionResultWhoWillArgs p_args)
		{
			Character selectedCharacter = p_args.m_selectedCharacter;
			Int32 num = 0;
			if (m_attribute == EPotionTarget.MIGHT)
			{
				num = selectedCharacter.CurrentAttributes.Might;
			}
			else if (m_attribute == EPotionTarget.MAGIC)
			{
				num = selectedCharacter.CurrentAttributes.Magic;
			}
			else if (m_attribute == EPotionTarget.PERCEPTION)
			{
				num = selectedCharacter.CurrentAttributes.Perception;
			}
			Single num2 = (Single)Math.Pow(num / (Single)m_requiredValue, 4.0);
			Party party = LegacyLogic.Instance.WorldManager.Party;
			NpcEffect npcEffect;
			if (party.HirelingHandler.HasEffect(ETargetCondition.HIRE_BONUSTRAPCHANCE, out npcEffect) && npcEffect.TargetEffect != ETargetCondition.NONE)
			{
				num2 *= 1f + npcEffect.EffectValue;
			}
			return Random.Value < num2;
		}
	}
}
