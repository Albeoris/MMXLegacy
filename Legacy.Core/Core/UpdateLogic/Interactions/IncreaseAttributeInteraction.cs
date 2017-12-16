using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class IncreaseAttributeInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 4;

		public const EPotionTarget DEFAULT_ATTRIBUTE = EPotionTarget.NONE;

		public const Int32 DEFAULT_VALUE = 0;

		public const Boolean DEFAULT_SINGLE = false;

		public const Boolean DEFAULT_PERMANENT = false;

		protected InteractiveObject m_interactiveObject;

		protected InteractiveObject m_parent;

		private EPotionTarget m_targetAttribute;

		private Int32 m_addValue;

		private Boolean m_isSingle;

		private Boolean m_isPermanent;

		public IncreaseAttributeInteraction()
		{
			m_targetAttribute = EPotionTarget.NONE;
			m_addValue = 0;
			m_isSingle = false;
			m_isPermanent = false;
		}

		public IncreaseAttributeInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		public EPotionTarget TargetAttribute => m_targetAttribute;

	    public Int32 AddValue => m_addValue;

	    public Boolean IsSingle => m_isSingle;

	    public Boolean IsPermanent => m_isPermanent;

	    protected override void DoExecute()
		{
			if (m_interactiveObject == null)
			{
				throw new NullReferenceException("Data could not be set because the interactive object is null! (" + m_parentID + ")");
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (m_isSingle)
			{
				Character member = party.GetMember(party.CurrentCharacter);
				IncreaseAttribute(member);
			}
			else
			{
				for (Int32 i = 0; i < 4; i++)
				{
					Character member2 = party.GetMember(i);
					IncreaseAttribute(member2);
					member2.CalculateCurrentAttributes();
				}
			}
			FinishExecution();
		}

		private void IncreaseAttribute(Character p_character)
		{
			Attributes attributes = (!m_isPermanent) ? p_character.CurrentAttributes : p_character.BaseAttributes;
			switch (m_targetAttribute)
			{
			case EPotionTarget.HP:
				attributes.HealthPoints += m_addValue;
				break;
			case EPotionTarget.MANA:
				attributes.ManaPoints += m_addValue;
				break;
			case EPotionTarget.MIGHT:
				attributes.Might += m_addValue;
				break;
			case EPotionTarget.MAGIC:
				attributes.Magic += m_addValue;
				break;
			case EPotionTarget.PERCEPTION:
				attributes.Perception += m_addValue;
				break;
			case EPotionTarget.DESTINY:
				attributes.Destiny += m_addValue;
				break;
			}
			if (m_isPermanent)
			{
				p_character.BaseAttributes = attributes;
			}
			else
			{
				p_character.CurrentAttributes = attributes;
			}
		}

		internal void ParseExtraEditor(String p_extra)
		{
			ParseExtra(p_extra);
		}

		protected override void ParseExtra(String p_extra)
		{
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (array.Length != 4)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					4
				}));
			}
			if (!Enum.IsDefined(typeof(EPotionTarget), array[0]))
			{
				throw new FormatException("First parameter " + array[0] + " was not an EPotionTarget entry!");
			}
			m_targetAttribute = (EPotionTarget)Enum.Parse(typeof(EPotionTarget), array[0]);
			if (!Int32.TryParse(array[1], out m_addValue))
			{
				throw new FormatException("Second parameter " + array[1] + " was not an int value!");
			}
			if (!Boolean.TryParse(array[2], out m_isSingle))
			{
				throw new FormatException("Third parameter " + array[2] + " was not a single flag!");
			}
			if (!Boolean.TryParse(array[3], out m_isPermanent))
			{
				throw new FormatException("Fourth parameter " + array[3] + " was not a permanent flag!");
			}
		}

		public override void FinishExecution()
		{
			if (m_parent != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_parent.DecreaseActivate(m_commandIndex);
			}
			base.FinishExecution();
		}
	}
}
