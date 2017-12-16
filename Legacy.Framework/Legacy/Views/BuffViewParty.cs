using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Views
{
	public class BuffViewParty : BuffViewBase
	{
		protected override EEventType BuffAddEventType => EEventType.PARTY_BUFF_ADDED;

	    protected override EEventType BuffRemoveEventType => EEventType.PARTY_BUFF_REMOVED;

	    protected override Boolean IsBuffAddEventForMe(Object p_sender, EventArgs p_args)
		{
			return true;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (MyController != null)
			{
				UpdateBuffs();
			}
		}

		protected override void OnGameLoaded(Object sender, EventArgs e)
		{
			UpdateBuffs();
		}

		private void UpdateBuffs()
		{
			Party p_party = (Party)MyController;
			if (UpdateBuff(p_party, EPartyBuffs.LIGHT_ORB, ECharacterSpell.SPELL_LIGHT_ORB))
			{
				return;
			}
			if (UpdateBuff(p_party, EPartyBuffs.TORCHLIGHT, ECharacterSpell.SPELL_FIRE_TORCHLIGHT))
			{
				return;
			}
			if (UpdateBuff(p_party, EPartyBuffs.DARK_VISION, ECharacterSpell.SPELL_DARK_VISION))
			{
				return;
			}
			if (UpdateBuff(p_party, EPartyBuffs.CELESTIAL_ARMOR, ECharacterSpell.SPELL_LIGHT_CELESTIAL_ARMOR))
			{
				return;
			}
		}

		private Boolean UpdateBuff(Party p_party, EPartyBuffs p_buff, ECharacterSpell p_castSpellToSet)
		{
			if (p_party.Buffs.HasBuff(p_buff))
			{
				m_BuffAppliedBySpellMap.Add(p_buff.ToString(), p_castSpellToSet.ToString());
				OnBuffEvent(p_party.Buffs.GetBuff(p_buff), null);
				return true;
			}
			return false;
		}

		protected override Boolean IsApplySpellEventForMe(Object p_sender, EventArgs p_args, out String out_buffNameKey, out String out_spellNameKey)
		{
			out_buffNameKey = null;
			out_spellNameKey = null;
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			if (spellEventArgs.AddedPartyBuffs != EPartyBuffs.NONE)
			{
				out_buffNameKey = spellEventArgs.AddedPartyBuffs.ToString();
				out_spellNameKey = GetSpellNameKey(spellEventArgs.Spell);
				return true;
			}
			return false;
		}

		protected override void GetBuffBaseFXPath(Object p_sender, EventArgs p_args, out String out_buffBaseFXPath, out String out_buffNameKey)
		{
			PartyBuff partyBuff = (PartyBuff)p_sender;
			out_buffBaseFXPath = partyBuff.StaticData.Gfx;
			out_buffNameKey = partyBuff.Type.ToString();
		}

		protected override String GetBuffDebugName(Object p_sender, EventArgs p_args)
		{
			return ((PartyBuff)p_sender).StaticData.Name;
		}
	}
}
