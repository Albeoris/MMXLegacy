using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.UpdateLogic;
using Legacy.Game.HUD;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SpellSelectPartyTarget")]
	public class SpellSelectPartyTarget : MonoBehaviour, IIngameContext, ICharacterSelectionListener
	{
		private Boolean m_active;

		private CharacterSpell m_spell;

		private Scroll m_scroll;

		public event EventHandler OpenSpellSelectPartyTarget;

		public event EventHandler CloseSpellSelectPartyTarget;

		public CharacterSpell Spell
		{
			get => m_spell;
		    set => m_spell = value;
		}

		public Scroll Scroll
		{
			get => m_scroll;
		    set => m_scroll = value;
		}

		private void Awake()
		{
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCloseMenu));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_1, new EventHandler<HotkeyEventArgs>(SelectMember1KeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_2, new EventHandler<HotkeyEventArgs>(SelectMember2KeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_3, new EventHandler<HotkeyEventArgs>(SelectMember3KeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_4, new EventHandler<HotkeyEventArgs>(SelectMember4KeyPressed));
		}

		private void OnDestroy()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCloseMenu));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_1, new EventHandler<HotkeyEventArgs>(SelectMember1KeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_2, new EventHandler<HotkeyEventArgs>(SelectMember2KeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_3, new EventHandler<HotkeyEventArgs>(SelectMember3KeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_4, new EventHandler<HotkeyEventArgs>(SelectMember4KeyPressed));
		}

		public void Activate()
		{
			m_active = true;
			DragDropManager.Instance.CancelDragAction();
			NGUITools.SetActive(gameObject, true);
		}

		public void Deactivate()
		{
			m_active = false;
			NGUITools.SetActive(gameObject, false);
		}

		private void OnCloseMenu(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && CloseSpellSelectPartyTarget != null)
			{
				CancelSpellCast();
				CloseSpellSelectPartyTarget(this, EventArgs.Empty);
			}
		}

		public void SelectMember1KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_active && p_args.KeyDown)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(0);
			}
		}

		public void SelectMember2KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_active && p_args.KeyDown)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(1);
			}
		}

		public void SelectMember3KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_active && p_args.KeyDown)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(2);
			}
		}

		public void SelectMember4KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_active && p_args.KeyDown)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(3);
			}
		}

		private void OnCancelButtonClick()
		{
			if (m_active && CloseSpellSelectPartyTarget != null)
			{
				CancelSpellCast();
				CloseSpellSelectPartyTarget(this, EventArgs.Empty);
			}
		}

		private void CancelSpellCast()
		{
			m_spell = null;
		}

		public Boolean SelectCharacter(Int32 p_index)
		{
			if (m_active && m_spell != null)
			{
				Boolean flag = true;
				Character character = LegacyLogic.Instance.WorldManager.Party.Members[p_index];
				if (m_spell.StaticData.RemovedConditions.Length > 0)
				{
					foreach (ECondition econdition in m_spell.StaticData.RemovedConditions)
					{
						if (econdition != ECondition.NONE)
						{
							flag = false;
							if (character.ConditionHandler.HasCondition(econdition))
							{
								flag = true;
								break;
							}
						}
					}
				}
				foreach (DamageData damageData in m_spell.StaticData.Damage)
				{
					if (damageData.Type == EDamageType.HEAL && (character.HealthPoints == character.MaximumHealthPoints || character.ConditionHandler.HasCondition(ECondition.DEAD)))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					if (m_scroll == null)
					{
						LegacyLogic.Instance.CommandManager.AddCommand(new CastSpellCommand(m_spell, LegacyLogic.Instance.WorldManager.Party.Members[p_index], null));
					}
					else
					{
						LegacyLogic.Instance.CommandManager.AddCommand(new CastSpellCommand(m_spell, LegacyLogic.Instance.WorldManager.Party.Members[p_index], m_scroll));
					}
				}
				m_scroll = null;
				m_spell = null;
				CloseSpellSelectPartyTarget(this, EventArgs.Empty);
				return true;
			}
			return m_active;
		}
	}
}
