using System;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.Views;
using UnityEngine;

namespace Legacy.Game.MMGUI.WorldMap
{
	public class MapController : MonoBehaviour
	{
		[SerializeField]
		private TabController m_tabController;

		public Boolean IsOpen => gameObject.activeSelf;

	    public void ToggleWorldMap()
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
				MonsterHPBarView.ShowHpBar = false;
				m_tabController.OnTabClicked(0, true);
				AudioController.Play("ButtonClickBookPageUp");
			}
			else if (m_tabController.CurrentTabIndex == 0)
			{
				Close();
			}
			else
			{
				m_tabController.OnTabClicked(0, false);
			}
		}

		public void ToggleAreaMap()
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
				MonsterHPBarView.ShowHpBar = false;
				m_tabController.OnTabClicked(1, true);
				AudioController.Play("ButtonClickBookPageUp");
			}
			else if (m_tabController.CurrentTabIndex == 1)
			{
				Close();
			}
			else
			{
				m_tabController.OnTabClicked(1, false);
			}
		}

		public void Close()
		{
			MonsterHPBarView.ShowHpBar = true;
			if (gameObject.activeSelf)
			{
				m_tabController.OnTabClicked(0, false);
				gameObject.SetActive(false);
				IngameController.Instance.UpdateLogs();
				AudioController.Play("ButtonClickBookPageDown");
				TooltipManager.Instance.Hide(this);
			}
		}

		private void Start()
		{
			gameObject.SetActive(false);
		}
	}
}
