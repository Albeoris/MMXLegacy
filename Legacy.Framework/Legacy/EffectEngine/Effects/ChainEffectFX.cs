using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class ChainEffectFX : FXBase
	{
		public GameObject m_ImpactPrefab;

		public String m_TargetsTag;

		private GameObject m_Instance;

		public override void Begin(Single p_lifetime, FXArgs p_args)
		{
			base.Begin(p_lifetime, p_args);
			if (m_ImpactPrefab != null)
			{
				UnityEventArgs<FXArgs> p_args2 = new UnityEventArgs<FXArgs>(this, p_args);
				Transform transform = null;
				FXTags component = p_args.Origin.GetComponent<FXTags>();
				if (component != null)
				{
					GameObject gameObject = component.FindOne("CastSpot");
					if (gameObject != null)
					{
						transform = gameObject.transform;
					}
				}
				GameObject gameObject2;
				if (transform != null)
				{
					gameObject2 = Helper.Instantiate<GameObject>(m_ImpactPrefab, transform.transform.position);
					transform.AddChildAlignOrigin(gameObject2.transform);
				}
				else
				{
					gameObject2 = Helper.Instantiate<GameObject>(m_ImpactPrefab, p_args.OriginTransform.position);
				}
				m_Instance = gameObject2;
				ChainLightning component2 = m_Instance.GetComponent<ChainLightning>();
				if (p_args.Targets != null)
				{
					foreach (GameObject gameObject3 in p_args.Targets)
					{
						if (!String.IsNullOrEmpty(m_TargetsTag))
						{
							FXTags component3 = gameObject3.GetComponent<FXTags>();
							if (component3 != null)
							{
								component2.Targets.Add(component3.FindOne(m_TargetsTag));
							}
							else
							{
								component2.Targets.Add(gameObject3);
							}
						}
						else
						{
							component2.Targets.Add(gameObject3);
						}
					}
				}
				else
				{
					component2.Targets.Add(p_args.Target);
				}
				SendBroadcastBeginEffect<UnityEventArgs<FXArgs>>(gameObject2, p_args2);
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_Instance = null;
		}
	}
}
