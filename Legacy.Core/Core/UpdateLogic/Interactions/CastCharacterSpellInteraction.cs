using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class CastCharacterSpellInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 2;

		public const Int32 DEFAULT_SPELL_ID = 0;

		public const Single DEFAULT_MAGIC_FACTOR = 1f;

		protected InteractiveObject m_interactiveObject;

		protected InteractiveObject m_parent;

		private Int32 m_spellID;

		private Single m_magicFactor;

		public CastCharacterSpellInteraction()
		{
			m_spellID = 0;
			m_magicFactor = 1f;
		}

		public CastCharacterSpellInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			CharacterSpell characterSpell = SpellFactory.CreateCharacterSpell((ECharacterSpell)m_spellID);
			if (characterSpell != null)
			{
				characterSpell.CastSpellByInteractiveObject(m_magicFactor);
				FinishExecution();
				return;
			}
			throw new Exception("Invalid spell ID " + m_spellID);
		}

		protected override void ParseExtra(String p_extra)
		{
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (array.Length != 2)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					2
				}));
			}
			if (!Int32.TryParse(array[0], out m_spellID))
			{
				throw new FormatException("First parameter " + array[1] + " was not a spell ID number!");
			}
			if (!Single.TryParse(array[1], out m_magicFactor))
			{
				throw new FormatException("Second parameter " + array[2] + " was not a factor!");
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
