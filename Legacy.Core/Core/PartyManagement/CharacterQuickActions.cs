using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using Legacy.Utilities;

namespace Legacy.Core.PartyManagement
{
	public class CharacterQuickActions : ISaveGameObject
	{
		private Action[] m_actions = new Action[10];

		public CharacterQuickActions()
		{
			InitEmpty();
		}

		public Action this[Int32 p_index]
		{
			get => m_actions[p_index];
		    set => m_actions[p_index] = value;
		}

		private void InitEmpty()
		{
			for (Int32 i = 0; i < m_actions.Length; i++)
			{
				m_actions[i] = new Action(EQuickActionType.NONE, null, ECharacterSpell.SPELL_FIRE_WARD);
			}
		}

		public void Load(SaveGameData p_data)
		{
			for (Int32 i = 0; i < m_actions.Length; i++)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("QuickAction" + i, null);
				if (saveGameData != null)
				{
					m_actions[i].Load(saveGameData);
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			for (Int32 i = 0; i < m_actions.Length; i++)
			{
				SaveGameData saveGameData = new SaveGameData("QuickAction" + i);
				m_actions[i].Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			}
		}

		public struct Action : ISaveGameObject
		{
			private EQuickActionType m_type;

			private Consumable m_item;

			private ECharacterSpell m_spell;

			public Action(EQuickActionType p_type, Consumable p_item, ECharacterSpell p_spell)
			{
				m_type = p_type;
				m_item = p_item;
				m_spell = p_spell;
			}

			public EQuickActionType Type => m_type;

		    public Consumable Item => m_item;

		    public ECharacterSpell Spell => m_spell;

		    public void Load(SaveGameData p_data)
			{
				m_type = (EQuickActionType)p_data.Get<Int32>("Type", 0);
				Int32 num = (Int32)p_data.Get<EDataType>("ItemType", EDataType.NONE);
				if (num != 0)
				{
					SaveGameData saveGameData = p_data.Get<SaveGameData>("Item", null);
					if (saveGameData != null)
					{
						try
						{
							m_item = (Consumable)ItemFactory.CreateItem((EDataType)num);
							m_item.Load(saveGameData);
						}
						catch (Exception ex)
						{
							LegacyLogger.Log(ex.ToString());
						}
					}
				}
				m_spell = (ECharacterSpell)p_data.Get<Int32>("Spell", 1);
			}

			public void Save(SaveGameData p_data)
			{
				p_data.Set<Int32>("Type", (Int32)m_type);
				if (m_item == null)
				{
					p_data.Set<Int32>("ItemType", 0);
				}
				else
				{
					EDataType itemType = m_item.GetItemType();
					p_data.Set<Int32>("ItemType", (Int32)itemType);
					SaveGameData saveGameData = new SaveGameData("Item");
					m_item.Save(saveGameData);
					p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
				}
				p_data.Set<Int32>("Spell", (Int32)m_spell);
			}

			public override String ToString()
			{
				return String.Format("[Action: Type={0}, Spell={1}]", Type, Spell);
			}
		}
	}
}
