using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Hints
{
	public class HintManager
	{
		private List<Hint> m_hints;

		private Boolean m_active;

		public HintManager()
		{
			m_hints = new List<Hint>();
			m_active = true;
		}

		public Boolean IsActive => m_active;

	    public void Initialize()
		{
			m_hints.Clear();
			foreach (HintStaticData sd in StaticDataHandler.GetIterator<HintStaticData>(EDataType.HINTS))
			{
				Hint item = new Hint(sd);
				m_hints.Add(item);
			}
			m_active = ConfigManager.Instance.Options.ShowHints;
		}

		public void SetActive(Boolean p_value)
		{
			if (m_active != p_value)
			{
				m_active = p_value;
				if (p_value != ConfigManager.Instance.Options.ShowHints)
				{
					ConfigManager.Instance.Options.ShowHints = p_value;
					ConfigManager.Instance.WriteConfigurations();
				}
				if (m_active)
				{
					foreach (Hint hint in m_hints)
					{
						hint.Shown = false;
					}
				}
			}
		}

		public Hint GetHint(EHintType p_type)
		{
			foreach (Hint hint in m_hints)
			{
				if (hint.Type == p_type)
				{
					return hint;
				}
			}
			return null;
		}

		public Hint GetHintByID(Int32 p_id)
		{
			foreach (Hint hint in m_hints)
			{
				if (hint.StaticID == p_id)
				{
					return hint;
				}
			}
			return null;
		}

		public List<Hint> GetHintsForCategory(EHintCategory p_category)
		{
			List<Hint> list = new List<Hint>();
			foreach (Hint hint in m_hints)
			{
				if (hint.Category == p_category || p_category == EHintCategory.SHOW_ALL)
				{
					list.Add(hint);
				}
			}
			return list;
		}

		public void TriggerHint(EHintType p_type)
		{
			if (!m_active)
			{
				return;
			}
			Hint hint = GetHint(p_type);
			if (hint != null && !hint.Shown)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(hint, EEventType.SHOW_HINT, EventArgs.Empty);
			}
		}

		public void Load()
		{
			SaveGameData saveGameData = new SaveGameData("globalHints");
			LegacyLogic.Instance.WorldManager.SaveGameManager.LoadSaveGameData(saveGameData, "hints.lsg");
			for (Int32 i = 0; i < m_hints.Count; i++)
			{
				m_hints[i].Shown = saveGameData.Get<Boolean>("shown" + m_hints[i].StaticID, false);
			}
		}

		public void Save()
		{
			SaveGameData saveGameData = new SaveGameData("globalHints");
			for (Int32 i = 0; i < m_hints.Count; i++)
			{
				saveGameData.Set<Boolean>("shown" + m_hints[i].StaticID, m_hints[i].Shown);
			}
			LegacyLogic.Instance.WorldManager.SaveGameManager.SaveSaveGameData(saveGameData, "hints.lsg");
		}
	}
}
