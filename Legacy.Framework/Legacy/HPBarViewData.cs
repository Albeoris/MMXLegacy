using System;
using UnityEngine;

namespace Legacy
{
	public class HPBarViewData : MonoBehaviour
	{
		[SerializeField]
		public UISlicedSprite HpBar;

		[SerializeField]
		public UISlicedSprite RecentDamage;

		[SerializeField]
		public UISlicedSprite Background;

		[SerializeField]
		public UISlicedSprite BarShadow;

		[SerializeField]
		public UILabel MonsterName;

		[SerializeField]
		public GameObject PositionRoot;
	}
}
