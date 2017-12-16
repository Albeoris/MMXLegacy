using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class SpawnPrefab : FXBase
	{
		public ETarget m_SpawnTarget;

		public GameObject m_Prefab;

		public String m_Tag;

		public Boolean m_SpawnAndForget;

		public Boolean m_AttachToTransform;

		public Boolean m_InheritTransformRotation;

		public Boolean m_IsFinishedWhenAllSpawnersGone;

		private Boolean m_isSlotPos;

		private Vector3 targetpos;

		private List<GameObject> m_Instances;

		public override Boolean IsFinished
		{
			get
			{
				if (m_IsFinishedWhenAllSpawnersGone)
				{
					foreach (GameObject x in m_Instances)
					{
						if (x != null)
						{
							return false;
						}
					}
					return true;
				}
				return base.IsFinished;
			}
		}

		public override void Begin(Single p_lifetime, FXArgs p_args)
		{
			base.Begin(p_lifetime, p_args);
			GameObject gameObject = p_args.Origin;
			m_isSlotPos = false;
			if (m_SpawnTarget == ETarget.TARGET)
			{
				gameObject = p_args.Target;
				m_isSlotPos = false;
			}
			else if (m_SpawnTarget == ETarget.SLOT_ORIGIN)
			{
				targetpos = p_args.SlotOriginPosition;
				m_isSlotPos = true;
			}
			else if (m_SpawnTarget == ETarget.SLOT_TARGET)
			{
				targetpos = p_args.SlotTargetPosition;
				m_isSlotPos = true;
			}
			m_Instances = new List<GameObject>();
			if (!String.IsNullOrEmpty(m_Tag))
			{
				FXTags component = gameObject.GetComponent<FXTags>();
				if (component != null)
				{
					GameObject[] array = component.Find(m_Tag);
					m_Instances.Capacity = array.Length;
					m_Instances.AddRange(array);
				}
				else
				{
					m_Instances.Add(gameObject);
				}
			}
			else
			{
				m_Instances.Add(gameObject);
			}
			Quaternion quaternion = Quaternion.LookRotation(p_args.SlotForward);
			UnityEventArgs<FXArgs> p_args2 = new UnityEventArgs<FXArgs>(this, p_args);
			for (Int32 i = 0; i < m_Instances.Count; i++)
			{
				if (!(m_Instances[i] == null))
				{
					Transform transform = m_Instances[i].transform;
					GameObject gameObject2;
					if (!m_isSlotPos)
					{
						gameObject2 = Helper.Instantiate<GameObject>(m_Prefab, transform.position, (!m_InheritTransformRotation) ? quaternion : transform.rotation);
					}
					else
					{
						gameObject2 = Helper.Instantiate<GameObject>(m_Prefab, targetpos, quaternion);
					}
					if (m_AttachToTransform)
					{
						transform.AddChild(gameObject2.transform);
					}
					SendBroadcastBeginEffect<UnityEventArgs<FXArgs>>(gameObject2, p_args2);
					m_Instances[i] = gameObject2;
				}
			}
		}

		public override void Destroy()
		{
			if (m_Instances == null)
			{
				return;
			}
			UnityEventArgs<FXArgs> p_args = new UnityEventArgs<FXArgs>(this, EffectArguments);
			foreach (GameObject gameObject in m_Instances)
			{
				if (gameObject != null)
				{
					SendBroadcastEndEffect<UnityEventArgs<FXArgs>>(gameObject, p_args);
				}
			}
			if (!m_SpawnAndForget)
			{
				Helper.DestroyList<GameObject>(m_Instances);
			}
			m_Instances.Clear();
		}

		public enum ETarget
		{
			ORIGIN,
			TARGET,
			SLOT_ORIGIN,
			SLOT_TARGET
		}
	}
}
