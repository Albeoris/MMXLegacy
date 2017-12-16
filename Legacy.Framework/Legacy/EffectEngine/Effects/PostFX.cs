using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class PostFX : FXBase
	{
		public String m_postFXScriptName;

		public Boolean m_SpawnAndForget;

		public Boolean m_SendDestroyMessage;

		public Boolean m_IsFinishedWhenAllSpawnersGone;

		private Component m_Instance;

		public override Boolean IsFinished
		{
			get
			{
				if (m_IsFinishedWhenAllSpawnersGone)
				{
					return m_Instance == null;
				}
				return base.IsFinished;
			}
		}

		public override void Begin(Single p_lifetime, FXArgs p_args)
		{
			base.Begin(p_lifetime, p_args);
			m_Instance = Camera.main.gameObject.AddComponent(m_postFXScriptName);
		}

		public override void Destroy()
		{
			if (m_Instance == null)
			{
				return;
			}
			if (m_SendDestroyMessage)
			{
				m_Instance.gameObject.SendMessage("Destroy", SendMessageOptions.RequireReceiver);
			}
			if (!m_SpawnAndForget)
			{
				UnityEngine.Object.Destroy(m_Instance);
				m_Instance = null;
			}
		}
	}
}
