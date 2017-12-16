using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;

namespace Legacy.Core.EventManagement
{
	public class SpellEventArgs : EventArgs
	{
		private List<SpellTarget> m_SpellTargets = new List<SpellTarget>();

		public SpellEventArgs(Spell p_spell)
		{
			if (p_spell == null)
			{
				throw new ArgumentNullException("p_spell");
			}
			Spell = p_spell;
		}

		protected SpellEventArgs()
		{
		}

		public BarkEventArgs[] BarkEventArgs { get; set; }

		public BloodMagicEventArgs BloodMagicArgs { get; set; }

		public List<SpellTarget> SpellTargets => m_SpellTargets;

	    public EPartyBuffs AddedPartyBuffs { get; set; }

		public Spell Spell { get; private set; }

		public ESpellResult Result { get; set; }

		public EDamageType DamageType { get; set; }

		public IEnumerable<T> SpellTargetsOfType<T>() where T : SpellTarget
		{
			foreach (SpellTarget target in SpellTargets)
			{
				if (target is T)
				{
					yield return (T)target;
				}
			}
			yield break;
		}

		public Boolean IsTarget(Object obj)
		{
			for (Int32 i = 0; i < m_SpellTargets.Count; i++)
			{
				if (m_SpellTargets[i].Target == obj)
				{
					return true;
				}
			}
			return false;
		}

		public override String ToString()
		{
			String text = "SpellEventArgs:\n";
			String text2 = text;
			text = String.Concat(new Object[]
			{
				text2,
				"\tSpellTargets(",
				m_SpellTargets.Count,
				"):\n"
			});
			foreach (SpellTarget spellTarget in m_SpellTargets)
			{
				text2 = text;
				text = String.Concat(new Object[]
				{
					text2,
					"\t",
					spellTarget,
					"\n"
				});
			}
			text2 = text;
			text = String.Concat(new Object[]
			{
				text2,
				"\tAddedPartyBuffs: ",
				AddedPartyBuffs,
				"\n"
			});
			text2 = text;
			text = String.Concat(new Object[]
			{
				text2,
				"\tSpell: ",
				Spell,
				"\n"
			});
			text2 = text;
			text = String.Concat(new Object[]
			{
				text2,
				"\tResult: ",
				Result,
				"\n"
			});
			return text;
		}
	}
}
