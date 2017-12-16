using System;
using Legacy.EffectEngine.Effects;
using UnityEngine;

namespace Legacy
{
	public class ProjectileDublicator : MonoBehaviour
	{
		[SerializeField]
		private const Single TARGET_ORIENTATION_LERP_TIME = 0.25f;

		[SerializeField]
		private Vector3 TARGET_LOCAL_POS = new Vector3(-0.45f, 0f, -1f);

		[SerializeField]
		private Quaternion TARGET_LOCAL_ROT = Quaternion.Euler(15f, 90f, 90f);

		[SerializeField]
		private String[] PROJECTILE_OBJECT_POSSIBLE_NAMES;

		[SerializeField]
		private Boolean IS_FORCE_DISABLE_AFTER_FX;

		[SerializeField]
		private Boolean IS_USING_ORIGINAL_POSITION;

		[SerializeField]
		private Boolean IS_USING_ORIGINAL_ROTATION;

		private Single m_EffectStartTime = -1f;

		private Transform m_Projectile;

		private Renderer m_render;

		private Boolean m_wasEnabledAtFirst;

		private void OnBeginEffect(UnityEventArgs<FXArgs> e)
		{
			if (e != null && e.EventArgs != null && e.EventArgs.Origin != null)
			{
				MeshRenderer[] componentsInChildren = e.EventArgs.Origin.GetComponentsInChildren<MeshRenderer>(true);
				m_render = null;
				for (Int32 i = 0; i < componentsInChildren.Length; i++)
				{
					foreach (String value in PROJECTILE_OBJECT_POSSIBLE_NAMES)
					{
						if (componentsInChildren[i].name.Contains(value))
						{
							m_render = componentsInChildren[i];
							i = componentsInChildren.Length;
							break;
						}
					}
				}
				if (m_render != null)
				{
					m_wasEnabledAtFirst = m_render.enabled;
					GameObject gameObject = m_render.gameObject;
					GameObject gameObject2 = Helper.Instantiate<GameObject>(gameObject);
					gameObject2.transform.parent = this.transform;
					gameObject2.transform.position = gameObject.transform.position;
					gameObject2.transform.rotation = gameObject.transform.rotation;
					gameObject2.transform.localScale = gameObject.transform.lossyScale;
					m_Projectile = gameObject2.transform;
					Transform[] array = new Transform[this.transform.childCount];
					for (Int32 k = 0; k < array.Length; k++)
					{
						array[k] = transform.GetChild(k);
					}
					foreach (Transform transform in array)
					{
						if (transform != gameObject2.transform)
						{
							transform.transform.localPosition = gameObject2.transform.localRotation * transform.transform.localPosition + gameObject2.transform.localPosition;
							transform.parent = gameObject2.transform;
						}
					}
					m_EffectStartTime = Time.time;
					gameObject2.renderer.enabled = true;
					m_render.enabled = false;
				}
				else
				{
					Debug.LogError("ProjectileDublicator: boomerang not found! " + e.EventArgs.Origin.name);
				}
			}
			else
			{
				Debug.LogError("ProjectileDublicator: event args are NULL!");
			}
		}

		private void Update()
		{
			if (m_Projectile != null)
			{
				Single num = (Time.time - m_EffectStartTime) / 0.25f;
				if (num <= 1f)
				{
					if (!IS_USING_ORIGINAL_POSITION)
					{
						m_Projectile.localPosition = Vector3.Lerp(m_Projectile.localPosition, TARGET_LOCAL_POS, num);
					}
					if (!IS_USING_ORIGINAL_ROTATION)
					{
						m_Projectile.localRotation = Quaternion.Slerp(m_Projectile.localRotation, TARGET_LOCAL_ROT, num);
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (m_render != null)
			{
				m_render.enabled = (m_wasEnabledAtFirst && !IS_FORCE_DISABLE_AFTER_FX);
			}
		}
	}
}
