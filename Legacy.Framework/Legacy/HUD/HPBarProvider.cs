using System;
using System.Collections.Generic;
using Legacy.EffectEngine;
using Legacy.MMGUI;
using Legacy.Views;
using UnityEngine;

namespace Legacy.HUD
{
	[AddComponentMenu("MM Legacy/MMGUI/HPBarProvider")]
	public class HPBarProvider : MonoBehaviour
	{
		private static HPBarProvider s_Instance;

		private static Dictionary<MonsterHPBarView, GameObject> s_HPBarList = new Dictionary<MonsterHPBarView, GameObject>();

		private static List<MonsterHPBarView> s_positionedList = new List<MonsterHPBarView>();

		[SerializeField]
		private GameObject m_hpBarPrefab;

		public static void CreateHPBar(MonsterHPBarView owner, Transform anchor, out GameObject root, out GameObject positionRoot, out UISlicedSprite hpBar, out UISlicedSprite recentDamage, out UISlicedSprite background, out UISlicedSprite barShadow, out UILabel monsterName)
		{
			root = null;
			positionRoot = null;
			hpBar = null;
			monsterName = null;
			background = null;
			barShadow = null;
			recentDamage = null;
			if (s_Instance == null || owner == null || anchor == null || s_HPBarList.ContainsKey(owner))
			{
				return;
			}
			GameObject gameObject = NGUITools.AddChild(s_Instance.gameObject, s_Instance.m_hpBarPrefab);
			UIFollowTarget uifollowTarget = gameObject.AddComponent<UIFollowTarget>();
			uifollowTarget.target = anchor;
			uifollowTarget.GameCamera = FXMainCamera.Instance.DefaultCamera.camera;
			uifollowTarget.UICamera = GUIMainCamera.Instance.camera;
			HPBarViewData component = gameObject.GetComponent<HPBarViewData>();
			root = gameObject;
			monsterName = component.MonsterName;
			hpBar = component.HpBar;
			background = component.Background;
			barShadow = component.BarShadow;
			recentDamage = component.RecentDamage;
			positionRoot = component.PositionRoot;
			s_HPBarList.Add(owner, gameObject);
		}

		public static void DestroyHPBar(MonsterHPBarView owner)
		{
			if (s_Instance == null || owner == null || !s_HPBarList.ContainsKey(owner))
			{
				return;
			}
			GameObject obj;
			if (s_HPBarList.TryGetValue(owner, out obj))
			{
				Destroy(obj);
				s_HPBarList.Remove(owner);
			}
			if (s_positionedList.Contains(owner))
			{
				s_positionedList.Remove(owner);
			}
		}

		public static Boolean CheckForCollision(MonsterHPBarView p_barToCheck)
		{
			foreach (MonsterHPBarView monsterHPBarView in s_positionedList)
			{
				if (p_barToCheck != monsterHPBarView && p_barToCheck.CheckCollision(monsterHPBarView))
				{
					return true;
				}
			}
			return false;
		}

		public static void AddForCollision(MonsterHPBarView p_barToAdd)
		{
			s_positionedList.Add(p_barToAdd);
		}

		private void Start()
		{
		}

		private void Awake()
		{
			if (s_Instance != null)
			{
				throw new Exception("HUDTextProvider\nInstance already set! by -> " + s_Instance);
			}
			s_Instance = this;
			if (m_hpBarPrefab == null)
			{
				throw new UnityException("HP bar prefab not defined!");
			}
		}

		private void Update()
		{
			s_positionedList.Clear();
		}
	}
}
