using System;
using Legacy.Core.Entities;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class MonsterDisappearView : BaseView
	{
		protected override void Awake()
		{
		}

		protected virtual void OnMonsterDisappeared(Object sender, EventArgs e)
		{
			if (sender == MyController)
			{
				Disappeared();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected void Disappeared()
		{
			if (MyController is Monster)
			{
				((Monster)MyController).Disappear();
			}
			Destroy(gameObject);
		}
	}
}
