using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class ProjectileImpact : FXBase
	{
		public GameObject m_Prefab;

		public Boolean m_ProjectileDir;

		public Vector3 m_UpAxis = Vector3.up;

		public Vector3 m_Scale = Vector3.one;

		public Boolean m_SpawnAndForget;

		public Boolean m_AttachToTransform;

		private GameObject m_Instance;

		public override void Begin(Single p_lifetime, FXArgs p_args)
		{
			base.Begin(p_lifetime, p_args);
			if (m_Prefab == null)
			{
				Debug.LogError("Prefab not defined!");
				ForceFinish();
				return;
			}
			Quaternion p_rotation;
			if (m_ProjectileDir)
			{
				Vector3 forward = p_args.BeginPointTransform.position - p_args.EndPointTransform.position;
				forward.Normalize();
				p_rotation = Quaternion.LookRotation(forward, m_UpAxis);
			}
			else
			{
				Vector3 forward2 = Vector3.Reflect(Vector3.forward, m_UpAxis);
				p_rotation = Quaternion.LookRotation(forward2, m_UpAxis);
			}
			GameObject gameObject = Helper.Instantiate<GameObject>(m_Prefab, p_args.EndPointTransform.position, p_rotation);
			gameObject.transform.localScale = m_Scale;
			if (m_AttachToTransform)
			{
				gameObject.transform.parent = p_args.EndPointTransform;
			}
			if (!m_SpawnAndForget)
			{
				m_Instance = gameObject;
			}
			SendBroadcastBeginEffect<UnityEventArgs<FXArgs>>(gameObject, new UnityEventArgs<FXArgs>(this, p_args));
		}

		public override void Destroy()
		{
			base.Destroy();
			if (m_Instance != null)
			{
				SendBroadcastEndEffect<UnityEventArgs<FXArgs>>(m_Instance, new UnityEventArgs<FXArgs>(this, EffectArguments));
				Helper.Destroy<GameObject>(ref m_Instance);
			}
		}
	}
}
