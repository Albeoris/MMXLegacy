using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Preconditions
{
	public class TestPrecondition : BasePrecondition
	{
		private String m_successText;

		private String m_failText;

		private EPotionTarget m_attribute;

		private Int32 m_requiredValue;

		public TestPrecondition() : base(EPreconditionType.TEST)
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

		public override void Trigger()
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
			if (Attribute != EPotionTarget.MIGHT && Attribute != EPotionTarget.MAGIC && Attribute != EPotionTarget.PERCEPTION && Attribute != EPotionTarget.DESTINY)
			{
				throw new Exception("Wrong attribute for a test-precondition");
			}
			base.Trigger();
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
			if (m_attribute == EPotionTarget.MIGHT)
			{
				if (selectedCharacter.CurrentAttributes.Might >= m_requiredValue)
				{
					selectedCharacter.BarkHandler.TriggerBark(EBarks.SUCCESS);
				}
				return selectedCharacter.CurrentAttributes.Might >= m_requiredValue;
			}
			if (m_attribute == EPotionTarget.MAGIC)
			{
				if (selectedCharacter.CurrentAttributes.Magic >= m_requiredValue)
				{
					selectedCharacter.BarkHandler.TriggerBark(EBarks.SUCCESS);
				}
				return selectedCharacter.CurrentAttributes.Magic >= m_requiredValue;
			}
			if (m_attribute == EPotionTarget.PERCEPTION)
			{
				if (selectedCharacter.CurrentAttributes.Perception >= m_requiredValue)
				{
					selectedCharacter.BarkHandler.TriggerBark(EBarks.SUCCESS);
				}
				return selectedCharacter.CurrentAttributes.Perception >= m_requiredValue;
			}
			if (m_attribute == EPotionTarget.DESTINY)
			{
				if (selectedCharacter.CurrentAttributes.Destiny >= m_requiredValue)
				{
					selectedCharacter.BarkHandler.TriggerBark(EBarks.SUCCESS);
				}
				return selectedCharacter.CurrentAttributes.Destiny >= m_requiredValue;
			}
			selectedCharacter.BarkHandler.TriggerBark(EBarks.FAIL);
			return false;
		}
	}
}
