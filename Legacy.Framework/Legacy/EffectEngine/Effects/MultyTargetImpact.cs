using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class MultyTargetImpact : FXBase
	{
		public GameObject m_ImpactPrefab;

		public Boolean m_AttachToTransform;

		public Boolean m_SpawnAndForget;

		public GameObject m_TargetsImpactPrefab;

		public String m_TargetsTag;

		public Boolean m_AttachToTargetTransforms;

		public Boolean m_SpawnAndForgetTargets;

		public Int32 m_SpawnPerTarget = 1;

		private GameObject m_Instance;

		private List<GameObject> m_TargetInstances;

		public override void Begin(Single p_lifetime, FXArgs p_args)
		{
			base.Begin(p_lifetime, p_args);
			m_SpawnPerTarget = Math.Max(m_SpawnPerTarget, 1);
			UnityEventArgs<FXArgs> p_args2 = new UnityEventArgs<FXArgs>(this, p_args);
			if (m_ImpactPrefab != null)
			{
				GameObject gameObject = Helper.Instantiate<GameObject>(m_ImpactPrefab, p_args.TargetTransform.position);
				if (m_AttachToTransform)
				{
					gameObject.transform.parent = p_args.TargetTransform;
				}
				m_Instance = gameObject;
				SendBroadcastBeginEffect<UnityEventArgs<FXArgs>>(gameObject, p_args2);
			}
			if (p_args.Targets != null && m_TargetsImpactPrefab != null)
			{
				m_TargetInstances = new List<GameObject>(p_args.Targets.Count);
				foreach (GameObject gameObject2 in p_args.Targets)
				{
					Transform transform = gameObject2.transform;
					GameObject gameObject;
					if (!String.IsNullOrEmpty(m_TargetsTag))
					{
						FXTags component = gameObject2.GetComponent<FXTags>();
						if (component != null)
						{
							GameObject[] array = component.Find(m_TargetsTag);
							if (array.Length > 1)
							{
								for (Int32 i = 0; i < m_SpawnPerTarget; i++)
								{
									transform = array.RandomElement<GameObject>().transform;
									gameObject = Helper.Instantiate<GameObject>(m_TargetsImpactPrefab, transform.position);
									if (m_AttachToTargetTransforms)
									{
										gameObject.transform.parent = transform;
									}
									m_TargetInstances.Add(gameObject);
									SendBroadcastBeginEffect<UnityEventArgs<FXArgs>>(gameObject, p_args2);
								}
								continue;
							}
							transform = array.RandomElement<GameObject>().transform;
						}
					}
					gameObject = Helper.Instantiate<GameObject>(m_TargetsImpactPrefab, transform.position);
					if (m_AttachToTargetTransforms)
					{
						gameObject.transform.parent = transform;
					}
					m_TargetInstances.Add(gameObject);
					SendBroadcastBeginEffect<UnityEventArgs<FXArgs>>(gameObject, p_args2);
				}
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			if (!m_SpawnAndForget)
			{
				Helper.Destroy<GameObject>(ref m_Instance);
			}
			m_Instance = null;
			if (!m_SpawnAndForgetTargets)
			{
				Helper.DestroyList<GameObject>(m_TargetInstances);
			}
			else if (m_TargetInstances != null)
			{
				m_TargetInstances.Clear();
			}
		}
	}
}
