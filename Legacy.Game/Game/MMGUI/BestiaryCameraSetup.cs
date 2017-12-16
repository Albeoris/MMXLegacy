using System;
using AssetBundles.Core;
using Legacy.Animations;
using Legacy.Core.Entities;
using Legacy.Views;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/BestiaryCameraSetup")]
	public class BestiaryCameraSetup : MonoBehaviour
	{
		private GameObject m_shownObject;

		private AnimHandler m_animHandler;

		private AnimatorControl m_animatorControl;

		private Boolean m_clicked;

		private Single m_deltaMove;

		private ESize m_monsterSize;

		[SerializeField]
		private Camera m_camera;

		[SerializeField]
		private Light m_keyLight;

		public Camera Camera => m_camera;

	    public Boolean Clicked
		{
			get => m_clicked;
	        set => m_clicked = value;
	    }

		public void InitCamera(RenderTexture p_target)
		{
			m_clicked = false;
			m_camera.targetTexture = p_target;
			m_camera.pixelRect = new Rect(0f, 0f, p_target.width, p_target.height);
			transform.position = new Vector3(transform.position.x, -2000f, transform.position.z);
		}

		public void DisplayMonsterPrefab(String p_prefabPath, ESize p_size)
		{
			m_monsterSize = p_size;
			AssetBundleManagers.Instance.Main.RequestAsset(p_prefabPath, 0, new AssetRequestCallback(OnMonsterPrefabLoaded), null);
		}

		private void OnMonsterPrefabLoaded(AssetRequest p_args)
		{
			GameObject gameObject = (GameObject)p_args.Asset;
			if (gameObject == null)
			{
				Debug.LogError("Error load asset");
				return;
			}
			gameObject = Helper.Instantiate<GameObject>(gameObject);
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
			LevelEntityMetadata componentInChildren = gameObject.GetComponentInChildren<LevelEntityMetadata>();
			if (componentInChildren != null)
			{
				m_keyLight.color = componentInChildren.KeyLightColor;
			}
			else
			{
				m_keyLight.color = new Color(0.768627465f, 0.6431373f, 0.6431373f, 1f);
			}
			GameObject gameObject2 = null;
			foreach (Transform transform in componentsInChildren)
			{
				if (String.Equals(transform.name, "model", StringComparison.InvariantCultureIgnoreCase))
				{
					gameObject2 = Helper.Instantiate<GameObject>(transform.gameObject);
					gameObject2.transform.parent = this.transform;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.layer = 8;
				}
			}
			AnimatorControl component = gameObject2.GetComponent<AnimatorControl>();
			if (component == null)
			{
				gameObject2.transform.localScale += gameObject2.transform.localScale + gameObject2.transform.localScale;
			}
			else
			{
				component.TargetRotation = gameObject2.transform.rotation;
				component.TargetPosition = gameObject2.transform.position;
			}
			if (m_monsterSize == ESize.BIG)
			{
				Vector3 localScale = gameObject2.transform.localScale;
				gameObject2.transform.localScale = new Vector3(localScale.x * 0.7f, localScale.y * 0.7f, localScale.z * 0.7f);
			}
			Helper.DestroyImmediate<GameObject>(gameObject.gameObject);
			gameObject = gameObject2;
			foreach (Transform transform2 in gameObject.GetComponentsInChildren<Transform>())
			{
				transform2.gameObject.layer = 8;
			}
			Reset();
			m_shownObject = gameObject;
			if (component != null)
			{
				m_animHandler = null;
				m_animatorControl = component;
			}
			else
			{
				m_animatorControl = null;
				m_animHandler = m_shownObject.GetComponentInChildren<AnimHandler>();
			}
		}

		public void HandleDragging(Vector2 p_delta)
		{
			m_deltaMove = p_delta.x;
		}

		public void PlayRandomAnimation()
		{
			if (m_shownObject != null)
			{
				if (m_animHandler != null)
				{
					if (!m_animHandler.IsPlaying(EAnimType.IDLE))
					{
						return;
					}
					EAnimType eanimType = (EAnimType)Random.Range(0, m_animHandler.Config.m_AnimationMap.Length);
					if (!String.IsNullOrEmpty(m_animHandler.Config.ClipName(eanimType)))
					{
						if (eanimType == EAnimType.DIE || eanimType == EAnimType.IDLE)
						{
							eanimType = EAnimType.DEFEND;
						}
						m_animHandler.Play(eanimType);
					}
				}
				if (m_animatorControl != null)
				{
					switch (Random.Range(0, 5))
					{
					case 0:
						m_animatorControl.Attack();
						break;
					case 1:
						m_animatorControl.AttackCritical();
						break;
					case 2:
						m_animatorControl.AttackRange();
						break;
					case 3:
						m_animatorControl.Evade();
						break;
					case 4:
						m_animatorControl.Hit();
						break;
					case 5:
						m_animatorControl.Block();
						break;
					}
				}
			}
		}

		public void Reset()
		{
			Helper.Destroy<AnimHandler>(ref m_animHandler);
			Helper.Destroy<AnimatorControl>(ref m_animatorControl);
			Helper.Destroy<GameObject>(ref m_shownObject);
		}

		private void Update()
		{
			if (m_shownObject != null && m_clicked && m_deltaMove != 0f)
			{
				m_shownObject.transform.Rotate(Vector3.up, -3f * m_deltaMove * Time.deltaTime);
				m_deltaMove = 0f;
				if (m_animatorControl != null)
				{
					m_animatorControl.TargetRotation = m_shownObject.transform.rotation;
				}
			}
		}
	}
}
