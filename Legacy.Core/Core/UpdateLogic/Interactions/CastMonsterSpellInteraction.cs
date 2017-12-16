using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class CastMonsterSpellInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 2;

		public const Int32 DEFAULT_SPELL_ID = 0;

		public const Single DEFAULT_MAGIC_FACTOR = 1f;

		protected InteractiveObject m_parent;

		protected InteractiveObject m_interactiveObject;

		private Monster m_dummyMonster;

		private MonsterCombatHandler m_dummyMonsterHandler;

		private MonsterSpell m_spell;

		private Int32 m_spellID;

		private Single m_magicFactor;

		public CastMonsterSpellInteraction()
		{
			m_spellID = 0;
			m_magicFactor = 1f;
		}

		public CastMonsterSpellInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
			m_interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			CreateDummyMonster();
			m_spell = SpellFactory.CreateMonsterSpell((EMonsterSpell)m_spellID, String.Empty, 1);
			m_spell.MonsterMagicPower = m_magicFactor;
			m_dummyMonsterHandler.CastedSpell = m_spell;
		}

		public Int32 SpellID => m_spellID;

	    public Single MagicFactor => m_magicFactor;

	    private void CreateDummyMonster()
		{
			m_dummyMonster = new Monster(10000, 0);
			m_dummyMonsterHandler = m_dummyMonster.CombatHandler;
			m_dummyMonsterHandler.CanCastSpell = true;
			m_dummyMonsterHandler.CastSpell = true;
			m_dummyMonsterHandler.CanMove = false;
			m_dummyMonsterHandler.IsFleeing = false;
			m_dummyMonsterHandler.RangedStrikes = 0;
		}

		protected override void DoExecute()
		{
			if (m_spell == null)
			{
				throw new Exception("Invalid spell ID " + m_spellID);
			}
			m_dummyMonsterHandler.DoAttack();
			FinishExecution();
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
