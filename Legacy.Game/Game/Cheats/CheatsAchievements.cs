using System;
using System.Collections.Generic;
using Legacy.Core.Achievements;
using Legacy.Core.Achievements.Conditions;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.Cheats
{
	public class CheatsAchievements : MonoBehaviour
	{
		[SerializeField]
		private UIPopupList m_list;

		[SerializeField]
		private UILabel m_countLabel;

		private Achievement m_currentAchievement;

		private void OnEnable()
		{
			Initialize();
			m_list.selection = m_list.items[0];
		}

		private void Initialize()
		{
			m_list.items.Clear();
			m_list.items.Add("Select...");
			List<AchievementStaticData> list = new List<AchievementStaticData>(StaticDataHandler.GetIterator<AchievementStaticData>(EDataType.ACHIEVEMENT));
			for (Int32 i = 0; i < list.Count; i++)
			{
				m_list.items.Add(list[i].StaticID + ". " + LocaManager.GetText(list[i].NameKey));
			}
		}

		public void OnMinusButtonClick(GameObject p_sender)
		{
			if (m_currentAchievement != null)
			{
				m_currentAchievement.Count = m_currentAchievement.Count - 1;
				LegacyLogic.Instance.WorldManager.AchievementManager.ForceCheck(m_currentAchievement.MainTrigger);
				Debug.Log(m_currentAchievement.MainTrigger);
				Debug.Log(m_currentAchievement.NameKey);
				UpdateData();
			}
		}

		public void OnPlusButtonClick(GameObject p_sender)
		{
			if (m_currentAchievement != null)
			{
				if (!(m_currentAchievement.Condition is GenericCounterCondition))
				{
					m_currentAchievement.Count = m_currentAchievement.Count + 1;
				}
				LegacyLogic.Instance.WorldManager.AchievementManager.ForceCheck(m_currentAchievement.MainTrigger);
				UpdateData();
			}
		}

		private void UpdateCount()
		{
			Debug.Log("Count Update");
			if (m_currentAchievement != null)
			{
				m_countLabel.text = m_currentAchievement.Count.ToString();
			}
		}

		private void UpdateColor()
		{
			if (m_currentAchievement != null)
			{
				m_list.textLabel.color = ((!m_currentAchievement.Aquired) ? Color.white : Color.green);
				m_countLabel.color = ((GetStaticDataWithID(m_currentAchievement.StaticID).Count <= 0) ? Color.red : Color.white);
			}
		}

		private AchievementStaticData GetStaticDataWithID(Int32 p_id)
		{
			return StaticDataHandler.GetStaticData<AchievementStaticData>(EDataType.ACHIEVEMENT, p_id);
		}

		public void OnSubmit()
		{
			Debug.Log("Submitted");
			if (m_currentAchievement != null)
			{
				m_countLabel.text = m_countLabel.text.Replace("|", String.Empty);
				Int32 count = m_currentAchievement.Count;
				if (Int32.TryParse(m_countLabel.text, out count))
				{
					m_currentAchievement.Count = count;
					LegacyLogic.Instance.WorldManager.AchievementManager.ForceCheck(m_currentAchievement.MainTrigger);
					UpdateData();
				}
				else
				{
					Debug.Log("Cannot Parse: " + m_countLabel.text);
				}
			}
		}

		public void OnClickedAquire()
		{
			if (m_currentAchievement != null && !m_currentAchievement.Aquired)
			{
				m_currentAchievement.Aquired = true;
				LegacyLogic.Instance.EventManager.InvokeEvent(m_currentAchievement, EEventType.ACHIEVEMENT_AQUIRED, new AchievementEventArgs(m_currentAchievement));
				UpdateData();
			}
		}

		public void OnSelectionChange()
		{
			Int32 p_id = 0;
			if (Int32.TryParse(m_list.selection.Substring(0, m_list.selection.IndexOf('.')).ToString(), out p_id))
			{
				Achievement achievement = LegacyLogic.Instance.WorldManager.AchievementManager.GetAchievement(p_id);
				if (achievement != null)
				{
					m_currentAchievement = achievement;
					UpdateData();
				}
			}
		}

		private void UpdateData()
		{
			UpdateCount();
			UpdateColor();
		}
	}
}
