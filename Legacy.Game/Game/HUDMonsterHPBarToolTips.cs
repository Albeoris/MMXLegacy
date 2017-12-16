using System;
using Legacy.Core.EventManagement;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.HUD;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game
{
	public class HUDMonsterHPBarToolTips : MonoBehaviour, IMonsterTooltip
	{
		private MonsterHPBarView m_contollerMonster;

		public void OnMonsterAssigned(Object p_sender)
		{
			if (p_sender is MonsterHPBarView)
			{
				m_contollerMonster = (MonsterHPBarView)p_sender;
				MonsterHPBarView contollerMonster = m_contollerMonster;
				contollerMonster.OnTooltipEvent = (EventHandler)Delegate.Combine(contollerMonster.OnTooltipEvent, new EventHandler(OnTooltip));
			}
		}

		public void InitializeMonsterTooltip(Object p_object)
		{
			OnMonsterAssigned(p_object);
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnTooltip(Object p_sender, EventArgs p_args)
		{
			MonsterBuffView monsterBuffView = p_sender as MonsterBuffView;
			if (monsterBuffView != null)
			{
				StringEventArgs stringEventArgs = p_args as StringEventArgs;
				if (stringEventArgs != null)
				{
					TooltipManager.Instance.Show(this, ((StringEventArgs)p_args).text, monsterBuffView.collider.transform.position, monsterBuffView.collider.transform.localScale * 0.5f);
				}
				if (p_args == EventArgs.Empty)
				{
					TooltipManager.Instance.Hide(this);
				}
			}
		}

		private void OnDestroy()
		{
			if (m_contollerMonster != null)
			{
				MonsterHPBarView contollerMonster = m_contollerMonster;
				contollerMonster.OnTooltipEvent = (EventHandler)Delegate.Remove(contollerMonster.OnTooltipEvent, new EventHandler(OnTooltip));
			}
			TooltipManager.Instance.Hide(this);
		}
	}
}
